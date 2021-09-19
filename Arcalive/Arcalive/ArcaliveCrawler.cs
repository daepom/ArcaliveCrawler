using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;

namespace Arcalive
{
    public partial class ArcaliveCrawler
    {
        public event EventHandler Print;

        public event EventHandler DumpText;

        public event EventHandler ShowCrawlingProgress;

        public static int CallTimes { get; private set; }

        private readonly string channelName;

        public ArcaliveCrawler(string channelName)
        {
            bool isFullLink = channelName.StartsWith("https://arca.live/b/");
            if (isFullLink)
            {
                this.channelName = channelName;
            }
            else
            {
                this.channelName = "https://arca.live/b/" + channelName;
            }

            CallTimes = 0;
        }

        public List<Post> CrawlBoards(DateTime? From = null, DateTime? To = null, int startPage = 1)
        {
            From ??= DateTime.Today.AddDays(1 - DateTime.Today.Day).AddMonths(-1);
            To ??= DateTime.Today;

            int page = startPage;
            List<Post> results = new List<Post>();

            bool isEalierThanFrom = true;

            while (isEalierThanFrom == true)
            {
                HtmlDocument doc = DownloadDoc(channelName + $"?p={page}");

                if (string.IsNullOrEmpty(doc.Text))
                    return results;

                Print?.Invoke(this, new PrintCallbackArg($"{CallTimes++,5} >> CrawlBoard >> Page: {page}"));

                var posts = doc.DocumentNode.SelectNodes("//div[contains(@class, 'list-table')]/a");

                int i = 0;
                for (; i < posts.Count; i++)
                {
                    if (posts[i].Attributes["class"].Value == "vrow")
                        // 공지사항이 아닌 글이 나올 때까지 스킵
                        break;
                }
                for (; i < posts.Count; i++)
                {
                    Post p = new Post();
                    
                    var tmpNode = posts[i].SelectSingleNode(".//div[2]/span[2]/time");
                    if (tmpNode == null)
                    {
                        // 2021-03-31 부로 생긴 "(권한 없음)" 기능에 대비한 예외 처리
                        Print?.Invoke(this, new PrintCallbackArg($"{CallTimes++,5} >> CrawlBoard >> Skip Post >> Access Denied"));
                        continue;
                    }

                    p.time = DateTime.Parse(tmpNode.Attributes["datetime"].Value);

                    if (p.time >= To)
                    {
                        Print?.Invoke(this, new PrintCallbackArg($"{CallTimes++,5} >> CrawlBoard >> Skip Post"));
                        continue;
                    }

                    if (p.time <= From)
                    {
                        isEalierThanFrom = false;
                        break;
                    }

                    var postfix = posts[i].Attributes["href"].Value;
                    p.link = "https://arca.live" + postfix.Substring(0, postfix.LastIndexOf('?'));
                    if (results.Any(e => e.link == p.link))
                    {
                        continue;
                    }

                    var commentNum = posts[i].SelectSingleNode(".//div[1]/span[2]/span[3]").InnerText;
                    bool can = int.TryParse(Regex.Replace(commentNum, @"\D", ""), out int cap);
                    p.comments = new List<Comment>(cap);

                    p.id = int.Parse(posts[i].SelectSingleNode(".//div[1]/span[1]").InnerText);
                    p.badge = posts[i].SelectSingleNode(".//div[1]/span[2]/span[1]").InnerText;
                    p.title = posts[i].SelectSingleNode(".//div[1]/span[2]/span[2]").InnerText;
                    p.author = posts[i].SelectSingleNode(".//div[2]/span[1]").InnerText;

                    results.Add(p);
                }
                page++;
            }

            return results;
        }

