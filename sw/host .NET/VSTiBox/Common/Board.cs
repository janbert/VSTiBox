using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VSTiBox
{
    public class Board
    {
        private SerialPortInterface mInterface;
        private int mBoardID  = 0; 

        public Board(SerialPortInterface serialInterface, int id)
        {
            mInterface = serialInterface;
            mInterface.MessageReceived += messageReceived;
            mBoardID = id;
            Buttons = new Button[4] { new Button(), new Button(), new Button(), new Button() };
            Encoders = new Encoder[4] { new Encoder(), new Encoder(), new Encoder(), new Encoder() };
            Leds = new Led[4] { new Led(mInterface, 0, id), new Led(mInterface, 1, id), new Led(mInterface, 2, id), new Led(mInterface, 3, id) };
        }

        public Button[] Buttons { get; private set; }
        public Encoder[] Encoders { get; private set; }
        public Led[] Leds { get; private set; }
        public double Temperature { get; private set; }

        public UInt16 Analog { get; private set; }

        public event TemperatueChangedDelegate TemperatureChanged;

        public event AnalogChangedDelegate AnalogChanged;

        public void ActivateLeds()
        {
            mInterface.WriteMessage(new SerialMessage(Command.LED_ACT, (byte)mBoardID, 0));
        }

        public void LedsOff()
        {
            foreach(Led led in Leds)
            {
                led.R = 0;
                led.G = 0;
                led.B = 0;
            }
            ActivateLeds();
        }


        void messageReceived(object sender, SerialMessageReceivedEventArgs e)
        {
            mInterface = (SerialPortInterface)sender;
            if (e.Message.ID != (byte)mBoardID)
            {
                return;
            }

            //byte[] swapped = new byte[]{e.Message.Data[1] ,e.Message.Data[0]};
            UInt16 u16dat = BitConverter.ToUInt16(e.Message.Data, 0);
            Int16 i16dat = BitConverter.ToInt16(e.Message.Data, 0);

            switch (e.Message.Command)
            {
                case Command.BTN:
                    Buttons[0].Set(!isBitSet(u16dat, 0));
                    Buttons[1].Set(!isBitSet(u16dat, 1));
                    Buttons[2].Set(!isBitSet(u16dat, 2));
                    Buttons[3].Set(!isBitSet(u16dat, 3));
                    break;
                case Command.RT_BTN:
                    Encoders[0].Button.Set(!isBitSet(u16dat, 0));
                    Encoders[1].Button.Set(!isBitSet(u16dat, 1));
                    Encoders[2].Button.Set(!isBitSet(u16dat, 2));
                    Encoders[3].Button.Set(!isBitSet(u16dat, 3));
                    break;
                case Command.RT_DELTA1:
                    Encoders[0].Delta = i16dat;
                    break;
                case Command.RT_DELTA2:
                    Encoders[1].Delta = i16dat;
                    break;
                case Command.RT_DELTA3:
                    Encoders[2].Delta = i16dat;
                    break;
                case Command.RT_DELTA4:
                    Encoders[3].Delta = i16dat;
                    break;
                case Command.TEMP:
                    Temperature = ((double)i16dat) / 10.0;
                    if (TemperatureChanged != null)
                    {
                        TemperatureChanged(this, Temperature);
                    }
                    break;
                case Command.ANA:
                    Analog = u16dat;
                    if (AnalogChanged != null)
                    {
                        AnalogChanged(this, Analog);
                    }
                    break;
                default:
                    // Do nothing
                    break;
            }
        }

        private Boolean isBitSet(UInt16 data, int bit)
        {
            int comp = (1 << bit);
            int res = (int)data & comp;
            return res > 0;
        }
    }
}
