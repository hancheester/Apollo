using Apollo.Core;
using Apollo.Core.Model;
using Apollo.Core.Services.Interfaces;
using Apollo.FrontStore.Extensions;
using Apollo.FrontStore.Models.Order;
using Apollo.Web.Framework.Security;
using Apollo.Web.Framework.Services.Catalog;
using System.Web.Mvc;

namespace Apollo.FrontStore.Controllers
{
#if DEBUG
    [ApolloHttpsRequirement(SslRequirement.No)]
#else
    [ApolloHttpsRequirement(SslRequirement.Yes)]
#endif
    public class OrderController : BasePublicController
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IOrderService _orderService;
        private readonly IAccountService _accountService;

        #endregion

        #region Ctor

        public OrderController(IWorkContext workContext,
                               IPriceFormatter priceFormatter,
                               IOrderService orderService,
                               IAccountService accountService)
        {
            _workContext = workContext;
            _priceFormatter = priceFormatter;
            _orderService = orderService;
            _accountService = accountService;
        }

        #endregion
        
        public ActionResult Orders(CustomerOrderPagingFilteringModel command)
        {
            if (_workContext.CurrentProfile.IsAnonymous)
                return new HttpUnauthorizedResult();

            var model = PrepareCustomerOrderListModel(command);
            return View(model);
        }

        public ActionResult Details(int id)
        {
            if (_workContext.CurrentProfile.IsAnonymous)
                return new HttpUnauthorizedResult();

            var order = _orderService.GetCompleteOrderByIdAndProfileId(id, _workContext.CurrentProfile.Id);

            var model = order.PrepareCustomerOrderModel();

            return View(model);
        }

        public ActionResult DownloadInvoice(int id)
        {
            if (_workContext.CurrentProfile.IsAnonymous)
                return new HttpUnauthorizedResult();

            var bytes = _orderService.PrintOrderToInvoicePdf(id, _workContext.CurrentProfile.Id);
            return File(bytes, "application/pdf", string.Format("order-{0}.pdf", id));
        }

        public ActionResult RewardPoints(CustomerRewardPointsPagingFilteringModel command)
        {
            if (_workContext.CurrentProfile.IsAnonymous)
                return new HttpUnauthorizedResult();

            var accountId = _accountService.GetAccountIdByProfileId(_workContext.CurrentProfile.Id);
            command.SelectedAccountId = accountId;

            var model = PrepareCustomerRewardPointsModel(command);

            return View(model);
        }
        
        #region Utilities

        protected CustomerRewardPointsModel PrepareCustomerRewardPointsModel(CustomerRewardPointsPagingFilteringModel command)
        {
            if (command.SelectedAccountId <= 0)
                return new CustomerRewardPointsModel();

            var model = new CustomerRewardPointsModel();
            var result = _accountService.GetPagedRewardPointHistory(
                accountId: command.SelectedAccountId,
                pageIndex: command.PageNumber - 1,
                pageSize: command.PageSize);

            for (int i = 0; i < result.Items.Count; i++)
            {
                var item = result.Items[i];

                var itemModel = new RewardPointsHistoryModel
                {
                    Id = item.Id,
                    Points = item.Points,
                    UsedPoints = item.UsedPoints,
                    PointsBalance = item.PointsBalance,
                    Message = item.Message,
                    CreatedOn = item.CreatedOnDate
                };
                
                model.RewardPoints.Add(itemModel);
            }

            model.PagingFilteringContext.LoadPagedList(result);
            model.PagingFilteringContext.SelectedAccountId = command.SelectedAccountId;
            
            return model;
        }

        protected CustomerOrderListModel PrepareCustomerOrderListModel(CustomerOrderPagingFilteringModel command)
        {
            int? orderId = null;
            if (!string.IsNullOrEmpty(command.q))
            {
                int temp = 0;
                if (int.TryParse(command.q, out temp))
                {
                    orderId = temp;
                }
            }
            var profileId = _workContext.CurrentProfile.Id;
            var model = new CustomerOrderListModel();
            var result = _orderService.GetPagedOrderOverviewModel(
                pageIndex: command.PageNumber - 1,
                pageSize: command.PageSize,
                profileIds: new int[] { profileId },
                orderIds: orderId.HasValue ? new int[] { orderId.Value } : null,
                statusCodes: ValidOrderStatus.VALID_CUSTOMER_STATUSES,
                orderBy: OrderSortingType.OrderPlacedDesc);

            for(int i = 0; i < result.Items.Count; i++)
            {
                var order = result.Items[i];

                var orderModel = new OrderSummaryModel
                {
                    Id = order.Id,
                    CreatedOn = order.OrderPlaced.Value,
                    OrderStatusCode = order.StatusCode,
                    OrderStatus = order.OrderStatus
                };

                // To avoid customer frustration, ON HOLD will be changed to ORDER PLACED
                if (orderModel.OrderStatusCode == OrderStatusCode.ON_HOLD)
                {
                    orderModel.OrderStatusCode = OrderStatusCode.ORDER_PLACED;
                    orderModel.OrderStatus = _orderService.GetOrderStatusByCode(OrderStatusCode.ORDER_PLACED);
                }

                orderModel.OrderTotal = _priceFormatter.FormatValue(order.GrandTotal, order.CurrencyCode);

                model.Orders.Add(orderModel);
            }

            model.PagingFilteringContext.LoadPagedList(result);
            model.PagingFilteringContext.q = command.q;

            if (model.Orders.Count > 0) model.Orders[0].Chosen = true;

            return model;
        }

        #endregion
    }
}