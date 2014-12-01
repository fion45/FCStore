#ifndef __SIGN_SERVER__
#define __SIGN_SERVER__

#include <stdio.h> 
#include <process.h>
#include "worker.h"
#include <Winsock2.h>

#define SIGN_TYPE						1
#define TASK_TYPE                       2
//================================================================================================================
//==== CZServer类
//================================================================================================================
class CZServer
{
private:
	CZServer();													//single tempelate
	static CZServer *instance;
public:
	INT32S WorkerConcurrency;									//同时工作的worker数
	INT32U MaxWorkerThreads;									//最大工作线程,是与app,web通信的线程
	INT32U WorkerPort;											//与Web,app通信的端口
	INT32U MaxWorkerSockets;									//worker打开最大的sockets	
public:
	INT32U runningWorkerThreads;								//正在运行的worker线程数
	HANDLE hWorkerIOCP;											//worker iocp
	HANDLE suicideEvent;										//结束事件
	SOCKET listener;											//主socket
	HANDLE scavengerThread;										// handle to scavenger thread
	ScavengerArgs scavengerArgs;								// args for scavenger thread

	vector< HANDLE > WorkerThreads;								//worker theads vector
	list< OV * >     WorkerOVs;									//Worder overlapped list	

	CString SysIni;												//系统ini路径

public:
	static CZServer* Instance(); 
	INT32U    InitServer();
	void      Run();
	INT32U    EndServer();
	void      WriteServerStatusFile();
};

extern CZServer * CZS;

void LogEvent(LPCTSTR pFormat, ...);

#endif