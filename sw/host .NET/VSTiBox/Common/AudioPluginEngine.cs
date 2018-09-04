using Jacobi.Vst.Interop.Host;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows.Forms;
#if NAUDIO_ASIO
#else
using BlueWave.Interop.Asio;
#endif
using Jacobi.Vst.Core;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using NAudio.Wave;
using VSTiBox.Common;

namespace VSTiBox
{
    public unsafe class AudioPluginEngine
    {
        const int NrOfPluginChannels = 8;

#if NAUDIO_ASIO
        NAudio.Wave.AsioOut mAsio;
        NAudio.Wave.BufferedWaveProvider mWaveProvider;
        byte[] mAsioLeftInt32LSBBuff;
        byte[] mAsioRightInt32LSBBuff;
#else
        private AsioDriver mAsio;
        Channel mOutputLeft;
        Channel mOutputRight;

#endif
        private bool mIsAsioRunning = false;
        private int mAsioBuffSize;
        private bool mFirstAsioBufferUpdateHandlerCall;
        private Thread mAsioThread;
        private ManualResetEventSlim mAsioStopEvent = new ManualResetEventSlim(false);
        private ManualResetEventSlim mAsioStopCompleteEvent = new ManualResetEventSlim(false);
        private ManualResetEventSlim mAsioStartEvent = new ManualResetEventSlim(false);
        private List<VstEvent> mFilteredMidiEvents = new List<VstEvent>(1024);
        private VstEvent[] mMidiPanicEvents;
        private bool[] mMidiPanic;
        private object mMP3RecorderLockObj = new object();
        private AsioMP3Recorder mMp3Recorder;
        private float[] mMP3Buff = new float[512 * 2]; // 2 channels
        private float mMaxVolLeft = 0.0f;
        private float mMaxVolRight = 0.0f;
        private Panel mHostScrollPanel;

        public VstPluginChannel[] PluginChannels { get; private set; }
        public VstPluginChannel MasterEffectPluginChannel { get; private set; }

        private SettingsManager mSettingsMgr;
        private VuMeter[] mVUMeters;
        private VstTimeInfo mVstTimeInfo = new VstTimeInfo();
        private float mPanLeft = 1.0f;
        private float mPanRight = 1.0f;
        private Stopwatch mCpuLoadStopWatch = new Stopwatch();
        private int mMaxCpuLoad = 0;
        private MidiDevice mMidiDevice;
        private MidiMessage[] mMidiInMessages = new MidiMessage[512];



        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="settingsMgr"></param>
        /// <param name="channels"></param>
        public AudioPluginEngine(SettingsManager settingsMgr, VuMeter[] meters, Panel hostScrollPanel)
        {
            // Add editor panel to host to allow scrolling
            mHostScrollPanel = hostScrollPanel;
            Panel hostEditorPanel = new Panel();
            hostScrollPanel.Controls.Add(hostEditorPanel);

            PluginChannels = new VstPluginChannel[NrOfPluginChannels];
            mMidiPanic = new bool[NrOfPluginChannels];

            // Create all channel, effect and master effect plugins  
            for (int i = 0; i < NrOfPluginChannels; i++)
            {
                PluginChannels[i] = new VstPluginChannel(settingsMgr.Settings.AsioBufferSize);
                mMidiPanic[i] = false;
            }
            MasterEffectPluginChannel = new VstPluginChannel(settingsMgr.Settings.AsioBufferSize);

            foreach (VstPlugin plugin in PluginChannels.SelectMany(x => x.AllPlugins))
            {
                plugin.EditorPanel = hostEditorPanel;
                // Let controls know if other control loaded an editor
                plugin.OnEditorOpening += chCtrl_OnEditorOpen;
                plugin.OnEditorOpened += chCtrl_OnEditorOpened;
                plugin.OnEditorClosed += chCtrl_OnEditorClose;
            }

            mSettingsMgr = settingsMgr;
            mVUMeters = meters;

            if (mSettingsMgr.Settings.VSTPluginFolders != null)
            {
                foreach (string path in mSettingsMgr.Settings.VSTPluginFolders)
                {
                    addPath(path);
                }
            }

            MasterPan = mSettingsMgr.Settings.MasterPan;

            // Send all notes off on midi channel 1 
            byte[] midiData = new byte[4];
            // http://www.midi.org/techspecs/midimessages.php
            midiData[0] = 176;  // Send all notes off on midi channel 1 
            midiData[1] = 123;  // All Notes Off
            midiData[2] = 0;
            midiData[3] = 0;

            VstMidiEvent midiPanicEvent = new VstMidiEvent(
                /*DeltaFrames*/ 0,
                /*NoteLength*/ 0,
                /*NoteOffset*/  0,
                                midiData,
                /*Detune*/        0,
                /*NoteOffVelocity*/ 127);

            mMidiPanicEvents = new VstEvent[1];
            mMidiPanicEvents[0] = midiPanicEvent;

            mMidiDevice = new MidiDevice();
        }

