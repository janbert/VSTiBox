using Jacobi.Vst.Core;
using Jacobi.Vst.Interop.Host;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VSTiBox.Common
{
    public enum PluginState
    {
        Empty, // (= Unloaded)
        Unloading,
        Deactivated,
        Activated
    };

    public enum MidiChannel
    {
        ChannelAll = 0,
        Channel1 = 1,
        Channel2 = 2,
        Channel3 = 3,
        Channel4 = 4,
        Channel5 = 5,
        Channel6 = 6,
        Channel7 = 7,
        Channel8 = 8,
        Channel9 = 9,
        Channel10 = 10,
        Channel11 = 11,
        Channel12 = 12,
        Channel13 = 13,
        Channel14 = 14,
        Channel15 = 15,
        Channel16 = 16,
    }

    public class VstPlugin
    {
        private AudioBufferInfo mParentInBuffers;
        private AudioBufferInfo mParentOutBuffers;
        private int mAsioBuffSize;
        private PluginState mState;
        private VstPluginContext mVstPluginContext;
        private ManualResetEventSlim mUnloadingCompleteEvent = new ManualResetEventSlim(false);
        private Boolean mEditorIsOpen = false;

        public event EventHandler<PluginStateChangeEventArgs> StateChanged;        
        public event EventHandler OnEditorOpening;
        public event EventHandler OnEditorOpened;
        public event EventHandler OnEditorClosed;

        private bool dbgIsEffect = false;

        public VstPlugin(AudioBufferInfo parentInBuffers, AudioBufferInfo parentOutBuffers, int asioBuffSize, bool dbg)
        {
            dbgIsEffect = dbg;
            mParentInBuffers = parentInBuffers;
            mParentOutBuffers = parentOutBuffers;
            mAsioBuffSize = asioBuffSize;
        }

        ~VstPlugin()
        {
            if (InputBuffers != null) InputBuffers = null;
            if (OutputBuffers != null) OutputBuffers = null;
        }
        
        public AudioBufferInfo OutputBuffers { get; set; }
        
        public AudioBufferInfo InputBuffers { get; set; }

        public string PluginName { get; private set; }
        
        public string ProgramName { get; private set; }
        
        public PluginState State 
        {
            get
            {
                return mState;
            }
            private set
            {
                mState = value;
                StateChanged(this, new PluginStateChangeEventArgs(value));
            } 
        }
        
        public MidiChannel MidiChannel { get; set; }
        
        public int EditorOpenedTimer { get; set; }
        
        public bool UseExtendedEffectRange { get; private set; }

        public Panel EditorPanel { get; set; }

        public VSTPreset VstPreset
        {  
            get
            {
                VSTPreset preset = new VSTPreset();
                preset.Name = PluginName;
                preset.State = State;
                if (preset.State != PluginState.Empty)
                {
                    preset.Data = GetData();
                }
                return preset;
            } 
            set
            {
                if(value.State != PluginState.Empty )
                {
                    if (value.Data != null)
                    {
                        SetData(value.Data);
                    }
                }
                if (value.State == PluginState.Activated)
                {
                    Activate();
                }
            }
        }
        
        public VstPluginContext VstPluginContext
        {
            get
            {
                return mVstPluginContext;
            }
        }

        public void SetVstPluginContext(VstPluginContext ctx, string name)
        {
            // Todo: JBK ; still after MainsChanged and StartProcess; ok?
            if (ctx != null)
            {
                // Create big enough buffers to fit all plugin types
                int inSize = 32; // value.PluginInfo.AudioInputCount;
                int outSize = 64;// value.PluginInfo.AudioOutputCount;

                if (mAsioBuffSize != 0)
                {
                    if (InputBuffers == null || inSize != InputBuffers.Count)
                    {
                        InputBuffers = null;    // Dispose first if already existed!
                        InputBuffers = new AudioBufferInfo(inSize, mParentInBuffers);
                    }

                    if (OutputBuffers == null || outSize != OutputBuffers.Count)
                    {
                        OutputBuffers = null;    // Dispose first if already existed!
                        OutputBuffers = new AudioBufferInfo(outSize, mParentOutBuffers);
                    }
                }

                PluginName = name;                 
                ProgramName = ctx.PluginCommandStub.GetProgramName(); 
                UseExtendedEffectRange = (PluginName == "Nexus");
                State = PluginState.Deactivated;
            }
            else
            {
                PluginName = string.Empty;
                ProgramName = string.Empty;
                State = PluginState.Empty;
            }
            mVstPluginContext = ctx;
        }
        

        public virtual void Deactivate()
        {
            if (VstPluginContext == null)
            {
                throw new Exception("shouldnt be here");
            }
            if (State != PluginState.Activated)
            {
                throw new Exception("Wrong state change for deactivate!");
            }
            State = PluginState.Deactivated;
        }

        public void FinishUnloading()
        {
            mUnloadingCompleteEvent.Set();
            State = PluginState.Empty;
            PluginName = string.Empty;
        }

        public void Unload()
        {
            if (State == PluginState.Unloading || State == PluginState.Empty)
            {
                throw new Exception("Shouldn't be here!");
            }

            if (State == PluginState.Activated)
            {
                Deactivate();
            }
            if (State == PluginState.Deactivated)
            {
                // Start unload
                CloseEditor();
                mUnloadingCompleteEvent.Reset();
                State = PluginState.Unloading;
                // Wait for asio callback to handle plugin deactivation to prevent a lock!
                if (mUnloadingCompleteEvent.Wait(1000) == false)
                {
                    // Asio driver not firing events and setting mUnloadingCompleteEvent: ignore
                }
                mVstPluginContext.PluginCommandStub.StopProcess();
                mVstPluginContext.PluginCommandStub.MainsChanged(false);
                mVstPluginContext.PluginCommandStub.Close();
                mVstPluginContext = null;
            }
        }

        public void Activate()
        {           
            if (State != PluginState.Deactivated)
            {
                throw new Exception("Wrong state change for activate!");
            }

            State = PluginState.Activated; 
        }
        
        public void ShowEditor()
        {
            if (VstPluginContext != null && /* EditorPanel.IsHandleCreated && */ EditorPanel.Handle != IntPtr.Zero && !mEditorIsOpen )
            {
                if (OnEditorOpening != null)
                {
                    // Force editor close on other plugins
                    OnEditorOpening(this, null);
                }

                VstPluginContext.PluginCommandStub.EditorOpen(EditorPanel.Handle);
                System.Drawing.Rectangle rect;
                VstPluginContext.PluginCommandStub.EditorGetRect(out rect);
                EditorPanel.Size = new Size(rect.Width, rect.Height);
                EditorOpenedTimer = 0;
                mEditorIsOpen = true;
            }

            if (mEditorIsOpen)
            {
                if (OnEditorOpened != null)
                {
                    OnEditorOpened(this, null);
                }
            }
        }

        public void CloseEditor()
        {
            if (VstPluginContext != null && mEditorIsOpen)
            {               
                VstPluginContext.PluginCommandStub.EditorIdle();
                VstPluginContext.PluginCommandStub.EditorClose();
                mEditorIsOpen = false;

                if (OnEditorClosed != null)
                {
                    // Let other controls know this control wil load plugin UI editor
                    OnEditorClosed(this, null);
                }
            }
        }

        private byte[] GetData()
        {
            if ((mVstPluginContext.PluginInfo.Flags & VstPluginFlags.ProgramChunks) == VstPluginFlags.ProgramChunks)
            {
                ProgramName = mVstPluginContext.PluginCommandStub.GetProgramName();
                return mVstPluginContext.PluginCommandStub.GetChunk(false);
            }
            else
            {
                ProgramName = mVstPluginContext.PluginCommandStub.GetProgramName();
                return GetParameterData();
            }
            
        }

        private void SetData(byte[] data)
        {
            if ((mVstPluginContext.PluginInfo.Flags & VstPluginFlags.ProgramChunks) == VstPluginFlags.ProgramChunks)
            {
                mVstPluginContext.PluginCommandStub.SetChunk(data, false);
            }
            else
            {
                SetParameters(data);
            }
            ProgramName = mVstPluginContext.PluginCommandStub.GetProgramName();
        }

        private void SetParameters(byte[] data)
        {
            try
            {
                VstParameterSet parameterSet = VstParameterSet.FromByteArray(data);
                mVstPluginContext.PluginCommandStub.SetProgram(parameterSet.ProgramNumber);
                foreach (var parameter in parameterSet.Parameters)
                {
                    mVstPluginContext.PluginCommandStub.SetParameter(parameter.Index, parameter.Value);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Cannot import VST parameters : " + ex.Message);
            }
        }

        [Serializable ]
        private class VstParameterSet
        {
            public int ProgramNumber;
            public List<VstParameter> Parameters;

            public VstParameterSet ()
            {
                Parameters = new List<VstParameter>();
            }

            public byte[] ToByteArray()
            {
                byte[] data;
                using (MemoryStream ms = new MemoryStream())
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    try
                    {
                        formatter.Serialize(ms, this);
                        data = ms.ToArray();
                    }
                    catch (SerializationException e)
                    {
                        Console.WriteLine("Failed to serialize VstParameterSet. Reason: " + e.Message);
                        throw;
                    }
                }
                return data;
            }


            public static VstParameterSet FromByteArray(byte[] data)
            {
                VstParameterSet parameterSet = null;
                using (MemoryStream ms = new MemoryStream(data))
                {
                    try
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        parameterSet = (VstParameterSet)formatter.Deserialize(ms);
                    }
                    catch (SerializationException e)
                    {
                        Console.WriteLine("Failed to deserialize VstParameterSet. Reason: " + e.Message);
                        throw;
                    }
                }

                return parameterSet;
            }
        }

         [Serializable ]
        private class VstParameter
        {
             public int Index;
             public float Value;
        }

         private byte[] GetParameterData()
         {
             VstParameterSet parameterSet = new VstParameterSet();

             int emptyCount = 0;
             int parameterCount = 0;
             const int MAX_PARAMETERS = 1000;
             try
             {
                 parameterSet.ProgramNumber = VstPluginContext.PluginCommandStub.GetProgram();

                 for (int i = 0; i < MAX_PARAMETERS; i++)
                 {
                     float parameterValue = VstPluginContext.PluginCommandStub.GetParameter(i);

                     string display = VstPluginContext.PluginCommandStub.GetParameterDisplay(i);
                     string label = VstPluginContext.PluginCommandStub.GetParameterLabel(i);
                     string name = VstPluginContext.PluginCommandStub.GetParameterName(i);

                     //if (parameterValue == 0.0f && (display == "0.000000" || display == "?") && label == string.Empty)
                     //{

                     if (!string.IsNullOrEmpty(name) || !string.IsNullOrEmpty(label))
                     {
                         VstParameter p = new VstParameter();
                         p.Index = i;
                         p.Value = parameterValue;
                         emptyCount = 0;
                         parameterSet.Parameters.Add(p);
                         parameterCount++;
                         if (parameterCount == MAX_PARAMETERS)
                         {
                             MessageBox.Show("More than " + MAX_PARAMETERS.ToString() + " parameters in VST!?!");
                             break;
                         }

                     }
                     else
                     {
                         // 100 x nothing?
                         if (emptyCount++ == 100)
                         {
                             // Assume all parameters are read; break 
                             break;
                         }
                     }
                 }

             }
             catch (Exception ex)
             {
                 MessageBox.Show("Cannot export VST parameters : " + ex.Message);
             }

             return parameterSet.ToByteArray();
         }       
    }
}
