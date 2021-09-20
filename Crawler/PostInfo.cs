using HtmlAgilityPack;
using System;
using System.Collections.Generic;

namespace Crawler
{
    public delegate int PageFinder(ArcaliveCrawler crawler, PostInfo info);

    public delegate bool BoardValidator(PostInfo target, object[] args);

    public delegate bool PostValidator(PostInfo target, object[] args);
    public class PostInfo
    {
        [NonSerialized] public HtmlNode boardSource;
        [NonSerialized, RequiresCrawlPost] public HtmlNode postSource;
        public string href;
        public DateTime dt;
        [RequiresCrawlPost]public List<CommentInfo> comments = new List<CommentInfo>();
        public string author;
        public string title;
        [RequiresCrawlPost]public string content;


        public static explicit operator PostInfo(HtmlNode node)
        {
            var result = new PostInfo {boardSource = node};
            return result;
        }
    }

    [Serializable]
    public class ArcalivePostInfo : PostInfo
    {
        public int id;
        public string badge;
        public int view;
        public bool restricted;

        public static explicit operator ArcalivePostInfo(HtmlNode node)
        {
            var result = new ArcalivePostInfo { boardSource = node };
            string href = result.boardSource.Attributes["href"]?.Value;
            if (href != null)
                result.href = "https://arca.live" + href.Substring(0, href.LastIndexOf('?'));
            return result;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class RequiresCrawlPost : Attribute
    {

    }
}