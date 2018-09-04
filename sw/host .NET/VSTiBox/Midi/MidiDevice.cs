using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectMidi;
using System.Windows.Forms;
using System.Collections.Concurrent;

namespace VSTiBox
{
     public class MidiMessageEventArgs : EventArgs
    {
        public MidiMessage MidiMessage { get; private set; }

        public MidiMessageEventArgs(MidiMessage msg)
        {
            MidiMessage = msg;
        }
    }

     class MidiSysexMessageEventArgs : EventArgs
    {
        public MidiSysexMessage MidiSysexMessage { get; private set; }

        public MidiSysexMessageEventArgs(MidiSysexMessage msg)
        {
            MidiSysexMessage = msg;
        }
    }

    
    /// <summary>
    /// Supports both DirectSound AND kernel32 midi functionality
    /// </summary>
    class MidiDevice : CReceiver
    {
        const int SYSTEM_EXCLUSIVE_MEM = 48000;
        
        private class MidiPort
        {
            public int Index;
            public string Name;
            public bool IsDirectMidi;

            public MidiPort(int index, string name, bool isDirectMidi)
            {
                Index = index;
                Name = name;
                IsDirectMidi = isDirectMidi;
            }
        }


        private ConcurrentStack<MidiMessage> mMidiEventStack = new ConcurrentStack<MidiMessage>();
       
        public event EventHandler<MidiMessageEventArgs> ReceivedMidiMessage;
        public event EventHandler<MidiSysexMessageEventArgs> ReceivedMidiSysexMessage;

        private List<int>  mOpenInPorts = new List<int> ();
        private List<int> mOpenOutPorts = new List<int> (); 
        private Kernel32Midi.InputDevice mMidiInputDevice;
        private Kernel32Midi.OutputDevice mMidiOutputDevice;

        private CDirectMusic mCDMusic;
        public CInputPort mCInPort;
        public COutputPort mCOutPort;

        private List<MidiPort> mMidiInPorts = new List<MidiPort>();
        private List<MidiPort> mMidiOutPorts = new List<MidiPort>();

        public string[] MidiInPortNames
        {
            get
            {
                return mMidiInPorts.Select(x => x.Name).ToArray();
            }
        }

        public string[] MidiOutPortNames
        {
            get
            {
                return mMidiOutPorts.Select(x => x.Name).ToArray();
            }
        }

        public MidiDevice()
        {
            mCDMusic = new CDirectMusic();
            mCDMusic.Initialize();
            // Initialize ports given the DirectMusic manager object
            try
            {
                // Initialize in ports
                mCInPort = new CInputPort();
                ((CMidiPort)mCInPort).Initialize(mCDMusic);
                CMidiPort midiIn = (CMidiPort)mCInPort;
                uint num = midiIn.GetNumPorts();
                for (uint i = 1; i <= num; i++)
                {
                    INFOPORT info;
                    midiIn.GetPortInfo(i, out info);
                    mMidiInPorts.Add(new MidiPort((int)i, info.szPortDescription + " (DirectSound)", true));
                }

                // Initialize out ports            
                mCOutPort = new COutputPort();
                ((CMidiPort)mCOutPort).Initialize(mCDMusic);
                CMidiPort midiOut = (CMidiPort)mCOutPort;
                num = midiOut.GetNumPorts();
                for (uint i = 1; i <= num; i++)
                {
                    INFOPORT info;
                    midiOut.GetPortInfo(i, out info);
                    mMidiOutPorts.Add(new MidiPort((int)i, info.szPortDescription + " (DirectSound)", true));
                }

            }
            catch (CDMusicException DMEx)
            {
                MessageBox.Show(DMEx.GetErrorDescription() + DMEx.ToString());
            }          

            // get Kernel32 midi in devices
            for (int i = 0; i < Kernel32Midi.InputDevice.InstalledDevices.Count; i++)
            {
                mMidiInPorts.Add(new MidiPort((int)i, Kernel32Midi.InputDevice.InstalledDevices[i].Name + " (Kernel32)", false));
            }

            // get Kernel32 midi out device
            for (int i = 0; i < Kernel32Midi.OutputDevice.InstalledDevices.Count; i++)
            {
                mMidiOutPorts.Add(new MidiPort((int)i, Kernel32Midi.OutputDevice.InstalledDevices[i].Name + " (Kernel32)", false));
            }
        }

        // Overriden functions
        public override void RecvMidiMsg(Int64 rt, UInt32 dwChannel, UInt32 dwBytesRead,
                             Byte[] lpBuffer)
        {
            // Receive sysex message
            MidiSysexMessage msg = new MidiSysexMessage(lpBuffer, dwChannel, rt);
            
            // Fire event if required
            if (null != ReceivedMidiSysexMessage)
            {
                ReceivedMidiSysexMessage(this, new MidiSysexMessageEventArgs (msg));
            }
        }

        public override void RecvMidiMsg(Int64 rt, UInt32 dwChannel, UInt32 dwMsg)
        {
            // Receive normal midi message
            MidiMessage msg = new MidiMessage(dwMsg, dwChannel, rt);
       
            // Put it on the stack
            mMidiEventStack.Push(msg);
            
            // But also fire event if required
            if (null != ReceivedMidiMessage)
            {
                ReceivedMidiMessage(this, new MidiMessageEventArgs(msg));
            }
        }

        void mMidiInputDevice_RawMessage(uint dwMsg, uint dwChannel)
        {
            mMidiEventStack.Push(new MidiMessage(dwMsg, dwChannel, 0));
        }

