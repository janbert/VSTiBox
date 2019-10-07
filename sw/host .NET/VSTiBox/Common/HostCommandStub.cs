using System;

using Jacobi.Vst.Core.Host;
using Jacobi.Vst.Interop.Host;
using Jacobi.Vst.Core;

namespace VSTiBox
{
    /// <summary>
    /// The HostCommandStub class represents the part of the host that a plugin can call.
    /// </summary>
    public abstract class VstHostCommandBase : IVstHostCommandStub 
    {     
        public VstTimeInfo VstTimeInfo { get; private set; }

        public virtual int BPM { get; set; }

        public VstHostCommandBase()
        {
            VstTimeInfo = new VstTimeInfo();
            BPM = 120;
        }

        #region IVstHostCommandsStub Members

        /// <inheritdoc />
        public IVstPluginContext PluginContext { get; set; }
        
        #endregion

        #region IVstHostCommands20 Members

        /// <inheritdoc />
        public bool BeginEdit(int index)
        {
            return false;
        }

        /// <inheritdoc />
        public Jacobi.Vst.Core.VstCanDoResult CanDo(string cando)
        {
            switch(cando)            
            {
                case("sendVstTimeInfo"):
                    return Jacobi.Vst.Core.VstCanDoResult.Yes;
                case ("sizeWindow"):
                    return Jacobi.Vst.Core.VstCanDoResult.No;
            }
            
            return Jacobi.Vst.Core.VstCanDoResult.Unknown;
        }

        /// <inheritdoc />
        public bool CloseFileSelector(Jacobi.Vst.Core.VstFileSelect fileSelect)
        {
            return false;
        }

        /// <inheritdoc />
        public bool EndEdit(int index)
        {
            return false;
        }

        /// <inheritdoc />
        public Jacobi.Vst.Core.VstAutomationStates GetAutomationState()
        {
            return Jacobi.Vst.Core.VstAutomationStates.Off;
        }

        /// <inheritdoc />
        public int GetBlockSize()
        {
            return 1024;
        }

        /// <inheritdoc />
        public string GetDirectory()
        {
            return null;
        }

        /// <inheritdoc />
        public int GetInputLatency()
        {
            return 0;
        }

        /// <inheritdoc />
        public Jacobi.Vst.Core.VstHostLanguage GetLanguage()
        {
            return Jacobi.Vst.Core.VstHostLanguage.NotSupported;
        }

        /// <inheritdoc />
        public int GetOutputLatency()
        {
            return 0;
        }

        /// <inheritdoc />
        public Jacobi.Vst.Core.VstProcessLevels GetProcessLevel()
        {
            return Jacobi.Vst.Core.VstProcessLevels.Unknown;
        }

        /// <inheritdoc />
        public string GetProductString()
        {
            return "VST.NET";
        }

        /// <inheritdoc />
        public float GetSampleRate()
        {
            return 44.1f;
        }

        /// <inheritdoc />
        public Jacobi.Vst.Core.VstTimeInfo GetTimeInfo(Jacobi.Vst.Core.VstTimeInfoFlags filterFlags)
        {
            VstTimeInfo.SampleRate = 44100.0;
            VstTimeInfo.Tempo = (double)BPM; 
            VstTimeInfo.PpqPosition = (VstTimeInfo.SamplePosition / VstTimeInfo.SampleRate) * (VstTimeInfo.Tempo / 60.0);
            VstTimeInfo.NanoSeconds = 0.0;
            VstTimeInfo.BarStartPosition = 0.0;
            VstTimeInfo.CycleStartPosition = 0.0;
            VstTimeInfo.CycleEndPosition = 0.0;
            VstTimeInfo.TimeSignatureNumerator = 4;
            VstTimeInfo.TimeSignatureDenominator = 4;
            VstTimeInfo.SmpteOffset = 0;
            VstTimeInfo.SmpteFrameRate = new Jacobi.Vst.Core.VstSmpteFrameRate();
            VstTimeInfo.SamplesToNearestClock = 0;
            VstTimeInfo.Flags = VstTimeInfoFlags.TempoValid |
                                VstTimeInfoFlags.PpqPositionValid | 
                                VstTimeInfoFlags.TransportPlaying;
            return VstTimeInfo;            
        }

        /// <inheritdoc />
        public string GetVendorString()
        {
            return "Jacobi Software";
        }

        /// <inheritdoc />
        public int GetVendorVersion()
        {
            return 1000;
        }

        /// <inheritdoc />
        public bool IoChanged()
        {
            return false;
        }

        /// <inheritdoc />
        public bool OpenFileSelector(Jacobi.Vst.Core.VstFileSelect fileSelect)
        {
            return false;
        }

        /// <inheritdoc />
        public bool ProcessEvents(Jacobi.Vst.Core.VstEvent[] events)
        {
            return false;
        }

        /// <inheritdoc />
        public bool SizeWindow(int width, int height)
        {
            return false;
        }

        /// <inheritdoc />
        public bool UpdateDisplay()
        {
            return false;
        }

        #endregion

        #region IVstHostCommands10 Members

        /// <inheritdoc />
        public int GetCurrentPluginID()
        {
            if (PluginContext.PluginInfo != null)
            {
                return PluginContext.PluginInfo.PluginID;
            }
            else
            {
                return 0;
            }
        }

        /// <inheritdoc />
        public int GetVersion()
        {
            return 1000;
        }

        /// <inheritdoc />
        public void ProcessIdle()
        {
        }

        /// <inheritdoc />
        public void SetParameterAutomated(int index, float value)
        {
        }

        #endregion
    }

    /// <summary>
    /// Event arguments used when one of the mehtods is called.
    /// </summary>
    public class PluginCalledEventArgs : EventArgs
    {
        /// <summary>
        /// Constructs a new instance with a <paramref name="message"/>.
        /// </summary>
        /// <param name="message"></param>
        public PluginCalledEventArgs(string message)
        {
            Message = message;
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        public string Message { get; private set; }
    }
}
