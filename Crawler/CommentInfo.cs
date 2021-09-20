using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler
{
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
