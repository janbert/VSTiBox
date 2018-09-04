using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using VSTiBox.Common;
using NAudio.Wave.SampleProviders;
using NAudio.Wave;

namespace VSTiBox
{
    public partial class EffectPluginSelectionControl : UserControl
    {
        public event EventHandler<Boolean> OnEffectInsertChanged;
        private VstPlugin[] mEffectPlugins;

        public EffectPluginSelectionControl()
        {
            InitializeComponent();
        }

        public void ShowEffectInserts(AudioPluginEngine engine, VstPlugin[] effectPlugins)
        {
            flpFxInserts.Controls.Clear();
            mEffectPlugins = effectPlugins;
            List<string> vstEffectNames = new List<string>();
            vstEffectNames.Add("None");
            vstEffectNames.AddRange(engine.AvailableEffectPlugins.Select(x => x.Name));
            string[] fxNames = vstEffectNames.ToArray();

            foreach (VstPlugin effectPlugin in effectPlugins)
            {
                EffectPluginControl c = new EffectPluginControl(engine, effectPlugin, fxNames);
                c.OnEffectInsertChanged += c_OnEffectInsertChanged;
                flpFxInserts.Controls.Add(c);
            }
            this.Visible = true;
        }

        void c_OnEffectInsertChanged(object sender, bool e)
        {
            if (null != OnEffectInsertChanged)
            {
                OnEffectInsertChanged(this, mEffectPlugins.Where(x=>x.State != PluginState.Empty ).Count() > 0);
            }
        }
    }
}
