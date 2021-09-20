using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crawler;

namespace CrawlTest
{
    class Program
    {
        static void Main(string[] args)
        {
            ArcaliveCrawler ac = new ArcaliveCrawler("momoirocode");
            ac.ApplyBasicSettings();
            var start = new PostInfo() { dt = DateTime.Now };
            var end = new PostInfo() { dt = DateTime.Now.AddHours(-1) }; 
            List<PostInfo> posts = new List<PostInfo>();
            posts.AddRange(ac.CrawlBoards(start, end));
            ac.CrawlPosts(posts);
            Console.WriteLine(posts.Count+"End");
            Console.ReadKey();
        }
    }
}
