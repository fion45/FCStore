#include "stdafx.h"
#include "FCII.h"
#include "FCIIDefined.h"
#include "json/json.h"
#include <string>
#include "CharConvert.h"

using namespace std;

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
		string tmpStr = T2AA((LPCTSTR)stream);
		Json::Reader reader;
		Json::Value root;
		if (!reader.parse(tmpStr, root, false))
		{
			return -1;
		}
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
	bool DontDis = false;
	switch (content->MainCMD)
	{
	case MC_Global:
		switch (content->SubCMD)
		{
		case SC_Login:
		{

			break;
		}
		default:
			DontDis = true;
			break;
		}
		break;
	default:
		DontDis = true;
		break;
	}
	if (DontDis)
	{
		//没有派送
	}
}