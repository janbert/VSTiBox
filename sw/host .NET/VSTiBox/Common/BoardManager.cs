using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace VSTiBox
{
    public delegate void DeltaChangedDelegate(Encoder enc, int delta);
    public delegate void TemperatueChangedDelegate(Board board, double temperature);
    public delegate void AnalogChangedDelegate(Board board, UInt16 value);

    class BoardManager 
    {
        private SerialPortInterface mInterface;
        private List<string> portNames = new List<string>();
        private Thread mThread;
        private Boolean mPortValid = false;
        private int portNumber = 0;

        public Board[] Boards {get;private set;}
        
        public event EventHandler Connected;

        public Boolean IsConnected
        {
            get;
            private set; 
        }

        public BoardManager()
        {
            IsConnected = false; 
            portNames.AddRange ( System.IO.Ports.SerialPort.GetPortNames());
            mInterface = new SerialPortInterface(-1);
            mInterface.MessageReceived += mSerialPortInterface_MessageReceived;
            if (portNames.Count > 0)
            {
                mInterface.PortName = portNames[0];
                mInterface.Connect();
                mInterface.WriteMessage(new SerialMessage(Command.SET_ID, 0, 0));
                mThread = new Thread(portSelectionThread);
                mThread.IsBackground = true;
                mThread.Priority = ThreadPriority.BelowNormal;
                mThread.Start();
            }
            else
            {
                MessageBox.Show("No COM ports found! Buttons and knobs will not work!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            Boards = new Board[] { new Board(mInterface, 1), new Board(mInterface, 2), new Board(mInterface, 4), new Board(mInterface, 8) };
        }

        public void Close()
        {
            if (mThread != null && mThread .IsAlive )
            {
                mThread.Abort();
            }
            if (IsConnected)
            {
                mInterface.Disconnect();
            }
        }

        public void ActivateLeds()
        {
            int boards = (1 << 0) | (1 << 1) | (1 << 2) | (1 << 3);
            mInterface.WriteMessage(new SerialMessage(Command.LED_ACT, (byte)boards, 0));
        }

        public void LedsOff()
        {
            foreach (Board board in Boards)
            {
                board.LedsOff();
            }
        }


        void portSelectionThread()
        {
            while (!mPortValid)
            {
                Thread.Sleep (1000);
                if(!mPortValid)
                {
                    portNumber++;
                    if (portNumber == portNames.Count)
                    {
                        portNumber = 0;
                    }
                    // Select next COM port 
                    mInterface.PortName = portNames[portNumber];
                    mInterface.WriteMessage(new SerialMessage(Command.SET_ID, 0, 0));
                }
            }
        }

        void mSerialPortInterface_MessageReceived(object sender, SerialMessageReceivedEventArgs e)
        {
            mInterface = (SerialPortInterface)sender;
            mPortValid = true; 

            UInt16 u16dat = BitConverter.ToUInt16(e.Message.Data, 0);
            Int16 i16dat = BitConverter.ToInt16(e.Message.Data, 0);

            if (e.Message.Command == Command.SET_ID)
            {
                IsConnected = true; 
                if (Connected != null)
                {                    
                    Connected(this, null);
                }
            }
        }
    }
}
