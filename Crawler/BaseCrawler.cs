using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler
{
    public abstract class BaseCrawler
    {
        protected List<PostInfo> _posts = new List<PostInfo>();
        protected List<BoardFilter> _boardFilters = new List<BoardFilter>();
        protected List<PostFilter> _postFilters = new List<PostFilter>();
        protected string _baseLink;
        /// <summary>
        /// 크롤링을 시작하는 글의 정보, 시간 순으로 따지자면 EndInfo보다 이후의 글을 가르킵니다
        /// </summary>
        public PostInfo StartInfo { get; set; }
        /// <summary>
        /// 크롤링을 중단하는 글의 정보, 시간 순으로 따지자면 StartInfo보다 이전의 글을 가르킵니다
        /// </summary>
        public PostInfo EndInfo { get; set; }
        public PageFinder PageFinder { get; protected set; }
        public IEnumerable<PostInfo> Posts
        {
            get
            {
                foreach (var postInfo in _posts)
                {
                    yield return postInfo;
                }
            }
        }
        public virtual string BaseLink {
            get => _baseLink;
        }

        protected BaseCrawler(string baseLink)
        {
            SetBaseLink(baseLink);
        }

        protected abstract void SetBaseLink(string str);
        protected bool RunBoardFilters(PostInfo current)
        {
            return _boardFilters.All(postValidator => postValidator(current, this));
        }
        protected bool RunPostFilters(PostInfo current)
        {
            return _postFilters.All(postValidator => postValidator(current, this));
        }

        public abstract void CrawlBoards();
        public abstract void CrawlPosts();
        public abstract void ApplyBasicSettings();
    }
}
