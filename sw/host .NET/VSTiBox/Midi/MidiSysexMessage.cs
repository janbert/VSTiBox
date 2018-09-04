using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSTiBox
{
    class MidiSysexMessage
    {
        public byte[] Data { get; set; }
        public UInt32 DwChannelGroup { get; set; }
        public Int64 Rt { get; set; }
        public MidiSysexMessage(byte[] data, UInt32 dwChannelGroup, Int64 rt)
        {
            Data = data;
            DwChannelGroup = dwChannelGroup;
            Rt = rt;
        }

        public override string ToString()
        {
            return string.Format("{0} ch {1} rt {2}", ByteArrayToString(Data), DwChannelGroup, Rt);
        }

        public static string ByteArrayToString(byte[] array)
        {
            StringBuilder hex = new StringBuilder(array.Length * 2);
            foreach (byte b in array)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
    }
}
