/**************************************************************************
*   Main.c VSTiBox button breakout
***************************************************************************/

#include "application.h"

//#define JTAG	// Select either JTAG/SWD or LED PWM functionality! They share the same pin.
 
// Board options
#define ENC0
#define ENC1
#define ENC2
#define ENC3

//#define ANA

// Defines
#define ABS(x)			(x>0 ? x:-x)
#define ID_INVALID 		0xFF
#define PROTOCOL_SIZE	5				// CMD, ID, BYTE0, BYTE1, CHK

#define PWM_FREQ	800		// Hz
#define PWM_PERIOD ((1.0/PWM_FREQ)*SystemClock) 

#define FUNC0 (0)
#define FUNC1 (1)
#define FUNC2 (2)
#define FUNC3 (3)
#define DIGITAL_MODE (1<<7)

#define PULL_UP (2<<3)

// Left to right:
// LED 1, 3, 2, 4
#define LEDR1		0		// P0.10	T16B0_MAT2	 	+ SWCLK
#define LEDR2		1		// P0.1		T32B0_MAT2		+ ISP
//#define LEDR3		2		// P1.7		T32B0_MAT1 ==> NOW TX!
#define LEDR3		2		// P2.6		PIO2_6 !!!
#define LEDR4		3		// P0.11	T32B0_MAT3
#define LEDG1		4		// P1.2 	T32B1_MAT1
#define LEDG2		5		// P0.8		T16B0_MAT0		
//#define LEDG3		6		// P1.6		T32B0_MAT0 ==> NOW RX!
#define LEDG3		6		// P1.5		PIO1_5 !!!
#define LEDG4		7		// P1.10	T16B1_MAT1
#define LEDB1		8		// P1.1 	T32B1_MAT0
#define LEDB2		9		// P1.9		T16B1_MAT0
#define LEDB3		10		// p1.3		T32B1_MAT2		+ SWDIO
#define LEDB4		11		// P0.9		T16B0_MAT1

#define BTN1	(!(GPIOGetValue(2, 4)))		//	P2.4
#define BTN2	(!(GPIOGetValue(2, 5)))		//	P2.5
#define BTN3	(!(GPIOGetValue(3, 1)))		//	P3.1
#define BTN4	(!(GPIOGetValue(2, 7)))		//	P2.7
#define E1_I	(!(GPIOGetValue(2, 10)))	//	P2.10
#define E2_I	(!(GPIOGetValue(2, 2)))		//	P2.2
#define E3_I	(!(GPIOGetValue(3, 2)))		//	P3.2
#define E4_I	(!(GPIOGetValue(3, 4)))		//  P3.4

#define CMD_SET_ID 		(0 | (1<<7))		// 0x80
#define CMD_BTN 			(1 | (1<<7))		// 0x81
#define CMD_RT_BTN 		(2 | (1<<7))		// 0x82
#define CMD_RT_DELTA(n) ((3+n) | (1<<7))	// n = 0..3  0x83/0x84/0x85/0x86
#define CMD_LED_VALUE	(7 | (1<<7))		// 0x87
#define CMD_ACT_LED 		(8 | (1<<7))		// 0x88
#define CMD_TEMP 			(9 | (1<<7))		// 0x89
#define CMD_ANA			(11 | (1<<7))		// 0x8B

#pragma pack(1)   	/* byte alignment */
typedef struct
{
		uint8_t cmd;
		uint8_t id;
		uint16_t val;
		uint8_t chk;
}Msg_t;
#pragma pack()		/* reset to default alignment */ 

typedef struct
{
	uint8_t port; 
	uint8_t pin;
	uint8_t val;
	uint16_t debounce;
}Input_t;

typedef struct 
{
	uint8_t irq;
	int8_t delta;
	Input_t A;
	Input_t B;
}Encoder_t;

extern volatile uint32_t UART_Status;
extern volatile uint32_t UART_TxEmpty;
extern volatile uint8_t  UART_RxBuff[BUFSIZE];
extern volatile uint32_t UART_RxCount;
volatile uint16_t AD7_val;       	/* Last converted AD7 value */
uint32_t systick=0;

float led[] = {	0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 	// Red		0...3
				0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 	// Green	0...3
				0.0, 0.0, 0.0, 0.0, 0.0, 0.0}; 	// Blue		0...3
									 
					// 	irq		d  		Aport 	Apin 	prevA	debounce	Bport 	Bpin 	prevB	debounce
