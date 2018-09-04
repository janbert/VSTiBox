/**************************************************************************
*   $Id: analog.h 41 2012-02-12 16:03:02Z sander $
*
*   Copyright: Sander Straatjes
*
***************************************************************************/

#ifndef ANALOG_H_INCLUDED
#define ANALOG_H_INCLUDED

#include "application.h"

uint32_t ADCRead( uint8_t channelNum, uint32_t samples );
void Init_ADC(void);
void Disable_ADC(void);
uint32_t Temp1(void);


#endif // ANALOG_H_INCLUDED
