using System;
using System.Collections.Generic;
using System.Linq;

namespace ArcaliveCrawler.Utils
{
    public class EstimatedTimeCalculator
    {
        private Queue<double> times = new Queue<double>();

        public void Enqueue(double item)
        {
            if (times.Count >= 100)
                times.Dequeue();
            times.Enqueue(item);
            AccumulatedCount++;
        }

        public double AverageTimePerOne => times.Sum() / (times.Count == 0 ? 1 : times.Count);

        public double EstimatedTime
        {
            get
            {
                double result = (TargetCount - AccumulatedCount) * AverageTimePerOne;
                return result < 0 ? 0 : result;
            }
        }

        public int AccumulatedCount { get; private set; }
        public int TargetCount { get; set; }
    }
}