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
    public partial class PianoOctave : UserControl
    {
        private List<PianoKey> mKeys = new List<PianoKey>();
        
        public PianoOctave()
        {
            InitializeComponent();
        
            mKeys.Add(keyC );
            mKeys.Add(keyCis );
            mKeys.Add(keyD);
            mKeys.Add(keyDis );
            mKeys.Add(keyE);
            mKeys.Add(keyF);
            mKeys.Add(keyFis);
            mKeys.Add(keyG);
            mKeys.Add(keyGis);
            mKeys.Add(keyA);
            mKeys.Add(keyAis);
            mKeys.Add(keyB);
        }

        public PianoKey[] Keys
        {
            get
            {
                return mKeys.ToArray();
            }
        }
    }
}
