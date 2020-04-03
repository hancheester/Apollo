using Apollo.Core.Model;
using Apollo.Core.Model.Entity;

namespace Apollo.Core
{
    /// <summary>
    /// Work context
    /// </summary>
    public interface IWorkContext
    {
        /// <summary>
        /// Gets or sets the current profile
        /// </summary>
        Profile CurrentProfile { get; set; }
        
        /// <summary>
        /// Gets or sets the original customer (in case the current one is impersonated)
        /// </summary>
        //Customer OriginalCustomerIfImpersonated { get; }
        
        /// <summary>
        /// Get or set current user working language
        /// </summary>
        //Language WorkingLanguage { get; set; }
        
        /// <summary>
        /// Get or set current user working currency
        /// </summary>
        Currency WorkingCurrency { get; set; }
        
        /// <summary>
        /// Get or set current tax display type
        /// </summary>
        TaxDisplayType TaxDisplayType { get; set; }

        Country CurrentCountry { get; set; }

        /// <summary>
        /// Get or set value indicating whether we're in admin area
        /// </summary>
        //bool IsAdmin { get; set; }
    }
}
