//****************************************************************************************************************
//****                                                 宏定义模块 
//****                                           遵循QS DEPT 开发标准
//****                                              开发人:YangLei
//****                                          最后修改日期:2009-06-20
//****************************************************************************************************************
#ifndef __DEFINE_H__
#define __DEFINE_H__
       
typedef unsigned char  INT8U;          							/* Unsigned  8 bit quantity 					*/
typedef signed   char  INT8S;          							/* Signed    8 bit quantity 					*/
typedef unsigned short INT16U;         							/* Unsigned 16 bit quantity 					*/
typedef signed   short INT16S;         							/* Signed   16 bit quantity 					*/
typedef unsigned int   INT32U;         							/* Unsigned 32 bit quantity 					*/
typedef signed   int   INT32S;         							/* Signed   32 bit quantity 					*/
typedef unsigned long  INT64U;

#define VALID(h)						( (h) != 0 && (h) != INVALID_HANDLE_VALUE )
#define INVALID_SIGN_THREAD_ID          0xFFFFFF00
#endif

