using HtmlAgilityPack;
using System;
using System.Collections.Generic;

namespace Crawler
{
    public delegate int PageFinder(ArcaliveCrawler crawler, PostInfo info);

    public delegate bool PostValidator(PostInfo target, PostInfo start, PostInfo end);
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

        protected virtual void DoExtraConvert()
        {

        }

        public static explicit operator PostInfo(HtmlNode node)
        {
            var result = new PostInfo {boardSource = node};
            result.DoExtraConvert();
            return result;
        }
    }

    [Serializable]
    public class ArcalivePostInfo : PostInfo
    {
        public int id;
        public string badge;

        protected override void DoExtraConvert()
        {
            base.DoExtraConvert();
            var s = boardSource.Attributes["href"]?.Value;
            if (s != null)
            {
                href = "https://arca.live" + s.Substring(0, s.LastIndexOf('?'));
            }
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class RequiresCrawlPost : Attribute
    {

    }
}