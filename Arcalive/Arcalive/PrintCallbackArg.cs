using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcalive
{
    public class PrintCallbackArg : EventArgs
    {
        public string Str { get; }

        public PrintCallbackArg(string callbackString)
        {
            Str = callbackString;
        }
    }
}
