using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arcalive;

namespace ArcaliveForm
{
    public partial class RankExportForm
    {
        string ExportRank1(int index)
        {
            var posts = GetDataFile(index);

            Dictionary<string, Ranking> RankDic = new Dictionary<string, Ranking>();
            Dictionary<string, double> PlayTimeDic = new Dictionary<string, double>();
            Dictionary<string, List<DateTime>> TimeByUsersDic = new Dictionary<string, List<DateTime>>();
            Dictionary<string, List<DateTime>> TimeByUsersDicDesc = new Dictionary<string, List<DateTime>>();
            foreach (var post in posts)
            {
                foreach (var comment in post.comments)
                {
                    if (RankDic.ContainsKey(comment.author) == false)
                        RankDic.Add(comment.author, new Ranking { comment = 1, post = 0, playTime = 0 });
                    else
                        RankDic[comment.author].comment++;

                    if (TimeByUsersDic.ContainsKey(comment.author) == false)
                        TimeByUsersDic.Add(comment.author, new List<DateTime>() { comment.time });
                    else
                        TimeByUsersDic[comment.author].Add(comment.time);
                }

                if (RankDic.ContainsKey(post.author) == false)
                    RankDic.Add(post.author, new Ranking { comment = 0, post = 1, playTime = 0 });
                else
                    RankDic[post.author].post++;

                if (TimeByUsersDic.ContainsKey(post.author) == false)
                    TimeByUsersDic.Add(post.author, new List<DateTime>() { post.time });
                else
                    TimeByUsersDic[post.author].Add(post.time);
            }

            var RankDicDesc = RankDic.OrderByDescending(x => x.Value.post);

            foreach (var a in TimeByUsersDic)
            {
                var list = a.Value.OrderByDescending(x => x).ToList();
                TimeByUsersDicDesc.Add(a.Key, list);
            }

            const int timeSpan = 10;

            foreach (var user in TimeByUsersDicDesc)
            {
                var timeList = user.Value;
                TimeSpan activeTime = TimeSpan.Zero;

                if (timeList.Count <= 1)
                {
                    activeTime += TimeSpan.FromMinutes(timeSpan);
                }
                else
                {
                    for (int i = 0; i < timeList.Count - 1; i++)
                    {
                        var diff = timeList[i] - timeList[i + 1];
                        if (diff <= TimeSpan.FromMinutes(timeSpan))
                        {
                            activeTime += diff;
                        }
                        else
                        {
                            activeTime += TimeSpan.FromMinutes(timeSpan);
                        }
                    }
                }

                if (RankDic.ContainsKey(user.Key) == false)
                {
                    Console.WriteLine("error");
                    throw new Exception("이상하다 이럴리가 없는데??");
                }
                else
                    RankDic[user.Key].playTime = activeTime.TotalHours;
            }

            StringBuilder sb = new StringBuilder();

            sb.Append("//갤창랭킹\r\n");
            sb.Append("작성자, 글, 댓글, 플레이타임(시)\r\n");
            foreach (var rank in RankDicDesc)
            {
                sb.Append($"{rank.Key}, {rank.Value.post}, {rank.Value.comment}, {Math.Round(rank.Value.playTime, 2)}\r\n");
            }

            return sb.ToString();
        }

        string ExportRank2(int index)
        {
            var posts = GetDataFile(index);

            StringBuilder sb = new StringBuilder();
            var timeDic = new Dictionary<string, int>();
            var weekDic = new Dictionary<string, int>();
            var dateDic = new Dictionary<int, int>();

            foreach (var post in posts)
            {
                var hour = post.time.ToString("HH");
                if (timeDic.ContainsKey(hour) == false)
                {
                    timeDic.Add(hour, 1);
                }
                else timeDic[hour]++;

                var week = post.time.DayOfWeek;
                if (weekDic.ContainsKey(week.ToString()) == false)
                {
                    weekDic.Add(week.ToString(), 1);
                }
                else weekDic[week.ToString()]++;

                var date = post.time.Date.Day;
                if (dateDic.ContainsKey(date) == false)
                {
                    dateDic.Add(date, 1);
                }
                else dateDic[date]++;
            }

            var timeDicDesc = timeDic.OrderBy(x => x.Key);
            var dateDicDesc = dateDic.OrderBy(x => x.Key);
            var weekDicDesc = weekDic.OrderBy(x => x.Key);

            sb.AppendLine("//시간별");
            foreach (var time in timeDicDesc)
            {
                sb.AppendLine($"{time.Key}, {time.Value}");
            }
            sb.AppendLine();
            sb.AppendLine("//요일별");
            foreach (var week in weekDicDesc)
            {
                sb.AppendLine($"{week.Key}, {week.Value}");
            }
            sb.AppendLine();
            sb.AppendLine("//날짜별");
            foreach (var date in dateDicDesc)
            {
                sb.AppendLine($"{date.Key}, {date.Value}");
            }

            return sb.ToString();
        }

