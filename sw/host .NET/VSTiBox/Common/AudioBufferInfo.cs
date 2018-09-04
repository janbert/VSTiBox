using Jacobi.Vst.Core;
using Jacobi.Vst.Interop.Host;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSTiBox
{
    public unsafe class AudioBufferInfo : IDisposable
    {
        public float*[] Raw { get; private set; }
        public int Count { get; private set; }
        public VstAudioBuffer[] Buffers { get; private set; }

        private VstAudioBufferManager mVstAudioBufferManager;
        
        public AudioBufferInfo(int count, int blockSize)
        {
            Count = count;

            // Create VST.NET output buffers
            Raw = new float*[count];
            Buffers = new VstAudioBuffer[count];
            mVstAudioBufferManager = new VstAudioBufferManager(count, blockSize);
            IEnumerator<VstAudioBuffer> bufferEnumerator = mVstAudioBufferManager.GetEnumerator();
            bufferEnumerator.MoveNext();
            for (int i = 0; i < count; i++)
            {
                Buffers[i] = (VstAudioBuffer)bufferEnumerator.Current;
                Raw[i] = ((IDirectBufferAccess32)Buffers[i]).Buffer;
                bufferEnumerator.MoveNext();
            }
        }

        public AudioBufferInfo(int count, AudioBufferInfo parentBuffer)
        {
            // Create child from existing parent buffers
            Count = count;
            
            Raw = new float*[count];
            Buffers = new VstAudioBuffer[count];            

            // Point to parent
            for (int i = 0; i < count; i++)
            {
                Buffers[i] = parentBuffer.Buffers[i];
                Raw[i] = parentBuffer.Raw[i]; 
            }
        }

        void IDisposable.Dispose()
        {
            if(mVstAudioBufferManager!=null)
            { 
                mVstAudioBufferManager.Dispose();
        
            }
        }
    }
}