Encoder_t enc_ab[] = {	{0, 	0,		{0,		7, 		0,		0},			{2,		9,		0,		0}},
						{0,		0,		{0,		6, 		0,		0},			{2,		1,		0,		0}},
						{0,		0,		{3,		0, 		0,		0},			{2,		3, 		0,		0}},
						{0,		0,		{2,		11, 	0,		0},			{3,		3,		0,		0}}};
		   
				// 	Port	pin		val	debounce
Input_t	btn[] =   {{2,		4, 		0,		0},	
					{2,		5, 		0,		0},	
					{3,		1, 		0,		0},	
					{2,		7, 		0,		0}};	
Input_t	enc_i[] =  {{2,		10,		0,		0},	
					{2,		2, 		0,		0},	
					{3,		2, 		0,		0},	
					{3,		4, 		0,		0}};	

void ClockInit(void);
void IO_Init(void);
void LedsInit(void);
void LedPWM(uint8_t led, float pwm);
void Led_value(uint16_t value);
void Led_activate(void);
void sendMessage(Msg_t *msg);
void ADC_init (void); 
void ADC_start(void);
static __inline void updateEnc(Encoder_t *enc);
static __inline uint8_t updateBtn(Input_t *inp);

/*----------------------------------------------------------------------------
  MAIN function
 *----------------------------------------------------------------------------*/
