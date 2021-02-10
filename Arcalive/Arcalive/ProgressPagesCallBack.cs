using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcalive
{
    public class ProgressPagesCallBack : EventArgs
    {
        public int CurrentPage { get; }
        public int TotalPages { get; }

        public int Time { get; }

        public ProgressPagesCallBack(int currentPage, int totalPages, int time = 0)
        {
            CurrentPage = currentPage;
            TotalPages = totalPages;
            Time = time;
        }
    }
}
