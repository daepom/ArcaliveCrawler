using System;
using System.Collections.Generic;
using System.Linq;
using Crawler;

namespace ArcaliveCrawler.Statistics
{
    public class StatisticsMaker_UserRanking : StatisticsMaker
    {
        public override string Name { get; } = "갤창랭킹";

        public StatisticsMaker_UserRanking() : base()
        {
        }

        public StatisticsMaker_UserRanking(List<PostInfo> posts) : base(posts)
        {

        }

        public override Statistics MakeStatistics()
        {
            var stat = new Statistics("작성자", "글", "댓글", "갤창력", "플레이타임(시)")
            {
                Name = this.Name
            };

            var dic = CountUserStat();

            foreach (var user in dic.OrderByDescending(x => x.Value.Power))
            {
                var statCount = user.Value;
                stat.AddRow(user.Key, statCount.post, statCount.comment, statCount.Power, Math.Round(statCount.PlayTime,2));
            }


            return stat;
        }

        private Dictionary<string, StatCount> CountUserStat()
        {
            var rankDic = new Dictionary<string, StatCount>();
            foreach (var post in Posts)
            {
                foreach (var comment in post.comments)
                {
                    if (rankDic.ContainsKey(comment.author) == false)
                        rankDic.Add(comment.author, new StatCount(0, 1, comment.dt));
                    else
                    {
                        var user = rankDic[comment.author];
                        user.comment++;
                        user.times.Add(comment.dt);
                    }
                }

                if (rankDic.ContainsKey(post.author) == false)
                    rankDic.Add(post.author, new StatCount(1, 0, post.dt));
                else
                {
                    var user = rankDic[post.author];
                    user.post++;
                    user.times.Add(post.dt);
                }
            }

            return rankDic;
        }

        private class StatCount
        {
            public int post;
            public int comment;
            private double? playTimeCached;
            public List<DateTime> times;

            public StatCount(int post, int comment, DateTime dt)
            {
                this.post = post;
                this.comment = comment;
                times = new List<DateTime>() {dt};
            }

            public double PlayTime
            {
                get
                {
                    if (playTimeCached != null) return playTimeCached.Value;
                    const int timeSpan = 10;
                    var orderedTimes = times.OrderByDescending(x => x).ToList();
                    TimeSpan activeTime = TimeSpan.Zero;

                    if (orderedTimes.Count <= 1)
                    {
                        activeTime += TimeSpan.FromMinutes(timeSpan);
                    }
                    else
                    {
                        for (int i = 0; i < orderedTimes.Count - 1; i++)
                        {
                            var diff = orderedTimes[i] - orderedTimes[i + 1];
                            if (diff <= TimeSpan.FromMinutes(timeSpan))
                            {
                                activeTime += diff;
                            }
                            else
                            {
                                activeTime += TimeSpan.FromMinutes(timeSpan);
                            }
                        }
                    }

                    playTimeCached = activeTime.TotalHours;
                    return playTimeCached.Value;
                }
            }

            public int Power => post * 100 + comment * 30;
        }
    }
}