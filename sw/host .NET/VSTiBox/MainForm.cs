using Jacobi.Vst.Interop.Host;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Linq;

#if NAUDIO_ASIO
#else
using BlueWave.Interop.Asio;
#endif

using System.Runtime.InteropServices;
using OpenHardwareMonitor.Hardware;
using VSTiBox.Common;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using VSTiBox.Controls;

namespace VSTiBox
{
    // todo: Pan mix
    // No chunk support : iterate Parameters ; test

    public partial class MainForm : Form
    {
        const int LED_RED_OFF = 0;
        const int LED_RED_DIMMED = 4;
        const int LED_RED_FULL = 65;

        private bool mBankHasOnsong = false; 
        private int mMenuEncPrevPos = 0;
        private bool mIgnoreEvents = true;
        private SettingsManager mSettingsMgr;
        private AudioPluginEngine mAudioPluginEngine;
       
        private ChannelControl[] mChannelControls;
        private BoardManager mBoardManager;
        private int mMouseCaptureCh;            // Current channel for mouse capture (when using keyboard keys)
        private MetronomeTimer mMetronomeTimer;

        private WasapiPlayer mMP3Player;
        private WasapiPlayer mClickTrackPlayer;
        private WasapiPlayer mMetronomePlayer;
        private WasapiPlayer mMultitrackPlayer;

        private Control[] mExclusiveVisibleControls;

        public MainForm()
        {
            InitializeComponent();

            mChannelControls = new ChannelControl[] { chCtrl0, chCtrl1, chCtrl2, chCtrl3, chCtrl4, chCtrl5, chCtrl6, chCtrl7 };    
            
            foreach(ChannelControl c in mChannelControls )
            {
                c.EffectPluginSelectionControl = effectPluginSelectionControl1;
                c.OnHighLighted += c_OnHighLighted;
            }

            // Array of UI controls of which only one is visible at a time
            mExclusiveVisibleControls = new Control[] { keyZoneControl1, 
                pnlEditorHostScroller, pnlSettings, pnlBankEditor, 
                recordControl1 ,effectPluginSelectionControl1, onSongControl  };

            foreach (Control control in mExclusiveVisibleControls)
            {
                control.VisibleChanged += control_VisibleChanged;
            }

            // Set other controls tot non-visible
            foreach (Control control in mExclusiveVisibleControls)
            {
                if (!control.Equals(pnlSettings))
                {
                    control.Visible = false;
                }
            }            
        }

        void control_VisibleChanged(object sender, EventArgs e)
        {
            // Control set to Visible?
            if(((Control)sender ).Visible )
            {
                if (mBankHasOnsong && onSongControl.Visible && !sender.Equals(onSongControl))
                {
                    // Other control tries to overrule onsongcontrol; ignore
                }
                else
                {
                    // Set other controls tot non-visible
                    foreach (Control control in mExclusiveVisibleControls)
                    {
                        if (!control.Equals(sender))
                        {
                            control.Visible = false;
                        }
                    }
                }
            }
        }

