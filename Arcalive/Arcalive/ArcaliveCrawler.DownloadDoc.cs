using HtmlAgilityPack;
using System.Net;
using System.Text;
using System.Threading;

namespace Arcalive
{
    public partial class ArcaliveCrawler
    {
        private HtmlDocument DownloadDoc(
            string link, string userAgent = "live.arca.android/0.8.207", int term = 0)
        {
            HtmlDocument doc = new HtmlDocument();
            using (WebClient client = new WebClient() { Encoding = Encoding.UTF8 })
            {
                string siteSource = string.Empty;
                client.Headers.Add("user-agent", userAgent);
                try
                {
                    siteSource = client.DownloadString(link);
                }
                catch (WebException e)
                {
                    int statusCode = (int)(e.Response as HttpWebResponse).StatusCode;
                    Print?.Invoke(this, new PrintCallbackArg($"{CallTimes++,5} >> DownloadDoc >> HTML {statusCode} Error"));
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

        private HtmlDocument DownloadDoc(string link, WebHeaderCollection headers, int term)
        {
            HtmlDocument doc = new HtmlDocument();
            using (WebClient client = new WebClient() { Encoding = Encoding.UTF8 })
            {
                string siteSource = string.Empty;
                client.Headers = headers;
                try
                {
                    siteSource = client.DownloadString(link);
                }
                catch (WebException e)
                {
                    int statusCode = (int)(e.Response as HttpWebResponse).StatusCode;
                    Print?.Invoke(this, new PrintCallbackArg($"{CallTimes++,5} >> DownloadDoc >> HTML {statusCode} Error"));
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