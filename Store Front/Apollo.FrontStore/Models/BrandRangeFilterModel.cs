using System;
using System.Collections.Generic;

namespace Apollo.FrontStore.Models
{
    public class BrandRangeFilterModel
    {
        public IList<Tuple<int, string, int>> Brands { get; set; }
        public int[] SelectedBrands { get; set; }

        public BrandRangeFilterModel()
        {
            Brands = new List<Tuple<int, string, int>>();
        }
    }
}