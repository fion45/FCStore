#include "stdafx.h"
#include "Server.h"
#include <Winsock2.h>

SERVICE_STATUS ServiceStatus; 
SERVICE_STATUS_HANDLE hStatus; 

CZServer * CZServer :: instance   = 0;
CZServer * CZS = CZServer::Instance();

const TCHAR *szServiceName = _T("RGServer");

//****************************************************************************************************************
//**** 函数名:Instance
//**** 功  能:单例实现
//**** 参  数:
//****        none            - none
//**** 返回值:
//****        none            - none
//****************************************************************************************************************	
CZServer * CZServer::Instance()
{
	if (instance == 0)
	{
		instance = new CZServer ();
	}
	return  instance;
}

//****************************************************************************************************************
//**** 函数名:CZServer
//**** 功  能:构造函数
//**** 参  数:
//****        none            - none
//**** 返回值:
//****        none            - none
//****************************************************************************************************************	
CZServer::CZServer():runningWorkerThreads(0),hWorkerIOCP(INVALID_HANDLE_VALUE),
suicideEvent(INVALID_HANDLE_VALUE),listener(INVALID_SOCKET),
scavengerThread(INVALID_HANDLE_VALUE)
{

}

//****************************************************************************************************************
//**** 函数名:InitServer
//**** 功  能:初始化server,创建worker线程/创建sign线程,初始等
//**** 参  数:
//****        none            - none
//**** 返回值:
//****        0               - init ok
//****        -1              - init error
//****************************************************************************************************************	
INT32U CZServer::InitServer()
{
	HANDLE handle;												// temporary thread handle
	INT32U i,dummy;
	
	//============================================================================================================
	//==== 初始化common DLL
	//============================================================================================================
	::CoInitializeEx(NULL, COINIT_MULTITHREADED);

	//============================================================================================================
	//==== 从ini文件中加载参数
	//============================================================================================================
	GetModuleFileName(NULL,GSVWorkPath.GetBuffer(MAX_PATH), MAX_PATH);
	GSVWorkPath.ReleaseBuffer();
	if(	GSVWorkPath.ReverseFind('\\') >= 0)
	{
		GSVWorkPath = GSVWorkPath.Left(GSVWorkPath.ReverseFind('\\')+1);
	}
	SysIni = GSVWorkPath + TEXT("system.ini");
	WorkerPort        = GetPrivateProfileInt(TEXT("COMMON"),TEXT("WorkerPort"),9523,SysIni);
	MaxWorkerThreads  = GetPrivateProfileInt(TEXT("COMMON"),TEXT("MaxWorkerThreads"),16,SysIni);
	WorkerConcurrency = GetPrivateProfileInt(TEXT("COMMON"),TEXT("WorkerConcurrency"),0,SysIni);
	MaxWorkerSockets  = GetPrivateProfileInt(TEXT("COMMON"),TEXT("MaxWorkerSockets"),64,SysIni);
	GSVSocketTimeOut  = GetPrivateProfileInt(TEXT("COMMON"),TEXT("SocketTimeOut"),60,SysIni);

	suicideEvent = CreateEvent( 0, TRUE, FALSE, 0 );
	if(! VALID( suicideEvent ))
	{
		LogEvent(_T("ERROR LINE(%d):Create suicideEvent"),__LINE__);
		return -1;
	}

	scavengerArgs.suicideEvent = CreateEvent( 0, TRUE, FALSE, 0 );
	if(! VALID( scavengerArgs.suicideEvent ))
	{
		LogEvent(_T("ERROR LINE(%d):Create scavenger.suicideEvent"), __LINE__);
		return -1;
	}

	//============================================================================================================
	//==== create IOCP
	//============================================================================================================
	hWorkerIOCP = CreateIoCompletionPort( INVALID_HANDLE_VALUE, 0, 0, WorkerConcurrency );
	if(! VALID( hWorkerIOCP ))
	{
		LogEvent(_T("ERROR LINE(%d):Create CreateIoCompletionPort1"), __LINE__);
		return -1;
	}


	//============================================================================================================
	//====  create worker threads
	//============================================================================================================
	SYSTEM_INFO SystemInfo;
    GetSystemInfo(&SystemInfo);
    MaxWorkerThreads = SystemInfo.dwNumberOfProcessors*2;

	for (i = 0; i < MaxWorkerThreads; ++ i )
	{
		handle = (HANDLE) _beginthreadex( 0, 0, Worker, this, 0, &dummy );
		if ( VALID( handle ) )
		{
			WorkerThreads.push_back( handle );
		}
		else
		{
			LogEvent(_T("WARING:Failed to create thread (%d) ,gle ==%d"),i, GetLastError());
		}
	}

	if ( MaxWorkerThreads != WorkerThreads.size() )
	{
		LogEvent(_T("WARING(%d/%d) (worker current/max)"),WorkerThreads.size(), MaxWorkerThreads);
	}

	//============================================================================================================
	//==== initialize winsock
	//============================================================================================================
	WSADATA wd = { 0 };
	INT32S e;

	e = WSAStartup( MAKEWORD( 2, 0 ), &wd );
	SetLastError( e );											// for _err()
	if(e != 0 || LOBYTE( wd.wVersion ) < 2)
	{
		LogEvent(_T("ERROR:WSAStartup"));
		return -1;
	}

	SOCKADDR_IN addr;
	listener = socket( AF_INET, SOCK_STREAM, IPPROTO_TCP );		// create listen socket
	if(listener == INVALID_SOCKET)
	{
		LogEvent(_T("ERROR:listener socket()"));
		return -1;
	}

	addr.sin_family      = AF_INET;
	addr.sin_addr.s_addr = INADDR_ANY;
	addr.sin_port = htons( (short) WorkerPort );

	e = bind( listener, (SOCKADDR *) &addr, sizeof addr );
	if(e == SOCKET_ERROR)
	{
		LogEvent(_T("ERROR:bind() "));
		return -1;
	}

	e = listen( listener, 200 );
	if(e == SOCKET_ERROR)
	{
		LogEvent(_T("ERROR:listen() "));
		return -1;
	}

	//============================================================================================================
	//==== create accept sockets and OV structs	
	//============================================================================================================
	for ( i = 0; i < MaxWorkerSockets; ++ i )
	{
		OV *o       = new OV;
		o->state    = stAccepting;
		o->listener = &listener;
		o->ix = (int)WorkerOVs.size();
		o->ReinitContext();
		if ( o->socket != INVALID_SOCKET )
		{
			WorkerOVs.push_back( o );
		}
		else
		{
			LogEvent(_T("WARING:Failed to create socket (%d) ,gle ==%d"),i, GetLastError());
		}
	}

	if ( MaxWorkerSockets != WorkerOVs.size() )
	{
		LogEvent(_T("WARING:Only (%d) instead of (%d) sockets were created"),WorkerOVs.size(), MaxWorkerSockets);
	}

	//===============================================================================================================
	//==== timeout线程
	//===============================================================================================================
	scavengerArgs.delay   = 500;								// 500 msec between runs
	scavengerArgs.timeout = 5000;								// time before killing an idle accepted socket

	scavengerArgs.pOVList = &WorkerOVs;
	//测试，先取消自动关闭空闲线程功能
	//scavengerThread = (HANDLE) _beginthreadex( 0, 0,ScavengePulingSockets, &scavengerArgs, 0, &dummy );
	//if ( ! VALID( scavengerThread ) )
	//{
	//	LogEvent(_T("ERROR:_beginthreadex( scavenger )"));
	//	return -1;
	//}
	//===============================================================================================================
	//==== connect listener socket to IOCP
	//===============================================================================================================
	if(0 == CreateIoCompletionPort( (HANDLE) listener, hWorkerIOCP, 0, WorkerConcurrency ) )
	{
		LogEvent(_T("ERROR:CreateIoCompletionPort( listener )"));
		return -1;
	}
	return 0;
}


