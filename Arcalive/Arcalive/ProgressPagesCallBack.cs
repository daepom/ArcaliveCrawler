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

        public ProgressPagesCallBack(int currentPage, int totalPages)
        {
            CurrentPage = currentPage;
            TotalPages = totalPages;
        }
    }
}
