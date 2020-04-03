using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using Apollo.Core.Services.Interfaces.DataBuilder;
using Apollo.DataAccess.Interfaces;
using System.Linq;

namespace Apollo.Core.Services.DataBuilder
{
    public class OrderBuilder : IOrderBuilder
    {
        #region Fields

        private readonly IRepository<Country> _countryRepository;
        private readonly IRepository<LineItem> _lineItemRepository;
        private readonly IRepository<ShippingOption> _shippingOptionRepository;
        private readonly IRepository<OrderComment> _orderCommentRepository;
        private readonly IRepository<USState> _usStateRepository;
        private readonly IRepository<OrderStatus> _orderStatusRepository;
        private readonly IRepository<PharmOrder> _pharmOrderRepository;
        private readonly IRepository<PharmItem> _pharmItemRepository;
        private readonly IOrderCalculator _orderCalculator;
        
        #endregion

        #region Constructor

        public OrderBuilder(
            IRepository<ShippingOption> shippingOptionRepository,
            IRepository<LineItem> lineItemRepository,
            IRepository<Country> countryRepository,
            IRepository<USState> usStateRepository,
            IRepository<OrderComment> orderCommentRepository,
            IRepository<OrderStatus> orderStatusRepository,
            IRepository<PharmOrder> pharmOrderRepository,
            IRepository<PharmItem> pharmItemRepository,
            IOrderCalculator orderCalculator)
        {
            _countryRepository = countryRepository;
            _usStateRepository = usStateRepository;
            _lineItemRepository = lineItemRepository;
            _shippingOptionRepository = shippingOptionRepository;
            _orderCommentRepository = orderCommentRepository;
            _orderStatusRepository = orderStatusRepository;
            _pharmOrderRepository = pharmOrderRepository;
            _pharmItemRepository = pharmItemRepository;
            _orderCalculator = orderCalculator;            
        }

        #endregion

        public Order Build(Order order, bool useDefaultCurrency = false)
        {
            order.ShippingOption = _shippingOptionRepository.Return(order.ShippingOptionId);
            order.LineItemCollection = _lineItemRepository.Table.Where(l => l.OrderId == order.Id).ToList();
            order.Country = _countryRepository.Return(order.CountryId);
            order.USState = _usStateRepository.Return(order.USStateId);
            order.ShippingUSState = _usStateRepository.Return(order.ShippingUSStateId);
            order.ShippingCountry = _countryRepository.Return(order.ShippingCountryId);
            order.OrderComments = _orderCommentRepository.Table.Where(c => c.OrderId == order.Id).OrderByDescending(c => c.DateStamp).ToList();
            order.PharmOrder = GetPharmOrderByOrderId(order);
            order.GrandTotal = _orderCalculator.GetOrderTotal(order, useDefaultCurrency);
            order.Tax = _orderCalculator.GetVAT(order, useDefaultCurrency);
            
            var item = _orderStatusRepository.Table.Where(o => o.StatusCode == order.StatusCode).FirstOrDefault();
            if (item != null)
                order.OrderStatus = item.Status;

            return order;
        }

        private PharmOrder GetPharmOrderByOrderId(Order order)
        {
            var pharm = _pharmOrderRepository.TableNoTracking.Where(p => p.OrderId == order.Id).FirstOrDefault();

            if (pharm != null)
            {
                pharm.Items = _pharmItemRepository.TableNoTracking.Where(i => i.PharmOrderId == pharm.Id).ToList();

                if (pharm.Items.Count > 0)
                {
                    var lineItems = order.LineItemCollection;

                    for (int i = 0; i < pharm.Items.Count; i++)
                    {
                        var item = lineItems.Where(x => x.ProductId == pharm.Items[i].ProductId)
                            .Where(x => x.ProductPriceId == pharm.Items[i].ProductPriceId)
                            .Select(x => new { x.Name, x.Option, x.Quantity })
                            .FirstOrDefault();

                        if (item != null)
                        {
                            pharm.Items[i].Name = item.Name;
                            pharm.Items[i].Option = item.Option;
                            pharm.Items[i].Quantity = item.Quantity;
                        }
                    }
                }
            }

            return pharm;
        }
    }
}