//****************************************************************************************************************
//**** 函数名:Run
//**** 功  能:运行server,直到结束,只是等待结束事件发生
//**** 参  数:
//****        none            - none
//**** 返回值:
//****        none            - none
//****************************************************************************************************************	
void CZServer::Run()
{
	LogEvent(_T("Server Start !"));
	WaitForSingleObject( suicideEvent, INFINITE );				//wait until end indicated
}



//****************************************************************************************************************
//**** 函数名:EndServer
//**** 功  能:结束Server,清除资源
//**** 参  数:
//****        none            - none
//**** 返回值:
//****        -1              - 清除server类资源失败
//****        0               - 清除server类资源成功
//****************************************************************************************************************	
INT32U CZServer::EndServer()
{
	INT32U i;

	closesocket( listener );									// no more accepts, please
	listener = INVALID_SOCKET;

	//===============================================================================================================
	//==== wait for worker thread terminations
	//===============================================================================================================
	for ( i = 0; i < MaxWorkerThreads; ++ i )
	{
		if(0 == PostQueuedCompletionStatus( hWorkerIOCP, 0, COMPKEY_DIEDIEDIE, 0 ) )
		{
			LogEvent(_T("ERROR:PostQueuedCompletionStatus"));
		}
	}

	i = WaitForMultipleObjects( (DWORD)WorkerThreads.size(), &WorkerThreads[0], TRUE, 15000 );
	switch ( i )
	{
		case WAIT_TIMEOUT:
			LogEvent(_T("WARING:Not all threads died in time."));
			break;
		case WAIT_FAILED:
			if(i != WAIT_OBJECT_0)
			{
				LogEvent(_T("ERROR:WaitForMultipleObjects() == WAIT_FAILED"));
				return -1;
			}
			break;
		default:
			break;
	}
	for ( vector< HANDLE >::iterator it = WorkerThreads.begin(); it != WorkerThreads.end(); ++it )
	{
		CloseHandle( *it );
	}
	WorkerThreads.clear();


	//============================================================================================================
	//==== kill off the scavenger
	//============================================================================================================
	SetEvent( scavengerArgs.suicideEvent );
	i = WaitForSingleObject( scavengerThread, 2 * scavengerArgs.timeout );
	CloseHandle( scavengerThread );
	CloseHandle( scavengerArgs.suicideEvent );

	//============================================================================================================
	//==== tear down and close sockets
	//============================================================================================================
	for ( list< OV *>::iterator it = WorkerOVs.begin(); it != WorkerOVs.end(); ++ it)
	{
		closesocket( (*it)->socket );
		delete (*it);
	}
	WorkerOVs.clear();

	WSACleanup();
	CloseHandle( hWorkerIOCP );
	CloseHandle( suicideEvent );

	//============================================================================================================
	//==== 结束common Dll
	//============================================================================================================
	::CoUninitialize();
	LogEvent(_T("Server Exit !"));
	return 0;
}

