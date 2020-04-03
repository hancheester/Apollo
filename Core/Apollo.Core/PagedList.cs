using System;
using System.Collections.Generic;
using System.Linq;

namespace Apollo.Core
{
    /// <summary>
    /// Paged list
    /// </summary>
    /// <typeparam name="T">T</typeparam>
    [Serializable]
    //public class PagedList<T> : List<T>, IPagedList<T>
    public class PagedList<T> : IPagedList<T>
    {
        IList<T> _items;
        public IList<T> Items
        {
            get { return _items; }
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="source">source</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        public PagedList(IQueryable<T> source, int pageIndex, int pageSize)
        {
            int total = source.Count();
            this.TotalCount = total;
            this.TotalPages = total / pageSize;

            if (total % pageSize > 0)
                TotalPages++;

            this.PageSize = pageSize;
            this.PageIndex = pageIndex;
            _items = source == null ? new List<T>() : source.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            //this.AddRange(source.Skip(pageIndex * pageSize).Take(pageSize).ToList());
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="source">source</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        public PagedList(IList<T> source, int pageIndex, int pageSize)
        {
            TotalCount = source.Count();
            TotalPages = TotalCount / pageSize;

            if (TotalCount % pageSize > 0)
                TotalPages++;

            this.PageSize = pageSize;
            this.PageIndex = pageIndex;
            _items = source == null ? new List<T>() : source.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            //this.AddRange(source.Skip(pageIndex * pageSize).Take(pageSize).ToList());
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="source">source</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="totalCount">Total count</param>
        public PagedList(IEnumerable<T> source, int pageIndex, int pageSize, int totalCount)
        {
            TotalCount = totalCount;
            TotalPages = TotalCount / pageSize;

            if (TotalCount % pageSize > 0)
                TotalPages++;

            this.PageSize = pageSize;
            this.PageIndex = pageIndex;
            _items = source == null ? new List<T>() : source.ToList();
            //this.AddRange(source);
        }

        public PagedList(IEnumerable<T> source, int pageIndex, int pageSize, int totalCount, 
            decimal maxPriceFilterByKeyword, decimal minPriceFilterByKeyword)
        {
            TotalCount = totalCount;
            TotalPages = TotalCount / pageSize;

            if (TotalCount % pageSize > 0)
                TotalPages++;

            this.PageSize = pageSize;
            this.PageIndex = pageIndex;
            _items = source == null ? new List<T>() : source.ToList();
            //this.AddRange(source);

            this.MaxPriceFilterByKeyword = maxPriceFilterByKeyword;
            this.MinPriceFilterByKeyword = MinPriceFilterByKeyword;
        }

        public decimal MaxPriceFilterByKeyword { get; private set; }
        public decimal MinPriceFilterByKeyword { get; private set; }

        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public int TotalPages { get; private set; }

        public bool HasPreviousPage
        {
            get { return (PageIndex > 0); }
        }
        public bool HasNextPage
        {
            get { return (PageIndex + 1 < TotalPages); }
        }
    }
}
