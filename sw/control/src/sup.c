/**************************************************************************
*   $Id: sup.c 41 2012-02-12 16:03:02Z sander $
*
*   Copyright: Sander Straatjes
*
***************************************************************************/

#include "sup.h"


void SUP_init(void)
{
    RFM_SSP_Init();
    RFM_Module_init();
    RFM_SetCallback(&SUP_RX_Callback, RX);
    RFM_SetCallback(&SUP_TX_Callback, TX);
    RF_TMR_Init();
    RF_TMR_SetMatch(RX, &SUP_RX_Callback, 1000);
    RF_TMR_SetMatch(TX, &SUP_TX_Callback, 1000);
    RF_TMR_Start(TX);
    RF_TMR_Start(RX);
}

int8_t SUP_RX_Callback(rfframe_t* pData, rfevent_t event)
{
    switch(event)
    {
    case eComplete:
        break;
    case eCrcError:
    case eIRQError:
        printf("RX error %u\n", event);
        break;
    case eTimeout:
        printf("RX timeout %u\n", event);
        break;
    default:
        printf("RX undefined event %u\n", event);
        break;
    }


    return 1;
}

int8_t SUP_TX_Callback(rfframe_t* pData, rfevent_t event)
{

    switch(event)
    {
    case eComplete:
        break;
    case eTimeout:
        printf("TX timeout %u\n", event);
        break;
    default:
        printf("TX undefined event %u\n", event);
        break;
    }

    printf("TX Event %u\n", event);

    return 1;
}
