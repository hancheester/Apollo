using FluentValidation;

namespace Apollo.Web.Framework.Validators
{
    public abstract class BaseApolloValidator<T> : AbstractValidator<T> where T : class
    {
        protected BaseApolloValidator()
        {
            PostInitialize();
        }

        /// <summary>
        /// Developers can override this method in custom partial classes
        /// in order to add some custom initialization code to constructors
        /// </summary>
        protected virtual void PostInitialize()
        {

        }
    }
}
