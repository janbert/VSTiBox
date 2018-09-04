/**************************************************************************
*   $Id: Switches.h 41 2012-02-12 16:03:02Z sander $
*
*   Copyright: Sander Straatjes
*
***************************************************************************/

#ifndef SWITCHES_H_INCLUDED
#define SWITCHES_H_INCLUDED

#include "application.h"

#define SWITCH1     1
#define SWITCH2     3
#define SWITCH3     11

void Switches_Init(void);
void PIOINT0_IRQHandler(void);


#endif // SWITCHES_H_INCLUDED
