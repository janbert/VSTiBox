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

namespace VSTiBox
{
    class MetroComboBox : UserControl
    {
        const int ITEMHEIGTH = 30;

        private BufferedGraphics NO_MANAGED_BACK_BUFFER = null;
        private BufferedGraphicsContext mGraphicManager;
        private BufferedGraphics mManagedBackBuffer;
        private Brush mBorderBrush = new SolidBrush(Color.FromArgb(132, 132, 132));
        private Pen mBorderPen = new Pen(Color.FromArgb(132, 132, 132));
        
        private int mSelectedIndex;
        private string[] mItems;
        
        private Color mBackColor = Color.FromArgb(23, 23, 23);
        private SolidBrush mBackBrush;

        public event EventHandler ValueChanged;

        private PopupScrollList mPopup;

        public Boolean IsExpanded { get { return mPopup != null; } }

        public void Expand()
        {
            if (mPopup == null)
            {
                mPopup = new PopupScrollList(mItems);
                mPopup.TopMost = true;

                Point absolute_location = this.Parent.PointToScreen(this.Location);
                mPopup.StartPosition = FormStartPosition.Manual;
                mPopup.ShowInTaskbar = false;
                mPopup.Top = absolute_location.Y - mPopup.Height + 1;
                mPopup.Left = absolute_location.X;
                mPopup.Width = this.Width;
                mPopup.Show();
                mPopup.ValueChanged += mPopup_ValueChanged;                
            }
        }

        void foof()
        {
            mPopup.Close();
            mPopup = null;
            this.Invalidate();
        }

        void mPopup_ValueChanged(object sender, EventArgs e)
        {
            mSelectedIndex = mPopup.SelectedIndex;
            
            if(ValueChanged != null) 
            {
                ValueChanged(this, e);
            }
           mPopup.Close();
           mPopup = null;
           this.Invalidate();
        }

        public void Collapse()
        {
            mPopup.Close();
        }
        
        public int SelectedIndex
        {
            get
            {
                return mSelectedIndex;
            }
            set
            {
                mSelectedIndex = value; 
                if (mPopup != null)
                {
                    mPopup.SelectedIndex = value;   
                }
                this.Invalidate();
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

        public MetroComboBox()
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
        }

        public string[] Items
        {
            get
            {
                return mItems;
            }
            set
            {
                if (value != null && value.Length > 0)
                {
                    mItems = value;
                    mSelectedIndex = 0;
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

            if (mPopup != null)
            {
                // todo: Property
                Brush blueBrush = new SolidBrush(Color.FromArgb(1, 123, 205));
                g.FillRectangle(blueBrush, this.Width - 24, 0, 24, this.Height);
            }

            // Draw border
            g.DrawRectangle (mBorderPen, 0,0, this.Width-1, this.Height-1);

            // Draw triangle
            const int HALF_TRIANGLE_SIZE = 4;
            Point point1 = new Point(this.Width - 5 - 2 *HALF_TRIANGLE_SIZE , (this.Height / 2) - HALF_TRIANGLE_SIZE);
            Point point2 = new Point(this.Width - 5, (this.Height / 2) - HALF_TRIANGLE_SIZE);
            Point point3 = new Point(this.Width - 5 - HALF_TRIANGLE_SIZE, (this.Height / 2) + HALF_TRIANGLE_SIZE);
            Point[] trianglePoints = { point1, point2, point3 }; 
            g.FillPolygon(mBorderBrush, trianglePoints);

            if (mItems != null && mSelectedIndex != -1)
            {
                g.DrawString(mItems[mSelectedIndex], this.Font, mTextBrush, 8.0f, (this.Height / 2) - (int)Font.Size);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (mPopup == null && mItems != null)
            {
                Expand();
            }
            Invalidate();
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
