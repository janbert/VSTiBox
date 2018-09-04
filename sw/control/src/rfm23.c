/**************************************************************************
*   $Id: rfm23b.c 41 2012-02-12 16:03:02Z sander $
*
*   Copyright: Sander Straatjes
*
***************************************************************************/

#include "rfm23.h"

#define rfdelay 10



rfmode_t rfm_mode;
rfframe_t TX_Frame, RX_Frame;
const uint8_t preframe[] = {0xAA,0xAA,0xAA,0x2D};
const uint8_t postframe[] = {0xAA,0xAA,0xAA};



rfcallback_t pRX_Callback = NULL;
rfcallback_t pTX_Callback = NULL;

void RFM_SetCallback(rfcallback_t pFunc,rfmode_t dir)
{
    if(dir == RX)
    {
        pRX_Callback = pFunc;
    }
    else
    {
        pTX_Callback = pFunc;
    }
}



void RFM_RX_IRQ(void)
{
    uint16_t data = 0;
    static int8_t i = -2;
    static uint8_t packet_length = 0;
    static uint8_t crc = 0;
    uint8_t crcofsize=0;
    uint8_t cnt=100;
    //static queue<unsigned char> temp;

    //Loop while interrupt is asserted
    while (cnt--)
    {
        if (GPIO_Get(NIRQ))
            break;

        /* Grab the packet's length byte */
        if (i == -2)
        {
            data = RFM_Status();
            if ( (data&0x8000) )
            {
                data = RFM_SSP_Read(0xB000);
                packet_length = (data&0x00FF);
                crc = RFM_crc8(crc, packet_length);
                RX_Frame.len = packet_length;
                RX_Frame.crcl = crc;
                RX_Frame.state = 1;
                i++;
            }
        }

        //If we exhaust the interrupt, exit
        if (GPIO_Get(NIRQ))
            break;

        // Check that packet length was correct
        if (i == -1)
        {
            data = RFM_Status();
            if ( (data&0x8000) )
            {
                data = RFM_SSP_Read(0xB000);
                crcofsize = (data&0x00FF);
                if (crcofsize != crc)
                {
                    //It was wrong, start over
                    i = -2;
                    packet_length = 0;
                    crc = 0;
                    //temp = queue<unsigned char>();
                    RFM_ResetRX();
                    RX_Frame.state = 0;
                }
                else
                {
                    crc = RFM_crc8(crc, crcofsize);
                    i++;
                }
            }
        }

        //If we exhaust the interrupt, exit
        if (GPIO_Get(NIRQ))
            break;

        /* Grab the packet's data */
        if (i >= 0 && i < packet_length)
        {
            data = RFM_Status();
            if ( (data&0x8000) )
            {
                data = RFM_SSP_Read(0xB000);
                RX_Frame.data[i] = data;
                //temp.push(data&0x00FF);
                //printf("%c",data&0x00FF);
                crc = RFM_crc8(crc, (unsigned char)(data&0x00FF));
                i++;
            }
        }

        //If we exhaust the interrupt, exit
        if (GPIO_Get(NIRQ))
            break;

        if (i >= packet_length)
        {
            data = RFM_Status();
            if ( (data&0x8000) )
            {


                data = RFM_SSP_Read(0xB000);
                if ((unsigned char)(data & 0x00FF) == crc)
                {
                    RX_Frame.crc = crc;
                    RX_Frame.state = 0xFF;

                }

                /* Tell RF Module we are finished, and clean up */
                i = -2;
                packet_length = 0;
                crc = 0;
                RFM_ResetRX();
                if(RX_Frame.state==0xFF)pRX_Callback(&RX_Frame,eComplete);
            }
        }
    }
    if(!cnt)
    {
        pRX_Callback(&RX_Frame,eIRQError);
        i = -2;
        packet_length = 0;
        crc = 0;
        RFM_ResetRX();
    }
}

