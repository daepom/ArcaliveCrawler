using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;

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

    public delegate void PrintDelegate(object obj);

    public partial class Arcalive
    {
        public event EventHandler Print;

        public event EventHandler DumpText;

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

        public Arcalive(string channelName)
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
        }

        /// <summary>
        /// 글을 크롤링합니다.
        /// </summary>
        /// <param name="from">이 시간 부터</param>
        /// <param name="to">이 시간 까지</param>
        /// <param name="printDelegate">중간중간 로그를 기록할 함수</param>
        /// <param name="readComments">댓글 작성자 등의 정보도 수집할까요? 주의: readComments를 true로 하면 파싱 속도가 급격히 느려짐</param>
        /// <param name="page">몇 페이지부터 읽을까요?</param>
        /// <returns></returns>
        public List<Post> GetPosts(DateTime? from = null, DateTime? to = null, bool readComments = false, int page = 1)
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
                    Print(this, new PrintCallbackArg($"Page: {page}"));
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
                            Print(this, new PrintCallbackArg($"시간 범위에 맞지 않는 글은 스킵:  {p.time}"));
                            continue;
                        }
                        var postfix = posts[i].Attributes["href"].Value;
                        p.link = "https://arca.live" + postfix.Substring(0, postfix.LastIndexOf('?'));
                        Print(this, new PrintCallbackArg(p.link));
                        if (results.Any(e => e.link == p.link))
                        {
                            Print(this, new PrintCallbackArg("중복방지"));
                            continue;
                        }
                        if (readComments == true)
                        {
                            p.comments = GetComments(p.link);
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
                // "//div[contains(@class, 'list-area')]"

                var commentNum = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'article-info')]/span[8]");

                DumpText(this, new PrintCallbackArg(doc.DocumentNode.SelectSingleNode(
                    "//div[contains(@class, 'fr-view article-content')]").InnerText));

                if (int.Parse(commentNum.InnerText) == 0) return results; // 댓글이 없으면 스킵

                try
                {
                    var commentWrappers = commentArea.Descendants(0)
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
                    // Do Nothing
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

        public Dictionary<string, int> GetCommentss(List<Post> posts)
        {
            Dictionary<string, int> results = new Dictionary<string, int>();
            ChromeOptions options = new ChromeOptions();
            options.AddUserProfilePreference("profile.default_content_setting_values.images", 2);

            using (ChromeDriver driver = new ChromeDriver(options))
            {
                foreach (var post in posts)
                {
                    driver.Navigate().GoToUrl(post.link);
                    driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(10);
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

                    var commentArea = driver.FindElementByXPath("/html/body/div/div[2]");
                    // /html/body/div[1]/div[3]/article/div/div[2]/div[3]
                    // /html/body/div[1]/div[3]/article/div/div[2]/div[5]/div[2]
                    var commentNum = commentArea.FindElement(By.XPath("/html/body/div/div[2]/article/div/div[2]/div[2]/div[2]/div[2]/span[8]"));
                    if (int.Parse(commentNum.Text) == 0) continue; // 댓글이 없으면 스킵

                    var commentWrappers = commentArea.FindElements(By.ClassName("comment-wrapper"));
                    foreach (var commentWrapper in commentWrappers)
                    {
                        string username = commentWrapper.FindElement(By.ClassName("user-info")).Text;
                        if (results.ContainsKey(username) == false)
                        {
                            results.Add(username, 1);
                        }
                        else
                        {
                            results[username]++;
                        }
                    }
                }
            }

            return results;
        }

        public static void SerializationPosts(List<Post> posts, string filename = "a.dat")
        {
            using(Stream ws = new FileStream(filename, FileMode.Create))
            {
                BinaryFormatter binary = new BinaryFormatter();
                binary.Serialize(ws, posts);
            }
        }

        public static List<Post> DeserializationPosts(string filename = "a.dat")
        {
            using(Stream rs = new FileStream(filename, FileMode.Open))
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
                        var s = Arcalive.GetChannelLink(Console.ReadLine());
                        Arcalive ac = new Arcalive(s);
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

                        Arcalive.SerializationPosts(posts);

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