        void c_OnHighLighted(object sender, EventArgs e)
        {
            foreach (ChannelControl c in mChannelControls)
            {
                if (!c.Equals(sender))
                {
                    c.HighlightControl(false);
                }
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            mIgnoreEvents = true;
            PopupForm popup = new PopupForm("Loading VSTiBox v2.1", -1);
            popup.Show();
            Application.DoEvents();

            this.MouseWheel += MainForm_MouseWheel;

            mSettingsMgr = new SettingsManager();
           
             // Create audio plugin engine with 8 plugin channels 
            mAudioPluginEngine = new AudioPluginEngine(mSettingsMgr, new VuMeter[] { vuLeft, vuRight }, pnlEditorHostScroller);

            // Link the UI to the engine plugin channels 
            for (int i = 0; i < 8; i++)
            {
                mChannelControls[i].PluginChannel = mAudioPluginEngine.PluginChannels[i];
            }

            mMetronomeTimer = new MetronomeTimer();
            mMetronomeTimer.Tick += mMetronome_Tick;
            keyZoneControl1.SetPluginManager(mAudioPluginEngine);
            recordControl1.SetPluginManager(mAudioPluginEngine);

            knobMasterPan.Value = (int)(mSettingsMgr.Settings.MasterPan * 100.0f);

            // Settings: metronome
            setMetronomeSamplePath();

            // Settings: VST paths            
            listbVstFolders.Items.AddRange(mSettingsMgr.Settings.VSTPluginFolders.ToArray());

            // Settings: mp3player
            mMP3Player = new WasapiPlayer();
            mMP3Player.Volume = 1.0f;
            recordControl1.SetWasapiPlayer(mMP3Player);
            cbMP3PlayerOutput.Items.AddRange(mMP3Player.AvailableDeviceNames);
            if (mMP3Player.AvailableDeviceNames.Contains(mSettingsMgr.Settings.MP3PlayerDevice))
            {
                cbMP3PlayerOutput.SelectedItem = mSettingsMgr.Settings.MP3PlayerDevice;
            }
            else
            {
                cbMP3PlayerOutput.SelectedIndex = 0;
            }

            // Settings: multitrack
            mMultitrackPlayer = new WasapiPlayer();
            audioPlaybackPanelMultiTrack.SetWasapiPlayer(mMultitrackPlayer);
            cbMultiTrackOutput.Items.AddRange(mMultitrackPlayer.AvailableDeviceNames);
            if (mMultitrackPlayer.AvailableDeviceNames.Contains(mSettingsMgr.Settings.MultiTrackDevice))
            {
                cbMultiTrackOutput.SelectedItem = mSettingsMgr.Settings.MultiTrackDevice;
            }
            else
            {
                cbMultiTrackOutput.SelectedIndex = 0;
            }

            // Settings: clicktrack
            mClickTrackPlayer = new WasapiPlayer();
            audioPlaybackPanelClickTrack.SetWasapiPlayer(mClickTrackPlayer);
            cbClickTrackOutput.Items.AddRange(mClickTrackPlayer.AvailableDeviceNames);
            if (mClickTrackPlayer.AvailableDeviceNames.Contains(mSettingsMgr.Settings.MetronomeAndClickTrackDevice))
            {
                cbClickTrackOutput.SelectedItem = mSettingsMgr.Settings.MetronomeAndClickTrackDevice;
            }
            else
            {
                cbClickTrackOutput.SelectedIndex = 0;
            }

            // Metronome
            mMetronomePlayer = new WasapiPlayer();
            mMetronomePlayer.DeviceName = mSettingsMgr.Settings.MetronomeAndClickTrackDevice;
            mMetronomePlayer.FileName = mSettingsMgr.Settings.MetronomeSampleFile;
            mMetronomePlayer.Volume = 1.0f;

            // Board IO 
            mBoardManager = new BoardManager();
            systemTimer.Enabled = true;
            systemTimer_Tick(null, null);

            chCtrl0.SetBoard(mBoardManager.Boards[0], 3, 1);
            chCtrl1.SetBoard(mBoardManager.Boards[0], 1, 0);
            chCtrl2.SetBoard(mBoardManager.Boards[0], 2, 3);
            chCtrl3.SetBoard(mBoardManager.Boards[0], 0, 2);
            chCtrl4.SetBoard(mBoardManager.Boards[1], 3, 1);
            chCtrl5.SetBoard(mBoardManager.Boards[1], 1, 0);
            chCtrl6.SetBoard(mBoardManager.Boards[1], 2, 3);
            chCtrl7.SetBoard(mBoardManager.Boards[1], 0, 2);

            // Btn menu Enter
            mBoardManager.Boards[2].Buttons[0].Pressed += menuEnterBtnPressed;
            mBoardManager.Boards[2].Buttons[0].Released += menuEnterBtnReleased;
            // Btn menu ALT
            mBoardManager.Boards[3].Buttons[3].Pressed += menuAltBtnPressed;
            mBoardManager.Boards[3].Buttons[3].Released += menuAltBtnReleased;
            // Btn menu save
            mBoardManager.Boards[2].Buttons[2].Pressed += menuSaveBtnPressed;
            // Btn menu MidiPanic
            mBoardManager.Boards[2].Buttons[1].Pressed += menuPanicBtnPressed;

            // Rotary menu encoder
            mBoardManager.Boards[3].Encoders[1].DeltaChanged += menuEncAbsPosChanged;
            // Btn metronome
            mBoardManager.Boards[3].Buttons[0].Pressed += btnMetronome_Pressed;
            // Btn multitrack
            mBoardManager.Boards[2].Buttons[3].Pressed += btnMultiAndClickTrack_Pressed;
            // Encoder btn metronome off
            mBoardManager.Boards[3].Encoders[3].Button.Pressed += btnMetronomeOff_Pressed;
            // Onsong load
            mBoardManager.Boards[3].Buttons[1].Pressed += btnOnsongLoad_Pressed;
            mBoardManager.Boards[3].Buttons[1].Released += btnOnsongLoad_Released;
            // Onsong next
            mBoardManager.Boards[3].Buttons[2].Pressed += btnOnsongNext_Pressed;
            mBoardManager.Boards[3].Buttons[2].Released += btnOnsongNext_Released;

            mBoardManager.LedsOff();

            foreach (ChannelControl ctrl in mChannelControls)
            {
                ctrl.EditKeyzone += ch_EditKeyzone;
                ctrl.SetAudioPluginEngine(mAudioPluginEngine);
            }

            bool asioFail = true;
            // get asio devices
            try
            {
#if NAUDIO_ASIO
                var driver = NAudio.Wave.AsioOut.GetDriverNames();
                cbAsio.Items.AddRange(driver);
#else
                BlueWave.Interop.Asio.InstalledDriver[] driver = BlueWave.Interop.Asio.AsioDriver.InstalledDrivers;
                for (int i = 0; i < driver.Count(); i++)
                {
                    cbAsioDriver.Items.Add(driver[i].Name);
                }
#endif
                if (mSettingsMgr.Settings.AsioDeviceNr != -1)
                {
                    if (cbAsioDriver.Items.Count > mSettingsMgr.Settings.AsioDeviceNr)
                    {
                        cbAsioDriver.SelectedIndex = mSettingsMgr.Settings.AsioDeviceNr;
                    }
                }

                asioFail = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            if (!asioFail)
            {
                // get asio buffsizes
                if (mSettingsMgr.Settings.AsioDeviceNr != -1)
                {
                    try
                    {
#if NAUDIO_ASIO
                        // todo
#else
                        AsioDriver asio = AsioDriver.SelectDriver(AsioDriver.InstalledDrivers[mSettingsMgr.Settings.AsioDeviceNr]);
#endif
                        List<int> buffSizes = new List<int>();
                        foreach (var item in cbAsioBufferSize.Items)
                        {
                            buffSizes.Add(int.Parse(item.ToString()));
                        }
                        for (int i = 0; i < buffSizes.Count; i++)
                        {
#if NAUDIO_ASIO
                            // todo
#else
                            if (buffSizes[i] < asio.BufferSizex.MinSize || buffSizes[i] > asio.BufferSizex.MaxSize)
                            {
                                buffSizes.RemoveAt(i);
                                i--;
                            }
#endif
                        }
#if NAUDIO_ASIO
                        // todo
#else
                        asio.Release();
                        asio = null;
#endif

                        for (int i = 0; i < cbAsioBufferSize.Items.Count; i++)
                        {
                            if ((string)cbAsioBufferSize.Items[i] == mSettingsMgr.Settings.AsioBufferSize.ToString())
                            {
                                cbAsioBufferSize.SelectedIndex = i;
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        asioFail = true;
                    }
                }
            }

            // get midi in devices
            lbMidiInputs.Items.AddRange(mAudioPluginEngine.MidiInPortNames);
            if (mSettingsMgr.Settings.MidiInDeviceNumbers != null)
            {
                for (int i = 0; i < mSettingsMgr.Settings.MidiInDeviceNumbers.Count; i++)
                {
                    int deviceNumber = mSettingsMgr.Settings.MidiInDeviceNumbers[i];
                    string deviceName = mAudioPluginEngine.MidiInPortNames[deviceNumber];

                    if (deviceNumber < mAudioPluginEngine.MidiInPortNames.Count())
                    {                        
                        if (!string.IsNullOrEmpty(deviceName) && lbMidiInputs.Items.Contains(deviceName))
                        {
                            lbMidiInputs.SelectedItems.Add(deviceName);
                        }
                        else
                        {
                            // Remove unknown device number
                            mSettingsMgr.Settings.MidiInDeviceNumbers.Remove(deviceNumber);
                        }
                    }
                }
            }

            // get midi out device
            for (int i = 0; i < mAudioPluginEngine.MidiOutPortNames.Count(); i++)
            {
                cbMidiOut.Items.Add(mAudioPluginEngine.MidiOutPortNames[i]);
            }
            if (mSettingsMgr.Settings.MidiOutDeviceNumber != -1)
            {
                if (cbMidiOut.Items.Count > mSettingsMgr.Settings.MidiOutDeviceNumber)
                {
                    cbMidiOut.SelectedIndex = mSettingsMgr.Settings.MidiOutDeviceNumber;
                }
            }

            if (mSettingsMgr.SelectedPlayList != null)
            {
                foreach (string bank in mSettingsMgr.SelectedPlayList.BankNames)
                {
                    bankControl.AddMenuItem(bank, bankSelect);
                }
            }
            else
            {

            }

           // If no playlist selected; use default 'hidden' playslist
            
            
            //// Force at least 1 bank
            //if(bankControl.MenuItems .Count() == 0)
            //{
            //    string bankName = "Bank 1";
            //    mSettingsMgr.CreateBank(bankName);
            //    bankControl.AddMenuItem(bankName, bankSelect);
            //    bankControl.SelectedIndex = bankControl.MenuItems.Count() - 1;
            //    bankSelect(bankName);               
            //}

            menuControl.AddMenuItem("Save bank", saveBank_Click);
            menuControl.AddMenuItem("Rename bank", renameBank_Click);
            menuControl.AddMenuItem("New bank", newBank_Click);
            menuControl.AddMenuItem("Backup bank", backupBank_Click);
            menuControl.AddMenuItem("Bank selection", bankSelection_Click);            
            menuControl.AddMenuItem("Record", record_Click);
            menuControl.AddMenuItem("Master FX", null);
            menuControl.AddMenuItem("View PDF", menuViewPDF_Click);
            menuControl.AddMenuItem("Set PDF", menuSetPDF_Click);
            menuControl.AddMenuItem("Set OnSong", menuSetOnsong_Click);
            menuControl.AddMenuItem("Set MultiTrack", menuSetMultiTrack_Click);
            menuControl.AddMenuItem("Set ClickTrack", menuSetClickTrack_Click);
            menuControl.AddMenuItem("Settings", menuSettings_Click);
            menuControl.AddMenuItem("Minimize", menuMinimize_Click);
            menuControl.AddMenuItem("Exit", menuExit_Click);

            // Jetzt geht los!
            if (!asioFail) mAudioPluginEngine.Start();
            btnSave.Enabled = false;
            popup.Close();

            pluginTimer.Enabled = true;
            mIgnoreEvents = false;
        }
        
        private void MainForm_Shown(object sender, EventArgs e)
        {
            formStartTimer.Enabled = true;
        }
        
        private void formStartTimer_Tick(object sender, EventArgs e)
        {
            formStartTimer.Enabled = false;
        
            try
            {
                // Select first bank from playlist
                if ((mSettingsMgr.SelectedPlayList != null) && (mSettingsMgr.SelectedPlayList.BankNames.Count > 0))
                {
                    bankSelect(mSettingsMgr.SelectedPlayList.BankNames[0]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void mMetronome_Tick()
        {
            //mPluginManager.PlayMetronomeTick();
            mMetronomePlayer.Play();
        }

        void btnMetronomeOff_Pressed(Button btn)
        {
            if (mMetronomeTimer.IsRunning)
            {
                mMetronomeTimer.Stop();
                mBoardManager.Boards[3].Leds[0].R = LED_RED_OFF;
                mBoardManager.Boards[3].ActivateLeds();
            }
            else
            {
                mMetronomeTimer.Start();
                mBoardManager.Boards[3].Leds[0].R = LED_RED_FULL;
                mBoardManager.Boards[3].ActivateLeds();
            }
        }

        void btnMetronome_Pressed(Button btn)
        {
            if (mMetronomeTimer.IsRunning)
            {
                mMetronomeTimer.Stop();
                mBoardManager.Boards[3].Leds[0].R = LED_RED_OFF;
                mBoardManager.Boards[3].ActivateLeds();
            }
            else
            {
                mMetronomeTimer.Start();
                mBoardManager.Boards[3].Leds[0].R = LED_RED_FULL;
                mBoardManager.Boards[3].ActivateLeds();
            }
        }

        void btnMultiAndClickTrack_Pressed(Button btn)
        {
            if (mMultitrackPlayer.IsReady)
            {
                if (mMultitrackPlayer.PlayBackState == PlaybackState.Playing)
                {
                    mMultitrackPlayer.Stop();
                    mClickTrackPlayer.Stop();
                    mBoardManager.Boards[2].Leds[3].R = LED_RED_DIMMED;
                    mBoardManager.Boards[2].ActivateLeds();
                }
                else
                {
                    if (mClickTrackPlayer.IsReady) mClickTrackPlayer.Play();
                    System.Threading.Thread.Sleep(90);
                    mMultitrackPlayer.Play();
                    mBoardManager.Boards[2].Leds[3].R = LED_RED_FULL;
                    mBoardManager.Boards[2].ActivateLeds();
                }
            }
        }

        // 3 . 1
        void btnOnsongLoad_Pressed(Button btn)
        {
            if (mBankHasOnsong)
            {
                mBoardManager.Boards[3].Leds[1].R = LED_RED_FULL;
                mBoardManager.Boards[3].ActivateLeds();
                this.Invoke(new Action(() =>
                {
                    onSongControl.LoadFile(mAudioPluginEngine.ActiveBank.OnSongFileName);
                    onSongControl.Visible = true;
                }));
            }
        }

        // 3 . 1
        void btnOnsongLoad_Released(Button btn)
        {
            if (mBankHasOnsong)
            {
                mBoardManager.Boards[3].Leds[1].R = LED_RED_DIMMED;
                mBoardManager.Boards[3].ActivateLeds();
            }
        }

        // 3 . 2
        void btnOnsongNext_Pressed(Button btn)
        {
            if (mBankHasOnsong)
            {
                mBoardManager.Boards[3].Leds[2].R = LED_RED_FULL;
                mBoardManager.Boards[3].ActivateLeds();

                this.Invoke(new Action(() =>
                {
                    onSongControl.NextSection();
                }));
            }
        }

        // 3 . 2
        void btnOnsongNext_Released(Button btn)
        {
            if (mBankHasOnsong)
            {
                mBoardManager.Boards[3].Leds[2].R = LED_RED_DIMMED;
                mBoardManager.Boards[3].ActivateLeds();
            }
        }

        void ch_EditKeyzone(object sender, EventArgs e)
        {
            foreach (var plugin in mAudioPluginEngine.PluginChannels.Select(x => x.InstrumentPlugin))
            {
                plugin.CloseEditor();
            }

            keyZoneControl1.Visible = true;
            keyZoneControl1.SetInstrumentPlugin(((ChannelControl)sender).PluginChannel.InstrumentPlugin);
        }

        void menuEnterBtnPressed(Button btn)
        {
            mBoardManager.Boards[2].Leds[0].R = LED_RED_FULL;
            mBoardManager.Boards[2].ActivateLeds();
            // Menu ALT button pressed?
            if (mBoardManager.Boards[3].Buttons[3].IsPressed)
            {
                this.Invoke(new Action(() => menuControl.SelectCurrentMenuItem()));
            }
            else
            {
                this.Invoke(new Action(() => bankControl.SelectCurrentMenuItem()));
            }
        }

        void menuEnterBtnReleased(Button btn)
        {
            mBoardManager.Boards[2].Leds[0].R = LED_RED_OFF;
            mBoardManager.Boards[2].ActivateLeds();
        }

        void menuAltBtnPressed(Button btn)
        {
            mBoardManager.Boards[3].Leds[3].R = LED_RED_FULL;
            mBoardManager.Boards[3].ActivateLeds();
        }

        void menuAltBtnReleased(Button btn)
        {
            mBoardManager.Boards[3].Leds[3].R = LED_RED_OFF;
            mBoardManager.Boards[3].ActivateLeds();
        }

        void menuSaveBtnPressed(Button btn)
        {
            this.BeginInvoke(new Action(() => saveBank_Click(string.Empty)));
        }

        void menuPanicBtnPressed(Button btn)
        {
            mAudioPluginEngine.MidiPanic();
        }

        private void menuEncAbsPosChanged(Encoder encoder, int pos)
        {
            int delta = mMenuEncPrevPos - pos;
            if (delta < 0)
            {
                // Menu ALT button pressed?
                if (mBoardManager.Boards[3].Buttons[3].IsPressed)
                {
                    menuControl.SelectedIndex++;
                }
                // Menu metronome button pressed?
                else if (mBoardManager.Boards[3].Buttons[0].IsPressed)
                {
                    this.BeginInvoke(new Action(() => pbBPMDown_Click(null, null)));
                }
                else
                {
                    bankControl.SelectedIndex++;
                }
            }
            else
            {
                // Menu ALT button pressed?
                if (mBoardManager.Boards[3].Buttons[3].IsPressed)
                {
                    menuControl.SelectedIndex--;
                }
                // Menu metronome button pressed?
                else if (mBoardManager.Boards[3].Buttons[0].IsPressed)
                {
                    this.BeginInvoke(new Action(() => pbBPMUp_Click(null, null)));
                }
                else
                {
                    bankControl.SelectedIndex--;
                }
            }
            mMenuEncPrevPos = pos;
        }

        private void bankSelect(string name)
        {
            PopupForm popup = new PopupForm("Selecting " + name, -1);
            popup.Show();
            Application.DoEvents();
            Bank bank = mSettingsMgr.GetBank(name);
            if (bank == null)
            {
                MessageBox.Show(name + " does not exist", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                bank = mSettingsMgr.GetBank(mSettingsMgr.BankNames[0]);
            }
            if (bank.BPM == 0) bank.BPM = 120;
            lblBPM.Text = bank.BPM.ToString();
            mMetronomeTimer.BPM = bank.BPM;
            mAudioPluginEngine.LoadBank(bank);
            mMultitrackPlayer.Volume = bank.MultiTrackVolume;
            mClickTrackPlayer.Volume = bank.ClickTrackVolume;
            setTrackFiles(bank.MultiTrackFileName, bank.ClickTrackFileName);
            setOnsongLed();
            popup.Close();
        }

        private void saveBank_Click(string name)
        {
            PopupForm popup = new PopupForm("Saving bank", -1);
            popup.Show();
            Application.DoEvents();
            mAudioPluginEngine.ActiveBank.MultiTrackFileName = mMultitrackPlayer.FileName;
            mAudioPluginEngine.ActiveBank.MultiTrackVolume = mMultitrackPlayer.Volume;
            mAudioPluginEngine.ActiveBank.ClickTrackFileName = mClickTrackPlayer.FileName;
            mAudioPluginEngine.ActiveBank.ClickTrackVolume = mClickTrackPlayer.Volume;
            mAudioPluginEngine.SaveBank();
            popup.Close();
        }

        private void backupBank_Click(string name)
        {
            string ext = System.IO.Path.GetExtension(mSettingsMgr.SettingsFileName);
            string fullWithoutExt = mSettingsMgr.SettingsFileName.Replace(ext, string.Empty);

            string newFile = String.Format("{0} {1:d-M-yyyy HH}h{2:mm}{3}", fullWithoutExt, DateTime.Now, DateTime.Now, ext);
            System.IO.File.Copy(mSettingsMgr.SettingsFileName, newFile);
            FileInfo fileInfo = new FileInfo(newFile);
            MessageBox.Show(string.Format("Copied settings ({0:0.##}MB) to {1}", (double)fileInfo.Length / (1024.0 * 1024.0), newFile));
        }

        private void bankSelection_Click(string name)
        {
            pnlBankEditor.Visible = true;
        }

        private void record_Click(string name)
        {
            recordControl1.Visible = true;
        }

        private void menuSettings_Click(string name)
        {
            if (mAudioPluginEngine.PluginOpenedInEditor != null)
            {
                mAudioPluginEngine.PluginOpenedInEditor.CloseEditor();
            }

            pnlSettings.Visible = true;
        }

        private void menuMinimize_Click(string name)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void menuExit_Click(string name)
        {
            this.Close();
        }

        private void menuSetPDF_Click(string name)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "PDF files (*.pdf)|*.pdf";
            ofd.RestoreDirectory = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    mAudioPluginEngine.ActiveBank.PDFFileName = ofd.FileName;
                    saveBank_Click(string.Empty);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void menuSetOnsong_Click(string name)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "OnSong files (*.onsong)|*.onsong";
            ofd.RestoreDirectory = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    mAudioPluginEngine.ActiveBank.OnSongFileName = ofd.FileName;
                    saveBank_Click(string.Empty);
                    setOnsongLed();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void menuSetMultiTrack_Click(string name)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            string allExtensions = "*.wav;*.aiff;*.mp3;*.aac";
            ofd.Filter = String.Format("All Supported Files|{0}|All Files (*.*)|*.*", allExtensions);
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    setTrackFiles(ofd.FileName, mClickTrackPlayer.FileName);
                    saveBank_Click(string.Empty);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void menuSetClickTrack_Click(string name)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            string allExtensions = "*.wav;*.aiff;*.mp3;*.aac";
            ofd.Filter = String.Format("All Supported Files|{0}|All Files (*.*)|*.*", allExtensions);
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    setTrackFiles(mMultitrackPlayer.FileName, ofd.FileName);
                    saveBank_Click(string.Empty);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void menuViewPDF_Click(string name)
        {
            if (!string.IsNullOrEmpty(mAudioPluginEngine.ActiveBank.PDFFileName))
            {
                if (File.Exists(mAudioPluginEngine.ActiveBank.PDFFileName))
                {
                    PDFViewForm form = new PDFViewForm(mAudioPluginEngine.ActiveBank.PDFFileName);
                    form.Show();
                }
            }
        }

        private void renameBank_Click(string name)
        {
            int menuIndex = bankControl.SelectedIndex;
            VSTMenuItem menuItem = bankControl.MenuItems[menuIndex];
            string oldName = menuItem.Name;

            EntryForm form = new EntryForm("Enter name", oldName);
            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string newName = form.Name;
                Bank bank = mSettingsMgr.GetBank(oldName);
                menuItem.RenameMenuItem(newName);
                bank.Name = newName;
                mSettingsMgr.SaveBank(bank);


                mSettingsMgr.SelectedPlayList.BankNames.Remove(oldName);
                mSettingsMgr.SelectedPlayList.BankNames.Add(newName);
                mSettingsMgr.SaveSettings();
            }
        }

        private void newBank_Click(string name)
        {
            EntryForm form = new EntryForm("Enter name", "");
            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                mSettingsMgr.CreateBank(form.Name);
                bankControl.AddMenuItem(form.Name, bankSelect);
                bankControl.SelectedIndex = bankControl.MenuItems.Count() - 1;
                bankSelect(form.Name);
                mAudioPluginEngine.SaveBank();
                mSettingsMgr.SelectedPlayList.BankNames.Add(form.Name);
                mSettingsMgr.SaveSettings();
            }
        }

        void MainForm_MouseWheel(object sender, MouseEventArgs e)
        {
            switch (mMouseCaptureCh)
            {
                case (1):
                    chCtrl0.EncoderChange(e.Delta / 120);
                    break;
                case (2):
                    chCtrl1.EncoderChange(e.Delta / 120);
                    break;
                case (3):
                    chCtrl2.EncoderChange(e.Delta / 120);
                    break;
                case (4):
                    chCtrl3.EncoderChange(e.Delta / 120);
                    break;
                case (5):
                    chCtrl4.EncoderChange(e.Delta / 120);
                    break;
                case (6):
                    chCtrl5.EncoderChange(e.Delta / 120);
                    break;
                case (7):
                    chCtrl6.EncoderChange(e.Delta / 120);
                    break;
                case (8):
                    chCtrl7.EncoderChange(e.Delta / 120);
                    break;
                default:
                    break;
            }
        }

        private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                this.Close();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            mSettingsMgr.SaveSettings();
            MessageBox.Show("Restart for ASIO changes to take effect", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                // Normal button
                case (Keys.Z):
                    mMouseCaptureCh = 1;
                    chCtrl0.ButtonPressed(null);
                    break;
                case (Keys.X):
                    mMouseCaptureCh = 2;
                    chCtrl1.ButtonPressed(null);
                    break;
                case (Keys.C):
                    mMouseCaptureCh = 3;
                    chCtrl2.ButtonPressed(null);
                    break;
                case (Keys.V):
                    mMouseCaptureCh = 4;
                    chCtrl3.ButtonPressed(null);
                    break;
                case (Keys.B):
                    mMouseCaptureCh = 5;
                    chCtrl4.ButtonPressed(null);
                    break;
                case (Keys.N):
                    mMouseCaptureCh = 6;
                    chCtrl5.ButtonPressed(null);
                    break;
                case (Keys.M):
                    mMouseCaptureCh = 7;
                    chCtrl6.ButtonPressed(null);
                    break;
                case (Keys.MButton | Keys.Back | Keys.ShiftKey | Keys.Space | Keys.F17): // ','
                    mMouseCaptureCh = 8;
                    chCtrl7.ButtonPressed(null);
                    break;
                // Encoder button
                case (Keys.A):
                    mMouseCaptureCh = 1;
                    chCtrl0.EncoderPressed(null);
                    break;
                case (Keys.S):
                    mMouseCaptureCh = 2;
                    chCtrl1.EncoderPressed(null);
                    break;
                case (Keys.D):
                    mMouseCaptureCh = 3;
                    chCtrl2.EncoderPressed(null);
                    break;
                case (Keys.F):
                    mMouseCaptureCh = 4;
                    chCtrl3.EncoderPressed(null);
                    break;
                case (Keys.G):
                    mMouseCaptureCh = 5;
                    chCtrl4.EncoderPressed(null);
                    break;
                case (Keys.H):
                    mMouseCaptureCh = 6;
                    chCtrl5.EncoderPressed(null);
                    break;
                case (Keys.J):
                    mMouseCaptureCh = 7;
                    chCtrl6.EncoderPressed(null);
                    break;
                case (Keys.K):
                    mMouseCaptureCh = 8;
                    chCtrl7.EncoderPressed(null);
                    break;
                default:
                    break;
            }
        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case (Keys.Z):
                    mMouseCaptureCh = 1;
                    chCtrl0.ButtonReleased(null);
                    break;
                case (Keys.X):
                    mMouseCaptureCh = 2;
                    chCtrl1.ButtonReleased(null);
                    break;
                case (Keys.C):
                    mMouseCaptureCh = 3;
                    chCtrl2.ButtonReleased(null);
                    break;
                case (Keys.V):
                    mMouseCaptureCh = 4;
                    chCtrl3.ButtonReleased(null);
                    break;
                case (Keys.B):
                    mMouseCaptureCh = 5;
                    chCtrl4.ButtonReleased(null);
                    break;
                case (Keys.N):
                    mMouseCaptureCh = 6;
                    chCtrl5.ButtonReleased(null);
                    break;
                case (Keys.M):
                    mMouseCaptureCh = 7;
                    chCtrl6.ButtonReleased(null);
                    break;
                case (Keys.MButton | Keys.Back | Keys.ShiftKey | Keys.Space | Keys.F17): // ','
                    mMouseCaptureCh = 8;
                    chCtrl7.ButtonReleased(null);
                    break;

                // Encoder button
                case (Keys.A):
                    mMouseCaptureCh = 1;
                    chCtrl0.EncoderReleased(null);
                    break;
                case (Keys.S):
                    mMouseCaptureCh = 2;
                    chCtrl1.EncoderReleased(null);
                    break;
                case (Keys.D):
                    mMouseCaptureCh = 3;
                    chCtrl2.EncoderReleased(null);
                    break;
                case (Keys.F):
                    mMouseCaptureCh = 4;
                    chCtrl3.EncoderReleased(null);
                    break;
                case (Keys.G):
                    mMouseCaptureCh = 5;
                    chCtrl4.EncoderReleased(null);
                    break;
                case (Keys.H):
                    mMouseCaptureCh = 6;
                    chCtrl5.EncoderReleased(null);
                    break;
                case (Keys.J):
                    mMouseCaptureCh = 7;
                    chCtrl6.EncoderReleased(null);
                    break;
                case (Keys.K):
                    mMouseCaptureCh = 8;
                    chCtrl7.EncoderReleased(null);
                    break;

                default:
                    break;
            }
        }

        private void cbAsio_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mIgnoreEvents) return;
            mSettingsMgr.Settings.AsioDeviceNr = cbAsioDriver.SelectedIndex;
            btnSave.Enabled = true;
        }

        private void cbAsioBufferSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mIgnoreEvents) return;
            mSettingsMgr.Settings.AsioBufferSize = int.Parse((string)cbAsioBufferSize.SelectedItem);
            btnSave.Enabled = true;
        }

        private void lbMidiInputs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mIgnoreEvents) return;
            mSettingsMgr.Settings.MidiInDeviceNumbers = lbMidiInputs.SelectedIndices.Cast<int>().ToList();
            btnSave.Enabled = true;
        }

