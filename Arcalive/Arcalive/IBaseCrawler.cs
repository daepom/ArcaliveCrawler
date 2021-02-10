using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcalive
{
    public interface IBaseCrawler<T>
    {
        List<T> CrawlBoards(DateTime? From = null, DateTime? To = null, int startPage = 1);
        List<T> CrawlPosts(List<T> Posts, List<string> skip = null);
    }
}
