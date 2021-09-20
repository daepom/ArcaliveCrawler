using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        public static readonly string AppUserAgent = "live.arca.android/0.8.272";

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
        public static string RedirectedUrl(string baseLink, out HtmlDocument doc, int term = 0)
        {
            Stopwatch sp = new Stopwatch();
            sp.Start();

            var request = (HttpWebRequest)WebRequest.Create(baseLink);
            request.UserAgent = AppUserAgent;
            request.AllowAutoRedirect = false;
            var response1 = (HttpWebResponse)request.GetResponse();
            string result = response1.Headers["location"];
            response1.Close();

            request = (HttpWebRequest)WebRequest.Create("https://arca.live" + result);
            request.UserAgent = AppUserAgent;
            string html = string.Empty;
            doc = new HtmlDocument();
            try
            {
                var response2 = (HttpWebResponse)request.GetResponse();
                using (StreamReader sr = new StreamReader(response2.GetResponseStream()))
                {
                    html = sr.ReadToEnd();
                }
            }
            catch
            {
                // Do Nothing
            }
            finally
            {
                doc.LoadHtml(html);
            }

            sp.Stop();

            int tick = (int)sp.ElapsedMilliseconds;
            Thread.Sleep(term - tick > 0 ? term - tick : 0);

            return result;
        }
    }
}
