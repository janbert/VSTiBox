using NAudio.Lame;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace VSTiBox
{
    public class AsioMP3Recorder : IDisposable
    {
        private LameMP3FileWriter mMp3Writer;
        private int mBlockSize;
        private FileStream mFS;
        private Thread mReadThread;
        private ByteArrayRingBuffer mRingBuffer;
        private ManualResetEvent mDataReady;
        private ManualResetEvent mExitEvent;
        public event EventHandler Closing;

        public string FileName { get; private set; }

        public AsioMP3Recorder(string path, int rate, int bits, int channels)
        {
            FileName = path;

            mExitEvent = new ManualResetEvent(false);
            mDataReady = new ManualResetEvent(false);

            WaveFormat mFormat = new WaveFormat(rate, bits, channels);
            mFS = new FileStream(path, FileMode.Create, FileAccess.Write);
            mMp3Writer = new LameMP3FileWriter(mFS, mFormat, 192);
            mBlockSize = 8192;  //  mMp3Writer.OptimalBufferSize;
            mRingBuffer = new ByteArrayRingBuffer(mBlockSize * 4);   // Fit 4x write block size in buffer
            mReadThread = new Thread(readThread);
            mReadThread.Priority = ThreadPriority.BelowNormal;
            mReadThread.IsBackground = true;
            mReadThread.Start();
        }
        
        void IDisposable.Dispose()
        {
            if (mMp3Writer != null)
            {
                mMp3Writer.Dispose();
            }
            if (mDataReady != null)
            {
                mDataReady.Dispose();
            }
            if (mExitEvent != null)
            {
                mExitEvent.Dispose();
            }
            if (mFS != null)
            {
                mFS.Dispose();
            }
        }

        private void readThread()
        {
            byte[] data;

            while (true)
            {
                // Check if any data in ring buffer
                lock (mRingBuffer)
                {
                    if (mRingBuffer.Count < mBlockSize)
                    {
                        mDataReady.Reset();
                    }
                }

                // Wait untill data is received 
                int i = WaitHandle.WaitAny(new WaitHandle[] { mDataReady, mExitEvent });
                int count = mRingBuffer.Count;
                if (i == 1 && count < mBlockSize)
                {
                    // Write remaining data in ringbuffer
                    if (mRingBuffer.Count > 0)
                    {
                        mMp3Writer.Write(mRingBuffer.Get(count), 0, count);
                    }

                    mMp3Writer.Close();
                    mFS.Close();

                    // Exit!
                    return;
                }

                // Copy data
                lock (mRingBuffer)
                {
                    // Copy the block data 
                    data = mRingBuffer.Get(mBlockSize);
                }

                // Handle data
                mMp3Writer.Write(data, 0, mBlockSize);
            }
        }

        private byte[] sampleToBytes(float sample)
        {
            // clip
            if (sample > 1.0f)
                sample = 1.0f;
            if (sample < -1.0f)
                sample = -1.0f;
            Int16 i16Val = (short)(sample * 32767);
            return BitConverter.GetBytes(i16Val);
        }


        public bool WriteSamples(float[] left, float[] right, int size)
        {
            // Convert stereo samples to 2xint16
            int arrSize = size * 2 * 2;
            var data = new byte[arrSize];
            int dst = 0;
            for (int i = 0; i < size; i++)
            {
                sampleToBytes(left[i]).CopyTo(data, dst);
                dst += 2;
                sampleToBytes(right[i]).CopyTo(data, dst);
                dst += 2;
            }

            lock (mRingBuffer)
            {
                if (!mRingBuffer.Put(data, arrSize))
                {
                    return false;
                }
                if (mRingBuffer.Count >= mBlockSize)
                {
                    mDataReady.Set();
                }
            }
            return true;
        }

        /// <summary>
        /// Non-blocking close
        /// </summary>
        public void Close()
        {
            if (Closing != null)
            {
                Closing(this, null);
            }
            mExitEvent.Set();
        }
    }
}