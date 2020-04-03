using System.Collections.Generic;

namespace Apollo.FrontStore.Models.Blog
{
    public partial class BlogPostYearModel
    {
        public BlogPostYearModel()
        {
            Months = new List<BlogPostMonthModel>();
        }
        public int Year { get; set; }
        public IList<BlogPostMonthModel> Months { get; set; }
    }

    public partial class BlogPostMonthModel
    {
        public int Month { get; set; }

        public int BlogPostCount { get; set; }
    }
}