using HtmlAgilityPack;
using System;

namespace Crawler
{
    public static class ArcaliveCrawlerUtility
    {
        public static readonly int StartPage = 1, MaxPage = 10000;

        public static void ParseData(this ArcalivePostInfo outInfo)
        {
            var bSource = outInfo?.boardSource;
            if (bSource == null)
                throw new ArgumentException("Argument or its boardSource is null");
            string href = bSource.Attributes["href"]?.Value;
            if (outInfo.href == null && href != null)
                outInfo.href = "https://arca.live" + href.Substring(0, href.LastIndexOf('?'));
            string dateString = bSource.SelectSingleNode(".//div[2]/span[2]/time")?.Attributes["datetime"]?.Value;
            if (dateString != null)
                outInfo.dt = DateTime.Parse(dateString);
            if (int.TryParse(bSource.SelectSingleNode(".//div[1]/span[1]")?.InnerText, out int id))
                outInfo.id = id;
            string badge = bSource.SelectSingleNode(".//div[1]/span[2]/span[1]")?.InnerText;
            if (badge != null)
                outInfo.badge = badge;
            string title = bSource.SelectSingleNode(".//div[1]/span[2]/span[2]")?.InnerText;
            if (title != null)
                outInfo.title = title;
            string author = bSource.SelectSingleNode(".//div[2]/span[1]")?.InnerText;
            if (author != null)
                outInfo.author = author;


            var pSource = outInfo.postSource;
            if (pSource == null)
                return;
            var articleInfoNode = pSource.SelectSingleNode("//div[contains(@class, 'article-info')]");
            var contentNode = pSource.SelectSingleNode("//div[contains(@class, 'fr-view article-content')]");
            var commentNumNode = articleInfoNode.SelectSingleNode(".//span[8]");
            var viewNumNode = articleInfoNode.SelectSingleNode(".//span[11]");
        }

        public static int PageFinder_BinarySearchPageByTime(ArcaliveCrawler crawler, PostInfo info)
        {
            DateTime TimeofFirstPost, TimeofLastPost, TargetTime = info.dt;
            bool found = false;
            int StartPage = ArcaliveCrawlerUtility.StartPage, MaxPage = ArcaliveCrawlerUtility.MaxPage;
            int currentPage = -1;

            while (found == false)
            {
                currentPage = (StartPage + MaxPage) / 2;

                HtmlDocument doc = ArcaliveDocDownloader.DownloadDoc(crawler.BaseLink + $"?p={currentPage}");
                if (string.IsNullOrEmpty(doc.Text))
                    return -1;

                var posts = doc.DocumentNode.SelectNodes("//div[contains(@class, 'list-table')]/a");

                if (posts.Count <= 1)
                {
                    // 글이 없을 조건 1
                    MaxPage = currentPage;
                    continue;
                }

                int i;
                for (i = 0; i < posts.Count; i++)
                {
                    if (posts[i].Attributes["class"].Value == "vrow")
                        // 공지사항이 아닌 글이 나올 때까지 스킵
                        break;
                }

                if (i == posts.Count)
                {
                    // 글이 없을 조건 2
                    MaxPage = currentPage;
                    continue;
                }

                TimeofFirstPost = DateTime.Parse(posts[i].SelectSingleNode(".//div[2]/span[2]/time").Attributes["datetime"].Value);
                TimeofLastPost = DateTime.Parse(posts[posts.Count - 1].SelectSingleNode(".//div[2]/span[2]/time").Attributes["datetime"].Value);

                if ((TargetTime >= TimeofLastPost && TargetTime <= TimeofFirstPost) || currentPage == 1)
                {
                    found = true;
                }
                else if (TargetTime >= TimeofLastPost)
                {
                    MaxPage = currentPage;
                }
                else
                {
                    StartPage = currentPage;
                }
            }

            return currentPage;
        }

        public static bool PostValidator_SkipNotices(PostInfo info, PostInfo start, PostInfo end)
        {
            return info?.boardSource?.Attributes["class"]?.Value != "vrow";
        }

        public static bool PostValidator_TestByDateTime(PostInfo info, PostInfo start, PostInfo end)
        {
            DateTime startTime = start.dt, endTime = end.dt, currentTime = info.dt;
            if (currentTime >= startTime || currentTime <= endTime) return false;
            return true;
        }
    }
}