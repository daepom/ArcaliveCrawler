using System.Collections;
using System.Collections.Generic;

namespace ArcaliveForm
{

    public class OptionsClass
    {
        public OptionsClass()
        {

        }

        /// <summary>
        /// 1 ~> startPage부터, 2 ~> 이진탐색
        /// </summary>
        public int StartPageFinding { get; set; }
        public int StartPage { get; set; }
        public List<string> SkippingTags { get; set; }
        public int CrawlingSolution { get; set; }
    }
}