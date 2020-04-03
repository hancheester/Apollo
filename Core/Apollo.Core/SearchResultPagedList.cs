using Apollo.Core.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apollo.Core
{
    [Serializable]
    public class SearchResultPagedList<T> : IPagedList<T>
    {
        private string _suggestedKeywords;
        private bool _hasSearchTerm;
        private SearchTerm _searchTerm;
        private IList<T> _items;

        public string SuggestedKeywords
        {
            get { return _suggestedKeywords; }
        }
        public bool HasSearchTerm
        {
            get { return _hasSearchTerm; }
        }
        public SearchTerm SearchTerm
        {
            get { return _searchTerm; }
        }
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
        public SearchResultPagedList(IQueryable<T> source, int pageIndex, int pageSize, int totalCount)
        {
            this.TotalCount = totalCount;
            this.TotalPages = totalCount / pageSize;

            if (TotalCount % pageSize > 0)
                TotalPages++;

            this.PageSize = pageSize;
            this.PageIndex = pageIndex;
            _items = source.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            //this.AddRange(source.Skip(pageIndex * pageSize).Take(pageSize).ToList());
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="source">source</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        public SearchResultPagedList(IList<T> source, int pageIndex, int pageSize, int totalCount)
        {
            TotalCount = totalCount;
            TotalPages = TotalCount / pageSize;

            if (TotalCount % pageSize > 0)
                TotalPages++;

            this.PageSize = pageSize;
            this.PageIndex = pageIndex;
            _items = source.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            //this.AddRange(source.Skip(pageIndex * pageSize).Take(pageSize).ToList());            
        }

        public SearchResultPagedList(IEnumerable<T> source, int pageIndex, int pageSize, int totalCount,
            decimal maxPriceFilterByKeyword, decimal minPriceFilterByKeyword)
        {
            TotalCount = totalCount;
            TotalPages = TotalCount / pageSize;

            if (TotalCount % pageSize > 0)
                TotalPages++;

            this.PageSize = pageSize;
            this.PageIndex = pageIndex;
            _items = source.ToList();
            //this.AddRange(source);
            
            this.MaxPriceFilterByKeyword = maxPriceFilterByKeyword;
            this.MinPriceFilterByKeyword = minPriceFilterByKeyword;
        }

        public SearchResultPagedList(IEnumerable<T> source, int pageIndex, int pageSize, int totalCount, 
            string suggestedKeywords, decimal maxPriceFilterByKeyword, decimal minPriceFilterByKeyword)
        {
            TotalCount = totalCount;
            TotalPages = TotalCount / pageSize;

            if (TotalCount % pageSize > 0)
                TotalPages++;

            this.PageSize = pageSize;
            this.PageIndex = pageIndex;
            _items = source.ToList();
            //this.AddRange(source);

            this._suggestedKeywords = suggestedKeywords;
            this.MaxPriceFilterByKeyword = maxPriceFilterByKeyword;
            this.MinPriceFilterByKeyword = minPriceFilterByKeyword;
        }

        public SearchResultPagedList(SearchTerm searchTerm)
        {
            this._searchTerm = searchTerm;
            this._hasSearchTerm = true;
        }

        public SearchResultPagedList(IList<T> source)
        {
            this._items = source.ToList();
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
