﻿using System.Collections.Generic;
using System.Text;
using Crawler;

namespace ArcaliveCrawler.Statistics
{
    public abstract class StatisticsMaker
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