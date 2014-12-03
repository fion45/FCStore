#pragma once
class FCII
{
public:
	bool m_isweb;

public:
	FCII();
	~FCII();
};

struct FCIIContent
{
	UINT16	Identify;
	UINT16	Code;
	UINT16	MainCMD;
	UINT16	SubCMD;
	UINT16	ArgLen;
	UINT32	DataLen;
	UINT8*	ArgContent;
	UINT8*	DataContent;
};