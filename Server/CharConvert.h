#include <string>

CString MyU2StrT(const wchar_t* szA);

CString MyA2StrT(const char* szA);

CString MyA2StrA(const char* szA);

CString MyA2StrU(const char* szA);

CString MyU2StrA(const wchar_t* szU);

CString MyU2StrU(const wchar_t* szU);

int MyU2A(const wchar_t* szU, char* szA, size_t cnt);

int MyA2U(const char* szA, wchar_t* szU, size_t cnt);

std::wstring MyT2U(LPCTSTR szStr);

std::string MyT2AA(LPCTSTR szStr);

std::string MyU2A(const wchar_t* szU);

std::wstring MyA2U(const char* szA);