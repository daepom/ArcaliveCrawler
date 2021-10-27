using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ArcaliveCrawler.Statistics
{
    public static class GenDatabase<T> where T : INameable
    {
        private static List<T> _list = new List<T>();

        public static void InitDatabase()
        {
            _list.Clear();
            var sMakers = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.IsClass && x.Name.StartsWith(typeof(T).Name) && !x.IsAbstract);
            foreach (var sMaker in sMakers)
            {
                _list.Add((T)Activator.CreateInstance(sMaker));
                Console.WriteLine(sMaker.Name);
            }
        }

        public static IEnumerable<T> StatisticsMakers => _list;

        public static T GetNamed(string name)
        {
            return _list.FirstOrDefault(maker => maker.Name == name);
        }
    }

    public interface INameable
    {
        string Name { get; }
    }
}