//****************************************************************************************************************
//**** 函数名:WriteServerStatusFile
//**** 功  能:把server当前的状态写入common/serverstatus.sys文件中去
//**** 参  数:
//****        none            - none
//**** 返回值:
//****        none            - none
//****************************************************************************************************************	
//void CZServer::WriteServerStatusFile()
//{
//	CFile stFile;
//	CString pathName;
//	statusHead FH;
//
//	pathName = GSVWorkPath + _T("common\\serverstatus.sys");
//	
//	if(stFile.Open(pathName,CFile::modeCreate|CFile::modeWrite|CFile::typeBinary))
//	{
//		FH.Flag = SERVERSTATUS_FLAG;
//		FH.WorkerConcurrency    = WorkerConcurrency;								
//		FH.MaxWorkerThreads     = MaxWorkerThreads;								
//		FH.MaxWorkerSockets     = MaxWorkerSockets;								
//		FH.runningWorkerThreads = runningWorkerThreads;					
//		FH.runningWorkerSockets = 0;		
//		for(list< OV * >::iterator ovi = WorkerOVs.begin();ovi != WorkerOVs.end();++ovi)
//		{
//			if( (*ovi)->state == stWaitingCMD || stSendingCMD == (*ovi)->state  )	
//			{
//				++FH.runningWorkerSockets;
//			}
//		}
//		stFile.Write(&FH, sizeof(FH));
//		LiveSigns->Write2StatusFile(stFile);
//		stFile.Close();
//	}
//
//}


