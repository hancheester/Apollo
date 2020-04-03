using Apollo.FrontStore.Validators.Product;
using FluentValidation.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Apollo.FrontStore.Models.Product
{
    [Validator(typeof(ProductReviewValidator))]
    public class AddProductReviewModel
    {
        public string Name { get; set; }
        public string UrlKey { get; set; }        
        public ProductBreadcrumbModel ProductBreadcrumb { get; set; }
        public ProductBoxModel ProductBox { get; set; }

        [AllowHtml]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [AllowHtml]
        [Display(Name = "Review")]
        public string Comment { get; set; }

        [Display(Name = "Nickname")]
        public string Alias { get; set; }

        [Display(Name = "Rating")]
        public int Rating { get; set; }

        public bool SuccessfullyAdded { get; set; }
        public string Result { get; set; }
    }
}