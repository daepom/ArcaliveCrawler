using Arcalive;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArcaliveForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        void WriteLog(object sender, EventArgs arg)
        {
            Invoke((Action)(() => {
                textBox2.AppendText((arg as PrintCallbackArg).Str + "\r\n");
            }));
        }

        StringBuilder sb;
        void DumpText(object sender, EventArgs arg)
        {
            Invoke((Action)(() =>
            {
                if(checkBox1.Checked) sb.AppendLine((arg as PrintCallbackArg).Str);
            }));
        }


        // 크롤링
        private async void button1_ClickAsync(object sender, EventArgs e)
        {
            sb = new StringBuilder();
            var channel = Arcalive.Arcalive.GetChannelLinks(channelNameTextBox.Text);
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

            Arcalive.Arcalive ac = new Arcalive.Arcalive(channel[0]);
            DateTime startDate = dateTimePicker1.Value.Date + dateTimePicker2.Value.TimeOfDay;
            DateTime endDate = dateTimePicker3.Value.Date + dateTimePicker4.Value.TimeOfDay;

            ac.DumpText += DumpText;
            ac.Print += WriteLog;

            List<Post> posts = new List<Post>();
            Task task = Task.Factory.StartNew(() =>
            {
                posts = ac.GetPosts(startDate, endDate, true, int.Parse(textBox1.Text));
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
            Arcalive.Arcalive.SerializationPosts(posts, filename);
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
                var posts = Arcalive.Arcalive.DeserializationPosts(openFile.FileName);

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
                var posts = Arcalive.Arcalive.DeserializationPosts(openFile.FileName);

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
                var posts = Arcalive.Arcalive.DeserializationPosts(openFile.FileName);

                StringBuilder sb = new StringBuilder();
                var timeDic = new Dictionary<string, int>();
                var weekDic = new Dictionary<string, int>();

                foreach(var post in posts)
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
                }

                var timeDicDesc = timeDic.OrderBy(x => x.Key);
                var weekDicDesc = timeDic.OrderBy(x => x.Key);

                foreach(var time in timeDicDesc)
                {
                    sb.AppendLine($"{time.Key}, {time.Value}");
                }
                sb.AppendLine();
                foreach(var week in weekDic)
                {
                    sb.AppendLine($"{week.Key}, {week.Value}");
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
    }
}