int main (void)                                	/* Main Program               */
{
	uint8_t neededbytes = PROTOCOL_SIZE; 
	uint16_t adc;
	uint8_t doFwd = 0, doReply = 0, doNew = 0;
	uint8_t i;
	int16_t temperature;	// 0.1 deg
	uint8_t tempIrq = 0, btnIrq = 0, rotBtnIrq = 0, adcIrq = 0;
	uint8_t id = ID_INVALID;			// My id
	Msg_t rxMsg;							// Message received
	Msg_t replyMsg;						// Message send in response to request (e.g. Temp)
	Msg_t fwdMsg;							// Message forward : not intended for this slave 
	Msg_t newMsg;							// Message created by the slave (e.g. button press / release / analog changed)
	
	// Huge delay for JTAG/SWD to take over before being set to GPIO (shared pin)! Without this delay debugging is 
    // not possible anymore when deselecting JTAG/SWD functionality. In this case only the on-chip bootloader can be used
	int n;
	for(n=0;n<2000000;n++)	// 3 sec
	{
		__NOP();
	}
	
	ClockInit();
	GPIOInit();
	IO_Init();
	SysTick_Config(SystemClock/10);          	/* Generate IRQ each ~100 ms   */
	UART_Init(115200);
	ADC_init();
	lm75a_init();
	LedsInit();
			
	while (1)                                   /* Loop forever               */
	{
		/* Systick @ 100ms */
		if(systick == 30)	// Every 3 sec
		{
			systick = 0;
			temperature = lm75a_readTemp();
			tempIrq = 1;
			
			// Check valid ADC value's
			if(AD7_val>3 && AD7_val<1020)	
			{
				if(AD7_val != adc)
				{
					#ifdef ANA
					adcIrq = 1;
					#endif
				}
			}
			adc = AD7_val;	
			ADC_start();		// Start a new ADC conversion
		}
		
		btnIrq |= updateBtn(&btn[0]);
		btnIrq |= updateBtn(&btn[1]);
		btnIrq |= updateBtn(&btn[2]);
		btnIrq |= updateBtn(&btn[3]);
	
#ifdef ENC0		
		rotBtnIrq |= updateBtn(&enc_i[0]);
#else
		updateBtn(&enc_i[0]);
#endif
#ifdef ENC1		
		rotBtnIrq |= updateBtn(&enc_i[1]);
#else
		updateBtn(&enc_i[1]);
#endif
#ifdef ENC2		
		rotBtnIrq |= updateBtn(&enc_i[2]);
#else
		updateBtn(&enc_i[2]);
#endif
#ifdef ENC3		
		rotBtnIrq |= updateBtn(&enc_i[3]);
#else
		updateBtn(&enc_i[3]);
#endif

		updateEnc(&enc_ab[0]);						// Encoder delta 1
		updateEnc(&enc_ab[1]);						// Encoder delta 2
		updateEnc(&enc_ab[2]);						// Encoder delta 3
		updateEnc(&enc_ab[3]);						// Encoder delta 4
		
		/*******************************************/
		/* BUILD NEW MESSAGES IN ORDER OF PRIORITY */
		/*******************************************/
		
		if(!doNew && btnIrq && id!=ID_INVALID)
		{
			// Create btn irq message
			newMsg.cmd = CMD_BTN;
			newMsg.id = id;
			newMsg.val = (btn[0].val << 0) | (btn[1].val << 1) | (btn[2].val << 2) | (btn[3].val << 3) ;
			doNew = 1;
			btnIrq = 0;
		}	
		
		if(!doNew && rotBtnIrq && id!=ID_INVALID)	
		{
			// Create rotary btn irq message
			newMsg.cmd = CMD_RT_BTN;
			newMsg.id = id;
			newMsg.val = (enc_i[0].val << 0) | (enc_i[1].val << 1) | (enc_i[2].val << 2) | (enc_i[3].val << 3) ;
			doNew = 1;
			rotBtnIrq = 0;
		}
		
		for(i=0;i<4;i++)
		{
			if(!doNew && enc_ab[i].irq && id!=ID_INVALID)
			{
				// Create rotary delta irq message
				newMsg.cmd = CMD_RT_DELTA(i);
				newMsg.id = id;
				newMsg.val = (uint16_t)enc_ab[i].delta;
				doNew = 1;
				enc_ab[i].irq = 0;
			}
		}	

		// ADC almost lowest priority
		if(!doNew && adcIrq && id!=ID_INVALID)
		{
			// Create ADC irq message
			newMsg.cmd = CMD_ANA;
			newMsg.id = id;
			newMsg.val = adc;
			doNew = 1;
			adcIrq = 0;
		}	

		// Temperature lowest priority
		if(!doNew && tempIrq && id!=ID_INVALID)
		{
			// Create temperature irq message
			newMsg.cmd = CMD_TEMP;
			newMsg.id = id;
			newMsg.val = temperature;
			doNew = 1;
			tempIrq = 0;
		}		
		
		/***************************/
		/* SEND AND RECEIVE ********/
		/***************************/
		
		// Check if everything has been sent, before making new messages
		if (!doFwd && !doReply && UART_RxCount >= neededbytes )
		{
			if((UART_RxBuff[neededbytes-PROTOCOL_SIZE] & (1<<7)) == 0)
			{
				// Not correct CMD byte
				neededbytes++;
			}
			else
			{
				// Handle 'PROTOCOL_SIZE' bytes
				LPC_UART->IER = IER_THRE | IER_RLS;							/* Disable RBR */
				
				rxMsg.cmd = UART_RxBuff[neededbytes-(PROTOCOL_SIZE-0)]; 
				rxMsg.id = UART_RxBuff[neededbytes-(PROTOCOL_SIZE-1)]; 
				rxMsg.val = ((uint16_t)UART_RxBuff[neededbytes-(PROTOCOL_SIZE-3)] << 8) | UART_RxBuff[neededbytes-(PROTOCOL_SIZE-2)];
				rxMsg.chk = UART_RxBuff[neededbytes-(PROTOCOL_SIZE-4)]; 
				
				// Copy bytes to beginning of buffer
				for(i=neededbytes; i<UART_RxCount; i++)
				{
					UART_RxBuff[i-neededbytes] = UART_RxBuff[i];	
				}
				UART_RxCount -= neededbytes;
				
				LPC_UART->IER = IER_THRE | IER_RLS | IER_RBR;		/* Re-enable RBR */
				
				// Command
				switch(rxMsg.cmd)
				{
					// 0	binary mask	Set ID	Each slave sets it bit				
					// 1	binary mask	Button	binary mask	1 = pressed, 0 = released			CAN BE COMBINED
					// 2	binary mask	Rotary button	binary mask	1 = pressed, 0 = released			
					// 3	binary mask	Rotary delta	 -16383 ... 16384				CANNOT BE COMBINED
					// 4	binary mask	Set leds	0 ... 32768				
					// 5	binary mask		0 ... 32768				
					// 6	binary mask		0 ... 32768				
					// 7	binary mask	Get temperature	 -16383 ... 16384	x 0,1 degrees C			
					// 8	binary mask	Filtered analog value 	0 ... 1023				
						case(CMD_SET_ID):
							for(i=0; i<8; i++)
							{
								if(!(rxMsg.id & (1<<i)))
								{
									// id not taken yet; take it
									id = (1<<i);
									break;
								}
							}
							
							fwdMsg.cmd = CMD_SET_ID; 
							fwdMsg.id = rxMsg.id | id;
							fwdMsg.val = 0;
							doFwd = 1;
							break;
						case(CMD_BTN):
						case(CMD_RT_BTN):
						case(CMD_RT_DELTA(0)):
						case(CMD_RT_DELTA(1)):
						case(CMD_RT_DELTA(2)):
						case(CMD_RT_DELTA(3)):
						case(CMD_ANA):
						case(CMD_TEMP):
							// Simply forward without handling
							fwdMsg.cmd = rxMsg.cmd; 
							fwdMsg.id = rxMsg.id;
							fwdMsg.val = rxMsg.val;
							doFwd = 1;
							break;
						case(CMD_LED_VALUE):
							if(rxMsg.id & id)
							{
								// Handle message
								Led_value(rxMsg.val);
							}

							// Forward message
							fwdMsg.cmd = rxMsg.cmd; 
							fwdMsg.id = rxMsg.id;
							fwdMsg.val = rxMsg.val;
							doFwd = 1;
							break;
						case(CMD_ACT_LED):
							if(rxMsg.id & id)
							{
								// Handle message
								Led_activate();
							}

							// Forward message
							fwdMsg.cmd = rxMsg.cmd; 
							fwdMsg.id = rxMsg.id;
							fwdMsg.val = rxMsg.val;
							doFwd = 1;
							break;
						default:
							// Unknown command!
							break;
				}
				
			}
		}
			
		if(UART_TxEmpty)
		{
			// Ready to send new message 
			if(doFwd)
			{
				doFwd = 0;
				sendMessage(&fwdMsg); 		// Non blocking because of FIFO
			}
			else if(doReply)
			{
				doReply = 0;
				sendMessage(&replyMsg);		// Non blocking because of FIFO
			}
			else if(doNew)
			{
				doNew = 0;
				sendMessage(&newMsg);		// Non blocking because of FIFO
			}
		}
	}
}

