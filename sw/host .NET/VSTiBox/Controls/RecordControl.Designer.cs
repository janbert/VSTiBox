namespace VSTiBox.Controls
{
    partial class RecordControl
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
            this.components = new System.ComponentModel.Container();
            this.pbStop = new System.Windows.Forms.PictureBox();
            this.pbPlay = new System.Windows.Forms.PictureBox();
            this.pbRecord = new System.Windows.Forms.PictureBox();
            this.lblRec = new System.Windows.Forms.Label();
            this.trackBarPosition = new System.Windows.Forms.TrackBar();
            this.timerPlay = new System.Windows.Forms.Timer(this.components);
            this.cbLoop = new System.Windows.Forms.CheckBox();
            this.lblTime = new System.Windows.Forms.Label();
            this.scrollListRecordings = new VSTiBox.ScrollList();
            this.volumeMeter2 = new NAudio.Gui.VolumeMeter();
            this.volumeMeter1 = new NAudio.Gui.VolumeMeter();
            this.knobVolume = new VSTiBox.Knob();
            ((System.ComponentModel.ISupportInitialize)(this.pbStop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbPlay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbRecord)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarPosition)).BeginInit();
            this.SuspendLayout();
            // 
            // pbStop
            // 
            this.pbStop.Enabled = false;
            this.pbStop.Image = global::VSTiBox.Properties.Resources.stop;
            this.pbStop.Location = new System.Drawing.Point(533, 467);
            this.pbStop.Name = "pbStop";
            this.pbStop.Size = new System.Drawing.Size(64, 64);
            this.pbStop.TabIndex = 2;
            this.pbStop.TabStop = false;
            this.pbStop.Click += new System.EventHandler(this.pbStop_Click);
            // 
            // pbPlay
            // 
            this.pbPlay.Enabled = false;
            this.pbPlay.Image = global::VSTiBox.Properties.Resources.play;
            this.pbPlay.Location = new System.Drawing.Point(463, 467);
            this.pbPlay.Name = "pbPlay";
            this.pbPlay.Size = new System.Drawing.Size(64, 64);
            this.pbPlay.TabIndex = 1;
            this.pbPlay.TabStop = false;
            this.pbPlay.Click += new System.EventHandler(this.pbPlay_Click);
            // 
            // pbRecord
            // 
            this.pbRecord.Image = global::VSTiBox.Properties.Resources.record;
            this.pbRecord.Location = new System.Drawing.Point(393, 467);
            this.pbRecord.Name = "pbRecord";
            this.pbRecord.Size = new System.Drawing.Size(64, 64);
            this.pbRecord.TabIndex = 0;
            this.pbRecord.TabStop = false;
            this.pbRecord.Click += new System.EventHandler(this.pbRecord_Click);
            // 
            // lblRec
            // 
            this.lblRec.AutoSize = true;
            this.lblRec.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRec.ForeColor = System.Drawing.Color.White;
            this.lblRec.Location = new System.Drawing.Point(410, 534);
            this.lblRec.Name = "lblRec";
            this.lblRec.Size = new System.Drawing.Size(0, 18);
            this.lblRec.TabIndex = 6;
            // 
            // trackBarPosition
            // 
            this.trackBarPosition.Enabled = false;
            this.trackBarPosition.Location = new System.Drawing.Point(3, 372);
            this.trackBarPosition.Maximum = 100;
            this.trackBarPosition.Name = "trackBarPosition";
            this.trackBarPosition.Size = new System.Drawing.Size(932, 45);
            this.trackBarPosition.TabIndex = 7;
            this.trackBarPosition.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarPosition.Scroll += new System.EventHandler(this.trackBarPlay_Scroll);
            // 
            // timerPlay
            // 
            this.timerPlay.Tick += new System.EventHandler(this.timerPlay_Tick);
            // 
            // cbLoop
            // 
            this.cbLoop.AutoSize = true;
            this.cbLoop.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbLoop.ForeColor = System.Drawing.Color.White;
            this.cbLoop.Location = new System.Drawing.Point(603, 514);
            this.cbLoop.Name = "cbLoop";
            this.cbLoop.Size = new System.Drawing.Size(58, 20);
            this.cbLoop.TabIndex = 8;
            this.cbLoop.Text = "Loop";
            this.cbLoop.UseVisualStyleBackColor = true;
            // 
            // lblTime
            // 
            this.lblTime.AutoSize = true;
            this.lblTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTime.ForeColor = System.Drawing.Color.White;
            this.lblTime.Location = new System.Drawing.Point(857, 392);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(66, 16);
            this.lblTime.TabIndex = 9;
            this.lblTime.Text = "0:00 / 0:00";
            // 
            // scrollListRecordings
            // 
            this.scrollListRecordings.Items = new string[0];
            this.scrollListRecordings.Location = new System.Drawing.Point(0, 10);
            this.scrollListRecordings.Name = "scrollListRecordings";
            this.scrollListRecordings.SelectedIndex = 0;
            this.scrollListRecordings.Size = new System.Drawing.Size(344, 356);
            this.scrollListRecordings.TabIndex = 5;
            this.scrollListRecordings.TextColor = System.Drawing.Color.White;
            this.scrollListRecordings.ValueChanged += new System.EventHandler(this.scrollListRecordings_ValueChanged);
            // 
            // volumeMeter2
            // 
            this.volumeMeter2.Amplitude = 0F;
            this.volumeMeter2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.volumeMeter2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.volumeMeter2.Location = new System.Drawing.Point(900, 423);
            this.volumeMeter2.MaxDb = 3F;
            this.volumeMeter2.MinDb = -60F;
            this.volumeMeter2.Name = "volumeMeter2";
            this.volumeMeter2.Size = new System.Drawing.Size(14, 102);
            this.volumeMeter2.TabIndex = 19;
            this.volumeMeter2.Text = "volumeMeter1";
            // 
            // volumeMeter1
            // 
            this.volumeMeter1.Amplitude = 0F;
            this.volumeMeter1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.volumeMeter1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.volumeMeter1.Location = new System.Drawing.Point(882, 423);
            this.volumeMeter1.MaxDb = 3F;
            this.volumeMeter1.MinDb = -60F;
            this.volumeMeter1.Name = "volumeMeter1";
            this.volumeMeter1.Size = new System.Drawing.Size(14, 102);
            this.volumeMeter1.TabIndex = 20;
            this.volumeMeter1.Text = "volumeMeter1";
            // 
            // knobVolume
            // 
            this.knobVolume.ForeColor = System.Drawing.Color.White;
            this.knobVolume.KnobColor = System.Drawing.Color.White;
            this.knobVolume.Location = new System.Drawing.Point(824, 472);
            this.knobVolume.MarkerColor = System.Drawing.Color.White;
            this.knobVolume.Name = "knobVolume";
            this.knobVolume.Size = new System.Drawing.Size(52, 62);
            this.knobVolume.TabIndex = 21;
            this.knobVolume.TickColor = System.Drawing.Color.White;
            this.knobVolume.ValueChanged += new System.EventHandler(this.knobVolume_ValueChanged);
            // 
            // RecordControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(23)))), ((int)(((byte)(23)))));
            this.Controls.Add(this.knobVolume);
            this.Controls.Add(this.volumeMeter2);
            this.Controls.Add(this.volumeMeter1);
            this.Controls.Add(this.lblTime);
            this.Controls.Add(this.cbLoop);
            this.Controls.Add(this.lblRec);
            this.Controls.Add(this.scrollListRecordings);
            this.Controls.Add(this.pbStop);
            this.Controls.Add(this.pbPlay);
            this.Controls.Add(this.pbRecord);
            this.Controls.Add(this.trackBarPosition);
            this.Name = "RecordControl";
            this.Size = new System.Drawing.Size(1054, 554);
            ((System.ComponentModel.ISupportInitialize)(this.pbStop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbPlay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbRecord)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarPosition)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbRecord;
        private System.Windows.Forms.PictureBox pbPlay;
        private System.Windows.Forms.PictureBox pbStop;
        private ScrollList scrollListRecordings;
        private System.Windows.Forms.Label lblRec;
        private System.Windows.Forms.TrackBar trackBarPosition;
        private System.Windows.Forms.Timer timerPlay;
        private System.Windows.Forms.CheckBox cbLoop;
        private System.Windows.Forms.Label lblTime;
        private NAudio.Gui.VolumeMeter volumeMeter2;
        private NAudio.Gui.VolumeMeter volumeMeter1;
        private Knob knobVolume;

    }
}
