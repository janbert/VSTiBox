using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using VSTiBox.Common;
using NAudio.Wave.SampleProviders;
using NAudio.Wave;

namespace VSTiBox.Controls
{
    public partial class RecordControl : UserControl
    {
        private const string RECORDINGSFOLDERNAME = "Recordings";

        private AudioPluginEngine mPluginManager;
        private AsioMP3Recorder mMP3Recorder;
        private WasapiPlayer mMP3Player;

        private string RecordingsFolder
        {
            get
            {
                return Path.Combine(Application.StartupPath, RECORDINGSFOLDERNAME);
            }
        }

        public void SetPluginManager(AudioPluginEngine pluginManager)
        {
            mPluginManager = pluginManager;
        }
        
        public void SetWasapiPlayer(WasapiPlayer player)
        {
            mMP3Player = player;
            mMP3Player.PlaybackStopped += OnPlaybackStopped;
            mMP3Player.PostVolumeMeter += mWasapiPlayer_PostVolumeMeter;
            knobVolume.Value = (int)(mMP3Player.Volume * 100.0f);
        }

        public RecordControl()
        {
            InitializeComponent();

            if (LicenseManager.UsageMode == LicenseUsageMode.Runtime )
            {

                if (!Directory.Exists(RecordingsFolder))
                {
                    Directory.CreateDirectory(RecordingsFolder);
                }
                else
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(RecordingsFolder);
                    List<string> items = new List<string>();
                    foreach (FileInfo fileInfo in dirInfo.GetFiles())
                    {

                        if (fileInfo.Extension == ".mp3")
                        {
                            items.Add(fileInfo.Name);
                        }

                    }
                    scrollListRecordings.Items = items.ToArray();
                    if (items.Count() > 0)
                    {
                        pbPlay.Enabled = true;
                    }
                }
            }
        }
        
        private void pbRecord_Click(object sender, EventArgs e)
        {
            if (mMP3Recorder == null)
            {
                // Start recording
                pbRecord.Image = Properties.Resources.stop;
                string fileName = getRecFileName();
                lblRec.Text = Path.GetFileName(fileName);
                mMP3Recorder = new AsioMP3Recorder(fileName, 44100, 16, 2);
                mPluginManager.AddMp3Recorder(mMP3Recorder);
            }
            else
            {
                // Stop recording
                pbRecord.Image = Properties.Resources.record;
                mMP3Recorder.Close();
                // Add recording to  
                List<string> items = scrollListRecordings.Items.ToList ();
                items.Add(Path.GetFileName(mMP3Recorder.FileName));
                scrollListRecordings.Items = items.ToArray();
                mMP3Recorder = null;
            }
        }

        private string getRecFileName()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(RecordingsFolder);

            int recNr = 0;
            while (true)
            {
                string name = string.Format("rec_{0:000}.mp3", recNr);
                if (dirInfo.GetFiles().FirstOrDefault(x => x.Name == name) == null)
                {
                    return Path.Combine(RecordingsFolder, name);
                }
                recNr++;
            }
        }

        private void pbPlay_Click(object sender, EventArgs e)
        {
            if (mMP3Player.PlayBackState == PlaybackState.Stopped )
            {
                int index = scrollListRecordings.SelectedIndex;
                string name = scrollListRecordings.Items[index];
                string fileName = Path.Combine(RecordingsFolder, name);

                pbPlay.Image = Properties.Resources.pause; 
                pbStop.Enabled = true;
                mMP3Player.FileName = fileName;
                mMP3Player.Play();
                trackBarPosition.Enabled = true;
                timerPlay.Enabled = true;
            }
            else
            {
                if (mMP3Player.PlayBackState== PlaybackState.Playing )
                {
                    // Pause
                    pbPlay.Image = Properties.Resources.play;  
                    mMP3Player.Pauze ();
                    timerPlay.Enabled = false;
                }
                else
                {
                    // Resume
                    pbPlay.Image = Properties.Resources.pause;
                    mMP3Player.Play();
                    timerPlay.Enabled = true;
                }
            }    
        }

        private void pbStop_Click(object sender, EventArgs e)
        {
            mMP3Player.Stop();
            pbPlay.Image = Properties.Resources.play;
            trackBarPosition.Enabled = false;
            timerPlay.Enabled = false;
            pbStop.Enabled = false;            
        }

        private void scrollListRecordings_ValueChanged(object sender, EventArgs e)
        {
            pbStop_Click(null, null);
            pbPlay_Click(null, null);
        }

        private void trackBarPlay_Scroll(object sender, EventArgs e)
        {
            TimeSpan t = TimeSpan.FromSeconds(mMP3Player.TotalTime.TotalSeconds * trackBarPosition.Value / 100.0);
            mMP3Player.CurrentTime = t;
        }

        private void timerPlay_Tick(object sender, EventArgs e)
        {
            if (mMP3Player == null) return;
            try
            {
                TimeSpan currentTime = mMP3Player.CurrentTime;
                if (currentTime.TotalSeconds == 0)
                {
                    trackBarPosition.Value = 0;
                }
                else
                {
                    trackBarPosition.Value = Math.Min(trackBarPosition.Maximum, (int)(100 * currentTime.TotalSeconds / mMP3Player.TotalTime.TotalSeconds));
                }
                lblTime.Text = String.Format("{0:00}:{1:00}", (int)currentTime.TotalMinutes, currentTime.Seconds);
            }
            catch
            {
                // Do nothing
            }
        }

        private void OnPlaybackStopped(object sender, StoppedEventArgs e)
        {
            if (e.Exception != null)
            {
                MessageBox.Show(e.Exception.Message, "Playback Device Error");
            }
            else
            {
                this.Invoke(new Action(() =>
                {
                    if (cbLoop.Checked)
                    {
                        mMP3Player.Play();
                    }
                    else
                    {
                        pbPlay.Image = Properties.Resources.play;
                    }
                }));
            }
        }

        private void mWasapiPlayer_PostVolumeMeter(object sender, StreamVolumeEventArgs e)
        {
            // we know it is stereo
            volumeMeter1.Amplitude = e.MaxSampleValues[0];
            volumeMeter2.Amplitude = e.MaxSampleValues[1];
        } 

        private void knobVolume_ValueChanged(object sender, EventArgs e)
        {
            mMP3Player.Volume = (float)knobVolume.Value / 100.0f;
            knobVolume.Text = String.Format("{0:0}dB", 20 * Math.Log10(mMP3Player.Volume));
        }
    }
}