        public void OpenInPort(int portNr)
        {
            if (!mOpenInPorts.Contains(portNr))
            {
                if (portNr < mMidiInPorts.Count)
                {
                    mOpenInPorts.Add(portNr);
                    MidiPort midiPort = mMidiInPorts[portNr];
                    if (midiPort.IsDirectMidi)
                    {
                        try
                        {
                            INFOPORT info;
                            CMidiPort midiIn = (CMidiPort)mCInPort;
                            midiIn.GetPortInfo((uint)midiPort.Index, out info);
                            bool hasInPort = (HR_DMUS)midiIn.GetPortInfo((uint)midiPort.Index, out info) == HR_DMUS.HR_S_OK;
                            if (!hasInPort)
                            {
                                throw new Exception("Midi-in port apparently does not support midi-in. Right...");
                            }
                            else
                            {
                                mCInPort.ActivatePort(info, SYSTEM_EXCLUSIVE_MEM);
                                mCInPort.SetReceiver(this);
                                // Activates input MIDI message handling 
                                mCInPort.ActivateNotification();
                            }
                        }
                        catch (CDMusicException DMEx)
                        {
                            MessageBox.Show(DMEx.GetErrorDescription() + DMEx.ToString());
                        }
                    }
                    else
                    {
                        mMidiInputDevice = Kernel32Midi.InputDevice.InstalledDevices[midiPort.Index];
                        if (mMidiInputDevice != null)
                        {
                            mMidiInputDevice.Open();
                            mMidiInputDevice.StartReceiving(null);
                            mMidiInputDevice.RawMessage += mMidiInputDevice_RawMessage;
                        }
                    }
                }
            }
        }

        public void CloseAllInPorts()
        {
            int[] portNumbers = mOpenInPorts.ToArray();
            foreach (int portNr in portNumbers )
            {
                CloseInPort(portNr);
            }
        }

        public void CloseInPort(int portNr)
        {
            mOpenInPorts.Remove(portNr);
            MidiPort midiPort = mMidiInPorts[portNr];
                if (midiPort.IsDirectMidi)
                {
                    mCInPort.ReleasePort();
                }
                else
                {
                    if (mMidiInputDevice != null)
                    {
                        mMidiInputDevice.StopReceiving();
                        mMidiInputDevice.Close();
                        mMidiInputDevice.RemoveAllEventHandlers();
                    }
                }
            
        }

        public void OpenOutPort(int portNr)
        {
            if (!mOpenOutPorts.Contains(portNr))
            {
                if (portNr < mMidiOutPorts.Count)
                {
                    mOpenOutPorts.Add(portNr);
                    MidiPort midiPort = mMidiOutPorts[portNr];
                    if (midiPort.IsDirectMidi)
                    {
                        try
                        {
                            INFOPORT info;
                            CMidiPort midiOut = (CMidiPort)mCOutPort;
                            midiOut.GetPortInfo((uint)midiPort.Index, out info);
                            bool hasOutPort = (HR_DMUS)midiOut.GetPortInfo((uint)midiPort.Index, out info) == HR_DMUS.HR_S_OK;
                            if (!hasOutPort)
                            {
                                throw new Exception("Midi-out port apparently does not support midi-out. Right...");
                            }
                            else
                            {
                                mCOutPort.ActivatePort(info, SYSTEM_EXCLUSIVE_MEM);
                            }
                        }
                        catch (CDMusicException DMEx)
                        {
                            MessageBox.Show(DMEx.GetErrorDescription() + DMEx.ToString());
                        }
                    }
                    else
                    {
                        mMidiOutputDevice = Kernel32Midi.OutputDevice.InstalledDevices[midiPort.Index];
                        if (mMidiOutputDevice != null)
                        {
                            mMidiOutputDevice.Open();
                        }
                    }
                }
            }
        }

        public void CloseOutPort(int portNr)
        {
            mOpenOutPorts.Remove(portNr);
            MidiPort midiPort = mMidiOutPorts[portNr];
            if (midiPort.IsDirectMidi)
            {
                mCOutPort.ReleasePort();
            }
            else
            {
                if (mMidiOutputDevice != null)
                {
                    mMidiOutputDevice.Close();
                }
            }
        }

        public int MidiInMessagesCount()
        {
            return mMidiEventStack.Count();
        }

        public int DequeueMidiInMessages(MidiMessage[] items, int startIndex, int count)
        {
            return mMidiEventStack.TryPopRange(items, startIndex, count);
        }

        public MidiMessage[] DequeueMidiInMessages()
        {
            int count = mMidiEventStack.Count();
            if (count == 0)
            {
                return new MidiMessage[] { };
            }
            else
            {
                MidiMessage[] messages = new MidiMessage[count];
                if (count == mMidiEventStack.TryPopRange(messages))
                {
                    return messages;
                }
                else
                {
                    throw new Exception("Cannot dequeue midi messages");
                }
            }
        }
      
        public void WriteMidiMessage(MidiMessage msg, int portNr)
        {
            MidiPort midiPort = mMidiOutPorts[portNr];
            if (midiPort.IsDirectMidi)
            {
                mCOutPort.SendMidiMsg(msg.DwMsg, msg.DwChannelGroup);
            }
            else
            {
                if (mMidiOutputDevice != null)
                {
                    // todo... mMidiOutputDevice.;
                }
            }
        }

        public void WriteSysex(byte[] lpMsg, uint dwLength, uint dwCchannelGroup, int portNr)
        {
            MidiPort midiPort = mMidiOutPorts[portNr];
            if (midiPort.IsDirectMidi)
            {
                mCOutPort.SendMidiMsg(lpMsg, dwLength, dwCchannelGroup);
            }
            else
            {
                // todo... mMidiOutputDevice.;
            }
        }
    }
}