        private void cbMidiOut_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mIgnoreEvents) return;
            mSettingsMgr.Settings.MidiOutDeviceNumber = cbMidiOut.SelectedIndex;
            btnSave.Enabled = true;
        }

        private void pluginTimer_Tick(object sender, EventArgs e)
        {
            if (null != mAudioPluginEngine.PluginOpenedInEditor)
            {
                try
                {
                    if (mAudioPluginEngine.PluginOpenedInEditor.EditorOpenedTimer < 10)
                    {
                        mAudioPluginEngine.PluginOpenedInEditor.EditorOpenedTimer++;
                    }
                    else
                    {
                        mAudioPluginEngine.PluginOpenedInEditor.VstPluginContext.PluginCommandStub.EditorIdle();
                    }
                }
                catch
                {
                    // Do nothing
                }
            }
        }

        private void systemTimer_Tick(object sender, EventArgs e)
        {
            lblTime.Text = string.Format("{0:HH:mm}", DateTime.Now);
            lblTempCPU.Text = string.Format("{0} °C", SystemDiagnostics.CPUTemperature);
            if (mBoardManager.IsConnected)
            {
                lblTempCase.Text = string.Format("{0} °C", mBoardManager.Boards[0].Temperature);
            }
            else
            {
                lblTempCase.Text = string.Empty;
            }
            //lblUsageCPU.Text = string.Format("{0} %", (int)SystemDiagnostics.CurrentCpuUsage);
            lblUsageCPU.Text = string.Format("{0} %", mAudioPluginEngine.MaxCpuLoad);
            lblUsageMemory.Text = string.Format("{0:00} GB", (double)SystemDiagnostics.AvailablePhysicalMemory / 1024.0 / 1024.0 / 1024.0);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => MainForm_FormClosing(sender, e)));
            }
            else
            {
                if (mAudioPluginEngine != null)
                {
                    mAudioPluginEngine.Stop();
                }
                mBoardManager.Close();
            }
        }

        private void pbBPMUp_Click(object sender, EventArgs e)
        {
            mAudioPluginEngine.BPM++;
            mMetronomeTimer.BPM = mAudioPluginEngine.BPM;
            lblBPM.Text = mAudioPluginEngine.BPM.ToString();
        }

        private void pbBPMDown_Click(object sender, EventArgs e)
        {
            mAudioPluginEngine.BPM--;
            mMetronomeTimer.BPM = mAudioPluginEngine.BPM;
            lblBPM.Text = mAudioPluginEngine.BPM.ToString();
        }

        private void lblBPM_DoubleClick(object sender, EventArgs e)
        {
            EntryForm f = new EntryForm("BPM", lblBPM.Text);
            if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                int i;
                if (int.TryParse(f.Name, out i))
                {
                    mAudioPluginEngine.BPM = i;
                    mMetronomeTimer.BPM = mAudioPluginEngine.BPM;
                    lblBPM.Text = mAudioPluginEngine.BPM.ToString();
                }
            }
        }


        private void pnlBankEditor_VisibleChanged(object sender, EventArgs e)
        {
            if (pnlBankEditor.Visible)
            {
                if (mSettingsMgr != null)
                {
                    btnLoadPlayList.Enabled = false;
                    listBoxBanks.Items.Clear();
                    listBoxBanks.Items.AddRange(mSettingsMgr.BankNames);
                    cbPlayList.Items.Clear();
                    cbPlayList.Items.AddRange(mSettingsMgr.Settings.PlayLists.Select(n => n.Name).ToArray());
                    if (cbPlayList.Items.Contains(mSettingsMgr.Settings.SelectedPlayListName))
                    {
                        cbPlayList.SelectedItem = mSettingsMgr.Settings.SelectedPlayListName;
                    }
                }
            }
        }

        private void knobMasterPan_ValueChanged(object sender, EventArgs e)
        {
            mSettingsMgr.Settings.MasterPan = (float)knobMasterPan.Value / 100.0f;
            mAudioPluginEngine.MasterPan = mSettingsMgr.Settings.MasterPan;
        }

        private void pbMasterFxInsert_Click(object sender, EventArgs e)
        {
            // todo: [JBK]
            //ShowEffectInserts()

            //FxInsertsForm form = new FxInsertsForm(mAudioPluginEngine, mAudioPluginEngine.MasterEffectPluginChannel.EffectPlugins);
            //form.ShowDialog();
        }

        private void cbMultiTrackOutput_SelectedIndexChanged(object sender, EventArgs e)
        {
            string deviceName = cbMultiTrackOutput.SelectedItem.ToString();
            mMultitrackPlayer.DeviceName = deviceName;
            mSettingsMgr.Settings.MultiTrackDevice = deviceName;
            btnSave.Enabled = true;
        }

        private void cbMP3PlayerOutput_SelectedIndexChanged(object sender, EventArgs e)
        {
            string deviceName = cbMP3PlayerOutput.SelectedItem.ToString();
            mMP3Player.DeviceName = deviceName;
            mSettingsMgr.Settings.MP3PlayerDevice = deviceName;
            btnSave.Enabled = true;
        }

        private void cbMetronomeOutput_SelectedIndexChanged(object sender, EventArgs e)
        {
            string deviceName = cbClickTrackOutput.SelectedItem.ToString();
            mClickTrackPlayer.DeviceName = deviceName;
            mSettingsMgr.Settings.MetronomeAndClickTrackDevice = deviceName;
            btnSave.Enabled = true;
        }

        private void btnMetronomeClickFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                mSettingsMgr.Settings.MetronomeSamplePath = fbd.SelectedPath;
                mSettingsMgr.SaveSettings();
                setMetronomeSamplePath();
            }
            btnSave.Enabled = true;
        }

        Dictionary<string, string> mMetronomeFiles = new Dictionary<string, string>();

        private void setMetronomeSamplePath()
        {
            string path = mSettingsMgr.Settings.MetronomeSamplePath;
            tbMetronomeSamplePath.Text = path;
            string fullName = mSettingsMgr.Settings.MetronomeSampleFile;
            if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(path);
                mMetronomeFiles.Clear();
                foreach (FileInfo fInfo in dirInfo.GetFiles("*.wav"))
                {
                    mMetronomeFiles.Add(fInfo.Name, fInfo.FullName);
                }
                listbMetronomeClicks.Items.Clear();
                listbMetronomeClicks.Items.AddRange(mMetronomeFiles.Keys.ToArray());

                if (!string.IsNullOrEmpty(fullName))
                {
                    string name = System.IO.Path.GetFileName(fullName);
                    if (mMetronomeFiles.Keys.Contains(name))
                    {
                        listbMetronomeClicks.SelectedItem = name;
                    }
                }
                else
                {
                    listbMetronomeClicks.SelectedIndex = 0;
                }
                listbMetronomeClicks.Enabled = true;
            }
            else
            {
                listbMetronomeClicks.Enabled = false;
            }


        }

        private void listbMetronomeClicks_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mMetronomeFiles.Count > 0)
            {
                string name = listbMetronomeClicks.SelectedItem.ToString();
                string fullName = mMetronomeFiles[name];
                if (mSettingsMgr.Settings.MetronomeSampleFile != fullName)
                {
                    mSettingsMgr.Settings.MetronomeSampleFile = fullName;
                    mSettingsMgr.SaveSettings();
                }
                mAudioPluginEngine.SetMetronomeSample(fullName);
            }
        }

        private void setTrackFiles(string multiTrackfileName, string clickTrackFile)
        {
            if (File.Exists(multiTrackfileName))
            {
                mMultitrackPlayer.FileName = multiTrackfileName;
                if (mMultitrackPlayer.IsReady)
                {
                    mBoardManager.Boards[2].Leds[3].R = LED_RED_DIMMED;
                }
                else
                {
                    mBoardManager.Boards[2].Leds[3].R = LED_RED_OFF;
                }
                mBoardManager.Boards[2].ActivateLeds();
            }

            if (File.Exists(clickTrackFile))
            {
                mClickTrackPlayer.FileName = clickTrackFile;
            }
        }

        private void setOnsongLed()
        {
            string fileName = mAudioPluginEngine.ActiveBank.OnSongFileName;
            if (!string.IsNullOrEmpty(fileName) && File.Exists(fileName))
            {
                mBankHasOnsong = true;
                mBoardManager.Boards[3].Leds[1].R = LED_RED_DIMMED;
                mBoardManager.Boards[3].Leds[2].R = LED_RED_DIMMED;
                mBoardManager.Boards[3].ActivateLeds();                
            }
            else
            {
                mBankHasOnsong = false;
                mBoardManager.Boards[3].Leds[1].R = LED_RED_OFF;
                mBoardManager.Boards[3].Leds[2].R = LED_RED_OFF;
                mBoardManager.Boards[3].ActivateLeds();
            }
        }

        private void btnVstFolderAdd_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                mSettingsMgr.Settings.VSTPluginFolders.Add(fbd.SelectedPath);
                listbVstFolders.Items.Add(fbd.SelectedPath);
                btnSave.Enabled = true;
            }
        }

        private void btnVstFolderRemove_Click(object sender, EventArgs e)
        {
            if (listbVstFolders.SelectedItem != null)
            {
                string item = listbVstFolders.SelectedItem.ToString();
                if (!string.IsNullOrEmpty(item) && mSettingsMgr.Settings.VSTPluginFolders.Contains(item))
                {
                    mSettingsMgr.Settings.VSTPluginFolders.Remove(item);
                    listbVstFolders.Items.Remove(item);
                    btnSave.Enabled = true;
                }
            }
            mSettingsMgr.SaveSettings();
        }

        private void btnAddToPlayList_Click(object sender, EventArgs e)
        {
            foreach (string bankName in listBoxBanks.SelectedItems)
            {
                if (!dragDropPlayList.Items.Contains(bankName))
                {
                    dragDropPlayList.Items.Add(bankName);
                    if (mSettingsMgr.SelectedPlayList != null)
                    {
                        mSettingsMgr.SelectedPlayList.BankNames.Remove(bankName);
                    }
                }
            }
            mSettingsMgr.SaveSettings();
        }

        private void btnRemoveFromPlayList_Click(object sender, EventArgs e)
        {
            var bankNames = dragDropPlayList.SelectedItems.Cast<string>().ToArray();
            for (int i = 0; i < bankNames.Count(); i++)
            {
                dragDropPlayList.Items.Remove(bankNames[i]);
                var playlist = mSettingsMgr.Settings.PlayLists.FirstOrDefault(n => n.Name == cbPlayList.SelectedItem.ToString());
                playlist.BankNames.Remove(bankNames[i]);
            }
        }

        private void cbPlayList_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnLoadPlayList.Enabled = cbPlayList.SelectedIndex >= 0;
            btnRemovePlayList.Enabled = cbPlayList.SelectedIndex >= 0;
            btnCopyPlayList.Enabled = cbPlayList.SelectedIndex >= 0;
            dragDropPlayList.Items.Clear();
            var playlist = mSettingsMgr.Settings.PlayLists.First(x => x.Name == cbPlayList.SelectedItem.ToString());            
            dragDropPlayList.Items.AddRange(playlist.BankNames.ToArray());
        }

        private void btnAddPlayList_Click(object sender, EventArgs e)
        {
            EntryForm form = new EntryForm("Enter playlist name", string.Empty);
            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (mSettingsMgr.Settings.PlayLists.Exists(n => n.Name == form.Name))
                {
                    MessageBox.Show(form.Name + " already exists!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    PlayList playlist = new PlayList();
                    playlist.Name = form.Name;
                    mSettingsMgr.Settings.PlayLists.Add(playlist);
                    cbPlayList.Items.Add(form.Name);
                    cbPlayList.SelectedItem = form.Name;
                    mSettingsMgr.SaveSettings();
                }
            }
        }

        private void btnRemovePlayList_Click(object sender, EventArgs e)
        {
            mSettingsMgr.Settings.PlayLists.Remove(mSettingsMgr.Settings.PlayLists[cbPlayList.SelectedIndex]);
            mSettingsMgr.Settings.SelectedPlayListName = string.Empty;
            cbPlayList.Items.RemoveAt(cbPlayList.SelectedIndex);
            mSettingsMgr.SaveSettings();
        }

        private void btnCopyPlayList_Click(object sender, EventArgs e)
        {
            EntryForm form = new EntryForm("Enter playlist name", mSettingsMgr.SelectedPlayList.Name);
            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (mSettingsMgr.Settings.PlayLists.Exists(n => n.Name == form.Name))
                {
                    MessageBox.Show(form.Name + " already exists!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    PlayList playlist = new PlayList();
                    playlist.Name = form.Name;
                    string[] bankNames = new string[mSettingsMgr.SelectedPlayList.BankNames.Count];
                    mSettingsMgr.SelectedPlayList.BankNames.CopyTo(bankNames);
                    playlist.BankNames.AddRange(bankNames);
                    mSettingsMgr.Settings.PlayLists.Add(playlist);
                    cbPlayList.Items.Add(form.Name);
                    cbPlayList.SelectedItem = form.Name;
                    mSettingsMgr.SaveSettings();
                }
            }
        }

        private void btnLoadPlaylist_Click(object sender, EventArgs e)
        {
            if (mSettingsMgr.Settings.SelectedPlayListName != cbPlayList.SelectedItem.ToString())
            {
                mSettingsMgr.Settings.SelectedPlayListName = cbPlayList.SelectedItem.ToString();
                mSettingsMgr.SaveSettings();
            }

            var list1 = mSettingsMgr.SelectedPlayList.BankNames;
            var list2 = dragDropPlayList.Items.Cast<string>().ToArray();
            bool equal = true;
            if (list1.Count() != list2.Count())
            {
                equal = false;
            }
            else
            {
                for (int i = 0; i < list1.Count(); i++)
                {
                    if (!String.Equals(list1[i], list2[i]))
                    {
                        equal = false;
                        break;
                    }
                }
            }
            if(!equal)
            {
                mSettingsMgr.SelectedPlayList.BankNames .Clear ();
                mSettingsMgr .SelectedPlayList .BankNames .AddRange (dragDropPlayList.Items.Cast<string>());
                mSettingsMgr.SaveSettings();
            }

            // Unload active bank manually
            mAudioPluginEngine.UnloadBank();
            bankControl.ClearMenuItems();
            foreach (string bank in mSettingsMgr.SelectedPlayList.BankNames)
            {
                bankControl.AddMenuItem(bank, bankSelect);
            }

            if (mSettingsMgr.SelectedPlayList.BankNames.Count > 0)
            {
                // Select first bank
                bankSelect(mSettingsMgr.SelectedPlayList.BankNames[0]);
            }
        }

        private void listBoxBanks_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnAddToPlayList.Enabled = listBoxBanks.SelectedItems.Count > 0;
        }

        private void dragDropPlayList_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnRemoveFromPlayList.Enabled = dragDropPlayList.SelectedItems.Count > 0;
        }   
    }
}
