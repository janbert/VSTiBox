using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualBasic;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Drawing;
using VSTiBox.Common;

namespace VSTiBox
{
    public enum RingType
    {
        Volume,
        Pan,
    }

    class RingSlider : UserControl
    {
        private BufferedGraphics NO_MANAGED_BACK_BUFFER = null;
        private BufferedGraphicsContext mGraphicManager;
        private BufferedGraphics mManagedBackBuffer;
       
        private SolidBrush mBackBrush;
        private Font mTextFont = new Font("Arial", 7);
        private int mValue;
        private Boolean mMouseCapture = false;

        public event EventHandler ValueChanged;

        private RingType mRingType = RingType.Volume ;

        public RingType Type
        {
            get
            {
                return mRingType;
            }
            set
            {
                mRingType = value;
            }
        }

        private int mRingWidth = 8;
        public int RingWidth
        {
            get
            {
                return mRingWidth;
            }
            set
            {
                mRingWidth = value;
                this.Invalidate();
            }
        }

        private int mStartAngle = 20;
        public int StartAngle 
        {
            get 
            {
                return mStartAngle; 
            }
            set
            {
                mStartAngle = value;
                this.Invalidate();
            }
        }

        private int mStopAngle = 160;
        public int StopAngle
        {
            get
            {
                return mStopAngle;
            }
            set
            {
                mStopAngle = value;
                if (mValue < min) mValue = min;
                this.Invalidate();
            }
        }

        private int max;
        public int Max
        {
            get
            {
                return max;
            }
            set
            {
                max = value;
                if (mValue > max) mValue = max;                
                this.Invalidate();
            }
        }

        private int min;
        public int Min
        {
            get
            {
                return min;
            }
            set
            {
                min = value;
                this.Invalidate();
            }
        }
      
        public int IntValue
        {
            get
            {
                return mValue;
            }
            set
            {
                int newValue = value;
                if (newValue < min) newValue = min;
                if (newValue > max) newValue = max;

                if (mValue != newValue)
                {
                    mValue = newValue;
                }
                this.Invalidate();
            }
        }

        public float VolumeValue
        {
            get
            {
                return VolumeLevelConverter.GetVolumeNormalized (mValue);
            }
        }

        private Color mBackColor = Color.FromArgb(23, 23, 23);
        public override Color BackColor
        {
            get
            {
                return mBackColor;
            }
            set
            {
                mBackColor = value;
                mBackBrush = new SolidBrush(value);
                this.Invalidate();
            }
        }

        private Color mRingForeColor = Color.FromArgb(1, 123, 205);
        private Pen mRingForePen;
        public override System.Drawing.Color ForeColor
        {
            get
            {
                return mRingForeColor;
            }
            set
            {
                mRingForeColor = value;
                mRingForePen = new Pen(value, mRingWidth );
                this.Invalidate();
            }
        }

        private Color mRingDBColor = Color.Orange;
        private Pen mRingDBPen;
        public System.Drawing.Color DBColor
        {
            get
            {
                return mRingDBColor;
            }
            set
            {
                mRingDBColor = value;
                mRingDBPen = new Pen(value, mRingWidth);
                this.Invalidate();
            }
        }   

        private Color mRingColor = Color.FromArgb(49,49,49);
        private Pen mRingBackPen;
        public Color RingBackColor
        {
            get
            {
                return mRingColor;
            }
            set
            {
                mRingColor = value;
                mRingBackPen = new Pen( mRingColor, mRingWidth);
                this.Invalidate();
            }
        }


        private Color mTextColor = Color.White;
        private Brush mTextBrush;
        public Color TextColor
        {
            get
            {
                return mTextColor;
            }
            set
            {
                mTextColor = value;
                mTextBrush = new SolidBrush(value);
                this.Invalidate();
            }
        }

