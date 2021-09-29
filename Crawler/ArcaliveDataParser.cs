using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crawler
{
    /// <summary>
    /// HtmlNode를 통해 Raw 데이터를 데이터 클래스에 바인딩하는 메서드가 들어있어요
    /// </summary>
    public static class ArcaliveDataParser
    {
        public static void ParsePostData(this ArcalivePostInfo outInfo)
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
            else
            {
                // (권한 없음)
                outInfo.restricted = true;
                return;
            }
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
            // TODO: 방식을 위에꺼처럼 더 안전하게 바꾸기
            var pSource = outInfo.postSource;
            if (pSource == null)
                return;
            var articleInfoNode = pSource.SelectSingleNode("//div[contains(@class, 'article-info')]");
            var contentNode = pSource.SelectSingleNode("//div[contains(@class, 'fr-view article-content')]");
            var commentNumNode = articleInfoNode.SelectSingleNode(".//span[8]");
            var viewNumNode = articleInfoNode.SelectSingleNode(".//span[11]");
            outInfo.content = contentNode.InnerText;
            outInfo.view = int.Parse(viewNumNode.InnerText);
            if (int.Parse(commentNumNode.InnerText) == 0) goto Done;
            var comments = outInfo.comments;
            if (pSource.SelectSingleNode("//div[contains(@class, 'pagination-wrapper my-2')]") != null)
            {
                int maxCommentPageNumber = int.Parse(pSource.SelectSingleNode("//div[contains(@class, 'pagination-wrapper my-2')]/ul/li[contains(@class, 'page-item active')]/a").InnerText);
                
                var commentAreaNode = pSource.SelectSingleNode("//div[contains(@class, 'list-area')]");
                comments.AddRange(ParseCommentData(commentAreaNode));
                for (int k = maxCommentPageNumber - 1; k >= 1; k--)
                {
                    HtmlDocument commentDoc = ArcaliveDocDownloader.DownloadDoc(outInfo.href + "?cp=" + k);
                    commentAreaNode = commentDoc.DocumentNode.SelectSingleNode("//div[contains(@class, 'list-area')]");
                    comments.AddRange(ParseCommentData(commentAreaNode));
                }
            }
            else
            {
                var commentAreaNode = pSource.SelectSingleNode("//div[contains(@class, 'list-area')]");
                comments.AddRange(ParseCommentData(commentAreaNode));
            }

            Done:
            outInfo.ClearSourceData();
            outInfo.completed = true;
        }

        private static List<CommentInfo> ParseCommentData(HtmlNode commentNode)
        {
            var commentWrappers = commentNode?.Descendants(0)?
                .Where(n => n.HasClass("comment-item")).ToList();
            if (commentWrappers == null)
                return new List<CommentInfo>();
            var lst = new List<CommentInfo>(commentWrappers.Count);
            for (int i = 0; i < commentWrappers.Count; i++)
            {
                lst.Add(null);
            }
            ArcaliveCommentInfo ParseComment(HtmlNode commentWrapper)
            {
                var c = new ArcaliveCommentInfo();
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
                c.dt = time;
                return c;
            }

            //for (int i = 0; i < lst.Count; i++)
            //{
            //    lst[i] = ParseComment(commentWrappers[i]);
            //}

            Parallel.For(0, lst.Count, i =>
            {
                lst[i] = ParseComment(commentWrappers[i]);
            });
            return lst;
        }
    }
}