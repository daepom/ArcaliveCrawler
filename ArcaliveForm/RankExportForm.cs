using Arcalive;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ArcaliveForm
{
    public partial class RankExportForm : Form
    {
        private List<Post> datafile;

        public List<Post> DataFile
        {
            get
            {
                var posts = datafile;

                if (checkBox1.Checked == true)
                {
                    DateTime startTime = dateTimePicker1.Value.Date + dateTimePicker2.Value.TimeOfDay;
                    DateTime endTime = dateTimePicker3.Value.Date + dateTimePicker4.Value.TimeOfDay;
                    posts = LimitPostsByTime(posts, startTime, endTime);
                }

                return posts;
            }
            set
            {
                datafile = value;
            }
        }

        public RankExportForm()
        {
            InitializeComponent();
        }

        private void RankExportForm_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedItem = comboBox1.Items[0];
            comboBox2.SelectedItem = comboBox2.Items[0];
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                dateTimePicker1.Enabled = dateTimePicker2.Enabled = dateTimePicker3.Enabled = dateTimePicker4.Enabled
                    = true;
            }
            else
            {
                dateTimePicker1.Enabled = dateTimePicker2.Enabled = dateTimePicker3.Enabled = dateTimePicker4.Enabled
                    = false;
            }
        }

        private List<Post> LimitPostsByTime(List<Post> posts, DateTime start, DateTime end)
        {
            List<Post> result = new List<Post>();
            foreach (var post in posts)
            {
                if (post.time >= start && post.time <= end)
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

        // 갤창 랭킹
        private void button1_Click(object sender, EventArgs e)
        {
            var posts = DataFile;

            Dictionary<string, Ranking> RankDic = new Dictionary<string, Ranking>();
            Dictionary<string, double> PlayTimeDic = new Dictionary<string, double>();
            Dictionary<string, List<DateTime>> TimeByUsersDic = new Dictionary<string, List<DateTime>>();
            Dictionary<string, List<DateTime>> TimeByUsersDicDesc = new Dictionary<string, List<DateTime>>();
            foreach (var post in posts)
            {
                foreach (var comment in post.comments)
                {
                    if (RankDic.ContainsKey(comment.author) == false)
                        RankDic.Add(comment.author, new Ranking { comment = 1, post = 0, playTime = 0 });
                    else
                        RankDic[comment.author].comment++;

                    if (TimeByUsersDic.ContainsKey(comment.author) == false)
                        TimeByUsersDic.Add(comment.author, new List<DateTime>() { comment.time });
                    else
                        TimeByUsersDic[comment.author].Add(comment.time);
                }

                if (RankDic.ContainsKey(post.author) == false)
                    RankDic.Add(post.author, new Ranking { comment = 0, post = 1, playTime = 0 });
                else
                    RankDic[post.author].post++;

                if (TimeByUsersDic.ContainsKey(post.author) == false)
                    TimeByUsersDic.Add(post.author, new List<DateTime>() { post.time });
                else
                    TimeByUsersDic[post.author].Add(post.time);
            }

            var RankDicDesc = RankDic.OrderByDescending(x => x.Value.post);

            foreach (var a in TimeByUsersDic)
            {
                var list = a.Value.OrderByDescending(x => x).ToList();
                TimeByUsersDicDesc.Add(a.Key, list);
            }

            int timeSpan = 15;

            foreach (var user in TimeByUsersDicDesc)
            {
                var timeList = user.Value;
                TimeSpan activeTime = TimeSpan.Zero;

                if (timeList.Count <= 1)
                {
                    activeTime += TimeSpan.FromMinutes(timeSpan);
                }
                else
                {
                    for (int i = 0; i < timeList.Count - 1; i++)
                    {
                        var diff = timeList[i] - timeList[i + 1];
                        if (diff <= TimeSpan.FromMinutes(timeSpan))
                        {
                            activeTime += diff;
                        }
                        else
                        {
                            activeTime += TimeSpan.FromMinutes(timeSpan);
                        }
                    }
                }


                if (RankDic.ContainsKey(user.Key) == false)
                {
                    Console.WriteLine("error");
                    continue; // 없는 경우가 있나??
                }
                else
                    RankDic[user.Key].playTime = activeTime.TotalHours;
            }

            StringBuilder sb = new StringBuilder();

            sb.Append("//갤창랭킹\r\n");
            sb.Append("작성자, 글, 댓글, 플레이타임(시)\r\n");
            foreach (var rank in RankDicDesc)
            {
                sb.Append($"{rank.Key}, {rank.Value.post}, {rank.Value.comment}, {Math.Round(rank.Value.playTime, 2)}\r\n");
            }

            SaveTextFile(sb.ToString());
        }

        // 시간별
        private void button2_Click(object sender, EventArgs e)
        {
            var posts = DataFile;

            StringBuilder sb = new StringBuilder();
            var timeDic = new Dictionary<string, int>();
            var weekDic = new Dictionary<string, int>();
            var dateDic = new Dictionary<int, int>();

            foreach (var post in posts)
            {
                var hour = post.time.ToString("HH");
                if (timeDic.ContainsKey(hour) == false)
                {
                    timeDic.Add(hour, 1);
                }
                else timeDic[hour]++;

                var week = post.time.DayOfWeek;
                if (weekDic.ContainsKey(week.ToString()) == false)
                {
                    weekDic.Add(week.ToString(), 1);
                }
                else weekDic[week.ToString()]++;

                var date = post.time.Date.Day;
                if (dateDic.ContainsKey(date) == false)
                {
                    dateDic.Add(date, 1);
                }
                else dateDic[date]++;
            }

            var timeDicDesc = timeDic.OrderBy(x => x.Key);
            var dateDicDesc = dateDic.OrderBy(x => x.Key);
            var weekDicDesc = weekDic.OrderBy(x => x.Key);

            sb.AppendLine("//시간별");
            foreach (var time in timeDicDesc)
            {
                sb.AppendLine($"{time.Key}, {time.Value}");
            }
            sb.AppendLine();
            sb.AppendLine("//요일별");
            foreach (var week in weekDicDesc)
            {
                sb.AppendLine($"{week.Key}, {week.Value}");
            }
            sb.AppendLine();
            sb.AppendLine("//날짜별");
            foreach (var date in dateDicDesc)
            {
                sb.AppendLine($"{date.Key}, {date.Value}");
            }

            SaveTextFile(sb.ToString());
        }

        // 아카콘 랭킹
        private void button3_Click(object sender, EventArgs e)
        {
            var posts = DataFile;

            Dictionary<string, int> arcaconDic = new Dictionary<string, int>();
            foreach (var post in posts)
            {
                foreach (var comment in post.comments)
                {
                    if (comment.isArcacon != true) continue;
                    else if (arcaconDic.ContainsKey(comment.content) == false)
                        arcaconDic.Add(comment.content, 1);
                    else
                        arcaconDic[comment.content]++;
                }
            }
            var arcaconDicDesc = arcaconDic.OrderByDescending(x => x.Value);

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("//아카콘 랭킹");
            sb.AppendLine($"총 댓글 / 아카콘 = {posts.Sum(x => x.comments.Count)} / {arcaconDicDesc.Sum(x => x.Value)}");
            sb.AppendLine("아카콘 링크, 사용 횟수");
            foreach (var dic in arcaconDicDesc)
            {
                sb.AppendLine($"{"https:" + dic.Key}, {dic.Value}");
            }

            SaveTextFile(sb.ToString());
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
                DataFile = ArcaliveCrawler.DeserializePosts(openFile.FileName);
                textBox1.Text = openFile.FileName;
                dateTimePicker1.Value = dateTimePicker2.Value = DataFile.Last().time;
                dateTimePicker3.Value = dateTimePicker4.Value = DataFile.First().time;
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
    }
}