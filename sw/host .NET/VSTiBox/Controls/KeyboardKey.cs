using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VSTiBox
{
    class KeyboardKey : Control
    {
        public bool Momentary { get; set; }
        private bool mPressed = false;

        public bool Pressed
        {
            get
            {
                return mPressed;
            }
            set
            {
                mPressed = value;
                Invalidate();
            }
        }

        public KeyboardKey()
        {
            this.SetStyle(ControlStyles.StandardDoubleClick, false);
        }

        private Color mPressedColor = Color.DarkBlue;
        public Color PressedColor
        {
            get
            {
                return mPressedColor;
            }

            set
            {
                mPressedColor = value;
            }
        }

        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
                this.Invalidate();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (Momentary)
            {
                Pressed = true;
            }
            else
            {
                Pressed = !Pressed;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (Momentary)
            {
                Pressed = false;
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (Momentary)
            {
                Pressed = false;
            }
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            // Do nothing
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Brush brush;
            if (Pressed)
            {
                brush = new SolidBrush(PressedColor);
            }
            else
            {
                brush = new SolidBrush(BackColor);
            }
            e.Graphics.FillRectangle(brush, 0, 0, Width, Height);

            if (this.Text == "Backspace")
            {
                e.Graphics.DrawImage(Properties.Resources.backspace, 28, 15);
            }
            else
            {
                var dim = e.Graphics.MeasureString(this.Text, this.Font);
                PointF p = new PointF(this.Width / 2 - dim.Width / 2, this.Height / 2 - dim.Height / 2);
                e.Graphics.DrawString(this.Text, this.Font, new SolidBrush(this.ForeColor), p, StringFormat.GenericDefault);
            }
        }
    }
}
