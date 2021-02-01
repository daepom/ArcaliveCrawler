using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Arcalive
{
    public class PrintCallbackArg : EventArgs
    {
        public string Str { get; }

        public PrintCallbackArg(string callbackString)
        {
            Str = callbackString;
        }
    }

    public partial class ArcaliveCrawler
    {
        public event EventHandler Print;

        public event EventHandler DumpText;

        public static int CallTimes { get; private set; }

        private string channelName = string.Empty;

        /// <summary>
        /// 키워드를 포함하는 채널의 주소 리스트를 반환합니다.
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 키워드를 포함하는 채널의 주소 하나(첫번째)를 반환합니다.
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
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

        private static HtmlDocument DownloadDoc(string link)
        {
            HtmlDocument doc = new HtmlDocument();
            using (WebClient client = new WebClient() { Encoding = Encoding.UTF8 })
            {
                string siteSource = client.DownloadString(link);
                doc.LoadHtml(siteSource);
            }

            // HTML 429 에러 방지용
            // 100 이상으로 설정하는 것을 권장
            Thread.Sleep(110);

            return doc;
        }

        public List<Post> CrawlBoards(DateTime? From = null, DateTime? To = null, int startPage = 1)
        {
            if (From == null) From = DateTime.Today.AddDays(1 - DateTime.Today.Day).AddMonths(-1);
            if (To == null) To = DateTime.Today;

            int page = startPage;
            List<Post> results = new List<Post>();

            bool isEalierThanFrom = true;

            while (isEalierThanFrom == true)
            {
                HtmlDocument doc = DownloadDoc(channelName + $"?p={page}");

                Print?.Invoke(this, new PrintCallbackArg($"{CallTimes++, 5} >> CrawlBoard >> Page: {page}"));

                var posts = doc.DocumentNode.SelectNodes("//div[contains(@class, 'list-table')]/a");

                int i;
                for (i = 0; i < posts.Count; i++)
                {
                    if (posts[i].Attributes["class"].Value == "vrow")
                        // 공지사항이 아닌 글이 나올 때까지 스킵
                        break;
                }
                for (; i < posts.Count; i++)
                {
                    Post p = new Post();

                    p.time = DateTime.Parse(posts[i].SelectSingleNode(".//div[2]/span[2]/time").Attributes["datetime"].Value);

                    if (p.time > To)
                    {
                        Print?.Invoke(this, new PrintCallbackArg($"시간 범위에 맞지 않는 글은 스킵:  {p.time}"));
                        continue;
                    }
                    else if (p.time < From)
                    {
                        isEalierThanFrom = false;
                        break;
                    }

                    var postfix = posts[i].Attributes["href"].Value;
                    p.link = "https://arca.live" + postfix.Substring(0, postfix.LastIndexOf('?'));
                    if (results.Any(e => e.link == p.link))
                    {
                        Print?.Invoke(this, new PrintCallbackArg("중복방지"));
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

        public List<Post> CrawlPosts(List<Post> Posts)
        {
            List<Post> results = new List<Post>();

            for(int i = 0; i < Posts.Count; i++)
            {
                HtmlDocument doc = DownloadDoc(Posts[i].link);
                Print?.Invoke(this, new PrintCallbackArg($"{CallTimes++, 5} >> CrawlPosts >> {Posts[i].id}"));

                Post newPost = new Post();
                newPost = Posts[i];

                var articleInfoNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'article-info')]");
                var commentAreaNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'list-area')]");
                var contentNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'fr-view article-content')]");

                var commentNumNode = articleInfoNode.SelectSingleNode(".//span[8]");
                var viewNumNode = articleInfoNode.SelectSingleNode(".//span[11]");

                newPost.content = contentNode.InnerText;
                newPost.view = int.Parse(viewNumNode.InnerText);

                DumpText?.Invoke(this, new PrintCallbackArg(doc.DocumentNode.SelectSingleNode(
                    "//div[contains(@class, 'fr-view article-content')]").InnerText));

                if (int.Parse(commentNumNode.InnerText) == 0) continue; // 댓글이 없으면 스킵

                List<Comment> comments = new List<Comment>();
                try
                {
                    var commentWrappers = commentAreaNode?.Descendants(0)
                    .Where(n => n.HasClass("comment-wrapper"));
                    foreach (var commentWrapper in commentWrappers)
                    {
                        Comment c = new Comment();
                        var author = commentWrapper.SelectSingleNode(".//div[1]/div/div[1]/span").InnerText;
                        c.author = author;
                        comments.Add(c);
                    }
                }
                catch
                {
                    // 댓글은 분명 없는데 commentNum.InnerText의
                    // 값이 0이 아닌 경우가 있어서 try/catch문을 씀
                }
                newPost.comments = comments;

                results.Add(newPost);
            }

            return results;
        }

        public List<Post> GetPosts(DateTime? from = null, DateTime? to = null, bool readComments = false, int page = 1, bool crawlSlowly = true)
        {
            if (from == null) from = DateTime.Today.AddDays(1 - DateTime.Today.Day).AddMonths(-1);
            if (to == null) to = DateTime.Today;

            List<Post> results = new List<Post>();

            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                bool isEalierThanFrom = true;

                while (isEalierThanFrom == true)
                {
                    Print?.Invoke(this, new PrintCallbackArg($"Page: {page}"));
                    // printDelegate($"Page: {page}");

                    string sitesource = client.DownloadString(channelName + $"?p={page}");
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(sitesource);

                    var posts = doc.DocumentNode.SelectNodes("//div[contains(@class, 'list-table')]/a");

                    int i;
                    for (i = 0; i < posts.Count; i++)
                    {
                        if (posts[i].Attributes["class"].Value == "vrow")
                        {
                            break; // 공지사항은 건너뛰기
                        }
                    }
                    for (; i < posts.Count; i++)
                    {
                        Post p = new Post();

                        p.time = DateTime.Parse(posts[i].SelectSingleNode(".//div[2]/span[2]/time").Attributes["datetime"].Value);
                        if ((p.time < to) == false)
                        {
                            Print?.Invoke(this, new PrintCallbackArg($"시간 범위에 맞지 않는 글은 스킵:  {p.time}"));
                            continue;
                        }
                        var postfix = posts[i].Attributes["href"].Value;
                        p.link = "https://arca.live" + postfix.Substring(0, postfix.LastIndexOf('?'));
                        Print?.Invoke(this, new PrintCallbackArg(p.link));
                        if (results.Any(e => e.link == p.link))
                        {
                            Print?.Invoke(this, new PrintCallbackArg("중복방지"));
                            continue;
                        }
                        if (readComments == true)
                        {
                            p.comments = GetComments(p.link);
                            if (crawlSlowly == true) Thread.Sleep(120);
                        }
                        else
                        {
                            var str = posts[i].SelectSingleNode(".//div[1]/span[2]/span[3]").InnerText;
                            int cap = new int();
                            bool can = int.TryParse(Regex.Replace(str, @"\D", ""), out cap);
                            p.comments = new List<Comment>(cap);
                        }

                        p.id = int.Parse(posts[i].SelectSingleNode(".//div[1]/span[1]").InnerText);
                        p.badge = posts[i].SelectSingleNode(".//div[1]/span[2]/span[1]").InnerText;
                        p.title = posts[i].SelectSingleNode(".//div[1]/span[2]/span[2]").InnerText;
                        p.author = posts[i].SelectSingleNode(".//div[2]/span[1]").InnerText;

                        results.Add(p);
                        //Post p = new Post();
                        //p.link = "https://arca.live" + posts[i].Attributes["href"].Value;
                        //var top = posts[i].Descendants("div").First();
                        //var bottom = posts[i].Descendants("div").ToList()[1];
                        //p.id = int.Parse(top.Descendants("span").First().InnerText);
                        //var col_title = top.Descendants("span").ToList()[2].Descendants("span");
                        //p.badge = col_title.ToList()[0].InnerText;
                        //p.title = col_title.ToList()[1].InnerText;
                        //p.author = bottom.Descendants("span").ToList()[0].InnerText;
                        //Console.WriteLine(bottom.Descendants("span").ToList()[2].InnerText);
                    }
                    page++;
                    if (results.Count == 0 || results.Last().time >= from)
                    {
                        isEalierThanFrom = true;
                    }
                    else
                    {
                        isEalierThanFrom = false;
                    }

                    if (crawlSlowly == true && readComments == false) Thread.Sleep(120);
                }
            }

            /*
            ChromeOptions options = new ChromeOptions();
            options.AddUserProfilePreference("profile.default_content_setting_values.images", 2);

            using (ChromeDriver driver = new ChromeDriver(options))
            {
                int page = 1;
                while (gotoNextPage == true)
                {
                    driver.Navigate().GoToUrl(channelName + $"?p={page}");
                    driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(10);
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

                    var posts = driver.FindElementByXPath("/html/body/div/div[2]/article/div/div[4]/div[2]")
                                      .FindElements(By.TagName("a"));
                    int i;
                    for (i = 0; i < posts.Count; i++)
                        if (posts[i].GetAttribute("class") == "vrow")
                            break; // 공지사항은 건너뛰기
                    for (; i < posts.Count; i++)
                    {
                        Post p = new Post();
                        p.link = posts[i].GetAttribute("href");
                        //var top = posts[i].FindElement(By.ClassName("vrow-top"));
                        //p.id = int.Parse(top.FindElement(By.ClassName("col-id")).Text);
                        //p.badge = top.FindElement(By.ClassName("col-title"))
                        //    .FindElement(By.ClassName("badge-success")).Text;
                        var bottom = posts[i].FindElement(By.ClassName("vrow-bottom"));
                        p.author = bottom.FindElement(By.ClassName("user-info")).Text;
                        p.time = DateTime.Parse(bottom.FindElement(By.TagName("time")).GetAttribute("datetime"));
                        results.Add(p);

                        if (results.Last().time.Month >= 10)
                        {
                            gotoNextPage = true;
                        }
                        else
                        {
                            gotoNextPage = false;
                        }
                    }

                    page++;
                }
            }
            */

            return results;
        }

        public List<Comment> GetComments(string postLink)
        {
            List<Comment> results = new List<Comment>();

            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                string sitesource = client.DownloadString(postLink);
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(sitesource);
                var commentArea = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'list-area')]");

                var commentNum = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'article-info')]/span[8]");

                DumpText?.Invoke(this, new PrintCallbackArg(doc.DocumentNode.SelectSingleNode(
                    "//div[contains(@class, 'fr-view article-content')]").InnerText));

                if (int.Parse(commentNum.InnerText) == 0) return results; // 댓글이 없으면 스킵

                try
                {
                    var commentWrappers = commentArea?.Descendants(0)
                    .Where(n => n.HasClass("comment-wrapper"));
                    foreach (var commentWrapper in commentWrappers)
                    {
                        Comment c = new Comment();
                        var author = commentWrapper.SelectSingleNode(".//div[1]/div/div[1]/span").InnerText;
                        c.author = author;
                        results.Add(c);
                    }
                }
                catch
                {
                    // 댓글은 분명 없는데 commentNum.InnerText의
                    // 값이 0이 아닌 경우가 있어서 try문을 씀
                }

                return results;

                //var channels = doc.DocumentNode.SelectNodes("/html/body/div/div[2]/article/div[2]/div");
                //foreach (var channel in channels)
                //{
                //    var channelName = channel.Descendants("a").First().InnerText;
                //    if (channelName.Contains(keyword))
                //    {
                //        var address = channel.Descendants("a").First().Attributes["href"].Value;
                //        result = "https://arca.live" + address;
                //        return result;
                //    }
                //}
            }
        }

        public static void SerializationPosts(List<Post> posts, string filename = "a.dat")
        {
            using (Stream ws = new FileStream(filename, FileMode.Create))
            {
                BinaryFormatter binary = new BinaryFormatter();
                binary.Serialize(ws, posts);
            }
        }

        public static List<Post> DeserializationPosts(string filename = "a.dat")
        {
            using (Stream rs = new FileStream(filename, FileMode.Open))
            {
                BinaryFormatter binary = new BinaryFormatter();
                return (List<Post>)binary.Deserialize(rs);
            }
        }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("기능을 선택해주세요.\n" +
                    "1. 채널 링크 출력\n" +
                    "2. 갤창 리포트 출력\n" +
                    "3. 글 데이터 파일 생성\n" +
                    "종료: Ctrl + C");
                int a = int.Parse(Console.ReadLine());
                switch (a)
                {
                    case 1:
                        Console.WriteLine("채널 검색 키워드를 입력해주세요.");
                        break;

                    case 2:
                        Console.WriteLine("채널 이름을 입력해주세요.");
                        var s = ArcaliveCrawler.GetChannelLink(Console.ReadLine());
                        ArcaliveCrawler ac = new ArcaliveCrawler(s);
                        DateTime d1 = new DateTime(2020, 11, 30, 23, 59, 59);
                        DateTime d2 = new DateTime(2020, 11, 1);
                        var posts = ac.GetPosts(d2, d1, true);

                        Dictionary<string, int> postAuthor = new Dictionary<string, int>();
                        Dictionary<string, int> commentAuthor = new Dictionary<string, int>();
                        foreach (var post in posts)
                        {
                            foreach (var comment in post.comments)
                            {
                                if (commentAuthor.ContainsKey(comment.author) == false)
                                    commentAuthor.Add(comment.author, 1);
                                else
                                    commentAuthor[comment.author]++;
                            }

                            if (postAuthor.ContainsKey(post.author) == false)
                                postAuthor.Add(post.author, 1);
                            else
                                postAuthor[post.author]++;
                        }

                        ArcaliveCrawler.SerializationPosts(posts);

                        var paDesc = postAuthor.OrderByDescending(x => x.Value);
                        var commentsDesc = commentAuthor.OrderByDescending(x => x.Value);

                        string txt = string.Empty;

                        Console.WriteLine("//글");
                        foreach (var dic in paDesc)
                        {
                            Console.WriteLine($"{dic.Key}, {dic.Value}");
                            txt += $"{dic.Key}, {dic.Value}\r\n";
                        }
                        Console.WriteLine("\n\n//댓글");
                        foreach (var comment in commentsDesc)
                        {
                            Console.WriteLine($"{comment.Key}, {comment.Value}");
                            txt += $"{comment.Key}, {comment.Value}\r\n";
                        }
                        File.WriteAllText("a.txt", txt);
                        break;

                    default:
                        break;
                }
                Console.WriteLine("아무키나 누르세요..");
                Console.ReadKey();
            }
        }
    }
}