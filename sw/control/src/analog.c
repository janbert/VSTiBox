/**************************************************************************
*   $Id: analog.c 41 2012-02-12 16:03:02Z sander $
*
*   Copyright: Sander Straatjes
*
***************************************************************************/

#include "analog.h"

void Init_ADC(void)
{
    LPC_SYSCON->PDRUNCFG &= ~(0x1<<4); // Disable Power down bit to the ADC block.
    LPC_SYSCON->SYSAHBCLKCTRL |= (1<<13);  // Enable AHB clock to the ADC.

    LPC_IOCON->R_PIO1_0    = 0x02;	// Select AD1 pin function
    LPC_IOCON->R_PIO1_1    = 0x02;	// Select AD2 pin function
    LPC_IOCON->PIO1_11   = 0x01;	// Select AD7 pin function

    LPC_ADC->CR = ((3-1)<<8); // ADC CLK = 12/3 = 4 MHz = 2.75 uS/conversion

}

void Disable_ADC(void)
{
    LPC_IOCON->R_PIO1_0 = (1<<0)|(0<<3)|(1<<6)|(1<<7); // Function GPIO, no PU/PD, digital mode
    LPC_IOCON->R_PIO1_1 = (1<<0)|(0<<3)|(1<<6)|(1<<7); // Function GPIO, no PU/PD, digital mode
    LPC_IOCON->PIO1_11 = (0<<0)|(0<<3)|(1<<6)|(1<<7); // Function GPIO, no PU/PD, digital mode

    LPC_SYSCON->SYSAHBCLKCTRL &= ~(1<<13);  // Disable AHB clock to the ADC.
    LPC_SYSCON->PDRUNCFG |= (0x1<<4); // Disable Power down bit to the ADC block.
}

uint32_t ADCRead( uint8_t channelNum, uint32_t samples )
{
    uint32_t i=0;
    uint32_t sum = 0;

    Init_ADC();

    LPC_ADC->INTEN = 0;

    LPC_ADC->CR &= 0xFFFFFF00;
    LPC_ADC->CR |= (1 << 16) | (1 << channelNum);

    for(i=0; i<samples; i++)
    {
        while(!(LPC_ADC->DR[channelNum] & (1UL<<31)));
        sum += ((LPC_ADC->DR[channelNum]>>6) & 0x3FF);
    }

    LPC_ADC->CR &= (1<<16);

    Disable_ADC();

    return(sum);
}

uint32_t Temp1(void)
{
    uint32_t result = 0;
    uint32_t resistor = 0;
    uint32_t temp = 0;
    int32_t offset = -590000;

    GPIOSetDir(1,5,1); // PTC power
    LPC_IOCON->PIO1_5 = (0<<0)|(0<<3)|(1<<6)|(1<<7); // Function GPIO, no PU/PD, digital mode
    LPC_GPIO1->DATA |= (1<<5);

    for(result=0; result<1000; result++)__NOP();

    result = ADCRead(1,10);

    resistor = (uint32_t)(225280000  /  result)-22000;
    temp = (resistor*1209-9789700+offset)/10000;

    LPC_GPIO1->DATA &= ~(1<<5);

    return(temp);
}
