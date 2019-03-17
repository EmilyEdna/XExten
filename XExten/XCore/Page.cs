using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XExten.XCore
{
    public class Page<T>
    {
        public int Total { get; set; }
        public int TotalPage { get; set; }
        public int CurrentPage { get; set; }
        public IQueryable<T> Queryable { get; set; }
    }
}
