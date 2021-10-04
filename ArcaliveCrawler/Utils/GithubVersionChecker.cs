﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace ArcaliveCrawler.Utils
{
    public class GithubVersionChecker
    { 
        private string url { get;}

        public GithubVersionChecker(string url)
        {
            this.url = url;
        }

        public string LatestRelease
        {
            get
            {
                string tmp = string.Empty;
                using (WebClient wc = new WebClient())
                {
                    tmp = wc.DownloadString(url);
                }

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(tmp);
                var releases = doc.DocumentNode.SelectNodes("//div[contains(@class, 'release-entry')]");
                var latestRelease = releases[0].SelectSingleNode("//div[contains(@class, 'd-flex flex-items-start')]/div[1]/a").InnerText;
                return latestRelease;
            }
        }
    }
}