static __inline void updateEnc(Encoder_t *enc)
{
	// Rotary position : no debouncing!
	uint8_t tmpA = GPIOGetValue(enc->A.port, enc->A.pin);
	uint8_t tmpB = GPIOGetValue(enc->B.port, enc->B.pin);
	
	if(tmpA && !enc->A.val)		// Risinge edge on A
	{
		if(!tmpB)enc->delta++;
		else enc->delta--;
		enc->irq = 1;
	}
	if(!tmpA && enc->A.val)		// Falling edge on A
	{
		if(tmpB)enc->delta++;
		else enc->delta--;
		enc->irq = 1;
	}
	if(tmpB && !enc->B.val)		// Risinge edge on B
	{
		if(tmpA)enc->delta++;
		else enc->delta--;
		enc->irq = 1;
	}
	if(!tmpB && enc->B.val)		// Falling edge on B
	{
		if(!tmpA)enc->delta++;
		else enc->delta--;
		enc->irq = 1;
	}
	enc->A.val = tmpA;
	enc->B.val = tmpB;
}

static __inline uint8_t updateBtn(Input_t *inp)
{
	uint8_t tmp = GPIOGetValue(inp->port, inp->pin);

	// Respond fast to key press (falling edge), and slowly to release (rising edge) 
	if(!tmp)
	{
		// Input low: key pressed
		inp->debounce = 0;		
		
		// Previously not pressed?
		if(inp->val)
		{
			// Falling edge (key press event)	
			inp->val = tmp;
			return 1;
		}
	}
	else 
	{
		inp->debounce++;
		if(inp->debounce > 2000)	
		{
			inp->debounce = 2000;
			
			// Previously not released?
			if(!inp->val)
			{
				// Rising edge (key release event)
				inp->val = tmp;
				return 1;
			}
		}		
	}
	
	return 0;
}

