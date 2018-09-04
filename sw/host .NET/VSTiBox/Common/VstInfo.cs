using System;
using System.Collections.Generic;
using System.Text;

namespace VSTiBox
{
    public class VstInfo
    {
        public String Name;
        public string DLLPath;
        public int AudioInputCount;
        public int AudioOutputCount;
        public Boolean CanReceiveVstMidiEvent;
    }
}
