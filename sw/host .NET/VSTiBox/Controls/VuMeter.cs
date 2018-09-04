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
using System.Linq;
using System.Collections.Concurrent;

namespace VSTiBox
{
    public class VuMeter : Control
    {
        private const int REFRESH_RATE_HZ = 30;
        private const int REFRESH_INTERVAL_MS = 1000 / REFRESH_RATE_HZ;
        private const int HOLD_TIME_MS = 500;        
        
        private float mMaxValue = 0.0f;
        private float mValue = 0.0f;
        private int mHoldTimerTicks = 0;
        private BufferedGraphics NO_MANAGED_BACK_BUFFER = null;
        private BufferedGraphicsContext mGraphicManager;
        private BufferedGraphics mManagedBackBuffer;
        private Color mBackColor = Color.FromArgb(23, 23, 23);
        private Color mForeColor = Color.Blue;        
        private SolidBrush mBackBrush;
        private SolidBrush mForeBrush;

        private Timer mInvokeTimer;
        ConcurrentStack<float> mValueStack = new ConcurrentStack<float>();
                
        public float Value 
        {         
            set
            {
                mValueStack.Push(value);                
            }
            get
            {
                return 0.0f;
            }
        }
      
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

        public override Color ForeColor
        {
            get
            {
                return mForeColor;
            }
            set
            {
                mForeColor = value;
                mForeBrush = new SolidBrush(value);
                this.Invalidate();
            }
        }
              
        public VuMeter()
        {
            this.Resize += onResize;
            InitializeComponent();

            mBackBrush = new SolidBrush(this.BackColor);
            mForeBrush = new SolidBrush(this.ForeColor);

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.DoubleBuffered = false;
            Application.ApplicationExit += MemoryCleanup;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            mGraphicManager = BufferedGraphicsManager.Current;
            mGraphicManager.MaximumBuffer = new Size(this.Width + 1, this.Height + 1);
            mManagedBackBuffer = mGraphicManager.Allocate(this.CreateGraphics(), ClientRectangle);
            mManagedBackBuffer.Graphics.SmoothingMode = SmoothingMode.HighQuality;

           mInvokeTimer = new Timer();
           mInvokeTimer.Interval = REFRESH_INTERVAL_MS;
           mInvokeTimer.Tick += mInvokeTimer_Tick;
           mInvokeTimer.Start();
        }

        protected override void DestroyHandle()
        {
            mInvokeTimer.Stop(); 
            base.DestroyHandle();            
        }

        void mInvokeTimer_Tick(object sender, EventArgs e)
        {
            int count = mValueStack.Count();
            if (count > 0)
            {
                float[] val = new float[count];
                mValueStack.TryPopRange(val);
                mValueStack.Clear();
                mValue = val.Max();
            }
            else
            {
                mValue = 0.0f;
            }

            if (mValue > mMaxValue)
            {
                // New max value
                mMaxValue = mValue;
                mHoldTimerTicks = HOLD_TIME_MS / REFRESH_INTERVAL_MS;
            }
            else
            {
                if (mHoldTimerTicks > 0)
                {
                    mHoldTimerTicks--;
                }
                else
                {
                    // Decrement max value
                    const float STEP = 0.03f;
                    if (mMaxValue > STEP)
                    {
                        mMaxValue -= STEP;
                    }
                    else
                    {
                        mMaxValue = 0.0f;
                    }
                }
            }

            this.Invalidate();
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

        protected override void OnPaintBackground(PaintEventArgs pevent)
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

        private void drawGraphics(Graphics g)
        {
            float h = this.Height;
            float w = this.Width;
            float y = (int)(mValue * h);
            // Draw background
            g.FillRectangle(mBackBrush, 0, 0, w, h-y);
            // Draw bar
            g.FillRectangle(mForeBrush, 0, h - y, w, y);
            // Draw max bar
            float ymax = mMaxValue * h;
            g.FillRectangle(mForeBrush,0, h - ymax + 4.0f, w, 4.0f);
        }

        private void onResize(object sender, EventArgs e)
        {
            mGraphicManager.MaximumBuffer = new Size(this.Width + 1, this.Height + 1);
            if (!object.ReferenceEquals(mManagedBackBuffer, NO_MANAGED_BACK_BUFFER))
            {
                mManagedBackBuffer.Dispose();
            }
            mManagedBackBuffer = mGraphicManager.Allocate(this.CreateGraphics(), ClientRectangle);            
            this.Refresh();
        }
    }
}
