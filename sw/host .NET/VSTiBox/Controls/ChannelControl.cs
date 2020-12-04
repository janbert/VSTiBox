using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Jacobi.Vst.Interop.Host;
using System.Linq;
using Jacobi.Vst.Core;
using System.Threading;
using VSTiBox.Common;
using VSTiBox.Controls;

namespace VSTiBox
{
    public partial class ChannelControl : UserControl
    {
        private AudioPluginEngine mAudioPluginEngine;      
        private Boolean mIsButtonDown = false;
        private Boolean mIsEncoderButtonDown = false;
        private Board mBoard;
        private int mBoardButtonIndex;
        private int mBoardEncoderIndex;
        private int mCurrentInstrument;
        private Boolean mFxActive;
        private VstPluginChannel mVstPluginChannel;
        bool mIgnoreEvents = true;

        public delegate void DebugMsgDelegate(object source, string text);
        public event DebugMsgDelegate OnDebugMessage;
        public event EventHandler EditKeyzone;
        public event EventHandler OnHighLighted;

        [Browsable(false)]
        public VstPluginChannel PluginChannel
        {
            get
            {
                return mVstPluginChannel;
            }
            set
            {
                if(value != null)
                {
                    mVstPluginChannel = value;
                    mVstPluginChannel.PresetImported += mVstPluginChannel_PresetImported;
                    foreach (VstPlugin plugin in mVstPluginChannel.AllPlugins)
                    {
                        plugin.StateChanged += Plugin_StateChanged;
                    }
                }
            }
        }

        public void SetBoard(Board board, int buttonIndex, int encoderIndex)
        {
            mBoard = board;
            mBoardButtonIndex = buttonIndex;
            mBoardEncoderIndex = encoderIndex;

            board.Buttons[buttonIndex].Pressed += ButtonPressed;
            board.Buttons[buttonIndex].Released += ButtonReleased;
            board.Encoders[encoderIndex].Button.Pressed += EncoderPressed;
            board.Encoders[encoderIndex].Button.Released += EncoderReleased;
            board.Encoders[encoderIndex].DeltaChanged += EncAbsPosChangeHandler;
        }

        public ChannelControl()
        {
            InitializeComponent();
        
            sliderVolume.IntValue = VolumeLevelConverter.GetVolumeStep (1.0f);
            pbOnOff.Image = null;
            pbFxInsert.Image = null;
            pbKeyZone.Image = null;
            cbMidiChannel.SelectedIndex = 1;
            mIgnoreEvents = false;
        }

        private EffectPluginSelectionControl mEffectPluginSelectionControl;
        public EffectPluginSelectionControl EffectPluginSelectionControl 
        {
            get
            {
                return mEffectPluginSelectionControl;
            }
            set
            {
                if (value != null)
                {
                    mEffectPluginSelectionControl = value;
                    mEffectPluginSelectionControl.OnEffectInsertChanged += EffectPluginSelectionControl_OnEffectInsertChanged;
                }
            }
        }

        public void SetAudioPluginEngine(AudioPluginEngine engine)
        {
            mAudioPluginEngine = engine;

            List<string> items = new List<string>();
            items.Add("Empty");
            foreach (VstInfo vst in mAudioPluginEngine.AvailableInstrumentPlugins)
            {
                items.Add(vst.Name);
            }
            cbInstrument.Items = items.ToArray();
        }

