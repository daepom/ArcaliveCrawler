using System;

namespace Crawler
{
    [Serializable]
    public class CommentInfo
    {
        [NonSerialized] public PostInfo parentPost;
        public DateTime dt;
        public string author;
        public string content;
    }

    [Serializable]
    public class ArcaliveCommentInfo : CommentInfo
    {
        public bool isArcacon;
        public int dataId;
    }
}