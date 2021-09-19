using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Crawler
{

    public static class ArcaliveDocDownloader
    {
        public static readonly string AppUserAgent = "live.arca.android/0.8.214";

        public static HtmlDocument DownloadDoc(string link, int term = 0)
        {
            HtmlDocument doc = new HtmlDocument();
            using (WebClient client = new WebClient { Encoding = Encoding.UTF8 })
            {
                string siteSource = string.Empty;
                client.Headers.Add("user-agent", AppUserAgent);
                try
                {
                    siteSource = client.DownloadString(link);
                }
                catch (WebException e)
                {
                }
                catch (Exception e)
                {
                }
                finally
                {
                    doc.LoadHtml(siteSource);
                }
            }

            // HTML 429 에러 방지용
            // 100 이상으로 설정하는 것을 권장
            // 하지만 공앱 헤더를 쓰면 쓸 필요가 없어짐
            Thread.Sleep(term);

            return doc;
        }

    }
}
