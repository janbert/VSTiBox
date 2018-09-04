/**************************************************************************
*   $Id: power.c 41 2012-02-12 16:03:02Z sander $
*
*   Copyright: Sander Straatjes
*
***************************************************************************/

#include "power.h"

uint32_t WDTOSC_freq = 0;
uint32_t CLKCONFIG = 0;

void LPMode(void)
{
 	//                              sys	rom		ram	 flashreg flasha gpio	iocon
    LPC_SYSCON->SYSAHBCLKCTRL &= ~((0<<0)|(1<<1)|(0<<2)|(1<<3)|(0<<4)|(0<<6)|(1<<16));
    LPC_SYSCON->PDRUNCFG |= (1<<3); // powerdown BOD
#if 0
	LPC_SYSCON->SYSAHBCLKCTRL &= ~(1<<4);
	LPC_SYSCON->PDRUNCFG |= (1<<2);
#endif
}

void Sleep(void)
{
    //		clksetup = LPC_SYSCON->SYSAHBCLKCTRL;
    //LPC_SYSCON->SYSAHBCLKCTRL = ((1<<0)|(1<<2)); // only leave the most needed enabled
    //LPC_SYSCON->SYSAHBCLKDIV  = 8;	   //divide by 1
    //LPC_SYSCON->PDRUNCFG     = ((1<<2)|(1<<3)|(1<<4)|(1<<5)|(1<<6)|(1<<7)|(1<<11)|(1<<12));
    GPIO_Masked(304,0);
    __WFI();
    GPIO_Masked(304,1);

    //LPC_SYSCON->PDRUNCFG     = ((1<<2)|(1<<3)|(1<<4)|(1<<5)|(1<<6)|(1<<7)|(1 << 8)|(1<<10)|(1 << 11)|(1 << 13)|(1 << 14)|(1 << 15));
    //LPC_SYSCON->SYSAHBCLKDIV  = 1;	   //divide by 1
    //LPC_SYSCON->SYSAHBCLKCTRL = clksetup;
}

void DeepSleep(uint32_t ms)
{
    IOCONPWR(1);

    LPC_SYSCON->SYSAHBCLKCTRL |= (1<<9); // enable clock to CT32B0
    LPC_SYSCON->WDTOSCCTRL = (0x1<<5) | (0x1F<<0);     //500k/64=7.8kHz, real 8.6kHz
    LPC_SYSCON->PDRUNCFG     &= ~(1 << 6);          /* Power-up WDTCLK      */

    // SW3 setup
    LPC_IOCON->R_PIO0_11 = (3<<0)|(2<<3)|(1<<6)|(1<<7)|(1<<10); // Function CT32B0_MAT3, no PU/PD, digital mode // SW1
    //GPIOIntDisable(0,SWITCH3);

    LPC_TMR32B0->PR  = 0;
    LPC_TMR32B0->TCR = (1<<1);	// reset counter and prescaler
    LPC_TMR32B0->CTCR = (0<<0)|(0<<2); // Timer mode
    LPC_TMR32B0->MR3 = (((WDTOSC_freq/64)*ms)/1000);
    LPC_TMR32B0->MCR = ((1<<10)|(1<<11)); // reset and stop on MR3
    LPC_TMR32B0->EMR = ((1<<3)|(1<<10));  // clr MR3 on match

    LPC_SYSCON->PDSLEEPCFG = 0x000018BF; //WDT ON, BOD OFF

    LPC_SYSCON->PDAWAKECFG = LPC_SYSCON->PDRUNCFG;      // Configure PDAWAKECFG to restore PDRUNCFG on wake up

    LPC_PMU->PCON=0;

    /* Configure Wakeup I/O */
    /* Specify the start logic to allow the chip to be waken up using PIO0_8 */
    LPC_SYSCON->STARTAPRP0          |=  ((0<<11)|(0<<1)|(0<<3)); // falling edge
    LPC_SYSCON->STARTRSRP0CLR       =   ((1<<11)|(1<<1)|(1<<3)); // Clear pending bit
    LPC_SYSCON->STARTERP0           |=  ((1<<11)|(1<<1)|(1<<3)); // Enable Start Logic
    NVIC_EnableIRQ(WAKEUP1_IRQn);
    NVIC_EnableIRQ(WAKEUP3_IRQn);
    NVIC_EnableIRQ(WAKEUP11_IRQn);

    CLKCONFIG = LPC_SYSCON->SYSAHBCLKCTRL; // save state off configuration
    LPC_SYSCON->SYSAHBCLKCTRL = ((1<<0)|(1<<2)|(1<<9)); // only leave the most needed enabled

    SCB->SCR |= (1<<2); // set SLEEPDEEP bit

    // Switch main clock to low-speed WDO
    LPC_SYSCON->MAINCLKSEL = 2;
    LPC_SYSCON->MAINCLKUEN = 0;
    LPC_SYSCON->MAINCLKUEN = 1; // toggle to enable
    LPC_SYSCON->MAINCLKUEN = 0;

    // Preload clock selection for quick switch back to IRC on wakeup
    LPC_SYSCON->MAINCLKSEL = 0;
    LPC_SYSCON->MAINCLKUEN = 0;

    LPC_TMR32B0->TCR = (1<<0);	// Enable timer and release reset

    __WFI();
}

