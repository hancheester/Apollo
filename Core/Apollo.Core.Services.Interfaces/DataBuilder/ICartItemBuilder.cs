using Apollo.Core.Model.Entity;

namespace Apollo.Core.Services.Interfaces.DataBuilder
{
    public interface ICartItemBuilder
    {
        CartItem Build(CartItem item);
    }
}
