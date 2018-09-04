using NAudio.CoreAudioApi;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VSTiBox.Common
{
    public class WasapiPlayer
    {
        private AudioFileReader audioFileReader;
        private SampleChannel sampleChannel;

        private Dictionary<string, MMDevice> mDeviceDict = new Dictionary<string, MMDevice>();
        private string mDeviceName;
        private MMDevice mMMDevice;
        IWavePlayer waveOut;
        string fileName;
        public event EventHandler<StoppedEventArgs> PlaybackStopped;

        public bool IsReady{ get ;private set;}
       
        public TimeSpan TotalTime
        {
            get
            {
                return audioFileReader.TotalTime ;
            }
        }

        public event EventHandler FileNameUpdated;

        public string FileName
        {
            set
            {
                if (fileName != value)
                {
                    fileName = value;
                    IsReady = false;
                    if(!string.IsNullOrEmpty(fileName) && System.IO.File.Exists (fileName))
                    {
                        IsReady = createWaveOut();
                    }
                    if (FileNameUpdated != null)
                    {
                        FileNameUpdated(this, null);
                    }
                }
            }
            get
            {
                return fileName!=null ? fileName : string.Empty;
            }
        }

        private void disposeWaveOut()
        {
            if (waveOut != null)
            {
                if (waveOut.PlaybackState == PlaybackState.Playing || waveOut.PlaybackState == PlaybackState.Paused)
                {
                    waveOut.Stop();
                }
            }
            if (audioFileReader != null)
            {
                // this one really closes the file and ACM conversion
                audioFileReader.Dispose();
                audioFileReader = null;
            }
            if (waveOut != null)
            {
                waveOut.Dispose();
                waveOut = null;
            }
        }

        private bool createWaveOut()
        {
            disposeWaveOut();
            
            ISampleProvider sampleProvider = null;
            try
            {
                sampleProvider = createInputStream(fileName);
            }
            catch (Exception createException)
            {
                MessageBox.Show(String.Format("{0}", createException.Message), "Error Loading File");
                return false;
            }

            try
            {
                const int LATENCY_MS = 25;
                waveOut = new WasapiOut(mMMDevice, AudioClientShareMode.Shared, true, LATENCY_MS);
                waveOut.PlaybackStopped += OnPlaybackStopped; 
                waveOut.Init(sampleProvider);
            }
            catch (Exception initException)
            {
                MessageBox.Show(String.Format("{0}", initException.Message), "Error Initializing Output");
                return false;
            }
            
            return true;
        }

        void OnPlaybackStopped(object sender, StoppedEventArgs e)
        {
            if (audioFileReader != null)
            {
                audioFileReader.Position = 0;
            }
                        
            if (PlaybackStopped != null)
            {
                PlaybackStopped(this, e);
            }
        }


        public PlaybackState PlayBackState
        {
            get
            {
                if (waveOut == null)
                {
                    return PlaybackState.Stopped;
                }
                return waveOut.PlaybackState;
            }
        }

        public void Play()
        {
            if (waveOut != null)
            {
                if (waveOut.PlaybackState == PlaybackState.Playing)
                {
                    return;
                }
                waveOut.Play();
            }
        }


        public void Stop()
        {
            if (waveOut != null)
            {
                waveOut.Stop();
            }
        }

        public void Pauze()
        {
            if (waveOut != null)
            {
                if (waveOut.PlaybackState == PlaybackState.Playing)
                {
                    waveOut.Pause();
                }
            }
        }
         
        public TimeSpan CurrentTime
        {
            get
            {
                if (waveOut != null && audioFileReader != null && waveOut.PlaybackState != PlaybackState.Stopped)
                {
                    return audioFileReader.CurrentTime;
                }
                else
                {
                    return TimeSpan.Zero;
                }
            }
            set
            {
                if (waveOut != null)
                {
                    audioFileReader.CurrentTime =value;
                }
            }
        }

        private float volume;
        public float Volume 
        {
            get
            {
                return volume;
            }
            set
            {
                volume = value;
                if (sampleChannel != null)
                {
                    sampleChannel.Volume = value;
                }
            }
        }

        private ISampleProvider createInputStream(string fileName)
        {
            this.audioFileReader = new AudioFileReader(fileName);
            sampleChannel = new SampleChannel(audioFileReader, true);
            sampleChannel.PreVolumeMeter += OnPreVolumeMeter;
            sampleChannel.Volume = Volume;
            
            var postVolumeMeter = new MeteringSampleProvider(sampleChannel);
            postVolumeMeter.StreamVolume += OnPostVolumeMeter;
            return postVolumeMeter;
        }

        public event EventHandler<StreamVolumeEventArgs> PostVolumeMeter;
        void OnPostVolumeMeter(object sender, StreamVolumeEventArgs e)
        {
            if (PostVolumeMeter != null)
            {
                PostVolumeMeter(this, e);
            }
        }

        public event EventHandler<StreamVolumeEventArgs> PreVolumeMeter;
        void OnPreVolumeMeter(object sender, StreamVolumeEventArgs e)
        {
            if (PreVolumeMeter != null)
            {
                PreVolumeMeter(this, e);
            }
        }


        public string[] AvailableDeviceNames
        {
            get
            {
                return mDeviceDict.Keys.ToArray();
            }
        }

        public string DeviceName
        {
            get
            {
                return mDeviceName;
            }
            set
            {
                if (mDeviceDict.ContainsKey(value))
                {
                    // Stop & cleanup in case it was running
                    disposeWaveOut();
                    OnPlaybackStopped(this, new StoppedEventArgs());
                    mDeviceName = value;
                    mMMDevice = mDeviceDict[value];
                }
            }
        }

        public WasapiPlayer()
        {
            var enumerator = new MMDeviceEnumerator();
            var endPoints = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);

            foreach (var endPoint in endPoints)
            {
                string description = string.Format("{0} ({1})", endPoint.FriendlyName, endPoint.DeviceFriendlyName);
                mDeviceDict.Add(description, endPoint);
            }

            IsReady = false;
        }
        
        ~WasapiPlayer()
        {
            disposeWaveOut();
        }

    }
}