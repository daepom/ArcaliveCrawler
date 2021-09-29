using ArcaliveForm;
using Crawler;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArcaliveCrawler.Utils
{
    public partial class ProgressArcaconRankForm : Form
    {
        private List<PostInfo> posts;
        public string Result = String.Empty;

        public ProgressArcaconRankForm(List<PostInfo> posts)
        {
            InitializeComponent();
            this.posts = posts;
        }

        private async void ProgressForm_Load(object sender, EventArgs e)
        {
            var t = Task.Factory.StartNew(() =>
            {
                Result = Do();
            });
            await t;
            Close();
        }

        private void Step(int max)
        {
            Invoke((Action)(() =>
            {
                progressBar1.Maximum = max;
                progressBar1.Minimum = 0;
                progressBar1.Step = 1;
                progressBar1.PerformStep();
            }));
        }

        private string Do()
        {
            var arcacons = new Dictionary<Arcacon, int>();
            foreach (var a in from post in posts from comment in post.comments where ((ArcaliveCommentInfo)comment).isArcacon select new Arcacon(((ArcaliveCommentInfo)comment).dataId, comment.content))
            {
                if (arcacons.ContainsKey(a) == false)
                {
                    arcacons.Add(a, 1);
                }
                else
                {
                    arcacons[a]++;
                }
            }

            // ID, 실제 링크
            var memoDic = new Dictionary<Arcacon, int>();
            // 어디 아카콘 종류인지 저장하는 메모 리스트
            var arcaconPacks = new List<ArcaconPack>();
            foreach (var currentArcacon in arcacons.Select(arcacon => arcacon.Key))
            {
                if (memoDic.ContainsKey(currentArcacon) == false)
                {
                    Step(arcacons.Count);
                    // 메모이제이션
                    foreach (var arcaconPack in from arcaconPack in arcaconPacks from packArcacon in arcaconPack.arcacons where currentArcacon.address == packArcacon.address select arcaconPack)
                    {
                        memoDic.Add(currentArcacon, arcaconPack.id);
                        goto OUT;
                    }

                    // 처음보는 아카콘일 경우

                    // 아카콘 추가
                    var redirectedUrl = ArcaliveDocDownloader.RedirectedUrl("https://arca.live/api/emoticon/shop/" + currentArcacon.dataid,
                        term: 100, doc: out var doc);
                    var number = int.Parse(redirectedUrl.Split('/').Last());
                    memoDic.Add(currentArcacon, number);

                    // 삭제된 아카콘일 경우
                    if (string.IsNullOrEmpty(doc.Text))
                        continue;

                    // 메모
                    var arcaconNodesImg = doc.DocumentNode.SelectNodes("//div[contains(@class, 'article-body')]/img");
                    var arcaconNodesVid = doc.DocumentNode.SelectNodes("//div[contains(@class, 'article-body')]/video");
                    var newArcaconPack = new ArcaconPack(number);
                    if (arcaconNodesImg != null)
                    {
                        foreach (var arcaconNode in arcaconNodesImg)
                        {
                            var src = arcaconNode.Attributes["src"].Value;
                            var newArcacon = new Arcacon(src);
                            newArcaconPack.arcacons.Add(newArcacon);
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
                            newArcaconPack.arcacons.Add(newArcacon);
                        }
                    }
                    arcaconPacks.Add(newArcaconPack);
                }

            OUT:;
            }

            // 실제 링크, 집계수
            var rankDic = new Dictionary<int, int>();
            foreach (var i in memoDic.Where(i => i.Value != -1))
            {
                if (rankDic.ContainsKey(i.Value) == false)
                {
                    rankDic.Add(i.Value, arcacons[i.Key]);
                }
                else
                {
                    rankDic[i.Value] += arcacons[i.Key];
                }
            }

            var rankDicDesc = rankDic.OrderByDescending(x => x.Value);

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("//아카콘 종류별 랭킹");
            sb.AppendLine($"아카콘 종류: {rankDicDesc.Count()}");
            //sb.AppendLine("삭제된 아카콘은 집계되지 않습니다.");
            sb.AppendLine("아카콘 링크, 총 사용 횟수");
            foreach (var dic in rankDicDesc)
            {
                sb.AppendLine($"{"https://arca.live/e/" + dic.Key}, {dic.Value}");
            }

            return sb.ToString();
        }
    }
}