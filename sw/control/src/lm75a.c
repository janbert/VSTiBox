/*****************************************************************************
 *   eeprom.c:  Driver for the 24LC08 EEPROM
 *
 *   Copyright(C) 2009, Embedded Artists AB
 *   All rights reserved.
 *
 ******************************************************************************/

/*
 * NOTE: I2C must have been initialized before calling any functions in this
 * file.
 */

/******************************************************************************
 * Includes
 *****************************************************************************/
#include "application.h"

/******************************************************************************
 * Defines and typedefs
 *****************************************************************************/
#define LM75A_I2C_ADDR    (0x48 << 1)

#define LM75A_ADDR_TEMP 0x00

/******************************************************************************
 * External global variables
 *****************************************************************************/
extern volatile uint32_t I2CCount;
extern volatile uint8_t I2CMasterBuffer[I2C_BUFSIZE];
extern volatile uint8_t I2CSlaveBuffer[I2C_BUFSIZE];
extern volatile uint32_t I2CMasterState;
extern volatile uint32_t I2CReadLength, I2CWriteLength;

/******************************************************************************
 * Local variables
 *****************************************************************************/

/******************************************************************************
 * Local Functions
 *****************************************************************************/

/******************************************************************************
 * Public Functions
 *****************************************************************************/

/******************************************************************************
 *
 * Description:
 *    Initialize the LM75 Driver
 *
 *****************************************************************************/
void lm75a_init (void)
{
	I2CInit( (uint32_t)I2CMASTER);    
}

/******************************************************************************
 *
 * Description:
 *    Read the temperature
 *
 * Params: None
 *
 * Returns:
 *   The measured temperature x 10, i.e. 26.5 degrees returned as 265
 *
 *****************************************************************************/
int16_t lm75a_readTemp(void)
{
    int16_t t = 0;
	int32_t t32;

    /* Write SLA(W), address, SLA(R), and read two bytes back. */
    I2CWriteLength = 2;
    I2CReadLength = 2;
    I2CMasterBuffer[0] = LM75A_I2C_ADDR;
    I2CMasterBuffer[1] = LM75A_ADDR_TEMP;
    I2CMasterBuffer[2] = LM75A_I2C_ADDR | RD_BIT;

    I2CEngine();

    /* 11 MSB bits used. Celcius is calculated as Temp data * 1/8 */
    t = (((int16_t)I2CSlaveBuffer[0] << 8) | ((int16_t)I2CSlaveBuffer[1]));
	t32 = ((int32_t)t * 10) >> 8;
    return (int16_t)t32;
}

