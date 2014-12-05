#pragma once
class FCIIExecuter;
struct FCIIContent;

class FCII
{
public:
	bool m_isweb;

	FCIIContent* m_receivedcontent = NULL;
	FCIIContent* m_sendcontent = NULL;
public:
	FCII(UINT8* stream, int streamLen, FCIIExecuter* executer);
	~FCII();

	static bool Analyse(UINT8* stream, int streamLen, FCIIContent* content);

	bool ReceiveStream(UINT8* stream, int streamLen);

	void SendStream(UINT8* stream, int* streamLen);
private:
	UINT8* m_receivedstream = NULL;
	UINT8* m_sendedstream = NULL;
	FCIIExecuter* m_executer;
};

const int UI16SIZE = sizeof(UINT16);
const int UI32SIZE = sizeof(UINT32);
const int FCIICONTENTHEADLEN = UI16SIZE * 6 + UI32SIZE;
struct FCIIContent
{
	UINT16	Identify;
	UINT16	CheckSum;
	UINT16	MainCMD;
	UINT16	SubCMD;
	UINT8	IsRequest;
	UINT8	Reserve[3];
	UINT16	ErrCode;
	UINT16	ArgLen;
	UINT32	DataLen;

	UINT8*	Arg;
	UINT8*	Data;
};

class FCIIExecuter
{
public:
	virtual FCIIContent* Dispatch(FCIIContent* content, UINT8* SendedStream);

	virtual bool Login(CString UserID, CString PSW, CString CheckCode);
};