        public float MasterPan
        {
            set
            {
                mPanLeft = 1.0f - Math.Min(Math.Max(value, 0.0f), 1.0f);
                mPanRight = 1.0f + Math.Min(Math.Max(value, -1.0f), 0.0f);
            }
        }

        public VstInfo[] AvailablePlugins
        {
            get
            {
                return mSettingsMgr.Settings.Instruments.Concat(mSettingsMgr.Settings.Effects).ToArray();
            }
        }

        public VstInfo[] AvailableInstrumentPlugins
        {
            get
            {
                return mSettingsMgr.Settings.Instruments.ToArray();
            }
        }

        public VstInfo[] AvailableEffectPlugins
        {
            get
            {
                return mSettingsMgr.Settings.Effects.ToArray();
            }
        }

        public int MaxCpuLoad
        {
            get
            {
                int ret = mMaxCpuLoad;
                mMaxCpuLoad = 0;
                return ret;
            }
        }

        public string[] MidiInPortNames
        {
            get
            {
                return mMidiDevice.MidiInPortNames;
            }
        }

        public string[] MidiOutPortNames
        {
            get
            {
                return mMidiDevice.MidiOutPortNames;
            }
        }

        public event EventHandler<MidiMessageEventArgs> MidiInMessageReceived
        {
            add
            {
                mMidiDevice.ReceivedMidiMessage += value;
            }
            remove
            {
                mMidiDevice.ReceivedMidiMessage -= value;
            }
        }

        public void MidiPanic()
        {
            for (int i = 0; i < NrOfPluginChannels; i++)
            {
                mMidiPanic[i] = true;
            }
        }

        public void Start()
        {
            if (mIsAsioRunning) return;

            if (mSettingsMgr.Settings.AsioDeviceNr != -1)// AsioDriver.InstalledDrivers
            {
                mCpuLoadStopWatch.Start();
                mAsioStopEvent.Reset();
                mAsioStartEvent.Reset();

                mFirstAsioBufferUpdateHandlerCall = true;
                mAsioThread = new Thread(asioThread);
                mAsioThread.SetApartmentState(ApartmentState.STA);
                mAsioThread.Priority = ThreadPriority.Normal;
                mAsioThread.Start();

                mAsioStartEvent.Wait();
                mIsAsioRunning = true;
            }
        }