void WAKEUP11_IRQHandler(void)
{
    LPC_SYSCON->MAINCLKUEN = 1; // toggle to enable the preset mainclk
    LPC_SYSCON->MAINCLKUEN = 0;

    LPC_SYSCON->STARTRSRP0CLR       =   ((1<<11)|(1<<1)|(1<<3)); // Clear pending bit on start logic

    // Restore clocks to chip modules
    LPC_SYSCON->SYSAHBCLKCTRL = CLKCONFIG;

    SCB->SCR &= ~(1<<2); // Clear SLEEPDEEP bit so MCU will enter Sleep mode on __WFI();

    LPC_TMR32B0->EMR = ((1<<3)|(1<<10));  // clr MR3 on match

    IOCONPWR(1);
    LPC_IOCON->R_PIO0_11 = (1<<0)|(2<<3)|(1<<6)|(1<<7); // Function PIO0_11, with PU, digital mode // SW1
    //LPC_GPIO0->IC |= (1<<SWITCH3);
    //GPIOIntEnable(0,SWITCH3);
    IOCONPWR(0);

    LPC_SYSCON->STARTERP0           &=  ~((1<<11)|(1<<1)|(1<<3)); // Enable Start Logic
}

// remap interrupt vectors to SRAM
void remap(void)
{
    uint32_t i=0;
    volatile uint32_t *src=0;
    volatile uint32_t *dst=0;

    __disable_irq();

    dst = (volatile uint32_t*)0x10000000;
    src = (volatile uint32_t*)0x0;

    for (i = 0; i < 512/4; i++)
    {
        dst[i] = src[i];
    }

    LPC_SYSCON->SYSMEMREMAP = 0x01;

    __enable_irq();
}

