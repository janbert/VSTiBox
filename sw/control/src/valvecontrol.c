/**************************************************************************
*   $Id: valvecontrol.c 41 2012-02-12 16:03:02Z sander $
*
*   Copyright: Sander Straatjes
*
***************************************************************************/

#include "valvecontrol.h"

uint32_t poscnt=0;
int32_t pos_target=0, position=0;
int8_t motor_dir=0;
uint8_t pos_state=0, poswdt=0;

#define OPEN    -1
#define CLOSE   1

#define OFF     0
#define IDLE    1
#define STOP    2
#define ACTIVE  3


void Valve_Set(int32_t pos)
{
    pos_target = pos*42;
    if(pos_target>position)Motor_Close();
    if(pos_target<position)Motor_Open();

	Motor_Duty(85);
}

void Pos_Init(void)
{
    IOCONPWR(1);

//	LPC_SYSCON->SYSAHBCLKCTRL |= (1<<8); // enable clock to CT16B1
    GPIOSetDir(1,10,1); //IR LED driver
	LPC_IOCON->PIO1_10 = (0<<0)|(0<<3)|(1<<6)|(1<<7); // Function GPIO, no PU/PD, digital mode
	LPC_GPIO1->DATA &= ~(1<<10);

	GPIOSetDir(1,8,0); //IR receiver
	LPC_IOCON->PIO1_8 = (0<<0)|(0<<3)|(1<<6)|(1<<7); // Function CT16B1_CAP0, no PU/PD, digital mode

    IOCONPWR(0);
}

void Pos_IRQ(void)
{
    static uint32_t posval=0, poslvl=0, idlecnt=0;
	uint32_t i=0;

    if(pos_state)
    {
        LPC_GPIO1->DATA |= (1<<10);
        for(i=0;i<0x0ff;i++){__NOP();}
        posval=((LPC_GPIO1->DATA>>8)&1);
        if(poslvl!=posval)
        {
            poslvl=posval;
            poscnt++;
            poswdt=0;
			position += motor_dir;
        }
        else
        {
            poswdt++;
        }
        LPC_GPIO1->DATA &= ~(1<<10);

        if(poswdt>40)Motor_Idle();
        if(pos_target==position)Motor_Stop();

        if(pos_state==STOP || pos_state==IDLE)idlecnt++;
        else idlecnt=0;

        if(idlecnt>180)
        {
            Motor_Idle();
			pos_state=OFF;
            if(pos_target<-5000)
            {
                position = 0;
                pos_target=0;
            }
        }


    }
}

void Motor_Duty(uint8_t duty)
{
    if(duty>100)duty=100;

    duty = 100-duty;
    if(duty==100)duty=101;
    LPC_TMR32B1->MR1	=  LPC_TMR32B1->MR3	= (PWM_period*duty)/100;
}

void Motor_Idle(void)
{
    IOCONPWR(1);

    LPC_TMR32B1->TCR = (1<<1);	// reset counter and prescaler

	// All bridge driver IO as input so nothing will happen and no power consumption
	GPIOSetDir(3,2,0); //MH1
	LPC_IOCON->PIO3_2 = (0<<0)|(0<<3)|(1<<6)|(1<<7); // Function GPIO, no PU/PD, digital mode
	GPIOSetDir(3,3,0); //MH2
	LPC_IOCON->PIO3_3 = (0<<0)|(0<<3)|(1<<6)|(1<<7); // Function GPIO, no PU/PD, digital mode
	GPIOSetDir(1,2,0); //ML1
	LPC_IOCON->R_PIO1_2 = (1<<0)|(0<<3)|(1<<6)|(1<<7); // Function GPIO, no PU/PD, digital mode
	GPIOSetDir(1,4,0); //ML2
	LPC_IOCON->PIO1_4 = (0<<0)|(0<<3)|(1<<6)|(1<<7); // Function GPIO, no PU/PD, digital mode

	LPC_SYSCON->SYSAHBCLKCTRL &= ~(1<<10); // disable clock to CT32B1 to save power
	IOCONPWR(0);

	pos_state=IDLE;
}

