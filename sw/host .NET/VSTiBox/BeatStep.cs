using System;
using System.Linq;
using System.Threading;

namespace BeatStepController
{
    public class BeatStep
    {
        public class EncoderChangedEventArgs : EventArgs
        {
            public int Index { get; private set; }
            public int Delta { get; private set; }
            public Controller Controller { get; private set; }

            public EncoderChangedEventArgs(int beatStepIndex, Controller controller, int delta)
            {
                Index = beatStepIndex;
                Delta = delta;
                Controller = controller;
            }
        }

        public class PadChangedEventArgs : EventArgs
        {
            public int Index { get; private set; }
            public bool State { get; private set; }
            public Controller Controller { get; private set; }

            public PadChangedEventArgs(int beatStepIndex, Controller controller, bool state)
            {
                Index = beatStepIndex;
                State = state;
                Controller = controller;
            }
        }


        const string MIDI_DEVICE_NAME = "Arturia BeatStep";
        private readonly byte[] ControllerNotes;
        private readonly byte[] ControllerMidiChannel;
        
        private Midi.InputDevice mMidiInput;
        private Midi.OutputDevice mMidiOutput;

        private ManualResetEventSlim mMidiMessageReceivedEvent = new ManualResetEventSlim(false);
        private Midi.SysExMessage mReceivedMidiSysexMessage;
        private ManualResetEventSlim mMidiSysexMessageReceivedEvent = new ManualResetEventSlim(false);
        private PadMode[] mControllerModes;
        private int mBeatstepDeviceIndex;

        public enum Parameter
        {
            Mode = 1,           // Set PadMode
            MidiChannel = 2,    // (vv: channel-1, 0-15)
            MmcCommand = 3,     // Used with PadMode==Mmc
            CC = 3,             // Used with PadMode==SilentCcSwitch/CcSwitch (vv from 0-127)
            Note = 3,           // Used with PadMode==Note (vv from 0-127, C0= 18)
            OffValue = 4,       // (vv from 0-127)
            OnValue = 5,        // (vv from 0-127)
            Behaviour = 6,      // 0=Toggle, 1=Gate. Aftertouch (channel pressure) is transmitted in Gate mode only. 
                                // In Toggle mode, velocity is transmitted with Note On and Note Off messages.
                                // For Encoders: 0=Absolute, 1-3=Relative mode 
                                // 1=centered at 64, turning a knob (slowly) clockwise yields 65, turning it anticlockwise yields 63
                                // 2=centered at 0. For RELATIVE2 it would be 1 and 127 respectively.  
                                // 3=centered at 16.
                                // Turning a knob faster yields higher delta values(knob acceleration).
            PadLed = 16,        // Set pad color           
        }

        public enum PadMode         // Use with Parameter.Mode
        {
            Off = 0,
            SilentCcSwitch = 1,     // switched MIDI CCs (think sustain pedal) without local LED feedback
            MMC = 7,                // MMC start/stop buttons
            CcSwitch = 8,           // switched MIDI CCs (think sustain pedal) with local LED feedback
            Note = 9,               // MIDI note key
            ProgramChange = 0x0B,   // Program Change button            
        }

        public enum PadColor
        {
            Off = 0x00,
            Red = 0x01,
            Pink = 0x11,
            Blue = 0x10
        }

        public enum PadBehaviour
        {
            Toggle = 0, // 0=Toggle, 
            Gate = 1,   // 1=Gate. Aftertouch (channel pressure) is transmitted in Gate mode only. 
                        // In Toggle mode, velocity is transmitted with Note On and Note Off messages.
        }

        public enum Controller
        {
            Encoder0 = 0x20,
            Encoder1 = 0x21,
            Encoder2 = 0x22,
            Encoder3 = 0x23,
            Encoder4 = 0x24,
            Encoder5 = 0x25,
            Encoder6 = 0x26,
            Encoder7 = 0x27,
            Encoder8 = 0x28,
            Encoder9 = 0x29,
            Encoder10 = 0x2a,
            Encoder11 = 0x2b,
            Encoder12 = 0x2c,
            Encoder13 = 0x2d,
            Encoder14 = 0x2e,
            Encoder15 = 0x2f,
            Volume = 0x30,
            StopButton = 0x58,
            StartButton = 0x59,
            CntrlSeqButton = 0x5A,
            ExtSyncButton = 0x5B,
            RecallButton = 0x5C,
            StoreButton = 0x5D,
            ShiftButton = 0x5E,
            ChannelButton = 0x5F,
            Pad0 = 0x70,
            Pad1 = 0x71,
            Pad2 = 0x72,
            Pad3 = 0x73,
            Pad4 = 0x74,
            Pad5 = 0x75,
            Pad6 = 0x76,
            Pad7 = 0x77,
            Pad8 = 0x78,
            Pad9 = 0x79,
            Pad10 = 0x7a,
            Pad11 = 0x7b,
            Pad12 = 0x7c,
            Pad13 = 0x7d,
            Pad14 = 0x7e,
            Pad15 = 0x7f,           
        }

