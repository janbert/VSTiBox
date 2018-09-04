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

namespace VSTiBox
{
    class ScrollList : UserControl
    {
        public const int ITEMHEIGTH = 30;

        private BufferedGraphics NO_MANAGED_BACK_BUFFER = null;
        private BufferedGraphicsContext mGraphicManager;
        private BufferedGraphics mManagedBackBuffer;
        private Brush mScrollBrush = new SolidBrush(Color.FromArgb(132, 132, 132));    
        private int mViewPos = 0;
        private int mSelectedIndex;
        private string[] mItems = new string[] { };
        private int mItemsInRange;
        private Color mBackColor = Color.FromArgb(23, 23, 23);
        private SolidBrush mBackBrush;
        private Font mSecondLineFont = new Font("Arial", 10);

        public event EventHandler ValueChanged;
       
        public int SelectedIndex 
        {
            get 
            {
                return mSelectedIndex; 
            }
            set 
            {
                int newIndex = value;

                if (newIndex > mSelectedIndex)
                {
                    // Check if view position has to folow selected index
                    if (newIndex - mViewPos > (mItemsInRange-1))
                    {
                        mViewPos = newIndex - (mItemsInRange-1);
                    }
                }
                else if (newIndex < mSelectedIndex)
                {
                    // Check if view position has to folow selected index
                    if (mViewPos > newIndex )
                    {
                        mViewPos = newIndex; 
                    }
                }
                mSelectedIndex = value;
                Invalidate();
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

        private Color mForeColor = Color.FromArgb(1, 123, 205);
        private SolidBrush mForeBrush;
        public override System.Drawing.Color ForeColor
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

        public ScrollList()
        {
            this.Resize += onResize;
            InitializeComponent();
            mForeBrush = new SolidBrush(this.ForeColor);
            mBackBrush = new SolidBrush(this.BackColor);
            mTextBrush = new SolidBrush(this.TextColor);

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.DoubleBuffered = false;
            Application.ApplicationExit += MemoryCleanup;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            mGraphicManager = BufferedGraphicsManager.Current;
            mGraphicManager.MaximumBuffer = new Size(this.Width + 1, this.Height + 1);
            mManagedBackBuffer = mGraphicManager.Allocate(this.CreateGraphics(), ClientRectangle);
            mManagedBackBuffer.Graphics.SmoothingMode = SmoothingMode.HighQuality;

            MouseWheelHandler.Add(this, onMouseWheel);
        }
      
        public string[] Items
        {
            get
            {
                return mItems;
            }
            set
            {
                if (value != null )
                {
                    mItems = value;
                    mSelectedIndex = 0;
                    mViewPos = 0;
                    mItemsInRange = Math.Min(this.Height / ITEMHEIGTH, mItems.Count());
                    this.Invalidate();
                }
            }
        }

        public void ClearItems()
        {
            mItems = null;
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
            // Draw background
            g.FillRectangle(mBackBrush, 0, 0, this.Width, this.Height);

            // Draw scrollbar line            
            g.FillRectangle(mScrollBrush, this.Width - 1, 0, 1, this.Height);

            if (mItems != null && mItems.Length > 0)
            {
                // Calc size of total items
                int itemsBeforeRange = mViewPos;
                int itemsAfterRange = Math.Max(0, mItems.Length - mItemsInRange - itemsBeforeRange);

                // Draw scrollbar
                int barSize = (int)(0.5f + (float)this.Height * (float)mItemsInRange / (float)mItems.Length);
                int barTop = (int)(0.5f + (float)this.Height * (float)mViewPos / (float)mItems.Length);

                g.FillRectangle(mScrollBrush, this.Width - 6, barTop, 5, barSize);

                // Draw selection rect
                int selectTop = (mSelectedIndex - mViewPos) * ITEMHEIGTH;
                g.FillRectangle(mForeBrush, 0, selectTop, this.Width - 7, ITEMHEIGTH);

                // Draw items text
                int textTop = ITEMHEIGTH / 2 - (int)Font.Size;
               
                for (int i = mViewPos; i < mViewPos + mItemsInRange; i++)
                {
                    if (mItems[i].Contains(Environment.NewLine))
                    {
                        // Split into two lines
                        string[] arr = mItems[i].Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                        g.DrawString(arr[0], this.Font, mTextBrush, 8.0f, (float)textTop-4);
                        g.DrawString(arr[1], mSecondLineFont, Brushes.LightGray, 8.0f, (float)textTop+4);
                    }
                    else
                    {
                        g.DrawString(mItems[i], this.Font, mTextBrush, 8.0f, (float)textTop);
                    }
                    textTop += ITEMHEIGTH;
                }
            }
        }

        void onMouseWheel(MouseEventArgs e)
        {
            if (e.Delta < 0)
            {
                if (mSelectedIndex < mItems.Length-1) SelectedIndex++;
                Invalidate();
            }
            else if (e.Delta > 0)
            {
                if (mSelectedIndex > 0) SelectedIndex--;
                Invalidate();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            int pos = e.Y / ITEMHEIGTH;
            pos += mViewPos;
            mSelectedIndex = pos; 
            Invalidate();
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            Invalidate();
            if (ValueChanged != null)
            {
                ValueChanged(null, null);
            }
        }

        private void onResize(object sender, EventArgs e)
        {
            mGraphicManager.MaximumBuffer = new Size(this.Width + 1, this.Height + 1);
            if (!object.ReferenceEquals(mManagedBackBuffer, NO_MANAGED_BACK_BUFFER))
            {
                mManagedBackBuffer.Dispose();
            }
            mManagedBackBuffer = mGraphicManager.Allocate(this.CreateGraphics(), ClientRectangle);
            mItemsInRange = Math.Min(this.Height / ITEMHEIGTH, mItems.Count ());
            this.Refresh();
        }
    }

    
}
