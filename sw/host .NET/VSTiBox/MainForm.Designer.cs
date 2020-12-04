namespace VSTiBox
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.pnlSettings = new System.Windows.Forms.Panel();
            this.lbMidiInputs = new System.Windows.Forms.ListBox();
            this.btnVstFolderRemove = new System.Windows.Forms.Button();
            this.listbVstFolders = new System.Windows.Forms.ListBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label14 = new System.Windows.Forms.Label();
            this.btnVstFolderAdd = new System.Windows.Forms.Button();
            this.cbMP3PlayerOutput = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.audioPlaybackPanelMultiTrack = new VSTiBox.AudioPlayback.AudioPlaybackPanel();
            this.audioPlaybackPanelClickTrack = new VSTiBox.AudioPlayback.AudioPlaybackPanel();
            this.label10 = new System.Windows.Forms.Label();
            this.listbMetronomeClicks = new System.Windows.Forms.ListBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tbMetronomeSamplePath = new System.Windows.Forms.TextBox();
            this.btnMetronomeClickFolder = new System.Windows.Forms.Button();
            this.cbMultiTrackOutput = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.cbClickTrackOutput = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnSave = new System.Windows.Forms.Button();
            this.cbAsioBufferSize = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cbMidiOut = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cbAsioDriver = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pluginTimer = new System.Windows.Forms.Timer(this.components);
            this.lblTime = new System.Windows.Forms.Label();
            this.lblTempCase = new System.Windows.Forms.Label();
            this.lblTempCPU = new System.Windows.Forms.Label();
            this.systemTimer = new System.Windows.Forms.Timer(this.components);
            this.lblUsageCPU = new System.Windows.Forms.Label();
            this.lblUsageMemory = new System.Windows.Forms.Label();
            this.lblBPM = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.pnlBankEditor = new System.Windows.Forms.Panel();
            this.btnCopyPlayList = new System.Windows.Forms.Button();
            this.btnRemoveFromPlayList = new System.Windows.Forms.Button();
            this.btnAddToPlayList = new System.Windows.Forms.Button();
            this.btnRemovePlayList = new System.Windows.Forms.Button();
            this.btnAddPlayList = new System.Windows.Forms.Button();
            this.cbPlayList = new System.Windows.Forms.ComboBox();
            this.listBoxBanks = new System.Windows.Forms.ListBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.dragDropPlayList = new VSTiBox.DragDropListBox.DragDropListBox();
            this.btnLoadPlayList = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.pbMasterFxInsert = new System.Windows.Forms.PictureBox();
            this.pbBPMDown = new System.Windows.Forms.PictureBox();
            this.pbBPMUp = new System.Windows.Forms.PictureBox();
            this.pictureBoxCPU = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pnlEditorHostScroller = new System.Windows.Forms.Panel();
            this.onSongControl = new VSTiBox.Controls.OnSongControl();
            this.knobMasterPan = new VSTiBox.Knob();
            this.vuLeft = new VSTiBox.VuMeter();
            this.vuRight = new VSTiBox.VuMeter();
            this.menuControl = new VSTiBox.MenuControl();
            this.bankControl = new VSTiBox.MenuControl();
            this.chCtrl7 = new VSTiBox.ChannelControl();
            this.chCtrl6 = new VSTiBox.ChannelControl();
            this.chCtrl5 = new VSTiBox.ChannelControl();
            this.chCtrl4 = new VSTiBox.ChannelControl();
            this.chCtrl3 = new VSTiBox.ChannelControl();
            this.chCtrl2 = new VSTiBox.ChannelControl();
            this.chCtrl1 = new VSTiBox.ChannelControl();
            this.chCtrl0 = new VSTiBox.ChannelControl();
            this.keyZoneControl1 = new VSTiBox.Controls.KeyZone();
            this.recordControl1 = new VSTiBox.Controls.RecordControl();
            this.effectPluginSelectionControl1 = new VSTiBox.EffectPluginSelectionControl();
            this.formStartTimer = new System.Windows.Forms.Timer(this.components);
            this.pnlSettings.SuspendLayout();
            this.pnlBankEditor.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbMasterFxInsert)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbBPMDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbBPMUp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCPU)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlSettings
            // 
            this.pnlSettings.Controls.Add(this.lbMidiInputs);
            this.pnlSettings.Controls.Add(this.btnVstFolderRemove);
            this.pnlSettings.Controls.Add(this.listbVstFolders);
            this.pnlSettings.Controls.Add(this.panel2);
            this.pnlSettings.Controls.Add(this.label14);
            this.pnlSettings.Controls.Add(this.btnVstFolderAdd);
            this.pnlSettings.Controls.Add(this.cbMP3PlayerOutput);
            this.pnlSettings.Controls.Add(this.label13);
            this.pnlSettings.Controls.Add(this.label12);
            this.pnlSettings.Controls.Add(this.label11);
            this.pnlSettings.Controls.Add(this.audioPlaybackPanelMultiTrack);
            this.pnlSettings.Controls.Add(this.audioPlaybackPanelClickTrack);
            this.pnlSettings.Controls.Add(this.label10);
            this.pnlSettings.Controls.Add(this.listbMetronomeClicks);
            this.pnlSettings.Controls.Add(this.label9);
            this.pnlSettings.Controls.Add(this.tbMetronomeSamplePath);
            this.pnlSettings.Controls.Add(this.btnMetronomeClickFolder);
            this.pnlSettings.Controls.Add(this.cbMultiTrackOutput);
            this.pnlSettings.Controls.Add(this.label8);
            this.pnlSettings.Controls.Add(this.cbClickTrackOutput);
            this.pnlSettings.Controls.Add(this.label7);
            this.pnlSettings.Controls.Add(this.panel1);
            this.pnlSettings.Controls.Add(this.btnSave);
            this.pnlSettings.Controls.Add(this.cbAsioBufferSize);
            this.pnlSettings.Controls.Add(this.label4);
            this.pnlSettings.Controls.Add(this.cbMidiOut);
            this.pnlSettings.Controls.Add(this.label3);
            this.pnlSettings.Controls.Add(this.label2);
            this.pnlSettings.Controls.Add(this.cbAsioDriver);
            this.pnlSettings.Controls.Add(this.label1);
            this.pnlSettings.Location = new System.Drawing.Point(0, 0);
            this.pnlSettings.Name = "pnlSettings";
            this.pnlSettings.Size = new System.Drawing.Size(1054, 634);
            this.pnlSettings.TabIndex = 14;
            this.pnlSettings.Visible = false;
            // 
            // lbMidiInputs
            // 
            this.lbMidiInputs.FormattingEnabled = true;
            this.lbMidiInputs.Location = new System.Drawing.Point(33, 105);
            this.lbMidiInputs.Name = "lbMidiInputs";
            this.lbMidiInputs.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.lbMidiInputs.Size = new System.Drawing.Size(205, 95);
            this.lbMidiInputs.TabIndex = 33;
            this.lbMidiInputs.SelectedIndexChanged += new System.EventHandler(this.lbMidiInputs_SelectedIndexChanged);
            // 
            // btnVstFolderRemove
            // 
            this.btnVstFolderRemove.Location = new System.Drawing.Point(418, 205);
            this.btnVstFolderRemove.Name = "btnVstFolderRemove";
            this.btnVstFolderRemove.Size = new System.Drawing.Size(24, 23);
            this.btnVstFolderRemove.TabIndex = 32;
            this.btnVstFolderRemove.Text = "-";
            this.btnVstFolderRemove.UseVisualStyleBackColor = true;
            this.btnVstFolderRemove.Click += new System.EventHandler(this.btnVstFolderRemove_Click);
            // 
            // listbVstFolders
            // 
            this.listbVstFolders.FormattingEnabled = true;
            this.listbVstFolders.Location = new System.Drawing.Point(267, 26);
            this.listbVstFolders.Name = "listbVstFolders";
            this.listbVstFolders.Size = new System.Drawing.Size(205, 173);
            this.listbVstFolders.TabIndex = 31;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(132)))), ((int)(((byte)(132)))));
            this.panel2.Location = new System.Drawing.Point(487, 18);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1, 518);
            this.panel2.TabIndex = 30;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label14.Location = new System.Drawing.Point(267, 10);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(62, 13);
            this.label14.TabIndex = 29;
            this.label14.Text = "VST folders";
            // 
            // btnVstFolderAdd
            // 
            this.btnVstFolderAdd.Location = new System.Drawing.Point(448, 205);
            this.btnVstFolderAdd.Name = "btnVstFolderAdd";
            this.btnVstFolderAdd.Size = new System.Drawing.Size(24, 23);
            this.btnVstFolderAdd.TabIndex = 27;
            this.btnVstFolderAdd.Text = "+";
            this.btnVstFolderAdd.UseVisualStyleBackColor = true;
            this.btnVstFolderAdd.Click += new System.EventHandler(this.btnVstFolderAdd_Click);
            // 
            // cbMP3PlayerOutput
            // 
            this.cbMP3PlayerOutput.FormattingEnabled = true;
            this.cbMP3PlayerOutput.Location = new System.Drawing.Point(33, 266);
            this.cbMP3PlayerOutput.Name = "cbMP3PlayerOutput";
            this.cbMP3PlayerOutput.Size = new System.Drawing.Size(205, 21);
            this.cbMP3PlayerOutput.TabIndex = 26;
            this.cbMP3PlayerOutput.SelectedIndexChanged += new System.EventHandler(this.cbMP3PlayerOutput_SelectedIndexChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label13.Location = new System.Drawing.Point(33, 250);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(93, 13);
            this.label13.TabIndex = 25;
            this.label13.Text = "MP3 player output";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label12.Location = new System.Drawing.Point(502, 230);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(54, 13);
            this.label12.TabIndex = 23;
            this.label12.Text = "Clicktrack";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label11.Location = new System.Drawing.Point(502, 9);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(53, 13);
            this.label11.TabIndex = 22;
            this.label11.Text = "Multitrack";
            // 
            // audioPlaybackPanelMultiTrack
            // 
            this.audioPlaybackPanelMultiTrack.Location = new System.Drawing.Point(505, 25);
            this.audioPlaybackPanelMultiTrack.Name = "audioPlaybackPanelMultiTrack";
            this.audioPlaybackPanelMultiTrack.Size = new System.Drawing.Size(534, 202);
            this.audioPlaybackPanelMultiTrack.TabIndex = 21;
            // 
            // audioPlaybackPanelClickTrack
            // 
            this.audioPlaybackPanelClickTrack.Location = new System.Drawing.Point(505, 247);
            this.audioPlaybackPanelClickTrack.Name = "audioPlaybackPanelClickTrack";
            this.audioPlaybackPanelClickTrack.Size = new System.Drawing.Size(534, 202);
            this.audioPlaybackPanelClickTrack.TabIndex = 20;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label10.Location = new System.Drawing.Point(267, 291);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(85, 13);
            this.label10.TabIndex = 19;
            this.label10.Text = "Metronome click";
            // 
            // listbMetronomeClicks
            // 
            this.listbMetronomeClicks.Enabled = false;
            this.listbMetronomeClicks.FormattingEnabled = true;
            this.listbMetronomeClicks.Location = new System.Drawing.Point(267, 306);
            this.listbMetronomeClicks.Name = "listbMetronomeClicks";
            this.listbMetronomeClicks.Size = new System.Drawing.Size(205, 173);
            this.listbMetronomeClicks.TabIndex = 18;
            this.listbMetronomeClicks.SelectedIndexChanged += new System.EventHandler(this.listbMetronomeClicks_SelectedIndexChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label9.Location = new System.Drawing.Point(267, 251);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(119, 13);
            this.label9.TabIndex = 17;
            this.label9.Text = "Metronome clicks folder";
            // 
            // tbMetronomeSamplePath
            // 
            this.tbMetronomeSamplePath.Location = new System.Drawing.Point(267, 267);
            this.tbMetronomeSamplePath.Name = "tbMetronomeSamplePath";
            this.tbMetronomeSamplePath.ReadOnly = true;
            this.tbMetronomeSamplePath.Size = new System.Drawing.Size(174, 20);
            this.tbMetronomeSamplePath.TabIndex = 16;
            // 
            // btnMetronomeClickFolder
            // 
            this.btnMetronomeClickFolder.Location = new System.Drawing.Point(448, 266);
            this.btnMetronomeClickFolder.Name = "btnMetronomeClickFolder";
            this.btnMetronomeClickFolder.Size = new System.Drawing.Size(24, 23);
            this.btnMetronomeClickFolder.TabIndex = 15;
            this.btnMetronomeClickFolder.Text = "...";
            this.btnMetronomeClickFolder.UseVisualStyleBackColor = true;
            this.btnMetronomeClickFolder.Click += new System.EventHandler(this.btnMetronomeClickFolder_Click);
            // 
            // cbMultiTrackOutput
            // 
            this.cbMultiTrackOutput.FormattingEnabled = true;
            this.cbMultiTrackOutput.Location = new System.Drawing.Point(33, 306);
            this.cbMultiTrackOutput.Name = "cbMultiTrackOutput";
            this.cbMultiTrackOutput.Size = new System.Drawing.Size(205, 21);
            this.cbMultiTrackOutput.TabIndex = 14;
            this.cbMultiTrackOutput.SelectedIndexChanged += new System.EventHandler(this.cbMultiTrackOutput_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label8.Location = new System.Drawing.Point(33, 290);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(86, 13);
            this.label8.TabIndex = 13;
            this.label8.Text = "Multitrack output";
            // 
            // cbClickTrackOutput
            // 
            this.cbClickTrackOutput.FormattingEnabled = true;
            this.cbClickTrackOutput.Location = new System.Drawing.Point(33, 346);
            this.cbClickTrackOutput.Name = "cbClickTrackOutput";
            this.cbClickTrackOutput.Size = new System.Drawing.Size(205, 21);
            this.cbClickTrackOutput.TabIndex = 12;
            this.cbClickTrackOutput.SelectedIndexChanged += new System.EventHandler(this.cbMetronomeOutput_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label7.Location = new System.Drawing.Point(33, 330);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(152, 13);
            this.label7.TabIndex = 11;
            this.label7.Text = "Metronome && Clicktrack output";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(132)))), ((int)(((byte)(132)))));
            this.panel1.Location = new System.Drawing.Point(251, 19);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1, 518);
            this.panel1.TabIndex = 10;
            // 
            // btnSave
            // 
            this.btnSave.Enabled = false;
            this.btnSave.Location = new System.Drawing.Point(976, 524);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 9;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // cbAsioBufferSize
            // 
            this.cbAsioBufferSize.FormattingEnabled = true;
            this.cbAsioBufferSize.Items.AddRange(new object[] {
            "32",
            "64",
            "96",
            "128",
            "192",
            "196",
            "200",
            "204",
            "256",
            "512",
            "1024"});
            this.cbAsioBufferSize.Location = new System.Drawing.Point(33, 65);
            this.cbAsioBufferSize.Name = "cbAsioBufferSize";
            this.cbAsioBufferSize.Size = new System.Drawing.Size(80, 21);
            this.cbAsioBufferSize.TabIndex = 8;
            this.cbAsioBufferSize.SelectedIndexChanged += new System.EventHandler(this.cbAsioBufferSize_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label4.Location = new System.Drawing.Point(33, 49);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "ASIO buffer size";
            // 
            // cbMidiOut
            // 
            this.cbMidiOut.FormattingEnabled = true;
            this.cbMidiOut.Location = new System.Drawing.Point(33, 226);
            this.cbMidiOut.Name = "cbMidiOut";
            this.cbMidiOut.Size = new System.Drawing.Size(205, 21);
            this.cbMidiOut.TabIndex = 5;
            this.cbMidiOut.SelectedIndexChanged += new System.EventHandler(this.cbMidiOut_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label3.Location = new System.Drawing.Point(33, 210);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "MIDI output";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label2.Location = new System.Drawing.Point(33, 89);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "MIDI inputs";
            // 
            // cbAsioDriver
            // 
            this.cbAsioDriver.FormattingEnabled = true;
            this.cbAsioDriver.Location = new System.Drawing.Point(33, 25);
            this.cbAsioDriver.Name = "cbAsioDriver";
            this.cbAsioDriver.Size = new System.Drawing.Size(205, 21);
            this.cbAsioDriver.TabIndex = 1;
            this.cbAsioDriver.SelectedIndexChanged += new System.EventHandler(this.cbAsio_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label1.Location = new System.Drawing.Point(33, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "ASIO driver";
            // 
            // pluginTimer
            // 
            this.pluginTimer.Tick += new System.EventHandler(this.pluginTimer_Tick);
            // 
            // lblTime
            // 
            this.lblTime.AutoSize = true;
            this.lblTime.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTime.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblTime.Location = new System.Drawing.Point(1239, 781);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(38, 15);
            this.lblTime.TabIndex = 20;
            this.lblTime.Text = "15:01";
            // 
            // lblTempCase
            // 
            this.lblTempCase.AutoSize = true;
            this.lblTempCase.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTempCase.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblTempCase.Location = new System.Drawing.Point(1239, 757);
            this.lblTempCase.Name = "lblTempCase";
            this.lblTempCase.Size = new System.Drawing.Size(37, 15);
            this.lblTempCase.TabIndex = 21;
            this.lblTempCase.Text = "20 °C";
            // 
            // lblTempCPU
            // 
            this.lblTempCPU.AutoSize = true;
            this.lblTempCPU.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTempCPU.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblTempCPU.Location = new System.Drawing.Point(1239, 741);
            this.lblTempCPU.Name = "lblTempCPU";
            this.lblTempCPU.Size = new System.Drawing.Size(37, 15);
            this.lblTempCPU.TabIndex = 22;
            this.lblTempCPU.Text = "22 °C";
            // 
            // systemTimer
            // 
            this.systemTimer.Interval = 1000;
            this.systemTimer.Tick += new System.EventHandler(this.systemTimer_Tick);
            // 
            // lblUsageCPU
            // 
            this.lblUsageCPU.AutoSize = true;
            this.lblUsageCPU.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUsageCPU.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblUsageCPU.Location = new System.Drawing.Point(1239, 710);
            this.lblUsageCPU.Name = "lblUsageCPU";
            this.lblUsageCPU.Size = new System.Drawing.Size(26, 15);
            this.lblUsageCPU.TabIndex = 24;
            this.lblUsageCPU.Text = "0 %";
            // 
            // lblUsageMemory
            // 
            this.lblUsageMemory.AutoSize = true;
            this.lblUsageMemory.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUsageMemory.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblUsageMemory.Location = new System.Drawing.Point(1239, 725);
            this.lblUsageMemory.Name = "lblUsageMemory";
            this.lblUsageMemory.Size = new System.Drawing.Size(43, 15);
            this.lblUsageMemory.TabIndex = 26;
            this.lblUsageMemory.Text = "0.8 GB";
            // 
            // lblBPM
            // 
            this.lblBPM.AutoSize = true;
            this.lblBPM.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBPM.ForeColor = System.Drawing.Color.White;
            this.lblBPM.Location = new System.Drawing.Point(2, 656);
            this.lblBPM.Name = "lblBPM";
            this.lblBPM.Size = new System.Drawing.Size(29, 16);
            this.lblBPM.TabIndex = 31;
            this.lblBPM.Text = "120";
            this.lblBPM.DoubleClick += new System.EventHandler(this.lblBPM_DoubleClick);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(2, 642);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(30, 13);
            this.label5.TabIndex = 34;
            this.label5.Text = "BPM";
            // 
            // pnlBankEditor
            // 
            this.pnlBankEditor.Controls.Add(this.btnCopyPlayList);
            this.pnlBankEditor.Controls.Add(this.btnRemoveFromPlayList);
            this.pnlBankEditor.Controls.Add(this.btnAddToPlayList);
            this.pnlBankEditor.Controls.Add(this.btnRemovePlayList);
            this.pnlBankEditor.Controls.Add(this.btnAddPlayList);
            this.pnlBankEditor.Controls.Add(this.cbPlayList);
            this.pnlBankEditor.Controls.Add(this.listBoxBanks);
            this.pnlBankEditor.Controls.Add(this.label16);
            this.pnlBankEditor.Controls.Add(this.label15);
            this.pnlBankEditor.Controls.Add(this.dragDropPlayList);
            this.pnlBankEditor.Controls.Add(this.btnLoadPlayList);
            this.pnlBankEditor.Controls.Add(this.label6);
            this.pnlBankEditor.Location = new System.Drawing.Point(0, 0);
            this.pnlBankEditor.Name = "pnlBankEditor";
            this.pnlBankEditor.Size = new System.Drawing.Size(1054, 634);
            this.pnlBankEditor.TabIndex = 35;
            this.pnlBankEditor.VisibleChanged += new System.EventHandler(this.pnlBankEditor_VisibleChanged);
            // 
            // btnCopyPlayList
            // 
            this.btnCopyPlayList.Location = new System.Drawing.Point(544, 64);
            this.btnCopyPlayList.Name = "btnCopyPlayList";
            this.btnCopyPlayList.Size = new System.Drawing.Size(72, 23);
            this.btnCopyPlayList.TabIndex = 21;
            this.btnCopyPlayList.Text = "Copy";
            this.btnCopyPlayList.UseVisualStyleBackColor = true;
            this.btnCopyPlayList.Click += new System.EventHandler(this.btnCopyPlayList_Click);
            // 
            // btnRemoveFromPlayList
            // 
            this.btnRemoveFromPlayList.Enabled = false;
            this.btnRemoveFromPlayList.Location = new System.Drawing.Point(334, 251);
            this.btnRemoveFromPlayList.Name = "btnRemoveFromPlayList";
            this.btnRemoveFromPlayList.Size = new System.Drawing.Size(39, 23);
            this.btnRemoveFromPlayList.TabIndex = 20;
            this.btnRemoveFromPlayList.Text = "<--";
            this.btnRemoveFromPlayList.UseVisualStyleBackColor = true;
            this.btnRemoveFromPlayList.Click += new System.EventHandler(this.btnRemoveFromPlayList_Click);
            // 
            // btnAddToPlayList
            // 
            this.btnAddToPlayList.Enabled = false;
            this.btnAddToPlayList.Location = new System.Drawing.Point(334, 211);
            this.btnAddToPlayList.Name = "btnAddToPlayList";
            this.btnAddToPlayList.Size = new System.Drawing.Size(39, 23);
            this.btnAddToPlayList.TabIndex = 19;
            this.btnAddToPlayList.Text = "-->";
            this.btnAddToPlayList.UseVisualStyleBackColor = true;
            this.btnAddToPlayList.Click += new System.EventHandler(this.btnAddToPlayList_Click);
            // 
            // btnRemovePlayList
            // 
            this.btnRemovePlayList.Location = new System.Drawing.Point(474, 64);
            this.btnRemovePlayList.Name = "btnRemovePlayList";
            this.btnRemovePlayList.Size = new System.Drawing.Size(64, 23);
            this.btnRemovePlayList.TabIndex = 18;
            this.btnRemovePlayList.Text = "Remove";
            this.btnRemovePlayList.UseVisualStyleBackColor = true;
            this.btnRemovePlayList.Click += new System.EventHandler(this.btnRemovePlayList_Click);
            // 
            // btnAddPlayList
            // 
            this.btnAddPlayList.Location = new System.Drawing.Point(396, 64);
            this.btnAddPlayList.Name = "btnAddPlayList";
            this.btnAddPlayList.Size = new System.Drawing.Size(72, 23);
            this.btnAddPlayList.TabIndex = 17;
            this.btnAddPlayList.Text = "Add";
            this.btnAddPlayList.UseVisualStyleBackColor = true;
            this.btnAddPlayList.Click += new System.EventHandler(this.btnAddPlayList_Click);
            // 
            // cbPlayList
            // 
            this.cbPlayList.FormattingEnabled = true;
            this.cbPlayList.Location = new System.Drawing.Point(396, 37);
            this.cbPlayList.Name = "cbPlayList";
            this.cbPlayList.Size = new System.Drawing.Size(282, 21);
            this.cbPlayList.TabIndex = 16;
            this.cbPlayList.SelectedIndexChanged += new System.EventHandler(this.cbPlayList_SelectedIndexChanged);
            // 
            // listBoxBanks
            // 
            this.listBoxBanks.FormattingEnabled = true;
            this.listBoxBanks.Location = new System.Drawing.Point(25, 35);
            this.listBoxBanks.Name = "listBoxBanks";
            this.listBoxBanks.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.listBoxBanks.Size = new System.Drawing.Size(282, 485);
            this.listBoxBanks.TabIndex = 15;
            this.listBoxBanks.SelectedIndexChanged += new System.EventHandler(this.listBoxBanks_SelectedIndexChanged);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.ForeColor = System.Drawing.Color.White;
            this.label16.Location = new System.Drawing.Point(393, 19);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(39, 13);
            this.label16.TabIndex = 14;
            this.label16.Text = "Playlist";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.ForeColor = System.Drawing.Color.White;
            this.label15.Location = new System.Drawing.Point(397, 99);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(71, 13);
            this.label15.TabIndex = 13;
            this.label15.Text = "Playlist banks";
            // 
            // dragDropPlayList
            // 
            this.dragDropPlayList.AllowDrop = true;
            this.dragDropPlayList.FormattingEnabled = true;
            this.dragDropPlayList.Location = new System.Drawing.Point(396, 115);
            this.dragDropPlayList.Name = "dragDropPlayList";
            this.dragDropPlayList.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.dragDropPlayList.Size = new System.Drawing.Size(282, 407);
            this.dragDropPlayList.TabIndex = 11;
            this.dragDropPlayList.SelectedIndexChanged += new System.EventHandler(this.dragDropPlayList_SelectedIndexChanged);
            // 
            // btnLoadPlayList
            // 
            this.btnLoadPlayList.Location = new System.Drawing.Point(603, 531);
            this.btnLoadPlayList.Name = "btnLoadPlayList";
            this.btnLoadPlayList.Size = new System.Drawing.Size(75, 23);
            this.btnLoadPlayList.TabIndex = 10;
            this.btnLoadPlayList.Text = "Load";
            this.btnLoadPlayList.UseVisualStyleBackColor = true;
            this.btnLoadPlayList.Click += new System.EventHandler(this.btnLoadPlaylist_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(22, 17);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(37, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Banks";
            // 
            // pbMasterFxInsert
            // 
            this.pbMasterFxInsert.Image = global::VSTiBox.Properties.Resources.insert_on;
            this.pbMasterFxInsert.Location = new System.Drawing.Point(1245, 634);
            this.pbMasterFxInsert.Name = "pbMasterFxInsert";
            this.pbMasterFxInsert.Size = new System.Drawing.Size(32, 32);
            this.pbMasterFxInsert.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbMasterFxInsert.TabIndex = 37;
            this.pbMasterFxInsert.TabStop = false;
            this.pbMasterFxInsert.Click += new System.EventHandler(this.pbMasterFxInsert_Click);
            // 
            // pbBPMDown
            // 
            this.pbBPMDown.Image = global::VSTiBox.Properties.Resources.arrow_down;
            this.pbBPMDown.Location = new System.Drawing.Point(7, 696);
            this.pbBPMDown.Margin = new System.Windows.Forms.Padding(0);
            this.pbBPMDown.Name = "pbBPMDown";
            this.pbBPMDown.Size = new System.Drawing.Size(16, 16);
            this.pbBPMDown.TabIndex = 33;
            this.pbBPMDown.TabStop = false;
            this.pbBPMDown.Click += new System.EventHandler(this.pbBPMDown_Click);
            // 
            // pbBPMUp
            // 
            this.pbBPMUp.Image = global::VSTiBox.Properties.Resources.arrow_up;
            this.pbBPMUp.Location = new System.Drawing.Point(7, 674);
            this.pbBPMUp.Margin = new System.Windows.Forms.Padding(0);
            this.pbBPMUp.Name = "pbBPMUp";
            this.pbBPMUp.Size = new System.Drawing.Size(16, 16);
            this.pbBPMUp.TabIndex = 32;
            this.pbBPMUp.TabStop = false;
            this.pbBPMUp.Click += new System.EventHandler(this.pbBPMUp_Click);
            // 
            // pictureBoxCPU
            // 
            this.pictureBoxCPU.Image = global::VSTiBox.Properties.Resources.cpu;
            this.pictureBoxCPU.Location = new System.Drawing.Point(1220, 715);
            this.pictureBoxCPU.Name = "pictureBoxCPU";
            this.pictureBoxCPU.Size = new System.Drawing.Size(16, 16);
            this.pictureBoxCPU.TabIndex = 25;
            this.pictureBoxCPU.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::VSTiBox.Properties.Resources.thermometer;
            this.pictureBox1.Location = new System.Drawing.Point(1216, 743);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(24, 24);
            this.pictureBox1.TabIndex = 23;
            this.pictureBox1.TabStop = false;
            // 
            // pnlEditorHostScroller
            // 
            this.pnlEditorHostScroller.AutoScroll = true;
            this.pnlEditorHostScroller.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(23)))), ((int)(((byte)(23)))));
            this.pnlEditorHostScroller.Location = new System.Drawing.Point(0, 0);
            this.pnlEditorHostScroller.Name = "pnlEditorHostScroller";
            this.pnlEditorHostScroller.Size = new System.Drawing.Size(1054, 634);
            this.pnlEditorHostScroller.TabIndex = 41;
            // 
            // onSongControl
            // 
            this.onSongControl.Location = new System.Drawing.Point(0, 0);
            this.onSongControl.Name = "onSongControl";
            this.onSongControl.Size = new System.Drawing.Size(1054, 634);
            this.onSongControl.TabIndex = 42;
            // 
            // knobMasterPan
            // 
            this.knobMasterPan.ForeColor = System.Drawing.Color.White;
            this.knobMasterPan.KnobBorderStyle = VSTiBox.KnobStyle.FlatBorder;
            this.knobMasterPan.KnobColor = System.Drawing.Color.White;
            this.knobMasterPan.Location = new System.Drawing.Point(1236, 668);
            this.knobMasterPan.MarkerColor = System.Drawing.Color.White;
            this.knobMasterPan.Minimum = -100;
            this.knobMasterPan.Name = "knobMasterPan";
            this.knobMasterPan.Size = new System.Drawing.Size(46, 37);
            this.knobMasterPan.TabIndex = 36;
            this.knobMasterPan.TickColor = System.Drawing.Color.White;
            this.knobMasterPan.ValueChanged += new System.EventHandler(this.knobMasterPan_ValueChanged);
            // 
            // vuLeft
            // 
            this.vuLeft.Location = new System.Drawing.Point(1250, 4);
            this.vuLeft.Name = "vuLeft";
            this.vuLeft.Size = new System.Drawing.Size(15, 623);
            this.vuLeft.TabIndex = 16;
            this.vuLeft.Text = "vuLeft";
            this.vuLeft.Value = 0F;
            // 
            // vuRight
            // 
            this.vuRight.Location = new System.Drawing.Point(1265, 4);
            this.vuRight.Name = "vuRight";
            this.vuRight.Size = new System.Drawing.Size(15, 623);
            this.vuRight.TabIndex = 15;
            this.vuRight.Text = "vuRight";
            this.vuRight.Value = 0F;
            // 
            // menuControl
            // 
            this.menuControl.Location = new System.Drawing.Point(1060, 354);
            this.menuControl.Name = "menuControl";
            this.menuControl.SelectedIndex = 0;
            this.menuControl.Size = new System.Drawing.Size(187, 280);
            this.menuControl.TabIndex = 12;
            // 
            // bankControl
            // 
            this.bankControl.Location = new System.Drawing.Point(1060, 4);
            this.bankControl.Name = "bankControl";
            this.bankControl.SelectedIndex = 0;
            this.bankControl.Size = new System.Drawing.Size(187, 323);
            this.bankControl.TabIndex = 11;
            // 
            // chCtrl7
            // 
            this.chCtrl7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(23)))), ((int)(((byte)(23)))));
            this.chCtrl7.EffectPluginSelectionControl = null;
            this.chCtrl7.Location = new System.Drawing.Point(1086, 640);
            this.chCtrl7.Name = "chCtrl7";
            this.chCtrl7.PluginChannel = null;
            this.chCtrl7.Size = new System.Drawing.Size(150, 160);
            this.chCtrl7.TabIndex = 9;
            // 
            // chCtrl6
            // 
            this.chCtrl6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(23)))), ((int)(((byte)(23)))));
            this.chCtrl6.EffectPluginSelectionControl = null;
            this.chCtrl6.Location = new System.Drawing.Point(936, 640);
            this.chCtrl6.Name = "chCtrl6";
            this.chCtrl6.PluginChannel = null;
            this.chCtrl6.Size = new System.Drawing.Size(150, 160);
            this.chCtrl6.TabIndex = 8;
            // 
            // chCtrl5
            // 
            this.chCtrl5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(23)))), ((int)(((byte)(23)))));
            this.chCtrl5.EffectPluginSelectionControl = null;
            this.chCtrl5.Location = new System.Drawing.Point(786, 640);
            this.chCtrl5.Name = "chCtrl5";
            this.chCtrl5.PluginChannel = null;
            this.chCtrl5.Size = new System.Drawing.Size(150, 160);
            this.chCtrl5.TabIndex = 7;
            // 
            // chCtrl4
            // 
            this.chCtrl4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(23)))), ((int)(((byte)(23)))));
            this.chCtrl4.EffectPluginSelectionControl = null;
            this.chCtrl4.Location = new System.Drawing.Point(636, 640);
            this.chCtrl4.Name = "chCtrl4";
            this.chCtrl4.PluginChannel = null;
            this.chCtrl4.Size = new System.Drawing.Size(150, 160);
            this.chCtrl4.TabIndex = 6;
            // 
            // chCtrl3
            // 
            this.chCtrl3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(23)))), ((int)(((byte)(23)))));
            this.chCtrl3.EffectPluginSelectionControl = null;
            this.chCtrl3.Location = new System.Drawing.Point(484, 640);
            this.chCtrl3.Name = "chCtrl3";
            this.chCtrl3.PluginChannel = null;
            this.chCtrl3.Size = new System.Drawing.Size(150, 160);
            this.chCtrl3.TabIndex = 5;
            // 
            // chCtrl2
            // 
            this.chCtrl2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(23)))), ((int)(((byte)(23)))));
            this.chCtrl2.EffectPluginSelectionControl = null;
            this.chCtrl2.Location = new System.Drawing.Point(334, 640);
            this.chCtrl2.Name = "chCtrl2";
            this.chCtrl2.PluginChannel = null;
            this.chCtrl2.Size = new System.Drawing.Size(150, 160);
            this.chCtrl2.TabIndex = 4;
            // 
            // chCtrl1
            // 
            this.chCtrl1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(23)))), ((int)(((byte)(23)))));
            this.chCtrl1.EffectPluginSelectionControl = null;
            this.chCtrl1.Location = new System.Drawing.Point(184, 640);
            this.chCtrl1.Name = "chCtrl1";
            this.chCtrl1.PluginChannel = null;
            this.chCtrl1.Size = new System.Drawing.Size(150, 160);
            this.chCtrl1.TabIndex = 3;
            // 
            // chCtrl0
            // 
            this.chCtrl0.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(23)))), ((int)(((byte)(23)))));
            this.chCtrl0.EffectPluginSelectionControl = null;
            this.chCtrl0.Location = new System.Drawing.Point(38, 640);
            this.chCtrl0.Name = "chCtrl0";
            this.chCtrl0.PluginChannel = null;
            this.chCtrl0.Size = new System.Drawing.Size(150, 160);
            this.chCtrl0.TabIndex = 1;
            // 
            // keyZoneControl1
            // 
            this.keyZoneControl1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(23)))), ((int)(((byte)(23)))));
            this.keyZoneControl1.Location = new System.Drawing.Point(0, 0);
            this.keyZoneControl1.Name = "keyZoneControl1";
            this.keyZoneControl1.Size = new System.Drawing.Size(1054, 634);
            this.keyZoneControl1.TabIndex = 28;
            // 
            // recordControl1
            // 
            this.recordControl1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(23)))), ((int)(((byte)(23)))));
            this.recordControl1.Location = new System.Drawing.Point(0, 0);
            this.recordControl1.Name = "recordControl1";
            this.recordControl1.Size = new System.Drawing.Size(1054, 634);
            this.recordControl1.TabIndex = 30;
            // 
            // effectPluginSelectionControl1
            // 
            this.effectPluginSelectionControl1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(23)))), ((int)(((byte)(23)))));
            this.effectPluginSelectionControl1.Location = new System.Drawing.Point(0, 0);
            this.effectPluginSelectionControl1.Name = "effectPluginSelectionControl1";
            this.effectPluginSelectionControl1.Size = new System.Drawing.Size(1054, 634);
            this.effectPluginSelectionControl1.TabIndex = 38;
            // 
            // formStartTimer
            // 
            this.formStartTimer.Interval = 1000;
            this.formStartTimer.Tick += new System.EventHandler(this.formStartTimer_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(23)))), ((int)(((byte)(23)))));
            this.ClientSize = new System.Drawing.Size(1280, 800);
            this.Controls.Add(this.pbMasterFxInsert);
            this.Controls.Add(this.knobMasterPan);
            this.Controls.Add(this.pbBPMDown);
            this.Controls.Add(this.pbBPMUp);
            this.Controls.Add(this.lblUsageMemory);
            this.Controls.Add(this.pictureBoxCPU);
            this.Controls.Add(this.lblUsageCPU);
            this.Controls.Add(this.lblTempCPU);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lblTempCase);
            this.Controls.Add(this.lblTime);
            this.Controls.Add(this.vuLeft);
            this.Controls.Add(this.vuRight);
            this.Controls.Add(this.menuControl);
            this.Controls.Add(this.bankControl);
            this.Controls.Add(this.chCtrl7);
            this.Controls.Add(this.chCtrl6);
            this.Controls.Add(this.chCtrl5);
            this.Controls.Add(this.chCtrl4);
            this.Controls.Add(this.chCtrl3);
            this.Controls.Add(this.chCtrl2);
            this.Controls.Add(this.chCtrl1);
            this.Controls.Add(this.chCtrl0);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lblBPM);
            this.Controls.Add(this.pnlSettings);
            this.Controls.Add(this.pnlBankEditor);
            this.Controls.Add(this.keyZoneControl1);
            this.Controls.Add(this.recordControl1);
            this.Controls.Add(this.effectPluginSelectionControl1);
            this.Controls.Add(this.pnlEditorHostScroller);
            this.Controls.Add(this.onSongControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "MainForm";
            this.Text = "VSTiBox";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MainForm_KeyPress);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyUp);
            this.pnlSettings.ResumeLayout(false);
            this.pnlSettings.PerformLayout();
            this.pnlBankEditor.ResumeLayout(false);
            this.pnlBankEditor.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbMasterFxInsert)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbBPMDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbBPMUp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCPU)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ChannelControl chCtrl0;
        private ChannelControl chCtrl1;
        private ChannelControl chCtrl2;
        private ChannelControl chCtrl3;
        private ChannelControl chCtrl4;
        private ChannelControl chCtrl5;
        private ChannelControl chCtrl6;
        private ChannelControl chCtrl7;
        private MenuControl bankControl;
        private MenuControl menuControl;
        private System.Windows.Forms.Panel pnlSettings;
        private System.Windows.Forms.ComboBox cbAsioBufferSize;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbMidiOut;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbAsioDriver;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSave;
        private VuMeter vuRight;
        private VuMeter vuLeft;
        private System.Windows.Forms.Timer pluginTimer;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.Label lblTempCase;
        private System.Windows.Forms.Label lblTempCPU;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Timer systemTimer;
        private System.Windows.Forms.Label lblUsageCPU;
        private System.Windows.Forms.PictureBox pictureBoxCPU;
        private System.Windows.Forms.Label lblUsageMemory;
        private Controls.KeyZone keyZoneControl1;
        private Controls.RecordControl recordControl1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblBPM;
        private System.Windows.Forms.PictureBox pbBPMUp;
        private System.Windows.Forms.PictureBox pbBPMDown;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel pnlBankEditor;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnLoadPlayList;
        private Knob knobMasterPan;
        private System.Windows.Forms.PictureBox pbMasterFxInsert;
        private System.Windows.Forms.ComboBox cbMultiTrackOutput;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cbClickTrackOutput;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ListBox listbMetronomeClicks;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tbMetronomeSamplePath;
        private System.Windows.Forms.Button btnMetronomeClickFolder;
        private AudioPlayback.AudioPlaybackPanel audioPlaybackPanelClickTrack;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private AudioPlayback.AudioPlaybackPanel audioPlaybackPanelMultiTrack;
        private System.Windows.Forms.ComboBox cbMP3PlayerOutput;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button btnVstFolderAdd;
        private System.Windows.Forms.ListBox listbVstFolders;
        private System.Windows.Forms.Button btnVstFolderRemove;
        private System.Windows.Forms.ListBox lbMidiInputs;
        private System.Windows.Forms.Button btnRemovePlayList;
        private System.Windows.Forms.Button btnAddPlayList;
        private System.Windows.Forms.ComboBox cbPlayList;
        private System.Windows.Forms.ListBox listBoxBanks;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private DragDropListBox.DragDropListBox dragDropPlayList;
        private System.Windows.Forms.Button btnRemoveFromPlayList;
        private System.Windows.Forms.Button btnAddToPlayList;
        private System.Windows.Forms.Button btnCopyPlayList;
        private EffectPluginSelectionControl effectPluginSelectionControl1;
        private System.Windows.Forms.Panel pnlEditorHostScroller;
        private VSTiBox.Controls.OnSongControl onSongControl;
        private System.Windows.Forms.Timer formStartTimer;
    }
}