        public enum MmcCommand
        {
            Stop = 1,
            Play = 2,
            DeferredPlay = 3,
            FastForward = 4,
            Rewind = 5,
            RecordStrobe = 6,
            RecordExit = 7,
            RecordReady = 8,
            Pause = 9,
            Eject = 10,
            Chase = 11,
            InListReset = 12,
        }

        public event EventHandler<EncoderChangedEventArgs> EncoderChanged;
        public event EventHandler<PadChangedEventArgs> PadChanged;

        private int GetIputDeviceNumber(int beatStepIndex)
        {
            int beatstepCount = 0;
            for (int i = 0; i < Midi.InputDevice.InstalledDevices.Count; ++i)
            {
                if (Midi.InputDevice.InstalledDevices[i].Name == MIDI_DEVICE_NAME)
                /* beatStepIndex == Midi.InputDevice.InstalledDevices[i].DeviceId */
                {
                    if (beatstepCount == beatStepIndex)
                    {
                        return i;
                    }
                    ++beatstepCount;
                }
            }
            return -1;
        }

        private int GetOutputDeviceNumber(int beatStepIndex)
        {
            // #TODO: fix
            if (AvailableDeviceCount == 2)
            {
                if (beatStepIndex == 1)
                {
                    beatStepIndex = 0;
                }
                else
                {
                    beatStepIndex = 1;
                }
            }

            int beatstepCount = 0;
            for (int i = 0; i < Midi.OutputDevice.InstalledDevices.Count; ++i)
            {
                if (Midi.InputDevice.InstalledDevices[i].Name == MIDI_DEVICE_NAME)
                {
                    if (beatstepCount == beatStepIndex)
                    {
                        return i;
                    }
                    ++beatstepCount;
                }                    
            }
            return -1;
        }

        static public int AvailableDeviceCount
        {
            get
            {
                return Midi.InputDevice.InstalledDevices.Where(x => x.Name.StartsWith(MIDI_DEVICE_NAME)).Count();
            }
        }


        public void Close()
        {
            if (mMidiInput != null && mMidiInput.IsOpen)
            {
                mMidiInput.Close();
            }
            if (mMidiOutput != null && mMidiOutput.IsOpen)
            {
                mMidiOutput.Close();
            }
        }

        public BeatStep()
        {
            int len = (int)Controller.Pad15 + 1; // #TODO  Enum.GetValues(typeof(Controller)).Length;
            mControllerModes = new PadMode[len];
            ControllerNotes = new byte[len];
            ControllerMidiChannel = new byte[len];
            for (int i = 0; i < len; i++)
            {
                mControllerModes[i] = PadMode.MMC;
                ControllerMidiChannel[i] = 0;
            }
        }