        public RingSlider()
        {
            this.Resize += DoubleBufferControl_Resize;
            InitializeComponent();
            mRingForePen = new Pen(this.ForeColor, mRingWidth);
            mRingBackPen = new Pen(this.RingBackColor, mRingWidth); 
            mBackBrush = new SolidBrush(this.BackColor);
            mTextBrush = new SolidBrush(this.TextColor);


            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.DoubleBuffered = false;
            Application.ApplicationExit += MemoryCleanup;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            mGraphicManager = BufferedGraphicsManager.Current;
            mGraphicManager.MaximumBuffer = new Size(this.Width, this.Height);
            mManagedBackBuffer = mGraphicManager.Allocate(this.CreateGraphics(), ClientRectangle);            
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // DoubleBufferControl
            // 
            this.ResumeLayout(false);
        }

        private void MemoryCleanup(object sender, EventArgs e)
        {
            if (!object.ReferenceEquals(mManagedBackBuffer, NO_MANAGED_BACK_BUFFER))
            {
                mManagedBackBuffer.Dispose();
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Do nothing	
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                // Draw the graphics
                drawGraphics(mManagedBackBuffer.Graphics);
                // paint the picture in from the back buffer into the form draw area
                mManagedBackBuffer.Render(e.Graphics);
            }
            catch
            {
                // Do nothing
            }
        }
        

        Font mInfinityFont = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

        private void drawGraphics(Graphics g)
        {
            int ringWidthDiv2 = mRingWidth / 2;                    
        
            // Paint background
            g.SmoothingMode = SmoothingMode.Default;
            g.FillRectangle(mBackBrush, 0, 0, this.Width, this.Height);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            if (Type == RingType.Volume)
            {
                float dB = VolumeLevelConverter.GetVolumeDecibel(mValue);
                int angleVolume0 = 107;
                int zeroDBValue = Math.Min(mValue, angleVolume0);

                // calc active range
                int totalAngle = mStopAngle - mStartAngle;
                int activeAngle1 = (int)((((float)zeroDBValue - min) / (max - min)) * (float)totalAngle);
                int inactiveAngle = totalAngle - activeAngle1;
                int activeAngle2 = 0;

                // Draw white part
                g.DrawArc(mRingForePen, ringWidthDiv2, ringWidthDiv2, this.Width - mRingWidth - 1, this.Width - mRingWidth - 1, 180 + mStartAngle, activeAngle1);
                if (mValue > zeroDBValue)
                {
                    int startAngle2 = 180 + mStartAngle + activeAngle1;
                    activeAngle2 = (int)((((float)mValue - min) / (max - min)) * (float)totalAngle) - activeAngle1;
                    inactiveAngle -= activeAngle2;

                    //Draw red part
                    g.DrawArc(mRingDBPen, ringWidthDiv2, ringWidthDiv2, this.Width - mRingWidth - 1, this.Width - mRingWidth - 1, startAngle2, activeAngle2);
                }

                // Draw greyed out part
                g.DrawArc(mRingBackPen, ringWidthDiv2, ringWidthDiv2, this.Width - mRingWidth - 1, this.Width - mRingWidth - 1, 180 + mStartAngle + activeAngle1 + activeAngle2, inactiveAngle);

                string display;

                if (mValue == min)
                {
                    // Draw - infinity string
                    //g.DrawString("-", this.Font, mTextBrush, (this.Width / 2) - 20, (this.Height / 2) - 4);
                   //g.DrawString("\u221E", this.Font/* mInfinityFont */, mTextBrush, (this.Width / 2) - 10, (this.Height / 2) - 2);
                    display = "-\u221E dB"; 
                }
                else
                {
                    display = string.Format("{0}{1:0.0} dB", dB > 0.0f ? "+" : "", dB);
                    // Draw value string
                    //g.DrawString(display, this.Font, mTextBrush, (this.Width / 2) - 40, (this.Height / 2) - 4);
                }
                //g.DrawString("dB", this.Font, mTextBrush, (this.Width / 2) + 9, (this.Height / 2) - 4);

                //display = mValue.ToString();
                // Draw value string
                float stringWidth = g.MeasureString(display, this.Font).Width;
                g.DrawString(display, this.Font, mTextBrush, (this.Width / 2) - stringWidth / 2.0f, (this.Width / 2) - 50);
            }
            else
            {
                // calc active range
                int totalAngle = mStopAngle - mStartAngle;
                int activeAngle = (int)((((float)mValue - min) / (max - min)) * (float)totalAngle);
                int inactiveAngle = totalAngle - activeAngle;
                string display;

                // Pan: draw first greyed out part
                g.DrawArc(mRingBackPen, ringWidthDiv2, ringWidthDiv2, this.Width - mRingWidth - 1, this.Width - mRingWidth - 1, 180 + mStartAngle, activeAngle - 3);
                // Draw active part
                g.DrawArc(mRingForePen, ringWidthDiv2, ringWidthDiv2, this.Width - mRingWidth - 1, this.Width - mRingWidth - 1, 180 + mStartAngle + activeAngle - 3, 6);
                // Draw greyed out part
                g.DrawArc(mRingBackPen, ringWidthDiv2, ringWidthDiv2, this.Width - mRingWidth - 1, this.Width - mRingWidth - 1, 180 + mStartAngle + activeAngle + 3, inactiveAngle - 3);

                display = mValue.ToString();
                // Draw value string
                float stringWidth = g.MeasureString(display, this.Font).Width;
                g.DrawString(display, this.Font, mTextBrush, (this.Width / 2) - stringWidth / 2.0f, (this.Width / 2) - 50);
            }                      
        }

