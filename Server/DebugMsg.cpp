#include "stdafx.h"
#include "ThreadSync.h"


#ifdef _DEBUG

CritSect OutPutCrit;

//****************************************************************************************************************
//**** ������:DebugMSG
//**** ��  ��:���������Ϣ,Ϊ��Ϣ��������,���������ֹ����
//**** ��  ��:
//****        pFormat         - format
//****        ....            - value
//**** ����ֵ:
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