using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;
using System.Globalization;


namespace VSTiBox
{
    public class NumericTextBox : TextBox
    {

        private bool ignoreTextChangedEventOnce = false;

        public bool AllowGroupSeparator {get;set;}
       
        public bool AllowDecimal { get; set; }

        private bool mAllowNegative;
        public bool AllowNegative
        {
            get { return mAllowNegative; }
            set
            {
                if (!value)
                    Min = 0;
                mAllowNegative = value;
            }
        }

        private bool mAllowHex;
        public bool AllowHex
        {
            get { return mAllowHex; }
            set
            {
                mAllowHex = value;
                if (!value)
                {
                    mAllowNegative = false;
                    AllowDecimal = false;
                    AllowGroupSeparator = false; 
                }
            }
        }

        public double Max{ get; set; }

        private double minValue;
        public double Min
        {
            get 
            {
                return minValue; 
            }
            set
            {
                if (value < 0)
                    AllowNegative = true;
                minValue = value;
            }
        }

        public void SetTextWithoutEvent(string value)
        {
            if (base.Text == value)
                return;
            ignoreTextChangedEventOnce = true;
            base.Text = value;
        }

        protected override void OnTextChanged(System.EventArgs e)
        {
            if (ignoreTextChangedEventOnce)
            {
                ignoreTextChangedEventOnce = false;
                return;
            }

            if (base.Text == string.Empty)
            {
                if (AllowHex)
                {
                    base.Text = ((long)Min).ToString("X");
                }
                else
                {
                    base.Text = Min.ToString();
                }
            }
            
            double value = DoubleValue;

            if (!string.IsNullOrEmpty(base.Text) && value > Max)
            {
                if (AllowHex)
                {
                    base.Text = ((long)Max).ToString("X");
                }
                else if (AllowDecimal)
                {
                    base.Text = Max.ToString();
                }
                else
                {
                    base.Text = ((int)Max).ToString();
                }
                MessageBox.Show("Value cannot be greater than " + base.Text, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            if (!string.IsNullOrEmpty(base.Text) && value < Min)
            {
                if (AllowHex)
                {
                    base.Text = ((long)Min).ToString("X");
                }
                else if (AllowDecimal)
                {
                    base.Text = Min.ToString();
                }
                else
                {
                    base.Text = ((int)Min).ToString();
                }
                MessageBox.Show("Value cannot be smaller than " + base.Text, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            base.OnTextChanged(e);
        }

        // Restricts the entry of characters to digits (including hex),
        // the negative sign, the e decimal point, and editing keystrokes (backspace).
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            char decimalSeparator = Convert.ToChar(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            char groupSeparator = Convert.ToChar(CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator);
            char negativeSign = Convert.ToChar(CultureInfo.CurrentCulture.NumberFormat.NegativeSign);
            
            string keyInput = e.KeyChar.ToString();

            if (Char.IsDigit(e.KeyChar))
            {
                // Digits are OK
            }
            else if ((e.KeyChar >= 'a' && e.KeyChar <= 'f' || e.KeyChar >= 'A' && e.KeyChar <= 'F') && AllowHex)
            {
                // a..f && A..F are ok.
            }
            else if ((keyInput.Equals('.') | keyInput.Equals(',')) & AllowDecimal)
            {
                // Change key to decimal seprator
                e.KeyChar = decimalSeparator;
                // Decimal operator OK
                if (base.Text.Contains(decimalSeparator))
                {
                    // Already contains a decimal seperator
                    e.Handled = true;
                }
            }
            else if (keyInput.Equals(groupSeparator) & AllowGroupSeparator)
            {
                // Group separator is OK
            }
            else if (keyInput.Equals(negativeSign) & AllowNegative)
            {
                // Only allow negative sign if property is set
            }
            else if (e.KeyChar == (char)Keys.Back)
            {
                // Backspace key is OK
                //    else if ((ModifierKeys & (Keys.Control | Keys.Alt)) != 0)
                //    {
                //     // Let the edit control handle control and alt key combinations
                //    }
            }
            else
            {
                // Consume this invalid key and beep.
                e.Handled = true;
            }
        }

        public int IntValue
        {
            get 
            {
                if (AllowHex)
                {
                    return Int32.Parse(this.Text,NumberStyles.HexNumber);
                }
                else
                {
                    return Int32.Parse(this.Text);
                }
            }
        }

        public double DoubleValue
        {
            get 
            {
                if (AllowHex)
                {
                    return (double)long.Parse(this.Text, NumberStyles.HexNumber);
                }
                else
                {
                    return double.Parse(this.Text);
                }
            }
        }
    }
}
