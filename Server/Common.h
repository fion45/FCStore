#pragma once


//安全删除指针对象
#ifndef SAFE_DELETEPTR
#  define SAFE_DELETEPTR(p)	if(NULL != (p) ) { delete p; p = NULL; }
#endif

//安全删除数组指针对象
#ifndef SAFE_DELETEARRPTR
#  define SAFE_DELETEARRPTR(arrp)	if(NULL != (arrp) ) { delete[] arrp; arrp = NULL; }
#endif

class Common
{
public:
	Common();
	~Common();
};

