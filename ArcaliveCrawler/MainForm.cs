using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArcaliveForm;

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
            string checkedVersion = string.Empty;

            if (currentVersion == checkedVersion)
                versionCheckResultLabel.Text = "최신 버전입니다";
            else
                versionCheckResultLabel.Text = "업데이트 가능";

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