        public List<Post> CrawlPosts(List<Post> Posts, List<string> skip = null)
        {
            List<Post> results = new List<Post>();

            for (int i = 0; i < Posts.Count; i++)
            {
                Stopwatch sp = new Stopwatch();
                sp.Start();

                if (skip.Any(x => (x == Posts[i].badge) && x != string.Empty))
                {
                    Print?.Invoke(this, new PrintCallbackArg($"{CallTimes++,5} >> CrawlPosts >> Skip Post by Tag"));
                    continue;
                }
                HtmlDocument doc = DownloadDoc(Posts[i].link);
                if (string.IsNullOrEmpty(doc.Text))
                {
                    Print?.Invoke(this, new PrintCallbackArg($"{CallTimes++,5} >> CrawlPosts >> Skip Post"));
                    continue;
                }
                Print?.Invoke(this, new PrintCallbackArg($"{CallTimes++,5} >> CrawlPosts >> {Posts[i].id}"));

                Post newPost = new Post();
                newPost = Posts[i];

                var articleInfoNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'article-info')]");

                var contentNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'fr-view article-content')]");

                var commentNumNode = articleInfoNode.SelectSingleNode(".//span[8]");
                var viewNumNode = articleInfoNode.SelectSingleNode(".//span[11]");

                newPost.content = contentNode.InnerText;
                newPost.view = int.Parse(viewNumNode.InnerText);

                DumpText?.Invoke(this, new PrintCallbackArg(doc.DocumentNode.SelectSingleNode(
                    "//div[contains(@class, 'fr-view article-content')]").InnerText));

                if (int.Parse(commentNumNode.InnerText) == 0) continue; // 댓글이 없으면 스킵

                // 댓글이 50개를 넘어가 페이지가 분리될 경우
                if (doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'pagination-wrapper my-2')]") != null)
                {
                    int maxCommentPageNumber = int.Parse(doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'pagination-wrapper my-2')]/ul/li[contains(@class, 'page-item active')]/a").InnerText);
                    List<Comment> comments = new List<Comment>();
                    var commentAreaNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'list-area')]");
                    comments.AddRange(ParseComments(commentAreaNode));
                    for (int k = maxCommentPageNumber - 1; k >= 1; k--)
                    {
                        HtmlDocument commentDoc = DownloadDoc(newPost.link + "?cp=" + k);
                        commentAreaNode = commentDoc.DocumentNode.SelectSingleNode("//div[contains(@class, 'list-area')]");
                        comments.AddRange(ParseComments(commentAreaNode));
                    }
                    newPost.comments = comments;
                }
                else
                {
                    var commentAreaNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'list-area')]");
                    newPost.comments = ParseComments(commentAreaNode);
                }

                results.Add(newPost);
                sp.Stop();
                ShowCrawlingProgress?.Invoke(this, new ProgressPagesCallBack(i + 1, Posts.Count, (int)sp.Elapsed.TotalMilliseconds));
            }

            return results;
        }

        private List<Comment> ParseComments(HtmlNode commentAreaNode)
        {
            List<Comment> comments = new List<Comment>();

            try
            {
                var commentWrappers = commentAreaNode?.Descendants(0)
                .Where(n => n.HasClass("comment-item"));

                foreach (var commentWrapper in commentWrappers)
                {
                    Comment c = new Comment();
                    var author = commentWrapper.SelectSingleNode(".//div/div[1]/span").InnerText;
                    var time = DateTime.Parse(commentWrapper.SelectSingleNode(".//div/div[1]/div[contains(@class, 'right')]/time").Attributes["datetime"].Value);
                    if (commentWrapper.SelectSingleNode(".//div/div[2]/div/img[@src]") != null)
                    {
                        var arcacon = commentWrapper.SelectSingleNode(".//div/div[2]/div/img[@src]").Attributes["src"].Value;
                        c.content = arcacon;
                        c.isArcacon = true;
                        c.dataId = int.Parse(commentWrapper.SelectSingleNode(".//div/div[2]/div/img[@data-id]")
                            .Attributes["data-id"].Value);
                    }
                    else if (commentWrapper.SelectSingleNode(".//div/div[2]/div/video[@src]") != null)
                    {
                        var arcacon = commentWrapper.SelectSingleNode(".//div/div[2]/div/video[@src]").Attributes["src"].Value;
                        if (arcacon.EndsWith(".mp4"))
                            arcacon += ".gif";
                        c.content = arcacon;
                        c.isArcacon = true;
                        c.dataId = int.Parse(commentWrapper.SelectSingleNode(".//div/div[2]/div/video[@data-id]")
                            .Attributes["data-id"].Value);
                    }
                    else
                    {
                        c.content = commentWrapper.SelectSingleNode(".//div/div[2]/div").InnerText;
                        c.isArcacon = false;
                    }
                    c.author = author;
                    c.time = time;
                    //c.arcacon = arcacon;
                    comments.Add(c);
                }
            }
            catch
            {
                // 댓글은 분명 없는데 commentNum.InnerText의
                // 값이 0이 아닌 경우가 있어서 try/catch문을 씀
            }

            return comments;
        }

        public int FindStartPage(DateTime TargetTime, int StartPage = 1, int MaxPage = 10000)
        {
            DateTime TimeofFirstPost, TimeofLastPost;
            bool isPageFound = false;
            int currentPage = -1;

            Print?.Invoke(this, new PrintCallbackArg($"{CallTimes++,5} >> FindStartPage >> Finding..."));

            while (isPageFound == false)
            {
                currentPage = (StartPage + MaxPage) / 2;

                HtmlDocument doc = DownloadDoc(channelName + $"?p={currentPage}");
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
                    isPageFound = true;
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

            Print?.Invoke(this, new PrintCallbackArg($"{CallTimes++,5} >> FindStartPage >> Found!"));

            return currentPage;
        }

        public static void SerializePosts(List<Post> posts, string filename = "a.dat")
        {
            using (Stream ws = new FileStream(filename, FileMode.Create))
            {
                BinaryFormatter binary = new BinaryFormatter();
                binary.Serialize(ws, posts);
            }
        }

        public static List<Post> DeserializePosts(string filename = "a.dat")
        {
            using (Stream rs = new FileStream(filename, FileMode.Open))
            {
                BinaryFormatter binary = new BinaryFormatter();
                return (List<Post>)binary.Deserialize(rs);
            }
        }

        public bool TryCrawl(out string channelTitle)
        {
            HtmlDocument doc = DownloadDoc(channelName);
            channelTitle = string.Empty;


            var postNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'board-title')]");
            if (postNode == null) return false;
            channelTitle = postNode.InnerText;
            return true;
        }

        [Obsolete]
        public static List<string> GetChannelLinks(string keyword)
        {
            List<string> results = new List<string>();

            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                string sitesource = client.DownloadString("https://arca.live/private_boards");
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(sitesource);
                var channels = doc.DocumentNode.SelectNodes("/html/body/div/div[3]/article/div[2]/div");
                foreach (var channel in channels)
                {
                    var channelName = channel.Descendants("a").First().InnerText;
                    if (channelName.Contains(keyword))
                    {
                        var address = channel.Descendants("a").First().Attributes["href"].Value;
                        results.Add("https://arca.live" + address);
                    }
                }
            }
            return results;
        }

        [Obsolete]
        public static string GetChannelLink(string keyword)
        {
            string result = string.Empty;

            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                string sitesource = client.DownloadString("https://arca.live/private_boards");
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(sitesource);
                var channels = doc.DocumentNode.SelectNodes("/html/body/div/div[2]/article/div[2]/div");
                foreach (var channel in channels)
                {
                    var channelName = channel.Descendants("a").First().InnerText;
                    if (channelName.Contains(keyword))
                    {
                        var address = channel.Descendants("a").First().Attributes["href"].Value;
                        result = "https://arca.live" + address;
                        return result;
                    }
                }
            }

            return result;
        }
    }
}