void sendMessage(Msg_t *msg)
{
	uint8_t arr[5];
	uint8_t *p, i, sum = 0; 
	
	p = (uint8_t*)msg;
	// Calc checksum
	for(i=0;i<4;i++)
	{
		sum+=*p;
		arr[i] = *p++;
	}
	arr[i] = sum & 0x7F;		
	UARTSend( arr, 5 ); 		// Non blocking because of FIFO
}

void SysTick_Handler (void)
{
	systick++;
}

void ClockInit(void)
{
    uint32_t i=31;

    //LPC_SYSCON->WDTOSCCTRL = (0x1<<5) | (0x1F<<0);     //500k/64=7.8kHz, real 8.6kHz
    LPC_SYSCON->PDRUNCFG     &= ~(1 << 5); 	/* Power-up System oscillator */
    for (i = 0; i < 200; i++) __NOP();
	
	LPC_SYSCON->SYSPLLCLKSEL = 1;
	LPC_SYSCON->SYSPLLCLKUEN = 1;
	
	#define MSEL 4 	// M = 5
	#define PSEL 1	// P = 2
	LPC_SYSCON->SYSPLLCTRL    = MSEL |(PSEL<<5);
	LPC_SYSCON->PDRUNCFG     &= ~(1 << 7);          	/* Power-up SYSPLL          */
	while (!(LPC_SYSCON->SYSPLLSTAT & 0x01))__NOP();	/* Wait Until PLL Locked    */
		
    LPC_SYSCON->MAINCLKSEL    = 3;     					/* Select Input clock to system PLL */
    LPC_SYSCON->MAINCLKUEN    = 0x01;               	/* Update MCLK Clock Source */
    LPC_SYSCON->MAINCLKUEN    = 0x00;               	/* Toggle Update Register   */
    LPC_SYSCON->MAINCLKUEN    = 0x01;
    while (!(LPC_SYSCON->MAINCLKUEN & 0x01));       	/* Wait Until Updated       */


    LPC_SYSCON->SYSAHBCLKDIV  = 1;	   //divide by 1
    //                           sys	rom		ram	 flashreg flasha gpio	iocon
    LPC_SYSCON->SYSAHBCLKCTRL = (1<<0)|(1<<1)|(1<<2)|(1<<3)|(1<<4)|(1<<6)|(1<<16);
    LPC_SYSCON->UARTCLKDIV    = 0;	   //divide by 1
    LPC_SYSCON->SSP0CLKDIV    = 0;
    LPC_SYSCON->SSP1CLKDIV    = 0;
}
	
void Led_value(uint16_t value)
{
	uint8_t led_nr = value >> 8;
	uint8_t led_val = value & 0xFF;	

	if(led_nr < 12)
	{
		led[led_nr] = ((float)led_val) / 255.0;
	}
}

void Led_activate()
{
	int i;
	for(i=0;i<12;i++)
	{
		LedPWM(i, led[i]);
	}
}

void IO_Init()
{	
	GPIOSetDir(2, 4, 0);		// BTN1 				// port num, bit position, direction (1 out, 0 input)
	LPC_IOCON->PIO2_4 = PULL_UP | FUNC0;
	
	GPIOSetDir(2, 5, 0);		// BTN2
	LPC_IOCON->PIO2_5 = PULL_UP | FUNC0;
	
	GPIOSetDir(3, 1, 0);		// BTN3
	LPC_IOCON->PIO3_1 = PULL_UP | FUNC0;
	
	GPIOSetDir(2, 7, 0);		// BTN4
	LPC_IOCON->PIO2_7 = PULL_UP | FUNC0;
	
	GPIOSetDir(2, 10, 0);		// E1_I
	LPC_IOCON->PIO2_10 = PULL_UP | FUNC0;
	
	GPIOSetDir(2, 2, 0);		// E2_I
	LPC_IOCON->PIO2_2 = PULL_UP | FUNC0;

	GPIOSetDir(3, 2, 0);		// E3_I
	LPC_IOCON->PIO3_2 = PULL_UP | FUNC0;

	GPIOSetDir(3, 4, 0);		// E4_I	
	LPC_IOCON->PIO3_4 = PULL_UP | FUNC0;
	
	GPIOSetDir(0, 7, 0);		// E1_A
	LPC_IOCON->PIO0_7 = PULL_UP | FUNC0;
	
	GPIOSetDir(0, 6, 0);		// E2_A
	LPC_IOCON->PIO0_6 = PULL_UP | FUNC0;
	
	GPIOSetDir(3, 0, 0);		// E3_A
	LPC_IOCON->PIO3_0 = PULL_UP | FUNC0;
	
	GPIOSetDir(2, 11, 0);		// E4_A
	LPC_IOCON->PIO2_11 = PULL_UP | FUNC0;
	
	GPIOSetDir(2, 9, 0);		// E1_B
	LPC_IOCON->PIO2_9 = PULL_UP | FUNC0;
	
	GPIOSetDir(2, 1, 0);		// E2_B
	LPC_IOCON->PIO2_1 = PULL_UP | FUNC0;
	
	GPIOSetDir(2, 3, 0);		// E3_B
	LPC_IOCON->PIO2_3 = PULL_UP | FUNC0;
	
	GPIOSetDir(3, 3, 0);		// E4_B
	LPC_IOCON->PIO3_3 = PULL_UP | FUNC0;
}

