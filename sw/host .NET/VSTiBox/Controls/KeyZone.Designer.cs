namespace VSTiBox.Controls
{
    partial class KeyZone
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
            this.label6 = new System.Windows.Forms.Label();
            this.nudTranspose = new System.Windows.Forms.NumericUpDown();
            this.btnAddOctave = new System.Windows.Forms.Button();
            this.btnSubtractOctave = new System.Windows.Forms.Button();
            this.rbtnControlPedalEffect = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.rbtnControlPedalVolume = new System.Windows.Forms.RadioButton();
            this.rbtnControlPedalNone = new System.Windows.Forms.RadioButton();
            this.cbNoteDrop = new System.Windows.Forms.CheckBox();
            this.pianoControl1 = new VSTiBox.Controls.Piano();
            this.comboNoteDropDelay = new System.Windows.Forms.ComboBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.cbSustain = new System.Windows.Forms.CheckBox();
            this.cbExpressionInvert = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.nudKeyboardVelocityOffset = new System.Windows.Forms.NumericUpDown();
            this.nudKeyboardVelocityGain = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.nudTranspose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudKeyboardVelocityOffset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudKeyboardVelocityGain)).BeginInit();
            this.SuspendLayout();
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(23)))), ((int)(((byte)(23)))));
            this.label6.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label6.Location = new System.Drawing.Point(26, 51);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(57, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "Transpose";
            // 
            // nudTranspose
            // 
            this.nudTranspose.Location = new System.Drawing.Point(89, 49);
            this.nudTranspose.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.nudTranspose.Name = "nudTranspose";
            this.nudTranspose.Size = new System.Drawing.Size(55, 20);
            this.nudTranspose.TabIndex = 8;
            this.nudTranspose.ValueChanged += new System.EventHandler(this.nudTranspose_ValueChanged);
            // 
            // btnAddOctave
            // 
            this.btnAddOctave.Location = new System.Drawing.Point(151, 47);
            this.btnAddOctave.Name = "btnAddOctave";
            this.btnAddOctave.Size = new System.Drawing.Size(57, 24);
            this.btnAddOctave.TabIndex = 9;
            this.btnAddOctave.Text = "+ octave";
            this.btnAddOctave.UseVisualStyleBackColor = true;
            this.btnAddOctave.Click += new System.EventHandler(this.btnAddOctave_Click);
            // 
            // btnSubtractOctave
            // 
            this.btnSubtractOctave.Location = new System.Drawing.Point(214, 47);
            this.btnSubtractOctave.Name = "btnSubtractOctave";
            this.btnSubtractOctave.Size = new System.Drawing.Size(57, 24);
            this.btnSubtractOctave.TabIndex = 10;
            this.btnSubtractOctave.Text = "- octave";
            this.btnSubtractOctave.UseVisualStyleBackColor = true;
            this.btnSubtractOctave.Click += new System.EventHandler(this.btnSubtractOctave_Click);
            // 
            // rbtnControlPedalEffect
            // 
            this.rbtnControlPedalEffect.AutoSize = true;
            this.rbtnControlPedalEffect.Checked = true;
            this.rbtnControlPedalEffect.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.rbtnControlPedalEffect.Location = new System.Drawing.Point(352, 35);
            this.rbtnControlPedalEffect.Name = "rbtnControlPedalEffect";
            this.rbtnControlPedalEffect.Size = new System.Drawing.Size(88, 17);
            this.rbtnControlPedalEffect.TabIndex = 12;
            this.rbtnControlPedalEffect.TabStop = true;
            this.rbtnControlPedalEffect.Text = "Effect control";
            this.rbtnControlPedalEffect.UseVisualStyleBackColor = true;
            this.rbtnControlPedalEffect.CheckedChanged += new System.EventHandler(this.rbtnControlPedalEffect_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(23)))), ((int)(((byte)(23)))));
            this.label2.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label2.Location = new System.Drawing.Point(349, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(144, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "MIDI expression pedal action";
            // 
            // rbtnControlPedalVolume
            // 
            this.rbtnControlPedalVolume.AutoSize = true;
            this.rbtnControlPedalVolume.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.rbtnControlPedalVolume.Location = new System.Drawing.Point(352, 55);
            this.rbtnControlPedalVolume.Name = "rbtnControlPedalVolume";
            this.rbtnControlPedalVolume.Size = new System.Drawing.Size(60, 17);
            this.rbtnControlPedalVolume.TabIndex = 14;
            this.rbtnControlPedalVolume.Text = "Volume";
            this.rbtnControlPedalVolume.UseVisualStyleBackColor = true;
            this.rbtnControlPedalVolume.CheckedChanged += new System.EventHandler(this.rbtnControlPedalVolume_CheckedChanged);
            // 
            // rbtnControlPedalNone
            // 
            this.rbtnControlPedalNone.AutoSize = true;
            this.rbtnControlPedalNone.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.rbtnControlPedalNone.Location = new System.Drawing.Point(352, 75);
            this.rbtnControlPedalNone.Name = "rbtnControlPedalNone";
            this.rbtnControlPedalNone.Size = new System.Drawing.Size(51, 17);
            this.rbtnControlPedalNone.TabIndex = 15;
            this.rbtnControlPedalNone.Text = "None";
            this.rbtnControlPedalNone.UseVisualStyleBackColor = true;
            this.rbtnControlPedalNone.CheckedChanged += new System.EventHandler(this.rbtnControlPedalNone_CheckedChanged);
            // 
            // cbNoteDrop
            // 
            this.cbNoteDrop.AutoSize = true;
            this.cbNoteDrop.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.cbNoteDrop.Location = new System.Drawing.Point(618, 35);
            this.cbNoteDrop.Name = "cbNoteDrop";
            this.cbNoteDrop.Size = new System.Drawing.Size(73, 17);
            this.cbNoteDrop.TabIndex = 16;
            this.cbNoteDrop.Text = "Note drop";
            this.cbNoteDrop.UseVisualStyleBackColor = true;
            this.cbNoteDrop.CheckedChanged += new System.EventHandler(this.cbNoteDrop_CheckedChanged);
            // 
            // pianoControl1
            // 
            this.pianoControl1.Location = new System.Drawing.Point(29, 109);
            this.pianoControl1.Name = "pianoControl1";
            this.pianoControl1.Size = new System.Drawing.Size(989, 87);
            this.pianoControl1.TabIndex = 6;
            this.pianoControl1.PianoKeyDown += new System.EventHandler<VSTiBox.Controls.PianoKeyEvent>(this.pianoControl1_PianoKeyDown);
            this.pianoControl1.PianoKeyUp += new System.EventHandler<VSTiBox.Controls.PianoKeyEvent>(this.pianoControl1_PianoKeyUp);
            // 
            // comboNoteDropDelay
            // 
            this.comboNoteDropDelay.Enabled = false;
            this.comboNoteDropDelay.FormattingEnabled = true;
            this.comboNoteDropDelay.Items.AddRange(new object[] {
            "1/8th",
            "1/4th",
            "1/2th",
            "1",
            "2",
            "4"});
            this.comboNoteDropDelay.Location = new System.Drawing.Point(697, 33);
            this.comboNoteDropDelay.Name = "comboNoteDropDelay";
            this.comboNoteDropDelay.Size = new System.Drawing.Size(55, 21);
            this.comboNoteDropDelay.TabIndex = 17;
            this.comboNoteDropDelay.SelectedIndexChanged += new System.EventHandler(this.comboNoteDropDelay_SelectedIndexChanged);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(943, 75);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 18;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // cbSustain
            // 
            this.cbSustain.AutoSize = true;
            this.cbSustain.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.cbSustain.Location = new System.Drawing.Point(618, 58);
            this.cbSustain.Name = "cbSustain";
            this.cbSustain.Size = new System.Drawing.Size(61, 17);
            this.cbSustain.TabIndex = 19;
            this.cbSustain.Text = "Sustain";
            this.cbSustain.UseVisualStyleBackColor = true;
            this.cbSustain.CheckedChanged += new System.EventHandler(this.cbSustain_CheckedChanged);
            // 
            // cbExpressionInvert
            // 
            this.cbExpressionInvert.AutoSize = true;
            this.cbExpressionInvert.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.cbExpressionInvert.Location = new System.Drawing.Point(499, 19);
            this.cbExpressionInvert.Name = "cbExpressionInvert";
            this.cbExpressionInvert.Size = new System.Drawing.Size(53, 17);
            this.cbExpressionInvert.TabIndex = 20;
            this.cbExpressionInvert.Text = "Invert";
            this.cbExpressionInvert.UseVisualStyleBackColor = true;
            this.cbExpressionInvert.CheckedChanged += new System.EventHandler(this.cbExpressionInvert_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(799, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 13);
            this.label1.TabIndex = 21;
            this.label1.Text = "Keyboard Velocity";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(799, 37);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 23;
            this.label3.Text = "Offset";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(799, 63);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 13);
            this.label4.TabIndex = 25;
            this.label4.Text = "Gain";
            // 
            // nudKeyboardVelocityOffset
            // 
            this.nudKeyboardVelocityOffset.Location = new System.Drawing.Point(840, 35);
            this.nudKeyboardVelocityOffset.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.nudKeyboardVelocityOffset.Name = "nudKeyboardVelocityOffset";
            this.nudKeyboardVelocityOffset.Size = new System.Drawing.Size(55, 20);
            this.nudKeyboardVelocityOffset.TabIndex = 26;
            this.nudKeyboardVelocityOffset.ValueChanged += new System.EventHandler(this.nudKeyboardVelocityOffset_ValueChanged);
            // 
            // nudKeyboardVelocityGain
            // 
            this.nudKeyboardVelocityGain.Location = new System.Drawing.Point(840, 61);
            this.nudKeyboardVelocityGain.Maximum = new decimal(new int[] {
            250,
            0,
            0,
            0});
            this.nudKeyboardVelocityGain.Name = "nudKeyboardVelocityGain";
            this.nudKeyboardVelocityGain.Size = new System.Drawing.Size(55, 20);
            this.nudKeyboardVelocityGain.TabIndex = 27;
            this.nudKeyboardVelocityGain.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudKeyboardVelocityGain.ValueChanged += new System.EventHandler(this.nudKeyboardVelocityGain_ValueChanged);
            // 
            // KeyZone
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(23)))), ((int)(((byte)(23)))));
            this.Controls.Add(this.nudKeyboardVelocityGain);
            this.Controls.Add(this.nudKeyboardVelocityOffset);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbExpressionInvert);
            this.Controls.Add(this.cbSustain);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.comboNoteDropDelay);
            this.Controls.Add(this.cbNoteDrop);
            this.Controls.Add(this.rbtnControlPedalNone);
            this.Controls.Add(this.rbtnControlPedalVolume);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.rbtnControlPedalEffect);
            this.Controls.Add(this.btnSubtractOctave);
            this.Controls.Add(this.btnAddOctave);
            this.Controls.Add(this.nudTranspose);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.pianoControl1);
            this.Name = "KeyZone";
            this.Size = new System.Drawing.Size(1054, 564);
            this.VisibleChanged += new System.EventHandler(this.KeyZoneControl_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.nudTranspose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudKeyboardVelocityOffset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudKeyboardVelocityGain)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Piano pianoControl1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown nudTranspose;
        private System.Windows.Forms.Button btnAddOctave;
        private System.Windows.Forms.Button btnSubtractOctave;
        private System.Windows.Forms.RadioButton rbtnControlPedalEffect;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton rbtnControlPedalVolume;
        private System.Windows.Forms.RadioButton rbtnControlPedalNone;
        private System.Windows.Forms.CheckBox cbNoteDrop;
        private System.Windows.Forms.ComboBox comboNoteDropDelay;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.CheckBox cbSustain;
        private System.Windows.Forms.CheckBox cbExpressionInvert;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown nudKeyboardVelocityOffset;
        private System.Windows.Forms.NumericUpDown nudKeyboardVelocityGain;
    }
}
