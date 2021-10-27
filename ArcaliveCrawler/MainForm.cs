using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArcaliveCrawler.Utils;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace ArcaliveCrawler
{
    public partial class MainForm : Form
    {
        public string currentVersion = "2.0.1";
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            var builtTime = System.IO.File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location).ToString("yyyy/MM/dd HH시mm분에 빌드됨");
            versionLabel.Text = currentVersion;
            builtTimeLabel.Text = builtTime;
            #region 버전 체크

            try
            {
                GithubVersionChecker vc =
                    new GithubVersionChecker("https://github.com/csh1668/ArcaliveCrawler/releases");
                string latestRelease = vc.LatestRelease;
                Console.WriteLine(latestRelease);

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

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/csh1668/ArcaliveCrawler/releases");
        }

        private void ArcaconUseStatExportButton_Click(object sender, EventArgs e)
        {
            Form f = new ArcaconUseStatForm()
            {
                StartPosition = FormStartPosition.CenterParent
            };
            f.ShowDialog();
        }
    }
}