        public void Open(int beatstepDeviceIndex)
        {
            mBeatstepDeviceIndex = beatstepDeviceIndex;
            int inputIndex = GetIputDeviceNumber(beatstepDeviceIndex);    
            mMidiInput = Midi.InputDevice.InstalledDevices[inputIndex];
            mMidiInput.Open();
            mMidiInput.StartReceiving(null, true);
            mMidiInput.SysEx += MMidiInput_SysEx;
            mMidiInput.ControlChange += MMidiInput_ControlChange;
            mMidiInput.ProgramChange += MMidiInput_ProgramChange;
            mMidiInput.NoteOn += MMidiInput_NoteOn;
            mMidiInput.NoteOff += MMidiInput_NoteOff;
            mMidiInput.RawMidiMessage += MMidiInput_RawMidiMessage;

            int outputIndex = GetOutputDeviceNumber(beatstepDeviceIndex);
            mMidiOutput = Midi.OutputDevice.InstalledDevices[outputIndex];
            mMidiOutput.Open();

            /* Config encoders and buttons*/
            WriteCommand(Parameter.Behaviour, Controller.Volume, 1); // 1=gate
            WriteCommand(Parameter.CC, Controller.Volume, (byte)Controller.Volume);

            WriteControllerMode(Controller.StopButton, PadMode.CcSwitch);
            WriteControllerMode(Controller.StartButton, PadMode.CcSwitch);
            WriteControllerMode(Controller.CntrlSeqButton, PadMode.CcSwitch);
            WriteControllerMode(Controller.ExtSyncButton, PadMode.CcSwitch);
            WriteControllerMode(Controller.RecallButton, PadMode.CcSwitch);
            WriteControllerMode(Controller.StoreButton, PadMode.CcSwitch);
            WriteControllerMode(Controller.ShiftButton, PadMode.CcSwitch);
            WriteControllerMode(Controller.ChannelButton, PadMode.CcSwitch);

            WriteCommand(Parameter.Behaviour, Controller.StopButton, 1);        // 1=gate
            WriteCommand(Parameter.Behaviour, Controller.StartButton, 1);
            WriteCommand(Parameter.Behaviour, Controller.CntrlSeqButton, 1);
            WriteCommand(Parameter.Behaviour, Controller.ExtSyncButton, 1);
            WriteCommand(Parameter.Behaviour, Controller.RecallButton, 1);
            WriteCommand(Parameter.Behaviour, Controller.StoreButton, 1);
            WriteCommand(Parameter.Behaviour, Controller.ShiftButton, 1);
            WriteCommand(Parameter.Behaviour, Controller.ChannelButton, 1);

            for (int i = 0; i < 16; ++i)
            {
                /* Encoders */
                var encoder = Controller.Encoder0 + i;
                WriteCommand(Parameter.Behaviour, encoder, 1); // 1=gate
                WriteCommand(Parameter.CC, encoder, (byte)encoder);

                /* Pads */
                var pad = Controller.Pad0 + i;
                //WriteNoteCommand(NoteCommand.Behaviour, pad, 1);            // Set gate
                //WriteNoteCommand(NoteCommand.MidiChannel, pad, 0);          // Set midi CH0
                //WriteNoteCommand(NoteCommand.Note, pad, (byte)(24+i));      // Set note C1=24
                
                WriteControllerMode(pad, PadMode.CcSwitch);
               
                /*//WritePadBehaviour(pad, PadBehaviour.Toggle);
                WriteCommand(Parameter.MidiChannel, pad, 0);
                WriteCommand(Parameter.OffValue, pad, 1);
                WriteCommand(Parameter.OnValue, pad, 2);*/
                WriteCommand(Parameter.Behaviour, pad, 1); // 1=gate
                WriteCommand(Parameter.CC, pad, (byte)pad);             
            }
        }

        private void MMidiInput_SysEx(Midi.SysExMessage msg)
        {
            mReceivedMidiSysexMessage = msg;
            mMidiSysexMessageReceivedEvent.Set();
        }

        private void MMidiInput_RawMidiMessage(byte[] data)
        {
         
        }

        private void MMidiInput_NoteOff(Midi.NoteOffMessage msg)
        {
            // #TODO
        }

        private void MMidiInput_NoteOn(Midi.NoteOnMessage msg)
        {
            // #TODO
        }

        private void MMidiInput_ProgramChange(Midi.ProgramChangeMessage msg)
        {
            // #TODO
        }

        private void MMidiInput_ControlChange(Midi.ControlChangeMessage msg)
        {
            Controller ctrl = (Controller)msg.Control;
            if (ctrl >= Controller.Encoder0 && ctrl <= Controller.Volume)
            {
                EncoderChanged?.Invoke(this, new EncoderChangedEventArgs(mBeatstepDeviceIndex, ctrl, msg.Value > 64 ? 1 : -1));
            }
            else if (ctrl >= Controller.Pad0 && ctrl <= Controller.Pad15)
            {
                PadChanged?.Invoke(this, new PadChangedEventArgs(mBeatstepDeviceIndex, ctrl, msg.Value == 127));

            }
        }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="pp">number of the parameter that is to be set – from 1 to 6</param>
            /// <param name="cc">number of the controller</param>
            /// <param name="vv">value of parameter</param>
           private  void WriteCommand(Parameter pp, Controller cc, byte vv)
        {
            byte[] data = new byte[] { 0xF0, 0x00, 0x20, 0x6B, 0x7F, 0x42, 0x02, 0x00, 0x00, 0x00, 0x00, 0xF7 };
            data[8] = (byte)pp;
            data[9] = (byte)cc;
            data[10] = vv;

            mMidiOutput.WriteSysex(data);
        }

