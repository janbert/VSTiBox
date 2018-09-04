using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace VSTiBox
{
    public enum LedColor
    {
        White,
        Red,
        Blue,
        Green, 
        Yellow,
        Orange,
        Pink,
        IceBlue,
        IceGreen,
        Off,
        Active,
        InActive,
    }

    public class Led
    {
        private SerialPortInterface mInterface;
        private int mBoardId;
        private int mLedNumber;

        public void SetColor(LedColor color)
        {
            switch (color)
            {
                case LedColor.White:
                    R = 100; G = 255; B = 95;
                    break;
                case LedColor.Red:
                    R = 255; G = 0; B = 0;
                    break;
                case LedColor.Blue:
                    R = 0; G = 0; B = 255;
                    break;
                case LedColor.Green:
                    R = 0; G = 255; B = 0;
                    break;
                case LedColor.Yellow:
                    R = 132; G = 255; B = 0;
                    break;
                case LedColor.Orange:
                    R = 180; G = 255; B = 0;
                    break;
                case LedColor.Pink:
                    R = 255; G = 100; B = 50;
                    break;
                case LedColor.IceBlue:
                    R = 0; G = 160; B = 255;
                    break;
                case LedColor.IceGreen:
                    R = 18; G = 255; B = 18;
                    break;
                case LedColor.Off:
                    R = 0; G = 0; B = 0;
                    break;
                case LedColor.Active :
                    R = 0; G = 32; B = 16;
                    break;
                case LedColor.InActive:
                    R = 9; G = 24; B = 9;
                    break;
                default:
                    break;
            }
            mInterface.WriteMessage(new SerialMessage(Command.LED_ACT, (byte)mBoardId, 0));
        }

        public Color Color
        {
            set
            {
                R = value.R;
                G = value.G;
                B = value.B;
                mInterface.WriteMessage(new SerialMessage(Command.LED_ACT, (byte)mBoardId, 0));
            }
        }

        public Led(SerialPortInterface serialInterface, int number, int boardID)
        {
            mInterface = serialInterface;
            mLedNumber = number;
            mBoardId = boardID;
        }

        public byte R
        {
            set
            {
                mInterface.WriteMessage(new SerialMessage(Command.LED_VALUE, (byte)mBoardId, value, (byte)mLedNumber));
            }
        }

        public byte G
        {
            set
            {
                mInterface.WriteMessage(new SerialMessage(Command.LED_VALUE, (byte)mBoardId, value, (byte)(mLedNumber + 4)));
            }
        }

        public byte B
        {
            set
            {
                mInterface.WriteMessage(new SerialMessage(Command.LED_VALUE, (byte)mBoardId, value, (byte)(mLedNumber + 8) ));
            }
        }
    }
}
