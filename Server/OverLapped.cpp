#include "stdafx.h"
#include <Winsock2.h>
#include <string>
#include <iostream>
#include <iomanip>
#include "mswsock.h"
#include "Type.h"
#include "OverLapped.h"

using namespace std;

//================================================================================================================
//==== a note on initialReceiveSize: If the client starts sending as soon as it has a connection, you can leave 
//==== this value != 0. If, however, the client expects the server to offer a banner first, you *must* set 
//==== initialReceiveSize to ZERO. If you don't, AcceptEx() will wait for the client to speak up, while the 
//==== client waits for the server to say something!
//================================================================================================================			
// available buffer minus the space for local/peer addresses
const int OV::initialReceiveSize = OV::bs - 2 * ( sizeof sockaddr_in + 16 );
const int OV::addrlen = sizeof(sockaddr_in) + 16;


//****************************************************************************************************************
//**** 函数名:DoClose
//**** 功  能:close socket
//**** 参  数:
//****        force           - force,default == false
//****        listenAgain     - listenAgain,default = true
//**** 返回值:
//****        none               
//****************************************************************************************************************	
void OV::DoClose( bool force, bool listenAgain)
{
	struct linger li = { 0, 0 };								// default: SO_DONTLINGER

	if ( force )
	{
		li.l_onoff = 1;											// SO_LINGER, timeout = 0
	}
	closestep = 0;
	setsockopt( socket, SOL_SOCKET, SO_LINGER, (char *) &li, sizeof li );

	closesocket(socket );
	socket = INVALID_SOCKET;

	if ( listenAgain )
	{
		ReinitContext();
	}
}

//****************************************************************************************************************
//**** 函数名:DoShutDown
//**** 功  能:shutdown socket
//**** 参  数:
//****        force           - force,default == false
//****        listenAgain     - listenAgain,default = true
//**** 返回值:
//****        none               
//****************************************************************************************************************	
void OV::DoShutDown()
{
	if(socket != INVALID_SOCKET)
	{
		if(shutdown(socket,SD_BOTH ))
		{
			#ifdef _DEBUG
			cout << " shutdown OK" << endl;
			#endif
		}
	}
}

//****************************************************************************************************************
//**** 函数名:ReinitContext
//**** 功  能:init socket
//**** 参  数:
//****        none            - none
//**** 返回值:
//****        none               
//****************************************************************************************************************	
void OV::ReinitContext()
{
	INT32S zero = 0;
	
	socket = ::socket( AF_INET, SOCK_STREAM, IPPROTO_TCP );
	if ( socket != INVALID_SOCKET )
	{
		state  = stAccepting;
		end    = &buf[0];

		setsockopt( socket, SOL_SOCKET, SO_SNDBUF, (char *) &zero, sizeof zero );
		setsockopt( socket, SOL_SOCKET, SO_RCVBUF, (char *) &zero, sizeof zero );

		// clear the area for AcceptEx() addresses. Why? Because if
		// OV::initialReceiveSize == 0 (that is, if SEND_BANNER_FIRST
		// is defined), no addresses are captured; this way, we get
		// at least zeroes instead of garbage.
		memset( buf, '\0', sizeof(buf));

		if ( ! AcceptEx( *listener, socket, &buf[0],initialReceiveSize, addrlen, addrlen, &nNumBytes, this ) )
		{
			// AcceptEx() returned FALSE, check for real errors
			if ( GetLastError() != ERROR_IO_PENDING )
			{
				#ifdef _DEBUG
				clog << "AcceptEx() in ReinitContext() failed, gle == " <<	GetLastError() << endl;
				#endif
			}
		}
	}
	else
	{
		#ifdef _DEBUG
		clog << "Socket creation in ReinitContext() failed, gle == " <<	GetLastError() << endl;
		#endif
	}
}



//****************************************************************************************************************
//**** 函数名:PostRead
//**** 功  能:returns true if read completed immediately
//**** 参  数:
//****        endvalue        - end 位置
//**** 返回值:
//****        none               
//****************************************************************************************************************	
void OV::PostRead(char* endloc)
{
	BOOL result;

	// if there is not enough room in the buffer,
	// flush it and set the ignore-this-cmd flag
	if(endloc == NULL)
	{
		end = &buf[0];
	}
	else
	{
		end = endloc;
	}
	needPostRead = false;
	state  = stWaitingCMD;						
	nNumBytes = (DWORD)(&buf[bs] - end);
	//memset(end, 0, nNumBytes);
	#ifdef USE_WSARECV
	recvBufferDescriptor.len = nNumBytes;
	recvBufferDescriptor.buf = end;
	recvFlags = 0;
	result = WSARecv( socket, &recvBufferDescriptor, 1,&nNumBytes, &recvFlags, this, 0 );
	result = ( result != SOCKET_ERROR );						// inverted logic
	#else
	result = ReadFile( (HANDLE) s, end, n, &n, &ov );
	#endif
	if ( result )												// everything OK
	{		
		return;
	}
	else
	{
		if ( GetLastError() != ERROR_IO_PENDING )
		{
			DoClose(true );
		}
	}

	return;
}

//****************************************************************************************************************
//**** 函数名:SendReply
//**** 功  能:
//**** 参  数:
//****        none            - none
//**** 返回值:
//****        none               
//****************************************************************************************************************	
void OV::SendReply()
{
	BOOL result;
	DWORD err;

	needPostRead = false;
	state        = stSendingCMD;
	// submit a WriteFile() for the reply buffer
	nNumBytes = (DWORD)sendSize;
	#ifdef USE_WSASEND
	sendBufferDescriptor.len = nNumBytes;
	// casting away const is not nice, but since the NT guys have
	// an innate hatred for const input argument declarations,
	// there is not much we can do
	sendBufferDescriptor.buf = sendbuf;
	result = WSASend( socket, &sendBufferDescriptor,1,&nNumBytes, 0, this, 0 );
	result = ( result != SOCKET_ERROR ); // WSASend() uses inverted logic wrt/WriteFile()
	err = WSAGetLastError();
	#else
	result = WriteFile( (HANDLE) s, reply.c_str(), n, &n, &ov );
	err = GetLastError();
	#endif
	if ( ! result )
	{
		if ( err != ERROR_IO_PENDING )
		{
			DoClose( true );
			// the fall-through does nothing because the caller
			// shouldn't post another read -- the reinitialised
			// socket has an AcceptEx() pending
		}
		else
		{
			// (else branch intentionally empty)

			// if we get here, gle == ERROR_IO_PENDING; nothing
			// left to do but return. Caller loops back to GQCS().
		}
	}
	else // WriteFile()
	{
		// the write completed immediately
		// this doesn't bother us -- we will still
		// get the completion packet
	}
}