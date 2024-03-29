﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ArcaliveCrawler.Statistics;
using ArcaliveCrawler.Utils;
using Crawler;

namespace ArcaliveCrawler
{
    public partial class RankExportForm : Form
    {
        private List<PostInfo> _datafile;
        public List<DateTime> StartTimes { get; set; }
        public List<DateTime> EndTimes { get; set; }
        private StatisticsMaker statisticsMaker;

        public List<PostInfo> DataFile
        {
            get
            {
                var posts = _datafile;


                return posts;
            }
            set
            {
                _datafile = value;
            }
        }

        List<PostInfo> GetDataFile(int index)
        {
            var posts = _datafile;
            if (checkBox1.Checked == true)
            {
                DateTime startTime = StartTimes[index];
                DateTime endTime = EndTimes[index];
                posts = LimitPostsByTime(posts, startTime, endTime);
            }
            Console.Write(posts.Count);
            return posts;
        }

        public RankExportForm()
        {
            InitializeComponent();
        }

        private void RankExportForm_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedItem = comboBox1.Items[0];
            comboBox2.SelectedItem = comboBox2.Items[0];
            statSelectionComboBox.Items.Add("선택되지 않음");
            statSelectionComboBox.SelectedIndex = 0;

            GenDatabase<StatisticsMaker>.InitDatabase();
            foreach (var maker in GenDatabase<StatisticsMaker>.StatisticsMakers.OrderBy(x => x.Name))
            {
                statSelectionComboBox.Items.Add(maker.Name);
            }
        }

        private void statExportButton_Click(object sender, EventArgs e)
        {
            string name = statSelectionComboBox.SelectedItem.ToString();
            var stat = GenDatabase<StatisticsMaker>.GetNamed(name);
            int cnt;
            if (StartTimes == null || checkBox1.Checked == false)
                cnt = 1;
            else
                cnt = StartTimes.Count;
            for (int i = 0; i < cnt; i++)
            {
                var dataFile = GetDataFile(i);
                if (name.Contains("아카콘 종류별"))
                {
                    ProgressArcaconRankForm f = new ProgressArcaconRankForm(dataFile)
                    {
                        StartPosition = FormStartPosition.CenterParent
                    };
                    f.ShowDialog();
                    SaveTextFile(f.Result);
                    return;
                }

                stat.Posts = dataFile;
                SaveTextFile(stat.MakeStatistics().ToString());
            }
            
        }

        private void statSelectionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (statSelectionComboBox.SelectedIndex > 0)
                statExportButton.Enabled = true;
            else
                statExportButton.Enabled = false;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                button5.Enabled = button6.Enabled = true;
            }
            else
            {
                button5.Enabled = button6.Enabled = false;
            }
        }

        private List<PostInfo> LimitPostsByTime(List<PostInfo> posts, DateTime start, DateTime end)
        {
            List<PostInfo> result = new List<PostInfo>();
            foreach (var post in posts)
            {
                if (post.dt >= start && post.dt <= end)
                    result.Add(post);
            }
            return result;
        }

        private void SaveTextFile(string str)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                SaveFileDialog saveFile = new SaveFileDialog
                {
                    DefaultExt = "txt",
                    Title = "텍스트 파일을 어디에 저장할까요?",
                    Filter = "텍스트 파일 (*.txt)|*.txt"
                };
                if (saveFile.ShowDialog() == DialogResult.OK)
                {
                    if (comboBox2.SelectedIndex == 0)
                        File.WriteAllText(saveFile.FileName, str, Encoding.UTF8);
                    else
                        File.WriteAllText(saveFile.FileName, str, Encoding.GetEncoding(51949));
                }
            }
            else
            {
                SaveFileDialog saveFile = new SaveFileDialog
                {
                    DefaultExt = "csv",
                    Title = "텍스트 파일을 어디에 저장할까요?",
                    Filter = "텍스트 파일 (*.csv)|*.csv"
                };
                if (saveFile.ShowDialog() == DialogResult.OK)
                {
                    if (comboBox2.SelectedIndex == 0)
                        File.WriteAllText(saveFile.FileName, str, Encoding.UTF8);
                    else
                        File.WriteAllText(saveFile.FileName, str, Encoding.GetEncoding(51949));
                }
            }
        }

        private void FileChooseButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog
            {
                DefaultExt = "dat",
                Title = "크롤링 데이터 파일을 선택해주세요.",
                Filter = "크롤링 데이터 파일 (*.dat)|*.dat"
            };
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                DataFile = DataFileUtility.DeserializePosts(openFile.FileName);
                textBox1.Text = openFile.FileName;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
            {
                textBox2.Enabled = true;
            }
            else
            {
                textBox2.Enabled = false;
            }
        }

        //추가 버튼
        private void button5_Click(object sender, EventArgs e)
        {
            var f = DataFile == null ? new AddDataForm() : 
                new AddDataForm(DataFile.Last().dt, DataFile.First().dt);
            f.StartPosition = FormStartPosition.CenterParent;

            if (f.ShowDialog() == DialogResult.OK)
            {
                var data = f.SendDateTime();

                StartTimes ??= new List<DateTime>();
                EndTimes ??= new List<DateTime>();


                StartTimes.Add(data[0]);
                EndTimes.Add(data[1]);

                label1.Text += $"{StartTimes.Count}) {data[0].Day}일 {data[0].TimeOfDay} 부터 {data[1].Day}일 {data[1].TimeOfDay}" + Environment.NewLine;
            }
        }

        //초기화 버튼
        private void button6_Click(object sender, EventArgs e)
        {
            label1.Text = string.Empty;;
            StartTimes = null;
            EndTimes = null;
        }


    }
}