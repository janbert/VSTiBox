/**************************************************************************
*   $Id: timeout.h 41 2012-02-12 16:03:02Z sander $
*
*   Copyright: Sander Straatjes
*
***************************************************************************/

#ifndef TIMEOUT_H_INCLUDED
#define TIMEOUT_H_INCLUDED

#include "application.h"

#define RFTMR LPC_TMR16B1

void RF_TMR_Init(void);
void RF_TMR_SetMatch(rfmode_t mode, rfcallback_t callback, uint16_t timeout);
void RF_TMR_Start(rfmode_t mode);
void RF_TMR_Feed(rfmode_t mode);
void RF_TMR_Stop(rfmode_t mode);
void TIMER16_1_IRQHandler(void);


#endif // TIMEOUT_H_INCLUDED
