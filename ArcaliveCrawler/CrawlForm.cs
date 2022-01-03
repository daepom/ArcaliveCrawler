using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArcaliveCrawler.Utils;
using Crawler;

namespace ArcaliveCrawler
{
    public partial class CrawlForm : Form
    {
        private EstimatedTimeCalculator ecal = new EstimatedTimeCalculator();
        private Stopwatch sw = new Stopwatch();
        private List<PostInfo> posts = null;
        public CrawlForm()
        {
            InitializeComponent();
        }

        private void CrawlForm_Load(object sender, EventArgs e)
        {
            dateTimePicker3.Value = dateTimePicker3.Value.AddDays(-1);
            crawlProgressBar.Style = ProgressBarStyle.Continuous;
            crawlProgressBar.Minimum = 0;
            crawlProgressBar.Step = 1;
            crawlProgressBar.Value = 0;
        }

        private async void CrawlStartButton_ClickAsync(object sender, EventArgs e)
        {
            string channelName = channelNameTextBox.Text;
            DateTime start = dateTimePicker1.Value.Date + dateTimePicker2.Value.TimeOfDay,
                end = dateTimePicker3.Value.Date + dateTimePicker4.Value.TimeOfDay;

            BaseCrawler crawler = new Crawler.ArcaliveCrawler(channelName);
            crawler.Logger.LogEventHandler += UpdateLog;
            crawler.ApplyBasicSettings();
            crawler.StartInfo = (PostInfo) start;
            crawler.EndInfo = (PostInfo) end;

            if (string.IsNullOrEmpty(channelNameTextBox.Text) || !ArcaliveCrawlerUtility.TestCrawl(crawler.BaseLink, out string testResult))
            {
                MessageBox.Show("주소가 없거나 잘못되었습니다. 다시 확인해주세요", "에러");
                return;
            }

            if (start < end)
            {
                MessageBox.Show("잘못된 날짜 선택입니다." + Environment.NewLine + "'가장 최근 글 정보'는 '가장 오래된 글 정보'보다 이후 시간대여야 합니다. ", "에러");
                return;
            }
            if (start - end >= TimeSpan.FromDays(60))
            {
                if (MessageBox.Show("너무 긴 기간의 설정은 에러를 불러일으킬 수 있습니다. " + Environment.NewLine +
                                    "한 달 단위로 크롤링 한 뒤에, 데이터 파일 병합 기능의 사용을 권장합니다." + Environment.NewLine +
                                    "계속 하시겠습니까?", "경고", MessageBoxButtons.YesNo) ==
                    DialogResult.No)
                {
                    return;
                }
            }

            logTextBox.AppendText($"{testResult}에서 크롤링을 시작합니다.");

            var t = Task.Factory.StartNew(() =>
            {
                crawler.CrawlBoards();
                crawler.CrawlPosts();
            });
            CrawlStartButton.Enabled = false;
            await t;
            SaveButton.Enabled = true;
            posts = crawler.Posts.ToList();

            AlertForm af = new AlertForm {StartPosition = FormStartPosition.CenterScreen, TopMost = true};
            af.ShowDialog();
            BringToFront();
        }

        private void UpdateLog(object sender, CrawlLogMessageInfo info)
        {
            Invoke((Action) (() =>
            {
                sw.Stop();
                logTextBox.AppendText(info.ToString() + Environment.NewLine);
                int crawled = info.parent.CrawledPostsCount, all = info.parent.AllPostsCount;
                progressLabel.Text = $"{crawled} / {all}";
                crawlProgressBar.Maximum = all;
                crawlProgressBar.Value = crawled;
                ecal.Enqueue(sw.Elapsed.TotalMilliseconds/1000);

                if (crawled > 10)
                {
                    ecal.TargetCount = all;
                    progressLabel.Text += $", {Math.Round(ecal.EstimatedTime, 1)}초 남음";
                }

                sw.Reset();
                sw.Start();
            }));
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog()
            {
                DefaultExt = "dat",
                Title = "데이터 파일을 어디에 저장할까요?",
                Filter = "크롤링 데이터 파일 (*.dat)|*.dat"
            };
            if (sf.ShowDialog() == DialogResult.OK)
            {
                DataFileUtility.SerializePosts(posts, sf.FileName);
                MessageBox.Show("저장되었습니다!");
                Close();
            }
        }
    }
}