using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using Apollo.Core.Services.Interfaces.DataBuilder;
using System;

namespace Apollo.Core.Services.DataBuilder
{
    public class AddressBuilder : IAddressBuilder
    {
        private readonly IShippingService _shippingService;

        public AddressBuilder(IShippingService shippingService)
        {
            _shippingService = shippingService;
        }

        public Address Build(Address address)
        {
            if (address == null) throw new ArgumentNullException("address");

            if (address.CountryId > 0) address.Country = _shippingService.GetCountryById(address.CountryId);
            if (address.USStateId > 0) address.USState = _shippingService.GetUSStateById(address.USStateId);

            return address;
        }
    }
}
