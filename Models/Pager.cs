using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Web;

namespace examination_system.Models
{
    public class Pager
    {
        public int TotalItems { get; private set; }
        public int CurrentPage { get; private set; }
        public int PageSize { get; private set; }

        public int TotalPages { get; private set; }
        public int StartPage { get; private set; }
        public int EndPage { get; private set; }
        public Pager() { }
        public Pager(int TotalItems,int CurrentPage,int PageSize) {
            int TotalPages = (int)Math.Ceiling((decimal)TotalItems / (decimal)PageSize);
            int StartPage = CurrentPage-5;
            int EndPage = CurrentPage+5;
            if (StartPage <= 0) {
                EndPage = EndPage-(StartPage-1);
                StartPage = 1;
            }
            if (EndPage > TotalPages) {
                EndPage = TotalPages;
                if (EndPage > 11) {
                    StartPage = EndPage-10;
                }
            }
            this.TotalItems = TotalItems;
            this.CurrentPage = CurrentPage;
            this.PageSize = PageSize;
            this.TotalPages = TotalPages;
            this.StartPage = StartPage;
            this.EndPage = EndPage;
        }
    }
}