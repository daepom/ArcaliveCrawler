using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Crawler
{
    public class ArcaliveCrawler
    {
        public string BaseLink { get; private set; }

        public ArcaliveCrawler(string channelName)
        {
            BaseLink = "https://arca.live/b/" + channelName;
        }

        public IEnumerable<HtmlNode> CrawlBoards(int startPage = 1)
        {
            while (true)
            {
                HtmlDocument doc = ArcaliveDocDownloader.DownloadDoc(BaseLink + "?p=" + startPage);
            }
        }
    }
}