void LedPWM(uint8_t led, float pwm)
{
	uint32_t u32pwm = PWM_PERIOD;
	u32pwm -= (pwm * PWM_PERIOD);

	switch(led)
	{
		case(LEDR1):
			LPC_TMR16B0->MR2 = u32pwm;
			break;
		case(LEDR2):
			LPC_TMR32B0->MR2 = u32pwm;
			break;
		case(LEDR3):
			LPC_TMR32B0->MR1 = u32pwm;	// soft PWM!
			if(pwm == 0.0)
			{
				GPIOSetValue(2, 6, 0);
			}
			break;
		case(LEDR4):
			LPC_TMR32B0->MR3 = u32pwm;
			break;
		case(LEDG1):
			LPC_TMR32B1->MR1 = u32pwm;
			break;
		case(LEDG2):
			LPC_TMR16B0->MR0 = u32pwm;
			break;
		case(LEDG3):
			LPC_TMR16B1->MR2 = u32pwm;	// soft PWM!
			if(pwm == 0.0)
			{
				GPIOSetValue(1, 5, 0);
			}
			break;
		case(LEDG4):
			LPC_TMR16B1->MR1 = u32pwm;
			break;
		case(LEDB1):
			LPC_TMR32B1->MR0 = u32pwm;
			break;
		case(LEDB2):
			LPC_TMR16B1->MR0 = u32pwm;
			break;
		case(LEDB3):
			LPC_TMR32B1->MR2 = u32pwm;
			break;
		case(LEDB4):
			LPC_TMR16B0->MR1 = u32pwm;
			break;
		default:
			break;
	}
}

