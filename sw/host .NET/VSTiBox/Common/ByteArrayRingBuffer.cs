using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Diagnostics;

namespace VSTiBox
{
    class ByteArrayRingBuffer
    {
        private byte[] mBuff;
        private int mTail = 0;
        private int mHead = 0;

        private int mCapacityPlusOne; 
        public int Capacity
        {
            get
            {
                return mCapacityPlusOne - 1;
            }
        }

        public ByteArrayRingBuffer(int capacity)
        {
            mCapacityPlusOne = capacity + 1 ;
            // Byte array actually contains one byte more than capacity, 
            // because tail can never filled up to head
            mBuff = new byte[mCapacityPlusOne];
        }

        /// <summary>
        /// Number of bytes in ringbuffer
        /// </summary>
        public int Count
        {
            get
            {
                if (mTail == mHead)
                {
                    return 0;
                }
                else if(mTail > mHead)
                {
                    return mTail - mHead;
                }
                else
                {
                    return mCapacityPlusOne - mHead + mTail;
                }
            }
        }

        /// <summary>
        /// Put bytes in the ringbuffer
        /// </summary>
        /// <param name="data">Bytes to put in</param>
        /// <param name="len">Number of bytes</param>
        /// <returns></returns>
        public Boolean Put(byte[] data, int len)
        {
            // Space enough?
            if (mCapacityPlusOne - 1 - Count < len)
            {
                return false;
            }

            lock (mBuff)
            {
                if (mTail + len > mCapacityPlusOne)
                {
                    // Split in two parts
                    int split = mCapacityPlusOne - mTail;
                    // First copy part to the end of the ringbuffer
                    Buffer.BlockCopy(data, 0, mBuff, mTail, split);
                    // Wrap around
                    Buffer.BlockCopy(data, split, mBuff, 0, len - split);
                    // Set new tail
                    mTail = len - split;
                }
                else
                {
                    // Only one copy action
                    Buffer.BlockCopy(data, 0, mBuff, mTail, len);
                    // Set new tail
                    mTail = mTail + len;
                }
            }
            return true;
        }

        /// <summary>
        /// Get a number of bytes from the ringbuffer
        /// </summary>
        /// <param name="len">Number of bytes to get</param>
        /// <returns></returns>
        public byte[] Get(int len)
        {
            if (len > Count)
            {
                throw new ArgumentException (string.Format("Only {0} bytes in buffer, requested {1}",Count , len));
            }

            byte[] data = new byte[len];
            lock (mBuff)
            {
                if (mHead + len > mCapacityPlusOne)
                {
                    // Split in two parts
                    int split = mCapacityPlusOne - mHead;
                    // First copy part to the end of the ringbuffer
                    Buffer.BlockCopy(mBuff, mHead, data, 0, split);
                    // Wrap around
                    Buffer.BlockCopy(mBuff, 0, data, split, len - split);
                    // Set new head
                    mHead = len - split;
                }
                else
                {
                    // Only one copy action
                    Buffer.BlockCopy(mBuff, mHead, data, 0, len);
                    // Set new head
                    mHead = mHead + len;
                }
            }
            return data;
        }

        /// <summary>
        /// Clear the ringbuffer
        /// </summary>
        public void Clear()
        {
            mHead = mTail;
        }
   }
}
