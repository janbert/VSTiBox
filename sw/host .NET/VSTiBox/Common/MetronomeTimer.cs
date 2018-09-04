using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSTiBox.Common
{
    class MetronomeTimer
    {
        private MediaTimer mTimer;

        public delegate void TickDelegate();
        public event TickDelegate Tick;

        public bool IsRunning{get;private set;}

        public int BPM 
        {
            set
            {
                try
                {
                    mTimer.Period = BPMToMilliseconds(value);
                }
                catch
                {
                    // Do nothing
                }
            } 
        }


        public MetronomeTimer()
        {
            IsRunning = false;
            mTimer = new MediaTimer();
            mTimer.Resolution = 1;
            mTimer.Period = BPMToMilliseconds(120);
            mTimer.Tick += mTimer_Tick;
        }

        void mTimer_Tick(object sender, EventArgs e)
        {
            if (Tick != null)
            {
                Tick();
            }
        }
        
        public void Start()
        {
            if (!IsRunning)
            {
                IsRunning = true;
                mTimer.Start();
                mTimer_Tick(null, null);
            }
        }

        public void Stop()
        {
            IsRunning = false;
            mTimer.Stop();
        }

        private int BPMToMilliseconds(int BPM)
        {
            return 60000 / BPM;
        }
    }
}
