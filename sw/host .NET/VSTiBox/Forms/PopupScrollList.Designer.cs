namespace VSTiBox
{
    partial class PopupScrollList
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.scrollList1 = new VSTiBox.ScrollList();
            this.SuspendLayout();
            // 
            // scrollList1
            // 
            this.scrollList1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scrollList1.Items = new string[0];
            this.scrollList1.Location = new System.Drawing.Point(1, 1);
            this.scrollList1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.scrollList1.Name = "scrollList1";
            this.scrollList1.SelectedIndex = 0;
            this.scrollList1.Size = new System.Drawing.Size(213, 222);
            this.scrollList1.TabIndex = 0;
            this.scrollList1.TextColor = System.Drawing.Color.White;
            // 
            // PopupScrollList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(132)))), ((int)(((byte)(132)))));
            this.ClientSize = new System.Drawing.Size(216, 224);
            this.Controls.Add(this.scrollList1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "PopupScrollList";
            this.Text = "PopupScrollList";
            this.ResumeLayout(false);

        }

        #endregion

        private ScrollList scrollList1;
    }
}