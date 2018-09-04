using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VSTiBox.Controls
{
    public partial class Piano : UserControl
    {
        public const int LowestKeyNumer = 21;
        public const int HighestKeyNumer = 108;

        private List<PianoKey> mKeys = new List<PianoKey>();

        public event EventHandler<PianoKeyEvent> PianoKeyDown;
        public event EventHandler<PianoKeyEvent> PianoKeyUp;

        public Piano()
        {
            InitializeComponent();
            mKeys.Add(keyLowA);
            mKeys.Add(keyLowAis);
            mKeys.Add(keyLowB);
            mKeys.AddRange(octave1.Keys);
            mKeys.AddRange(octave2.Keys);
            mKeys.AddRange(octave3.Keys);
            mKeys.AddRange(octave4.Keys);
            mKeys.AddRange(octave5.Keys);
            mKeys.AddRange(octave6.Keys);
            mKeys.AddRange(octave7.Keys);
            mKeys.Add(keyHighC);

            foreach (PianoKey key in mKeys)
            {
                key.MouseUp += key_MouseUp;
                key.MouseDown += key_MouseDown;
            }
        }

        void key_MouseUp(object sender, MouseEventArgs e)
        {
            if (PianoKeyUp != null)
            {
                PianoKeyUp(sender, new PianoKeyEvent(mKeys.IndexOf((PianoKey)sender) + LowestKeyNumer));
            }
        }

        void key_MouseDown(object sender, MouseEventArgs e)
        {
            if (PianoKeyDown != null)
            {
                PianoKeyDown(sender, new PianoKeyEvent(mKeys.IndexOf((PianoKey)sender) + LowestKeyNumer));
            }
        }

        public void TurnKeyOn(int keyNumber)
        {
            keyNumber = Math.Max(keyNumber, LowestKeyNumer);
            keyNumber = Math.Min(keyNumber, HighestKeyNumer);
            keyNumber -= LowestKeyNumer;
            mKeys[keyNumber].TurnKeyOn();
        }

        public void TurnAllKeysOff()
        {
            for (int nr = 0; nr < HighestKeyNumer - LowestKeyNumer + 1; nr++)
            {
                mKeys[nr].TurnKeyOff();
            }
        }

        public void TurnRangeKeysOn(int min, int max)
        {
            min = Math.Max(min, LowestKeyNumer) - LowestKeyNumer;
            max = Math.Min(max, HighestKeyNumer) - LowestKeyNumer;
            
            for (int nr = min; nr <= max; nr++)
            {
                mKeys[nr].TurnKeyOn();
            }
        }

        public void TurnKeyOff(int keyNumber)
        {
            keyNumber = Math.Max(keyNumber, LowestKeyNumer);
            keyNumber = Math.Min(keyNumber, HighestKeyNumer);
            keyNumber -= LowestKeyNumer;
            mKeys[keyNumber].TurnKeyOff();
        }

        public void TurnRangeKeysOff(int min, int max)
        {
            min = Math.Max(min, LowestKeyNumer) - LowestKeyNumer;
            max = Math.Min(max, HighestKeyNumer) - LowestKeyNumer;

            for (int nr = min; nr <= max; nr++)
            {
                mKeys[nr].TurnKeyOff();
            }
        }
    }

    public class PianoKeyEvent : EventArgs
    {
        public int KeyNumber { get; private set; }
        public PianoKeyEvent(int keyNumber)
        {
            KeyNumber = keyNumber;
        }
    }
}
