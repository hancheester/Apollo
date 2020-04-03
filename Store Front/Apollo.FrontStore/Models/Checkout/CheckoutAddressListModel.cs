using Apollo.Core.Model;
using Apollo.FrontStore.Models.Common;
using System.Collections.Generic;

namespace Apollo.FrontStore.Models.Checkout
{
    public class CheckoutAddressListModel
    {
        public IList<AddressModel> ExistingAddresses { get; set; }
        public AddressType Type { get; set; }

        /// <summary>
        /// Used on one-page checkout page
        /// </summary>
        public bool NewAddressPreselected { get; set; }

        public CheckoutAddressListModel()
        {
            ExistingAddresses = new List<AddressModel>();        
        }
    }
}