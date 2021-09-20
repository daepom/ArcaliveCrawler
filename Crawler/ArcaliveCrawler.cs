using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crawler
{
    public class ArcaliveCrawler
    {
        public string BaseLink { get; private set; }
        public PageFinder PageFinder { get; private set; }
        public List<PostValidator> PostValidators { get; private set; } = new List<PostValidator>();

        public ArcaliveCrawler(string channelName)
        {
            BaseLink = "https://arca.live/b/" + channelName;
        }

        public void ApplyBasicSettings()
        {
            PageFinder = ArcaliveCrawlerUtility.PageFinder_BinarySearchPageByTime;
            PostValidators.AddRange(new List<PostValidator>
            {
                ArcaliveCrawlerUtility.PostValidator_SkipNotices, ArcaliveCrawlerUtility.PostValidator_TestByDateTime
            });
        }

        private bool RunPostValidators(PostInfo current, PostInfo start, PostInfo end)
        {
            return PostValidators.All(postValidator => postValidator(current, start, end));
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

                var postNodes = pageDoc.DocumentNode.SelectNodes("//div[contains(@class, 'list-table')]/a");
                var validPosts = postNodes.Where(x =>
                    RunPostValidators((PostInfo) x, startPostInfo, endPostInfo)).Select(x => (ArcalivePostInfo) x).ToList();

                foreach (var info in validPosts)
                {
                    yield return info;
                }

                crawlNextPage = validPosts.Any();
                currentPage++;
            } while (crawlNextPage);
        }

        public IEnumerable<PostInfo> CrawlPosts(IEnumerable<PostInfo> posts)
        {
            if(posts == null) yield break;
        }
    }
}