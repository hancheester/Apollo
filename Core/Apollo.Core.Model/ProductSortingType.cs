namespace Apollo.Core.Model
{
    public enum ProductSortingType
    {
        /// <summary>
        /// Position (display order)
        /// </summary>
        Position = 0,
        /// <summary>
        /// Name: A to Z
        /// </summary>
        NameAsc = 5,
        /// <summary>
        /// Name: Z to A
        /// </summary>
        NameDesc = 6,
        /// <summary>
        /// Price: Low to High
        /// </summary>
        PriceAsc = 10,
        /// <summary>
        /// Price: High to Low
        /// </summary>
        PriceDesc = 11,
        /// <summary>
        /// Product creation date
        /// </summary>
        CreatedOn = 15,
        IdAsc = 16,
        IdDesc = 17,
        /// <summary>
        /// Product review score: High to Low
        /// </summary>
        ReviewScoreDesc = 18,
        StockDesc = 19,
        SoldQuantityDesc = 20
    }
}
