#ifndef __OVERLAPPED_H__
#define __OVERLAPPED_H__

#include <Winsock2.h>
#include <string>
#include "Type.h"
#include "FCII.h"

//================================================================================================================
//====  if defined, code will use WsaSend(), else WriteFile()
//================================================================================================================
#define USE_WSASEND						1  

//================================================================================================================
//==== if defined, code will use WSARecv(), else ReadFile()
//================================================================================================================
#define USE_WSARECV						1

//================================================================================================================
//==== stats
//================================================================================================================
enum State { stClosed, stAccepting, stWaitingCMD, stSendingCMD };


//================================================================================================================
//==== inheriting from OVERLAPPED limits us to one outstanding I/O per connection. Ask me if I care ...
//===============================================================================================================
struct OV: public OVERLAPPED
{
	static const INT32S addrlen;
	static const INT32S initialReceiveSize;
	const  enum { bs = 2048 };

	DWORD  tag;
	INT32S ix;													// makes debugging easier
	INT32U closestep;											// 0=先shutdown,然后再close = 1

	//===============================================================================================================
	//==== user id
	//===============================================================================================================
	INT32U userID;
	LARGE_INTEGER StartTime;									//记录最后一次通信时的时间
	State state;												// what the connection is doing
	SOCKET *listener;
	SOCKET socket;												// the connection's socket handle
	DWORD nNumBytes;											// number of bytes received or sent
	char *end;													// points to first free byte in buf
	bool needPostRead;											// == true to please call postread

	sockaddr_in local, peer;									// addresses

	char buf[bs+64];											// receive buffer

	INT32U sendSize;    										// send data size
	char sendbuf[bs+64];										// send buffer

	//std::string filename;										// file name for read or write

	#ifdef USE_WSASEND
	// no array, as we won't ever transmit more than one buffer at a time
	WSABUF sendBufferDescriptor;
	#endif

	#ifdef USE_WSARECV
	// no array, as we won't ever transmit more than one buffer at a time
	WSABUF recvBufferDescriptor;
	DWORD recvFlags;											// needed by WSARecv()
	#endif

	FCII* fcii;

	OV(): tag( '@@@@' ), state( stClosed ), socket( SOCKET_ERROR ), end( &buf[0] ), closestep(0)
	{
		Internal = InternalHigh = Offset = OffsetHigh = 0;
		hEvent = 0; 
		listener = NULL; 
		fcii = NULL;
	}
	OV( const OV &o )
	{
		tag = o.tag;
		ix = o.ix;
		closestep = o.closestep;
		state = o.state;
		socket = o.socket;
		nNumBytes = o.nNumBytes;
		end = &buf[0] + ( o.end - &o.buf[0] );
		local = o.local;
		peer = o.peer;
		fcii = o.fcii;
		memcpy( buf, o.buf, bs );
	}
	~OV() { tag = '----'; }										// real cleanup handled by main function

	void DoClose( bool force = false, bool listenAgain = true );
	void SendReply();
	void PostRead(char* endloc=NULL);
	void ReinitContext();
	void DoShutDown();
};


#endif