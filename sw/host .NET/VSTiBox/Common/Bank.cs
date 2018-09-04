using System;
using System.Collections.Generic;
using System.Text;

namespace VSTiBox
{
    public class Bank
    {
        public delegate void BPMChangedDelegate(int bpm);
        public event BPMChangedDelegate BPMChanged;
        
        public string Name;
        public ChannelPreset[] Presets;
        public string PDFFileName;
        public string MultiTrackFileName;
        public float MultiTrackVolume;

        public string ClickTrackFileName;
        public float ClickTrackVolume;
        
        private int mBPM;
        public int BPM
        {
            get
            {
                return mBPM;
            }
            set
            {
                mBPM = value;
                if (BPMChanged != null)
                {
                    BPMChanged(value);
                }
            }
        }
    }
}
