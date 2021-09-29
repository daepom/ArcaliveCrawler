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
            BaseCrawler crawler = new ArcaliveCrawler("momoirocode");
            crawler.ApplyBasicSettings();
            var start = (PostInfo) DateTime.Now;
            var end = (PostInfo) DateTime.Now.AddMinutes(-120);
            crawler.StartInfo = start;
            crawler.EndInfo = end;
            crawler.CrawlBoards();
            crawler.CrawlPosts();
            Console.WriteLine(crawler.Posts.ToList().Count);
            Console.ReadKey();
        }
    }
}
