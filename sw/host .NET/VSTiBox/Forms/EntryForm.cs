using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace VSTiBox
{
    public partial class EntryForm : Form
    {
        public EntryForm(string header, string startValue)
        {
            InitializeComponent();
            this.DialogResult = DialogResult.Cancel;
            this.Text = header;
            tbName.Text = startValue;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Name = tbName.Text; 
            this.Close();
        }

        private void tbName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
	        {
                this.DialogResult = DialogResult.OK; 
                this.Name = tbName.Text;
                this.Close();
	        }
        }

        private void keyboardControl1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back)
            {
                if (tbName.Text.Length > 0 && tbName.SelectionStart > 0)
                {
                    tbName.SelectionStart--;
                    tbName.SelectionLength = 1;
                    tbName.SelectedText = "";
                }
            }
            if (e.KeyCode == Keys.Enter)
            {
                this.DialogResult = DialogResult.OK;
                this.Name = tbName.Text;
                this.Close();
            }
        }

        private void keyboardControl1_KeyPress(object sender, KeyPressEventArgs e)
        {
            tbName.AppendText(e.KeyChar.ToString());
        }        
    }
}
