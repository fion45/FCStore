#ifndef __WORKER_H__
#define __WORKER_H__

#include <Winsock2.h>
#include <string>
#include "Type.h"
#include "OverLapped.h"

using namespace std;


//================================================================================================================
//==== IOCP worker function declaration
//================================================================================================================
INT32U CALLBACK Worker( void *arg );


//================================================================================================================
// OVERLAPPED wrapper to use as connection context
// We don't use completion keys; instead, I use the OVERLAPPED pointer to get at my OV struct below.
// this frees up the completion key, which I only use to tell my workers to commit suicide.
//================================================================================================================
#define COMPKEY_DIEDIEDIE				((DWORD) -1L)
#define COMPKEY_REINITSELF				((DWORD) -2L)

//================================================================================================================
//==== scavenger function declaration
//================================================================================================================
struct ScavengerArgs
{
	list<OV *> *pOVList;
	INT32S delay;						// milliseconds between two runs of the idle socket scavenger
	INT32S timeout;						// max idle time before first data on a socket, in milliseconds
	HANDLE suicideEvent;				// tells scavenger when to die
};

extern CString GSVWorkPath;										//系统工作路径,读写文件都以这个为前缀
extern INT32U  GSVSocketTimeOut;

INT32U CALLBACK ScavengePulingSockets( void *arg );
void DoIo( OV &ov );
void DoClose( OV &ov, bool force = false, bool listenAgain = true );
void ReinitContext( OV &ov );
void PostRead( OV &ov );
void SendReply( OV &ov );
bool DoCommands( OV &ov );
void DoOneCommand( char *p, OV &ov );

#endif
