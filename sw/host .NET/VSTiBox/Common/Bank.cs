using System;
using System.Collections.Generic;
using System.Text;

namespace VSTiBox
{
    public class Bank
    {
        public int Version;
        public string Name;
        public ChannelPreset[] Presets;
        public string PDFFileName;
        public string OnSongFileName;
        public string MultiTrackFileName;
        public float MultiTrackVolume;
        public string ClickTrackFileName;
        public float ClickTrackVolume;
        public int BPM;
    }
}
