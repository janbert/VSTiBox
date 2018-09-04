/**************************************************************************
*   $Id: rftypes.h 41 2012-02-12 16:03:02Z sander $
*
*   Copyright: Sander Straatjes
*
***************************************************************************/

#ifndef RFTYPES_H_INCLUDED
#define RFTYPES_H_INCLUDED

typedef enum
{
	eComplete,
	eTimeout,
	eCrcError,
	eIRQError
}rfevent_t;

typedef enum
{
	RX,
	TX,
	PD
}rfmode_t;

typedef struct
{
    uint8_t netid;
    uint8_t len;
    uint8_t crcl;
    uint8_t data[22];
    uint8_t crc;
    uint8_t state;
}rfframe_t;

typedef int8_t (*rfcallback_t)(rfframe_t*, rfevent_t);

typedef struct
{
    uint8_t enable;
    uint16_t dt;
    uint16_t match;
    rfcallback_t callback;
}rftimeout_t;




#endif // RFTYPES_H_INCLUDED
