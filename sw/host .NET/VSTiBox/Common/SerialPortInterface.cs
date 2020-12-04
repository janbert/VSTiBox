using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.IO;
using System.Windows.Forms;
using System.Threading;
//using Kernel32SerialPort;

namespace VSTiBox
{
    public enum Command
    {
        SET_ID = 0x80,
        BTN = 0x81,
        RT_BTN = 0x82,
        RT_DELTA1 = 0x83,
        RT_DELTA2 = 0x84,
        RT_DELTA3 = 0x85,
        RT_DELTA4 = 0x86,
        LED_VALUE = 0x87,
        LED_ACT = 0x88,
        TEMP = 0x89,
        ANA = 0x8B,
    }

    public class SerialMessage
    {
        public Command Command { get; set; }
        public byte ID { get; set; }
        public byte[] Data { get; set; }
        public byte[] GetBytes()
        {
            byte[] buff = new byte[5];
            buff[0] = (byte)Command;
            buff[1] = ID;
            buff[2] = Data[0];
            buff[3] = Data[1];
            
            byte calc_sum = 0;
            for (int i = 0; i < 4; i++)
            {
                calc_sum += buff[i];
            }
            calc_sum &= 0x7F;
            buff[4] = calc_sum;
            return buff;
        }

        public SerialMessage()
        {

        }

        public SerialMessage(byte[] data)
        {
            Command = (Command)data[0];
            ID = data[1];
            Data = new byte[] { data[2], data[3] };
        }

        public SerialMessage(Command cmd, byte id, UInt16 u16data)
        {
            Command = cmd;
            ID = id;
            Data = BitConverter.GetBytes(u16data);
            // Swap bytes
            Data = new byte[] { Data[1], Data[0] };
        }

        public SerialMessage(Command cmd, byte id, byte data0, byte data1)
        {
            Command = cmd;
            ID = id;
            Data = new byte[] { data0, data1 }; ;
        }

        public int Length { get { return 5; } }
    }

    public class SerialMessageReceivedEventArgs : EventArgs
    {
        public SerialMessage Message;
        public SerialMessageReceivedEventArgs(byte[] data)
        {
            Message = new SerialMessage(data);
        }
    }

    public class SerialPortInterface
    {
        private const int BAUD_RATE = 115200;
        private SerialPort mPort;
        private bool mIsPortNameSet = false;
        private  Thread mWriteThread;
        private  bool mExit = false;
        private BlockingQueue<SerialMessage> mWriteQueue = new BlockingQueue<SerialMessage>(100);
        
        public event EventHandler<SerialMessageReceivedEventArgs> MessageReceived;

        public SerialPortInterface(int timeout)
        {
            mPort = new SerialPort();
            mPort.BaudRate = BAUD_RATE;
            //mPort.ReadTimeout = 100;// timeout;
            //mPort.
        }

        public string PortName
        {
            get
            {
                return mPort.PortName;
            }
            set
            {
                bool isOpen = mPort.IsOpen;
                if (isOpen)
                {
                    lock (mPort)
                    {
                        mPort.Close();
                    }
                }
                mPort.PortName = value;
                mIsPortNameSet = true;
                if (isOpen)
                {
                    try
                    {
                        mPort.Open();
                    }
                    catch
                    {
                        isOpen = false;
                    }
                }
            }
        }

        public void Connect()
        {
            if (!mIsPortNameSet)
            {
                throw new Exception("Cannot connect: PortName not set.");
            }
            
            mPort.Open();
            mPort.DataReceived += mPort_DataReceived;
            mWriteThread = new Thread(writeDequeueHandler);
            mWriteThread.Priority = ThreadPriority.BelowNormal;
            mWriteThread.IsBackground = true;
            mWriteThread.Start();
        }

        public void Disconnect()
        {
            if (mPort.IsOpen)
            {             
                mExit = true;
                mPort.ReadTimeout = 1; 
                mWriteQueue.Clear();
                if (mWriteThread != null && mWriteThread.IsAlive)
                {
                    mWriteThread.Abort();
                }
                Thread.Sleep(100);
                lock (mPort)
                {
                    mPort.Close();
                }                
            }
        }

        public void WriteMessage(SerialMessage msg)
        {
            if (mPort.IsOpen)
            {
                mWriteQueue.Enqueue(msg);
            }
        }

        private void writeDequeueHandler()
        {
            SerialMessage msg;
            while (!mExit)
            {
                if (mWriteQueue.Dequeue(out msg))
                {
                    lock (mPort)
                    {
                        if (mPort.IsOpen)
                        {
                            mPort.Write(msg.GetBytes(), 0, msg.Length);
                            //mPort.Write(msg.GetBytes());
                        }
                    }
                    
                }
            }
        }
        
        void mPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort port = (SerialPort)sender;
            byte[] buf = new byte[port.BytesToRead];
            port.Read(buf, 0, buf.Length);
            foreach (Byte b in buf)
            {
                handleRxByte(b);
            }
        }

        const int MSGSIZE = 4; 
        static byte[] mRxData = new byte[MSGSIZE];
        static int mRxSeq = 0;

        void handleRxByte(byte rx)
        {
            int i = 0; 
            
            switch (mRxSeq)
            {
                case (0):
                    mRxData[0] = rx;
                    if ((mRxData[0] & 0x80) == 0x80)
                    {
                        mRxSeq = 1;
                    }
                    break;

                case (1):
                case (2):
                case (3):
                    mRxData[mRxSeq] = rx;
                    mRxSeq++;
                    break;
                case (4):
                    byte rx_sum = rx;
                    byte calc_sum = 0;
                    for (i = 0; i < MSGSIZE; i++)
                    {
                        calc_sum += mRxData[i];
                    }
                    calc_sum &= 0x7F;
                    if (rx_sum == calc_sum)
                    {
                        if (MessageReceived != null)
                        {
                            MessageReceived(this, new SerialMessageReceivedEventArgs(mRxData));
                        }
                    }
                    mRxSeq = 0;
                    break;
            }
        }
    
        public void ClearReadBuffer()
        {
            try
            {
                mPort.DiscardInBuffer();
            }
            catch
            {
            }
        }
    }
}
