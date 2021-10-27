using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Crawler;

namespace ArcaliveCrawler.Statistics
{
    public abstract class StatisticsMaker : INameable
    {
        public abstract string Name { get; }
        public List<PostInfo> Posts { get; set; }
        protected StringBuilder sb = new StringBuilder();

        public bool RequiresLongWork { get; protected set; } = false;

        public StatisticsMaker()
        {
        }

        public StatisticsMaker(List<PostInfo> posts)
        {
            Posts = posts;
        }

        public abstract Statistics MakeStatistics();


    }
}
