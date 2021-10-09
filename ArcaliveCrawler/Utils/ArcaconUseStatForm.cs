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
using ArcaliveCrawler.Statistics;
using Crawler;

namespace ArcaliveCrawler.Utils
{
    public partial class ArcaconUseStatForm : Form
    {
        public ArcaconUseStatForm()
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

            var doc = ArcaliveDocDownloader.DownloadDoc("https://arca.live/e/" + textBox1.Text);
            var arcaconNodesImg = doc.DocumentNode.SelectNodes("//div[contains(@class, 'article-body')]/div/img");
            var arcaconNodesVid = doc.DocumentNode.SelectNodes("//div[contains(@class, 'article-body')]/div/video");
            var pack = new ArcaconPack(int.Parse(textBox1.Text));
            if (arcaconNodesImg != null)
            {
                foreach (var arcaconNode in arcaconNodesImg)
                {
                    var src = arcaconNode.Attributes["src"].Value;
                    var newArcacon = new Arcacon(src);
                    Console.WriteLine(src);
                    pack.arcacons.Add(newArcacon);
                }
            }
            if (arcaconNodesVid != null)
            {
                foreach (var arcaconNode in arcaconNodesVid)
                {
                    var src = arcaconNode.Attributes["src"].Value.EndsWith("mp4")
                        ? arcaconNode.Attributes["src"].Value + ".gif"
                        : arcaconNode.Attributes["src"].Value;
                    var newArcacon = new Arcacon(src);
                    Console.WriteLine(src);
                    pack.arcacons.Add(newArcacon);
                }
            }

            StatisticsMaker sm = new StatisticsMaker_ArcaconIndividualRanking(posts);
            var st = sm.MakeStatistics();
            StringBuilder sb = new StringBuilder();

            Dictionary<string, int> resultDic = new Dictionary<string, int>();

            foreach (var arcacon in pack.arcacons)
            {
                var i = st.FirstOrDefault(x => x[0] == "https:" + arcacon.address);
                if (i != null)
                {
                    resultDic.Add(i[0], int.Parse(i[1]));
                }
                else
                    resultDic.Add("https:" + arcacon.address, 0);
            }

            foreach (var pair in resultDic.OrderByDescending(x => x.Value))
            {
                sb.AppendLine($"{pair.Key}, {pair.Value}");
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
