using Apollo.FrontStore.Models.ShoppingCart;
using System.Collections.Generic;

namespace Apollo.FrontStore.Models
{
    public class MiniShoppingCartModel
    {
        public IList<CartItemModel> Items { get; set; }
        public string SubTotal { get; set; }
        public string Message { get; set; }
        public string CurrencyCode { get; set; }

        public MiniShoppingCartModel()
        {
            Items = new List<CartItemModel>();
        }
    }
}