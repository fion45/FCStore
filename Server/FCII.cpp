#include "stdafx.h"
#include "FCII.h"
#include "FCIIDefined.h"
#include "json/json.h"
#include <string>
#include "CharConvert.h"
#include "sha1.h"

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
		//获得Sec-WebSocket-Key值
		basic_string <char>::size_type bindex = tmpStr.find_first_of("Sec-WebSocket-Key:") + strlen("Sec-WebSocket-Key:");
		basic_string <char>::size_type eindex = tmpStr.find_first_of("Sec-WebSocket-Vers:");
		tmpStr = tmpStr.substr(bindex, eindex - bindex);
		tmpStr.append("258EAFA5-E914-47DA-95CA-C5AB0DC85B11");
		//SHA1加密
		SHA1 sha1;
		char buffer[41];
		sha1.SHA_GO(tmpStr.data(), buffer);
		//构造数据包发送
		string BuildBuf = "HTTP/1.1 101 Switching Protocols\r\nConnection:Upgrade\r\nServer:beetle websocket server\r\nUpgrade:WebSocket\r\nDate:Mon, 26 Nov 2012 23:42:44 GMT\r\nAccess-Control-Allow-Credentials:true\r\nAccess-Control-Allow-Headers:content-type\r\nSec-WebSocket-Accept:";
		BuildBuf.append(buffer);
		m_hasbuilded = true;
		m_sendedstream = new UINT8[BuildBuf.length()];
		CopyMemory(m_sendedstream, BuildBuf.data(), BuildBuf.length());
	}
	else
	{
		m_hasbuilded = false;
		ReceiveStream(stream, streamLen);
	}
}


FCII::~FCII()
{
	
}

bool FCII::Analyse(UINT8* stream,int streamLen, FCIIContent* content)
{
	content = (FCIIContent*)stream;
	//检查长度
	if (streamLen != content->DataLen + sizeof(UINT16)* 6 + sizeof(UINT32))
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
	m_hasbuilded = false;
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
		Json::Value dataLen = root["DataLen"];
		streamLen = FCIICONTENTHEADLEN + dataLen.asUInt();
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
		*((UINT16*)streamPtr) = (UINT16)(root["IsRequest"].asUInt());
		streamPtr += UI16SIZE;
		*((UINT16*)streamPtr) = (UINT16)(root["ErrCode"].asUInt());
		streamPtr += UI16SIZE;
		*((UINT16*)streamPtr) = (UINT16)(root["DataLen"].asUInt());
		streamPtr += UI32SIZE;
		tmpStr = root["Data"].asString();
		UINT32 tmpLen = (UINT32)(root["DataLen"].asUInt());
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
	m_sendedcontent = m_executer->Dispatch(m_receivedcontent, m_sendedstream);
	if (m_isweb)
	{
		string tmpStr = "";
		char buf[100];
		ZeroMemory(buf, 100);
		sprintf_s(buf, 100, "{'Identify':%d,", m_sendedcontent->Identify);
		tmpStr.append(buf);
		ZeroMemory(buf, 100);
		sprintf_s(buf, 100, "'CheckSum':%d,", m_sendedcontent->CheckSum);
		tmpStr.append(buf);
		ZeroMemory(buf, 100);
		sprintf_s(buf, 100, "'MainCMD':%d,", m_sendedcontent->MainCMD);
		tmpStr.append(buf);
		ZeroMemory(buf, 100);
		sprintf_s(buf, 100, "'SubCMD':%d,", m_sendedcontent->SubCMD);
		tmpStr.append(buf);
		ZeroMemory(buf, 100);
		sprintf_s(buf, 100, "'IsRequest':%d,", m_sendedcontent->IsRequest);
		tmpStr.append(buf);
		ZeroMemory(buf, 100);
		sprintf_s(buf, 100, "'ErrCode':%d,", m_sendedcontent->ErrCode);
		tmpStr.append(buf);
		ZeroMemory(buf, 100);
		sprintf_s(buf, 100, "'DataLen':%d,", m_sendedcontent->DataLen);
		tmpStr.append(buf);
		tmpStr.append("'Data':'");
		int tmpLen = m_sendedcontent->DataLen + 1;
		char* bufPtr = new char[tmpLen];
		ZeroMemory(bufPtr, tmpLen);
		CopyMemory(bufPtr, m_sendedcontent->Data, m_sendedcontent->DataLen);
		tmpStr.append(bufPtr);
		delete[] bufPtr;
		tmpStr.append("'}");
		stream = new UINT8[tmpStr.length()];
		CopyMemory(stream, tmpStr.data(), tmpStr.length());
		*streamLen = tmpStr.length();
	}
	else
	{
		*streamLen = FCIICONTENTHEADLEN + m_sendedcontent->DataLen;
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