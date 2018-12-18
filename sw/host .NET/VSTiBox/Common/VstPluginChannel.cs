using Jacobi.Vst.Interop.Host;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSTiBox.Common
{
    public class ChannelPresetImportEventArgs : EventArgs
    {
        public ChannelPreset ChannelPreset { get; private set; }

        public ChannelPresetImportEventArgs(ChannelPreset preset)
        {
            ChannelPreset = preset;
        }
    }

    public class VstPluginChannel
    {
        public const int NumberOfEffectPlugins = 4; // todo: dynamic effect plugins array size

        public AudioBufferInfo OutputBuffers { get; set; }
        public AudioBufferInfo InputBuffers { get; set; }

        public VstInstrumentPlugin InstrumentPlugin { get; set; }

        public VstPlugin[] EffectPlugins { get; private set; }

        public event EventHandler<ChannelPresetImportEventArgs> PresetImported;

        public List<VstPlugin> AllPlugins
        {
            get
            {
                List<VstPlugin> allPlugins = new List<VstPlugin>();
                allPlugins.Add(InstrumentPlugin);
                allPlugins.AddRange(EffectPlugins);
                return allPlugins;
            }
        }

        public VstPluginChannel(int asioBuffSize)
        {
            // Create big enough buffers to fit all plugin types
            int inSize = 32; 
            int outSize = 64;

            if (asioBuffSize != 0)
            {
                if (InputBuffers == null || inSize != InputBuffers.Count)
                {
                    InputBuffers = null;    // Dispose first if already existed!
                    InputBuffers = new AudioBufferInfo(inSize, asioBuffSize);
                }

                if (OutputBuffers == null || outSize != OutputBuffers.Count)
                {
                    OutputBuffers = null;    // Dispose first if already existed!
                    OutputBuffers = new AudioBufferInfo(outSize, asioBuffSize);
                }
            }

            InstrumentPlugin = new VstInstrumentPlugin(InputBuffers, OutputBuffers, asioBuffSize);
            EffectPlugins = new VstPlugin[NumberOfEffectPlugins];
            for (int i = 0; i < NumberOfEffectPlugins; i++)
            {
                EffectPlugins[i] = new VstPlugin(InputBuffers, OutputBuffers, asioBuffSize, true);
            }
        }

        public ChannelPreset ExportChannelPreset()
        {
            ChannelPreset preset = new ChannelPreset(); 
            preset.Volume = InstrumentPlugin.Volume;
            preset.Pan = InstrumentPlugin.Pan;
            preset.KeyZoneActive = InstrumentPlugin.KeyZoneActive;
            preset.KeyZoneLower = InstrumentPlugin.KeyZoneLower;
            preset.KeyZoneUpper = InstrumentPlugin.KeyZoneUpper;
            preset.Transpose = InstrumentPlugin.Transpose;
            preset.ExpressionPedalFunction = InstrumentPlugin.ExpressionPedalFunction;
            preset.ExpressionPedalInvert = InstrumentPlugin.ExpressionPedalInvert;
            preset.NoteDrop = InstrumentPlugin.NoteDrop;
            preset.NoteDropDelayIndex = InstrumentPlugin.NoteDropDelayIndex;
            preset.MidiChannel = InstrumentPlugin.MidiChannel;
            preset.SustainEnabled = InstrumentPlugin.SustainEnabled;
            preset.KeyboardVelocityOffset = InstrumentPlugin.KeyboardVelocityOffset;
            preset.KeyboardVelocityGain = InstrumentPlugin.KeyboardVelocityGain;

        preset.InstrumentVstPreset = InstrumentPlugin.VstPreset;

            for(int i=0; i < NumberOfEffectPlugins ; i++)
            {
                preset.EffectVstPresets[i] = EffectPlugins[i].VstPreset;
            }
            return preset;
        }
               
        public void ImportChannelPreset(ChannelPreset preset)
        {
            //int 0...127 value to float 0.0...1.0
            InstrumentPlugin.Volume = preset.Volume;
            InstrumentPlugin.Pan = preset.Pan;
            InstrumentPlugin.KeyZoneActive = preset.KeyZoneActive;
            InstrumentPlugin.KeyZoneLower = preset.KeyZoneLower;
            InstrumentPlugin.KeyZoneUpper = preset.KeyZoneUpper;
            InstrumentPlugin.Transpose = preset.Transpose;
            InstrumentPlugin.ExpressionPedalFunction = preset.ExpressionPedalFunction;
            InstrumentPlugin.ExpressionPedalInvert = preset.ExpressionPedalInvert;
            InstrumentPlugin.NoteDrop = preset.NoteDrop;
            InstrumentPlugin.NoteDropDelayIndex = preset.NoteDropDelayIndex;
            InstrumentPlugin.MidiChannel = preset.MidiChannel;
            InstrumentPlugin.SustainEnabled = preset.SustainEnabled;
            InstrumentPlugin.VstPreset = preset.InstrumentVstPreset;
            InstrumentPlugin.KeyboardVelocityOffset = preset.KeyboardVelocityOffset;
            InstrumentPlugin.KeyboardVelocityGain = preset.KeyboardVelocityGain;

            for (int i = 0; i < NumberOfEffectPlugins; i++)
            {
                EffectPlugins[i].VstPreset = preset.EffectVstPresets[i];
            }

            if (PresetImported != null)
            {
                PresetImported(this, new ChannelPresetImportEventArgs(preset));
            }
        }
    }
}
