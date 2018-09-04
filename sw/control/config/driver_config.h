/*
 * driver_config.h
 *
 *  Created on: Aug 31, 2010
 *      Author: nxp28548
 */

#ifndef DRIVER_CONFIG_H_
#define DRIVER_CONFIG_H

#include <LPC11xx.h>

#define IOCONPWR(x)	(x? (LPC_SYSCON->SYSAHBCLKCTRL |= (1<<16)):(LPC_SYSCON->SYSAHBCLKCTRL &= ~(1<<16)))

#define CONFIG_ENABLE_DRIVER_CRP						1
#define CONFIG_CRP_SETTING_NO_CRP						1

#define CONFIG_ENABLE_DRIVER_TIMER32					1
#define CONFIG_TIMER32_DEFAULT_TIMER32_0_IRQHANDLER		1

#define CONFIG_ENABLE_DRIVER_GPIO						1
#define CONFIG_ENABLE_DRIVER_I2C                        1
#define CONFIG_I2C_DEFAULT_I2C_IRQHANDLER				1
#define CONFIG_ENABLE_DRIVER_WDT						1

 /* DRIVER_CONFIG_H_ */
#endif
