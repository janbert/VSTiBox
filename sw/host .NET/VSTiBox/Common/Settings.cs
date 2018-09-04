using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Xml.Serialization;

namespace VSTiBox
{
    public class Settings
    {
        public List<string> VSTPluginFolders;
        public List<string> KnownVSTPluginDlls;
        public List<int> MidiInDeviceNumbers;
        public int MidiOutDeviceNumber;
        public int AsioDeviceNr;
        public int AsioBufferSize;
        public List<string> BankFileNames;          // Based on bank.Name attribute   
        public string SelectedPlayListName;
        public List<PlayList> PlayLists;         
        public List<VstInfo> Instruments;
        public List<VstInfo> Effects; 
        public VSTPreset[] MasterFxInserts;
        public Color ChannelActive;
        public Color ChannelInactive;
        public Color ChannelOff;
        public Color PanicButton;
        public Color MenuButton;
        public Color PDFButton;
        public float MasterPan;                     // Left -1.0 ... 0.0 middle ... 1.0 right        
        
        public string MetronomeSamplePath;
        public string MetronomeSampleFile;

        public string MetronomeAndClickTrackDevice; // ClickTrackVolume in Bank! 
        public float MetronomeVolume;       
        public string MultiTrackDevice;             // MultiTrackVolume in Bank!            
        public string MP3PlayerDevice;        
    }

    public class PlayList
    {
        public string Name { get; set; }
        public List<string> BankNames { get; set; }

        public PlayList ()
        {
            BankNames = new List<string>();
        }
    }
}