//****************************************************************************************************************
//**** 函数名:ControlHandler
//**** 功  能:SCM控制函数,用于设置状态
//**** 参  数:
//****        dwOpcode        - 操作代码
//**** 返回值:
//****        none            - none
//****************************************************************************************************************	
BOOL CALLBACK ConsoleHandler( DWORD ConsoleEvent )
{
	switch ( ConsoleEvent )
	{
		case CTRL_LOGOFF_EVENT:									// FALL THROUGH
		case CTRL_C_EVENT:										// FALL THROUGH
		case CTRL_BREAK_EVENT:									// FALL THROUGH
		case CTRL_CLOSE_EVENT:									// FALL THROUGH
		case CTRL_SHUTDOWN_EVENT:
			if ( CZS->suicideEvent != 0 )
			{
				SetEvent( CZS->suicideEvent );
				return TRUE;
			}		
		default:												// FALL THROUGH because we cannot abort right now
			return FALSE;
	}
}

//****************************************************************************************************************
//**** 函数名:ServiceMain
//**** 功  能:主服务程序
//**** 参  数:
//****        dwArgc          - argc
//****        lpszArgv        - argv
//**** 返回值:
//****        none            - none
//****************************************************************************************************************	
void ServiceMain(  DWORD dwArgc, LPTSTR* lpszArgv) 
{ 
	#if(APPLICATION_TYPE == APP_SERVICE)
	ServiceStatus.dwServiceType       = SERVICE_WIN32; 
	ServiceStatus.dwCurrentState      = SERVICE_START_PENDING; 
	ServiceStatus.dwControlsAccepted  = SERVICE_ACCEPT_STOP | SERVICE_ACCEPT_SHUTDOWN;
	ServiceStatus.dwWin32ExitCode     = 0; 
	ServiceStatus.dwCheckPoint        = 0; 
	ServiceStatus.dwWaitHint          = 0; 
	ServiceStatus.dwServiceSpecificExitCode = 0; 

	hStatus = RegisterServiceCtrlHandler(szServiceName, (LPHANDLER_FUNCTION)ControlHandler); 
	if (hStatus == (SERVICE_STATUS_HANDLE)0)
	{ 		
		LogEvent(_T("Registering Control Handler failed"));
		return;													
	}  
	
	// We report the running status to SCM. 
	ServiceStatus.dwCurrentState = SERVICE_RUNNING; 
	SetServiceStatus (hStatus, &ServiceStatus);
	#endif	

	#if(APPLICATION_TYPE == APP_DEBUG)
	SetConsoleCtrlHandler( ConsoleHandler, TRUE );
	#endif
	
	if(CZS->InitServer() == -1)
	{		
		LogEvent(_T("Server Init Error !"));
		#if(APPLICATION_TYPE == APP_SERVICE)
		ServiceStatus.dwCurrentState = 	SERVICE_STOPPED; 
		ServiceStatus.dwWin32ExitCode = -1; 
		SetServiceStatus(hStatus, &ServiceStatus); 
		#endif
		return ;
	}


	CZS->Run();
	CZS->EndServer();

	#if(APPLICATION_TYPE == APP_DEBUG)
	SetConsoleCtrlHandler( ConsoleHandler, FALSE );
	#endif

	#if(APPLICATION_TYPE == APP_SERVICE)
	ServiceStatus.dwCurrentState = 	SERVICE_STOPPED; 
	ServiceStatus.dwWin32ExitCode = 0; 
	SetServiceStatus(hStatus, &ServiceStatus); 
	#endif
	return ;
}


//****************************************************************************************************************
//**** 函数名:main
//**** 功  能:主函数
//**** 参  数:
//****        argc            - argc
//****        argv            - argv
//**** 返回值:
//****        none            - none
//****************************************************************************************************************	
INT32S _tmain(INT32S argc, _TCHAR* argv[])
{
	#if APPLICATION_TYPE == APP_SERVICE
	SERVICE_TABLE_ENTRY ServiceTable[2];
	ServiceTable[0].lpServiceName = szServiceName;
	ServiceTable[0].lpServiceProc = (LPSERVICE_MAIN_FUNCTION)ServiceMain;

	ServiceTable[1].lpServiceName = NULL;
	ServiceTable[1].lpServiceProc = NULL;

	if (argc > 1 && _tcscmp(argv[1], TEXT("/install")) == 0)
	{
		Install();
	}
	else if (argc > 1 &&  _tcscmp(argv[1], TEXT("/uninstall")) == 0)
	{
		Uninstall();
	}
	else
	{
		if (!StartServiceCtrlDispatcher(ServiceTable))			// 启动服务的控制分派机线程
		{
			LogEvent(_T("Register Service Main Function Error!"));
		}
	}
	#else
	ServiceMain(argc, argv);
	#endif

	return 1;
}

