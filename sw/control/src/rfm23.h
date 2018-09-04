/**************************************************************************
*   $Id: rfm23.h 41 2012-02-12 16:03:02Z sander $
*
*   Copyright: Sander Straatjes
*
***************************************************************************/

#ifndef RFM23_H_INCLUDED
#define RFM23_H_INCLUDED

#include "application.h"

#define NIRQ 206

#define RFSSP LPC_SSP1


#define NETID 0xD4

/* SSP Status register */
#define SSPSR_TFE       (0x1<<0)
#define SSPSR_TNF       (0x1<<1)
#define SSPSR_RNE       (0x1<<2)
#define SSPSR_RFF       (0x1<<3)
#define SSPSR_BSY       (0x1<<4)


void RFM_SetCallback(rfcallback_t pFunc,rfmode_t dir);
rfframe_t *RFM_DataRdy(void);
void RFM_TX(uint8_t *data, int16_t length);
void RFM_ChangeMode(rfmode_t _mode);
void RFM_ResetRX(void);
uint16_t RFM_Status(void);
void RFM_RX_IRQ(void);
void RFM_TX_IRQ(void);
void RFM_Module_init(void);

void RFM_IO_Init(void);

void RFM_SSP_Init(void);
uint16_t RFM_SSP_Read(uint16_t cmd);
void RFM_SSP_Write(uint16_t data);
void RFM_SSP_CMD(uint16_t data);
void RFM_SSP_Sleep(void);

void rf_delay_us(uint32_t t);
unsigned char RFM_crc8(unsigned char crc, unsigned char data);

#endif // RFM23_H_INCLUDED
