namespace VSTiBox
{
    partial class ChannelControl
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
            this.pbOnOff = new System.Windows.Forms.PictureBox();
            this.pbKeyZone = new System.Windows.Forms.PictureBox();
            this.pbFxInsert = new System.Windows.Forms.PictureBox();
            this.lblName = new System.Windows.Forms.Label();
            this.sliderPan = new VSTiBox.RingSlider();
            this.cbInstrument = new VSTiBox.MetroComboBox();
            this.sliderVolume = new VSTiBox.RingSlider();
            this.cbMidiChannel = new VSTiBox.MetroComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbOnOff)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbKeyZone)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbFxInsert)).BeginInit();
            this.SuspendLayout();
            // 
            // pbOnOff
            // 
            this.pbOnOff.Image = global::VSTiBox.Properties.Resources.inactive;
            this.pbOnOff.Location = new System.Drawing.Point(4, 39);
            this.pbOnOff.Name = "pbOnOff";
            this.pbOnOff.Size = new System.Drawing.Size(32, 32);
            this.pbOnOff.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbOnOff.TabIndex = 9;
            this.pbOnOff.TabStop = false;
            // 
            // pbKeyZone
            // 
            this.pbKeyZone.Image = global::VSTiBox.Properties.Resources.key_zone_on;
            this.pbKeyZone.Location = new System.Drawing.Point(68, 39);
            this.pbKeyZone.Name = "pbKeyZone";
            this.pbKeyZone.Size = new System.Drawing.Size(32, 32);
            this.pbKeyZone.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbKeyZone.TabIndex = 8;
            this.pbKeyZone.TabStop = false;
            this.pbKeyZone.Click += new System.EventHandler(this.pbKeyZone_Click);
            // 
            // pbFxInsert
            // 
            this.pbFxInsert.Image = global::VSTiBox.Properties.Resources.insert_on;
            this.pbFxInsert.Location = new System.Drawing.Point(36, 39);
            this.pbFxInsert.Name = "pbFxInsert";
            this.pbFxInsert.Size = new System.Drawing.Size(32, 32);
            this.pbFxInsert.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbFxInsert.TabIndex = 7;
            this.pbFxInsert.TabStop = false;
            this.pbFxInsert.Click += new System.EventHandler(this.pbFxInsert_Click);           
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblName.ForeColor = System.Drawing.Color.White;
            this.lblName.Location = new System.Drawing.Point(7, 78);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(56, 16);
            this.lblName.TabIndex = 10;
            this.lblName.Text = "<name>";
            // 
            // sliderPan
            // 
            this.sliderPan.DBColor = System.Drawing.Color.Orange;
            this.sliderPan.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sliderPan.IntValue = 0;
            this.sliderPan.Location = new System.Drawing.Point(11, 104);
            this.sliderPan.Max = 127;
            this.sliderPan.Min = -127;
            this.sliderPan.Name = "sliderPan";
            this.sliderPan.RingBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(49)))), ((int)(((byte)(49)))));
            this.sliderPan.RingWidth = 8;
            this.sliderPan.Size = new System.Drawing.Size(136, 54);
            this.sliderPan.StartAngle = 20;
            this.sliderPan.StopAngle = 160;
            this.sliderPan.TabIndex = 6;
            this.sliderPan.TextColor = System.Drawing.Color.White;
            this.sliderPan.Type = VSTiBox.RingType.Pan;
            this.sliderPan.Visible = false;
            this.sliderPan.ValueChanged += new System.EventHandler(this.sliderPan_ValueChanged);
            // 
            // cbInstrument
            // 
            this.cbInstrument.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbInstrument.Items = null;
            this.cbInstrument.Location = new System.Drawing.Point(5, 5);
            this.cbInstrument.Name = "cbInstrument";
            this.cbInstrument.SelectedIndex = 0;
            this.cbInstrument.Size = new System.Drawing.Size(140, 27);
            this.cbInstrument.TabIndex = 4;
            this.cbInstrument.TextColor = System.Drawing.Color.White;
            this.cbInstrument.ValueChanged += new System.EventHandler(this.cbInstrument_ValueChanged);
            // 
            // sliderVolume
            // 
            this.sliderVolume.DBColor = System.Drawing.Color.Silver;
            this.sliderVolume.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sliderVolume.IntValue = 0;
            this.sliderVolume.Location = new System.Drawing.Point(7, 104);
            this.sliderVolume.Margin = new System.Windows.Forms.Padding(0);
            this.sliderVolume.Max = 127;
            this.sliderVolume.Min = 0;
            this.sliderVolume.Name = "sliderVolume";
            this.sliderVolume.RingBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(49)))), ((int)(((byte)(49)))));
            this.sliderVolume.RingWidth = 8;
            this.sliderVolume.Size = new System.Drawing.Size(136, 54);
            this.sliderVolume.StartAngle = 20;
            this.sliderVolume.StopAngle = 160;
            this.sliderVolume.TabIndex = 3;
            this.sliderVolume.TextColor = System.Drawing.Color.White;
            this.sliderVolume.Type = VSTiBox.RingType.Volume;
            this.sliderVolume.ValueChanged += new System.EventHandler(this.sliderVolume_ValueChanged);
            // 
            // cbMidiChannel
            // 
            this.cbMidiChannel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbMidiChannel.Items = new string[] {
        "All",
        "1",
        "2",
        "3",
        "4",
        "5",
        "6",
        "7",
        "8",
        "9",
        "10",
        "11",
        "12",
        "13",
        "14",
        "15",
        "16"};
            this.cbMidiChannel.Location = new System.Drawing.Point(103, 46);
            this.cbMidiChannel.Name = "cbMidiChannel";
            this.cbMidiChannel.SelectedIndex = 0;
            this.cbMidiChannel.Size = new System.Drawing.Size(41, 18);
            this.cbMidiChannel.TabIndex = 11;
            this.cbMidiChannel.TextColor = System.Drawing.Color.White;
            this.cbMidiChannel.ValueChanged += new System.EventHandler(this.cbMidiChannel_ValueChanged);
            // 
            // ChannelControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(23)))), ((int)(((byte)(23)))));
            this.Controls.Add(this.cbMidiChannel);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.pbOnOff);
            this.Controls.Add(this.pbKeyZone);
            this.Controls.Add(this.pbFxInsert);
            this.Controls.Add(this.sliderPan);
            this.Controls.Add(this.cbInstrument);
            this.Controls.Add(this.sliderVolume);
            this.Name = "ChannelControl";
            this.Size = new System.Drawing.Size(150, 160);
            ((System.ComponentModel.ISupportInitialize)(this.pbOnOff)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbKeyZone)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbFxInsert)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private RingSlider sliderVolume;
        private MetroComboBox cbInstrument;
        private RingSlider sliderPan;
        private System.Windows.Forms.PictureBox pbFxInsert;
        private System.Windows.Forms.PictureBox pbKeyZone;
        private System.Windows.Forms.PictureBox pbOnOff;
        private System.Windows.Forms.Label lblName;
        private MetroComboBox cbMidiChannel;
    }
}
