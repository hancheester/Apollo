using System;
using System.Collections.Generic;

namespace Apollo.FrontStore.Models
{
    public class CategoryFilterRangeFilterModel
    {
        public IList<Tuple<int, string, int>> Filters { get; set; }
        public int[] SelectedFilters { get; set; }

        public CategoryFilterRangeFilterModel()
        {
            Filters = new List<Tuple<int, string, int>>();
        }
    }
}