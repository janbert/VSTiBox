/**************************************************************************
*   $Id: Switches.c 41 2012-02-12 16:03:02Z sander $
*
*   Copyright: Sander Straatjes
*
***************************************************************************/

#include "Switches.h"


void PIOINT0_IRQHandler(void)
{
    uint32_t regVal;

    regVal = LPC_GPIO0->MIS;
    if ( regVal & (1<<SWITCH1) )
    {
        printf("SW1\n");
    }
    if ( regVal & (1<<SWITCH2) )
    {
        printf("SW2\n");
    }
    if ( regVal & (1<<SWITCH3) )
    {
        printf("SW3\n");
    }

    LPC_GPIO0->IC |= regVal;
    return;
}

void Switches_Init(void)
{
    NVIC_EnableIRQ(EINT0_IRQn);
    IOCONPWR(1);

    // SW1 setup
    LPC_IOCON->PIO0_1 = (0<<0)|(2<<3)|(1<<6)|(1<<7); // Function PIO0_1, with PU, digital mode // SW1
    GPIOSetDir(0,SWITCH1,0);
    GPIOSetInterrupt(0,SWITCH1,0,0,0);
    GPIOIntEnable(0,SWITCH1);

    // SW2 setup
    LPC_IOCON->PIO0_3 = (0<<0)|(2<<3)|(1<<6)|(1<<7); // Function PIO0_3, with PU, digital mode // SW1
    GPIOSetDir(0,SWITCH2,0);
    GPIOSetInterrupt(0,SWITCH2,0,0,0);
    GPIOIntEnable(0,SWITCH2);

    // SW3 setup
    LPC_IOCON->R_PIO0_11 = (1<<0)|(2<<3)|(1<<6)|(1<<7); // Function PIO0_11, with PU, digital mode // SW1
    GPIOSetDir(0,SWITCH3,0);
    GPIOSetInterrupt(0,SWITCH3,0,0,0);
    GPIOIntEnable(0,SWITCH3);

    IOCONPWR(0);
}
