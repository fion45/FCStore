#pragma once
enum EMainCMD
{
	MC_Global = 0x01
};

enum ESubCMD
{
	SC_Login = 0x01
};

struct SWPInteraction
{
	UINT8 Flag;
	UINT8 Flag2;
	UINT16 ExtendedPayloadLen;
	UINT8 SmallPayload[];

};