using System;

namespace Apollo.Core.Model.Entity
{
    public class OfferBanner : BaseEntity
    {
        public string MediaFilename { get; set; }     
        public string MediaAlt { get; set; }
        public string Link { get; set; }
        public string Title { get; set; }
        public bool Enabled { get; set; }
        public int Priority { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public OfferBanner()
        {
            MediaFilename = string.Empty;
            MediaAlt = string.Empty;
            Link = string.Empty;
            Title = string.Empty;
        }
    }
}
