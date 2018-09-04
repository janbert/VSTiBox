namespace VSTiBox.Controls
{
    partial class Piano
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
            this.keyHighC = new VSTiBox.PianoKey();
            this.keyLowAis = new VSTiBox.PianoKey();
            this.keyLowA = new VSTiBox.PianoKey();
            this.keyLowB = new VSTiBox.PianoKey();
            this.octave1 = new VSTiBox.Controls.PianoOctave();
            this.octave7 = new VSTiBox.Controls.PianoOctave();
            this.octave6 = new VSTiBox.Controls.PianoOctave();
            this.octave5 = new VSTiBox.Controls.PianoOctave();
            this.octave4 = new VSTiBox.Controls.PianoOctave();
            this.octave3 = new VSTiBox.Controls.PianoOctave();
            this.octave2 = new VSTiBox.Controls.PianoOctave();
            this.pianoOctave1 = new VSTiBox.Controls.PianoOctave();
            this.SuspendLayout();
            // 
            // keyHighC
            // 
            this.keyHighC.KeyOffColor = System.Drawing.Color.White;
            this.keyHighC.KeyOnColor = System.Drawing.Color.Blue;
            this.keyHighC.Location = new System.Drawing.Point(969, 0);
            this.keyHighC.Name = "keyHighC";
            this.keyHighC.Orientation = VSTiBox.PianoKeyOrientation.Vertical;
            this.keyHighC.Shape = VSTiBox.PianoKeyShape.RectShape;
            this.keyHighC.Size = new System.Drawing.Size(20, 87);
            this.keyHighC.TabIndex = 15;
            this.keyHighC.Text = "pianoKey4";
            // 
            // keyLowAis
            // 
            this.keyLowAis.KeyOffColor = System.Drawing.Color.Black;
            this.keyLowAis.KeyOnColor = System.Drawing.Color.Blue;
            this.keyLowAis.Location = new System.Drawing.Point(13, 0);
            this.keyLowAis.Name = "keyLowAis";
            this.keyLowAis.Orientation = VSTiBox.PianoKeyOrientation.Vertical;
            this.keyLowAis.Shape = VSTiBox.PianoKeyShape.RectShape;
            this.keyLowAis.Size = new System.Drawing.Size(13, 58);
            this.keyLowAis.TabIndex = 14;
            this.keyLowAis.Text = "pianoKey4";
            // 
            // keyLowA
            // 
            this.keyLowA.KeyOffColor = System.Drawing.Color.White;
            this.keyLowA.KeyOnColor = System.Drawing.Color.Blue;
            this.keyLowA.Location = new System.Drawing.Point(0, 0);
            this.keyLowA.Name = "keyLowA";
            this.keyLowA.Orientation = VSTiBox.PianoKeyOrientation.Vertical;
            this.keyLowA.Shape = VSTiBox.PianoKeyShape.LShape;
            this.keyLowA.Size = new System.Drawing.Size(20, 87);
            this.keyLowA.TabIndex = 13;
            this.keyLowA.Text = "pianoKey1";
            // 
            // keyLowB
            // 
            this.keyLowB.KeyOffColor = System.Drawing.Color.White;
            this.keyLowB.KeyOnColor = System.Drawing.Color.Blue;
            this.keyLowB.Location = new System.Drawing.Point(19, 0);
            this.keyLowB.Name = "keyLowB";
            this.keyLowB.Orientation = VSTiBox.PianoKeyOrientation.Vertical;
            this.keyLowB.Shape = VSTiBox.PianoKeyShape.LShapeBackwards;
            this.keyLowB.Size = new System.Drawing.Size(20, 87);
            this.keyLowB.TabIndex = 11;
            this.keyLowB.Text = "pianoKey7";
            // 
            // octave1
            // 
            this.octave1.Location = new System.Drawing.Point(38, 0);
            this.octave1.Name = "octave1";
            this.octave1.Size = new System.Drawing.Size(133, 87);
            this.octave1.TabIndex = 8;
            // 
            // octave7
            // 
            this.octave7.Location = new System.Drawing.Point(836, 0);
            this.octave7.Name = "octave7";
            this.octave7.Size = new System.Drawing.Size(134, 87);
            this.octave7.TabIndex = 6;
            // 
            // octave6
            // 
            this.octave6.Location = new System.Drawing.Point(703, 0);
            this.octave6.Name = "octave6";
            this.octave6.Size = new System.Drawing.Size(134, 87);
            this.octave6.TabIndex = 5;
            // 
            // octave5
            // 
            this.octave5.Location = new System.Drawing.Point(570, 0);
            this.octave5.Name = "octave5";
            this.octave5.Size = new System.Drawing.Size(134, 87);
            this.octave5.TabIndex = 4;
            // 
            // octave4
            // 
            this.octave4.Location = new System.Drawing.Point(437, 0);
            this.octave4.Name = "octave4";
            this.octave4.Size = new System.Drawing.Size(134, 87);
            this.octave4.TabIndex = 3;
            // 
            // octave3
            // 
            this.octave3.Location = new System.Drawing.Point(304, 0);
            this.octave3.Name = "octave3";
            this.octave3.Size = new System.Drawing.Size(134, 87);
            this.octave3.TabIndex = 2;
            // 
            // octave2
            // 
            this.octave2.Location = new System.Drawing.Point(171, 0);
            this.octave2.Name = "octave2";
            this.octave2.Size = new System.Drawing.Size(134, 87);
            this.octave2.TabIndex = 1;
            // 
            // pianoOctave1
            // 
            this.pianoOctave1.Location = new System.Drawing.Point(44, 0);
            this.pianoOctave1.Name = "pianoOctave1";
            this.pianoOctave1.Size = new System.Drawing.Size(155, 87);
            this.pianoOctave1.TabIndex = 0;
            // 
            // PianoControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.keyHighC);
            this.Controls.Add(this.keyLowAis);
            this.Controls.Add(this.keyLowA);
            this.Controls.Add(this.keyLowB);
            this.Controls.Add(this.octave1);
            this.Controls.Add(this.octave7);
            this.Controls.Add(this.octave6);
            this.Controls.Add(this.octave5);
            this.Controls.Add(this.octave4);
            this.Controls.Add(this.octave3);
            this.Controls.Add(this.octave2);
            this.Controls.Add(this.pianoOctave1);
            this.Name = "PianoControl";
            this.Size = new System.Drawing.Size(989, 87);
            this.ResumeLayout(false);

        }

        #endregion

        private PianoOctave pianoOctave1;
        private PianoOctave octave2;
        private PianoOctave octave4;
        private PianoOctave octave3;
        private PianoOctave octave7;
        private PianoOctave octave6;
        private PianoOctave octave5;
        private PianoOctave octave1;
        private PianoKey keyLowB;
        private PianoKey keyLowAis;
        private PianoKey keyLowA;
        private PianoKey keyHighC;
    }
}
