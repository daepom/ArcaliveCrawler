using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArcaliveForm
{
    public partial class OptionsForm : Form
    {
        public event EventHandler OptionsCallBack;

        public OptionsForm()
        {
            InitializeComponent();
            
        }

        private void OptionsForm_Load(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(PFASetPageButton, "오른쪽에 입력한 페이지부터 크롤링을 시작합니다.");
            toolTip1.SetToolTip(PFABinaryRadioButton, "이진탐색 알고리즘을 사용하여 날짜와 일치하는 페이지를 찾고, 크롤링을 시작합니다.");
            
            OptionsClass options = (this.Owner as Form1).Options;
            if (options.StartPageFinding == 1) PFASetPageButton.Checked = true;
            else PFABinaryRadioButton.Checked = true;
            textBox1.Text = options.StartPage.ToString();
        }

        private void SaveOptionsButton_Click(object sender, EventArgs e)
        {
            int pageFindingAlgorithm = PFASetPageButton.Checked ? 1 : 2,
                startPage = int.Parse(textBox1.Text);
            var str = textBox2.Text.Split(',');
            var skip = str.ToList();

            OptionsClass options = new OptionsClass()
            {
                StartPageFinding = pageFindingAlgorithm,
                StartPage = startPage,
                SkippingTags = skip
            };
            OptionsCallBack?.Invoke(this, new OptionsCallBack(options));
            Close();
        }

        private void ResetOptionsButton_Click(object sender, EventArgs e)
        {
            PFASetPageButton.Checked = true;
            textBox1.Text = "1";
            textBox2.Text = "신문고,";
        }
    }
}