        private void WriteGlobalMidiChannel(byte midichannel)
        {
            byte[] data = new byte[] { 0xF0, 0x00, 0x20, 0x6B, 0x7F, 0x42, 0x02, 0x00, 0x50, 0x0B, midichannel, 0xF7 };
            mMidiOutput.WriteSysex(data);
        }

        private void WriteMidiMessage(byte midichannel, byte cmd, byte data0, byte data1)
        {
            UInt32 dwMsg = 0;
            dwMsg = (UInt32)cmd<<4 | midichannel;
            dwMsg |= (UInt32)data0 << 8;
            dwMsg |= (UInt32)data1 << 16;
            //mMidiDevice.WriteMidiMessage(new MidiMessage(dwMsg, 0, 0), 0);
        }

        private bool ReadCommand(byte pp, byte cc, out byte vv)
        {
            byte[] data = new byte[] { 0xF0, 0x00, 0x20, 0x6B, 0x7F, 0x42, 0x01, 0x00, 0x00, 0x00, 0xF7 };
            data[8] = pp;
            data[9] = cc;

            mMidiSysexMessageReceivedEvent.Reset();
            mMidiOutput.WriteSysex(data);
            if (mMidiSysexMessageReceivedEvent.Wait(3000))
            {
                vv = mReceivedMidiSysexMessage.Data[9];
                return true;
            }
            vv = 0;
            return false;
        }

        private void WriteMmcCommand(Controller controller, MmcCommand mmc)
        {
            if (mControllerModes[(int)controller] != PadMode.MMC)
            {
                // Set controller in MMC mode
                WriteCommand(Parameter.Mode, controller, (byte)PadMode.MMC);
            }
            WriteCommand(Parameter.MmcCommand, controller, (byte)mmc);
        }

        public enum CcCommand
        {
            MidiChannel = 2,    // (vv: channel-1, 0-15)
            CC = 3,             // (vv from 0-127)
            OffValue = 4,       // (vv from 0-127)
            OnValue = 5,        // (vv from 0-127)
            Behaviour = 6,      // 0=Toggle, 1=Gate. Aftertouch (channel pressure) is transmitted in Gate mode only. 
                                // In Toggle mode, velocity is transmitted with Note On and Note Off messages.
        }

        public enum ProgramChangeCommand
        {
            MidiChannel = 2, // (vv: channel-1, 0-15)
            ProgramChangeValue = 3, // (0-127)
            BankLsb = 4, // LSB(0-127).
            BankMsb = 5, // MSB(0-127).
        }

        public enum NoteCommand
        {
            MidiChannel = 2,    //(vv: channel-1, 0-15)
            Note = 3,           // (vv from 0-127, C0= 18)
            Behaviour = 6,      // 0=Toggle, 1=Gate. Toggle means that one push switches the button on, 
                                // the next switches it on, while Gate sends On as long as the button is pressed. 
                                // Aftertouch(channel pressure) is transmitted in Gate mode only.In Toggle mode, 
                                // velocity is transmitted with Note On and Note Off messages.
        }

        private void WriteControllerMode(Controller cc, PadMode mode)
        {
            WriteCommand(Parameter.Mode, cc, (byte)mode);
        }

        public void SetPadColor(Controller cc, PadColor color)
        {
            WriteCommand(Parameter.PadLed, cc, (byte)color);
        }

        public void WritePadBehaviour(Controller cc, PadBehaviour behaviour)
        {
            WriteCommand(Parameter.Behaviour, cc, (byte)behaviour);
        }
        
        public void WriteNoteCommand(NoteCommand pp, Controller cc, byte vv)
        {
            if (mControllerModes[(int)cc] != PadMode.Note)
            {
                // Set controller in Note mode
                WriteCommand(Parameter.Mode, cc, (byte)PadMode.Note);
                mControllerModes[(int)cc] = PadMode.Note;
            }

            if (pp == NoteCommand.Note)
            {
                ControllerNotes[(int)cc] = vv;
            }
            else if (pp == NoteCommand.MidiChannel)
            {
                ControllerMidiChannel[(int)cc] = vv;
            }

            byte[] data = new byte[] { 0xF0, 0x00, 0x20, 0x6B, 0x7F, 0x42, 0x02, 0x00, (byte)pp, (byte)cc, vv, 0xF7 };
            mMidiOutput.WriteSysex(data);
        }
    }
}
