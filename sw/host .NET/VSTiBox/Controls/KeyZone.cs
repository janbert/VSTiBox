using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Jacobi.Vst.Core;
using VSTiBox.Common;

namespace VSTiBox.Controls
{
    public partial class KeyZone : UserControl
    {
        private VstInstrumentPlugin mInstrumentPlugin;
        private AudioPluginEngine mPluginManager;
        private List<int> mActiveNotes = new List<int>();

        public KeyZone()
        {
            InitializeComponent();
        }

        public void SetPluginManager(AudioPluginEngine manager)
        {
            mPluginManager = manager;
        }

        private bool mIgnoreEvents = false;
        public void SetInstrumentPlugin(VstInstrumentPlugin plugin)
        {
            mIgnoreEvents = true;

            mActiveNotes.Clear();
            mInstrumentPlugin = plugin;
            nudTranspose.Value = mInstrumentPlugin.Transpose;
            rbtnControlPedalEffect.Checked = mInstrumentPlugin.ExpressionPedalFunction == ExpressionPedalFunction.EffectControl;
            cbExpressionInvert.Checked = mInstrumentPlugin.ExpressionPedalInvert;
            rbtnControlPedalVolume.Checked = mInstrumentPlugin.ExpressionPedalFunction == ExpressionPedalFunction.VolumeControl;
            rbtnControlPedalNone.Checked = mInstrumentPlugin.ExpressionPedalFunction == ExpressionPedalFunction.None;
            cbNoteDrop.Checked = mInstrumentPlugin.NoteDrop;
            comboNoteDropDelay.Enabled = mInstrumentPlugin.NoteDrop;
            comboNoteDropDelay.SelectedIndex = mInstrumentPlugin.NoteDropDelayIndex;
            cbSustain.Checked = mInstrumentPlugin.SustainEnabled;
            nudKeyboardVelocityOffset.Value = mInstrumentPlugin.KeyboardVelocityOffset;
            nudKeyboardVelocityGain.Value = (int)(mInstrumentPlugin.KeyboardVelocityGain * 100.0f);

            // Set keys according to channel info
            if (mInstrumentPlugin.KeyZoneActive)
            {
                if (mInstrumentPlugin.KeyZoneLower > Piano.LowestKeyNumer)
                {
                    pianoControl1.TurnRangeKeysOff(Piano.LowestKeyNumer, mInstrumentPlugin.KeyZoneLower - 1);
                }

                pianoControl1.TurnRangeKeysOn(mInstrumentPlugin.KeyZoneLower, mInstrumentPlugin.KeyZoneUpper);

                if (mInstrumentPlugin.KeyZoneUpper < Piano.HighestKeyNumer)
                {
                    pianoControl1.TurnRangeKeysOff(mInstrumentPlugin.KeyZoneUpper + 1, Piano.HighestKeyNumer);
                }
            }
            else
            {
                pianoControl1.TurnAllKeysOff();
            }
            mIgnoreEvents = false;
        }

        private void pianoControl1_PianoKeyDown(object sender, PianoKeyEvent e)
        {
            //            mMidiMgr.RecvMidiMsg(0, 0, Kernel32Midi.ShortMsg.EncodeNoteOn(Kernel32Midi.Channel.Channel1, (Kernel32Midi.Pitch)e.KeyNumber, 100)); 
        }

        private void pianoControl1_PianoKeyUp(object sender, PianoKeyEvent e)
        {
            //          mMidiMgr.RecvMidiMsg(0, 0, Kernel32Midi.ShortMsg.EncodeNoteOff(Kernel32Midi.Channel.Channel1, (Kernel32Midi.Pitch)e.KeyNumber, 100));
        }

        private void keyDown(int nr)
        {
            if (!mActiveNotes.Contains(nr))
            {
                mActiveNotes.Add(nr);
                setRange(true);
            }
        }

        private void keyUp(int nr)
        {
            if (mActiveNotes.Contains(nr))
            {
                mActiveNotes.Remove(nr);
                setRange(false);
            }
        }

        private void setRange(bool down)
        {
            int rangeLower;
            int rangeUpper;

            if (mActiveNotes.Count() == 0)
            {
                //Do nothing
                return;
            }
            else
            {
                if (mActiveNotes.Count() == 1)
                {
                    // Ignore keyup
                    if (!down)
                    {
                        //Do nothing
                        return;
                    }

                    // Range is set to 1 key!
                    rangeLower = mActiveNotes[0];
                    rangeUpper = mActiveNotes[0];

                    //addText(string.Format("Range: {0} only", rangeLower));
                }
                else
                {
                    // Active range selection on
                    rangeLower = mActiveNotes.Min();
                    rangeUpper = mActiveNotes.Max();

                    //addText(string.Format("Range: {0} - {1}", rangeLower, rangeUpper));
                }
            }

            if (rangeLower == Piano.LowestKeyNumer &&
               rangeUpper == Piano.HighestKeyNumer)
            {
                // All notes off. No selection
                mInstrumentPlugin.KeyZoneActive = false;
                pianoControl1.TurnAllKeysOff();
            }
            else
            {
                // Set region keys
                mInstrumentPlugin.KeyZoneActive = true;
                mInstrumentPlugin.KeyZoneLower = rangeLower;
                mInstrumentPlugin.KeyZoneUpper = rangeUpper;

                if (rangeLower > Piano.LowestKeyNumer)
                {
                    pianoControl1.TurnRangeKeysOff(Piano.LowestKeyNumer, rangeLower - 1);
                }

                pianoControl1.TurnRangeKeysOn(rangeLower, rangeUpper);

                if (rangeUpper < Piano.HighestKeyNumer)
                {
                    pianoControl1.TurnRangeKeysOff(rangeUpper + 1, Piano.HighestKeyNumer);
                }
            }
        }

