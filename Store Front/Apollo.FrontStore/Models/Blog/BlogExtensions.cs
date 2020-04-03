using Apollo.Core.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apollo.FrontStore.Models.Blog
{
    public static class BlogExtensions
    {
        public static string[] ParseTags(this BlogPost blogPost)
        {
            if (blogPost == null) throw new ArgumentNullException("blogPost");

            var parsedTags = new List<string>();
            if (!string.IsNullOrEmpty(blogPost.Tags))
            {
                string[] tags2 = blogPost.Tags.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string tag2 in tags2)
                {
                    var tmp = tag2.Trim();
                    if (!string.IsNullOrEmpty(tmp))
                        parsedTags.Add(tmp);
                }
            }
            return parsedTags.ToArray();
        }

        public static IList<BlogPost> GetPostsByDate(this IList<BlogPost> source, DateTime dateFrom, DateTime dateTo)
        {
            return source.Where(p => dateFrom.Date <= p.CreatedOnDate && p.CreatedOnDate.Date <= dateTo).ToList();
        }
    }
}