using Apollo.Core.Model.Entity;

namespace Apollo.Core.Services.Interfaces.DataBuilder
{
    public interface IOrderBuilder
    {
        Order Build(Order order, bool useDefaultCurrency = false);
    }
}
