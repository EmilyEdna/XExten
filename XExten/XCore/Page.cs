using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XExten.XCore
{
    /// <summary>
    /// paging
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Page<T>
    {
        /// <summary>
        /// Total
        /// </summary>
        public int Total { get; set; }
        /// <summary>
        /// TotalPage
        /// </summary>
        public int TotalPage { get; set; }
        /// <summary>
        /// CurrentPage
        /// </summary>
        public int CurrentPage { get; set; }
        /// <summary>
        ///  Result
        /// </summary>
        public IQueryable<T> Queryable { get; set; }
    }
    /// <summary>
    /// Not Map
    /// </summary>
    public class IgnoreMappedAttribute : Attribute
    {

    }
}
