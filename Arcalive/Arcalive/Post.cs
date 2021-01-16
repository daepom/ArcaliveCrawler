using System;
using System.Collections.Generic;

namespace Arcalive
{
    [Serializable]
    public struct Post
    {
        public string link;
        public int id;
        public string badge;
        public string title;
        public string author;
        public DateTime time;
        public int view;
        public int rate;
        public List<Comment> comments;
    }
}