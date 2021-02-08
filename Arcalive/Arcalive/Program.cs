using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Arcalive
{
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

                        ArcaliveCrawler.SerializePosts(posts);

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