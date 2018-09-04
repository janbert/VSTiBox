using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSTiBox.Common
{
    class VolumeLevelConverter
    {
        public const int VOLUME_STEPS = 127;
        public const float VOLUME_STEPSIZE_DECIBEL = 0.5f;
        public const float VOLUME_MAX_DECIBEL = 10.0f;
                
        float[] mVolumes; 

        private VolumeLevelConverter()
        {
            List<float>  volumes = new List<float>();

            int steps = VOLUME_STEPS+1;
            float db = VOLUME_MAX_DECIBEL - steps * VOLUME_STEPSIZE_DECIBEL + VOLUME_STEPSIZE_DECIBEL;
            for (int i = 0; i < steps; i++)
            {
                volumes.Add((float)Math.Pow(10.0, (double)db / 20.0));
                db += VOLUME_STEPSIZE_DECIBEL;
            }
            volumes[0] = 0.0f;
            mVolumes = volumes.ToArray();
        }

        public static float GetVolumeNormalized(int step)
        {
            return Instance.mVolumes[step];
        }

        public static int GetVolumeStep(float volumeNormalized)
        {
            return Instance.mVolumes.ToList().IndexOf(volumeNormalized);
        }

        public static float GetVolumeDecibel(int step)
        {
            return (step - (VOLUME_STEPS + 1)) * VOLUME_STEPSIZE_DECIBEL + 10 + VOLUME_STEPSIZE_DECIBEL;
        }

        /// <summary>
        /// Singleton pattern implementation to get Class instance
        /// </summary>
        public static VolumeLevelConverter Instance
        {
            get
            {
                return Nested.instance;
            }
        }

        /// <summary>
        /// Nested class for singleton implementation
        /// </summary>
        private class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }

            internal static readonly VolumeLevelConverter instance = new VolumeLevelConverter();
        }
    }
}
