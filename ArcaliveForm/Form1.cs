using Arcalive;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
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


        // 크롤링
        private async void button1_ClickAsync(object sender, EventArgs e)
        {
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

            ac.Print += WriteLog;

            List<Post> posts = new List<Post>();
            Task task = Task.Factory.StartNew(() =>
            {
                ac.GetPosts(startDate, endDate, true, int.Parse(textBox1.Text));
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

                string txt = string.Empty;

                txt += "//글\r\n";
                foreach (var dic in paDesc)
                {
                    txt += $"{dic.Key}, {dic.Value}\r\n";
                }
                txt += "\r\n//댓글\r\n";
                foreach (var comment in commentsDesc)
                {
                    txt += $"{comment.Key}, {comment.Value}\r\n";
                }

                SaveFileDialog saveFile = new SaveFileDialog
                {
                    DefaultExt = "txt",
                    Title = "텍스트 파일을 어디에 저장할까요?"
                };
                if (saveFile.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(saveFile.FileName, txt);
                }
            }
            else return;
        }
    }
}