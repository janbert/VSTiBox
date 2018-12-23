using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace VSTiBox.Controls
{
    public partial class OnSongControl : UserControl
    {
        public OnSongControl()
        {
            InitializeComponent();
        }

        private Font mHeaderLargeFont = new Font("Consolas", 16, FontStyle.Bold);
        private Font mHeaderSmallFont = new Font("Consolas", 14, FontStyle.Regular);
        private Font mBoldFont = new Font("Consolas", 12, FontStyle.Bold);
        private Font mNormalFont = new Font("Consolas", 12, FontStyle.Bold);

        enum SectionType
        {
            None,
            Intro,
            Instrumental,
            Verse,
            Chorus,
            Pre_Chorus,
            Bridge,
            Ending, 
            Tag,
        };

        private class Section
        {
            public SectionType SectionType;
            public int Number;
            public List<string> Lines;
            public string Id
            {
                get
                {
                    if (Number != -1)
                    {
                        return SectionType.ToString()[0].ToString() + Number.ToString();
                    }
                    else
                    {
                        return SectionType.ToString()[0].ToString();
                    }
                }
            }

            public Section(SectionType type, int number)
            {
                Lines = new List<string>();
                SectionType = type;
                Number = number;
            }
        }

        private List<string> mHeaderLines = new List<string>();
        private List<Section> mSections = new List<Section>();
        private List<string> mFlow = new List<string>();
        private int mFlowIndex = 0;

        public void LoadFile(string fileName)
        {
            mFlow.Clear();
            mFlowIndex = 0;
            mHeaderLines.Clear();

            if (!File.Exists(fileName))
            {
                throw new ArgumentException(fileName + " does not exists!");
            }
            else if (Path.GetExtension(fileName) != ".onsong")
            {
                throw new ArgumentException("Wrong extension for " + fileName + "!");
            }

            var lines = File.ReadLines(fileName).ToArray();


            int i = 0;
            for (; i < lines.Count(); i++)
            {
                mHeaderLines.Add(lines[i]);
                rtbText.AppendSelection(lines[i] + Environment.NewLine, Color.White, mNormalFont);
                if (lines[i].StartsWith("Flow"))
                {
                    var flow = lines[i].Replace("Flow:", "").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    List<string> flowList = new List<string>();
                    for (int n = 0; n < flow.Count(); n++)
                    {
                        if (flow[n] == "(2x)")
                        {
                            flowList.Add(flow[n - 1]);
                        }
                        else if (flow[n] == "(3x)")
                        {
                            flowList.Add(flow[n - 1]);
                            flowList.Add(flow[n - 1]);
                        }
                        else
                        {
                            flowList.Add(flow[n]);
                        }
                    }
                    mFlow.InsertRange(0, flowList.ToArray());
                    i++;
                    break;
                }
            }

            SectionType sectionType = SectionType.None;
            Section section = null;

            for (; i < lines.Count(); i++)
            {
                if ((lines[i] == string.Empty) && ((i + 1) != lines.Count()) && lines[i + 1].EndsWith(":"))
                {
                    i += 1;
                    string line = lines[i];
                    line = line.Remove(line.IndexOf(':'));
                    string sectionName = line;
                    int sectionNr = -1;
                    if (line.Contains(" "))
                    {
                        var split = line.Split(new char[] { ' ' });
                        sectionName = split[0];
                        int.TryParse(split[1], out sectionNr);
                    }
                    sectionType = SectionType.None;
                    Enum.TryParse(sectionName.Replace('-', '_'), out sectionType);

                    if (sectionType != SectionType.None)
                    {
                        section = new Section(sectionType, sectionNr);
                        mSections.Add(section);
                    }
                }
                else if (sectionType != SectionType.None && section != null)
                {
                    section.Lines.Add(lines[i]);
                }
            }

            ParseSection(0);
        }

        public void NextSection()
        {
            if (mFlowIndex < mFlow.Count - 1)
            {
                mFlowIndex++;
                ParseSection(mFlowIndex);
            }
        }

        public void PreviousSection()
        {
            if (mFlowIndex != 0)
            {
                mFlowIndex--;
                ParseSection(mFlowIndex);
            }
        }

        void ParseSection(int index)
        {
            rtbText.SuspendLayout();
            rtbText.Clear();

            try
            {
                // Show header
                rtbText.AppendSelection(mHeaderLines[0] + Environment.NewLine, Color.White, mHeaderLargeFont);
                rtbText.AppendSelection(mHeaderLines[1] + Environment.NewLine, Color.White, mHeaderSmallFont);
                for (int i = 2; i < mHeaderLines.Count(); i++)
                {
                    rtbText.AppendSelection(mHeaderLines[i] + Environment.NewLine, Color.White, mNormalFont);
                }

                Section section = mSections.First(x => x.Id == mFlow[index]);
                rtbText.AppendSelection(string.Join(" ", mFlow.Take(index + 1)) + Environment.NewLine + Environment.NewLine, Color.White, mNormalFont);

                if (section.Number == -1)
                {
                    rtbText.AppendSelection(section.SectionType.ToString() + Environment.NewLine + Environment.NewLine, Color.White, mHeaderSmallFont);
                }
                else
                {
                    rtbText.AppendSelection(section.SectionType.ToString() + " " + section.Number.ToString() + Environment.NewLine + Environment.NewLine, Color.White, mHeaderSmallFont);
                }

                for (int i = 0; i < section.Lines.Count(); i++)
                {
                    if (section.SectionType == SectionType.Bridge || section.SectionType == SectionType.Chorus || section.SectionType == SectionType.Verse)
                    {
                        // Split line in 2: Chords and text
                        string line = section.Lines[i];
                        if (line.Contains("["))
                        {
                            StringBuilder sbChords = new StringBuilder();
                            StringBuilder sbText = new StringBuilder();

                            bool inChord = false;
                            int inChordCount = 0;
                            foreach (var c in line)
                            {
                                if (c == '[')
                                {
                                    inChord = true;
                                    inChordCount = 0;
                                }
                                else if (c == ']')
                                {
                                    inChord = false;
                                }
                                else
                                {
                                    if (inChord)
                                    {
                                        sbChords.Append(c);
                                        inChordCount++;
                                    }
                                    else
                                    {
                                        if (inChordCount > 0)
                                        {
                                            inChordCount--;
                                        }
                                        else
                                        {
                                            sbChords.Append(' ');
                                        }
                                        sbText.Append(c);
                                    }
                                }
                            }

                            rtbText.AppendSelection(sbChords.ToString() + Environment.NewLine, Color.White, mBoldFont);
                            rtbText.AppendSelection(sbText.ToString() + Environment.NewLine, Color.White, mNormalFont);
                        }
                        else
                        {
                            rtbText.AppendSelection(section.Lines[i] + Environment.NewLine, Color.White, mNormalFont);
                        }
                    }
                    else
                    {
                        rtbText.AppendSelection(section.Lines[i] + Environment.NewLine, Color.White, mNormalFont);
                    }
                }
            }
            catch (Exception ex)
            {
                rtbText.AppendSelection("ERROR: " + ex.Message, Color.White, mHeaderLargeFont);
            }
            rtbText.ResumeLayout();
        }
    }
}