using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crawler;

namespace ArcaliveCrawler.Statistics
{
    public class StatisticsMaker_ViewByBadge : StatisticsMaker
    {
        public override string Name { get; } = "말머리별 평균 조회수";

        public StatisticsMaker_ViewByBadge() : base()
        {
        }

        public StatisticsMaker_ViewByBadge(List<PostInfo> posts) : base(posts)
        {
        }
        public override Statistics MakeStatistics()
        {
            Statistics stat = new Statistics("말머리", "평균 조회수") {Name = Name};

            foreach (var i in Count().OrderByDescending(x => x.Value.AverageView))
            {
                string badge = i.Key;
                if (string.IsNullOrEmpty(badge))
                    badge = "(말머리 없음)";
                stat.AddRow(badge, Math.Round(i.Value.AverageView,2));
            }

            return stat;

        }

        private Dictionary<string, StatCount> Count()
        {
            Dictionary<string, StatCount> viewByBadge = new Dictionary<string, StatCount>();
            foreach (var post in Posts.Cast<ArcalivePostInfo>())
            {
                string badge = post.badge;
                int view = post.view;
                if (viewByBadge.ContainsKey(badge) == false)
                {
                    viewByBadge.Add(badge, new StatCount(view));
                }

                viewByBadge[badge].AddView(view);
            }

            return viewByBadge;
        }

        private class StatCount
        {
            public int OverallView { get; private set; }
            public int Count { get; private set; }

            public StatCount(int view)
            {
                OverallView = view;
                Count = 1;
            }
            public double AverageView => OverallView / (double) Count;

            public void AddView(int view)
            {
                OverallView += view;
                Count++;
            }
        }
    }
}
