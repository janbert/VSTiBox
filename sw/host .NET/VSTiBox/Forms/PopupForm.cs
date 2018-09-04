using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VSTiBox
{
    public partial class PopupForm : Form
    {
        public PopupForm(string text, int time)
        {
            InitializeComponent();
            label1.Text = text;
            if (time > 0)
            {
                timer1.Interval = time;
                timer1.Enabled = true; 
            }
            this.TopMost = true; 
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Close();
        }

        public new void Close()
        {
            FadeOut(this, 20);
        }

        private async void FadeOut(Form o, int interval = 80)
        {
            //Object is fully visible. Fade it out
            while (o.Opacity > 0.0)
            {
                await Task.Delay(interval);
                o.Opacity -= 0.05;
            }
            o.Opacity = 0; //make fully invisible       
            base.Close();
        }
    }
}
