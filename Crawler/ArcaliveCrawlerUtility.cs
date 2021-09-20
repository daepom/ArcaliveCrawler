using HtmlAgilityPack;
using System;

namespace Crawler
{
    public static class ArcaliveCrawlerUtility
    {
        public static readonly int StartPage = 1, MaxPage = 10000;


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

        public static bool BoardValidator_SkipNotices(PostInfo info, object[] args)
        {
            return info?.boardSource?.Attributes["class"]?.Value == "vrow";
        }

        public static bool BoardValidator_TestByDateTime(PostInfo info, object[] args)
        {
            DateTime startTime = ((PostInfo)args[0]).dt, endTime = ((PostInfo)args[1]).dt, currentTime = info.dt;
            if (currentTime >= startTime || currentTime <= endTime) return false;
            return true;
        }

        public static bool PostValidator_SkipByTag(PostInfo info, object[] args)
        {
            return true;
        }
    }
}