/**************************************************************************
*   $Id: timeout.c 41 2012-02-12 16:03:02Z sander $
*
*   Copyright: Sander Straatjes
*
***************************************************************************/

#include "timeout.h"

rftimeout_t aTimer[4];

void RF_TMR_Init(void)
{
    LPC_SYSCON->SYSAHBCLKCTRL |= (1<<8); // enable clock to CT16B1
    RFTMR->PR  = SystemClock/1000;         // 1ms
    RFTMR->TCR = (1<<1);	// reset counter and prescaler
    RFTMR->CTCR = (0<<0); // Timer rfm_mode
    RFTMR->TCR = (1<<0);	// Enable timer and release reset

    NVIC_EnableIRQ(TIMER_16_1_IRQn); // Enable the TIMER1 Interrupt
}

void RF_TMR_SetMatch(rfmode_t mode, rfcallback_t callback, uint16_t timeout)
{
    if(mode == RX)
    {
        aTimer[0].callback = callback;
        aTimer[0].dt = timeout;
    }

    if(mode == TX)
    {
        aTimer[1].callback = callback;
        aTimer[1].dt = timeout;
    }
}

void RF_TMR_Start(rfmode_t mode)
{
    uint16_t cntval= RFTMR->TC;

    if(mode == RX)
    {
        aTimer[0].match = cntval+aTimer[0].dt;
        RFTMR->MR0 = aTimer[0].match;
        RFTMR->MCR |= (1<<0);           // enable the match interrupt
        aTimer[0].enable = TRUE;
    }

    if(mode == TX)
    {
        aTimer[1].match = cntval+aTimer[1].dt;
        RFTMR->MR1 = aTimer[1].match;
        RFTMR->MCR |= (1<<3);           // enable the match interrupt
        aTimer[1].enable = TRUE;
    }
}

void RF_TMR_Feed(rfmode_t mode)
{
    uint16_t cntval= RFTMR->TC;

    if(mode == RX)
    {
        aTimer[0].match = cntval+aTimer[0].dt;
        RFTMR->MR0 = aTimer[0].match;
    }

    if(mode == TX)
    {
        aTimer[1].match = cntval+aTimer[1].dt;
        RFTMR->MR1 = aTimer[1].match;
    }
}

void RF_TMR_Stop(rfmode_t mode)
{
    if(mode == RX)
    {
        RFTMR->MCR &= ~(1<<0);           // enable the match interrupt
        aTimer[0].enable = FALSE;
    }

    if(mode == TX)
    {
        RFTMR->MCR &= ~(1<<3);           // enable the match interrupt
        aTimer[1].enable = FALSE;
    }
}

void TIMER16_1_IRQHandler(void)
{
    uint32_t regval=0;
    regval = RFTMR->IR;

    if ( RFTMR->IR & (1<<0) )
    {
        if(aTimer[0].enable)
        {
            aTimer[0].callback(&RX_Frame, eTimeout);
            RF_TMR_Stop(RX);
        }
    }

    if ( RFTMR->IR & (1<<1) )
    {
        if(aTimer[1].enable)
        {
            aTimer[1].callback(&TX_Frame, eTimeout);
            RF_TMR_Stop(TX);
        }
    }

    if ( RFTMR->IR & (1<<2) )
    {

    }

    if ( RFTMR->IR & (1<<3) )
    {

    }


    RFTMR->IR = regval;
}
