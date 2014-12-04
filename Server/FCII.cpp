#include "stdafx.h"
#include "FCII.h"


FCII::FCII(UINT8* stream, FCIIExecuter* executer)
{
	//判断是不是websocket数据流
	CString tmpStr(stream);
	m_isweb = tmpStr.Left(3) == _T("GET");
	m_executer = executer;
}


FCII::~FCII()
{
	
}

bool FCII::Analyse(UINT8* stream,int streamLen, FCIIContent* content)
{
	content = (FCIIContent*)stream;
	content->Data = content->Arg + content->ArgLen;
	//检查长度
	if (streamLen != content->ArgLen + content->DataLen + sizeof(UINT16)* 6 + sizeof(UINT32))
		return false;
	//检查checkSum
	UINT16 tmpCS = 0;
	for (int i = 0; i < streamLen; i++)
	{
		tmpCS += *(stream + i);
	}
	if (content->CheckSum != tmpCS)
		return false;
	return true;
}

bool FCII::ReceiveStream(UINT8* stream, bool* continueTag)
{
	if (m_isweb)
	{

	}
}

int FCII::SendStream(UINT8* stream, bool* continueTag)
{

	if (m_isweb)
	{

	}
}


FCIIContent* FCIIExecuter::Dispatch(FCIIContent* content)
{

}