void CalWDTCLK(void)
{
    uint32_t i=0, timerval=0;
    uint32_t start=0, end=0;

    LPC_SYSCON->WDTOSCCTRL = (0x1<<5) | (0x0<<0);     //500k/2=250kHz, real 275.6kHz
    LPC_SYSCON->PDRUNCFG     &= ~(1 << 6);          /* Power-up WDTCLK      */
    for (i = 0; i < 200; i++) __NOP();

    LPC_SYSCON->SYSAHBCLKCTRL |= ((1<<9) | (1<<15)); // enable clock to CT32B0 and WDT

    LPC_SYSCON->WDTCLKSEL = 0x2; //select wdt oscillator

    LPC_SYSCON->WDTCLKUEN = 0;  // Arm WDT clock selection update
    LPC_SYSCON->WDTCLKUEN = 1;  // Update WDT clock selection
    LPC_SYSCON->WDTCLKUEN = 0;  // Arm WDT clock selection update
    LPC_SYSCON->WDTCLKDIV = 1;  // WDT clock divide by 1

    LPC_WDT->TC = 0xFFFFFF;   // big value, WDT timer duration = don't care
    LPC_WDT->MOD = 1;           // enable WDT for counting/interrupts but not resets
    LPC_WDT->FEED = 0xAA;       // WDT start sequence step 1
    LPC_WDT->FEED = 0x55;       // WDT start sequence step 2

    LPC_TMR32B0->PR  = 0;
    LPC_TMR32B0->TCR = (1<<1);	// reset counter and prescaler
    LPC_TMR32B0->CTCR = (0<<0)|(0<<2); // Timer mode

    LPC_TMR32B0->TCR = (1<<0);	// Enable timer and release reset

    __disable_irq(); // interrupts can offset the calibration

    timerval = LPC_TMR32B0->TC + ( _IRC_CLK / 10 );
    while(timerval >= LPC_TMR32B0->TC)
    {
        __NOP();
    }

    start = LPC_WDT->TV;

    timerval = LPC_TMR32B0->TC + ( _IRC_CLK / 10 );
    while(timerval>=LPC_TMR32B0->TC)
    {
        __NOP();
    }

    end = LPC_WDT->TV;

    LPC_SYSCON->WDTCLKDIV = 0;  // WDT clock divide by 1
    LPC_SYSCON->SYSAHBCLKCTRL &= ~((1<<9) | (1<<15)); // disable clock to CT16B0 and WDT

    LPC_SYSCON->WDTOSCCTRL = (0x1<<5) | (0x1F<<0);     //500k/64=7.8kHz, real 8.6kHz

    __enable_irq();

    WDTOSC_freq = (start-end)*80;
#if __DEBUG__
    printf("WDT:%u\n",WDTOSC_freq);
#endif

    LPC_SYSCON->PDRUNCFG     |= (1 << 6);          /* Power-down WDTCLK      */
}

void ClockInit(void)
{
    uint32_t i=31;

    LPC_SYSCON->WDTOSCCTRL = (0x1<<5) | (0x1F<<0);     //500k/64=7.8kHz, real 8.6kHz
    LPC_SYSCON->PDRUNCFG     &= ~(1 << 6);          /* Power-up WDTCLK      */
    for (i = 0; i < 200; i++) __NOP();

    LPC_SYSCON->MAINCLKSEL    = 0;     				/* Select IRC Clock*/
    LPC_SYSCON->MAINCLKUEN    = 0x01;               /* Update MCLK Clock Source */
    LPC_SYSCON->MAINCLKUEN    = 0x00;               /* Toggle Update Register   */
    LPC_SYSCON->MAINCLKUEN    = 0x01;
    while (!(LPC_SYSCON->MAINCLKUEN & 0x01));       /* Wait Until Updated       */


    LPC_SYSCON->SYSAHBCLKDIV  = 1;	   //divide by 1
    //                           sys	rom		ram	 flashreg flasha gpio	iocon
    LPC_SYSCON->SYSAHBCLKCTRL = (1<<0)|(1<<1)|(1<<2)|(1<<3)|(1<<4)|(1<<6)|(1<<16);
    LPC_SYSCON->UARTCLKDIV    = 0;	   //divide by 1
    LPC_SYSCON->SSP0CLKDIV    = 0;
    LPC_SYSCON->SSP1CLKDIV    = 0;
}

