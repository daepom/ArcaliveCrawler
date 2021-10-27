using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Crawler;

namespace ArcaliveCrawler.Utils
{
    public partial class WordCloudTextExportForm : Form
    {
        public WordCloudTextExportForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<PostInfo> posts;
            OpenFileDialog openFile = new OpenFileDialog
            {
                DefaultExt = "dat",
                Title = "크롤링 데이터 파일을 선택해주세요.",
                Filter = "크롤링 데이터 파일 (*.dat)|*.dat"
            };
            if (openFile.ShowDialog() != DialogResult.OK) return;
            posts = DataFileUtility.DeserializePosts(openFile.FileName);

            StringBuilder sb = new StringBuilder();
            foreach (var post in posts)
            {
                if (checkBox1.Checked)
                    sb.AppendLine(post.title);
                if (checkBox2.Checked)
                    sb.AppendLine(post.content);
                if (checkBox3.Checked)
                {
                    foreach (var comment in post.comments.Cast<ArcaliveCommentInfo>().Where(x => x.isArcacon == false))
                    {
                        sb.AppendLine(comment.content);
                    }
                }
            }

            SaveFileDialog saveFile = new SaveFileDialog
            {
                DefaultExt = "txt",
                Title = "텍스트 파일을 어디에 저장할까요?",
                Filter = "텍스트 파일 (*.txt)|*.txt"
            };
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveFile.FileName, sb.ToString(), Encoding.UTF8);
            }
        }
    }
}