rfframe_t *RFM_DataRdy(void)
{
    if(RX_Frame.state==0xFF)
    {
        return(&RX_Frame);
    }
    else
    {
        return 0;
    }
}

void RFM_TX_IRQ(void)
{
    static uint8_t i=0;
    uint8_t *p=0;

    ///TODO clear timeout

    if(TX_Frame.state==1)
    {
        RFM_SSP_CMD(0xB800 + preframe[i]);
        i++;
        if(i>=sizeof(preframe))
        {
            TX_Frame.state++;
            i=0;
            return;
        }
    }
    if(TX_Frame.state==2)
    {
        p= (uint8_t *)&TX_Frame;
        RFM_SSP_CMD(0xB800 + p[i]);
        i++;
        if(i>=(TX_Frame.len+3))
        {
            TX_Frame.state++;
            i=0;
            return;
        }
    }
    if(TX_Frame.state==3)
    {
        RFM_SSP_CMD(0xB800 + TX_Frame.crc);
        TX_Frame.state++;
        return;
    }
    if(TX_Frame.state==4)
    {
        RFM_SSP_CMD(0xB800 + postframe[i]);
        i++;
        if(i>=sizeof(postframe))
        {
            TX_Frame.state++;
            i=0;
            return;
        }
    }
    if(TX_Frame.state==5)
    {
        TX_Frame.state = 0xFF;
        ///TODO: Stop timeout timer
        RFM_ChangeMode(RX);
        return;
    }
}

void RFM_TX(uint8_t *data, int16_t length)
{
    int8_t crc = 0;
    uint8_t i=0;

    TX_Frame.state = 0;

    TX_Frame.netid = NETID;
    TX_Frame.len = length;
    /* Packet Length */
    crc = RFM_crc8(crc, length);
    TX_Frame.crcl = crc;

    crc = RFM_crc8(crc, crc);
    /* Packet Data */
    for (i=0; i<length; i++)
    {
        TX_Frame.data[i] = data[i];
        crc = RFM_crc8(crc, data[i]);
    }
    TX_Frame.crc = crc;

    TX_Frame.state = 1;

    RFM_ChangeMode(TX);
    RFM_SSP_CMD(0x0000);

    if(!GPIO_Get(NIRQ))RFM_TX_IRQ();

    ///TODO: add timeout
}

uint16_t RFM_Status(void)
{
    return (RFM_SSP_Read(0x0000));
}

void RFM_SSP_Sleep(void)
{
    LPC_SYSCON->SSP1CLKDIV    = 0;
    LPC_SYSCON->SYSAHBCLKCTRL &= ~(1<<18); // disable clock to SSP
}

void RFM_SSP_Wake(void)
{
    LPC_SYSCON->SYSAHBCLKCTRL |= (1<<18); // enable clock to SSP
    LPC_SYSCON->SSP1CLKDIV    = 1;
}

uint16_t RFM_SSP_Read(uint16_t cmd)
{
    uint16_t data=0;

    while((RFSSP->SR&SSPSR_RNE))
    {
        data = RFSSP->DR;    // clear the RX fifo
    }
    while((RFSSP->SR&SSPSR_BSY)) {}; // wait for room in TX fifo
    RFSSP->DR = cmd;
    while((RFSSP->SR&SSPSR_BSY)) {}; // wait for room in TX fifo

    data = RFSSP->DR;
    return (data);
}

void RFM_SSP_Write(uint16_t data)
{
    while((RFSSP->SR&SSPSR_BSY)) {}; // wait for room in TX fifo
    RFSSP->DR = data;
    while((RFSSP->SR&SSPSR_BSY)) {}; // wait for room in TX fifo
}

void RFM_SSP_CMD(uint16_t data)
{
    while(!(RFSSP->SR&SSPSR_TNF)) {}; // wait for room in TX fifo
    RFSSP->DR = data;
}


