namespace VSTiBox
{
    partial class EffectPluginSelectionControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.flpFxInserts = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // flpFxInserts
            // 
            this.flpFxInserts.Location = new System.Drawing.Point(23, 24);
            this.flpFxInserts.Name = "flpFxInserts";
            this.flpFxInserts.Size = new System.Drawing.Size(456, 172);
            this.flpFxInserts.TabIndex = 1;
            // 
            // EffectSelectionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(23)))), ((int)(((byte)(23)))));
            this.Controls.Add(this.flpFxInserts);
            this.Name = "EffectSelectionControl";
            this.Size = new System.Drawing.Size(1054, 554);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flpFxInserts;


    }
}
