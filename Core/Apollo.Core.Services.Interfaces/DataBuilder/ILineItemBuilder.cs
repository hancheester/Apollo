using Apollo.Core.Model.Entity;

namespace Apollo.Core.Services.Interfaces.DataBuilder
{
    public interface ILineItemBuilder
    {
        LineItem Build(LineItem item);
    }
}
