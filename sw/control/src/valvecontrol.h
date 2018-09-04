/**************************************************************************
*   $Id: valvecontrol.h 41 2012-02-12 16:03:02Z sander $
*
*   Copyright: Sander Straatjes
*
***************************************************************************/

#ifndef VALVECONTROL_H_INCLUDED
#define VALVECONTROL_H_INCLUDED

#ifdef __cplusplus
extern "C" {
#endif 

#include "application.h"

#define PWM_frequency 20000
#define PWM_period SystemClock/PWM_frequency

void Motor_Init(void);
void Motor_Open(void);
void Motor_Close(void);
void Motor_Stop(void);
void Motor_Idle(void);
void Motor_Duty(uint8_t duty);

void Pos_Init(void);
void Pos_IRQ(void);
uint32_t Pos_Get(void);

void Valve_Set(int32_t pos);

#ifdef __cplusplus
}
#endif 

#endif // VALVECONTROL_H_INCLUDED
