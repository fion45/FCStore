#pragma once


//��ȫɾ��ָ�����
#ifndef SAFE_DELETEPTR
#  define SAFE_DELETEPTR(p)	if(NULL != (p) ) { delete p; p = NULL; }
#endif

//��ȫɾ������ָ�����
#ifndef SAFE_DELETEARRPTR
#  define SAFE_DELETEARRPTR(arrp)	if(NULL != (arrp) ) { delete[] arrp; arrp = NULL; }
#endif

class Common
{
public:
	Common();
	~Common();
};

