#pragma once
class FCII
{
public:
	bool m_isweb;

	FCIIContent* m_currentcontent;
public:
	FCII(UINT8* stream, FCIIExecuter* executer);
	~FCII();

	static bool Analyse(UINT8* stream, int streamLen, FCIIContent* content);

	bool ReceiveStream(UINT8* stream, bool* continueTag);

	int SendStream(UINT8* stream, bool* continueTag);
private:
	UINT8* m_currentreceive;
	FCIIExecuter* m_executer;
};

struct FCIIContent
{
	UINT16	Identify;
	UINT16	CheckSum;
	UINT16	MainCMD;
	UINT16	SubCMD;
	UINT16	ErrCode;
	UINT16	ArgLen;
	UINT32	DataLen;

	UINT8*	Arg;
	UINT8*	Data;
};

class FCIIExecuter
{
public:
	FCIIContent* Dispatch(FCIIContent* content);

	virtual bool Login(CString UserID, CString PSW, CString CheckCode) = 0;
};