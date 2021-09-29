using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler
{
    // 크롤링 데이터를 담는 클래스
    // 필요한 것: PostInfo 리스트, 
    [Serializable]
    public class CrawlData
    {
        #region Fields
        private readonly List<PostInfo> _posts = new List<PostInfo>();
        #endregion

        public PostInfo this[int index]
        {
            get => _posts[index];
            set => _posts[index] = value;
        }
    }
}
