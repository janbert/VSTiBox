/**************************************************************************
*   $Id: sup.h 41 2012-02-12 16:03:02Z sander $
*
*   Copyright: Sander Straatjes
*
***************************************************************************/

#ifndef SUP_H_INCLUDED
#define SUP_H_INCLUDED

#include "application.h"




void SUP_init(void);
int8_t SUP_RX_Callback(rfframe_t*, rfevent_t event);
int8_t SUP_TX_Callback(rfframe_t*, rfevent_t event);



#endif // SUP_H_INCLUDED
