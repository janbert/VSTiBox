using Jacobi.Vst.Core;
using Jacobi.Vst.Interop.Host;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VSTiBox.Common
{
    public enum ExpressionPedalFunction
    {
        EffectControl,
        VolumeControl,
        None,
    }

    public class VstInstrumentPlugin : VstPlugin
    {
        public int Transpose { get; set; }
        public float Volume { get; set; }  // Range 0.0 ... 1.0 for fast mixing!
        public float Pan { get; set; }

        public bool KeyZoneActive { get; set; }
        public int KeyZoneLower { get; set; }
        public int KeyZoneUpper { get; set; }

        public Note[] Notes = new Note[256];
        public bool NoteDrop { get; set; }
        public int NoteDropDelay { get; set; }

        private int mNotedropDelayIndex;
        public int NoteDropDelayIndex
        {
            get
            {
                return mNotedropDelayIndex;
            }
            set
            {
                mNotedropDelayIndex = value;
                // todo: fix with databind
                int bpm = 120;
                int buffsize = 128;
                double tmp = (44100 / buffsize) * (60.0 / bpm);

                switch (value)
                {
                    case 0: // "1/8th":
                        NoteDropDelay = (int)(tmp / 8);
                        break;
                    case 1: //"1/4th":
                        NoteDropDelay = (int)(tmp / 4);
                        break;
                    case 2: //"1/2th":
                        NoteDropDelay = (int)(tmp / 2);
                        break;
                    case 3: //"1":
                        NoteDropDelay = (int)(tmp);
                        break;
                    case 4: // "2":
                        NoteDropDelay = (int)(tmp * 2);
                        break;
                    case 5: // "4":
                        NoteDropDelay = (int)(tmp * 4);
                        break;
                    default:
                        break;
                }
            }
        }

        public bool SustainEnabled { get; set; }

        public int KeyboardVelocityOffset { get; set; }

        public float KeyboardVelocityGain { get; set; }

        public ExpressionPedalFunction ExpressionPedalFunction { get; set; }

        public bool ExpressionPedalInvert {get;set; }

        public override void Deactivate()
        {
            base.Deactivate();

            foreach (Note note in Notes)
            {
                note.Pressed = false;
            }
        }

        public VstInstrumentPlugin(AudioBufferInfo parentInBuffers, AudioBufferInfo parentOutBuffers, int asioBuffSize) 
            : base(parentInBuffers, parentOutBuffers, asioBuffSize, false) 
        {
            for (int i = 0; i < 256; i++)
            {
                Notes[i] = new Note();
            }
        }
    }

    public class PluginStateChangeEventArgs : EventArgs
    {
        public PluginState PluginState { get; private set; }
     
        public PluginStateChangeEventArgs(PluginState pluginState)
        {
            PluginState = pluginState;
        }
    }
}