void RFM_Module_init(void)
{

    // 1. Configuration Setting Command
    RFM_SSP_CMD(
        RFM_CONFIG_EL           |
        RFM_CONFIG_EF           |
        RFM_CONFIG_BAND_868     |
        RFM_CONFIG_X_12_5pf // meh, using default
    );

    // 2. Power Management Command
    RFM_SSP_CMD(
        RFM_POWER_MANAGEMENT  |
        RFM_POWER_MANAGEMENT_EBB |
        RFM_POWER_MANAGEMENT_DC
    );

    // 3. Frequency Setting Command 868 MHz
    RFM_SSP_CMD(
        RFM_FREQUENCY            |
        0x0680
    );

    // 4. Data Rate Command
    RFM_SSP_CMD(
        RFM_DATA_RATE |
        RFM_DATA_RATE_57600
    );

    // 5. Receiver Control Command
    RFM_SSP_CMD(
        RFM_RX_CONTROL_VDI  |
        RFM_RX_CONTROL_VDI_FAST |
        RFM_RX_CONTROL_BW_134   |
        RFM_RX_CONTROL_GAIN_0   |
        RFM_RX_CONTROL_RSSI_103
    );

    // 6. Data Filter Command
    RFM_SSP_CMD(
        RFM_DATA_FILTER_AL      |
        //RFM_DATA_FILTER_ML      |
        RFM_DATA_FILTER_DIG     |
        0x0004
    );

    // 7. FIFO and Reset Mode Command
    RFM_SSP_CMD(
        RFM_FIFO_AL     |
        RFM_FIFO_DR     |
        RFM_FIFO_FF     |
        0x0080
    );

    // SPIWrite( CMD_SYNC_PATTERN | 0x00D4 );        // pattern: 11010100
    // 8. FIFO Syncword
    // Leave as default: 0xD4

    // 9. Receiver FIFO Read
    // when the interupt goes high, (and if we can assume that it was a fifo fill interrupt) we can read a byte using:
    // result = RFM_READ_FIFO();

    // 10. AFC Command
    RFM_SSP_CMD(
        RFM_AFC_AUTO_VDI        |  //Note this might be changed to improve range. Refer to datasheet.
        //RFM_AFC_AUTO_INDEPENDENT    |
        RFM_AFC_RANGE_LIMIT_15_16   |
        RFM_AFC_ST                  |
        RFM_AFC_EN                  |
        RFM_AFC_OE
    );

    // 11. TX Configuration Control Command
    RFM_SSP_CMD(
        RFM_TX_CONTROL        |
        RFM_TX_CONTROL_MOD_90 |
        RFM_TX_CONTROL_POW_15
    );


    // 12. PLL Setting Command
    RFM_SSP_CMD(
        0xCC77 & ~0x01 // Setting the PLL bandwith, less noise, but max bitrate capped at 86.2
        // I think this will slow down the pll's reaction time. Not sure, check with someone!
    );

    // 13. Wake-up Timer Command
    RFM_SSP_CMD(
        RFM_WAKEUP_TIMER
    );

    // 14. Low Duty-Cycle Command
    RFM_SSP_CMD(
        RFM_LOW_DUTY_CYCLE
    );

    // 15. Low Battery Detector Command
    RFM_SSP_CMD(
        RFM_LOW_BATT_DETECT
    );

    RFM_ChangeMode(RX);
    RFM_ResetRX();
    RFM_SSP_CMD(0x0000);
//    RFM_ChangeMode(PD);

    LPC_GPIO2->IC = 0xFFFFFFFF;
    NVIC_EnableIRQ(EINT2_IRQn);
}


