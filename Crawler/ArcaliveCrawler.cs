using System;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Crawler
{
    public class ArcaliveCrawler
    {
        public string BaseLink { get; private set; }
        public PageFinder PageFinder { get; private set; }
        public List<BoardValidator> BoardValidators { get; } = new List<BoardValidator>();
        public List<PostValidator> PostValidators { get; } = new List<PostValidator>();

        public ArcaliveCrawler(string channelName)
        {
            BaseLink = "https://arca.live/b/" + channelName;
        }

        public void ApplyBasicSettings()
        {
            PageFinder = ArcaliveCrawlerUtility.PageFinder_BinarySearchPageByTime;
            BoardValidators.Clear();
            BoardValidators.AddRange(new List<BoardValidator>
            {
                ArcaliveCrawlerUtility.BoardValidator_SkipNotices, ArcaliveCrawlerUtility.BoardValidator_TestByDateTime
            });
            PostValidators.Clear();
            PostValidators.AddRange(new List<PostValidator>
            {

            });
        }

        private bool RunBoardValidators(PostInfo current, object[] args)
        {
            return BoardValidators.All(postValidator => postValidator(current, args));
        }
        private bool RunPostValidators(PostInfo current, object[] args)
        {
            return PostValidators.All(postValidator => postValidator(current, args));
        }

        public IEnumerable<PostInfo> CrawlBoards(PostInfo startPostInfo, PostInfo endPostInfo)
        {
            int currentPage = PageFinder(this, startPostInfo);
            bool crawlNextPage;
            do
            {
                var pageDoc = ArcaliveDocDownloader.DownloadDoc(BaseLink + $"?p={currentPage}");
                if (string.IsNullOrEmpty(pageDoc.Text))
                    yield break;

                var postNodes = pageDoc.DocumentNode.SelectNodes("//div[contains(@class, 'list-table')]/a")
                    .Select(x => (ArcalivePostInfo) x).ToList();
                Parallel.ForEach(postNodes, ArcaliveDataParser.ParsePostData);
                var validPosts = postNodes.Where(x =>
                    RunBoardValidators(x, new object[]{ startPostInfo , endPostInfo})).ToList();
                foreach (var info in validPosts)
                {
                    yield return info;
                }

                crawlNextPage = validPosts.Any();
                currentPage++;
            } while (crawlNextPage);
        }

        public void CrawlPosts(IEnumerable<PostInfo> outPosts)
        {
            if (outPosts == null) throw new ArgumentNullException();
            var postInfos = outPosts.Select(x => (ArcalivePostInfo) x).ToList();
            if (postInfos.Any(x => x.boardSource == null))
                throw new ArgumentException("Call CrawlBoards before call this");
            foreach (var postInfo in postInfos)
            {
                var postDoc = ArcaliveDocDownloader.DownloadDoc(postInfo.href);
                if (string.IsNullOrEmpty(postDoc.Text)) continue;
                postInfo.postSource = postDoc.DocumentNode;
            }

            Parallel.ForEach(postInfos, ArcaliveDataParser.ParsePostData);
        }
    }
}