using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcaliveForm
{
    public class OptionsCallBack : EventArgs
    {
        public OptionsCallBack(OptionsClass options)
        {
            Options = options;
        }

        public OptionsClass Options { get; set; }
    }
}