void RFM_SSP_Init(void)
{

    LPC_SYSCON->PRESETCTRL |= (0x1<<2);
    LPC_SYSCON->SYSAHBCLKCTRL |= (1<<18); // enable clock to SSP1
    LPC_SYSCON->SSP1CLKDIV    = 1;

    RFSSP->CPSR = 6;//2MHz
    RFSSP->CR0 = ((0xF<<0)|(0<<4)|(0<<6)|(0<<7)|(0<<8));
    RFSSP->CR1 = ((0<<0)|(1<<1)|(0<<2)|(0<<3));

    RFM_IO_Init();
}

void rf_delay_us(uint32_t t)
{
    while(t)
    {
        t--;
        __NOP();
        __NOP();
        __NOP();
        __NOP();
        __NOP();
        __NOP();
        __NOP();
    }

    __NOP();
    __NOP();
    __NOP();
    __NOP();
    __NOP();
    __NOP();
}

/* Change the mode of the RF module to Transmitting or Receiving */
void RFM_ChangeMode(rfmode_t mode)
{
    if((mode != PD) && (rfm_mode ==PD))
    {
        RFM_SSP_Wake();
    }

    if (mode == TX)
    {
        RFM_SSP_CMD(0x8239); //!er,!ebb,ET,ES,EX,!eb,!ew,DC
    }
    if (mode == RX)
    {
        RFM_SSP_CMD(0x8299); //er,!ebb,ET,ES,EX,!eb,!ew,DC
    }
    if (mode == PD)
    {
        RFM_SSP_Write(0x8201); //er,!ebb,ET,ES,EX,!eb,!ew,DC
        RFM_SSP_Sleep();
    }
    rfm_mode = mode;
}

// Tell the RF Module this packet is received and wait for the next
void RFM_ResetRX(void)
{
    RFM_SSP_CMD(0xCA81);
    RFM_SSP_CMD(0xCA83);
}

// Calculate RFM_crc8
unsigned char RFM_crc8(unsigned char crc, unsigned char data)
{
    uint8_t i=0;

    crc = crc ^ data;
    for (i = 0; i < 8; i++)
    {
        if (crc & 0x01)
        {
            crc = (crc >> 1) ^ 0x8C;
        }
        else
        {
            crc >>= 1;
        }
    }
    return crc;
}

void PIOINT2_IRQHandler(void)
{
    uint32_t regVal;

    regVal = LPC_GPIO2->MIS;
    if ( regVal & (1<<6) )
    {
        RFM_SSP_Wake();
        //printf("RFMIRQ\n");
        GPIO_Toggle(204);
        switch(rfm_mode)
        {
        case RX:
            RFM_RX_IRQ();
            break;

        case TX:
            RFM_TX_IRQ();
            break;

        default:
            break;
        }
    }


    LPC_GPIO2->IC |= regVal;
    return;
}

void RFM_IO_Init(void)
{
    IOCONPWR(1);

    GPIOSetDir(2,0,0);  // nSSEL
    LPC_IOCON->PIO2_0 = (2<<0)|(0<<3)|(1<<6)|(1<<7); // Function GPIO, no PU/PD, digital mode

    GPIOSetDir(2,1,0);  // SCK
    LPC_IOCON->PIO2_1 = (2<<0)|(0<<3)|(1<<6)|(1<<7); // Function GPIO, no PU/PD, digital mode

    GPIOSetDir(2,2,0);  // MISO
    LPC_IOCON->PIO2_2 = (2<<0)|(0<<3)|(1<<6)|(1<<7); // Function GPIO, no PU/PD, digital mode

    GPIOSetDir(2,3,0);  // MOSI
    LPC_IOCON->PIO2_3 = (2<<0)|(0<<3)|(1<<6)|(1<<7); // Function GPIO, no PU/PD, digital mode

    GPIOSetDir(2,6,0);  // nIRQ
    LPC_IOCON->PIO2_6 = (0<<0)|(0<<3)|(1<<6)|(1<<7); // Function GPIO, no PU/PD, digital mode
    GPIOSetInterrupt(2,6,0,0,0);
    GPIOIntEnable(2,6);

    IOCONPWR(0);
}
