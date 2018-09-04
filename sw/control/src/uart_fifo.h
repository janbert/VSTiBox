/**************************************************************************
*   $Id: uart_fifo.h 41 2012-02-12 16:03:02Z sander $
*
*   Copyright: Sander Straatjes
*
***************************************************************************/

#ifndef __UART_FIFO_H_
#define __UART_FIFO_H_

#include "application.h"

#define FIFO_SIZE 5
#define FIFOS 16

#define UART_TERM_CHAR '\r'
#define UART_TX 0
#define UART_RX 1

typedef struct{
	unsigned char Data[FIFO_SIZE];
	unsigned char WRptr;
	unsigned char RDptr;
	unsigned char data_ready;
}FIFO_t;

unsigned char FIFO_Init(unsigned char nr);
unsigned char FIFO_Getc(unsigned char nr);
unsigned char FIFO_GetLine(unsigned char nr, unsigned char *s);
unsigned char FIFO_DataCnt(unsigned char nr);
unsigned char FIFO_DataRdy(unsigned char nr);

unsigned char FIFO_UART_RX(unsigned char nr, unsigned char c);
unsigned char FIFO_UART_TX(unsigned char nr, unsigned char c);
void FIFO2UART(unsigned char nr);


#endif // __UART_FIFO_H_