        private int angleToValue(int angle)
        {
            angle -= 180; 
            
            if(angle < mStartAngle )
            {
                angle = mStartAngle ;
            }
            else if ( angle > mStopAngle)
            {
                angle = mStopAngle;
            }
            
            angle -= mStartAngle;
            return ((angle * (max-min)) / (mStopAngle - mStartAngle)) + min; 
        }

        private float getAngleFromMousePointer(int x, int y)
        {
            float angle = 0.0f;
            
            int centerX = this.Width / 2;
            int centerY = this.Width / 2; 

            // Calculate relate x,y to center
            x = x - centerX;
            y = y - centerX;

            if(x == 0 && y == 0)
            {
                angle = 0.0f;
            }
            else if (x == 0 && y > 0)
            {
                angle = 90.0f;
            }
            else if (x == 0 && y < 0)
            {
                angle = 270.0f;
            }
            else
            {
                angle = Rad2Deg((float)Math.Atan((double)y / (double)x));
                if ( x < 0)angle += 180;
                if (angle < 0 ) angle += 360;
            }
            return angle;
        }

        float Rad2Deg(float value)
        {
            return value * 180 / (float)Math.PI;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            float angle = getAngleFromMousePointer(e.X, e.Y);
            IntValue = angleToValue((int)angle);
            mMouseCapture = true;
            Invalidate();
        }
                
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (mMouseCapture)
            {
                float angle = getAngleFromMousePointer(e.X, e.Y);
                IntValue = angleToValue((int)angle);
                if (ValueChanged != null)
                {
                    ValueChanged(null, null);
                }
                Invalidate();
                mMouseCapture = false;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (mMouseCapture)
            {
                float angle = getAngleFromMousePointer(e.X, e.Y);
                IntValue = angleToValue((int)angle);
                Invalidate();
            }
        }

        private void DoubleBufferControl_Resize(object sender, EventArgs e)
        {
            mGraphicManager.MaximumBuffer = new Size(this.Width, this.Height);
            if (!object.ReferenceEquals(mManagedBackBuffer, NO_MANAGED_BACK_BUFFER))
            {
                mManagedBackBuffer.Dispose();
            }
            mManagedBackBuffer = mGraphicManager.Allocate(this.CreateGraphics(), ClientRectangle);
            this.Refresh();
        }
    }
}