#if APPLICATION_TYPE == APP_SERVICE
//****************************************************************************************************************
//**** 函数名:ControlHandler
//**** 功  能:SCM控制函数,用于设置状态
//**** 参  数:
//****        dwOpcode        - 操作代码
//**** 返回值:
//****        none            - none
//****************************************************************************************************************	
void ControlHandler(DWORD dwOpcode) 
{ 
    switch (dwOpcode)
    {
		case SERVICE_CONTROL_STOP:
		case SERVICE_CONTROL_SHUTDOWN:
			if ( VALID(CZS->suicideEvent))
			{
				SetEvent( CZS->suicideEvent );
			}	
			break;
		case SERVICE_CONTROL_PAUSE:
			break;
		case SERVICE_CONTROL_CONTINUE:
			break;
		case SERVICE_CONTROL_INTERROGATE:
			break;
		default:
			break;
    }
	SetServiceStatus (hStatus, &ServiceStatus);
	return; 
}

//****************************************************************************************************************
//**** 函数名:IsInstalled
//**** 功  能:判断服务是否已经被安装
//**** 参  数:
//****        none            - none
//**** 返回值:
//****        true            - 已安装,false表示没安装
//****************************************************************************************************************	
BOOL IsInstalled()
{
    BOOL bResult = FALSE;

	//打开服务控制管理器
    SC_HANDLE hSCM = OpenSCManager(NULL, NULL, SC_MANAGER_ALL_ACCESS);
    if (hSCM != NULL)
    {
		//打开服务
        SC_HANDLE hService = OpenService(hSCM, szServiceName, SERVICE_QUERY_CONFIG);
        if (hService != NULL)
        {
            bResult = TRUE;
            CloseServiceHandle(hService);
        }
        CloseServiceHandle(hSCM);
    }
    return bResult;
}


//****************************************************************************************************************
//**** 函数名:SetSvcFailureAction
//**** 功  能:设置服务程序失败后的操作,目前设置为重启服务程序
//**** 参  数:
//****        none            - none
//**** 返回值:
//****        none            - none
//****************************************************************************************************************	
VOID SetSvcFailureAction()
{
    SC_HANDLE schSCManager;
    SC_HANDLE schService;

    schSCManager = OpenSCManager( NULL, NULL, SC_MANAGER_ALL_ACCESS);  
    if (NULL == schSCManager) 
    {
        MessageBox(NULL, _T("OpenSCManager failed"), szServiceName, MB_OK);
		return;
    }

    schService = OpenService( schSCManager, szServiceName,  SERVICE_ALL_ACCESS); 
 
    if (schService == NULL)
    { 
		MessageBox(NULL, _T("OpenService failed"), szServiceName, MB_OK);
        CloseServiceHandle(schSCManager);
        return;
    }    
	
	//============================================================================================================
	//==== 设置服务属性,设置服务失败后,一分种后重启
	//============================================================================================================
	SERVICE_FAILURE_ACTIONS lpInfo={0};
	SC_ACTION Actions[3];
	Actions[0].Delay = 60*1000;
	Actions[0].Type  = SC_ACTION_RESTART;
	Actions[1].Delay = 60*1000;
	Actions[1].Type  = SC_ACTION_RESTART;
	Actions[2].Delay = 60*1000;
	Actions[2].Type  = SC_ACTION_RESTART;


	lpInfo.dwResetPeriod = INFINITE;							//60S
	lpInfo.lpRebootMsg   = _T("");
	lpInfo.lpCommand     = _T("");
	lpInfo.lpsaActions   = Actions;
	lpInfo.cActions      = 3; 
	if(!ChangeServiceConfig2(schService,SERVICE_CONFIG_FAILURE_ACTIONS, &lpInfo))
	{
		MessageBox(NULL, _T("Set Service FailureAction Error"), szServiceName, MB_OK);
	}

    CloseServiceHandle(schService); 
    CloseServiceHandle(schSCManager);
}

