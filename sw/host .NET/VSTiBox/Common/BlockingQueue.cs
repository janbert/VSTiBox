using System;
using System.Collections;
using System.Threading;

namespace VSTiBox
{
    public sealed class BlockingQueue<T>
    {
        private T[] mBuffer;
        public int mCount;
        private int mSize;
        private int mHead;
        private int mTail;
        private readonly object mSyncRoot;
        private ManualResetEvent mQueueNotFullEvent = new ManualResetEvent(true);
        private ManualResetEvent mQueueNotEmptyEvent = new ManualResetEvent(false);

        public BlockingQueue(int size)
        {
            if (size < 1)
            {
                throw new ArgumentOutOfRangeException("Size must be > 0");
            }
            mSyncRoot = new object();
            mSize = size;
            mBuffer = new T[size];
            mCount = 0;
            mHead = 0;
            mTail = 0;
        }

        public object[] Values
        {
            get
            {
                object[] values;
                lock (mSyncRoot)
                {
                    values = new object[mCount];
                    int pos = mHead;
                    for (int i = 0; i < mCount; i++)
                    {
                        values[i] = mBuffer[pos];
                        pos = (pos + 1) % mSize;
                    }
                }
                return values;
            }
        }

        public bool Enqueue(T item)
        {
            return Enqueue(item, Timeout.Infinite);
        }

        public bool Enqueue(T item, int millisecondsTimeout)
        {
            while (true)
            {
                lock (mSyncRoot)
                {
                    if (mCount < mSize)
                    {
                        mBuffer[mTail] = item;
                        mTail = (mTail + 1) % mSize;
                        mCount++;
                        if (mCount == 1)
                        {
                            mQueueNotEmptyEvent.Set();
                        }
                        return true;
                    }
                    else
                    {
                        mQueueNotFullEvent.Reset();
                    }
                }
                if (!mQueueNotFullEvent.WaitOne(millisecondsTimeout, false))
                {
                    return false;
                }
            }
        }

        public bool Dequeue(out T obj)
        {
            return Dequeue(out obj, Timeout.Infinite);
        }
      
        public bool Dequeue(out T obj, int millisecondsTimeout)
        {
            obj = default(T);

            while (true)
            {
                lock (mSyncRoot)
                {
                    if (mCount > 0)
                    {
                        obj = mBuffer[mHead];
                        mBuffer[mHead] = default(T);
                        mHead = (mHead + 1) % mSize;
                        mCount--;

                        if (mCount == (mSize - 1))
                        {
                            mQueueNotFullEvent.Set();
                        }
                        return true;
                    }
                    else
                    {
                        mQueueNotEmptyEvent.Reset();
                    }
                }

                if (!mQueueNotEmptyEvent.WaitOne(millisecondsTimeout, false))
                {
                    return false;
                }
            }
        }

        public void Clear()
        {
            lock (mSyncRoot)
            {
                mCount = 0;
                mHead = 0;
                mTail = 0;
                for (int i = 0; i < mBuffer.Length; i++)
                {
                    mBuffer[i] = default(T);
                }
            }
        }
    } 
}