        string ExportRank3(int index)
        {
            var posts = GetDataFile(index);

            Dictionary<string, int> arcaconDic = new Dictionary<string, int>();
            foreach (var comment in posts.SelectMany(post => post.comments))
            {
                if (comment.isArcacon != true) continue;
                else if (arcaconDic.ContainsKey(comment.content) == false)
                    arcaconDic.Add(comment.content, 1);
                else
                    arcaconDic[comment.content]++;
            }

            if (checkBox3.Checked == true)
            {
                var tmpList = arcaconDic.ToList();
                HashSet<string> toRemove = new HashSet<string>();
                List<byte[]> hashs = new List<byte[]>();

                foreach (var pair in tmpList)
                {
                    using (Bitmap bitmap = ImageComparer.DownloadImageFromUrl("https:" + pair.Key))
                    {
                        ImageConverter converter = new ImageConverter();
                        hashs.Add((byte[])converter.ConvertTo(bitmap, typeof(byte[])));
                    }
                }

                for (int i = 0; i < tmpList.Count - 1; i++)
                {
                    if (toRemove.Contains(tmpList[i].Key))
                        continue;
                    for (int j = i + 1; j < tmpList.Count; j++)
                    {
                        if (ImageComparer.Compare(hashs[i], hashs[j]) == true)
                        {
                            Console.WriteLine($"{tmpList[i].Key} == {tmpList[j].Key}");
                            arcaconDic[tmpList[i].Key] += arcaconDic[tmpList[j].Key];
                            toRemove.Add(tmpList[j].Key);
                        }
                    }
                }
                foreach (var str in toRemove)
                {
                    arcaconDic.Remove(str);
                }
            }

            var arcaconDicDesc = arcaconDic.OrderByDescending(x => x.Value);

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("//아카콘 랭킹");
            sb.AppendLine($"총 댓글 / 아카콘 = {posts.Sum(x => x.comments.Count)} / {arcaconDicDesc.Sum(x => x.Value)}");
            sb.AppendLine("아카콘 링크, 사용 횟수");
            foreach (var dic in arcaconDicDesc)
            {
                sb.AppendLine($"{"https:" + dic.Key}, {dic.Value}");
            }

            return sb.ToString();
        }

        string ExportRank4(int index)
        {
            var posts = GetDataFile(index);
            // ID, 집계 수
            var arcacons = new Dictionary<Arcacon, int>();
            foreach (var a in from post in posts from comment in post.comments where comment.isArcacon select new Arcacon(comment.dataId, comment.content))
            {
                if (arcacons.ContainsKey(a) == false)
                {
                    arcacons.Add(a, 1);
                }
                else
                {
                    arcacons[a]++;
                }
            }

            // ID, 실제 링크
            var memoDic = new Dictionary<Arcacon, int>();
            // 어디 아카콘 종류인지 저장하는 메모 리스트
            var arcaconPacks = new List<ArcaconPack>();
            foreach (var currentArcacon in arcacons.Select(arcacon => arcacon.Key))
            {
                if (memoDic.ContainsKey(currentArcacon) == false)
                {
                    // 메모이제이션
                    foreach (var arcaconPack in from arcaconPack in arcaconPacks from packArcacon in arcaconPack.arcacons where currentArcacon.address == packArcacon.address select arcaconPack)
                    {
                        memoDic.Add(currentArcacon, arcaconPack.id);
                        goto OUT;
                    }

                    // 처음보는 아카콘일 경우

                    // 아카콘 추가
                    var redirectedUrl = new ArcaliveCrawler("asd").GetRedirectedUrl("https://arca.live/api/emoticon/shop/" + currentArcacon.dataid,
                        term: 100, doc: out var doc);
                    var number = int.Parse(redirectedUrl.Split('/').Last());
                    memoDic.Add(currentArcacon, number);

                    // 삭제된 아카콘일 경우
                    if (string.IsNullOrEmpty(doc.Text))
                        continue;

                    // 메모
                    var arcaconNodesImg = doc.DocumentNode.SelectNodes("//div[contains(@class, 'article-body')]/img");
                    var arcaconNodesVid = doc.DocumentNode.SelectNodes("//div[contains(@class, 'article-body')]/video");
                    var newArcaconPack = new ArcaconPack(number);
                    if (arcaconNodesImg != null)
                    {
                        foreach (var arcaconNode in arcaconNodesImg)
                        {
                            var src = arcaconNode.Attributes["src"].Value;
                            var newArcacon = new Arcacon(src);
                            newArcaconPack.arcacons.Add(newArcacon);
                        }
                    }
                    if (arcaconNodesVid != null)
                    {
                        foreach (var arcaconNode in arcaconNodesVid)
                        {
                            var src = arcaconNode.Attributes["src"].Value.EndsWith("mp4")
                                ? arcaconNode.Attributes["src"].Value + ".gif"
                                : arcaconNode.Attributes["src"].Value;
                            var newArcacon = new Arcacon(src);
                            newArcaconPack.arcacons.Add(newArcacon);
                        }
                    }
                    arcaconPacks.Add(newArcaconPack);
                }

                OUT:;
            }

            // 실제 링크, 집계수
            var rankDic = new Dictionary<int, int>();
            foreach (var i in memoDic.Where(i => i.Value != -1))
            {
                if (rankDic.ContainsKey(i.Value) == false)
                {
                    rankDic.Add(i.Value, arcacons[i.Key]);
                }
                else
                {
                    rankDic[i.Value] += arcacons[i.Key];
                }
            }

            var rankDicDesc = rankDic.OrderByDescending(x => x.Value);

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("//아카콘 종류별 랭킹");
            sb.AppendLine($"아카콘 종류: {rankDicDesc.Count()}");
            //sb.AppendLine("삭제된 아카콘은 집계되지 않습니다.");
            sb.AppendLine("아카콘 링크, 총 사용 횟수");
            foreach (var dic in rankDicDesc)
            {
                sb.AppendLine($"{"https://arca.live/e/" + dic.Key}, {dic.Value}");
            }

            return sb.ToString();
        }
    }
}
