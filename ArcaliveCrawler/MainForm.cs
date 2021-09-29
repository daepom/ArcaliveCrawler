using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArcaliveForm;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace ArcaliveCrawler
{
    public partial class MainForm : Form
    {
        public string currentVersion = "2.0";
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            versionLabel.Text = currentVersion;
            #region 버전 체크

            try
            {
                string tmp = string.Empty;
                using (WebClient wc = new WebClient())
                {
                    tmp = wc.DownloadString("https://github.com/csh1668/ArcaliveCrawler/releases");
                }

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(tmp);
                var releases = doc.DocumentNode.SelectNodes("//div[contains(@class, 'release-entry')]");
                var latestRelease = releases[0].SelectSingleNode("//div[contains(@class, 'd-flex flex-items-start')]/div[1]/a").InnerText;

                if (currentVersion == latestRelease)
                    linkLabel1.Text = "최신 버전입니다";
                else
                    linkLabel1.Text = "업데이트 가능";
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                linkLabel1.Text = "GitHub 연결 실패";
            }

            #endregion
        }

        private void CrawlButton_Click(object sender, EventArgs e)
        {
            Form f = new CrawlForm()
            {
                StartPosition = FormStartPosition.CenterParent
            };
            f.ShowDialog();
        }

        private void StatExportButton_Click(object sender, EventArgs e)
        {
            Form f = new RankExportForm()
            {
                StartPosition = FormStartPosition.CenterParent
            };
            f.ShowDialog();
        }
    }
}
