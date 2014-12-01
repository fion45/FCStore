// worker thread code
#include "stdafx.h"
#include "worker.h"
#include <assert.h>
#include "mswsock.h"
#include "Server.h"
#include "DebugMsg.h"
#include "OverLapped.h"

CString GSVWorkPath;											//系统工作路径,读写文件都以这个为前缀
INT32U  GSVSocketTimeOut;


CString ErrorString(DWORD err)
{
	CString Error;
	LPTSTR s;
	if(::FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER |FORMAT_MESSAGE_FROM_SYSTEM,
				NULL,err,0,	(LPTSTR)&s,	0,NULL) == 0)
	{ 
		CString t;
		t.Format(_T("%s,%d"), err, LOWORD(err));
		Error = t;
	} 
	else
	{ 
		LPTSTR p = _tcschr(s, _T('\r'));
		if(p != NULL)
		{ 
			*p = _T('\0');
		} 
		Error = s;
		::LocalFree(s);
	} 
	return Error;
} 

//****************************************************************************************************************
//**** 函数名:ScavengePulingSockets
//**** 功  能:closes sockets that were connected but never received data
//**** 参  数:
//****        arg             - ScavengerArgs
//**** 返回值:
//****        0               
//****************************************************************************************************************	
INT32U CALLBACK ScavengePulingSockets( void *arg )
{
	DWORD st;
	list< OV * >::iterator ovi;
	INT32S seconds, len;
	LARGE_INTEGER Frequency,EndCount;

	QueryPerformanceFrequency(&Frequency);

	while ( 1 )
	{
		QueryPerformanceCounter(&EndCount);
		// scavenge sockets
		for ( ovi = CZS->WorkerOVs.begin(); ovi != CZS->WorkerOVs.end(); ++ ovi)
		{
			if ( (*ovi)->state == stAccepting )
			{
				// stAccepting means AcceptEx() called, but no completion yet
				// BUGBUG sanity check
				if ( (*ovi)->end < (*ovi)->buf || (*ovi)->end > &(*ovi)->buf[OV::bs] )
				{
					DebugMSG(_T("\nSocket(%d) Buf Error(end != buf)"),(*ovi)->ix);
				}

				// determine if the socket is connected
				seconds = 0;
				len = sizeof seconds;
				if ( 0 == getsockopt( (*ovi)->socket, SOL_SOCKET, SO_CONNECT_TIME, (char *) &seconds, &len ) )
				{
					if ( seconds != -1 && seconds * 1000 > ((ScavengerArgs *) arg)->timeout )
					{
						DebugMSG(_T("\nSocket(%d) Idle (%d) Seconds, Close By Server(postOV)"),(*ovi)->ix, seconds);
						PostQueuedCompletionStatus( CZS->hWorkerIOCP, 0, COMPKEY_REINITSELF, *ovi ) ;
						//closesocket( (*ovi)->socket );
					}
				}
			}
			else if( (*ovi)->state == stWaitingCMD || stSendingCMD == (*ovi)->state  )					
			{													//看看是否超时,超时后关闭socket				
				if(((EndCount.QuadPart -(*ovi)->StartTime.QuadPart)/Frequency.QuadPart) > GSVSocketTimeOut)							
				{		
					DebugMSG(_T("\nSocket(%d) TimeOut (%d) Seconds, PostOV"),(*ovi)->ix, GSVSocketTimeOut);
					PostQueuedCompletionStatus( CZS->hWorkerIOCP, 0, COMPKEY_REINITSELF, *ovi ) ;
					/*
					if((*ovi)->closestep == 0)
					{
						DebugMSG(_T("\nSocket(%d) TimeOut (%d) Seconds, Shutdown"),(*ovi)->ix, GSVSocketTimeOut);
						shutdown( (*ovi)->socket, SD_BOTH);			
						(*ovi)->closestep=1;
					}
					else if((*ovi)->socket != INVALID_SOCKET )
					{
						DebugMSG(_T("\nSocket(%d) TimeOut (%d) Seconds, Closed"),(*ovi)->ix, GSVSocketTimeOut);
						closesocket( (*ovi)->socket );			
						(*ovi)->socket = INVALID_SOCKET;
					}
					*/
				}				
			}
		}

		// pause until next run due
		st = WaitForSingleObject( ((ScavengerArgs *) arg)->suicideEvent,((ScavengerArgs *) arg)->delay );
		if ( st != WAIT_TIMEOUT )
		{
			break;
		}
	}

	return 0;
}