        private void KeyZoneControl_VisibleChanged(object sender, EventArgs e)
        {
            if (mPluginManager != null)
            {
                if (this.Visible)
                {
                    mPluginManager.MidiInMessageReceived += mPluginManager_MidiInMessageReceived; //mMidiMgr_MidiIn;
                }
                else
                {
                    mPluginManager.MidiInMessageReceived -= mPluginManager_MidiInMessageReceived;
                }
            }
        }

        void mPluginManager_MidiInMessageReceived(object sender, MidiMessageEventArgs e)
        {
            int pitch;
            if (e.MidiMessage.IsNoteOn(out pitch))
            {
                this.BeginInvoke(new Action(() => keyDown(pitch)));
            }
            else if (e.MidiMessage.IsNoteOff(out pitch))
            {
                this.BeginInvoke(new Action(() => keyUp(pitch)));
            }
        }

        private void nudTranspose_ValueChanged(object sender, EventArgs e)
        {
            mInstrumentPlugin.Transpose = (int)nudTranspose.Value;
        }

        private void btnAddOctave_Click(object sender, EventArgs e)
        {
            mInstrumentPlugin.Transpose += 12;
            nudTranspose.Value = mInstrumentPlugin.Transpose;
        }

        private void btnSubtractOctave_Click(object sender, EventArgs e)
        {
            mInstrumentPlugin.Transpose -= 12;
            nudTranspose.Value = mInstrumentPlugin.Transpose;
        }

        private void rbtnControlPedalEffect_CheckedChanged(object sender, EventArgs e)
        {
            if (mIgnoreEvents) return;
            if (rbtnControlPedalEffect.Checked) mInstrumentPlugin.ExpressionPedalFunction = ExpressionPedalFunction.EffectControl;
        }

        private void rbtnControlPedalVolume_CheckedChanged(object sender, EventArgs e)
        {
            if (mIgnoreEvents) return;
            if (rbtnControlPedalVolume.Checked) mInstrumentPlugin.ExpressionPedalFunction = ExpressionPedalFunction.VolumeControl;
        }

        private void rbtnControlPedalNone_CheckedChanged(object sender, EventArgs e)
        {
            if (mIgnoreEvents) return;
            if (rbtnControlPedalNone.Checked) mInstrumentPlugin.ExpressionPedalFunction = ExpressionPedalFunction.None;
        }

        private void cbNoteDrop_CheckedChanged(object sender, EventArgs e)
        {
            if (mIgnoreEvents) return;
            mInstrumentPlugin.NoteDrop = cbNoteDrop.Checked;
            comboNoteDropDelay.Enabled = mInstrumentPlugin.NoteDrop;
            if (mInstrumentPlugin.NoteDrop)
            {
                // Default setting: 1
                comboNoteDropDelay.SelectedIndex = 3;
            }
        }

        private void comboNoteDropDelay_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mIgnoreEvents) return;
            mInstrumentPlugin.NoteDropDelayIndex = comboNoteDropDelay.SelectedIndex;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            mActiveNotes.Clear();
            mInstrumentPlugin.KeyZoneActive = false;
            pianoControl1.TurnAllKeysOff();
        }

        private void cbSustain_CheckedChanged(object sender, EventArgs e)
        {
            if (mIgnoreEvents) return;

            mInstrumentPlugin.SustainEnabled = cbSustain.Checked;
        }

        private void cbExpressionInvert_CheckedChanged(object sender, EventArgs e)
        {
            if (mIgnoreEvents) return;
            mInstrumentPlugin.ExpressionPedalInvert = cbExpressionInvert.Checked;
        }

        private void nudKeyboardVelocityOffset_ValueChanged(object sender, EventArgs e)
        {
            mInstrumentPlugin.KeyboardVelocityOffset = (int)nudKeyboardVelocityOffset.Value;
        }

        private void nudKeyboardVelocityGain_ValueChanged(object sender, EventArgs e)
        {
            mInstrumentPlugin.KeyboardVelocityGain = (float)nudKeyboardVelocityGain.Value / 100.0f;
        }
    }
}