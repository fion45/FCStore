#include "StdAfx.h"
#include "CharConvert.h"

CString U2StrT(const wchar_t* szA)
{
	#ifndef UNICODE
			
	#else
		return CString(szA);
	#endif
}

CString A2StrT(const char* szA)
{
	#ifndef UNICODE
		A2StrA(szA);
	#else
		return A2StrU(szA);
	#endif
}

CString A2StrA(const char* szA)
{
	CString result(szA);
	return result;
}

CString A2StrU(const char* szA)
{
	int cLen = A2U(szA,NULL,0);
	wchar_t* szU = new wchar_t[cLen];
	memset(szU,0,cLen * sizeof(wchar_t));
	A2U(szA,szU,cLen);
	CString result(szU);
	delete[] szU;
	return result;
}

CString U2StrA(const wchar_t* szU)
{
	int cLen = U2A(szU,NULL,0);
	char* szA = new char[cLen];
	memset(szA,0,cLen * sizeof(char));
	U2A(szU,szA,cLen);
	CString result(szA);
	delete[] szA;
	return result;
}

CString U2StrU(const wchar_t* szU)
{
	return CString(szU);
}

int U2A(const wchar_t* szU,char* szA,size_t cnt)
{
	return WideCharToMultiByte (CP_ACP, 0, szU, -1, szA, cnt, NULL, NULL);
}

int A2U(const char* szA,wchar_t* szU,size_t cnt)
{
	return MultiByteToWideChar (CP_ACP, 0, szA, -1, szU, cnt);
}

std::wstring T2U(LPCTSTR szStr)
{
#ifndef UNICODE
	return A2U(szStr);
#else
	return std::wstring(szStr);
#endif
}

std::string T2AA(LPCTSTR szStr)
{
#ifndef UNICODE
	return std::string(szStr);
#else
	return U2A(szStr);
#endif
}

std::string U2A(const wchar_t* szU)
{
	int nRetCode=U2A(szU,0,0);
	if(0==nRetCode)
		return std::string();
	std::string str(nRetCode-1,'\0');
	U2A(szU,(char*)(str.c_str()),nRetCode);
	return str;
}

std::wstring A2U(const char* szA)
{
	int nRetCode=A2U(szA,0,0);
	if(0==nRetCode)
		return std::wstring();
	std::wstring str(nRetCode-1,'\0');
	A2U(szA,(wchar_t*)(str.c_str()),nRetCode);
	return str;
}