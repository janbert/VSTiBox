using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using VSTiBox.Common;

namespace VSTiBox.AudioPlayback
{
    public partial class AudioPlaybackPanel : UserControl
    {
        private WasapiPlayer mWasapiPlayer;

        public AudioPlaybackPanel()
        {
            InitializeComponent();
            labelFileName.Text = string.Empty;
        }

        public void SetWasapiPlayer(WasapiPlayer player)
        {
            mWasapiPlayer = player;
            mWasapiPlayer.PlaybackStopped += OnPlaybackStopped;
            mWasapiPlayer.FileNameUpdated += mWasapiPlayer_FileNameUpdated;

            mWasapiPlayer.PreVolumeMeter += mWasapiPlayer_PreVolumeMeter;
            mWasapiPlayer.PostVolumeMeter += mWasapiPlayer_PostVolumeMeter;
            volumeSlider1.Volume = mWasapiPlayer.Volume;
        }

        void mWasapiPlayer_FileNameUpdated(object sender, EventArgs e)
        {
            try
            {
                labelTotalTime.Text = String.Format("{0:00}:{1:00}",
                    (int)mWasapiPlayer.TotalTime.TotalMinutes,
                   mWasapiPlayer.TotalTime.Seconds);
                labelFileName.Text = mWasapiPlayer.FileName;
                volumeSlider1.Volume = mWasapiPlayer.Volume;
            }
            catch 
            { 
                // Do nothing
            }
        }

        private void OnButtonPlayClick(object sender, EventArgs e)
        {
            mWasapiPlayer.Play();
        }

        private void mWasapiPlayer_PreVolumeMeter(object sender, StreamVolumeEventArgs e)
        {
            // we know it is stereo
            waveformPainter1.AddMax(e.MaxSampleValues[0]);
            waveformPainter2.AddMax(e.MaxSampleValues[1]);
        }

        private  void mWasapiPlayer_PostVolumeMeter(object sender, StreamVolumeEventArgs e)
        {
            // we know it is stereo
            volumeMeter1.Amplitude = e.MaxSampleValues[0];
            volumeMeter2.Amplitude = e.MaxSampleValues[1];
        }

        private void OnPlaybackStopped(object sender, StoppedEventArgs e)
        {
            if (e.Exception != null)
            {
                MessageBox.Show(e.Exception.Message, "Playback Device Error");
            }
        }

        private void OnButtonPauseClick(object sender, EventArgs e)
        {
            mWasapiPlayer.Pauze();
        }

        private void OnVolumeSliderChanged(object sender, EventArgs e)
        {
            mWasapiPlayer.Volume = volumeSlider1.Volume;
        }

        private void OnButtonStopClick(object sender, EventArgs e)
        {
            mWasapiPlayer.Stop();
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            if (mWasapiPlayer == null) return;
            try
            {
                TimeSpan currentTime = mWasapiPlayer.CurrentTime;
                if (currentTime.TotalSeconds == 0)
                {
                    trackBarPosition.Value = 0;
                }
                else
                {
                    trackBarPosition.Value = Math.Min(trackBarPosition.Maximum, (int)(100 * currentTime.TotalSeconds / mWasapiPlayer.TotalTime.TotalSeconds));
                }
                labelCurrentTime.Text = String.Format("{0:00}:{1:00}", (int)currentTime.TotalMinutes, currentTime.Seconds);
            }
            catch
            {
                // Do nothing
            }
        }

        private void trackBarPosition_Scroll(object sender, EventArgs e)
        {
            TimeSpan t = TimeSpan.FromSeconds(mWasapiPlayer.TotalTime.TotalSeconds * trackBarPosition.Value / 100.0);
            mWasapiPlayer.CurrentTime = t;
        }

        private void OnOpenFileClick(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            string allExtensions = "*.wav;*.aiff;*.mp3;*.aac";
            openFileDialog.Filter = String.Format("All Supported Files|{0}|All Files (*.*)|*.*", allExtensions);
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                mWasapiPlayer.FileName = openFileDialog.FileName;
            }
        }
    }
}

