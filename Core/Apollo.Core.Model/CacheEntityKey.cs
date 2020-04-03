using System;

namespace Apollo.Core.Model
{
    [Flags]
    public enum CacheEntityKey
    {
        Product = 1,
        Brand = 2,
        Category = 4,
        Offer = 8,
        Campaign = 16,
        Setting = 32,
        LargeBanner = 64,
        Currency = 128,
        Country = 256,
        ShippingOption = 512,
        CustomDictionary = 1024,
        Blog = 2018,
        Widget = 4036
    }
}