void LedsInit(void)
{
#ifndef JTAG	
	LPC_IOCON->SWCLK_PIO0_10 = FUNC3;				// LEDR1 0	// P0.10	T16B0_MAT2	 	+ SWCLK
#endif	
	LPC_IOCON->PIO0_1 =	FUNC2;						// LEDR2 1	// P0.1		T32B0_MAT2		+ ISP
	LPC_IOCON->PIO2_6 =	FUNC0;						// LEDR3 2	// P2.6		PIO2_6 !!! ==> soft pwm on CT32B0_MAT1
	GPIOSetDir(2, 6, 1);
	GPIOSetValue(2, 6, 0);
	LPC_IOCON->R_PIO0_11 = FUNC3|DIGITAL_MODE;		// LEDR4 3	// P0.11	T32B0_MAT3 	// todo (1<<6) ?!
	LPC_IOCON->R_PIO1_2 = FUNC3|DIGITAL_MODE;		// LEDG1 4	// P1.2 	T32B1_MAT1
	LPC_IOCON->PIO0_8 = FUNC2; 						// LEDG2 5 	// P0.8		T16B0_MAT0		
	LPC_IOCON->PIO1_5 =	FUNC0;						// LEDG3 6	// P1.5		PIO1_5 !!! ==> soft pwm on CT16B1_MAT2
	GPIOSetDir(1, 5, 1);
	GPIOSetValue(1, 5, 0);
	LPC_IOCON->PIO1_10 = FUNC2;						// LEDG4 7	// P1.10	T16B1_MAT1
	LPC_IOCON->R_PIO1_1 = FUNC3|DIGITAL_MODE;		// LEDB1 8	// P1.1 	T32B1_MAT0
	LPC_IOCON->PIO1_9 = FUNC1; 						// LEDB2 9	// P1.9		T16B1_MAT0
#ifndef JTAG
	LPC_IOCON->SWDIO_PIO1_3 = FUNC3|DIGITAL_MODE;	// LEDB3 10	// p1.3		T32B1_MAT2		+ SWDIO
#endif	
	LPC_IOCON->PIO0_9 = FUNC2;						// LEDB4 11	// P0.9		T16B0_MAT1
	
	LPC_SYSCON->SYSAHBCLKCTRL |= (1<<7)|(1<<8)|(1<<9)|(1<<10); 	// Enable clock to CT16B0/1, CT32B0/1
	
	// T16B0	: G2=MAT0, 	B4=MAT1, 	R1=MAT2, 
	// T16B1	: B2=MAT0, 	G4=MAT1,  	g3s=MAT2
	// T32B0	: 			r3s=MAT1	R2=MAT2, 	R4=MAT3, 
	// T32B1	: B1=MAT0, 	G1=MAT1, 	B3=MAT2
	
	/* CT16B0 */
	LPC_TMR16B0->EMR = (2<<8)|(2<<6)|(2<<4)|(1<<2)|(1<<1)|(1<<0);	// External match : set high on match 0,1,2 
	LPC_TMR16B0->PWMC = (1<<0)|(1<<1)|(1<<2);	// Enable the PWM on match 0,1&2 
	LPC_TMR16B0->MR0 = 0xFFFFFFFF;
	LPC_TMR16B0->MR1 = 0xFFFFFFFF;
	LPC_TMR16B0->MR2 = 0xFFFFFFFF;
	LPC_TMR16B0->MR3 = PWM_PERIOD;				
	LPC_TMR16B0->MCR = (1<<10);					// Reset on MR3 
	LPC_TMR16B0->CCR = 0; 						// Timer mode
	LPC_TMR16B0->TCR = (1<<1);					// reset counter and prescaler
	LPC_TMR16B0->TCR = (1<<0);					// enable timer and release reset
	
	/* CT16B1 : using irq */
	LPC_TMR16B1->EMR = (2<<6)|(2<<4)|(1<<1)|(1<<0);	// External match : set high on match 0,1 
	LPC_TMR16B1->PWMC = (1<<0)|(1<<1);			// Enable the PWM on match 0,1&2 
	LPC_TMR16B1->MR0 = 0xFFFFFFFF;
	LPC_TMR16B1->MR1 = 0xFFFFFFFF;
	LPC_TMR16B1->MR2 = 0xFFFFFFFF;
	LPC_TMR16B1->MR3 = PWM_PERIOD;				
	LPC_TMR16B1->MCR = (1<<6)|(1<<9)|(1<<10);	// Reset on MR3, interrupt on MR3 && MR2 
	LPC_TMR16B1->CCR = 0; 						// Timer mode
	LPC_TMR16B1->TCR = (1<<1);					// reset counter and prescaler
	LPC_TMR16B1->TCR = (1<<0);					// enable timer and release reset
	
	/* CT32B0 : using irq */
	LPC_TMR32B0->EMR = (2<<10)|(2<<8)|(1<<3)|(1<<2);	// External match : set high on match 2,3
	LPC_TMR32B0->PWMC = (1<<2)|(1<<3);			// Enable the PWM on match 2&3 
	LPC_TMR32B0->MR0 = PWM_PERIOD;
	LPC_TMR32B0->MR1 = 0xFFFFFFFF;
	LPC_TMR32B0->MR2 = 0xFFFFFFFF;
	LPC_TMR32B0->MR3 = 0xFFFFFFFF;				
	LPC_TMR32B0->MCR = (1<<0)|(1<<1)|(1<<3);	// Reset on MR0, interrupt on MR0 && MR1 
	LPC_TMR32B0->CCR = 0; 						// Timer mode
	LPC_TMR32B0->TCR = (1<<1);					// reset counter and prescaler
	LPC_TMR32B0->TCR = (1<<0);					// enable timer and release reset

	/* CT32B1 */
	LPC_TMR32B1->EMR = (2<<8)|(2<<6)|(2<<4)|(1<<2)|(1<<1)|(1<<0);	// External match : set high on match 0,1,2 
	LPC_TMR32B1->PWMC = (1<<0)|(1<<1)|(1<<2);	// Enable the PWM on match 0,1&2 
	LPC_TMR32B1->MR0 = 0xFFFFFFFF;
	LPC_TMR32B1->MR1 = 0xFFFFFFFF;
	LPC_TMR32B1->MR2 = 0xFFFFFFFF;
	LPC_TMR32B1->MR3 = PWM_PERIOD;				
	LPC_TMR32B1->MCR = (1<<10);					// Reset on MR3 
	LPC_TMR32B1->CCR = 0; 						// Timer mode
	LPC_TMR32B1->TCR = (1<<1);					// reset counter and prescaler
	LPC_TMR32B1->TCR = (1<<0);					// enable timer and release reset
	
	NVIC_EnableIRQ(TIMER_16_1_IRQn);    // Enable CT16B1 Interrupt       
	NVIC_EnableIRQ(TIMER_32_0_IRQn);    // Enable CT32B0 Interrupt       
}

