/**************************************************************************
*   $Id: application.h 41 2012-02-12 16:03:02Z sander $
*
*   Copyright: Sander Straatjes
*
***************************************************************************/

#ifndef APPLICATION_H_INCLUDED
#define APPLICATION_H_INCLUDED


//hardware usage:
// LPC_TMR32B0              -> WDTCLK cal. + Deep Sleep wake-up
// LPC_TMR32B1              -> Motor PWM
// LPC_TMR16B0
// LPC_TMR16B1              -> RFM12B
// Port 0 interrupts        -> Switches
// Systick                  -> System timer + Pos timer
// UART                     -> Debug UART
// SSP0
// SSP1                     -> RFM12B
// I2C                      -> SE95
// ADC                      -> Temperature + Vbat
// CAN
// WDT

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include "LPC11xx.h"
#include "type.h"
#include "gpio.h"
#include "i2c.h"
#include "lm75a.h"

#include "analog.h"
//#include "power.h"
#include "uart.h"

#define XTAL_CLK     (10000000UL)    /* XTAL oscillator frequency */
#define PLL_CLK      (50000000UL)    /* PLL frequency */
#define SystemClock PLL_CLK



#endif // APPLICATION_H_INCLUDED
