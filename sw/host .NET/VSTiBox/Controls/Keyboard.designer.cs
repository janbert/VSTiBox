using System.Drawing;
namespace VSTiBox
{
    partial class Keyboard
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
            this.keyDot = new VSTiBox.KeyboardKey();
            this.keySpace = new VSTiBox.KeyboardKey();
            this.keyEnter = new VSTiBox.KeyboardKey();
            this.keyQuestionMark = new VSTiBox.KeyboardKey();
            this.keyComma = new VSTiBox.KeyboardKey();
            this.keyM = new VSTiBox.KeyboardKey();
            this.keyN = new VSTiBox.KeyboardKey();
            this.keyB = new VSTiBox.KeyboardKey();
            this.keyV = new VSTiBox.KeyboardKey();
            this.keyC = new VSTiBox.KeyboardKey();
            this.keyX = new VSTiBox.KeyboardKey();
            this.keyZ = new VSTiBox.KeyboardKey();
            this.keyShift = new VSTiBox.KeyboardKey();
            this.keyHighComma = new VSTiBox.KeyboardKey();
            this.keyL = new VSTiBox.KeyboardKey();
            this.keyK = new VSTiBox.KeyboardKey();
            this.keyJ = new VSTiBox.KeyboardKey();
            this.keyH = new VSTiBox.KeyboardKey();
            this.keyG = new VSTiBox.KeyboardKey();
            this.keyF = new VSTiBox.KeyboardKey();
            this.keyD = new VSTiBox.KeyboardKey();
            this.keyS = new VSTiBox.KeyboardKey();
            this.keyA = new VSTiBox.KeyboardKey();
            this.keyBackspace = new VSTiBox.KeyboardKey();
            this.keyP = new VSTiBox.KeyboardKey();
            this.keyO = new VSTiBox.KeyboardKey();
            this.keyI = new VSTiBox.KeyboardKey();
            this.keyU = new VSTiBox.KeyboardKey();
            this.keyY = new VSTiBox.KeyboardKey();
            this.keyT = new VSTiBox.KeyboardKey();
            this.keyR = new VSTiBox.KeyboardKey();
            this.keyE = new VSTiBox.KeyboardKey();
            this.keyW = new VSTiBox.KeyboardKey();
            this.keyQ = new VSTiBox.KeyboardKey();
            this.SuspendLayout();
            // 
            // keyDot
            // 
            this.keyDot.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(47)))), ((int)(((byte)(55)))));
            this.keyDot.ForeColor = System.Drawing.Color.White;
            this.keyDot.Location = new System.Drawing.Point(460, 112);
            this.keyDot.Momentary = true;
            this.keyDot.Name = "keyDot";
            this.keyDot.PressedColor = System.Drawing.Color.DarkBlue;
            this.keyDot.Size = new System.Drawing.Size(44, 45);
            this.keyDot.TabIndex = 35;
            this.keyDot.Text = ".";
            this.keyDot.Click += new System.EventHandler(this.key_Click);
            // 
            // keySpace
            // 
            this.keySpace.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(47)))), ((int)(((byte)(55)))));
            this.keySpace.ForeColor = System.Drawing.Color.White;
            this.keySpace.Location = new System.Drawing.Point(110, 163);
            this.keySpace.Momentary = true;
            this.keySpace.Name = "keySpace";
            this.keySpace.PressedColor = System.Drawing.Color.DarkBlue;
            this.keySpace.Size = new System.Drawing.Size(344, 45);
            this.keySpace.TabIndex = 34;
            this.keySpace.Text = " ";
            this.keySpace.Click += new System.EventHandler(this.key_Click);
            // 
            // keyEnter
            // 
            this.keyEnter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(47)))), ((int)(((byte)(55)))));
            this.keyEnter.ForeColor = System.Drawing.Color.White;
            this.keyEnter.Location = new System.Drawing.Point(530, 61);
            this.keyEnter.Momentary = true;
            this.keyEnter.Name = "keyEnter";
            this.keyEnter.PressedColor = System.Drawing.Color.DarkBlue;
            this.keyEnter.Size = new System.Drawing.Size(75, 45);
            this.keyEnter.TabIndex = 32;
            this.keyEnter.Text = "Enter";
            this.keyEnter.Click += new System.EventHandler(this.key_EnterClick);
            // 
            // keyQuestionMark
            // 
            this.keyQuestionMark.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(47)))), ((int)(((byte)(55)))));
            this.keyQuestionMark.ForeColor = System.Drawing.Color.White;
            this.keyQuestionMark.Location = new System.Drawing.Point(510, 112);
            this.keyQuestionMark.Momentary = true;
            this.keyQuestionMark.Name = "keyQuestionMark";
            this.keyQuestionMark.PressedColor = System.Drawing.Color.DarkBlue;
            this.keyQuestionMark.Size = new System.Drawing.Size(44, 45);
            this.keyQuestionMark.TabIndex = 31;
            this.keyQuestionMark.Text = "?";
            this.keyQuestionMark.Click += new System.EventHandler(this.key_Click);
            // 
            // keyComma
            // 
            this.keyComma.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(47)))), ((int)(((byte)(55)))));
            this.keyComma.ForeColor = System.Drawing.Color.White;
            this.keyComma.Location = new System.Drawing.Point(410, 112);
            this.keyComma.Momentary = true;
            this.keyComma.Name = "keyComma";
            this.keyComma.PressedColor = System.Drawing.Color.DarkBlue;
            this.keyComma.Size = new System.Drawing.Size(44, 45);
            this.keyComma.TabIndex = 29;
            this.keyComma.Text = ",";
            this.keyComma.Click += new System.EventHandler(this.key_Click);
            // 
            // keyM
            // 
            this.keyM.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(47)))), ((int)(((byte)(55)))));
            this.keyM.ForeColor = System.Drawing.Color.White;
            this.keyM.Location = new System.Drawing.Point(360, 112);
            this.keyM.Momentary = true;
            this.keyM.Name = "keyM";
            this.keyM.PressedColor = System.Drawing.Color.DarkBlue;
            this.keyM.Size = new System.Drawing.Size(44, 45);
            this.keyM.TabIndex = 28;
            this.keyM.Text = "m";
            this.keyM.Click += new System.EventHandler(this.key_Click);
            // 
            // keyN
            // 
            this.keyN.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(47)))), ((int)(((byte)(55)))));
            this.keyN.ForeColor = System.Drawing.Color.White;
            this.keyN.Location = new System.Drawing.Point(310, 112);
            this.keyN.Momentary = true;
            this.keyN.Name = "keyN";
            this.keyN.PressedColor = System.Drawing.Color.DarkBlue;
            this.keyN.Size = new System.Drawing.Size(44, 45);
            this.keyN.TabIndex = 27;
            this.keyN.Text = "n";
            this.keyN.Click += new System.EventHandler(this.key_Click);
            // 
            // keyB
            // 
            this.keyB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(47)))), ((int)(((byte)(55)))));
            this.keyB.ForeColor = System.Drawing.Color.White;
            this.keyB.Location = new System.Drawing.Point(260, 112);
            this.keyB.Momentary = true;
            this.keyB.Name = "keyB";
            this.keyB.PressedColor = System.Drawing.Color.DarkBlue;
            this.keyB.Size = new System.Drawing.Size(44, 45);
            this.keyB.TabIndex = 26;
            this.keyB.Text = "b";
            this.keyB.Click += new System.EventHandler(this.key_Click);
            // 
            // keyV
            // 
            this.keyV.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(47)))), ((int)(((byte)(55)))));
            this.keyV.ForeColor = System.Drawing.Color.White;
            this.keyV.Location = new System.Drawing.Point(210, 112);
            this.keyV.Momentary = true;
            this.keyV.Name = "keyV";
            this.keyV.PressedColor = System.Drawing.Color.DarkBlue;
            this.keyV.Size = new System.Drawing.Size(44, 45);
            this.keyV.TabIndex = 25;
            this.keyV.Text = "v";
            this.keyV.Click += new System.EventHandler(this.key_Click);
            // 
            // keyC
            // 
            this.keyC.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(47)))), ((int)(((byte)(55)))));
            this.keyC.ForeColor = System.Drawing.Color.White;
            this.keyC.Location = new System.Drawing.Point(160, 112);
            this.keyC.Momentary = true;
            this.keyC.Name = "keyC";
            this.keyC.PressedColor = System.Drawing.Color.DarkBlue;
            this.keyC.Size = new System.Drawing.Size(44, 45);
            this.keyC.TabIndex = 24;
            this.keyC.Text = "c";
            this.keyC.Click += new System.EventHandler(this.key_Click);
            // 
            // keyX
            // 
            this.keyX.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(47)))), ((int)(((byte)(55)))));
            this.keyX.ForeColor = System.Drawing.Color.White;
            this.keyX.Location = new System.Drawing.Point(110, 112);
            this.keyX.Momentary = true;
            this.keyX.Name = "keyX";
            this.keyX.PressedColor = System.Drawing.Color.DarkBlue;
            this.keyX.Size = new System.Drawing.Size(44, 45);
            this.keyX.TabIndex = 23;
            this.keyX.Text = "x";
            this.keyX.Click += new System.EventHandler(this.key_Click);
            // 
            // keyZ
            // 
            this.keyZ.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(47)))), ((int)(((byte)(55)))));
            this.keyZ.ForeColor = System.Drawing.Color.White;
            this.keyZ.Location = new System.Drawing.Point(60, 112);
            this.keyZ.Momentary = true;
            this.keyZ.Name = "keyZ";
            this.keyZ.PressedColor = System.Drawing.Color.DarkBlue;
            this.keyZ.Size = new System.Drawing.Size(44, 45);
            this.keyZ.TabIndex = 22;
            this.keyZ.Text = "z";
            this.keyZ.Click += new System.EventHandler(this.key_Click);
            // 
            // keyShift
            // 
            this.keyShift.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(47)))), ((int)(((byte)(55)))));
            this.keyShift.ForeColor = System.Drawing.Color.White;
            this.keyShift.Location = new System.Drawing.Point(10, 112);
            this.keyShift.Momentary = false;
            this.keyShift.Name = "keyShift";
            this.keyShift.PressedColor = System.Drawing.Color.DarkBlue;
            this.keyShift.Size = new System.Drawing.Size(44, 45);
            this.keyShift.TabIndex = 21;
            this.keyShift.Text = "Shift";
            this.keyShift.Click += new System.EventHandler(this.keyShift_Click);
            // 
            // keyHighComma
            // 
            this.keyHighComma.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(47)))), ((int)(((byte)(55)))));
            this.keyHighComma.ForeColor = System.Drawing.Color.White;
            this.keyHighComma.Location = new System.Drawing.Point(480, 61);
            this.keyHighComma.Momentary = true;
            this.keyHighComma.Name = "keyHighComma";
            this.keyHighComma.PressedColor = System.Drawing.Color.DarkBlue;
            this.keyHighComma.Size = new System.Drawing.Size(44, 45);
            this.keyHighComma.TabIndex = 20;
            this.keyHighComma.Text = "\'";
            this.keyHighComma.Click += new System.EventHandler(this.key_Click);
            // 
            // keyL
            // 
            this.keyL.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(47)))), ((int)(((byte)(55)))));
            this.keyL.ForeColor = System.Drawing.Color.White;
            this.keyL.Location = new System.Drawing.Point(430, 61);
            this.keyL.Momentary = true;
            this.keyL.Name = "keyL";
            this.keyL.PressedColor = System.Drawing.Color.DarkBlue;
            this.keyL.Size = new System.Drawing.Size(44, 45);
            this.keyL.TabIndex = 19;
            this.keyL.Text = "l";
            this.keyL.Click += new System.EventHandler(this.key_Click);
            // 
            // keyK
            // 
            this.keyK.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(47)))), ((int)(((byte)(55)))));
            this.keyK.ForeColor = System.Drawing.Color.White;
            this.keyK.Location = new System.Drawing.Point(380, 61);
            this.keyK.Momentary = true;
            this.keyK.Name = "keyK";
            this.keyK.PressedColor = System.Drawing.Color.DarkBlue;
            this.keyK.Size = new System.Drawing.Size(44, 45);
            this.keyK.TabIndex = 18;
            this.keyK.Text = "k";
            this.keyK.Click += new System.EventHandler(this.key_Click);
            // 
            // keyJ
            // 
            this.keyJ.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(47)))), ((int)(((byte)(55)))));
            this.keyJ.ForeColor = System.Drawing.Color.White;
            this.keyJ.Location = new System.Drawing.Point(330, 61);
            this.keyJ.Momentary = true;
            this.keyJ.Name = "keyJ";
            this.keyJ.PressedColor = System.Drawing.Color.DarkBlue;
            this.keyJ.Size = new System.Drawing.Size(44, 45);
            this.keyJ.TabIndex = 17;
            this.keyJ.Text = "j";
            this.keyJ.Click += new System.EventHandler(this.key_Click);
            // 
            // keyH
            // 
            this.keyH.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(47)))), ((int)(((byte)(55)))));
            this.keyH.ForeColor = System.Drawing.Color.White;
            this.keyH.Location = new System.Drawing.Point(280, 61);
            this.keyH.Momentary = true;
            this.keyH.Name = "keyH";
            this.keyH.PressedColor = System.Drawing.Color.DarkBlue;
            this.keyH.Size = new System.Drawing.Size(44, 45);
            this.keyH.TabIndex = 16;
            this.keyH.Text = "h";
            this.keyH.Click += new System.EventHandler(this.key_Click);
            // 
            // keyG
            // 
            this.keyG.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(47)))), ((int)(((byte)(55)))));
            this.keyG.ForeColor = System.Drawing.Color.White;
            this.keyG.Location = new System.Drawing.Point(230, 61);
            this.keyG.Momentary = true;
            this.keyG.Name = "keyG";
            this.keyG.PressedColor = System.Drawing.Color.DarkBlue;
            this.keyG.Size = new System.Drawing.Size(44, 45);
            this.keyG.TabIndex = 15;
            this.keyG.Text = "g";
            this.keyG.Click += new System.EventHandler(this.key_Click);
            // 
            // keyF
            // 
            this.keyF.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(47)))), ((int)(((byte)(55)))));
            this.keyF.ForeColor = System.Drawing.Color.White;
            this.keyF.Location = new System.Drawing.Point(180, 61);
            this.keyF.Momentary = true;
            this.keyF.Name = "keyF";
            this.keyF.PressedColor = System.Drawing.Color.DarkBlue;
            this.keyF.Size = new System.Drawing.Size(44, 45);
            this.keyF.TabIndex = 14;
            this.keyF.Text = "f";
            this.keyF.Click += new System.EventHandler(this.key_Click);
            // 
            // keyD
            // 
            this.keyD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(47)))), ((int)(((byte)(55)))));
            this.keyD.ForeColor = System.Drawing.Color.White;
            this.keyD.Location = new System.Drawing.Point(130, 61);
            this.keyD.Momentary = true;
            this.keyD.Name = "keyD";
            this.keyD.PressedColor = System.Drawing.Color.DarkBlue;
            this.keyD.Size = new System.Drawing.Size(44, 45);
            this.keyD.TabIndex = 13;
            this.keyD.Text = "d";
            this.keyD.Click += new System.EventHandler(this.key_Click);
            // 
            // keyS
            // 
            this.keyS.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(47)))), ((int)(((byte)(55)))));
            this.keyS.ForeColor = System.Drawing.Color.White;
            this.keyS.Location = new System.Drawing.Point(80, 61);
            this.keyS.Momentary = true;
            this.keyS.Name = "keyS";
            this.keyS.PressedColor = System.Drawing.Color.DarkBlue;
            this.keyS.Size = new System.Drawing.Size(44, 45);
            this.keyS.TabIndex = 12;
            this.keyS.Text = "s";
            this.keyS.Click += new System.EventHandler(this.key_Click);
            // 
            // keyA
            // 
            this.keyA.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(47)))), ((int)(((byte)(55)))));
            this.keyA.ForeColor = System.Drawing.Color.White;
            this.keyA.Location = new System.Drawing.Point(30, 61);
            this.keyA.Momentary = true;
            this.keyA.Name = "keyA";
            this.keyA.PressedColor = System.Drawing.Color.DarkBlue;
            this.keyA.Size = new System.Drawing.Size(44, 45);
            this.keyA.TabIndex = 11;
            this.keyA.Text = "a";
            this.keyA.Click += new System.EventHandler(this.key_Click);
            // 
            // keyBackspace
            // 
            this.keyBackspace.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(47)))), ((int)(((byte)(55)))));
            this.keyBackspace.ForeColor = System.Drawing.Color.White;
            this.keyBackspace.Location = new System.Drawing.Point(510, 10);
            this.keyBackspace.Momentary = true;
            this.keyBackspace.Name = "keyBackspace";
            this.keyBackspace.PressedColor = System.Drawing.Color.DarkBlue;
            this.keyBackspace.Size = new System.Drawing.Size(95, 45);
            this.keyBackspace.TabIndex = 10;
            this.keyBackspace.Text = "Backspace";
            this.keyBackspace.Click += new System.EventHandler(this.keyBackspace_Click);
            // 
            // keyP
            // 
            this.keyP.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(47)))), ((int)(((byte)(55)))));
            this.keyP.ForeColor = System.Drawing.Color.White;
            this.keyP.Location = new System.Drawing.Point(460, 10);
            this.keyP.Momentary = true;
            this.keyP.Name = "keyP";
            this.keyP.PressedColor = System.Drawing.Color.DarkBlue;
            this.keyP.Size = new System.Drawing.Size(44, 45);
            this.keyP.TabIndex = 9;
            this.keyP.Text = "p";
            this.keyP.Click += new System.EventHandler(this.key_Click);
            // 
            // keyO
            // 
            this.keyO.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(47)))), ((int)(((byte)(55)))));
            this.keyO.ForeColor = System.Drawing.Color.White;
            this.keyO.Location = new System.Drawing.Point(410, 10);
            this.keyO.Momentary = true;
            this.keyO.Name = "keyO";
            this.keyO.PressedColor = System.Drawing.Color.DarkBlue;
            this.keyO.Size = new System.Drawing.Size(44, 45);
            this.keyO.TabIndex = 8;
            this.keyO.Text = "o";
            this.keyO.Click += new System.EventHandler(this.key_Click);
            // 
            // keyI
            // 
            this.keyI.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(47)))), ((int)(((byte)(55)))));
            this.keyI.ForeColor = System.Drawing.Color.White;
            this.keyI.Location = new System.Drawing.Point(360, 10);
            this.keyI.Momentary = true;
            this.keyI.Name = "keyI";
            this.keyI.PressedColor = System.Drawing.Color.DarkBlue;
            this.keyI.Size = new System.Drawing.Size(44, 45);
            this.keyI.TabIndex = 7;
            this.keyI.Text = "i";
            this.keyI.Click += new System.EventHandler(this.key_Click);
            // 
            // keyU
            // 
            this.keyU.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(47)))), ((int)(((byte)(55)))));
            this.keyU.ForeColor = System.Drawing.Color.White;
            this.keyU.Location = new System.Drawing.Point(310, 10);
            this.keyU.Momentary = true;
            this.keyU.Name = "keyU";
            this.keyU.PressedColor = System.Drawing.Color.DarkBlue;
            this.keyU.Size = new System.Drawing.Size(44, 45);
            this.keyU.TabIndex = 6;
            this.keyU.Text = "u";
            this.keyU.Click += new System.EventHandler(this.key_Click);
            // 
            // keyY
            // 
            this.keyY.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(47)))), ((int)(((byte)(55)))));
            this.keyY.ForeColor = System.Drawing.Color.White;
            this.keyY.Location = new System.Drawing.Point(260, 10);
            this.keyY.Momentary = true;
            this.keyY.Name = "keyY";
            this.keyY.PressedColor = System.Drawing.Color.DarkBlue;
            this.keyY.Size = new System.Drawing.Size(44, 45);
            this.keyY.TabIndex = 5;
            this.keyY.Text = "y";
            this.keyY.Click += new System.EventHandler(this.key_Click);
            // 
            // keyT
            // 
            this.keyT.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(47)))), ((int)(((byte)(55)))));
            this.keyT.ForeColor = System.Drawing.Color.White;
            this.keyT.Location = new System.Drawing.Point(210, 10);
            this.keyT.Momentary = true;
            this.keyT.Name = "keyT";
            this.keyT.PressedColor = System.Drawing.Color.DarkBlue;
            this.keyT.Size = new System.Drawing.Size(44, 45);
            this.keyT.TabIndex = 4;
            this.keyT.Text = "t";
            this.keyT.Click += new System.EventHandler(this.key_Click);
            // 
            // keyR
            // 
            this.keyR.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(47)))), ((int)(((byte)(55)))));
            this.keyR.ForeColor = System.Drawing.Color.White;
            this.keyR.Location = new System.Drawing.Point(160, 10);
            this.keyR.Momentary = true;
            this.keyR.Name = "keyR";
            this.keyR.PressedColor = System.Drawing.Color.DarkBlue;
            this.keyR.Size = new System.Drawing.Size(44, 45);
            this.keyR.TabIndex = 3;
            this.keyR.Text = "r";
            this.keyR.Click += new System.EventHandler(this.key_Click);
            // 
            // keyE
            // 
            this.keyE.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(47)))), ((int)(((byte)(55)))));
            this.keyE.ForeColor = System.Drawing.Color.White;
            this.keyE.Location = new System.Drawing.Point(110, 10);
            this.keyE.Momentary = true;
            this.keyE.Name = "keyE";
            this.keyE.PressedColor = System.Drawing.Color.DarkBlue;
            this.keyE.Size = new System.Drawing.Size(44, 45);
            this.keyE.TabIndex = 2;
            this.keyE.Text = "e";
            this.keyE.Click += new System.EventHandler(this.key_Click);
            // 
            // keyW
            // 
            this.keyW.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(47)))), ((int)(((byte)(55)))));
            this.keyW.ForeColor = System.Drawing.Color.White;
            this.keyW.Location = new System.Drawing.Point(60, 10);
            this.keyW.Momentary = true;
            this.keyW.Name = "keyW";
            this.keyW.PressedColor = System.Drawing.Color.DarkBlue;
            this.keyW.Size = new System.Drawing.Size(44, 45);
            this.keyW.TabIndex = 1;
            this.keyW.Text = "w";
            this.keyW.Click += new System.EventHandler(this.key_Click);
            // 
            // keyQ
            // 
            this.keyQ.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(47)))), ((int)(((byte)(55)))));
            this.keyQ.ForeColor = System.Drawing.Color.White;
            this.keyQ.Location = new System.Drawing.Point(10, 10);
            this.keyQ.Momentary = true;
            this.keyQ.Name = "keyQ";
            this.keyQ.PressedColor = System.Drawing.Color.DarkBlue;
            this.keyQ.Size = new System.Drawing.Size(44, 45);
            this.keyQ.TabIndex = 0;
            this.keyQ.Text = "q";
            this.keyQ.Click += new System.EventHandler(this.key_Click);
            // 
            // KeyboardControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.keyDot);
            this.Controls.Add(this.keySpace);
            this.Controls.Add(this.keyEnter);
            this.Controls.Add(this.keyQuestionMark);
            this.Controls.Add(this.keyComma);
            this.Controls.Add(this.keyM);
            this.Controls.Add(this.keyN);
            this.Controls.Add(this.keyB);
            this.Controls.Add(this.keyV);
            this.Controls.Add(this.keyC);
            this.Controls.Add(this.keyX);
            this.Controls.Add(this.keyZ);
            this.Controls.Add(this.keyShift);
            this.Controls.Add(this.keyHighComma);
            this.Controls.Add(this.keyL);
            this.Controls.Add(this.keyK);
            this.Controls.Add(this.keyJ);
            this.Controls.Add(this.keyH);
            this.Controls.Add(this.keyG);
            this.Controls.Add(this.keyF);
            this.Controls.Add(this.keyD);
            this.Controls.Add(this.keyS);
            this.Controls.Add(this.keyA);
            this.Controls.Add(this.keyBackspace);
            this.Controls.Add(this.keyP);
            this.Controls.Add(this.keyO);
            this.Controls.Add(this.keyI);
            this.Controls.Add(this.keyU);
            this.Controls.Add(this.keyY);
            this.Controls.Add(this.keyT);
            this.Controls.Add(this.keyR);
            this.Controls.Add(this.keyE);
            this.Controls.Add(this.keyW);
            this.Controls.Add(this.keyQ);
            this.Name = "KeyboardControl";
            this.Size = new System.Drawing.Size(613, 218);
            this.ResumeLayout(false);

        }

        #endregion

        private KeyboardKey keyQ;
        private KeyboardKey keyW;
        private KeyboardKey keyE;
        private KeyboardKey keyR;
        private KeyboardKey keyT;
        private KeyboardKey keyP;
        private KeyboardKey keyO;
        private KeyboardKey keyI;
        private KeyboardKey keyU;
        private KeyboardKey keyY;
        private KeyboardKey keyBackspace;
        private KeyboardKey keyHighComma;
        private KeyboardKey keyL;
        private KeyboardKey keyK;
        private KeyboardKey keyJ;
        private KeyboardKey keyH;
        private KeyboardKey keyG;
        private KeyboardKey keyF;
        private KeyboardKey keyD;
        private KeyboardKey keyS;
        private KeyboardKey keyA;
        private KeyboardKey keyComma;
        private KeyboardKey keyM;
        private KeyboardKey keyN;
        private KeyboardKey keyB;
        private KeyboardKey keyV;
        private KeyboardKey keyC;
        private KeyboardKey keyX;
        private KeyboardKey keyZ;
        private KeyboardKey keyShift;
        private KeyboardKey keyQuestionMark;
        private KeyboardKey keyEnter;
        private KeyboardKey keySpace;
        private KeyboardKey keyDot;
    }
}
