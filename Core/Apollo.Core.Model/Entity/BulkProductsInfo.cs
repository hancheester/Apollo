using System;
using System.Data;

namespace Apollo.Core.Model.Entity
{
    public class BulkProductsInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int BrandId { get; set; }
        public string UrlRewrite { get; set; }
        public int StepQuantity { get; set; }
        public string ProductCode { get; set; }
        public int DeliveryId { get; set; }
        public int RestrictedGroupId { get; set; }
        public int CategoryId { get; set; }
        public int OptionType { get; set; }
        public bool ProductEnabled { get; set; }
        public bool VisibleIndividually { get; set; }
        public bool IsPharmaceutical { get; set; }
        public bool HasFreeWrapping { get; set; }
        public bool OpenForOffer { get; set; }
        public bool Discontinued { get; set; }
        public bool EnforceStockCount { get; set; }
        public bool IsGoogleProductSearchDisabled { get; set; }
        public bool IsPhoneOrder { get; set; }
        public int TaxCategoryId { get; set; }
        public string PriceCode { get; set; }
        public decimal Price { get; set; }
        public string Size { get; set; }
        public int? ColourId { get; set; }
        public string Barcode { get; set; }
        public int Weight { get; set; }
        public bool PriceEnabled { get; set; }
        public int Stock { get; set; }
        public int? MaximumAllowedPurchaseQuantity { get; set; }
        public string MediaType { get; set; }
        public string MediaFileName { get; set; }
        public string ThumbnailFileName { get; set; }
        public string HighResFileName { get; set; }        
        public string TagActiveIngredientsValue { get; set; }
        public string TagApplicationValue { get; set; }
        public string TagDirectionForUseValue { get; set; }
        public string TagPrecautionValue { get; set; }
        public string TagIndicationsValue { get; set; }
        public string TagAllergenInfoValue { get; set; }

        public void ConvertRowData(DataRow row)
        {
            Name = row["name*"].ToString();
            Description = row["description*"].ToString();
            BrandId = Convert.ToInt32(row["brandid*"]);
            UrlRewrite = row["urlrewrite*"].ToString();
            StepQuantity = Convert.ToInt32(row["stepquantity*"]);
            ProductCode = row["productcode"].ToString();
            DeliveryId = Convert.ToInt32(row["deliveryid*"]);
            RestrictedGroupId = Convert.ToInt32(row["restrictedgroupid*"]);
            CategoryId = Convert.ToInt32(row["categoryid*"]);
            OptionType = Convert.ToInt32(row["optiontype*"]);
            ProductEnabled = !(row["enabled*"].ToString()).Equals("0");
            VisibleIndividually = !(row["visibleindividually*"].ToString()).Equals("0");
            IsPharmaceutical = !(row["ispharmaceutical*"].ToString()).Equals("0");
            HasFreeWrapping = !(row["hasfreeWrapping*"].ToString()).Equals("0");
            OpenForOffer = !(row["openforoffer*"].ToString()).Equals("0");
            Discontinued = !(row["discontinued*"].ToString()).Equals("0");
            EnforceStockCount = !(row["enforcestockcount*"].ToString()).Equals("0");
            IsGoogleProductSearchDisabled = !(row["isgoogleproductsearchdisabled*"].ToString()).Equals("0");
            IsPhoneOrder = !(row["isphoneorder*"].ToString()).Equals("0");
            TaxCategoryId = Convert.ToInt32(row["taxcategoryid*"]);
            PriceCode = row["pricecode"].ToString();
            Price = Convert.ToDecimal(row["price*"]);
            Size = row["size"].ToString();
            ColourId = string.IsNullOrEmpty(row["colourid"].ToString()) ? default(int?) : Convert.ToInt32(row["colourid"]);
            Barcode = row["barcode"].ToString();
            Weight = Convert.ToInt32(row["weight*"]);
            PriceEnabled = !(row["priceenabled*"].ToString()).Equals("0");
            Stock = string.IsNullOrEmpty(row["stock"].ToString()) ? 0 : Convert.ToInt32(row["stock"]);
            MaximumAllowedPurchaseQuantity = string.IsNullOrEmpty(row["maximumallowedpurchasequantity"].ToString()) ? default(int?) : Convert.ToInt32(row["maximumallowedpurchasequantity"]);
            MediaType = row["mediatype"].ToString();
            MediaFileName = row["mediafilename"].ToString();
            ThumbnailFileName = row["thumbnailfilename"].ToString();
            HighResFileName = row["highresfilename"].ToString();
            TagActiveIngredientsValue = row["activeingredients"].ToString();
            TagApplicationValue = row["application"].ToString();
            TagDirectionForUseValue = row["directionforuse"].ToString();
            TagPrecautionValue = row["precaution"].ToString();
            TagIndicationsValue = row["indications"].ToString();
            TagAllergenInfoValue = row["allergeninfo"].ToString();
        }

    }
}
