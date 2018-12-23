using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Linq;
using VSTiBox.Common;

namespace VSTiBox
{
    public class SettingsManager
    {
        private const string SETTINGS_FILENAME = "settings.xml";
        private const string SETTINGS_PATH = "VSTiBox2";
        private Dictionary<Bank, string> mBankDict = new Dictionary<Bank, string>();

        public string SettingsFileName
        {
            get
            {
                return Path.Combine(GetDataFolder(), SETTINGS_FILENAME);
            }
        }

        private string GetDataFolder()
        {
            string path = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), SETTINGS_PATH);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        public PlayList SelectedPlayList
        {
            get
            {
                return Settings.PlayLists.FirstOrDefault(n => n.Name == Settings.SelectedPlayListName);
            }
        }

        public Bank GetBank(string name)
        {
            Bank bank = null;
            foreach (KeyValuePair<Bank, string> kvp in mBankDict)
            {
                if (kvp.Value == name)
                {
                    bank = kvp.Key;
                    break;
                }
            }
            return bank;
        }
                        
        public Settings Settings { get; private set; }

        public SettingsManager()
        {
            Settings = null;

            if (File.Exists(SettingsFileName))
            {
                XmlSerializer x = XmlSerializer.FromTypes(new[] { typeof(Settings) })[0];
                //XmlSerializer x = new XmlSerializer(typeof(Settings));

                using (TextReader reader = new StreamReader(SettingsFileName))
                {
                    try
                    {
                        Settings = (Settings)x.Deserialize(reader);

                        List<string> failedList = new List<string>();
                       
                        foreach (string bankFileName in Settings.BankFileNames)
                        {
                            Bank bank;
                            if (DeserializeBank(bankFileName, out bank))
                            {
                                                                mBankDict.Add(bank, bank.Name);
                            }
                            else
                            {
                                failedList.Add(bankFileName);
                                MessageBox.Show("Failed to load " + bankFileName, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK);
                    }
                }
            }

            if (Settings == null)
            {
                Settings = GetDefaultSettings();
                CreateBank("Bank 1");  // Initiates a save                
            }           
        }

        private bool DeserializeBank(string bankFileName, out Bank bank)
        {           
            if (File.Exists(bankFileName))
            {
                XmlSerializer x = XmlSerializer.FromTypes(new[] { typeof(Bank) })[0];
                using (TextReader reader = new StreamReader(bankFileName))
                {
                    try
                    {
                        bank = (Bank)x.Deserialize(reader);

                        /* Update to version 1 */
                        if (bank.Version == 0)
                        {
                            bank.Version = 1;
                            foreach (var preset in bank.Presets)
                            {
                                preset.KeyboardVelocityGain = 1.0f;
                                preset.KeyboardVelocityOffset = 0;
                            }
                        }

                        return true; 
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK);
                    }
                }
            }

            bank = null;
            return false; 
        }

        public string[] BankNames
        {
            get
            {
                List<string> names = new List<string>();
                foreach (Bank bank in mBankDict.Keys)
                {
                    names.Add(bank.Name);
                }
                return names.ToArray();
            }
        }

        public void SaveBank(Bank bank)
        {
            // Check if name changed, then filename has to change too
            if (mBankDict[bank] != bank.Name)
            {
                string oldFileName = GetBankFileName(mBankDict[bank]);
                Settings.BankFileNames.Remove(oldFileName);
                try
                {
                    File.Delete(oldFileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(oldFileName + " error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                mBankDict[bank] = bank.Name;
                string newFileName = GetBankFileName(bank.Name);
                Settings.BankFileNames.Add(newFileName);
                SaveSettings();
            }

            // Save bank
            SerializeBank(bank);
        }

        public void RemoveBank(string bankName)
        {
            Bank bank = null;
            foreach (KeyValuePair<Bank, string> kvp in mBankDict)
            {
                if (kvp.Value == bankName)
                {
                    bank = kvp.Key;
                    break;
                }
            }

            string existingFileName = GetBankFileName(bankName);
            Settings.BankFileNames.Remove(existingFileName);
            try
            {
                File.Delete(existingFileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(existingFileName + " error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            mBankDict.Remove (bank);
            Settings.BankFileNames.Remove(existingFileName);
            SaveSettings();
        }
        
        private bool SerializeBank(Bank bank)
        {
            string bankFileName = GetBankFileName(bank.Name);
            
            XmlSerializer x = new XmlSerializer(typeof(Bank));
            using (TextWriter writer = new StreamWriter(bankFileName))
            {
                try
                {
                    x.Serialize(writer, bank);
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK);
                }
            }

            bank = null;
            return false;
        }

        private string GetBankFileName(string name)
        {
            // Remove all special characters
            name = System.Text.RegularExpressions.Regex.Replace(name, "[^0-9a-zA-Z]+", "");
            return Path.Combine(GetDataFolder(), name + ".xml");
        }

        private Settings GetDefaultSettings()
        {
            Settings settings = new Settings();
            settings.BankFileNames = new List<string>();
            settings.VSTPluginFolders = new List<string>();
            settings.Effects = new List<VstInfo>();
            settings.Instruments = new List<VstInfo>();
            settings.KnownVSTPluginDlls = new List<string>();
            settings.AsioBufferSize = 512;
            settings.AsioDeviceNr = -1;
            settings.MidiInDeviceNumbers = new List<int>();
            settings.MidiOutDeviceNumber = -1;
            settings.MasterFxInserts = new VSTPreset[VstPluginChannel.NumberOfEffectPlugins];            
            settings.PlayLists = new List<PlayList>();
            PlayList defaultPlaylist = new PlayList();
            defaultPlaylist.Name = "Default";
            settings.PlayLists.Add(defaultPlaylist);
            settings.SelectedPlayListName = defaultPlaylist.Name; 
            return settings;
        }

        public void CreateBank(string name)
        {
            Bank bank = new Bank();
            List<ChannelPreset> presets = new List<ChannelPreset>();
            for (int x = 0; x < 8; x++)
            {
                ChannelPreset preset = new ChannelPreset();
                preset.InstrumentVstPreset = new VSTPreset();
                preset.InstrumentVstPreset.Name = string.Empty;
                preset.InstrumentVstPreset.State = PluginState.Empty;

                preset.EffectVstPresets = new VSTPreset[VstPluginChannel.NumberOfEffectPlugins];
                for (int i = 0; i < VstPluginChannel.NumberOfEffectPlugins; i++)
                {
                    preset.EffectVstPresets[i] = new VSTPreset();
                    preset.EffectVstPresets[i].State = PluginState.Empty;
                    preset.EffectVstPresets[i].Name = string.Empty;
                }
                preset.Volume = 1.0f;
                preset.Pan = 0.0f;
                presets.Add(preset);
            }
            bank.Presets = presets.ToArray();
            bank.Name = name;
            bank.BPM = 120;
            bank.MultiTrackVolume = 1.0f;
            mBankDict.Add(bank, bank.Name);
            Settings.BankFileNames.Add(GetBankFileName(name));
            SaveBank(bank);
            SaveSettings();
        }

        public void SaveSettings()
        {
            XmlSerializer x = new XmlSerializer(typeof(Settings));
            using (TextWriter writer = new StreamWriter(SettingsFileName))
            {
                try
                {
                    x.Serialize(writer, Settings);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK);
                }
            }
        }
    }
}
