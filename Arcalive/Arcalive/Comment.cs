using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcalive
{
    [Serializable]
    public struct Comment
    {
        public string author;
        public bool isArcacon;
        public string content;
        public DateTime time;
        public int dataId;
    }
}
