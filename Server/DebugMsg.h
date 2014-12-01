#ifndef __DEBUG_MSG_H__
#define __DEBUG_MSG_H__


void _DebugMSG(LPCTSTR pFormat, ...);

#ifdef _DEBUG
#define DebugMSG _DebugMSG
#else
#define DebugMSG 
#endif

#endif


