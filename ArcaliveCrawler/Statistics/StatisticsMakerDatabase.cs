using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ArcaliveCrawler.Statistics
{
    public static class StatisticsMakerDatabase
    {
        private static List<StatisticsMaker> _list = new List<StatisticsMaker>();

        public static void InitDatabase()
        {
            var sMakers = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.IsClass && x.Name.StartsWith(nameof(StatisticsMaker)) && !x.IsAbstract);
            foreach (var sMaker in sMakers)
            {
                _list.Add((StatisticsMaker)Activator.CreateInstance(sMaker));
                Console.WriteLine(sMaker.Name);
            }
        }

        public static IEnumerable<StatisticsMaker> StatisticsMakers => _list;

        public static StatisticsMaker GetNamed(string name)
        {
            foreach (var maker in _list)
            {
                if (maker.Name == name)
                    return maker;
            }

            return null;
        }
    }
}
