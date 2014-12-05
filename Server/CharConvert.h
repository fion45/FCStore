#include <string>

CString U2StrT(const wchar_t* szA);

CString A2StrT(const char* szA);

CString A2StrA(const char* szA);

CString A2StrU(const char* szA);

CString U2StrA(const wchar_t* szU);

CString U2StrU(const wchar_t* szU);

int U2A(const wchar_t* szU,char* szA,size_t cnt);

int A2U(const char* szA,wchar_t* szU,size_t cnt);

std::wstring T2U(LPCTSTR szStr);

std::string T2AA(LPCTSTR szStr);

std::string U2A(const wchar_t* szU);

std::wstring A2U(const char* szA);