//****************************************************************************************************************
//**** 函数名:Install
//**** 功  能:安装服务函数
//**** 参  数:
//****        none            - none
//**** 返回值:
//****        true            - 已安装,false表示没安装
//****************************************************************************************************************	
BOOL Install()
{
    if (IsInstalled())
	{
		return TRUE;
	}
	//============================================================================================================
	//==== 打开服务控制管理器
    //============================================================================================================
	SC_HANDLE hSCM = OpenSCManager(NULL, NULL, SC_MANAGER_ALL_ACCESS);
    if (hSCM == NULL)
    {
        MessageBox(NULL, _T("Couldn't open service manager"), szServiceName, MB_OK);
        return FALSE;
    }

    // Get the executable file path
    TCHAR szFilePath[MAX_PATH];
    GetModuleFileName(NULL, szFilePath, MAX_PATH);

	//============================================================================================================
	//==== 创建服务
	//============================================================================================================
    SC_HANDLE hService = CreateService(
        hSCM, szServiceName, szServiceName,
        SERVICE_ALL_ACCESS, SERVICE_WIN32_OWN_PROCESS,
        SERVICE_AUTO_START, SERVICE_ERROR_NORMAL,
        szFilePath, NULL, NULL, _T(""), NULL, NULL);

    if (hService == NULL)
    {
        CloseServiceHandle(hSCM);
        MessageBox(NULL, _T("Couldn't create service"), szServiceName, MB_OK);
        return FALSE;
    }
	

    CloseServiceHandle(hService);
    CloseServiceHandle(hSCM);
	SetSvcFailureAction();
    return TRUE;
}


//****************************************************************************************************************
//**** 函数名:Uninstall
//**** 功  能:删除服务函数
//**** 参  数:
//****        none            - none
//**** 返回值:
//****        true            - 已删除,false删除失败
//****************************************************************************************************************	
BOOL Uninstall()
{
    if (!IsInstalled())
	{
        return TRUE;
	}

    SC_HANDLE hSCM = OpenSCManager(NULL, NULL, SC_MANAGER_ALL_ACCESS);

    if (hSCM == NULL)
    {
        MessageBox(NULL, _T("Couldn't open service manager"), szServiceName, MB_OK);
        return FALSE;
    }

    SC_HANDLE hService = ::OpenService(hSCM, szServiceName, SERVICE_STOP | DELETE);

    if (hService == NULL)
    {
        CloseServiceHandle(hSCM);
        MessageBox(NULL, _T("Couldn't open service"), szServiceName, MB_OK);
        return FALSE;
    }
    SERVICE_STATUS status;
    ControlService(hService, SERVICE_CONTROL_STOP, &status);

	//删除服务
    BOOL bDelete = ::DeleteService(hService);
    CloseServiceHandle(hService);
    CloseServiceHandle(hSCM);

    if (bDelete)
	{
        return TRUE;
	}

    LogEvent(_T("Service could not be deleted"));
    return FALSE;
}

#endif
//****************************************************************************************************************
//**** 函数名:LogEvent
//**** 功  能:记录服务事件
//**** 参  数:
//****        pFormat         - format
//****        ....            - value
//**** 返回值:
//****        none            - none
//****************************************************************************************************************	
void LogEvent(LPCTSTR pFormat, ...)
{
    TCHAR   chMsg[256];
    HANDLE  hEventSource;
    LPTSTR  lpszStrings[1];
    va_list pArg;

    va_start(pArg, pFormat);
    _vstprintf(chMsg,256, pFormat, pArg);
    va_end(pArg);

    lpszStrings[0] = chMsg;
	
	hEventSource = RegisterEventSource(NULL, szServiceName);
	if (hEventSource != NULL)
	{
		ReportEvent(hEventSource, EVENTLOG_INFORMATION_TYPE, 0, 0, NULL, 1, 0, (LPCTSTR*) &lpszStrings[0], NULL);
		DeregisterEventSource(hEventSource);
	}
}
