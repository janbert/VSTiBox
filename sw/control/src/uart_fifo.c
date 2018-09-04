/**************************************************************************
*   $Id: uart_fifo.c 41 2012-02-12 16:03:02Z sander $
*
*   Copyright: Sander Straatjes
*
***************************************************************************/

#include "uart_fifo.h"

FIFO_t fifo[FIFOS];

unsigned char FIFO_Init(unsigned char nr)
{
	unsigned long j=0;

	if(nr>=FIFOS) return 0;

	for(j=0;j<FIFO_SIZE;j++)
	{
		fifo[nr].Data[j]=0;
	}
	fifo[nr].WRptr=0;
	fifo[nr].RDptr=FIFO_SIZE-1;
	fifo[nr].data_ready = 0;

	return 1;
}

void FIFO2UART(unsigned char nr)
{
    uint8_t i=0, cnt=0;

    cnt = FIFO_DataCnt(0);

    if(cnt > 16) cnt = 16;

    for(i=0;i<cnt;i++)
    {
        LPC_UART->THR = FIFO_Getc(0);
    }
}

unsigned char FIFO_UART_TX(unsigned char nr, unsigned char c)
{
	uint32_t datacnt = FIFO_DataCnt(0), lsr = LPC_UART->LSR;

	FIFO_UART_RX(nr, c);

    if(datacnt==0 && (lsr & (1<<5)))
    {
        FIFO2UART(0);
    }

    return(c);
}

unsigned char FIFO_UART_RX(unsigned char nr, unsigned char c)
{
	static unsigned char lastchar=0;

	if(fifo[nr].WRptr == fifo[nr].RDptr)   // buffer overflow
	{
		FIFO_Init(nr);
		return 0;
	}

	if( (lastchar == UART_TERM_CHAR) && (c == UART_TERM_CHAR))return 1; // remove double newlines

	fifo[nr].Data[fifo[nr].WRptr] = c;
	fifo[nr].WRptr++;
	if(c == UART_TERM_CHAR)
	{
		fifo[nr].data_ready++;
	}
	lastchar = c;
	return 1;
}

unsigned char FIFO_GetLine(unsigned char nr, unsigned char *s)
{
    uint8_t cnt =0;
    unsigned char c=0;

    while( cnt < 0xFF)
    {
        c = FIFO_Getc(nr);
        if( (c == UART_TERM_CHAR) || (c == 0) )
        {
            s[cnt]='\0';
            break;
        }
        s[cnt] = c;
        cnt++;
    }

    return cnt;
}

unsigned char FIFO_Getc(unsigned char nr)
{
	unsigned char c=0;

	fifo[nr].RDptr++;

	if(fifo[nr].WRptr == fifo[nr].RDptr) //buffer underflow
	{
		//FIFO_Init(nr);
		return 0;
	}

	c = fifo[nr].Data[fifo[nr].RDptr];
	if(c==UART_TERM_CHAR && fifo[nr].data_ready>0) fifo[nr].data_ready--;

	return(c);
}

unsigned char FIFO_DataCnt(unsigned char nr)
{
	return(fifo[nr].WRptr - fifo[nr].RDptr -1);
}

unsigned char FIFO_DataRdy(unsigned char nr)
{
	return(fifo[nr].data_ready);
}

