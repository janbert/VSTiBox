using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace VSTiBox
{
    public partial class PopupScrollList : Form
    {
        public event EventHandler ValueChanged; 
        
        public PopupScrollList(string[] items)
        {
            this.Height = ScrollList.ITEMHEIGTH * items.Length;
            InitializeComponent();
            scrollList1.ValueChanged += scrollList1_ValueChanged;
            scrollList1.Items = items;
        }

        void scrollList1_ValueChanged(object sender, EventArgs e)
        {
            if (ValueChanged != null)
            {
                ValueChanged(null, null);
            }
        }
        
        public int SelectedIndex
        {
            get 
            { 
                return scrollList1.SelectedIndex; 
            }
            set
            { 
                scrollList1.SelectedIndex = value;
            }
        }
    }
}
