using Apollo.FrontStore.Models.Blog;
using Apollo.Web.Framework.Validators;
using FluentValidation;

namespace Apollo.FrontStore.Validators.Blog
{
    public class BlogPostValidator : BaseApolloValidator<BlogPostModel>
    {
        public BlogPostValidator()
        {
            RuleFor(x => x.CommentText).NotEmpty().WithMessage("Comment is required.");
        }
    }
}