/**************************************************************************
*   $Id: power.h 41 2012-02-12 16:03:02Z sander $
*
*   Copyright: Sander Straatjes
*
***************************************************************************/

#ifndef POWER_H_INCLUDED
#define POWER_H_INCLUDED

#include "application.h"

void ClockInit(void);
void IO_Init2(void);
void remap(void);

void LPMode(void);
void Sleep(void);
void DeepSleep(uint32_t ms);
void WAKEUP11_IRQHandler(void);

void CalWDTCLK(void);


#endif // POWER_H_INCLUDED
