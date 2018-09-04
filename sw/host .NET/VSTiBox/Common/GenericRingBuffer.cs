using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VSTiBox
{
    public sealed class GenericRingBuffer<T>
    {
        private readonly T[] mArray;
        private int mHead;
        private int mTail;
        private readonly int mCapacity;

        /// <summary>
        /// Initializes a new instance of RingBuffer with maximum 511 elements
        /// </summary>
        public GenericRingBuffer()
            : this(512)
        {
        }

        /// <summary>
        /// Initializes RingBuffer with the specified capacity-1; e.g; size 512 means maximum 511 elements
        /// </summary>
        /// <param name="size">Maximum number of elements to store</param>
        public GenericRingBuffer(int size)
        {
            //Check if size is power of 2           
            if ((size & (size - 1)) != 0)
                throw new ArgumentOutOfRangeException("size", "Size must be a power of 2");

            mCapacity = size;
            mArray = new T[size];            
            mHead = 0;
            mTail = 0;
        }

        /// <summary>
        /// Number of items in buffer
        /// </summary>
        public int Count
        {
            get
            {
                if (mTail == mHead)
                {
                    return 0;
                }
                else if (mHead > mTail)
                {
                    return mHead - mTail;
                }
                else
                {
                    return mCapacity - mTail + mHead;
                }
            }
        }

        /// <summary>
        /// Add a single item
        /// </summary>
        /// <param name="item">Item</param>
        /// <returns>Returns true if added, returns false if buffer full</returns>
        public bool Add(T item)
        {
            // Space enough?
            if (mCapacity - 1 - Count == 0)
            {
                return false;
            }

            int newHead = mHead + 1;           
            newHead %= mCapacity;           
            mArray[newHead] = item;
            mHead = newHead;          
            return true; 
        }

        /// <summary>
        /// Peek the oldest item  
        /// </summary>
        /// <returns>Single item</returns>
        public T Peek()
        {            
            int newTail = mTail + 1;
            newTail %= mCapacity;            
            T ret = mArray[newTail];           
            return ret;
        }

        /// <summary>
        /// Peek all items  
        /// </summary>
        /// <param name="all">All items in buffer</param>
        /// <returns>Number of items in buffer</returns>
        public int PeekAll(out T[] all)
        {
            int count = Count;
            if (count == 0)
            {
                all = null;
            }
            else
            {
                all = new T[count];
                int newTail = mTail;
                for (int i = 0; i < count; i++)
                {
                    newTail++;
                    newTail %= mCapacity;
                    all[i] = mArray[newTail];
                }
               
            }
            return count;
        }

        /// <summary>
        /// Free number of items after being used with Peek or PeekAll
        /// </summary>
        /// <param name="count"></param>
        public void Free(int count)
        {
            int newTail = mTail + count;
            newTail %= mCapacity;           
            mTail = newTail;
        }
         
        /// <summary>
        /// Clear all the items in the buffer
        /// </summary>
        public void Clear()
        {
            mHead = mTail;
        }                     
    }
}

