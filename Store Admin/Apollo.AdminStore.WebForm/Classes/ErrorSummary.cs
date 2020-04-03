using System.Web.UI;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.Classes
{
    public class ErrorSummary : BaseValidator
    {
        public static void AddError(string message, string validationGroup, Page page)
        {
            ErrorSummary error = new ErrorSummary(message, validationGroup);
            error.IsValid = false;
            error.ErrorMessage = message;

            page.Validators.Add(error);
        }

        public ErrorSummary(string errMessage, string validationGroup)
        {
            base.ErrorMessage = errMessage;
            base.IsValid = false;
            base.ValidationGroup = validationGroup;
        }

        protected override bool EvaluateIsValid()
        {
            return false;
        }
    }
}