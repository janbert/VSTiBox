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

        private void RaisePluginCalled(string message)
        {

        }

        #region IVstHostCommandsStub Members

        /// <inheritdoc />
        public IVstPluginContext PluginContext { get; set; }
        
        #endregion

        #region IVstHostCommands20 Members

        /// <inheritdoc />
        public bool BeginEdit(int index)
        {
            RaisePluginCalled("BeginEdit(" + index + ")");
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
            
            RaisePluginCalled("CanDo(" + cando + ")");
            return Jacobi.Vst.Core.VstCanDoResult.Unknown;
        }

        /// <inheritdoc />
        public bool CloseFileSelector(Jacobi.Vst.Core.VstFileSelect fileSelect)
        {
            RaisePluginCalled("CloseFileSelector(" + fileSelect.Command + ")");
            return false;
        }

        /// <inheritdoc />
        public bool EndEdit(int index)
        {
            RaisePluginCalled("EndEdit(" + index + ")");
            return false;
        }

        /// <inheritdoc />
        public Jacobi.Vst.Core.VstAutomationStates GetAutomationState()
        {
            RaisePluginCalled("GetAutomationState()");
            return Jacobi.Vst.Core.VstAutomationStates.Off;
        }

        /// <inheritdoc />
        public int GetBlockSize()
        {
            RaisePluginCalled("GetBlockSize()");
            return 1024;
        }

        /// <inheritdoc />
        public string GetDirectory()
        {
            RaisePluginCalled("GetDirectory()");
            return null;
        }

        /// <inheritdoc />
        public int GetInputLatency()
        {
            RaisePluginCalled("GetInputLatency()");
            return 0;
        }

        /// <inheritdoc />
        public Jacobi.Vst.Core.VstHostLanguage GetLanguage()
        {
            RaisePluginCalled("GetLanguage()");
            return Jacobi.Vst.Core.VstHostLanguage.NotSupported;
        }

        /// <inheritdoc />
        public int GetOutputLatency()
        {
            RaisePluginCalled("GetOutputLatency()");
            return 0;
        }

        /// <inheritdoc />
        public Jacobi.Vst.Core.VstProcessLevels GetProcessLevel()
        {
            RaisePluginCalled("GetProcessLevel()");
            return Jacobi.Vst.Core.VstProcessLevels.Unknown;
        }

        /// <inheritdoc />
        public string GetProductString()
        {
            RaisePluginCalled("GetProductString()");
            return "VST.NET";
        }

        /// <inheritdoc />
        public float GetSampleRate()
        {
            RaisePluginCalled("GetSampleRate()");
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
            RaisePluginCalled("GetVendorString()");
            return "Jacobi Software";
        }

        /// <inheritdoc />
        public int GetVendorVersion()
        {
            RaisePluginCalled("GetVendorVersion()");
            return 1000;
        }

        /// <inheritdoc />
        public bool IoChanged()
        {
            RaisePluginCalled("IoChanged()");
            return false;
        }

        /// <inheritdoc />
        public bool OpenFileSelector(Jacobi.Vst.Core.VstFileSelect fileSelect)
        {
            RaisePluginCalled("OpenFileSelector(" + fileSelect.Command + ")");
            return false;
        }

        /// <inheritdoc />
        public bool ProcessEvents(Jacobi.Vst.Core.VstEvent[] events)
        {
            RaisePluginCalled("ProcessEvents(" + events.Length + ")");
            return false;
        }

        /// <inheritdoc />
        public bool SizeWindow(int width, int height)
        {
            RaisePluginCalled("SizeWindow(" + width + ", " + height + ")");
            return false;
        }

        /// <inheritdoc />
        public bool UpdateDisplay()
        {
            RaisePluginCalled("UpdateDisplay()");
            return false;
        }

        #endregion

        #region IVstHostCommands10 Members

        /// <inheritdoc />
        public int GetCurrentPluginID()
        {
            RaisePluginCalled("GetCurrentPluginID()");
            return PluginContext.PluginInfo.PluginID;
        }

        /// <inheritdoc />
        public int GetVersion()
        {
            RaisePluginCalled("GetVersion()");
            return 1000;
        }

        /// <inheritdoc />
        public void ProcessIdle()
        {
            RaisePluginCalled("ProcessIdle()");
        }

        /// <inheritdoc />
        public void SetParameterAutomated(int index, float value)
        {
            RaisePluginCalled("SetParameterAutomated(" + index + ", " + value + ")");
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