void IO_Init2(void)
{
#if !__DEBUG__
    LPC_IOCON->SWDIO_PIO1_3		= 0xC1;		// ARM_SWD/P1_3/ADCIN4/CT32B1_MAT2
    LPC_IOCON->SWCLK_PIO0_10	= 0xC1;		// JTAG_CLK/P0_10/SSP_CLK/CT16B0_MAT2
    LPC_IOCON->RESET_PIO0_0 	= 0xC1;      // RST_BOOT/P0_0
#endif

////#if !ENABLE_TIMER
//    /* PIO0_8 needs to be configured to generate a Match output */
//    LPC_IOCON->PIO0_8			= 0xC0;		// SSP_MISO/CT16B0_MAT0/TRACE_CLK
////#endif /* ENABLE_TIMER */
//
////#if !ENABLE_CLKOUT
//    LPC_IOCON->PIO0_1			= 0xC0;      // CLKOUT/CT32B0_MAT2/USB_SOF
////#endif /* ENABLE_CLKOUT */
//
//    LPC_IOCON->PIO2_6			= 0xC0;
//    LPC_IOCON->PIO2_0			= 0xC0;
//    LPC_IOCON->PIO1_8			= 0xC0;
//    LPC_IOCON->PIO0_2			= 0xC0;      // SSEL/CT16B0_CAP0
//    LPC_IOCON->PIO2_7			= 0xC0;
//    LPC_IOCON->PIO2_8			= 0xC0;
//    LPC_IOCON->PIO2_1			= 0xC0;
//    LPC_IOCON->PIO0_3			= 0xC0;      // USB VBUS
//    LPC_IOCON->PIO0_4			= 0xC0;      // I2C_SCL, no pull-up, inactive
//    LPC_IOCON->PIO0_5			= 0xC0;      // I2C_SDA, no pull-up, inactive
//    LPC_IOCON->PIO1_9			= 0xC0;      // CT16B1_MAT0
//    LPC_IOCON->PIO3_4			= 0xC0;
//    LPC_IOCON->PIO2_4			= 0xC0;
//    LPC_IOCON->PIO2_5			= 0xC0;
//    LPC_IOCON->PIO3_5			= 0xC0;
//    LPC_IOCON->PIO0_6			= 0xC0;      // USB SoftConnect
//    LPC_IOCON->PIO0_7			= 0xC0;
//    LPC_IOCON->PIO2_9			= 0xC0;
//    LPC_IOCON->PIO2_10			= 0xC0;
//    LPC_IOCON->PIO2_2			= 0xC0;
//    LPC_IOCON->PIO0_9			= 0xC0;		// SSP_MOSI/CT16B0_MAT1/TRACE_SWV
//    LPC_IOCON->PIO1_10			= 0xC0;		// ADCIN6/CT16B1_MAT1
//    LPC_IOCON->PIO2_11			= 0xC0;
//    LPC_IOCON->R_PIO0_11		= 0xC1;		// JTAG_TDI/P0_11/ADCIN0/CT32B0_MAT3
//    LPC_IOCON->R_PIO1_0 		= 0xC1;		// JTAG_TMS/P1_0/ADCIN1/CT32B1_CAP0
//    LPC_IOCON->R_PIO1_1  		= 0xC1;		// JTAG_TDO/P1_1/ADCIN2/CT32B1_MAT0
//    LPC_IOCON->R_PIO1_2 		= 0xC1;		// JTAG_TRST/P1_2/ADCIN3/CT32B1_MAT1
//    LPC_IOCON->PIO3_0			= 0xC0;
//    LPC_IOCON->PIO3_1			= 0xC0;
//    LPC_IOCON->PIO2_3			= 0xC0;
//    LPC_IOCON->PIO1_4			= 0xC0;		// ADCIN5/CT32B1_MAT3/WAKEUP
//    LPC_IOCON->PIO1_11			= 0xC0;		// ADCIN7
//    LPC_IOCON->PIO3_2			= 0xC0;
//    LPC_IOCON->PIO1_5			= 0xC0;      // UART_DIR/CT32B0_CAP0
//    LPC_IOCON->PIO1_6			= 0xC0;      // UART_RXD/CT32B0_MAT0
//    LPC_IOCON->PIO1_7			= 0xC0;      // UART_TXD/CT32B0_MAT1
//    LPC_IOCON->PIO3_3			= 0xC0;


//    LPC_GPIO0->DIR =\
//                    LPC_GPIO1->DIR =\
//                                    LPC_GPIO2->DIR =\
//                                            LPC_GPIO3->DIR = 0xFFFFFFFF;
//
//    LPC_GPIO0->DATA  =\
//                      LPC_GPIO1->DATA  =\
//                                        LPC_GPIO2->DATA  =\
//                                                LPC_GPIO3->DATA  = 0;
//
//    GPIOSetDir(2,10,1); // top led  Yellow
//    GPIOSetDir(2,7,1); // middle led RED
//    GPIOSetDir(2,8,1); // bottom led  Yellow
//    GPIOSetDir(2,4,1); // CAN H pin
//    GPIOSetDir(3,4,1); // CAN L pin
}