void Motor_Stop(void) // both top fets conducting to short the motor, both lower fets idle
{
    IOCONPWR(1);
	GPIOSetDir(3,2,1); //MH1
	GPIOSetDir(3,3,1); //MH2

	LPC_GPIO3->DATA &= ~((1<<2)|(1<<3));
	LPC_GPIO3->DATA |= (0<<2)|(0<<3);

	LPC_IOCON->R_PIO1_2 = (1<<0)|(0<<3)|(1<<6)|(1<<7); // Function GPIO, no PU/PD, digital mode
	LPC_IOCON->PIO1_4 = (0<<0)|(0<<3)|(1<<6)|(1<<7); // Function GPIO, no PU/PD, digital mode

    LPC_TMR32B1->TCR = (1<<1);	// reset counter and prescaler

	IOCONPWR(0);

	pos_state=STOP;
}

void Motor_Open(void)
{
    motor_dir=OPEN;
    pos_state=ACTIVE;
	poswdt=0;

    IOCONPWR(1);
	GPIOSetDir(3,2,1); //MH1
	GPIOSetDir(3,3,1); //MH2

	LPC_GPIO3->DATA &= ~((1<<2)|(1<<3));
	LPC_GPIO3->DATA |= (1<<2)|(0<<3);

	LPC_IOCON->R_PIO1_2 = (3<<0)|(0<<3)|(1<<6)|(1<<7); // Function CT32B1_MAT1, no PU/PD, digital mode
	LPC_IOCON->PIO1_4 = (0<<0)|(0<<3)|(1<<6)|(1<<7); // Function GPIO, no PU/PD, digital mode

	LPC_SYSCON->SYSAHBCLKCTRL |= (1<<10); // enable clock to CT32B1
	LPC_TMR32B1->TCR = (1<<0);	// enable timer and release reset
	IOCONPWR(0);
}

void Motor_Close(void)
{
    motor_dir=CLOSE;
    pos_state=ACTIVE;
	poswdt=0;

    IOCONPWR(1);
	GPIOSetDir(3,2,1); //MH1
	GPIOSetDir(3,3,1); //MH2

	LPC_GPIO3->DATA &= ~((1<<2)|(1<<3));
	LPC_GPIO3->DATA |= (0<<2)|(1<<3);

	LPC_IOCON->R_PIO1_2 = (1<<0)|(0<<3)|(1<<6)|(1<<7); // Function GPIO, no PU/PD, digital mode
	LPC_IOCON->PIO1_4 = (2<<0)|(0<<3)|(1<<6)|(1<<7); // Function CT32B1_MAT3, no PU/PD, digital mode

	LPC_SYSCON->SYSAHBCLKCTRL |= (1<<10); // enable clock to CT32B1
	LPC_TMR32B1->TCR = (1<<0);	// enable timer and release reset
	IOCONPWR(0);
}

void Motor_Init(void)
{
    IOCONPWR(1);
	// Init bridge driver IO as input so nothing will happen and no power consumption
	GPIOSetDir(3,2,0); //MH1
	LPC_IOCON->PIO3_2 = (0<<0)|(0<<3)|(1<<6)|(1<<7); // Function GPIO, no PU/PD, digital mode
	GPIOSetDir(3,3,0); //MH2
	LPC_IOCON->PIO3_3 = (0<<0)|(0<<3)|(1<<6)|(1<<7); // Function GPIO, no PU/PD, digital mode
	GPIOSetDir(1,2,0); //ML1
	LPC_IOCON->R_PIO1_2 = (1<<0)|(0<<3)|(1<<6)|(1<<7); // Function GPIO, no PU/PD, digital mode
	GPIOSetDir(1,4,0); //ML2
	LPC_IOCON->PIO1_4 = (0<<0)|(0<<3)|(1<<6)|(1<<7); // Function GPIO, no PU/PD, digital mode

	LPC_SYSCON->SYSAHBCLKCTRL |= (1<<10); // enable clock to CT32B1
	/* Setup the external match register */
	LPC_TMR32B1->EMR = (2<<10)|(2<<6)|(1<<3)|(1<<1)|(1<<3);
	/* Enable the selected PWMs and enable Match3 */
	LPC_TMR32B1->PWMC = (1<<1)|(1<<3);


	LPC_TMR32B1->MR0	= PWM_period;
	LPC_TMR32B1->MR1	= \
	LPC_TMR32B1->MR3	= 0xFFFFFFFF;
	LPC_TMR32B1->MCR = 1<<1;				/* Reset on MR0 */
	LPC_TMR32B1->CCR = 0; // Timer mode
	LPC_TMR32B1->TCR = (1<<1);	// reset counter and prescaler

	LPC_SYSCON->SYSAHBCLKCTRL &= ~(1<<10); // disable clock to CT32B1 to save power
	IOCONPWR(0);
}
