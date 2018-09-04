using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VSTiBox
{
    public partial class Keyboard : UserControl
    {
        public new event KeyPressEventHandler KeyPress;
        public new event KeyEventHandler KeyDown;
        private List<KeyboardKey> mKeysWithCasing = new List<KeyboardKey>();

        public Keyboard()
        {
            InitializeComponent();

            mKeysWithCasing.Add(keyA);
            mKeysWithCasing.Add(keyB);
            mKeysWithCasing.Add(keyC);
            mKeysWithCasing.Add(keyD);
            mKeysWithCasing.Add(keyE);
            mKeysWithCasing.Add(keyF);
            mKeysWithCasing.Add(keyG);
            mKeysWithCasing.Add(keyH);
            mKeysWithCasing.Add(keyI);
            mKeysWithCasing.Add(keyJ);
            mKeysWithCasing.Add(keyK);
            mKeysWithCasing.Add(keyL);
            mKeysWithCasing.Add(keyM);
            mKeysWithCasing.Add(keyN);
            mKeysWithCasing.Add(keyO);
            mKeysWithCasing.Add(keyP);
            mKeysWithCasing.Add(keyQ);
            mKeysWithCasing.Add(keyR);
            mKeysWithCasing.Add(keyS);
            mKeysWithCasing.Add(keyT);
            mKeysWithCasing.Add(keyU);
            mKeysWithCasing.Add(keyV);
            mKeysWithCasing.Add(keyW);
            mKeysWithCasing.Add(keyX);
            mKeysWithCasing.Add(keyY);
            mKeysWithCasing.Add(keyZ);
        }

        private void key_Click(object sender, EventArgs e)
        {
            KeyboardKey key = (KeyboardKey)sender;

            if (KeyPress != null)
            {
                KeyPress(this, new KeyPressEventArgs(key.Text[0]));
            }

            if (keyShift.Pressed)
            {
                keyShift.Pressed = false;
                keyShift_Click(null, null);
            }
        }

        private void keyBackspace_Click(object sender, EventArgs e)
        {
            if (KeyDown != null)
            {
                KeyDown(this, new KeyEventArgs(Keys.Back));
            }
        }

        private void keyShift_Click(object sender, EventArgs e)
        {
            foreach (KeyboardKey key in mKeysWithCasing)
            {
                key.Text = keyShift.Pressed ? key.Text.ToUpper() : key.Text.ToLower();
            }
        }

        private void key_EnterClick(object sender, EventArgs e)
        {
            if (KeyDown != null)
            {
                KeyDown(this, new KeyEventArgs(Keys.Enter));
            }
        }
    }
}