__irq void TIMER16_1_IRQHandler(void)
{
	if (LPC_TMR16B1->IR & (1<<3))	// Match 3 irq? : timer reset
	{
		// Clear pending interrupt
		LPC_TMR16B1->IR |=(1<<3);
		
		// Led G3 off (if not 100% on)
		if(led[6] != 1.0 )GPIOSetValue(1, 5, 0);
	}
	if (LPC_TMR16B1->IR & (1<<2))	// Match 2 irq? : G3 soft pwm
	{
		// Clear pending interrupt
		LPC_TMR16B1->IR |=(1<<2);
		
		// Led G3 on
		GPIOSetValue(1, 5, 1);
	} 
}

__irq void TIMER32_0_IRQHandler(void)
{
	if (LPC_TMR32B0->IR & (1<<0))	// Match 0 irq? : timer reset
	{
		// Clear pending interrupt
		LPC_TMR32B0->IR |=(1<<0);
		
		// Led R3 off (if not 100% on)
		if(led[2] != 1.0 )GPIOSetValue(2, 6, 0);
	}
	if (LPC_TMR32B0->IR & (1<<1))	// Match 1 irq? : G3 soft pwm
	{
		// Clear pending interrupt
		LPC_TMR32B0->IR |=(1<<1);
		
		// Led R3 on
		GPIOSetValue(2, 6, 1);
	} 
}

/*----------------------------------------------------------------------------
  Function that initializes ADC
 *----------------------------------------------------------------------------*/
void ADC_init (void) 
{
  /* configure PIN GPIO0.11 for AD0 */
  LPC_SYSCON->SYSAHBCLKCTRL |= ((1UL <<  6) |   /* enable clock for GPIO      */
                                (1UL << 16) );  /* enable clock for IOCON     */

  LPC_IOCON->PIO1_11 = (1<<0);         			/* P1.11 is AD7               */
  LPC_GPIO1->DIR &= ~(1UL << 11);               /* configure GPIO as input    */

  /* configure ADC */
  LPC_SYSCON->PDRUNCFG      &= ~(1UL <<  4);    /* Enable power to ADC block  */
  LPC_SYSCON->SYSAHBCLKCTRL |=  (1UL << 13);    /* Enable clock to ADC block  */

  LPC_ADC->CR          =  ( 1UL <<  7) |        /* select AD7 pin             */
                          (11UL <<  8) |        /* ADC clock is 50MHz/12 = 4.2MHz / 11 clocks = 378 kHz */
                          ( 1UL << 21);         /* enable ADC                 */ 

  LPC_ADC->INTEN       =  ( 1UL <<  8);         /* global enable interrupt    */

  NVIC_EnableIRQ(ADC_IRQn);                     /* enable ADC Interrupt       */
}

void ADC_start()
{
	LPC_ADC->CR |=  (1UL << 24);          /* Start A/D Conversion               */
}

void ADC_IRQHandler(void) 
{
	unsigned int ADC_reg;

	ADC_reg = LPC_ADC->STAT;              /* Read ADC status clears interrupt   */
	ADC_reg = LPC_ADC->GDR;               /* Read conversion result             */
	AD7_val = (ADC_reg >> 6) & 0x3FF;     /* Store converted value              */
}
