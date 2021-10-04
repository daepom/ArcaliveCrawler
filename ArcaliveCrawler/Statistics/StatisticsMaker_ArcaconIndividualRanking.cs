using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crawler;

namespace ArcaliveCrawler.Statistics
{
    public class StatisticsMaker_ArcaconIndividualRanking : StatisticsMaker
    {
        public override string Name { get; } = "아카콘 개별";

        public StatisticsMaker_ArcaconIndividualRanking() { }

        public StatisticsMaker_ArcaconIndividualRanking(List<PostInfo> posts) : base(posts) { }

        public override Statistics MakeStatistics()
        {
            var stat = new Statistics("아카콘 주소", "사용 횟수", "주작 지수");
            var dic = CountStatCount();

            foreach (var pair in dic.OrderByDescending(x => x.Value.Usage))
            {
                var statCount = pair.Value;
                stat.AddRow("https:" + pair.Key, statCount.Usage, Math.Round(statCount.Maliciousness, 2));
            }

            stat.Name = Name;
            stat.Description = "주작 지수 = (해당 아카콘을 사용한 유저들 중, 사용률이 높은 10% 유저들의 사용 횟수) / 전체 사용 횟수";
            return stat;
        }

        private Dictionary<string, StatCount> CountStatCount()
        {
            Dictionary<string, StatCount> arcaconDic = new Dictionary<string, StatCount>();
            foreach (var comment in Posts.SelectMany(post => post.comments))
            {
                if (((ArcaliveCommentInfo)comment).isArcacon == false) continue;
                if (arcaconDic.ContainsKey(comment.content) == false)
                {
                    var count = new StatCount();
                    Count(count.ByUsers, comment.author);
                    arcaconDic.Add(comment.content, count);
                }
                else
                {
                    var count = arcaconDic[comment.content];
                    Count(count.ByUsers, comment.author);
                }
            }

            return arcaconDic;
        }

        private void Count(Dictionary<string, int> target, string str)
        {
            if(target.ContainsKey(str) == false)
                target.Add(str, 1);
            else
            {
                target[str]++;
            }
        }

        private class StatCount
        {
            public Dictionary<string, int> ByUsers { get; } = new Dictionary<string, int>();

            public int Usage => ByUsers.Values.Sum();

            public double Maliciousness
            {
                // 시그모이드 활용
                get
                {
                    int users = ByUsers.Count;
                    double result = 0d;
                    var tmp = ByUsers.OrderByDescending(x => x.Value).ToList();
                    for (int i = 0; i < users; i++)
                    {
                        if (users / 10d <= i)
                            break;
                        result += tmp[i].Value;
                    }

                    result /= Usage;
                    return result;
                }
            }

            public StatCount() { }
        }
    }
}
