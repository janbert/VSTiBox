using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSTiBox
{
    public class MidiMessage
    {
        public byte[] Data { get; set; }           

        public UInt32 DwMsg
        {
            get
            {
                return BitConverter.ToUInt32(Data, 0);
            }
            set
            {
                Data = BitConverter.GetBytes(value);
            }
        }

        public UInt32 DwChannelGroup { get; set; }
        
        public Int64 Rt { get; set; }

        public MidiMessage(UInt32 dwMsg, UInt32 dwChannelGroup, Int64 rt)
        {
            Data = BitConverter.GetBytes(dwMsg);
            DwChannelGroup = dwChannelGroup;
            Rt = rt;
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3} ch {4} rt {5}", Data[0], Data[1], Data[2], Data[3], DwChannelGroup, Rt);
        }

        public bool IsNoteOn()
        {
            return (Data[0] & 0xf0) == 0x90;
        }
        
        public bool IsNoteOn(out int pitch)
        {
            pitch = Data[1];
            return (Data[0] & 0xf0) == 0x90;
        }
        
        public bool IsNoteOff()
        {
            return (Data[0] & 0xf0) == 0x80;
        }

        public bool IsNoteOff(out int pitch)
        {
            pitch = Data[1];
            return (Data[0] & 0xf0) == 0x80;
        }

        public bool IsSustain()
        {
            return ((Data[0] & 0xf0) == 0xB0) && (Data[1] == 64);
            // Data[2] >= 64 : sustain on
            // Data[2] < 64 : sustain off
        }

        public byte Pitch
        {
            get
            {
                return Data[1];
            }
        }

        public int Channel
        {
            get
            {
                return Data[0] & 0x0f;
            }
        }

    }
}
