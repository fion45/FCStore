#include "stdafx.h"
#include "ThreadSync.h"


#ifdef _DEBUG

CritSect OutPutCrit;

//****************************************************************************************************************
//**** 函数名:DebugMSG
//**** 功  能:输出调试信息,为信息的完整性,这个函数阻止重入
//**** 参  数:
//****        pFormat         - format
//****        ....            - value
//**** 返回值:
//****        none            - none
//****************************************************************************************************************	
void _DebugMSG(LPCTSTR pFormat, ...)
{
    CString str;
	ThreadLock Lock(OutPutCrit);

    va_list args;
    va_start(args, pFormat);
    str.FormatV(pFormat, args);
    va_end(args);
	str += _T("\r\n");
    _tprintf_s(str);
    return;
}


#endif