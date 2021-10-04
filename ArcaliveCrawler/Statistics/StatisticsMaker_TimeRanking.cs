using System;
using System.Collections.Generic;
using System.Linq;
using Crawler;

namespace ArcaliveCrawler.Statistics
{
    public abstract class StatisticsMaker_TimeRanking : StatisticsMaker
    {
        public StatisticsMaker_TimeRanking() : base()
        {
        }

        public StatisticsMaker_TimeRanking(List<PostInfo> posts) : base(posts)
        {

        }

        protected class StatCount
        {
            public int post;
            public int comment;
            public int PostAndComment => post + comment;

            public StatCount(int post, int comment)
            {
                this.post = post;
                this.comment = comment;
            }
        }
    }

    public class StatisticsMaker_TimeRankingByTime : StatisticsMaker_TimeRanking
    {
        public override string Name { get; } = "시간별 글, 댓글";

        public StatisticsMaker_TimeRankingByTime() : base()
        {
        }
        public StatisticsMaker_TimeRankingByTime(List<PostInfo> posts) : base(posts)
        {

        }

        public override ArcaliveCrawler.Statistics.Statistics MakeStatistics()
        {
            var stat = new ArcaliveCrawler.Statistics.Statistics("시각", "글", "댓글", "글+댓글") { Name = this.Name };
            foreach (var count in CountTimeCounts().OrderBy(x => x.Key))
            {
                var value = count.Value;
                stat.AddRow(count.Key, value.post, value.comment, value.PostAndComment);
            }

            return stat;
        }

        private Dictionary<string, StatCount> CountTimeCounts()
        {
            var dic = new Dictionary<string, StatCount>();
            foreach (var postInfo in Posts)
            {
                string hour = postInfo.dt.ToString("HH");
                if (dic.ContainsKey(hour) == false)
                    dic.Add(hour, new StatCount(1, 0));
                else
                {
                    var tmp = dic[hour];
                    tmp.post++;
                }

                foreach (var commentInfo in postInfo.comments)
                {
                    hour = commentInfo.dt.ToString("HH");
                    if (dic.ContainsKey(hour) == false)
                        dic.Add(hour, new StatCount(0, 1));
                    else
                    {
                        var tmp = dic[hour];
                        tmp.comment++;
                    }
                }
            }

            return dic;
        }
    }

    public class StatisticsMaker_TimeRankingByDay : StatisticsMaker_TimeRanking
    {
        public override string Name { get; } = "요일별 글, 댓글";

        public StatisticsMaker_TimeRankingByDay() : base()
        {
        }
        public StatisticsMaker_TimeRankingByDay(List<PostInfo> posts) : base(posts)
        {

        }

        public override Statistics MakeStatistics()
        {
            var stat = new Statistics("요일", "글", "댓글", "글+댓글") { Name = this.Name };
            var dic = CountTimeCounts();

            void AddRow(string str)
            {
                if(dic.TryGetValue(str, out var value))
                    stat.AddRow(str, value.post, value.comment, value.PostAndComment);
            }

            AddRow("월요일"); AddRow("화요일"); AddRow("수요일"); AddRow("목요일");
            AddRow("금요일"); AddRow("토요일"); AddRow("일요일");

            return stat;
        }

        private Dictionary<string, StatCount> CountTimeCounts()
        {
            var dic = new Dictionary<string, StatCount>();
            foreach (var postInfo in Posts)
            {
                string hour = DayOfWeekToStringKorean(postInfo.dt.DayOfWeek);
                if (dic.ContainsKey(hour) == false)
                    dic.Add(hour, new StatCount(1, 0));
                else
                {
                    var tmp = dic[hour];
                    tmp.post++;
                }

                foreach (var commentInfo in postInfo.comments)
                {
                    hour = DayOfWeekToStringKorean(commentInfo.dt.DayOfWeek);
                    if (dic.ContainsKey(hour) == false)
                        dic.Add(hour, new StatCount(0, 1));
                    else
                    {
                        var tmp = dic[hour];
                        tmp.comment++;
                    }
                }
            }

            return dic;
        }

        public static string DayOfWeekToStringKorean(DayOfWeek week)
        {
            switch (week)
            {
                case DayOfWeek.Sunday:
                    return "일요일";

                case DayOfWeek.Monday:
                    return "월요일";

                case DayOfWeek.Tuesday:
                    return "화요일";

                case DayOfWeek.Wednesday:
                    return "수요일";

                case DayOfWeek.Thursday:
                    return "목요일";

                case DayOfWeek.Friday:
                    return "금요일";

                case DayOfWeek.Saturday:
                    return "토요일";

                default:
                    throw new ArgumentOutOfRangeException(nameof(week), week, null);
            }
        }
    }

    public class StatisticsMaker_TimeRankingByDate : StatisticsMaker_TimeRanking
    {
        public override string Name { get; } = "날짜별 글 댓글";

        public StatisticsMaker_TimeRankingByDate() : base()
        {
        }

        public StatisticsMaker_TimeRankingByDate(List<PostInfo> posts) : base(posts)
        {

        }

        public override ArcaliveCrawler.Statistics.Statistics MakeStatistics()
        {
            var stat = new ArcaliveCrawler.Statistics.Statistics("날짜", "글", "댓글", "글+댓글") {Name = this.Name};
            foreach (var count in CountTimeCounts().OrderBy(x => x.Key))
            {
                var value = count.Value;
                stat.AddRow(count.Key+"일", value.post, value.comment, value.PostAndComment);
            }

            return stat;
        }

        private Dictionary<int, StatCount> CountTimeCounts()
        {
            var dic = new Dictionary<int, StatCount>();
            foreach (var postInfo in Posts)
            {
                int day = postInfo.dt.Day;
                if (dic.ContainsKey(day) == false)
                    dic.Add(day, new StatCount(1, 0));
                else
                {
                    var tmp = dic[day];
                    tmp.post++;
                }

                foreach (var commentInfo in postInfo.comments)
                {
                    day = commentInfo.dt.Day;
                    if (dic.ContainsKey(day) == false)
                        dic.Add(day, new StatCount(0, 1));
                    else
                    {
                        var tmp = dic[day];
                        tmp.comment++;
                    }
                }
            }

            return dic;
        }
    }
}