//****************************************************************************************************************
//**** 函数名:Worker
//**** 功  能:working threads
//**** 参  数:
//****        arg             - ScavengerArgs
//**** 返回值:
//****        0               
//****************************************************************************************************************	
INT32U CALLBACK Worker( void *arg )
{
	DWORD n, gle;
	ULONG key;
	BOOL result;
	OV   *pov;

	InterlockedIncrement( &CZS->runningWorkerThreads );
	for ( ; ; )
	{
		InterlockedDecrement( &CZS->runningWorkerThreads );
		result = GetQueuedCompletionStatus( CZS->hWorkerIOCP, &n,	&key, (OVERLAPPED **) &pov, INFINITE );

		// check if we are to die
		if ( key == COMPKEY_DIEDIEDIE )
		{
			break;												// sniffle
		}
		else if( key == COMPKEY_REINITSELF )
		{
			if(result == TRUE && pov)
			{
				pov->DoClose();
			}
			InterlockedIncrement( &CZS->runningWorkerThreads );
			continue;
		}

		gle = GetLastError();
		if ( pov != 0 )
		{
			pov->nNumBytes = n;
		}
		InterlockedIncrement( &CZS->runningWorkerThreads );

		if ( result == FALSE ) // something gone awry?
		{
			DebugMSG(_T("IOCP RETURN FALSE,GLE=%d"),gle);
			// something failed. Was it the call itself, or did we just eat
			// a failed socket I/O?
			if ( pov == 0 ) // the IOCP op failed
			{
				// no key, no byte count, no overlapped info available!
				// how do I handle this?
				// by ignoring it. I don't even have a socket handle to close.
				// You might want to abort instead, as something is seriously
				// wrong if we get here.
			}
			else
			{
				// key, byte count, and overlapped are valid.
				// tear down the connection and requeue the socket
				// (unless the listener socket is closed, in which case
				// this is a failed AcceptEx(), and we stop accepting)
				if(ERROR_OPERATION_ABORTED != gle && ERROR_NETNAME_DELETED != gle)
				{
					pov->DoShutDown();
					pov->PostRead();					
				}
			}
		}
		else 
		{
			QueryPerformanceCounter(&pov->StartTime);
			DoIo( *pov ); 
		}
	}

	InterlockedDecrement( &CZS->runningWorkerThreads );
	return 0;
}



//****************************************************************************************************************
//**** 函数名:DoIo
//**** 功  能:handle a successful I/O
//**** 参  数:
//****        ov              - overlapped
//**** 返回值:
//****        none               
//****************************************************************************************************************	
void DoIo( OV &ov )
{
	INT32S locallen, remotelen;
	CString RemoteIP;
	sockaddr_in *plocal = 0, *premote = 0;

	switch ( ov.state )
	{
		case stAccepting:										//TCP 连接
			GetAcceptExSockaddrs( &ov.buf[0], OV::initialReceiveSize, ov.addrlen, ov.addrlen, 
				                 (sockaddr **) &plocal, &locallen,(sockaddr **) &premote, &remotelen );
			memcpy( &ov.local, plocal, sizeof sockaddr_in );
			memcpy( &ov.peer, premote, sizeof sockaddr_in );

			setsockopt(ov.socket,SOL_SOCKET,SO_UPDATE_ACCEPT_CONTEXT,(char*) &CZS->listener,sizeof(CZS->listener));

			//====================================================================================================
			//==== We still need to associate the newly connected socket to our IOCP:
			//====================================================================================================
			CreateIoCompletionPort( (HANDLE) ov.socket, CZS->hWorkerIOCP, 0, CZS->WorkerConcurrency);
			RemoteIP = inet_ntoa(ov.peer.sin_addr);
			//DebugMSG(_T("[%d] stAccepting: from %s:%d"),ov.ix,RemoteIP,ntohs(ov.peer.sin_port));

			if ( ov.nNumBytes != 0 )							// we received something during AcceptEx()
			{ 
				ov.state = stWaitingCMD;
				ov.end += ov.nNumBytes;
				DoCommands( ov );
			}
			else
			{
				ov.PostRead();
			}
			break;
		case stWaitingCMD:										//等命令
			if ( ov.nNumBytes == SOCKET_ERROR )					// error?
			{
				DebugMSG(_T("[%d] stWaitingCMD: ov.n == SOCKET_ERROR"),ov.ix);
				ov.DoClose(true );
			}
			else if ( ov.nNumBytes == 0 )						// connection closing?
			{				
				//DebugMSG(_T("[%d] stWaitingCMD: ov.n == 0, closing"),ov.ix);
				ov.DoClose(true );
			}
			else
			{
				//DebugMSG(_T("[%d] stWaitingCMD: ov.n == %d"),ov.ix,ov.nNumBytes);
				ov.end += ov.nNumBytes;
				DoCommands( ov );				
			}
			break;
		case stSendingCMD:										//写命令出去已完成, post another read
			if ( ov.nNumBytes == SOCKET_ERROR )					// error?
			{
				DebugMSG(_T("[%d] stSendingCMD: ov.n == SOCKET_ERROR"),ov.ix);
				ov.DoClose(true );
			}
			else if ( ov.nNumBytes == 0 )						// connection closing?
			{
				//DebugMSG(_T("[%d] stSendingCMD: ov.n == 0, closing"),ov.ix);
				ov.DoClose(true );
			}
			else
			{
				//DebugMSG(_T("[%d] stSendingCMD: ov.n == %d"),ov.ix,ov.nNumBytes);
				ov.PostRead( );
			}
			break;
		default:
			ov.PostRead();
			break;
	}
}


//****************************************************************************************************************
//**** 函数名:DoCommands
//**** 功  能:获得一条命令并处理
//**** 参  数:
//****        ov              - overlapped
//**** 返回值:
//****        false           - 无法识别的命令,或命令不全 
//****************************************************************************************************************	
bool DoCommands(OV &ov)
{
	ov.needPostRead = true;										/* 默认是需要调用PostRead函数*/

	//处理数据

	
	if (ov.needPostRead)
	{
		ov.PostRead();
	}
	return  false;
}


//****************************************************************************************************************
//**** 函数名:DoOneCommand
//**** 功  能:执行一条命令
//**** 参  数:
//****        ov              - overlapped
//****        cmd             - cmd string
//****        cmdsize         - cmd string size
//**** 返回值:
//****        none               
//****************************************************************************************************************	
void DoOneCommand(OV &ov, INT8U *cmd, INT32U cmdsize)
{
	
}


