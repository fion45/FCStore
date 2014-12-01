#ifndef __SIGN_SERVER__
#define __SIGN_SERVER__

#include <stdio.h> 
#include <process.h>
#include "worker.h"
#include <Winsock2.h>

#define SIGN_TYPE						1
#define TASK_TYPE                       2
//================================================================================================================
//==== CZServer��
//================================================================================================================
class CZServer
{
private:
	CZServer();													//single tempelate
	static CZServer *instance;
public:
	INT32S WorkerConcurrency;									//ͬʱ������worker��
	INT32U MaxWorkerThreads;									//������߳�,����app,webͨ�ŵ��߳�
	INT32U WorkerPort;											//��Web,appͨ�ŵĶ˿�
	INT32U MaxWorkerSockets;									//worker������sockets	
public:
	INT32U runningWorkerThreads;								//�������е�worker�߳���
	HANDLE hWorkerIOCP;											//worker iocp
	HANDLE suicideEvent;										//�����¼�
	SOCKET listener;											//��socket
	HANDLE scavengerThread;										// handle to scavenger thread
	ScavengerArgs scavengerArgs;								// args for scavenger thread

	vector< HANDLE > WorkerThreads;								//worker theads vector
	list< OV * >     WorkerOVs;									//Worder overlapped list	

	CString SysIni;												//ϵͳini·��

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