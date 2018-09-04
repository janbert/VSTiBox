using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VSTiBox.Common;
using Jacobi.Vst.Interop.Host;

namespace VSTiBox
{
    public partial class EffectPluginControl : UserControl
    {
        private VstPlugin mVstPlugin;
        private  AudioPluginEngine mAudioPluginEngine;
        private  bool mIgnoreEvents = true;

        public event EventHandler<Boolean> OnEffectInsertChanged;

        public EffectPluginControl(AudioPluginEngine audioPluginEngine, VstPlugin vstPlugin, string[] effectNames)
        {
            mAudioPluginEngine = audioPluginEngine;
            InitializeComponent();

            mVstPlugin = vstPlugin;

            cbEnabled.Checked = (vstPlugin.State == Common.PluginState.Activated);
            cbEffectNames.Items.AddRange(effectNames);

            if (vstPlugin.State == PluginState.Empty)
            {
                cbEffectNames.SelectedIndex = 0;
            }
            else
            {
                try
                {
                    cbEffectNames.SelectedItem = vstPlugin.PluginName;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            cbEnabled.Enabled = cbEffectNames.SelectedIndex != 0;
            btnEdit.Enabled = vstPlugin.State != PluginState.Empty;
            mIgnoreEvents = false;
        }

        private void cbEnabled_CheckedChanged(object sender, EventArgs e)
        {
            if (mIgnoreEvents) return;

            if (cbEnabled.Checked)
            {
                mVstPlugin.Activate();
            }
            else
            {
                mVstPlugin.Deactivate();
            }
            if (null != OnEffectInsertChanged)
            {
                OnEffectInsertChanged(this, cbEnabled.Checked);
            }
        }

        private void cbEffectNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mIgnoreEvents) return;

            if (mVstPlugin.State != PluginState.Empty)
            {
                mVstPlugin.Unload();
            }

            if (cbEffectNames.SelectedItem.ToString() != "None")
            {
                string effectName = cbEffectNames.SelectedItem.ToString();
                mVstPlugin.SetVstPluginContext(mAudioPluginEngine.CreateVstPluginContext(effectName) , effectName);
                btnEdit.Enabled = true;
            }

            cbEnabled.Enabled = cbEffectNames.SelectedIndex != 0;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            mVstPlugin.ShowEditor();
        }
    }
}
