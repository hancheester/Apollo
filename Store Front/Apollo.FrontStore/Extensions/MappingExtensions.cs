using Apollo.Core;
using Apollo.Core.Domain.Orders;
using Apollo.Core.Infrastructure;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Model.OverviewModel;
using Apollo.Core.Services.Interfaces;
using Apollo.FrontStore.Models;
using Apollo.FrontStore.Models.Checkout;
using Apollo.FrontStore.Models.Common;
using Apollo.FrontStore.Models.Media;
using Apollo.FrontStore.Models.Order;
using Apollo.FrontStore.Models.Product;
using Apollo.FrontStore.Models.ShoppingCart;
using Apollo.Web.Framework.Services.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Apollo.FrontStore.Extensions
{
    public static class MappingExtensions
    {
        public static AddressModel PrepareAddressModel(this Address address)
        {
            var model = new AddressModel
            {
                Id = address.Id,
                Name = address.Name,
                AddressLine1 = address.AddressLine1,
                AddressLine2 = address.AddressLine2,
                City = address.City,
                County = address.County,
                PostCode = address.PostCode,
                CountryId = address.CountryId.ToString(),
                CountryName = address.Country.Name,
                USStateId = address.USStateId,
                USState = address.USState == null ? string.Empty : address.USState.State, 
                IsBilling = address.IsBilling,
                IsShipping = address.IsShipping
            };

            return model;
        }

        public static Address PrepareAddressEntity(this AddressModel model)
        {
            var shippingService = EngineContext.Current.Resolve<IShippingService>();

            var entity = new Address
            {
                Name = model.Name,
                AddressLine1 = model.AddressLine1,
                AddressLine2 = model.AddressLine2,
                City = model.City,
                County = model.County,
                PostCode = model.PostCode,
                CountryId = Convert.ToInt32(model.CountryId)
            };

            var country = shippingService.GetCountryById(entity.CountryId);
            if (country.ISO3166Code == "US")
                entity.USStateId = model.USStateId.Value;

            return entity;
        }

        public static IList<EstimateShippingModel.ShippingOptionModel> PrepareShippingOptionModel(
            this IList<ShippingOptionOverviewModel> options,
            int selectedOptionId = 0)
        {
            var models = new List<EstimateShippingModel.ShippingOptionModel>();
            var priceFormatter = EngineContext.Current.Resolve<IPriceFormatter>();

            foreach (var option in options)
	        {
                var model = new EstimateShippingModel.ShippingOptionModel
                {
                    Id = option.Id,
                    Name = option.Name,
                    Description = option.Cost == 0M ? "FREE" : priceFormatter.FormatPrice(option.Cost),
                    Cost = option.Cost,
                    Selected = option.Id == selectedOptionId
                };
                models.Add(model);
            }

            return models;
        }
        
        public static IList<CartItemOverviewModel> ApplyMaximumQuantityRule(this IList<CartItemOverviewModel> items)
        {
            var cartService = EngineContext.Current.Resolve<ICartService>();
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            var models = new List<CartItemOverviewModel>();

            foreach (var item in items)
            {
                var maxQuantity = item.CalculateMaxQuantity();
                
                // Allowed quantities
                if (maxQuantity > 0)
                {
                    // If cart item has exceeded maximum allowed quantity, then remove it from cart
                    if (maxQuantity < item.Quantity)
                    {
                        cartService.DeleteCartItemsByProfileIdAndCartItemId(workContext.CurrentProfile.Id, item.Id);
                    }
                    else
                    {
                        models.Add(item);
                    }
                }
                else
                {
                    cartService.DeleteCartItemsByProfileIdAndCartItemId(workContext.CurrentProfile.Id, item.Id);
                }
            }

            return models;
        }

        public static IList<CartItemModel> PrepareCartItemModels(this IList<CartItemOverviewModel> items, bool IsEditable = true)
        {
            var models = new List<CartItemModel>();
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            var priceFormatter = EngineContext.Current.Resolve<IPriceFormatter>();
            
            foreach (var item in items)
            {
                var maxQuantity = item.CalculateMaxQuantity();
                var price = 0M;
                var oldPrice = 0M;
                var discount = 0M;
                var percentageSavingNote = string.Empty;
                
                // Do not include FREE item
                if (item.OfferPriceInclTax > 0)
                {
                    if (workContext.CurrentCountry.IsEC)
                    {
                        price = item.OfferPriceInclTax;
                        oldPrice = item.PriceInclTax;
                        discount = item.PriceInclTax - item.OfferPriceInclTax;
                        if (discount > 0)
                        {
                            percentageSavingNote = string.Format("SAVE {0} ({1:#.##}%)", priceFormatter.FormatPrice(discount), discount / oldPrice * 100);
                        }
                    }
                    else
                    {
                        price = item.OfferPriceExclTax;
                        oldPrice = item.PriceExclTax;
                        discount = item.PriceExclTax - item.OfferPriceExclTax;
                        if (discount > 0)
                            percentageSavingNote = string.Format("SAVE {0} ({1:#.##}%)", priceFormatter.FormatPrice(discount), discount / oldPrice * 100);
                    }
                }

                var model = new CartItemModel
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductPriceId = item.ProductPriceId,
                    Name = item.Name,
                    ThumbnailFilename = item.ThumbnailFilename,
                    Price = price,
                    OldPrice = oldPrice,
                    Discount = discount,
                    SavePercentageNote = percentageSavingNote,
                    Option = item.Option,
                    UrlKey = item.UrlKey,
                    Quantity = item.Quantity,
                    IsEditable = IsEditable
                };                
                
                if (item.OfferPriceExclTax <= 0)
                {
                    model.IsEditable = false;
                }
                
                for (int i = item.StepQuantity; i <= maxQuantity; i = i + item.StepQuantity)
                {
                    model.AllowedQuantities.Add(new SelectListItem
                    {
                        Text = i.ToString(),
                        Value = i.ToString()
                    });
                }                

                models.Add(model);                
            }

            return models;
        }

        public static IList<ProductBoxModel> PrepareProductBoxModels(this IList<ProductOverviewModel> products, string styleClass = "", string imageLoadType = "")
        {
            if (products == null) throw new ArgumentNullException("products");

            var models = new List<ProductBoxModel>();

            if (products.Count > 0)
            {
                foreach (var item in products)
                {
                    var model = PrepareProductBoxModel(item, styleClass, imageLoadType);
                    models.Add(model);
                }
            }

            return models;
        }

        public static IList<ProductBoxModel> PrepareProductBoxModels(this IList<OfferRelatedItem> items)
        {
            var models = new List<ProductOverviewModel>();

            if (items.Count > 0)
            {
                var productService = EngineContext.Current.Resolve<IProductService>();
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].Enabled)
                    {
                        var model = productService.GetProductOverviewModelById(items[i].ProductId);
                        if (model != null) models.Add(model);
                    }
                }
            }

            return models.PrepareProductBoxModels();
        }

        public static ProductBoxModel PrepareProductBoxModel(this ProductOverviewModel product, string styleClass = "", string imageLoadType = "")
        {
            if (product == null) throw new ArgumentNullException("product");
            
            var workContext = EngineContext.Current.Resolve<IWorkContext>();

            var model = new ProductBoxModel
            {
                Id = product.Id,
                Name = product.Name,
                DefaultOption = product.DefaultOption,
                Options = product.Options,
                UrlKey = product.UrlKey,
                StyleClass = styleClass,
                AverageReviewRating = product.AverageReviewRating,
                ReviewCount = product.ReviewCount,
                ProductMark = product.ProductMark,
                ProductMarkType = product.ProductMarkType,
                ProductMarkExpiryDate = product.ProductMarkExpiryDate,
                ShortDescription = product.FullDescription,
                StockAvailability = product.StockAvailability,
                CurrencyCode = workContext.WorkingCurrency.CurrencyCode,
                RelatedOffers = product.RelatedOffers,
                ImageLoadType = imageLoadType
            };

            #region Image

            var picture = new PictureModel
            {
                ImageUrl = string.IsNullOrEmpty(product.GridMediaFilename) ? "/content/img/no-image.gif" : product.GridMediaFilename,
                AlternateText = product.Name,
                Title = product.Name
            };

            model.Picture = picture;

            #endregion

            #region Stock availability

            if (product.StockAvailability == false)
            {
                if (product.Discontinued == true)
                {
                    model.DisableBuyButton = true;
                    model.AMPNote = model.Note = "DISCONTINUED";
                    model.ButtonMessage = "View";
                }
                else if (product.ProductEnforcedStockCount || product.BrandEnforcedStockCount)
                {
                    model.DisableBuyButton = true;
                    model.AMPNote = model.Note = "SOLD OUT";
                    model.ButtonMessage = "View";
                }
                else if (product.ShowPreOrderButton == true)
                {
                    model.AMPNote = model.Note = "AVAILABLE FOR PRE-ORDER";
                    model.ButtonMessage = "Pre-order Now";
                }
            }

            #endregion

            #region Price range
            
            if (product.PriceExclTaxRange != null && product.PriceExclTaxRange.Length > 0)
            {
                var priceFormatter = EngineContext.Current.Resolve<IPriceFormatter>();

                if (product.PriceExclTaxRange.Length == 1)
                {
                    if (workContext.CurrentCountry.IsEC)
                    {
                        model.PriceRange = priceFormatter.FormatPrice(product.PriceInclTaxRange[0]);
                        model.PriceValue = priceFormatter.FormatPrice(product.PriceInclTaxRange[0], showCurrency: false);
                    }
                    else
                    {
                        model.PriceRange = priceFormatter.FormatPrice(product.PriceExclTaxRange[0]);
                        model.PriceValue = priceFormatter.FormatPrice(product.PriceExclTaxRange[0], showCurrency: false);
                    }
                }
                else
                {
                    if (workContext.CurrentCountry.IsEC)
                    { 
                        model.PriceRange = priceFormatter.FormatPrice(product.PriceInclTaxRange[0]) + " - " + priceFormatter.FormatPrice(product.PriceInclTaxRange[1]);
                        model.PriceValue = priceFormatter.FormatPrice(product.PriceInclTaxRange[0], showCurrency: false);
                    }
                    else
                    { 
                        model.PriceRange = priceFormatter.FormatPrice(product.PriceExclTaxRange[0]) + " - " + priceFormatter.FormatPrice(product.PriceExclTaxRange[1]);
                        model.PriceValue = priceFormatter.FormatPrice(product.PriceExclTaxRange[0], showCurrency: false);
                    }
                }
            }

            #endregion

            return model;
        }

        public static IList<SpecialOfferModel.SingleOfferModel> PrepareSingleOfferModel(this IList<OfferRule> offers)
        {
            var models = new List<SpecialOfferModel.SingleOfferModel>();

            foreach (var offer in offers)
            {
                var model = new SpecialOfferModel.SingleOfferModel
                {
                    Alias = offer.Alias,
                    OfferTypeId = offer.OfferTypeId,
                    UrlKey = offer.UrlRewrite,
                    Image = new PictureModel { ImageUrl = offer.SmallImage, Title = offer.Alias, AlternateText = offer.Alias },
                    Description = offer.ShortDescription
                };

                models.Add(model);
            }

            return models;
        }

        public static IList<OfferRule> FilterOffers(this IList<OfferRule> offers, bool? displayOnHeaderStrip = null)
        {
            var filteredOffers = new List<OfferRule>();

            foreach (var offer in offers)
            {
                // Make sure it is within the date range.
                if ((offer.StartDate.HasValue == false || offer.StartDate.Value.CompareTo(DateTime.Now) <= 0) &&
                    (offer.EndDate.HasValue == false || offer.EndDate.Value.CompareTo(DateTime.Now) >= 0))                    
                {
                    // Only offer which is allowed to display on special offer page.
                    if (offer.ShowInOfferPage)
                    {
                        if (displayOnHeaderStrip.HasValue)
                        {
                            if (displayOnHeaderStrip.Value && offer.DisplayOnHeaderStrip)
                                filteredOffers.Add(offer);
                        }
                        else
                        {
                            filteredOffers.Add(offer);
                        }
                    }                        

                    // Stop here if "Stop futher rules processings" flag is false.
                    if (offer.ProceedForNext == false) break;
                }
            }

            return filteredOffers;
        }
        
        public static IList<SelectListItem> PrepareCountries(this IList<Country> countries)
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem
            {
                Text = "- Please select -",
                Value = string.Empty
            });

            for (int i = 0; i < countries.Count; i++)
            {
                list.Add(new SelectListItem
                {
                    Text = countries[i].Name,
                    Value = countries[i].Id.ToString()
                });
            }

            return list;
        }

        public static IList<SelectListItem> PrepareStates(this IList<USState> states)
        {
            var list = new List<SelectListItem>();
            for (int i = 0; i < states.Count; i++)
            {
                list.Add(new SelectListItem
                {
                    Text = states[i].State,
                    Value = states[i].Id.ToString()
                });
            }

            return list;
        }

        public static CartPharmOrder PrepareCartPharmOrderEntity(this PharmOrderModel model, int profileId)
        {
            var entity = new CartPharmOrder
            {
                ProfileId = profileId,
                TakenByOwner = model.TakenByOwner,
                OwnerAge = model.OwnerAge,
                HasOtherCondMed = model.HasCondition,
                OtherCondMed = model.OwnerCondition
            };

            return entity;
        }

        public static CartPharmItem PrepareCartPharmItemEntity(this PharmItemModel model)
        {
            var entity = new CartPharmItem
            {
                ProductId = model.ProductId,
                ProductPriceId = model.ProductPriceId,
                Symptoms = model.Symptoms,
                MedForSymptom = model.MedForSymptom,
                Age = model.Age,
                HasOtherCondMed = model.HasOtherCondMed,
                OtherCondMed = model.OtherCondMed,
                PersistedInDays = model.PersistedInDays,
                ActionTaken = model.ActionTaken,
                HasTaken = model.HasTaken.ToString(),
                TakenQuantity = model.HasTaken ? model.TakenQuantity : null,
                LastTimeTaken = model.HasTaken ? model.LastTimeTaken : null
            };

            return entity;
        }

        public static int CalculateMaxQuantity(this CartItemOverviewModel item)
        {
            return CalculateMaxQuantity(
                item.IsPharmaceutical,
                item.Discontinued,
                item.BrandEnforcedStockCount,
                item.ProductEnforcedStockCount,
                item.Stock,
                item.MaximumAllowedPurchaseQuantity);
        }

        public static int CalculateMaxQuantity(this ProductOverviewModel product, int stock, int? maximumAllowedPurchaseQuantity)
        {
            return CalculateMaxQuantity(
                product.IsPharmaceutical,
                product.Discontinued,
                product.BrandEnforcedStockCount,
                product.ProductEnforcedStockCount,
                stock,
                maximumAllowedPurchaseQuantity);
        }

        public static int CalculateMaxQuantity(
            bool isPharmaceutical, 
            bool discontinued, 
            bool brandEnforcedStockCount, 
            bool productEnforcedStockCount,
            int stock,
            int? maximumAllowedPurchaseQuantity)
        {
            //TODO: It should come from configuration
            var maxQuantity = 10;

            if (isPharmaceutical)
            {
                // 1st priority is pharmaceutical item
                var shoppingCartSettings = EngineContext.Current.Resolve<ShoppingCartSettings>();
                var maxPharmProduct = shoppingCartSettings.MaxPharmaceuticalProduct;

                maxQuantity = maxPharmProduct;
            }
            else if ((discontinued == true)
                    || ((brandEnforcedStockCount || productEnforcedStockCount)))
            {
                // 2nd priority is discontinued item
                maxQuantity = maxQuantity > stock ? stock : 10;
            }

            // 3rd priority is to check maximum allowed quantity per price option
            if ((maximumAllowedPurchaseQuantity.HasValue) && (maxQuantity > maximumAllowedPurchaseQuantity.Value))
            {
                maxQuantity = maximumAllowedPurchaseQuantity.Value;
            }

            return maxQuantity;
        }

        public static CustomerOrderModel PrepareCustomerOrderModel(this Order order)
        {
            var priceFormatter = EngineContext.Current.Resolve<IPriceFormatter>();
            
            var model = new CustomerOrderModel
            {
                Id = order.Id,
                OrderTotal = priceFormatter.FormatValue(order.GrandTotal, order.CurrencyCode),
                CreatedOn = order.OrderPlaced.Value,
                OrderStatusCode = order.StatusCode,
                OrderStatus = order.OrderStatus
            };

            // To avoid customer frustration, ON HOLD will be changed to ORDER PLACED
            if (model.OrderStatusCode == OrderStatusCode.ON_HOLD)
            {
                var orderService = EngineContext.Current.Resolve<IOrderService>();

                model.OrderStatusCode = OrderStatusCode.ORDER_PLACED;
                model.OrderStatus = orderService.GetOrderStatusByCode(OrderStatusCode.ORDER_PLACED);
            }

            var orderItems = order.LineItemCollection.Where(x => ValidLineStatus.VALID_LINE_STATUSES.Contains(x.StatusCode)).ToList();
            foreach (var item in orderItems)
            {
                var itemModel = new LineItemModel
                {
                    Name = item.Name,
                    Option = item.Option,
                    Quantity = item.Quantity,
                    UnitPrice = priceFormatter.FormatValue(item.PriceInclTax * item.ExchangeRate, item.CurrencyCode),
                    Subtotal = priceFormatter.FormatValue(item.PriceInclTax * item.Quantity * item.ExchangeRate, item.CurrencyCode)
                };

                model.Items.Add(itemModel);
            }

            model.Totals.Subtotal = priceFormatter.FormatValue(orderItems.Select(x => (x.PriceInclTax * x.Quantity * x.ExchangeRate)).Sum(), order.CurrencyCode);
            model.Totals.Discount = order.DiscountAmount > 0M ? priceFormatter.FormatValue(order.DiscountAmount, order.CurrencyCode) : null;
            model.Totals.UsedPointsAmount = order.AllocatedPoint > 0 ? priceFormatter.FormatValue(order.AllocatedPoint / 100M * order.ExchangeRate, order.CurrencyCode) : null;
            model.Totals.UsedPoints = order.AllocatedPoint;
            model.Totals.ShippingCost = priceFormatter.FormatValue(order.ShippingCost * order.ExchangeRate, order.CurrencyCode);
            model.Totals.VAT = priceFormatter.FormatValue(orderItems.Select(x => ((x.PriceInclTax - x.PriceExclTax) * x.Quantity * x.ExchangeRate)).Sum(), order.CurrencyCode);
            model.Totals.VATMessage = order.ShippingCountry.IsEC ? "included" : "discount";

            // Some orders may not have billing address
            if (order.Country != null)
            {
                model.Billing = new AddressModel
                {
                    Name = order.BillTo,
                    AddressLine1 = order.AddressLine1,
                    AddressLine2 = order.AddressLine2,
                    City = order.City,
                    CountryName = order.Country.Name,
                    PostCode = order.PostCode
                };
            }

            if (order.ShippingCountry != null)
            {
                model.Shipping = new AddressModel
                {
                    Name = order.ShipTo,
                    AddressLine1 = order.ShippingAddressLine1,
                    AddressLine2 = order.ShippingAddressLine2,
                    City = order.ShippingCity,
                    CountryName = order.ShippingCountry.Name,
                    PostCode = order.ShippingPostCode
                };
            }

            // Check if the order is valid for invoice download
            model.IsAllowedToDownloadInvoice = ValidOrderStatus.VALID_STATUSES.Contains(order.StatusCode);

            return model;
        }

        public static IList<BannerModel> PrepareLargeBannerModels(this IList<LargeBanner> banners)
        {
            var models = new List<BannerModel>();

            if (banners != null)
            {
                models = banners.Select(x => new BannerModel
                {
                    Link = x.Link,
                    Picture = new PictureModel
                    {
                        ImageUrl = "/media/banner/large/" + x.MediaFilename,
                        Title = x.Title,
                        AlternateText = x.MediaAlt
                    }
                }).ToList();
            }

            return models;
        }

        public static IList<BannerModel> PrepareBrandBannerModels(this IList<BrandMedia> banners)
        {
            var models = new List<BannerModel>();

            if (banners != null)
            {
                models = banners.Select(x => new BannerModel
                {
                    Link = x.Link,
                    Picture = new PictureModel
                    {
                        ImageUrl = "/media/brand/" + x.MediaFilename,
                        Title = x.Title,
                        AlternateText = x.Alt
                    }
                }).ToList();
            }

            return models;
        }

        public static IList<ProductPriceModel> PrepareProductPriceModels(
            this IList<ProductPrice> items,
            OptionType type,
            bool discontinued,
            bool productEnforceStockCount,
            bool brandEnforceStockCount,
            bool isPreSelected = false)
        {
            var models = new List<ProductPriceModel>();

            foreach (var item in items)
            {
                models.Add(item.PrepareProductPriceModel(
                    type,
                    discontinued,
                    productEnforceStockCount,
                    brandEnforceStockCount,
                    isPreSelected));
            }

            return models;
        }

        public static ProductPriceModel PrepareProductPriceModel(
            this ProductPrice item,
            OptionType type,
            bool discontinued,
            bool productEnforceStockCount,
            bool brandEnforceStockCount,
            bool isPreSelected = false)
        {
            var visible = true;
            var messageAfterHidden = string.Empty;
            var stockAvailability = true;

            if (item.Stock <= 0)
            {
                if (discontinued == true)
                {
                    visible = false;
                    messageAfterHidden = "Discontinued";
                    stockAvailability = false;
                }
                else if (brandEnforceStockCount || productEnforceStockCount)
                {
                    stockAvailability = false;
                }
                else if (item.PriceExclTax <= 0M)
                {
                    visible = false;
                }
            }

            var displayRRP = false;

            if (item.OfferRuleId > 0)
            {
                var offerService = EngineContext.Current.Resolve<IOfferService>();
                var offerRule = offerService.GetOfferRuleById(item.OfferRuleId);
                if (offerRule != null && offerRule.ShowRRP)
                {
                    displayRRP = true;
                }
            }

            var price = item.PriceExclTax;
            var offerPrice = item.OfferPriceExclTax;
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            if (workContext.CurrentCountry.IsEC)
            {
                price = item.PriceInclTax;
                offerPrice = item.OfferPriceInclTax;
            }

            var savePercentageNote = string.Empty;
            var priceFormatter = EngineContext.Current.Resolve<IPriceFormatter>();

            if (item.OfferRuleId > 0)
                savePercentageNote = string.Format("SAVE {0} ({1:#.##}%)", priceFormatter.FormatPrice(price - offerPrice), (price - offerPrice) / price * 100);

            var utilityService = EngineContext.Current.Resolve<IUtilityService>();
            var currencies = utilityService.GetAllCurrency();
            var metaPrices = new Dictionary<SchemaMetaTagModel, SchemaMetaTagModel>();
            var metaOfferPrices = new Dictionary<SchemaMetaTagModel, SchemaMetaTagModel>();

            if (isPreSelected)
            {
                foreach (var currency in currencies)
                {
                    var metaCurrency = new SchemaMetaTagModel
                    {
                        ItemProp = "priceCurrency",
                        Content = currency.CurrencyCode
                    };

                    metaPrices.Add(metaCurrency, new SchemaMetaTagModel
                    {
                        ItemProp = "price",
                        Content = string.Format("{0:0.00}", price * currency.ExchangeRate)
                    });

                    metaOfferPrices.Add(metaCurrency, new SchemaMetaTagModel
                    {
                        ItemProp = "price",
                        Content = string.Format("{0:0.00}", offerPrice * currency.ExchangeRate)
                    });
                }
            }

            return new ProductPriceModel
            {
                Id = item.Id,
                Option = GetOptionByOptionType(type, item),
                PictureModel = GetOptionImageByOptionType(type, item),
                Price = price > 0M ? priceFormatter.FormatPrice(price) : null,
                OfferPrice = offerPrice > 0M ? priceFormatter.FormatPrice(offerPrice) : null,
                CurrencyCode = workContext.WorkingCurrency.CurrencyCode,
                CurrencySymbol = workContext.WorkingCurrency.Symbol,
                OfferPriceValue = offerPrice > 0M ? priceFormatter.FormatPrice(offerPrice, showCurrency: false) : null,
                PriceValue = price > 0M ? priceFormatter.FormatPrice(price, showCurrency: false) : null,
                OfferRuleId = item.OfferRuleId,
                SavePercentageNote = savePercentageNote,
                Visible = visible,
                MessageAfterHidden = messageAfterHidden,
                StockAvailability = stockAvailability,
                DisplayRRP = displayRRP,
                IsPreSelected = isPreSelected,
                SchemaMetaPrices = metaPrices,
                SchemaMetaOfferPrices = metaOfferPrices,
                ProductMediaId = item.ProductMediaId,
            };
        }

        private static string GetOptionByOptionType(OptionType type, ProductPrice price)
        {
            switch (type)
            {
                case OptionType.Size:
                    return price.Size;
                case OptionType.Colour:
                    if (price.ColourId.HasValue)
                    {
                        var productService = EngineContext.Current.Resolve<IProductService>();
                        var colour = productService.GetColour(price.ColourId.Value);
                        if (colour != null) return colour.Value;
                    }
                    return string.Empty;
                case OptionType.GiftCard:
                case OptionType.None:
                default:
                    return string.Empty;
            }
        }

        private static PictureModel GetOptionImageByOptionType(OptionType type, ProductPrice price)
        {
            switch (type)
            {
                case OptionType.Colour:
                    if (price.ColourId.HasValue)
                    {
                        var productService = EngineContext.Current.Resolve<IProductService>();
                        var colour = productService.GetColour(price.ColourId.Value);
                        if (colour != null)
                            return new PictureModel
                            {
                                ImageUrl = string.Format("/media/colour/{0}", colour.ColourFilename),
                                Title = colour.Value,
                                AlternateText = colour.Value
                            };
                    }
                    return new PictureModel();
                case OptionType.Size:
                case OptionType.GiftCard:
                case OptionType.None:
                default:
                    return new PictureModel();
            }
        }
    }
}