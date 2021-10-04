using HtmlAgilityPack;
using System;
using System.Reflection;

namespace Crawler
{
    public static class ArcaliveCrawlerUtility
    {
        public static readonly int StartPage = 1, MaxPage = 10000;


        public static int PageFinder_BinarySearchPageByTime(BaseCrawler crawler)
        {
            //crawler.Logger.Log(MethodBase.GetCurrentMethod().Name, "Start");

            DateTime TimeofFirstPost, TimeofLastPost, TargetTime = crawler.StartInfo.dt;
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

            //crawler.Logger.Log(MethodBase.GetCurrentMethod().Name, "Found!", currentPage);
            return currentPage;
        }

        public static bool BoardFilter_SkipNotices(PostInfo info, BaseCrawler crawler)
        {
            return info?.boardSource?.Attributes["class"]?.Value == "vrow";
        }

        public static bool BoardFilter_TestByDateTime(PostInfo info, BaseCrawler crawler)
        {
            DateTime startTime = crawler.StartInfo.dt, endTime = crawler.EndInfo.dt, currentTime = info.dt;
            return currentTime < startTime && currentTime > endTime;
        }

        public static bool PostFilter_SkipByTag(PostInfo info, BaseCrawler crawler)
        {
            return true;
        }

        public static bool TestCrawl(string link, out string result)
        {
            Console.WriteLine(link);
            result = string.Empty;
            try
            {
                var testDoc = ArcaliveDocDownloader.DownloadDoc(link);
                var node = testDoc.DocumentNode.SelectSingleNode("//div[contains(@class, 'board-title')]/a[2]");
                if (node?.InnerText == null)
                    return false;
                result = node.InnerText;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            return true;
        }
    }
}