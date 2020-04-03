using Apollo.Core.Model.Entity;

namespace Apollo.Core.Services.Interfaces.DataBuilder
{
    public interface IProductBuilder
    {
        Product Build(int productId);
        Product Build(Product product);
    }
}
