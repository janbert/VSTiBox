using System;
using System.Collections.Generic;
using System.Text;
using VSTiBox.Common;

namespace VSTiBox
{
    [Serializable]
    public class ChannelPreset
    {   
        public const int NumberOfEffectPlugins = 8;

        public float Volume;
        public float Pan;               
        public Boolean KeyZoneActive;
        public int KeyZoneLower;
        public int KeyZoneUpper;
        public int Transpose;
        public ControlPedalAction ControlPedalAction;
        public bool NoteDrop;
        public int NoteDropDelayIndex; 
        public MidiChannel MidiChannel;
        public bool SustainEnabled;
        public VSTPreset InstrumentVstPreset;
        public VSTPreset[] EffectVstPresets;

        public ChannelPreset()
        {
            InstrumentVstPreset = new VSTPreset();
            EffectVstPresets = new VSTPreset[NumberOfEffectPlugins];
            for (int i = 0; i < NumberOfEffectPlugins; i++)
            {
                EffectVstPresets[i] = new VSTPreset();
                EffectVstPresets[i].State = PluginState.Empty;
            }
            
            Volume = 1.0f;
            KeyZoneActive = false;
            MidiChannel = Common.MidiChannel.Channel1;
            SustainEnabled = true;
            ControlPedalAction = Common.ControlPedalAction.EffectControl;
            NoteDrop = false;
        }
    }

    [Serializable]
    public class VSTPreset
    {
        public PluginState State;
        public string Name;
        public byte[] Data;                // Chunk or Parameters data
    }
}
