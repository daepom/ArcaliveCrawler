using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler
{
    public class EventLogger
    {
        private int _number = 0;

        public event EventHandler<CrawlLogMessageInfo> LogEventHandler;
        private BaseCrawler parent;
        public EventLogger(BaseCrawler parent)
        {
            this.parent = parent;
        }

        private int CurrentId => _number++;

        public void Log(string str)
        {
            LogEventHandler?.Invoke(this, new CrawlLogMessageInfo(str, CrawlLogMessageInfo.MessageLevel.Log, CurrentId, parent));
        }

        public void Log(params object[] objs)
        {
            Log(string.Join("/", objs));
        }

        public void Warn(string str)
        {
            LogEventHandler?.Invoke(this, new CrawlLogMessageInfo(str, CrawlLogMessageInfo.MessageLevel.Warn, CurrentId, parent));
        }

        public void Error(string str)
        {
            LogEventHandler?.Invoke(this, new CrawlLogMessageInfo(str, CrawlLogMessageInfo.MessageLevel.Error, CurrentId, parent));
        }
    }

    public struct CrawlLogMessageInfo
    {
        public int id;
        public string message;
        public CrawlLogMessageInfo(string message, MessageLevel level, int id, BaseCrawler parent)
        {
            this.message = message;
            this.level = level;
            this.id = id;
            this.parent = parent;
        }

        public BaseCrawler parent;

        public enum MessageLevel
        {
            Log = 0, Warn, Error
        }

        

        public MessageLevel level;

        public override string ToString()
        {
            return $"{id,5}/{level,5}/{message}";
        }
    }
}
