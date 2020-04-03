using Apollo.Core.Caching;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.DataAccess.Interfaces;
using System.Linq;

namespace Apollo.Core.Services
{
    public static class EntityExtensions
    {
        #region Constants

        private const string COLOUR_BY_ID_KEY = "colour.id-{0}";
             
        #endregion

        public static string GetOption(this ProductPrice price, OptionType type, ICacheManager cacheManager, IRepository<Colour> colourRepository)
        {
            var option = string.Empty;
            switch (type)
            {
                case OptionType.Size:
                    option = price.Size;
                    break;
                case OptionType.Colour:
                    if (price.ColourId.HasValue)
                    {
                        string key = string.Format(COLOUR_BY_ID_KEY, price.ColourId.Value);

                        var colour = cacheManager.Get(key, delegate
                        {
                            var optionColour = colourRepository.TableNoTracking.Where(x => x.Id == price.ColourId.Value).FirstOrDefault();
                            return optionColour;
                        });                        

                        if (colour != null) option = colour.Value;
                    }
                    break;
                case OptionType.None:
                case OptionType.GiftCard:                    
                default:
                    break;
            }

            return option;
        }

        public static decimal GetPrice(this CartItem cartItem, bool inclTax = true)
        {
            var price = 0M;
            
            switch ((CartItemMode)cartItem.CartItemMode)
            {
                case CartItemMode.InitialPrice:
                    price = inclTax ? cartItem.ProductPrice.PriceInclTax : cartItem.ProductPrice.PriceExclTax;
                    break;
                case CartItemMode.FreeItem:
                    break;
                default:
                    price = inclTax ? cartItem.ProductPrice.OfferPriceInclTax : cartItem.ProductPrice.OfferPriceExclTax;
                    break;
            }

            return price;
        }

        public static LineItem ToLineItem(this CartItem cartItem, int orderId, string currencyCode, decimal exchangeRate, string lineStatus)
        {
            return new LineItem
            {
                OrderId = orderId,
                ProductPriceId = cartItem.ProductPriceId,
                ProductId = cartItem.ProductId,
                Name = cartItem.Product.Name,
                Option = cartItem.ProductPrice.Option,
                PriceInclTax =  cartItem.GetPrice(),
                PriceExclTax = cartItem.GetPrice(inclTax: false),
                InitialPriceInclTax = cartItem.ProductPrice.PriceInclTax,
                InitialPriceExclTax = cartItem.ProductPrice.PriceExclTax,
                Quantity = cartItem.Quantity,
                PendingQuantity = cartItem.Quantity,
                Wrapped = false,                
                IsPharmaceutical = cartItem.Product.IsPharmaceutical,
                Weight = cartItem.ProductPrice.Weight,
                CostPrice = cartItem.ProductPrice.CostPrice,
                ExchangeRate = exchangeRate,
                CurrencyCode = currencyCode,
                StatusCode = lineStatus
            };
        }

        public static string GetDefaultOption(this Product product)
        {
            var option = string.Empty;
            switch ((OptionType)product.OptionType)
            {
                case OptionType.Size:
                    var activeSizes = product.ProductPrices.Where(pp => pp.Enabled == true).ToList();
                    //if (activeSizes.Count > 1)
                    //    options = activeSizes.FirstOrDefault().Size + " - " + activeSizes.LastOrDefault().Size;
                    //else if (activeSizes.Count == 1)
                    //    options = activeSizes.FirstOrDefault().Size;

                    if (activeSizes.Count == 1) option = activeSizes.FirstOrDefault().Size;
                    break;

                case OptionType.Colour:
                case OptionType.None:
                default:
                    break;
            }

            return option;
        }

        public static string GetOptionString(this Product product)
        {
            var options = string.Empty;
            var activeOptions = product.ProductPrices.Where(pp => pp.Enabled == true).ToList();
            switch ((OptionType)product.OptionType)
            {
                case OptionType.Size:                    
                    if (activeOptions.Count > 1) options = string.Format("{0} options", activeOptions.Count());
                    break;

                case OptionType.Colour:
                    if (activeOptions.Count > 1) options = string.Format("{0} colours", activeOptions.Count());
                    break;
                case OptionType.None:
                default:
                    break;
            }

            return options;
        }
    }
}
