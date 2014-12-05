#include "stdafx.h"
#include "FCII.h"
#include "FCIIDefined.h"
#include "json/json.h"
#include <string>
#include "CharConvert.h"

using namespace std;

FCII::FCII(UINT8* stream, int streamLen, FCIIExecuter* executer)
{
	//判断是不是websocket数据流
	string tmpStr((char*)stream);
	m_isweb = tmpStr.substr(0, 3) == "GET";
	m_executer = executer;
	if (m_isweb)
	{
		//先进行握手
	}
	else
	{
		ReceiveStream(stream, streamLen);
	}
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

bool FCII::ReceiveStream(UINT8* stream, int streamLen)
{
	SAFE_DELETEARRPTR(m_receivedstream);
	SAFE_DELETEARRPTR(m_sendedstream);
	SAFE_DELETEARRPTR(m_receivedstream);

	if (m_isweb)
	{
		string tmpStr = MyT2AA((LPCTSTR)stream);
		Json::Reader reader;
		Json::Value root;
		if (!reader.parse(tmpStr, root, false))
		{
			//解析失败
			return false;
		}
		Json::Value argLen = root["ArgLen"];
		Json::Value dataLen = root["DataLen"];
		streamLen = FCIICONTENTHEADLEN + argLen.asUInt() + dataLen.asUInt();
		m_receivedstream = new UINT8[streamLen];
		UINT8* streamPtr = m_receivedstream;
		*((UINT16*)streamPtr) = (UINT16)(root["Identify"].asUInt());
		streamPtr += UI16SIZE;
		*((UINT16*)streamPtr) = (UINT16)(root["CheckSum"].asUInt());
		streamPtr += UI16SIZE;
		*((UINT16*)streamPtr) = (UINT16)(root["MainCMD"].asUInt());
		streamPtr += UI16SIZE;
		*((UINT16*)streamPtr) = (UINT16)(root["SubCMD"].asUInt());
		streamPtr += UI16SIZE;
		*((UINT16*)streamPtr) = (UINT16)(root["ErrCode"].asUInt());
		streamPtr += UI16SIZE;
		*((UINT16*)streamPtr) = (UINT16)(root["ArgLen"].asUInt());
		streamPtr += UI16SIZE;
		*((UINT16*)streamPtr) = (UINT16)(root["DataLen"].asUInt());
		streamPtr += UI32SIZE;
		tmpStr = root["Arg"].asString();
		UINT32 tmpLen = (UINT16)(root["ArgLen"].asUInt());
		CopyMemory(streamPtr, tmpStr.data(), tmpLen);
		streamPtr += tmpLen;
		tmpStr = root["Data"].asString();
		tmpLen = (UINT16)(root["DataLen"].asUInt());
		CopyMemory(streamPtr, tmpStr.data(), tmpLen);
	}
	else
	{
		m_receivedstream = new UINT8[streamLen];
		CopyMemory(m_receivedstream, stream, streamLen);
	}
	return Analyse(stream, streamLen, m_receivedcontent);
}

void FCII::SendStream(UINT8* stream, int* streamLen)
{
	m_sendcontent = m_executer->Dispatch(m_receivedcontent, m_sendedstream);
	if (m_isweb)
	{
		string tmpStr = "";
		char buf[100];
		ZeroMemory(buf, 100);
		sprintf_s(buf, 100, "{'Identify':%d,", m_sendcontent->Identify);
		tmpStr.append(buf);
		ZeroMemory(buf, 100);
		sprintf_s(buf, 100, "'CheckSum':%d,", m_sendcontent->CheckSum);
		tmpStr.append(buf);
		ZeroMemory(buf, 100);
		sprintf_s(buf, 100, "'MainCMD':%d,", m_sendcontent->MainCMD);
		tmpStr.append(buf);
		ZeroMemory(buf, 100);
		sprintf_s(buf, 100, "'SubCMD':%d,", m_sendcontent->SubCMD);
		tmpStr.append(buf);
		ZeroMemory(buf, 100);
		sprintf_s(buf, 100, "'ErrCode':%d,", m_sendcontent->ErrCode);
		tmpStr.append(buf);
		ZeroMemory(buf, 100);
		sprintf_s(buf, 100, "'ArgLen':%d,", m_sendcontent->ArgLen);
		tmpStr.append(buf);
		ZeroMemory(buf, 100);
		sprintf_s(buf, 100, "'DataLen':%d,", m_sendcontent->DataLen);
		tmpStr.append(buf);
		tmpStr.append("'Arg':'");
		int tmpLen = m_sendcontent->ArgLen + 1;
		char* bufPtr = new char[tmpLen];
		ZeroMemory(bufPtr, tmpLen);
		CopyMemory(bufPtr, m_sendcontent->Arg, m_sendcontent->ArgLen);
		tmpStr.append(bufPtr);
		delete[] bufPtr;
		tmpStr.append("','Data':'");

		tmpLen = m_sendcontent->DataLen + 1;
		bufPtr = new char[tmpLen];
		ZeroMemory(bufPtr, tmpLen);
		CopyMemory(bufPtr, m_sendcontent->Data, m_sendcontent->DataLen);
		tmpStr.append(bufPtr);
		delete[] bufPtr;
		tmpStr.append("'}");
		stream = new UINT8[tmpStr.length];
		CopyMemory(stream, tmpStr.data(), tmpStr.length);
		*streamLen = tmpStr.length;
	}
	else
	{
		*streamLen = m_sendcontent->ArgLen + m_sendcontent->DataLen + FCIICONTENTHEADLEN;
		stream = new UINT8[*streamLen];
		CopyMemory(stream, m_sendedstream, *streamLen);
	}
}


FCIIContent* FCIIExecuter::Dispatch(FCIIContent* content, UINT8* SendedStream)
{
	FCIIContent* result = NULL;
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
	return result;
}