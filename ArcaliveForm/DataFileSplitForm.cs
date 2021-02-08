using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Arcalive;

namespace ArcaliveForm
{
    public partial class DataFileSplitForm : Form
    {
        public DataFileSplitForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DateTime splitTime = dateTimePicker1.Value.Date + dateTimePicker2.Value.TimeOfDay;
            OpenFileDialog openFile = new OpenFileDialog
            {
                DefaultExt = "dat",
                Title = "분리할 크롤링 데이터 파일을 선택해주세요.",
                Filter = "크롤링 데이터 파일 (*.dat)|*.dat"
            };
            if(openFile.ShowDialog() == DialogResult.OK)
            {
                var first = new List<Post>();
                var second = new List<Post>();
                var original = ArcaliveCrawler.DeserializePosts(openFile.FileName);

                foreach(var post in original)
                {
                    if (post.time <= splitTime)
                        first.Add(post);
                    else
                        second.Add(post);
                }

                SaveFileDialog saveFile1 = new SaveFileDialog
                {
                    DefaultExt = "dat",
                    Title = "데이터 파일을 어디에 저장할까요?",
                    Filter = "크롤링 데이터 파일 (*.dat)|*.dat"
                };
                SaveFileDialog saveFile2 = new SaveFileDialog
                {
                    DefaultExt = "dat",
                    Title = "데이터 파일을 어디에 저장할까요?",
                    Filter = "크롤링 데이터 파일 (*.dat)|*.dat"
                };
                if (saveFile2.ShowDialog() == DialogResult.OK)
                {
                    ArcaliveCrawler.SerializePosts(second, saveFile2.FileName);
                }
                if (saveFile1.ShowDialog() == DialogResult.OK)
                {
                    ArcaliveCrawler.SerializePosts(first, saveFile1.FileName);
                }

                MessageBox.Show("저장 완료!");
            }
        }
    }
}
