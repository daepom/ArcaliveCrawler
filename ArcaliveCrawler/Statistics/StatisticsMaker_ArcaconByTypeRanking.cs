using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crawler;

namespace ArcaliveCrawler.Statistics
{
    public class StatisticsMaker_ArcaconByTypeRanking : StatisticsMaker
    {
        public override string Name { get; } = "아카콘 종류별";

        public StatisticsMaker_ArcaconByTypeRanking() : base() { RequiresLongWork = true; }

        public StatisticsMaker_ArcaconByTypeRanking(List<PostInfo> posts) : base(posts) { RequiresLongWork = true; }

        public override Statistics MakeStatistics()
        {
            var dic = CountArcaconPack(CountArcacon());
            throw new NotImplementedException();
        }

        private Dictionary<Arcacon, int> CountArcacon()
        {
            var result = new Dictionary<Arcacon, int>();
            foreach (var a in Posts.SelectMany(x => x.comments).Select(x => (ArcaliveCommentInfo)x).Where(x => x.isArcacon).Select(x => new Arcacon(x.content, x.dataId.ToString())))
            {
                if (result.ContainsKey(a) == false)
                {
                    result.Add(a, 1);
                }
                else
                {
                    result[a]++;
                }
            }

            return result;
        }

        private Dictionary<int, int> CountArcaconPack(Dictionary<Arcacon, int> orig)
        {
            // ID, 실제 링크
            var memoDic = new Dictionary<Arcacon, int>();
            // 어디 아카콘 종류인지 저장하는 메모 리스트
            var arcaconPacks = new List<ArcaconMemo>();
            foreach (var currentArcacon in orig.Select(arcacon => arcacon.Key))
            {
                if (memoDic.ContainsKey(currentArcacon) == false)
                {
                    //Step(arcacons.Count);
                    // 메모이제이션
                    foreach (var arcaconPack in from arcaconPack in arcaconPacks from packArcacon in arcaconPack.contents where currentArcacon.link == packArcacon.link select arcaconPack)
                    {
                        memoDic.Add(currentArcacon, arcaconPack.id);
                        goto OUT;
                    }

                    // 처음보는 아카콘일 경우

                    // 아카콘 추가
                    var redirectedUrl = ArcaliveDocDownloader.RedirectedUrl("https://arca.live/api/emoticon/shop/" + currentArcacon.dataId,
                        term: 5, doc: out var doc);
                    var number = int.Parse(redirectedUrl.Split('/').Last());
                    memoDic.Add(currentArcacon, number);

                    // 삭제된 아카콘일 경우
                    if (string.IsNullOrEmpty(doc.Text))
                        continue;

                    // 메모
                    var arcaconNodesImg = doc.DocumentNode.SelectNodes("//div[contains(@class, 'article-body')]/img");
                    var arcaconNodesVid = doc.DocumentNode.SelectNodes("//div[contains(@class, 'article-body')]/video");
                    var newArcaconPack = new ArcaconMemo {id = number};
                    if (arcaconNodesImg != null)
                    {
                        foreach (var arcaconNode in arcaconNodesImg)
                        {
                            var src = arcaconNode.Attributes["src"].Value;
                            var newArcacon = new Arcacon(src);
                            newArcaconPack.contents.Add(newArcacon);
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
                            newArcaconPack.contents.Add(newArcacon);
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
                    rankDic.Add(i.Value, orig[i.Key]);
                }
                else
                {
                    rankDic[i.Value] += orig[i.Key];
                }
            }

            return rankDic;
        }

        private class Arcacon
        {
            public string link;
            public string dataId;

            public Arcacon(string link)
            {
                this.link = link;
            }

            public Arcacon(string link, string dataId) : this(link)
            {
                this.dataId = dataId;
            }

            public override bool Equals(object obj)
            {
                return obj is Arcacon a && Equals(a);
            }

            private bool Equals(Arcacon other)
            {
                return link == other.link && dataId == other.dataId;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((link != null ? link.GetHashCode() : 0) * 397) ^ (dataId != null ? dataId.GetHashCode() : 0);
                }
            }
        }
        private class ArcaconMemo
        {
            public int id;
            public List<Arcacon> contents = new List<Arcacon>();
        }
    }
}