        public void ButtonPressed(Button btn)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => ButtonPressed(btn)));
            }
            else
            {
                mIsButtonDown = true;

                SetPanOrVolumeVisible();
            }
        }

        public void ButtonReleased(Button btn)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => ButtonReleased(btn)));
            }
            else
            {
                mIsButtonDown = false;
                SetPanOrVolumeVisible();

                // Change of instrument?
                if (mCurrentInstrument != cbInstrument.SelectedIndex)
                {
                    cbInstrument_ValueChanged(null, null);
                }
                else
                {
                    if (PluginChannel.InstrumentPlugin .State == PluginState.Empty)
                    {
                        // No plugin loaded ; suggest selecting a new instrument
                        if (cbInstrument.IsExpanded)
                        {
                            debugMessage("Collapse");
                            cbInstrument.Collapse();
                        }
                        else
                        {
                            debugMessage("Expand");
                            cbInstrument.Expand();
                        }
                    }
                    else if (PluginChannel.InstrumentPlugin.State == PluginState.Deactivated)
                    {
                        mVstPluginChannel.InstrumentPlugin.Activate();
                    }
                    else
                    {
                        mVstPluginChannel.InstrumentPlugin.Deactivate();
                    }
                }
            }
        }

        public void EncoderPressed(Button btn)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => EncoderPressed(btn)));
            }
            else
            {
                mIsEncoderButtonDown = true;
                SetPanOrVolumeVisible();
            }
        }

        public void EncoderReleased(Button btn)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => EncoderReleased(btn)));
            }
            else
            {
                mIsEncoderButtonDown = false;
                SetPanOrVolumeVisible();
            }
        }

        private void SetPanOrVolumeVisible()
        {
            if (PluginChannel.InstrumentPlugin.State != PluginState.Empty )
            {
                PluginChannel.InstrumentPlugin.ShowEditor();             
                HighlightControl(true);                
            }

            if (mIsEncoderButtonDown && !mIsButtonDown)
            {
                // Show pan button
                sliderPan.Visible = true;
                sliderVolume.Visible = false;
            }
            else
            {
                // Show volume button
                sliderVolume.Visible = true;
                sliderPan.Visible = false;
            }
        }

        private bool mEncFirst = true;
        private int mEncPos = 0;
        private void EncAbsPosChangeHandler(Encoder encoder, int pos)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => EncAbsPosChangeHandler(encoder, pos)));
            }
            else
            {
                if (mEncFirst)
                {
                    mEncFirst = false;
                    mEncPos = pos;
                }
                else
                {
                    int delta = pos - mEncPos;
                    mEncPos = pos;
                    if (delta > -127 && delta < 127)
                    {
                        EncoderChange(-delta); /*Inverted or control v1.3 */
                    }
                }
            }
        }

        public void EncoderChange(int delta)
        {
            if (!mIsButtonDown && !mIsEncoderButtonDown)
            {
                // Change volume
                int vol = sliderVolume.IntValue;
                vol += delta;
                if (vol < 0) vol = 0;
                if (vol > sliderVolume.Max) vol = sliderVolume.Max;
                sliderVolume.IntValue = vol;
                PluginChannel.InstrumentPlugin.Volume = sliderVolume.VolumeValue;
            }
            else if (mIsEncoderButtonDown && !mIsButtonDown)
            {
                // Change Pan
                int pan = sliderPan.IntValue;
                pan += delta;
                if (pan < sliderPan.Min) pan = sliderPan.Min;
                if (pan > sliderPan.Max) pan = sliderPan.Max;
                sliderPan.IntValue = pan;
                PluginChannel.InstrumentPlugin.Pan = (float)pan / (float)(sliderPan.Max);
            }
            else if (mIsButtonDown && !mIsEncoderButtonDown)
            {
                // Change instrument
            }
            else if (mIsButtonDown && mIsEncoderButtonDown)
            {
                // Todo? New functionality?
            }
        }
                
        void mVstPluginChannel_PresetImported(object sender, ChannelPresetImportEventArgs e)
        {
            mIgnoreEvents = true;

            lblName.Text = PluginChannel.InstrumentPlugin.ProgramName;

            sliderVolume.IntValue = VolumeLevelConverter.GetVolumeStep (e.ChannelPreset.Volume);
            sliderPan.IntValue = (int)(e.ChannelPreset.Pan * 127.0f);

            if (this.IsHandleCreated)
            {
                this.BeginInvoke(new Action(() => pbKeyZone.Image = e.ChannelPreset.KeyZoneActive ? Properties.Resources.key_zone_on : Properties.Resources.key_zone_off));
            }

            cbMidiChannel.SelectedIndex = (int)e.ChannelPreset.MidiChannel;

            if (e.ChannelPreset.InstrumentVstPreset.State != PluginState.Empty)
            {
                for (int i = 0; i < cbInstrument.Items.Count(); i++)
                {
                    if (cbInstrument.Items[i] == e.ChannelPreset.InstrumentVstPreset.Name)
                    {
                        cbInstrument.SelectedIndex = i;
                        mCurrentInstrument = i;
                    }
                }
            }
            else
            {
                cbInstrument.SelectedIndex = 0;
                mCurrentInstrument = 0;
            }

            SetPluginStateUI(e.ChannelPreset.InstrumentVstPreset.State);
            mIgnoreEvents = false;
        }

        private void cbInstrument_ValueChanged(object sender, EventArgs e)
        {
            if (mCurrentInstrument != cbInstrument.SelectedIndex && cbInstrument.SelectedIndex != -1)
            {
                if (PluginChannel.InstrumentPlugin.State == PluginState.Activated)
                {
                    PluginChannel.InstrumentPlugin.Deactivate();
                }
                if (PluginChannel.InstrumentPlugin.State == PluginState.Deactivated)
                {
                    PluginChannel.InstrumentPlugin.Unload();

                    // Wait untill state becomes empty
                    while (PluginChannel.InstrumentPlugin.State == PluginState.Unloading)           // TODO: [JBK] bugfix; hangs if asio driver failed to load
                    {
                        Thread.Sleep(50);
                    }
                }
            } 
            
            if (cbInstrument.SelectedIndex == 0)
            {
                // Selected index 0 = empty; 
                //PluginChannel.InstrumentPlugin.SetVstPluginContext(null, null);
            }
            else if (cbInstrument.SelectedIndex > 0)
            {
                // Selected index 0 = empty; 
                // Change of instrument?
                if (mCurrentInstrument != cbInstrument.SelectedIndex)
                {
                    string newInstrumentName = cbInstrument.Items[cbInstrument.SelectedIndex];

                    // Create new preset with activated state
                    ChannelPreset  newPreset = new ChannelPreset();                   
                    newPreset.InstrumentVstPreset.Name = newInstrumentName;
                    newPreset.InstrumentVstPreset.State = PluginState.Activated;
                    newPreset.InstrumentVstPreset.Data = null;
                    mAudioPluginEngine.LoadChannelPreset(mVstPluginChannel, newPreset);                   

                    PluginChannel.InstrumentPlugin.ShowEditor();
                    HighlightControl(true);
                }
            }
            mCurrentInstrument = cbInstrument.SelectedIndex;
        }

        void Plugin_StateChanged(object sender, PluginStateChangeEventArgs e)
        {
            SetPluginStateUI(e.PluginState);
        }

        private void SetPluginStateUI(PluginState state)
        {
            if (this.IsHandleCreated)
            {
                switch (state)
                {
                    case PluginState.Empty:
                        GC.Collect();
                        HighlightControl(false);
                        this.BeginInvoke(new Action(() => pbOnOff.Image = null));
                        this.BeginInvoke(new Action(() => pbFxInsert.Image = null));
                        this.BeginInvoke(new Action(() => pbKeyZone.Image = null));
                        mBoard.Leds[mBoardButtonIndex].SetColor(LedColor.Off);
                        break;
                    case PluginState.Unloading:
                        mBoard.Leds[mBoardButtonIndex].SetColor(LedColor.Red);
                        break;
                    case PluginState.Deactivated:
                        this.BeginInvoke(new Action(() => pbOnOff.Image = Properties.Resources.inactive));
                        mBoard.Leds[mBoardButtonIndex].SetColor(LedColor.InActive);
                        this.BeginInvoke(new Action(() => pbFxInsert.Image = mFxActive ? Properties.Resources.insert_on : Properties.Resources.insert_off));
                        break;
                    case PluginState.Activated:
                        this.BeginInvoke(new Action(() => pbOnOff.Image = Properties.Resources.active));
                        mBoard.Leds[mBoardButtonIndex].SetColor(LedColor.Active);
                        break;
                }

                // todo
                //this.BeginInvoke(new Action(() => pbKeyZone.Image = Vstch   mKeyZoneActive ? Properties.Resources.key_zone_on : Properties.Resources.key_zone_off));
            }
        }

        private void sliderPan_ValueChanged(object sender, EventArgs e)
        {
            // Change Pan
            int pan = sliderPan.IntValue;
            mVstPluginChannel.InstrumentPlugin.Pan = (float)pan / (float)(sliderPan.Max);
        }

        private void sliderVolume_ValueChanged(object sender, EventArgs e)
        {
            // Change volume
            mVstPluginChannel.InstrumentPlugin.Volume = sliderVolume.VolumeValue;
        }

        void debugMessage(string text)
        {
            if (OnDebugMessage != null)
            {
                OnDebugMessage(this, text);
            }
        }

        private void pbKeyZone_Click(object sender, EventArgs e)
        {
            HighlightControl(true);

            if (EditKeyzone != null)
            {
                EditKeyzone(this, null);
            }
        }

        public void HighlightControl(bool value)
        {
            if (value)
            {
                this.BackColor = Color.FromArgb(123, 123, 123);
                sliderPan.BackColor = this.BackColor;
                sliderVolume.BackColor = this.BackColor;

                if(null!=OnHighLighted)
                {
                    OnHighLighted(this, null);
                }
            }
            else
            {
                this.BackColor = Color.FromArgb(23, 23, 23);
                sliderPan.BackColor = this.BackColor;
                sliderVolume.BackColor = this.BackColor;
            }
        }

        private void cbMidiChannel_ValueChanged(object sender, EventArgs e)
        {
            if (!mIgnoreEvents && cbMidiChannel.SelectedIndex != -1)
            {
                PluginChannel.InstrumentPlugin.MidiChannel = (MidiChannel)cbMidiChannel.SelectedIndex;
            }
        }

        void EffectPluginSelectionControl_OnEffectInsertChanged(object sender, bool e)
        {
            if (mFxActive != e)
            {
                mFxActive = e;
                if (mFxActive)
                {
                    pbFxInsert.Image = Properties.Resources.insert_on;
                }
                else
                {
                    pbFxInsert.Image = Properties.Resources.insert_on;
                }
            }
        }

        private void pbFxInsert_Click(object sender, EventArgs e)
        {
            mEffectPluginSelectionControl.ShowEffectInserts(mAudioPluginEngine, PluginChannel.EffectPlugins);
            HighlightControl(true);
        }
    }

    public class Note
    {
        private bool pressed = false;
        public bool Pressed
        {
            get
            {
                return pressed;
            }
            set
            {
                pressed = value;
                if (!value) PressedTime = 0;
            }
        }

        public int PressedTime = 0;     // Note on time
        public byte Velocity;
    }
}
