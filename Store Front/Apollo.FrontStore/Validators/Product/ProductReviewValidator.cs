using Apollo.FrontStore.Models.Product;
using Apollo.Web.Framework.Validators;
using FluentValidation;

namespace Apollo.FrontStore.Validators.Product
{
    public class ProductReviewValidator : BaseApolloValidator<AddProductReviewModel>
    {
        public ProductReviewValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required.");
            RuleFor(x => x.Title).Length(1, 100).WithMessage("Maximum length of title is 100 characters.");
            RuleFor(x => x.Comment).NotEmpty().WithMessage("Review is required.");
            RuleFor(x => x.Comment).Length(1, 300).WithMessage("Maximum length of product review is 300 characters.");
            RuleFor(x => x.Alias).NotEmpty().WithMessage("Nickname is required.");
            RuleFor(x => x.Alias).Length(1, 100).WithMessage("Maximum length of alias is 100 characters.");
        }
    }
}