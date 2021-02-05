using Arcalive;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArcaliveForm
{
    public partial class Form1 : Form
    {
        private StringBuilder sb;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var currentRelease = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Text += " / " + currentRelease;

            string html = string.Empty;
            using (WebClient wc = new WebClient())
            {
                html = wc.DownloadString("https://github.com/tjgus1668/arcalivecrawler/releases");
            }
            HtmlAgilityPack.HtmlDocument hd = new HtmlAgilityPack.HtmlDocument();
            hd.LoadHtml(html);
            var releases = hd.DocumentNode.SelectNodes("//div[contains(@class, 'release-entry')]");
            var latestRelease = releases[0].SelectSingleNode("//div[contains(@class, 'd-flex flex-items-start')]/div[1]/a").InnerText;

            if (currentRelease == latestRelease) linkLabel1.Text = "최신 버전입니다.";
            else linkLabel1.Text = "최신 버전 이용 가능.";
        }

        private void WriteLog(object sender, EventArgs arg)
        {
            Invoke((Action)(() =>
            {
                textBox2.AppendText((arg as PrintCallbackArg).Str + "\r\n");
            }));
        }

        private void DumpText(object sender, EventArgs arg)
        {
            Invoke((Action)(() =>
            {
                if (checkBox1.Checked) sb.AppendLine((arg as PrintCallbackArg).Str);
            }));
        }

        // 크롤링
        private async void button1_ClickAsync(object sender, EventArgs e)
        {
            sb = new StringBuilder();
            var channel = ArcaliveCrawler.GetChannelLinks(channelNameTextBox.Text);
            if (channel.Count > 1)
            {
                MessageBox.Show("채널 검색 결과가 2개 이상입니다.");
                return;
                // TODO: 리스트 띄우고 설정창
            }
            else if (channel.Count == 0)
            {
                MessageBox.Show("채널 검색 결과가 없습니다.");
                return;
            }

            ArcaliveCrawler ac = new ArcaliveCrawler(channel[0]);
            DateTime startDate = dateTimePicker1.Value.Date + dateTimePicker2.Value.TimeOfDay;
            DateTime endDate = dateTimePicker3.Value.Date + dateTimePicker4.Value.TimeOfDay;

            ac.DumpText += DumpText;
            ac.Print += WriteLog;

            var str = textBox3.Text.Split(',');
            var skip = str.ToList();

            List<Post> posts = new List<Post>();
            Task task = Task.Factory.StartNew(() =>
            {
                // posts = ac.GetPosts(startDate, endDate, checkBox3.Checked, int.Parse(textBox1.Text));
                posts = ac.CrawlBoards(startDate, endDate, int.Parse(textBox1.Text));
                posts = ac.CrawlPosts(posts, skip);
            });
            await Task.WhenAll(task);

            string filename = string.Empty;
            SaveFileDialog saveFile = new SaveFileDialog
            {
                DefaultExt = "dat",
                Title = "크롤링 데이터 파일을 어디에 저장할까요?"
            };
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                filename = saveFile.FileName;
            }
            else return;
            ArcaliveCrawler.SerializationPosts(posts, filename);
            if (checkBox1.Checked == true)
            {
                SaveFileDialog saveDump = new SaveFileDialog
                {
                    DefaultExt = "txt",
                    Title = "덤프 파일을 어디에 저장할까요?"
                };
                if (saveDump.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(saveDump.FileName, sb.ToString());
                }
            }
            MessageBox.Show("저장 완료");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog
            {
                DefaultExt = "dat",
                Title = "크롤링 데이터 파일을 선택해주세요."
            };
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                var posts = ArcaliveCrawler.DeserializationPosts(openFile.FileName);

                Dictionary<string, int> postAuthor = new Dictionary<string, int>();
                Dictionary<string, int> commentAuthor = new Dictionary<string, int>();
                foreach (var post in posts)
                {
                    foreach (var comment in post.comments)
                    {
                        if (commentAuthor.ContainsKey(comment.author) == false)
                            commentAuthor.Add(comment.author, 1);
                        else
                            commentAuthor[comment.author]++;
                    }

                    if (postAuthor.ContainsKey(post.author) == false)
                        postAuthor.Add(post.author, 1);
                    else
                        postAuthor[post.author]++;
                }
                var paDesc = postAuthor.OrderByDescending(x => x.Value);
                var commentsDesc = commentAuthor.OrderByDescending(x => x.Value);

                StringBuilder sb = new StringBuilder();

                sb.Append("//글\r\n");
                foreach (var dic in paDesc)
                {
                    sb.Append($"{dic.Key}, {dic.Value}\r\n");
                }
                sb.Append("\r\n//댓글\r\n");
                foreach (var comment in commentsDesc)
                {
                    sb.Append($"{comment.Key}, {comment.Value}\r\n");
                }

                SaveFileDialog saveFile = new SaveFileDialog
                {
                    DefaultExt = "txt",
                    Title = "텍스트 파일을 어디에 저장할까요?"
                };
                if (saveFile.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(saveFile.FileName, sb.ToString());
                }
            }
            else return;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog
            {
                DefaultExt = "dat",
                Title = "크롤링 데이터 파일을 선택해주세요."
            };
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                var posts = ArcaliveCrawler.DeserializationPosts(openFile.FileName);

                StringBuilder sb = new StringBuilder();

                posts.ForEach(x => { sb.Append($"{x.title}\r\n"); });

                SaveFileDialog saveFile = new SaveFileDialog
                {
                    DefaultExt = "txt",
                    Title = "텍스트 파일을 어디에 저장할까요?"
                };
                if (saveFile.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(saveFile.FileName, sb.ToString());
                }
            }
            else return;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog
            {
                DefaultExt = "dat",
                Title = "크롤링 데이터 파일을 선택해주세요."
            };
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                var posts = ArcaliveCrawler.DeserializationPosts(openFile.FileName);

                StringBuilder sb = new StringBuilder();
                var timeDic = new Dictionary<string, int>();
                var weekDic = new Dictionary<string, int>();
                var dateDic = new Dictionary<string, int>();

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
                    if (dateDic.ContainsKey(date.ToString()) == false)
                    {
                        dateDic.Add(date.ToString(), 1);
                    }
                    else dateDic[date.ToString()]++;
                }

                var timeDicDesc = timeDic.OrderBy(x => x.Key);
                var dateDicDesc = dateDic.OrderBy(x => x.Key);

                foreach (var time in timeDicDesc)
                {
                    sb.AppendLine($"{time.Key}, {time.Value}");
                }
                sb.AppendLine();
                foreach(var week in weekDic)
                {
                    sb.AppendLine($"{week.Key}, {week.Value}");
                }
                sb.AppendLine();
                foreach (var date in dateDicDesc)
                {
                    sb.AppendLine($"{date.Key}, {date.Value}");
                }

                SaveFileDialog saveFile = new SaveFileDialog
                {
                    DefaultExt = "txt",
                    Title = "텍스트 파일을 어디에 저장할까요?"
                };
                if (saveFile.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(saveFile.FileName, sb.ToString());
                }
            }
            else return;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/tjgus1668/ArcaliveCrawler/releases");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog
            {
                DefaultExt = "dat",
                Title = "크롤링 데이터 파일을 여러 개 선택해주세요.",
                Multiselect = true
            };
            if(openFile.ShowDialog() == DialogResult.OK)
            {
                List<Post> CombinedPosts = new List<Post>();
                foreach(var file in openFile.FileNames)
                {
                    CombinedPosts.AddRange(ArcaliveCrawler.DeserializationPosts(file));
                }

                SaveFileDialog saveFile = new SaveFileDialog
                {
                    DefaultExt = "dat",
                    Title = "데이터 파일을 어디에 저장할까요?"
                };
                if (saveFile.ShowDialog() == DialogResult.OK)
                {
                    ArcaliveCrawler.SerializationPosts(CombinedPosts, saveFile.FileName);
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog
            {
                DefaultExt = "dat",
                Title = "크롤링 데이터 파일을 선택해주세요."
            };
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                var posts = ArcaliveCrawler.DeserializationPosts(openFile.FileName);

                Dictionary<string, int> arcaconDic = new Dictionary<string, int>();
                foreach (var post in posts)
                {
                    foreach (var comment in post.comments)
                    {
                        if (string.IsNullOrEmpty(comment.arcacon)) continue;

                        else if (arcaconDic.ContainsKey(comment.arcacon) == false)
                            arcaconDic.Add(comment.arcacon, 1);
                        else
                            arcaconDic[comment.arcacon]++;
                    }
                }
                var arcaconDicDesc = arcaconDic.OrderBy(x => x.Value);

                StringBuilder sb = new StringBuilder();

                sb.Append("//아카콘 랭킹\r\n");
                foreach (var dic in arcaconDicDesc)
                {
                    sb.Append($"{dic.Key}, {dic.Value}\r\n");
                }

                SaveFileDialog saveFile = new SaveFileDialog
                {
                    DefaultExt = "txt",
                    Title = "텍스트 파일을 어디에 저장할까요?"
                };
                if (saveFile.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(saveFile.FileName, sb.ToString());
                }
            }
            else return;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            DataFileSplitForm form = new DataFileSplitForm();
            form.StartPosition = FormStartPosition.CenterParent;
            form.Show();
        }
    }
}