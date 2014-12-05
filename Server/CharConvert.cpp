#include "stdafx.h"
#include "CharConvert.h"

CString MyU2StrT(const wchar_t* szA)
{
#ifndef UNICODE

#else
	return CString(szA);
#endif
}

CString MyA2StrT(const char* szA)
{
#ifndef UNICODE
	MyA2StrA(szA);
#else
	return MyA2StrU(szA);
#endif
}

CString MyA2StrA(const char* szA)
{
	CString result(szA);
	return result;
}

CString MyA2StrU(const char* szA)
{
	int cLen = MyA2U(szA, NULL, 0);
	wchar_t* szU = new wchar_t[cLen];
	memset(szU, 0, cLen * sizeof(wchar_t));
	MyA2U(szA, szU, cLen);
	CString result(szU);
	delete[] szU;
	return result;
}

CString MyU2StrA(const wchar_t* szU)
{
	int cLen = MyU2A(szU, NULL, 0);
	char* szA = new char[cLen];
	memset(szA, 0, cLen * sizeof(char));
	MyU2A(szU, szA, cLen);
	CString result(szA);
	delete[] szA;
	return result;
}

CString MyU2StrU(const wchar_t* szU)
{
	return CString(szU);
}

int MyU2A(const wchar_t* szU, char* szA, size_t cnt)
{
	return WideCharToMultiByte(CP_ACP, 0, szU, -1, szA, cnt, NULL, NULL);
}

int MyA2U(const char* szA, wchar_t* szU, size_t cnt)
{
	return MultiByteToWideChar(CP_ACP, 0, szA, -1, szU, cnt);
}

std::wstring MyT2U(LPCTSTR szStr)
{
#ifndef UNICODE
	return MyA2U(szStr);
#else
	return std::wstring(szStr);
#endif
}

std::string MyT2AA(LPCTSTR szStr)
{
#ifndef UNICODE
	return std::string(szStr);
#else
	return MyU2A(szStr);
#endif
}

std::string MyU2A(const wchar_t* szU)
{
	int nRetCode = MyU2A(szU, 0, 0);
	if (0 == nRetCode)
		return std::string();
	std::string str(nRetCode - 1, '\0');
	MyU2A(szU, (char*)(str.c_str()), nRetCode);
	return str;
}

std::wstring MyA2U(const char* szA)
{
	int nRetCode = MyA2U(szA, 0, 0);
	if (0 == nRetCode)
		return std::wstring();
	std::wstring str(nRetCode - 1, '\0');
	MyA2U(szA, (wchar_t*)(str.c_str()), nRetCode);
	return str;
}
