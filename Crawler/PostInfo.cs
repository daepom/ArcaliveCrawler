using HtmlAgilityPack;
using System;
using System.Collections.Generic;

namespace Crawler
{
    public delegate int PageFinder(BaseCrawler crawler);

    public delegate bool BoardFilter(PostInfo target, BaseCrawler crawler);

    public delegate bool PostFilter(PostInfo target, BaseCrawler crawler);

    [Serializable]
    public class PostInfo : IEquatable<PostInfo>
    {
        [NonSerialized] public HtmlNode boardSource;
        [NonSerialized, RequiresCrawlPost] public HtmlNode postSource;
        public string href;
        public DateTime dt;
        [RequiresCrawlPost]public List<CommentInfo> comments = new List<CommentInfo>();
        public string author;
        public string title;
        [RequiresCrawlPost]public string content;
        [NonSerialized] public bool completed;


        public static explicit operator PostInfo(HtmlNode node)
        {
            var result = new PostInfo {boardSource = node};
            return result;
        }

        public static explicit operator PostInfo(DateTime dt)
        {
            var result = new PostInfo {dt = dt};
            return result;
        }

        public void ClearSourceData()
        {
            boardSource = null;
            postSource = null;
        }

        public bool Equals(PostInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return href == other.href && dt.Equals(other.dt) && Equals(comments, other.comments) && author == other.author && title == other.title && content == other.content;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PostInfo) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (href != null ? href.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ dt.GetHashCode();
                hashCode = (hashCode * 397) ^ (comments != null ? comments.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (author != null ? author.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (title != null ? title.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (content != null ? content.GetHashCode() : 0);
                return hashCode;
            }
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
            return result;
        }

        public override bool Equals(object obj)
        {
            return obj is ArcalivePostInfo a && Equals(a);
        }

        protected bool Equals(ArcalivePostInfo other)
        {
            return base.Equals(other) && id == other.id && badge == other.badge && view == other.view && restricted == other.restricted;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ id;
                hashCode = (hashCode * 397) ^ (badge != null ? badge.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ view;
                hashCode = (hashCode * 397) ^ restricted.GetHashCode();
                return hashCode;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class RequiresCrawlPost : Attribute
    {

    }
}