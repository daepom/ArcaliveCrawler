﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;

namespace Crawler
{
    public class ArcaliveCrawler : BaseCrawler
    {
        public override int AllPostsCount
        {
            get
            {
                return _posts.Where(x => ((ArcalivePostInfo) x)?.restricted == false).ToList().Count;
            }
        }

        public override int CrawledPostsCount
        {
            get
            {
                return _posts.Where(x => ((ArcalivePostInfo) x)?.completed == true).ToList().Count;
            }
        }

        public ArcaliveCrawler(string channelName) : base(channelName)
        {
        }

        public override void ApplyBasicSettings()
        {
            PageFinder = ArcaliveCrawlerUtility.PageFinder_BinarySearchPageByTime;
            _boardFilters.Clear();
            _boardFilters.AddRange(new List<BoardFilter>
            {
                ArcaliveCrawlerUtility.BoardFilter_SkipNotices, ArcaliveCrawlerUtility.BoardFilter_TestByDateTime
            });
            _postFilters.Clear();
            _postFilters.AddRange(new List<PostFilter>
            {
            });
        }
        protected override void SetBaseLink(string str)
        {
            if (str.StartsWith("https://arca.live/b/"))
                _baseLink = str;
            else
                _baseLink = "https://arca.live/b/" + str;
        }


        public override void CrawlBoards()
        {
            int currentPage = PageFinder(this);
            bool crawlNextPage;
            do
            {
                crawlNextPage = false;
                var posts = CrawlBoardAt(currentPage).Where(RunBoardFilters).ToList();
                Logger.Log(MethodBase.GetCurrentMethod().Name, currentPage);
                foreach (var postInfo in posts)
                {
                    if (_posts.Contains(postInfo))
                    {
                        continue;
                    }
                    _posts.Add(postInfo);
                    crawlNextPage = true;
                }
                currentPage++;
            } while (crawlNextPage);
        }

        private IEnumerable<PostInfo> CrawlBoardAt(int page)
        {
            if (page < 1 || page > 10000)
                throw new ArgumentOutOfRangeException(nameof(page), "page < 1 || page > 10000");
            

            var pageDoc = ArcaliveDocDownloader.DownloadDoc(BaseLink + $"?p={page}");
            if (string.IsNullOrEmpty(pageDoc.Text))
                yield break;

            foreach (var info in pageDoc.DocumentNode.SelectNodes("//div[contains(@class, 'list-table')]/a").Select(x => (ArcalivePostInfo)x))
            {
                info.ParsePostData();
                yield return info;
            }
        }

        public override void CrawlPosts()
        {
            var postInfos = _posts.Select(x => (ArcalivePostInfo)x).ToList();
            if (postInfos.Any(x => x.boardSource == null))
                throw new ArgumentException("Call CrawlBoards before call this method");
            Task t = null;
            foreach (var postInfo in postInfos)
            {
                var postDoc = ArcaliveDocDownloader.DownloadDoc(postInfo.href);
                if (string.IsNullOrEmpty(postDoc.Text)) continue;
                postInfo.postSource = postDoc.DocumentNode;
                t = Task.Factory.StartNew(() =>
                {
                    postInfo.ParsePostData();
                });
                Logger.Log(MethodBase.GetCurrentMethod().Name, postInfo.id);
            }

            t.Wait();
            Logger.Log(MethodBase.GetCurrentMethod().Name, "크롤링 완료!");
        }
    }
}