        [DllImport("avrt.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr AvSetMmThreadCharacteristics(string taskName, out uint taskIndex);

        [DllImport("avrt.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool AvRevertMmThreadCharacteristics(IntPtr avrtHandle);

#if  NAUDIO_ASIO
#else
        [STAThread]
#endif
        public unsafe void asioThread()
        {
            if (mSettingsMgr.Settings.MidiInDeviceNumbers == null)
            {
                mSettingsMgr.Settings.MidiInDeviceNumbers = new List<int>();
            }
            for (int i = 0; i < mSettingsMgr.Settings.MidiInDeviceNumbers.Count(); i++)
            {
                mMidiDevice.OpenInPort(mSettingsMgr.Settings.MidiInDeviceNumbers[i]);
            }

            try
            {
#if NAUDIO_ASIO
                mAsio = new NAudio.Wave.AsioOut(mSettingsMgr.Settings.AsioDeviceNr);
#else
                mAsio = AsioDriver.SelectDriver(AsioDriver.InstalledDrivers[mSettingsMgr.Settings.AsioDeviceNr]);
#endif
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            if (mAsio != null)
            {
#if NAUDIO_ASIO
                    if (mAsio != null)
                    {
                        mWaveProvider = new NAudio.Wave.BufferedWaveProvider(new NAudio.Wave.WaveFormat(44100, 16, 2));
                        mAsio.InitRecordAndPlayback(mWaveProvider, 2, 44100);
                        mAsio.AudioAvailable += mAsio_AudioAvailable;
                        mAsioBuffSize = mSettingsMgr.Settings.AsioBufferSize;
                    }
#else
                int p = mAsio.BufferSizex.PreferredSize;
                int max = mAsio.BufferSizex.MaxSize;
                int min = mAsio.BufferSizex.MinSize;

                if (mSettingsMgr.Settings.AsioBufferSize < min)
                {
                    mSettingsMgr.Settings.AsioBufferSize = min;
                    mSettingsMgr.SaveSettings();
                }

                if (mSettingsMgr.Settings.AsioBufferSize > max)
                {
                    mSettingsMgr.Settings.AsioBufferSize = max;
                    mSettingsMgr.SaveSettings();
                }
                mAsioBuffSize = mSettingsMgr.Settings.AsioBufferSize;

                // get our driver wrapper to create its buffers
                mAsio.CreateBuffers(mAsioBuffSize);
                // this is our buffer fill event we need to respond to
                mAsio.BufferUpdate += new EventHandler(asio_BufferUpdateHandler);
                mOutputLeft = mAsio.OutputChannels[0];
                mOutputRight = mAsio.OutputChannels[1];
#endif
                // todo: test
                //mMixLeft = new float[mAsioBuffSize];
                //mMixRight = new float[mAsioBuffSize];

                // and off we go

                stopWatchTicksForOneAsioBuffer = Stopwatch.Frequency / (44100 / mAsioBuffSize);
#if NAUDIO_ASIO
                    mAsioLeftInt32LSBBuff = new byte[mAsioBuffSize * 4];
                    mAsioRightInt32LSBBuff = new byte[mAsioBuffSize * 4];
                    mAsio.Play();
#else
                mAsio.Start();
#endif
                mAsioStartEvent.Set();

                // Keep running untill stop request!
                mAsioStopEvent.Wait();
                stopAsio();
            }
            else
            {
                mIsAsioRunning = false;
                mAsioStartEvent.Set();
            }
        }

        public void Stop()
        {
            if (mIsAsioRunning)
            {
                mAsioStopCompleteEvent.Reset();
                mAsioStopEvent.Set();
                mAsioStopCompleteEvent.Wait();
            }
        }

        private void stopPlugin(VstPlugin plugin)
        {
            if (plugin.State == PluginState.Activated)
            {
                plugin.Deactivate();
            }

            if (plugin.State == PluginState.Deactivated)
            {
                plugin.Unload();
            }
        }

        private void stopAsio()
        {
            if (mMp3Recorder != null)
            {
                mMp3Recorder.Close();
            }

            foreach (VstPluginChannel ch in PluginChannels)
            {
                foreach (VstPlugin effectPlugin in ch.EffectPlugins)
                {
                    stopPlugin(effectPlugin);
                }
                stopPlugin(ch.InstrumentPlugin);
            }

            mMidiDevice.CloseAllInPorts();

            if (mAsio != null)
            {
                mAsio.Stop();
                // Wait at least 200 ms for the last buffer update event to have happened
                Thread.Sleep(200);
#if NAUDIO_ASIO
                mAsio.Dispose();
#else
                mAsio.Release();
#endif
                mAsio = null;
            }
            mIsAsioRunning = false;
            mAsioStopCompleteEvent.Set();
        }

        public void AddMp3Recorder(AsioMP3Recorder recorder)
        {
            lock (mMP3RecorderLockObj)
            {
                mMp3Recorder = recorder;
                mMp3Recorder.Closing += mMp3Recorder_Closing;
            }
        }

        void mMp3Recorder_Closing(object sender, EventArgs e)
        {
            lock (mMP3RecorderLockObj)
            {
                mMp3Recorder.Closing -= mMp3Recorder_Closing;
                mMp3Recorder = null;
            }
        }

        private VstEvent[] filterMidiInMessages(int midiInCount, VstInstrumentPlugin plugin)
        {
            //bool active, int lowerNote, int upperNote, int transpose, ControlPedalAction pedal)
            //, ch.KeyZoneLower, ch.KeyZoneUpper, ch.Transpose, ch.ControlPedalAction
            int pitch;
            mFilteredMidiEvents.Clear();

            bool wrongChannel = false;

            for (int i = 0; i < midiInCount; i++)
            {
                MidiMessage msg = mMidiInMessages[i];

                wrongChannel = ((msg.Channel + 1) != (int)plugin.MidiChannel) && (plugin.MidiChannel != MidiChannel.ChannelAll);

                // Add note to selection
                // Copy event
                byte[] data = new byte[4];
                msg.Data.CopyTo(data, 0);
                VstMidiEvent newEv = new VstMidiEvent(
                    /*DeltaFrames*/ 0,
                    /*NoteLength*/ 0,
                    /*NoteOffset*/  0,
                    data,
                    /*Detune*/        0,
                    /*NoteOffVelocity*/ 127);

                // Force to midi channel 1 = 0xn0
                data[0] = (byte)(data[0] & 0xf0);

                if (data[0] == 176 && data[1] == 11)
                {
                    switch (plugin.ControlPedalAction)
                    {
                        case ControlPedalAction.EffectControl:
                            // Re-route foot control to wheel!
                            data[1] = 1;

                            if (plugin.UseExtendedEffectRange)
                            {
                                // Invert direction :)
                                int wheel = data[2] * 2;
                                if (wheel > 255) wheel = 255;
                                data[2] = (byte)(255 - wheel);
                            }
                            else
                            {
                                // Invert direction :)
                                data[2] = (byte)(127 - data[2]);
                            }
                            break;
                        case ControlPedalAction.VolumeControl:
                            // Do nothing; 
                            break;
                        case ControlPedalAction.None:
                            // Next for; do not handle this midi command 
                            continue;
                        default:
                            // Do nothing
                            break;
                    }
                }

                if (msg.IsNoteOn(out pitch))
                {
                    // Note-on event
                    if (wrongChannel || (plugin.State != PluginState.Activated || plugin.KeyZoneActive && (pitch < plugin.KeyZoneLower || pitch > plugin.KeyZoneUpper)))
                    {
                        // Ignore note-on
                        continue;
                    }
                    plugin.Notes[pitch].Pressed = true;
                    plugin.Notes[pitch].Velocity = data[2];
                    newEv.Data[1] += (byte)plugin.Transpose;
                    mFilteredMidiEvents.Add(newEv);
                }
                else if (msg.IsNoteOff(out pitch))
                {
                    // Note-off event
                    if (wrongChannel || (plugin.KeyZoneActive && (pitch < plugin.KeyZoneLower || pitch > plugin.KeyZoneUpper)))
                    {
                        // Ignore note-off
                        continue;
                    }
                    newEv.Data[1] += (byte)plugin.Transpose;
                    if (plugin.Notes[pitch].PressedTime >= plugin.NoteDropDelay)
                    {
                        // Also send note-off message for note-dropped note
                        byte[] noteOffData = new byte[4];
                        noteOffData[0] = 128;
                        noteOffData[1] = (byte)(pitch - 12);
                        noteOffData[2] = plugin.Notes[pitch].Velocity;
                        noteOffData[3] = 112;
                        VstMidiEvent noteOffEvent = new VstMidiEvent(
                            /*DeltaFrames*/ 0,
                            /*NoteLength*/ 0,
                            /*NoteOffset*/  0,
                            noteOffData,
                            /*Detune*/        0,
                            /*NoteOffVelocity*/ 127);
                        mFilteredMidiEvents.Add(noteOffEvent);
                    }
                    plugin.Notes[pitch].Pressed = false;
                    mFilteredMidiEvents.Add(newEv);
                }
                else if (msg.IsSustain())
                {
                    if (plugin.SustainEnabled)
                    {
                        mFilteredMidiEvents.Add(newEv);
                    }
                    continue;
                }
                else
                {
                    // All other events
                    mFilteredMidiEvents.Add(newEv);
                }
            }


            // Process note on times; for now only required when NoteDrop = true
            if (plugin.NoteDrop)
            {
                for (int i = 0; i < 255; i++)
                {
                    if (plugin.Notes[i].Pressed)
                    {
                        plugin.Notes[i].PressedTime++;
                        if (plugin.Notes[i].PressedTime == plugin.NoteDropDelay)
                        {
                            byte[] noteDropData = new byte[4];
                            noteDropData[0] = 144;
                            noteDropData[1] = (byte)(i - 12);
                            noteDropData[2] = plugin.Notes[i].Velocity;
                            noteDropData[3] = 112;
                            VstMidiEvent newEv = new VstMidiEvent(
                                /*DeltaFrames*/ 0,
                                /*NoteLength*/ 0,
                                /*NoteOffset*/  0,
                                noteDropData,
                                /*Detune*/        0,
                                /*NoteOffVelocity*/ 127);
                            mFilteredMidiEvents.Add(newEv);
                        }
                    }
                }
            }

            return mFilteredMidiEvents.ToArray();
        }

#if NAUDIO_ASIO

        private unsafe void mAsio_AudioAvailable(object sender, NAudio.Wave.AsioAudioAvailableEventArgs e)
        {
           
#else

        //private long[] ticksArray = new long[344];
        //private int ticksArrayIndex = 0;
        //private long prevTicks = 0;
        private long stopWatchTicksForOneAsioBuffer = 0;
        /// <summary>
        /// Handle ASIO update event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void asio_BufferUpdateHandler(object sender, EventArgs e)
        {
            mCpuLoadStopWatch.Restart();

            //long elapsedticksoutsidehandler = mCpuLoadStopWatch.ElapsedTicks;
            //ticksArray[ticksArrayIndex++] = elapsedticksoutsidehandler - prevTicks;
            //if (ticksArrayIndex == 344)
            //{
            //    ticksArrayIndex = 0;
            //}
            //prevTicks = elapsedticksoutsidehandler;

#endif
            if (mFirstAsioBufferUpdateHandlerCall)
            {
                uint taskIndex;
                Thread.CurrentThread.Priority = ThreadPriority.Highest;
                IntPtr handle = AvSetMmThreadCharacteristics("Pro Audio", out taskIndex);
                mFirstAsioBufferUpdateHandlerCall = false;
            }

            // Clear buffer
            for (int index = 0; index < mAsioBuffSize; index++)
            {
                mOutputLeft[index] = 0.0f;
                mOutputRight[index] = 0.0f;
            }

            // Dequeue all midi in messages            
            int midiInCount = mMidiDevice.MidiInMessagesCount();

            // Expand array if required
            if (midiInCount > mMidiInMessages.Length)
            {
                mMidiInMessages = new MidiMessage[midiInCount];
            }

            // Now dequeue midi messages
            midiInCount = mMidiDevice.DequeueMidiInMessages(mMidiInMessages, 0, midiInCount);

            for (int i = 0; i < NrOfPluginChannels; i++)
            {
                bool midiPanic = mMidiPanic[i];
                mMidiPanic[i] = false;

                VstPluginChannel channel = PluginChannels[i];
                if (channel.InstrumentPlugin.State == PluginState.Empty)
                {
                    continue;
                }
                else if (channel.InstrumentPlugin.State == PluginState.Unloading)
                {
                    channel.InstrumentPlugin.FinishUnloading();
                    continue;
                }

                // Activated OR deactivated (still receiving note-off messages):

                VstEvent[] filteredMidiEvents = filterMidiInMessages(midiInCount, channel.InstrumentPlugin);

                if (midiPanic)
                {
                    channel.InstrumentPlugin.VstPluginContext.PluginCommandStub.ProcessEvents(mMidiPanicEvents);
                }
                else
                {
                    channel.InstrumentPlugin.VstPluginContext.PluginCommandStub.ProcessEvents(filteredMidiEvents);
                }

                // Get audio from the instrument plugin
                channel.InstrumentPlugin.VstPluginContext.PluginCommandStub.ProcessReplacing(channel.InstrumentPlugin.InputBuffers.Buffers, channel.InstrumentPlugin.OutputBuffers.Buffers);

                bool swappedBuffers = false;

                // Effect plugins
                for (int n = 0; n < VstPluginChannel.NumberOfEffectPlugins; n++)
                {
                    VstPlugin effectPlugin = channel.EffectPlugins[n];

                    if (effectPlugin.State == PluginState.Empty)
                    {
                        continue;
                    }
                    else if (effectPlugin.State == PluginState.Unloading)
                    {
                        effectPlugin.FinishUnloading();
                        continue;
                    }
                    else if (effectPlugin.State == PluginState.Activated)
                    {
                        if (midiPanic)
                        {
                            channel.InstrumentPlugin.VstPluginContext.PluginCommandStub.ProcessEvents(mMidiPanicEvents);
                        }
                        else
                        {
                            channel.InstrumentPlugin.VstPluginContext.PluginCommandStub.ProcessEvents(filteredMidiEvents);
                        }

                        if (!swappedBuffers)
                        {
                            // Take outputbuffer of the previous 'process replacing' as input, and write output to the other (input) buffer.
                            effectPlugin.VstPluginContext.PluginCommandStub.ProcessReplacing(effectPlugin.OutputBuffers.Buffers, effectPlugin.InputBuffers.Buffers);
                            swappedBuffers = true;
                        }
                        else
                        {
                            // Take inputbuffer of the previous 'process replacing' as input, and write output to the other (output) buffer.
                            effectPlugin.VstPluginContext.PluginCommandStub.ProcessReplacing(effectPlugin.InputBuffers.Buffers, effectPlugin.OutputBuffers.Buffers);
                            swappedBuffers = false;
                        }
                    }
                }

                // Now copy to mix
                for (int index = 0; index < mAsioBuffSize; index++)
                {
                    if (swappedBuffers)
                    {
                        // todo: Pan 
                        mOutputLeft[index] += channel.InputBuffers.Raw[0][index] * channel.InstrumentPlugin.Volume;
                        mOutputRight[index] += channel.InputBuffers.Raw[1][index] * channel.InstrumentPlugin.Volume;
                    }
                    else
                    {
                        // todo: Pan 
                        mOutputLeft[index] += channel.OutputBuffers.Raw[0][index] * channel.InstrumentPlugin.Volume;
                        mOutputRight[index] += channel.OutputBuffers.Raw[1][index] * channel.InstrumentPlugin.Volume;
                    }
                }
            }

            //mMidiEventsAll.Free (midiEventsAllCount );
            //mMidiEventsAllExceptNoteOn.Free(midiEventsAllExceptNoteOnCount);

            // Master pan + measure max volume levels + 
            mMaxVolLeft = 0.0f;
            mMaxVolRight = 0.0f;
            for (int index = 0; index < mAsioBuffSize; index++)
            {
                // Master pan
                mOutputLeft[index] *= mPanLeft;
                mOutputRight[index] *= mPanRight;

                // Get max volume levels
                if (mOutputLeft[index] > mMaxVolLeft) mMaxVolLeft = mOutputLeft[index];
                if (mOutputRight[index] > mMaxVolRight) mMaxVolRight = mOutputRight[index];
            }
            mVUMeters[0].Value = mMaxVolLeft;
            mVUMeters[1].Value = mMaxVolRight;

            // todo: [JBK] put back
            //lock (mMP3RecorderLockObj)
            //{
            //    if (mMp3Recorder != null)
            //    {
            //        mMp3Recorder.WriteSamples(mMixLeft, mMixRight, mAsioBuffSize);
            //    }
            //}      

#if NAUDIO_ASIO
            // Copy mix            
            for (int index = 0; index < mMixLeft.Length; index++)
            {
                // First copy left sample
                Buffer.BlockCopy(sampleToInt32Bytes(mMixLeft[index]), 0, mAsioLeftInt32LSBBuff, index * 4, 4);
                // Then copy right sample
                Buffer.BlockCopy(sampleToInt32Bytes(mMixRight[index]), 0, mAsioRightInt32LSBBuff, index * 4, 4);
            }

            // Copy left buff
            Marshal.Copy(mAsioLeftInt32LSBBuff, 0, e.OutputBuffers[0], e.SamplesPerBuffer * 4);

            // Copy right buff
            Marshal.Copy(mAsioRightInt32LSBBuff, 0, e.OutputBuffers[1], e.SamplesPerBuffer * 4);

            mVstTimeInfo.SamplePosition++;
            e.WrittenToOutputBuffers = true;
#else
            // Start buffer calculations for next asio call.
            mVstTimeInfo.SamplePosition++;
#endif
            int cpuLoad = 0;
            long elapsedTicksDuringHandler = mCpuLoadStopWatch.ElapsedTicks;
            cpuLoad = (int)(elapsedTicksDuringHandler * 100 / stopWatchTicksForOneAsioBuffer);
            if (cpuLoad > mMaxCpuLoad)
            {
                mMaxCpuLoad = cpuLoad;
            }
        }

#if NAUDIO_ASIO
        private byte[] sampleToInt32Bytes(float sample)
        {
            // clip
            if (sample > 1.0f)
                sample = 1.0f;
            if (sample < -1.0f)
                sample = -1.0f;
            int i32 = (int)(sample * Int32.MaxValue);
            return BitConverter.GetBytes(i32);
        }
#endif

        private void hostCmdStub_PluginCalled(object sender, PluginCalledEventArgs e)
        {
            HostCommandStub hostCmdStub = (HostCommandStub)sender;

            // can be null when called from inside the plugin main entry point.
            if (hostCmdStub.PluginContext.PluginInfo != null)
            {
                Debug.WriteLine("Plugin " + hostCmdStub.PluginContext.PluginInfo.PluginID + " called:" + e.Message);
            }
            else
            {
                Debug.WriteLine("The loading Plugin called:" + e.Message);
            }
        }


        private Dictionary<VstPluginContext, string> mRecycledPluginContextDictionary = new Dictionary<VstPluginContext, string>();

        private Bank mBank;
        public Bank Bank
        {
            get
            {
                return mBank;
            }
        }

        public void LoadBank(Bank bank)
        {
            mBank = bank;
            bool firstEditorLoaded = false;

            // Make a list of required plugins that can be re-used from currently active plugins. Unload all others
            List<string> requiredPlugins = new List<string>();

            if (mBank != null)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (mBank.Presets[i].InstrumentVstPreset.State != PluginState.Empty)
                    {
                        requiredPlugins.Add(mBank.Presets[i].InstrumentVstPreset.Name);
                    }
                    var res = mBank.Presets[i].EffectVstPresets.Where(n => n.State != PluginState.Empty).Select(x => x.Name);
                    if (res.Count() > 0)
                    {
                        requiredPlugins.AddRange(res.ToArray());
                    }
                }
            }

            // Unload active plugins first             
            for (int i = 0; i < 8; i++)
            {
                foreach (var plugin in PluginChannels[i].AllPlugins)
                {
                    if (plugin.State == PluginState.Activated)
                    {
                        plugin.Deactivate();
                    }
                    if (plugin.State == PluginState.Deactivated)
                    {
                        string pluginName = plugin.PluginName;
                        if (requiredPlugins.Contains(pluginName))
                        {
                            requiredPlugins.Remove(pluginName);
                            // Move pluginContext from plugin class to local dictionary
                            mRecycledPluginContextDictionary.Add(plugin.VstPluginContext, pluginName);
                            // Remove object reference from plugin class
                            plugin.SetVstPluginContext(null,null);
                        }
                        else
                        {
                            plugin.Unload();
                        }
                    }
                }
            }

            // Now load new plugins
            for (int i = 0; i < 8; i++)
            {
                if (mBank != null)
                {
                    ChannelPreset preset = mBank.Presets[i];

                    LoadChannelPreset(PluginChannels[i], preset);

                    // Only the first channel control with a loaded plugin should load an editor.
                    if (preset.InstrumentVstPreset.State != PluginState.Empty && !firstEditorLoaded)
                    {
                        firstEditorLoaded = true;
                        PluginChannels[i].InstrumentPlugin.ShowEditor();
                    }
                }
            }

            if (mRecycledPluginContextDictionary.Count != 0)
            {
                throw new Exception("mRecycledPluginContextDictionary.Count = " + mRecycledPluginContextDictionary.Count);
            }
        }

        public void UnloadBank()
        {
            // Unload active plugins              
            for (int i = 0; i < 8; i++)
            {
                foreach (var plugin in PluginChannels[i].AllPlugins)
                {
                    if (plugin.State == PluginState.Activated)
                    {
                        plugin.Deactivate();
                    }
                    if (plugin.State == PluginState.Deactivated)
                    {
                        plugin.Unload();
                    }
                }
            }
        }

        public void LoadChannelPreset(VstPluginChannel channel, ChannelPreset channelPreset)
        {
            // First load instrument
            if (channelPreset.InstrumentVstPreset.State != PluginState.Empty)
            {
                channel.InstrumentPlugin.SetVstPluginContext( GetVstPluginContext(channelPreset.InstrumentVstPreset.Name),channelPreset.InstrumentVstPreset.Name);
            }

            // Then load all effects            
            for (int i = 0; i < VstPluginChannel.NumberOfEffectPlugins; i++)
            {
                if (channelPreset.EffectVstPresets[i].State != PluginState.Empty)
                {
                    channel.EffectPlugins[i].SetVstPluginContext(GetVstPluginContext(channelPreset.EffectVstPresets[i].Name),channelPreset.EffectVstPresets[i].Name);
                }
            }

            channel.ImportChannelPreset(channelPreset);
        }

        public VstPluginContext GetVstPluginContext(string name)
        {
            VstPluginContext pluginContext = null;

            KeyValuePair<VstPluginContext, string> kvp = mRecycledPluginContextDictionary.FirstOrDefault(x => (string)x.Value == name);
            if (kvp.Key != null)
            {
                // Use recycled VstPluginContext
                pluginContext = ((KeyValuePair<VstPluginContext, string>)kvp).Key;
                // Now remove from dictionary
                mRecycledPluginContextDictionary.Remove(pluginContext);
            }
            else
            {
                // Create new VstPluginContext
                pluginContext = CreateVstPluginContext(name);
            }
            return pluginContext;
        }

        public VstPluginContext CreateVstPluginContext(string pluginName)
        {
            VstPluginContext pluginContext = null;

            string pluginPath = AvailablePlugins.First(x => x.Name == pluginName).DLLPath;

            try
            {
                HostCommandStub hostCmdStub = new HostCommandStub(mVstTimeInfo, mBank);
                hostCmdStub.PluginCalled += new EventHandler<PluginCalledEventArgs>(hostCmdStub_PluginCalled);
                pluginContext = VstPluginContext.Create(pluginPath, hostCmdStub);

                // add custom data to the context
                pluginContext.Set("PluginPath", pluginName);
                pluginContext.Set("HostCmdStub", hostCmdStub);

                // actually open the plugin itself
                pluginContext.PluginCommandStub.Open();
                pluginContext.PluginCommandStub.SetBlockSize(mAsioBuffSize);
                pluginContext.PluginCommandStub.SetSampleRate(44100f);

                // wrap these in using statements to automatically call Dispose and cleanup the unmanaged memory.
                pluginContext.PluginCommandStub.MainsChanged(true);
                pluginContext.PluginCommandStub.StartProcess();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return pluginContext;
        }

        public void SaveBank()
        {
            for (int i = 0; i < 8; i++)
            {
                mBank.Presets[i] = PluginChannels[i].ExportChannelPreset();

                ChannelPreset preset = mBank.Presets[i];

            }
            mSettingsMgr.SaveBank(mBank);
        }



        public void SetMetronomeSample(string path)
        {
            if (File.Exists(path))
            {
                using (AudioFileReader reader = new AudioFileReader(path))
                {
                    int len = (int)reader.Length;
                    var buff = new float[len];
                    reader.Read(buff, 0, len);
                }
            }
        }

        private void addPath(string path)
        {
            Boolean saveSettings = false;
            if (!Directory.Exists(path)) return;

            // Recursive
            foreach (var dir in new DirectoryInfo(path).GetDirectories())
            {
                addPath(dir.FullName);
            }

            foreach (FileInfo file in new DirectoryInfo(path).GetFiles())
            {
                if (file.Extension == ".dll")
                {
                    if (mSettingsMgr.Settings.KnownVSTPluginDlls.Contains(file.FullName))
                    {
                        continue;
                    }

                    try
                    {
                        HostCommandStub tmpHostCmdStub = new HostCommandStub(mVstTimeInfo, null);
                        using (VstPluginContext ctx = VstPluginContext.Create(file.FullName, tmpHostCmdStub))
                        {
                            // add custom data to the context
                            ctx.Set("PluginPath", file.FullName);
                            ctx.Set("HostCmdStub", tmpHostCmdStub);

                            VstInfo info = new VstInfo();

                            info.AudioInputCount = ctx.PluginInfo.AudioInputCount;
                            info.AudioOutputCount = ctx.PluginInfo.AudioOutputCount;
                            info.CanReceiveVstMidiEvent = ctx.PluginCommandStub.CanDo(VstCanDoHelper.ToString(VstPluginCanDo.ReceiveVstMidiEvent)) == VstCanDoResult.Yes;
                            info.Name = file.Name.Replace(".dll", string.Empty); // ctx.PluginCommandStub.GetEffectName(); 
                            info.DLLPath = file.FullName;

                            // Effect?
                            if (info.AudioInputCount > 0 && info.AudioOutputCount > 0)
                            {
                                mSettingsMgr.Settings.Effects.Add(info);
                            }

                            // VSTi?
                            if (info.CanReceiveVstMidiEvent)
                            {
                                mSettingsMgr.Settings.Instruments.Add(info);
                            }

                            if ((ctx.PluginInfo.Flags & VstPluginFlags.ProgramChunks) != VstPluginFlags.ProgramChunks)
                            {
                                MessageBox.Show(string.Format("{0} does not support chunks", info.Name));
                            }
                        }
                    }
                    catch
                    {
                        // Do nothing
                    }

                    // Files to exclude from future search
                    mSettingsMgr.Settings.KnownVSTPluginDlls.Add(file.FullName);
                    saveSettings = true;
                }
            }

            if (saveSettings)
            {
                mSettingsMgr.SaveSettings();
            }
        }

        void chCtrl_OnEditorOpen(object sender, EventArgs e)
        {
            foreach (var plugin in PluginChannels.SelectMany(x => x.AllPlugins))
            {
                if (!plugin.Equals(sender))
                {
                    plugin.CloseEditor();
                }
            }    
        }

        void chCtrl_OnEditorOpened(object sender, EventArgs e)
        { 
            mHostScrollPanel.Visible = true;
            PluginOpenedInEditor = (VstPlugin)sender;
        }

        void chCtrl_OnEditorClose(object sender, EventArgs e)
        {
            PluginOpenedInEditor = null;
        }

        public VstPlugin PluginOpenedInEditor { get; private set; }
    }
}