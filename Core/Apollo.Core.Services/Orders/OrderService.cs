using Apollo.Core.Caching;
using Apollo.Core.Domain;
using Apollo.Core.Domain.Media;
using Apollo.Core.Logging;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Model.OverviewModel;
using Apollo.Core.Services.Common;
using Apollo.Core.Services.Directory;
using Apollo.Core.Services.Interfaces;
using Apollo.Core.Services.Interfaces.DataBuilder;
using Apollo.Core.Services.Payment;
using Apollo.Core.Services.Security;
using Apollo.DataAccess;
using Apollo.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Entity.SqlServer;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Apollo.Core.Services.Orders
{
    public class OrderService : BaseRepository, IOrderService
    {
        #region Constants

        private const string WAREHOUSE = "warehouse";
        
        #endregion

        #region Fields

        private readonly IDbContext _dbContext;
        private readonly IRepository<CartItem> _cartItemRepository;
        private readonly IRepository<CartPharmOrder> _cartPharmOrderRepository;
        private readonly IRepository<CartPharmItem> _cartPharmItemRepository;
        private readonly IRepository<Currency> _currencyRepository;
        private readonly IRepository<EmailInvoice> _emailInvoiceRepository;
        private readonly IRepository<Country> _countryRepository;
        private readonly IRepository<USState> _usStateRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<LineItem> _lineItemRepository;
        private readonly IRepository<ShippingOption> _shippingOptionRepository;
        private readonly IRepository<OrderStatus> _orderStatusRepository;
        private readonly IRepository<LineStatus> _lineStatusRepository;
        private readonly IRepository<OrderShipment> _orderShipmentRepository;
        private readonly IRepository<ItemShipment> _itemShipmentRepository;
        private readonly IRepository<OrderPayment> _orderPaymentRepository;
        private readonly IRepository<OrderComment> _orderCommentRepository;
        private readonly IRepository<Branch> _branchRepository;
        private readonly IRepository<BranchStockStatus> _branchStockStatusRepository;
        private readonly IRepository<BranchAllocation> _branchAllocationRepository;
        private readonly IRepository<BranchPendingAllocation> _branchPendingAllocationRepository;
        private readonly IRepository<BranchPendingLine> _branchPendingLineRepository;
        private readonly IRepository<BranchItemStatus> _branchItemStatusRepository;
        private readonly IRepository<BranchProductStock> _branchProductStockRepository;
        private readonly IRepository<DefaultBranch> _defaultBranchRepository;
        private readonly IRepository<Refund> _refundRepository;
        private readonly IRepository<OrderIssue> _orderIssueRepository;
        private readonly IRepository<ProductPrice> _productPriceRepository;
        private readonly IRepository<RestrictedGroupMapping> _restrictedGroupMappingRepository;
        private readonly IRepository<RestrictedGroup> _restrictedGroupRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductGroup> _productGroupRepository;
        private readonly IRepository<ProductGroupMapping> _productGroupMappingRepository;
        private readonly IRepository<Account> _accountRepository;
        private readonly IRepository<PharmOrder> _pharmOrderRepository;
        private readonly IRepository<PharmItem> _pharmItemRepository;
        private readonly IRepository<SagePayDirect> _sagePayDirectRepository;
        private readonly IRepository<SagePayLog> _sagePayLogRepository;
        private readonly IRepository<SystemCheck> _systemCheckRepository;
        private readonly IRepository<SysCheckAddress> _sysCheckAddressRepository;
        private readonly IRepository<SysCheckEmail> _sysCheckEmailRepository;
        private readonly IRepository<SysCheckName> _sysCheckNameRepository;
        private readonly IRepository<SysCheckPostCode> _sysCheckPostCodeRepository;
        private readonly IRepository<WarehouseAllocation> _warehouseAllocationRepository;
        private readonly IRepository<OfferRule> _offerRuleRepository;
        private readonly IRepository<Colour> _colourRepository;
        private readonly IRepository<BranchOrderItem> _branchOrderItemRepository;
        private readonly IRepository<BranchOrder> _branchOrderRepository;
        private readonly IPaymentSystemService _paymentSystemService;
        private readonly IOrderCalculator _orderCalculator;
        private readonly IEmailManager _emailManager;
        private readonly ICryptographyService _cryptographyService;
        private readonly IAccountService _accountService;
        private readonly ICurrencyService _currencyService;
        private readonly IShippingService _shippingService;
        private readonly ICartService _cartService;
        private readonly IPdfService _pdfService;
        private readonly ICsvService _csvService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ISystemCheckService _systemCheckService;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly MediaSettings _mediaSettings;
        private readonly IOrderBuilder _orderBuilder;
        private readonly ILineItemBuilder _lineItemBuilder;
        private readonly IProductBuilder _productBuilder;
        private readonly ICacheManager _cacheManager;
        private readonly ILogger _logger;

        #endregion

        #region Constructor

        public OrderService(
            IDbContext dbContext,
            IRepository<CartItem> cartItemRepository,
            IRepository<CartPharmOrder> cartPharmOrderRepository,
            IRepository<CartPharmItem> cartPharmItemRepository,
            IRepository<Currency> currencyRepository,
            IRepository<EmailInvoice> emailInvoiceRepository,
            IRepository<Country> countryRepository,
            IRepository<USState> usStateRepository,
            IRepository<Order> orderRepository,
            IRepository<LineItem> lineItemRepository,
            IRepository<ShippingOption> shippingOptionRepository,
            IRepository<OrderStatus> orderStatusRepository,
            IRepository<LineStatus> lineStatusRepository,
            IRepository<OrderShipment> orderShipmentRepository,
            IRepository<ItemShipment> itemShipmentRepository,
            IRepository<OrderPayment> orderPaymentRepository,
            IRepository<OrderComment> orderCommentRepository,
            IRepository<Branch> branchRepository,
            IRepository<BranchStockStatus> branchStockStatusRepository,
            IRepository<BranchAllocation> branchAllocationRepository,
            IRepository<BranchPendingAllocation> branchPendingAllocationRepository,
            IRepository<BranchPendingLine> branchPendingLineRepository,
            IRepository<BranchItemStatus> branchItemStatusRepository,
            IRepository<BranchProductStock> branchProductStockRepository,
            IRepository<DefaultBranch> defaultBranchRepository,
            IRepository<Refund> refundRepository,
            IRepository<OrderIssue> orderIssueRepository,
            IRepository<ProductPrice> productPriceRepository,
            IRepository<RestrictedGroupMapping> restrictedGroupMappingRepository,
            IRepository<RestrictedGroup> restrictedGroupRepository,
            IRepository<Product> productRepository,
            IRepository<ProductGroup> productGroupRepository,
            IRepository<ProductGroupMapping> productGroupMappingRepository,            
            IRepository<Account> accountRepository,
            IRepository<PharmOrder> pharmOrderRepository,
            IRepository<PharmItem> pharmItemRepository,
            IRepository<SagePayDirect> sagePayDirectRepository,
            IRepository<SagePayLog> sagePayLogRepository,
            IRepository<SystemCheck> systemCheckRepository,
            IRepository<SysCheckAddress> sysCheckAddressRepository,
            IRepository<SysCheckEmail> sysCheckEmailRepository,
            IRepository<SysCheckName> sysCheckNameRepository,
            IRepository<SysCheckPostCode> sysCheckPostCodeRepository,
            IRepository<WarehouseAllocation> warehouseAllocationRepository,
            IRepository<OfferRule> offerRuleRepository,
            IRepository<Colour> colourRepository,
            IRepository<BranchOrderItem> branchOrderItemRepository,
            IRepository<BranchOrder> branchOrderRepository,
            IPaymentSystemService paymentSystemService,
            IOrderCalculator orderCalculator,
            IEmailManager emailManager,
            ICryptographyService cryptographyService,
            IAccountService accountService,
            ICurrencyService currencyService,
            IShippingService shippingService,
            ICartService cartService,
            ILogBuilder logBuilder,
            IPdfService pdfService,
            ICsvService csvService,
            IGenericAttributeService genericAttributeService,
            ISystemCheckService systemCheckService,
            MediaSettings mediaSettings,
            StoreInformationSettings storeInformationSettings,
            IOrderBuilder orderBuilder,
            ILineItemBuilder lineItemBuilder,
            IProductBuilder productBuilder,
            ICacheManager cacheManager)
        {
            _dbContext = dbContext;
            _cartItemRepository = cartItemRepository;
            _cartPharmOrderRepository = cartPharmOrderRepository;
            _cartPharmItemRepository = cartPharmItemRepository;
            _currencyRepository = currencyRepository;
            _emailInvoiceRepository = emailInvoiceRepository;
            _countryRepository = countryRepository;
            _usStateRepository = usStateRepository;
            _orderRepository = orderRepository;
            _lineItemRepository = lineItemRepository;
            _shippingOptionRepository = shippingOptionRepository;
            _orderStatusRepository = orderStatusRepository;
            _lineStatusRepository = lineStatusRepository;
            _orderShipmentRepository = orderShipmentRepository;
            _itemShipmentRepository = itemShipmentRepository;
            _orderPaymentRepository = orderPaymentRepository;
            _orderCommentRepository = orderCommentRepository;
            _branchRepository = branchRepository;
            _branchStockStatusRepository = branchStockStatusRepository;
            _branchAllocationRepository = branchAllocationRepository;
            _branchPendingAllocationRepository = branchPendingAllocationRepository;
            _branchPendingLineRepository = branchPendingLineRepository;
            _branchItemStatusRepository = branchItemStatusRepository;
            _branchProductStockRepository = branchProductStockRepository;
            _defaultBranchRepository = defaultBranchRepository;
            _refundRepository = refundRepository;
            _orderIssueRepository = orderIssueRepository;
            _restrictedGroupMappingRepository = restrictedGroupMappingRepository;
            _restrictedGroupRepository = restrictedGroupRepository;
            _productPriceRepository = productPriceRepository;
            _productRepository = productRepository;
            _productGroupRepository = productGroupRepository;
            _productGroupMappingRepository = productGroupMappingRepository;
            _accountRepository = accountRepository;
            _pharmOrderRepository = pharmOrderRepository;
            _pharmItemRepository = pharmItemRepository;
            _sagePayDirectRepository = sagePayDirectRepository;
            _sagePayLogRepository = sagePayLogRepository;
            _systemCheckRepository = systemCheckRepository;
            _sysCheckAddressRepository = sysCheckAddressRepository;
            _sysCheckEmailRepository = sysCheckEmailRepository;
            _sysCheckNameRepository = sysCheckNameRepository;
            _sysCheckPostCodeRepository = sysCheckPostCodeRepository;
            _warehouseAllocationRepository = warehouseAllocationRepository;
            _offerRuleRepository = offerRuleRepository;
            _colourRepository = colourRepository;
            _branchOrderItemRepository = branchOrderItemRepository;
            _branchOrderRepository = branchOrderRepository;
            _paymentSystemService = paymentSystemService;
            _orderCalculator = orderCalculator;
            _emailManager = emailManager;
            _cryptographyService = cryptographyService;
            _accountService = accountService;
            _currencyService = currencyService;
            _shippingService = shippingService;
            _cartService = cartService;
            _pdfService = pdfService;
            _csvService = csvService;
            _genericAttributeService = genericAttributeService;
            _systemCheckService = systemCheckService;
            _mediaSettings = mediaSettings;
            _storeInformationSettings = storeInformationSettings;
            _orderBuilder = orderBuilder;
            _lineItemBuilder = lineItemBuilder;
            _productBuilder = productBuilder;
            _cacheManager = cacheManager;
            _logger = logBuilder.CreateLogger(GetType().FullName);
        }

        #endregion

        #region Return

        public decimal GetOrderTotal(int orderId)
        {
            var order = GetCompleteOrderById(orderId);
            if (order != null) return _orderCalculator.GetOrderTotal(order);

            return 0M;
        }

        public decimal GetVAT(int orderId)
        {
            var order = GetCompleteOrderById(orderId);
            if (order != null) return _orderCalculator.GetVAT(order);

            return 0M;
        }

        public SystemCheck GetSystemCheckByOrderId(int orderId)
        {
            return _systemCheckRepository.Table.Where(s => s.OrderId == orderId).FirstOrDefault();
        }

        public SagePayLog GetSagePayLogById(int sagepayLogId)
        {
            return _sagePayLogRepository.Return(sagepayLogId);
        }

        public Refund GetRefundById(int refundId)
        {
            return _refundRepository.Return(refundId);
        }

        public IList<OrderIssue> GetOrderIssueList()
        {
            return _orderIssueRepository.Table.ToList();
        }

        public LineItem GetLineItemById(int lineItemId)
        {
            var item = _lineItemRepository.Return(lineItemId);

            if (item != null)
                _lineItemBuilder.Build(item);

            return item;
        }

        public IList<OrderStatus> GetOrderStatusList()
        {
            return _orderStatusRepository.Table.ToList();
        }

        public int GetLastNumberOfDaysOrdersByProfileId(int profileId, int days)
        {
            DateTime dateTo = DateTime.Now;
            DateTime dateFrom = DateTime.Now.AddDays(-1 * days);

            int count = _orderRepository.Table
                .Where(x => x.ProfileId == profileId)
                .Where(x => x.Paid == true)
                .Where(x => ValidOrderStatus.VALID_STATUSES.Contains(x.StatusCode))
                .Where(x => x.OrderPlaced.Value >= dateFrom)
                .Where(x => x.OrderPlaced.Value <= dateTo)
                .Count();

            return count;
        }

        public IList<LineItem> GetLastNDaysPharmaceauticalLines(int profileId, DateTime dateFrom, DateTime dateTo, string[] lineStatusFilter, string[] orderStatusFilter)
        {
            var items = _lineItemRepository.Table
                .Join(_orderRepository.Table, l => l.OrderId, o => o.Id, (l, o) => new { l, o })
                .Where(x => x.l.IsPharmaceutical == true)
                .Where(x => lineStatusFilter.Contains(x.l.StatusCode))
                .Where(x => orderStatusFilter.Contains(x.o.StatusCode))
                .Where(x => x.o.ProfileId == profileId)
                .Where(x => x.o.OrderPlaced.Value >= dateFrom)
                .Where(x => x.o.OrderPlaced.Value <= dateTo)
                .Select(x => x.l)
                .OrderBy(x => x.Name)
                .ToList();

            return items;
        }

        public IList<LineItem> GetLastNDaysPharmaceauticalLinesFromThisAddress(string addressLine1, string postcode, DateTime dateFrom, DateTime dateTo, string[] lineStatusFilter, string[] orderStatusFilter)
        {
            var items = _lineItemRepository.Table
                .Join(_orderRepository.Table, l => l.OrderId, o => o.Id, (l, o) => new { l, o })
                .Where(x => x.l.IsPharmaceutical == true)
                .Where(x => lineStatusFilter.Contains(x.l.StatusCode))
                .Where(x => orderStatusFilter.Contains(x.o.StatusCode))
                .Where(x => x.o.ShippingAddressLine1.ToLower() == addressLine1.ToLower())
                .Where(x => x.o.ShippingPostCode.ToLower() == postcode.ToLower())
                .Where(x => x.o.OrderPlaced.Value >= dateFrom)
                .Where(x => x.o.OrderPlaced.Value <= dateTo)
                .Select(x => x.l)
                .OrderBy(x => x.Name)
                .ToList();

            return items;
        }

        public int[] GetPrevNextOrderId(int orderId)
        {
            var prev_next = new int[2];

            var prevId = _orderRepository.TableNoTracking
                .Where(x => x.Id < orderId)
                .OrderByDescending(x => x.Id)
                .Select(x => x.Id)
                .FirstOrDefault();

            var nextId = _orderRepository.TableNoTracking
                .Where(x => x.Id > orderId)
                .OrderBy(x => x.Id)
                .Select(x => x.Id)
                .FirstOrDefault();

            prev_next = new int [] { prevId, nextId };

            return prev_next;
        }

        public LineItemOverviewModel GetLineItemOverviewModel(int lineItemId)
        {
            var item = _lineItemRepository.Return(lineItemId);

            if (item != null)
            {
                var model = BuildLineItemOverviewModel(item);
                return model;
            }

            return null;
        }

        public PagedList<LineItemOverviewModel> GetPagedLineItemOverviewModel(
            int pageIndex = 0,
            int pageSize = 2147483647,
            IList<int> orderIds = null,
            IList<int> productIds = null,
            string name = null,
            string barcode = null,            
            string option = null,
            string statusCode = null,
            LineItemSortingType orderBy = LineItemSortingType.OrderIdAsc)
        {
            var list = GetLineItemLoadPaged(
                pageIndex,
                pageSize,
                orderIds,
                productIds,
                name,
                barcode,
                option,
                statusCode,
                orderBy);

            var items = new Collection<LineItemOverviewModel>();

            for (int i = 0; i < list.Items.Count; i++)
            {
                items.Add(BuildLineItemOverviewModel(list.Items[i]));
            }

            return new PagedList<LineItemOverviewModel>(items, pageIndex, pageSize, list.TotalCount);
        }

        public PagedList<LineItem> GetLineItemLoadPaged(
            int pageIndex = 0,
            int pageSize = 2147483647,
            IList<int> orderIds = null,
            IList<int> productIds = null,
            string name = null,
            string barcode = null,            
            string option = null,
            string statusCode = null,
            LineItemSortingType orderBy = LineItemSortingType.OrderIdAsc)
        {
            var pName = GetParameter("Name", name);
            var pBarcode = GetParameter("Barcode", barcode);
            var pOption = GetParameter("Option", option);
            var pStatusCode = GetParameter("StatusCode", statusCode);
            var pOrderBy = GetParameter("OrderBy", (int)orderBy, true);
            var pPageIndex = GetParameter("PageIndex", pageIndex, true);
            var pPageSize = GetParameter("PageSize", pageSize);
            var pTotalRecords = GetParameterIntegerOutput("TotalRecords");

            if (orderIds != null && orderIds.Contains(0))
            {
                orderIds = orderIds.ToList();
                orderIds.Remove(0);
            }
            string commaSeparatedOrderIds = orderIds == null ? "" : string.Join(",", orderIds);
            var pOrderIds = GetParameter("OrderIds", commaSeparatedOrderIds);

            if (productIds != null && productIds.Contains(0))
                productIds.Remove(0);
            string commaSeparatedProductIds = productIds == null ? "" : string.Join(",", productIds);
            var pProductIds = GetParameter("ProductIds", commaSeparatedProductIds);

            var items = _dbContext.ExecuteStoredProcedureList<LineItem>(
                "LineItem_LoadPaged",
                pOrderIds,
                pProductIds,
                pName,
                pBarcode,
                pOption,
                pStatusCode,
                pOrderBy,
                pPageIndex,
                pPageSize,
                pTotalRecords);

            //return items
            int totalRecords = (pTotalRecords.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecords.Value) : 0;

            if (items.Count > 0)
                items = items.Select(x => _lineItemBuilder.Build(x)).ToList();

            return new PagedList<LineItem>(items, pageIndex, pageSize, totalRecords);
        }

        public PagedList<BranchPendingLineOverviewModel> GetPagedBranchPendingLineOverviewModel(
            int pageIndex = 0,
            int pageSize = 2147483647,
            string name = null,
            string option = null,
            string barcode = null,
            string statusCode = null,
            int? branchId = null,
            bool showEmptyStatus = false,
            BranchPendingLineSortingType orderBy = BranchPendingLineSortingType.NameAsc)
        {
            var list = GetBranchPendingLineLoadPaged(
                pageIndex,
                pageSize,
                name,
                option,
                barcode,
                statusCode,
                branchId,
                showEmptyStatus,
                orderBy);

            var items = new Collection<BranchPendingLineOverviewModel>();

            for (int i = 0; i < list.Items.Count; i++)
            {
                items.Add(BuildBranchPendingLineOverviewModel(list.Items[i]));
            }

            return new PagedList<BranchPendingLineOverviewModel>(items, pageIndex, pageSize, list.TotalCount);
        }

        public PagedList<BranchPendingLine> GetBranchPendingLineLoadPaged(
            int pageIndex = 0,
            int pageSize = 2147483647,
            string name = null,
            string option = null,            
            string barcode = null,
            string statusCode = null,
            int? branchId = null,
            bool showEmptyStatus = false,
            BranchPendingLineSortingType orderBy = BranchPendingLineSortingType.NameAsc)
        {
            var query = _branchPendingLineRepository.Table
                .GroupJoin(_productRepository.Table,
                           bpl => bpl.ProductId,
                           p => p.Id,
                           (bpl, p) => new { bpl, p = p.DefaultIfEmpty() })
                .SelectMany(x => x.p,
                            (x, p) => new { x.bpl, p })
                .GroupJoin(_productPriceRepository.Table,
                           bpl_p => bpl_p.bpl.ProductPriceId,
                           pp => pp.Id,
                           (bpl_p, pp) => new { bpl_p.bpl, bpl_p.p, pp = pp.DefaultIfEmpty() })
                .SelectMany(x => x.pp,
                            (x, pp) => new { x.bpl, x.p, pp });

            if (!string.IsNullOrEmpty(name))
                query = query.Where(x => x.p.Name.Contains(name));

            if (!string.IsNullOrEmpty(option))
                query = query.Where(x => x.pp.Option.Contains(option));

            if (!string.IsNullOrEmpty(barcode))
                query = query.Where(x => x.pp.Barcode.Contains(barcode));

            if (branchId != null)
                query = query.Where(x => x.bpl.BranchId == branchId.Value);

            var validLineStatus = new List<string> { LineStatusCode.AWAITING_STOCK,
                                                     LineStatusCode.PARTIAL_SHIPPING };

            var validOrderStatus = new List<string> { OrderStatusCode.ORDER_PLACED,
                                                      OrderStatusCode.STOCK_WARNING,
                                                      OrderStatusCode.PARTIAL_SHIPPING,
                                                      OrderStatusCode.STOCK_WARNING };

            if (!string.IsNullOrEmpty(statusCode))
                query = query.Where(x =>
                    _branchItemStatusRepository.Table
                    .Where(y => y.StatusCode == statusCode)
                    .Select(y => y.Id)
                    .Contains(x.bpl.BranchItemStatusId));

            if (showEmptyStatus)
            {
                query = query.Where(x => x.bpl.BranchItemStatusId == 0);
            }

            query = query.Where(y =>
                _branchPendingAllocationRepository.Table
                    .Join(_branchAllocationRepository.Table, bpa => bpa.BranchAllocationId, ba => ba.Id, (bpa, ba) => new { bpa, ba })
                    .Join(_lineItemRepository.Table, bpa_ba => bpa_ba.ba.LineItemId, l => l.Id, (bpa_ba, l) => new { bpa_ba.bpa, bpa_ba.ba, l })
                    .Join(_orderRepository.Table, bpa_ba_l => bpa_ba_l.l.OrderId, o => o.Id, (bpa_ba_l, o) => new { bpa_ba_l.bpa, bpa_ba_l.ba, bpa_ba_l.l, o })
                    .Where(x => x.bpa.BranchPendingLineId == y.bpl.Id)
                    .Where(x => validLineStatus.Contains(x.l.StatusCode))
                    .Where(x => validOrderStatus.Contains(x.o.StatusCode))
                    .Where(x => (x.l.PendingQuantity - x.l.InTransitQuantity - x.l.AllocatedQuantity) > 0)
                    .Count() > 0
            );
            
            int totalRecords = query.Count();

            switch (orderBy)
            {
                default:
                case BranchPendingLineSortingType.NameAsc:
                    query = query.OrderBy(x => x.p.Name);
                    break;
                case BranchPendingLineSortingType.NameDesc:
                    query = query.OrderByDescending(x => x.p.Name);
                    break;
                case BranchPendingLineSortingType.LastUpdateAsc:
                    query = query.OrderBy(x => x.bpl.LastUpdatedDateTime);
                    break;
                case BranchPendingLineSortingType.LastUpdateDesc:
                    query = query.OrderByDescending(x => x.bpl.LastUpdatedDateTime);
                    break;
            }

            query = query.Skip(pageIndex * pageSize).Take(pageSize);

            var items = query.ToList().Select(x =>
            {
                x.bpl.Product = _productBuilder.Build(x.bpl.ProductId);
                x.bpl.ProductPrice = x.bpl.Product.ProductPrices.Where(pp => pp.Id == x.bpl.ProductPriceId).FirstOrDefault();
                return x.bpl;
            }).ToList();

            return new PagedList<BranchPendingLine>(items, pageIndex, pageSize, totalRecords);
        }

        public PagedList<OrderComment> GetOrderCommentLoadPaged(
            int pageIndex = 0,
            int pageSize = 2147483647,
            IList<int> orderIds = null,            
            string fromDateStamp = null,
            string toDateStamp = null,
            string commentText = null,
            string fullName = null,
            OrderCommentSortingType orderBy = OrderCommentSortingType.IdAsc)
        {
            var query = _orderCommentRepository.Table;

            if (orderIds != null && orderIds.Count > 0)
                query = query.Where(x => orderIds.Contains(x.OrderId));

            DateTime fromDate;
            if (!string.IsNullOrEmpty(fromDateStamp)
                && DateTime.TryParseExact(fromDateStamp, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDate))
            {
                query = query.Where(x => fromDate >= x.DateStamp);
            }

            DateTime toDate;
            if (!string.IsNullOrEmpty(toDateStamp)
                && DateTime.TryParseExact(toDateStamp, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out toDate))
            {
                // To include the whole day itself. dd/MM/yyyy 23:59:59:999 is expected.
                toDate = toDate.AddDays(1).AddMilliseconds(-1);
                query = query.Where(x => toDate <= x.DateStamp);
            }

            if (!string.IsNullOrEmpty(commentText))
                query = query.Where(x => x.CommentText.Contains(commentText));

            if (!string.IsNullOrEmpty(fullName))
                query = query.Where(x => x.FullName.Contains(fullName));

            int totalRecords = query.Count();

            switch (orderBy)
            {
                default:
                case OrderCommentSortingType.IdAsc:
                    query = query.OrderBy(x => x.Id);
                    break;
                case OrderCommentSortingType.IdDesc:
                    query = query.OrderByDescending(x => x.Id);
                    break;
                case OrderCommentSortingType.DateStampAsc:
                    query = query.OrderBy(x => x.DateStamp);
                    break;
                case OrderCommentSortingType.DateStampDesc:
                    query = query.OrderByDescending(x => x.DateStamp);
                    break;
                case OrderCommentSortingType.CommentTextAsc:
                    query = query.OrderBy(x => x.CommentText);
                    break;
                case OrderCommentSortingType.CommentTextDesc:
                    query = query.OrderByDescending(x => x.CommentText);
                    break;
                case OrderCommentSortingType.FullNameAsc:
                    query = query.OrderBy(x => x.FullName);
                    break;
                case OrderCommentSortingType.FullNameDesc:
                    query = query.OrderByDescending(x => x.FullName);
                    break;
            }

            query = query.Skip(pageIndex * pageSize).Take(pageSize);

            var comments = query.ToList();

            return new PagedList<OrderComment>(comments, pageIndex, pageSize, totalRecords);
        }

        public PagedList<Refund> GetRefundLoadPaged(
            int pageIndex = 0,
            int pageSize = 2147483647,
            IList<int> orderIds = null,
            bool isCancellation = false,
            bool? isCompleted = null,
            string fromDateStamp = null,
            string toDateStamp = null,
            RefundSortingType orderBy = RefundSortingType.IdAsc)
        {
            var query = _refundRepository.Table;

            if (orderIds != null && orderIds.Count > 0)
                query = query.Where(x => orderIds.Contains(x.OrderId));

            query = query.Where(x => x.IsCancellation == isCancellation);

            if (isCompleted.HasValue)
                query = query.Where(x => x.IsCompleted == isCompleted);

            DateTime fromDate;
            if (!string.IsNullOrEmpty(fromDateStamp)
                && DateTime.TryParseExact(fromDateStamp, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDate))
            {
                query = query.Where(x => fromDate <= x.DateStamp);
            }

            DateTime toDate;
            if (!string.IsNullOrEmpty(toDateStamp)
                && DateTime.TryParseExact(toDateStamp, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out toDate))
            {
                // To include the whole day itself. dd/MM/yyyy 23:59:59:999 is expected.
                toDate = toDate.AddDays(1).AddMilliseconds(-1);
                query = query.Where(x => toDate >= x.DateStamp);
            }

            int totalRecords = query.Count();

            switch (orderBy)
            {
                default:
                case RefundSortingType.IdAsc:
                    query = query.OrderBy(x => x.Id);
                    break;
                case RefundSortingType.IdDesc:
                    query = query.OrderByDescending(x => x.Id);
                    break;
                case RefundSortingType.OrderIdAsc:
                    query = query.OrderBy(x => x.OrderId);
                    break;
                case RefundSortingType.OrderIdDesc:
                    query = query.OrderByDescending(x => x.OrderId);
                    break;
                case RefundSortingType.DateStampAsc:
                    query = query.OrderBy(x => x.DateStamp);
                    break;
                case RefundSortingType.DateStampDesc:
                    query = query.OrderByDescending(x => x.DateStamp);
                    break;
                case RefundSortingType.IsCompletedAsc:
                    query = query.OrderBy(x => x.IsCompleted);
                    break;
                case RefundSortingType.IsCompletedDesc:
                    query = query.OrderByDescending(x => x.IsCompleted);
                    break;
            }

            query = query.Skip(pageIndex * pageSize).Take(pageSize);

            var refunds = query.ToList();

            return new PagedList<Refund>(refunds, pageIndex, pageSize, totalRecords);
        }

        public PagedList<OrderShipmentOverviewModel> GetPagedOrderShipmentOverviewModel(
            int pageIndex = 0,
            int pageSize = 2147483647,
            IList<int> orderIds = null,
            string shippingName = null,
            string trackingRef = null,
            OrderShipmentSortingType orderBy = OrderShipmentSortingType.OrderIdDesc)
        {
            var query = _orderShipmentRepository.Table;

            if (orderIds != null && orderIds.Count > 0)
                query = query.Where(x => orderIds.Contains(x.OrderId));

            if (!string.IsNullOrEmpty(shippingName))
                query = query.Where(x => x.Carrier.Contains(shippingName));

            if (!string.IsNullOrEmpty(trackingRef))
                query = query.Where(x => x.TrackingRef.Contains(trackingRef));

            int totalRecords = query.Count();

            switch (orderBy)
            {
                case OrderShipmentSortingType.OrderIdAsc:
                    query = query.OrderBy(x => x.OrderId);
                    break;
                default:
                case OrderShipmentSortingType.OrderIdDesc:
                    query = query.OrderByDescending(x => x.OrderId);
                    break;
                case OrderShipmentSortingType.TrackingRefAsc:
                    query = query.OrderBy(x => x.TrackingRef);
                    break;
                case OrderShipmentSortingType.TrackingRefDesc:
                    query = query.OrderByDescending(x => x.TrackingRef);
                    break;
                case OrderShipmentSortingType.ShippingOptionNameAsc:
                    query = query.OrderBy(x => x.Carrier);
                    break;
                case OrderShipmentSortingType.ShippingOptionNameDesc:
                    query = query.OrderByDescending(x => x.Carrier);
                    break;
            }

            query = query.Skip(pageIndex * pageSize).Take(pageSize);

            var shipments = query.ToList()
                .Select(x => BuildOrderShipmentOverviewModel(x))
                .ToList();

            return new PagedList<OrderShipmentOverviewModel>(shipments, pageIndex, pageSize, totalRecords);
        }

        public PagedList<OrderOverviewModel> GetPagedOrderOverviewModel(
            int pageIndex = 0,
            int pageSize = 2147483647,
            IList<int> orderIds = null,
            IList<int> profileIds = null,
            string fromOrderPlacedOn = null,
            string toOrderPlacedOn = null,
            string fromLastActivity = null,
            string toLastActivity = null,
            string emails = null,
            string shippingOptionName = null,
            int shippingCountryId = 0,
            IList<string> statusCodes = null,
            string issueCode = null,
            string address = null,
            bool? readyForPacking = null,
            OrderSortingType orderBy = OrderSortingType.IdAsc)
        {
            var list = GetOrderLoadPaged(
                pageIndex,
                pageSize,
                orderIds,
                profileIds,
                fromOrderPlacedOn,
                toOrderPlacedOn,
                fromLastActivity,
                toLastActivity,
                emails,
                shippingOptionName,
                shippingCountryId,
                statusCodes,
                issueCode,
                address,
                readyForPacking,
                orderBy);

            var items = new Collection<OrderOverviewModel>();

            for(int i = 0; i < list.Items.Count; i++)
            {
                var order = _orderBuilder.Build(list.Items[i]);                
                items.Add(BuildOrderOverviewModel(order));
            }

            return new PagedList<OrderOverviewModel>(items, pageIndex, pageSize, list.TotalCount);
        }

        public PagedList<Order> GetOrderLoadPaged(
            int pageIndex = 0,
            int pageSize = 2147483647,
            IList<int> orderIds = null,
            IList<int> profileIds = null,
            string fromOrderPlacedOn = null,
            string toOrderPlacedOn = null,
            string fromLastActivity = null,
            string toLastActivity = null,
            string emails = null,
            string shippingOptionName = null,
            int shippingCountryId = 0,
            IList<string> statusCodes = null,
            string issueCode = null,
            string address = null,
            bool? readyForPacking = null,
            OrderSortingType orderBy = OrderSortingType.IdAsc)
        {
            var pFromOrderPlacedOn = GetParameter("FromOrderPlacedOn", fromOrderPlacedOn);
            var pToOrderPlacedOn = GetParameter("ToOrderPlacedOn", toOrderPlacedOn);
            var pFromLastActivity = GetParameter("FromLastActivity", fromLastActivity);
            var pToLastActivity = GetParameter("ToLastActivity", toLastActivity);
            var pEmails = GetParameter("Emails", emails);
            var pShippingOptionName = GetParameter("ShippingOptionName", shippingOptionName);
            var pShippingCountryId = GetParameter("ShippingCountryId", shippingCountryId);
            var pIssueCode = GetParameter("IssueCode", issueCode);
            var pAddress = GetParameter("Address", address);
            var pReadyForPacking = GetParameter("ReadyForPacking", readyForPacking);
            var pOrderBy = GetParameter("OrderBy", (int)orderBy, true);
            var pPageIndex = GetParameter("PageIndex", pageIndex, true);
            var pPageSize = GetParameter("PageSize", pageSize);
            var pTotalRecords = GetParameterIntegerOutput("TotalRecords");
            
            if (orderIds != null && orderIds.Contains(0))
                orderIds.Remove(0);
            string commaSeparatedOrderIds = orderIds == null ? string.Empty : string.Join(",", orderIds);
            var pOrderIds = GetParameter("OrderIds", commaSeparatedOrderIds);

            if (profileIds != null && profileIds.Contains(0))
                profileIds.Remove(0);
            string commaSeparatedUserIds = profileIds == null ? string.Empty : string.Join(",", profileIds);
            var pProfileIds = GetParameter("ProfileIds", commaSeparatedUserIds);

            if (statusCodes != null && statusCodes.Contains(string.Empty))
                statusCodes.Remove(string.Empty);
            if (statusCodes != null && statusCodes.Contains(null))
                statusCodes.Remove(null);
            string commaSeparatedStatusCodes = statusCodes == null ? string.Empty : string.Join(",", statusCodes);
            var pStatusCodes = GetParameter("StatusCodes", commaSeparatedStatusCodes);

            var orders = _dbContext.ExecuteStoredProcedureList<Order>(
                    "Order_LoadPaged",
                    pOrderIds,
                    pProfileIds,
                    pFromOrderPlacedOn,
                    pToOrderPlacedOn,
                    pFromLastActivity,
                    pToLastActivity,
                    pEmails,
                    pShippingOptionName,
                    pShippingCountryId,
                    pStatusCodes,
                    pIssueCode,
                    pAddress,
                    pReadyForPacking,
                    pOrderBy,
                    pPageIndex,
                    pPageSize,
                    pTotalRecords);

            //return orders
            int totalRecords = (pTotalRecords.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecords.Value) : 0;
            
            return new PagedList<Order>(orders, pageIndex, pageSize, totalRecords);
        }

        public int GetProfileIdByOrderId(int orderId)
        {
            var profileId = _orderRepository.Table.Where(o => o.Id == orderId).Select(o => o.ProfileId).FirstOrDefault();
            return profileId;
        }

        public int GetPrevNextPackingOrderId(int currentOrderId,
                                            bool isPrev,
                                            string inNextDayShippingIdXml,
                                            string inLocalStandardShippingIdXml,
                                            string inLocalFirstClassIdXml,
                                            string inLocalTrackedIdXml,
                                            bool? nextInternational,
                                            bool? nextExpress,
                                            bool? nextLocalStandard,
                                            bool? nextLocalFirstClass,
                                            bool? nextLocalTracked,
                                            bool? nextTraceable,
                                            int selectedCountryId)
        {
            var OrderId = GetParameter("OrderId", currentOrderId);
            var InLocalNextDayIdXml = GetParameter("InLocalNextDayIdXml", inNextDayShippingIdXml);
            var InLocalStandardIdXml = GetParameter("InLocalStandardIdXml", inLocalStandardShippingIdXml);
            var InLocalFirstClassIdXml = GetParameter("InLocalFirstClassIdXml", inLocalFirstClassIdXml);
            var InLocalTrackedIdXml = GetParameter("InLocalTrackedIdXml", inLocalTrackedIdXml);
            var NextInternational = GetParameter("NextInternational", nextInternational);
            var NextExpress = GetParameter("NextExpress", nextExpress);
            var NextLocalStandard = GetParameter("NextLocalStandard", nextLocalStandard);
            var NextLocalFirstClass = GetParameter("NextLocalFirstClass", nextLocalFirstClass);
            var NextLocalTracked = GetParameter("NextLocalTracked", nextLocalTracked);
            var NextTraceable = GetParameter("NextTraceable", nextTraceable);
            var SelectedCountryId = GetParameter("SelectedCountryId", selectedCountryId);

            int orderId = 0;
            if (isPrev)
                orderId = _dbContext.SqlQuery<int>("EXEC GetPrevPackingOrderId @OrderId,@InLocalNextDayIdXml,@InLocalStandardIdXml,@InLocalFirstClassIdXml,@InLocalTrackedIdXml,@NextInternational,@NextExpress,@NextLocalStandard,@NextLocalFirstClass,@NextLocalTracked,@NextTraceable,@SelectedCountryId",
                          OrderId, InLocalNextDayIdXml, InLocalStandardIdXml, InLocalFirstClassIdXml, InLocalTrackedIdXml, NextInternational, NextExpress, NextLocalStandard, NextLocalFirstClass, NextLocalTracked, NextTraceable, SelectedCountryId).Single();
            else
                orderId = _dbContext.SqlQuery<int>("EXEC GetNextPackingOrderId @OrderId,@InLocalNextDayIdXml,@InLocalStandardIdXml,@InLocalFirstClassIdXml,@InLocalTrackedIdXml,@NextInternational,@NextExpress,@NextLocalStandard,@NextLocalFirstClass,@NextLocalTracked,@NextTraceable,@SelectedCountryId",
                          OrderId, InLocalNextDayIdXml, InLocalStandardIdXml, InLocalFirstClassIdXml, InLocalTrackedIdXml, NextInternational, NextExpress, NextLocalStandard, NextLocalFirstClass, NextLocalTracked, NextTraceable, SelectedCountryId).Single();

            return orderId;
        }

        public BranchAllocation GetBranchAllocationByLineItemId(int lineItemId)
        {
            var item = _branchAllocationRepository.Table.Where(b => b.LineItemId == lineItemId).FirstOrDefault();
            return item;
        }

        public AccountOverviewModel GetAccountOverviewModelByOrderId(int orderId)
        {
            var id = _orderRepository.Table.Where(o => o.Id == orderId).Select(o => o.ProfileId).FirstOrDefault();

            if (id != 0)            
                return _accountService.GetAccountOverviewModelByProfileId(id);
            
            return null;
        }

        public IList<SagePayLog> GetSagePayLogsByOrderId(int orderId)
        {
            var list = _sagePayLogRepository.Table.Where(s => s.OrderId == orderId).OrderByDescending(s => s.Id).ToList();

            return list;
        }

        public PharmOrder GetPharmOrderByOrderId(int orderId)
        {
            var pharm = _pharmOrderRepository.TableNoTracking.Where(p => p.OrderId == orderId).FirstOrDefault();

            if (pharm != null)
            {
                pharm.Items = _pharmItemRepository.TableNoTracking.Where(i => i.PharmOrderId == pharm.Id).ToList();

                if (pharm.Items.Count > 0)
                {
                    var lineItems = GetLineItemOverviewModelListByOrderId(orderId);

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

        public string GetOfferNameByOrderId(int orderId)
        {
            var promocode = _orderRepository.Table
                .Where(o => o.Id == orderId)
                .Select(o => o.PromoCode)
                .FirstOrDefault();

            if (!string.IsNullOrEmpty(promocode))
            {
                var name = _offerRuleRepository.Table
                    .Where(o => o.PromoCode.ToLower() == promocode.ToLower())
                    .Select(o => o.Name)
                    .FirstOrDefault();

                return name;
            }

            return string.Empty;
        }

        public SagePayDirectOverviewModel GetLastSagePayDirectViewModelByOrderId(int orderId)
        {
            var item = _sagePayDirectRepository.Table
                .Where(s => s.OrderId == orderId)
                .Where(s => s.Completed == true)
                .Where(s => s.TxType == SagePayAccountTxType.DEFERRED || s.TxType == SagePayAccountTxType.PAYMENT)
                .OrderByDescending(s => s.Id)
                .Select(s => new SagePayDirectOverviewModel
                {
                    AVSCV2 = s.AVSCV2,
                    AddressResult = s.AddressResult,
                    PostCodeResult = s.PostCodeResult,
                    CV2Result = s.CV2Result,
                    ThreeDSecureStatus = s.ThreeDSecureStatus
                })
                .FirstOrDefault();

            return item;
        }

        public IList<EmailInvoice> GetPaidEmailInvoicesByOrderId(int orderId)
        {
            return _emailInvoiceRepository.Table
                .Where(e => e.OrderId == orderId)
                .Where(e => e.Paid == true)
                .ToList();
        }

        public int GetPaidOrderCountByProfileId(int profileId)
        {
            var count = _orderRepository.TableNoTracking
                .Where(o => o.ProfileId == profileId)
                .Where(o => o.Paid == true)
                .Select(o => o.Id)
                .Count();

            return count;
        }

        public string GetLatestStatusFromLog(int orderId)
        {
            return _paymentSystemService.GetLatestStatusFromLog(orderId);
        }

        public string GetCurrencyCodeByOrderId(int orderId)
        {
            var currencyCode = _orderRepository.Table.Where(o => o.Id == orderId).Select(o => o.CurrencyCode).FirstOrDefault();
            return currencyCode;
        }

        public int GetShipmentRate(string chosenDate, int notLessThanDays, int notMoreThanDays)
        {
            var ChosenDate = GetParameter("ChosenDate", chosenDate);
            var NotLessThanDays = GetParameter("NotLessThanDays", notLessThanDays);
            var NotMoreThanDays = GetParameter("NotMoreThanDays", notMoreThanDays);

            var count = _dbContext.SqlQuery<int>("EXEC ShipmentOrderRate_GetInfo @ChosenDate, @NotLessThanDays, @NotMoreThanDays", 
                                                 ChosenDate, NotLessThanDays, NotMoreThanDays).Single();

            return count;
        }

        public DateTime? GetBranchItemStatusExpectedDeliveryById(int branchItemStatusId)
        {
            var item = _branchItemStatusRepository.Return(branchItemStatusId);

            if (item != null) return item.ExpectedDelivery;
            return null;
        }

        public IList<string> GetDistinctLastUpdatedBy()
        {
            var list = _orderCommentRepository.Table
                .Select(o => o.FullName)
                .Distinct()
                .ToList();

            return list;
        }
        
        public IList<IGrouping<int, LineItem>> GetLastThreeMonthsLineItems()
        {
            var orderIdList = _orderRepository.Table
                .Where(o => o.OrderPlaced >= (SqlFunctions.DateAdd("m", -3, DateTime.Now.Date)))
                .Select(o => o.Id)
                .ToList();

            var lineItems = _lineItemRepository.Table
                .Where(l => orderIdList.Contains(l.OrderId))
                .GroupBy(l => l.ProductId)
                .ToList();

            return lineItems;
        }
        
        public EmailInvoice GetEmailInvoiceById(int id)
        {
            EmailInvoice item = _emailInvoiceRepository.Return(id);

            if (item != null)
            {
                item.Country = _countryRepository.Return(item.CountryId);
                item.USState = _usStateRepository.Return(item.USStateId);
            }

            return item;
        }

        public EmailInvoice GetEmailInvoiceByEncodedKey(string encodedKey)
        {
            EmailInvoice item = _emailInvoiceRepository.TableNoTracking
                .Where(x => x.EncodedKey.ToLower() == encodedKey.ToLower())
                .FirstOrDefault();

            if (item != null)
            {
                item.Country = _countryRepository.Return(item.CountryId);
                item.USState = _usStateRepository.Return(item.USStateId);
            }

            return item;
        }

        public IList<Order> GetRecentOrderListByProfileId(int profileId, int limit)
        {
            var list = _orderRepository.Table
                .Where(o => o.ProfileId == profileId)
                .Take(limit)
                .OrderByDescending(o => o.Id)
                .ToList();

            return list;
        }
        
        public Order GetCompleteOrderById(int id, int? profileId = null)
        {
            Order order = null;

            if (profileId.HasValue)
            {
                order = _orderRepository.Table
                    .Where(x => x.Id == id)
                    .Where(x => x.ProfileId == profileId.Value)
                    .FirstOrDefault();
            }
            else
            {
                order = _orderRepository.Return(id);
            }

            if (order != null)
            {
                order = _orderBuilder.Build(order);
                order.LineItemCollection = order.LineItemCollection.Select(x => _lineItemBuilder.Build(x)).ToList();                
            }

            return order;
        }

        public Order GetCompleteOrderByIdAndProfileId(int id, int profileId)
        {
            Order order = _orderRepository.Table.Where(x => x.Id == id && x.ProfileId == profileId).FirstOrDefault();

            if (order != null)
                order = _orderBuilder.Build(order);

            return order;
        }

        public Order GetLastValidOrderByProfileId(int profileId, bool useDefaultCurrency = false)
        {
            Order order = _orderRepository.Table
                .Where(x => x.ProfileId == profileId)
                .Where(x => x.StatusCode == OrderStatusCode.ORDER_PLACED || x.StatusCode == OrderStatusCode.ON_HOLD)
                .OrderByDescending(x => x.OrderPlaced)
                .FirstOrDefault();

            if (order != null)
                order = _orderBuilder.Build(order, useDefaultCurrency);

            return order;
        }

        public string GetOrderStatusCodeByOrderId(int id)
        {
            var code = _orderRepository.Table.Where(o => o.Id == id).Select(o => o.StatusCode).FirstOrDefault();
            return code;
        }

        public string GetLastNameForLastUpdatedByOrderId(int orderId)
        {
            var name = _orderCommentRepository.Table
                .Where(o => o.OrderId == orderId)
                .OrderByDescending(o => o.DateStamp)
                .Select(o => o.FullName)
                .FirstOrDefault();

            return name;
        }

        public bool GetPaidStatusByOrderId(int id)
        {
            var paid = _orderRepository.Table.Where(o => o.Id == id).Select(o => o.Paid).FirstOrDefault();
            return paid;
        }

        public bool GetPaidStatusByEmailInvoiceId(int id)
        {
            var paid = _emailInvoiceRepository.Table.Where(e => e.Id == id).Select(e => e.Paid).FirstOrDefault();
            return paid;
        }

        public IList<Order> GetShippingOrderByTime(DateTime timeStamp)
        {
            List<Order> list = _orderRepository.Table
                .Join(_orderShipmentRepository.Table, o => o.Id, os => os.OrderId, (o, os) => new { o, os })
                .Where(o_os => o_os.os.TimeStamp < timeStamp)
                .Select(o_os => o_os.o)
                .ToList();

            return list;
        }
        
        /// <summary>
        /// Calculate total paid amount using exchange rate.
        /// </summary>
        public decimal CalculateTotalPaidAmountByOrderId(int orderId)
        {
            var amount = _orderPaymentRepository.Table
                .Where(o => o.OrderId == orderId)
                .Where(o => o.IsCompleted == true)
                .DefaultIfEmpty()
                .Sum(o => (decimal?)(o.Amount * o.ExchangeRate)) ?? 0M;

            return amount;
        }

        /// <summary>
        /// Calculate total refunded amount using exchange rate.
        /// </summary>
        public decimal CalculateTotalRefundedAmountByOrderId(int orderId)
        {
            var total = _refundRepository.Table
                .Where(r => r.OrderId == orderId)
                .Where(r => r.IsCompleted == true)
                .DefaultIfEmpty()
                .Sum(r => (decimal?)(r.ValueToRefund * r.ExchangeRate)) ?? 0M;

            return total;
        }

        public string GetOrderStatusByCode(string code)
        {
            string key = string.Format(CacheKey.ORDER_STATUS_CODE_KEY, code);
            var status = _cacheManager.Get(key, delegate ()
            {
                var item = _orderStatusRepository.Table.Where(o => o.StatusCode == code).FirstOrDefault();
                if (item != null) return item.Status;
                return null;
            });
            return status;
        }

        public string GetLineStatusByCode(string code)
        {
            string key = string.Format(CacheKey.ORDER_LINE_STATUS_CODE_KEY, code);
            var status = _cacheManager.Get(key, delegate ()
            {
                var item = _lineStatusRepository.Table.Where(l => l.StatusCode == code).FirstOrDefault();
                if (item != null) return item.Status;
                return null;
            });
            return status;
        }

        public int GetOrderCountForPacking()
        {
            IList<string> orderStatusList = new List<string> { OrderStatusCode.ORDER_PLACED, OrderStatusCode.PARTIAL_SHIPPING };
            IList<string> invalidLineStatusList = new List<string> {
                LineStatusCode.AWAITING_STOCK,
                LineStatusCode.PICK_IN_PROGRESS,
                LineStatusCode.PENDING,
                LineStatusCode.ORDERED,
                LineStatusCode.STOCK_WARNING};

            var count = _orderRepository.Table
                .Where(o => orderStatusList.Contains(o.StatusCode))
                .Where(o => _lineItemRepository.Table
                            .Where(l => l.StatusCode == LineStatusCode.GOODS_ALLOCATED)
                            .Where(l => !invalidLineStatusList.Contains(l.StatusCode))
                            .Select(l => l.OrderId)
                            .Contains(o.Id))
                .Count();

            return count;
        }

        public DataTable GetOrderCountGroupByDate(string inLineStatusXml, string inOrderStatusXml)
        {
            DataTable dt = new DataTable("GetOrderCountGroupByDate");
            dt.Columns.Add("Date");
            dt.Columns.Add("NumOrder");

            object InLineStatusXml, InOrderStatusXml;

            InLineStatusXml = GetParameter("InLineStatusXml", inLineStatusXml);
            InOrderStatusXml = GetParameter("InOrderStatusXml", inOrderStatusXml);

            var data = _dbContext.SqlQuery<spOrderCount_GroupByDate_Result>("EXEC OrderCount_GroupByDate @InLineStatusXml, @InOrderStatusXml", InLineStatusXml, InOrderStatusXml).ToList();

            if (data != null)
            {
                foreach (var item in data)
                {
                    DataRow dr = dt.NewRow();
                    dr["Date"] = item.Date;
                    dr["NumOrder"] = item.NumOrder;
                    dt.Rows.Add(dr);
                }
            }

            return dt;
        }
        
        public IList<LineItem> GetLineItemListByStatusCodeAndDate(DateTime fromDate, 
                                                                  DateTime toDate,
                                                                  string[] arrayOrderStatus,
                                                                  string[] arrayLineStatus)
        {
            IList<LineItem> list = _lineItemRepository.Table
                .Join(_orderRepository.Table, l => l.OrderId, o => o.Id, (l, o) => new { l, o })                
                .Where(l_o => arrayOrderStatus.Contains(l_o.o.StatusCode))
                .Where(l_o => arrayLineStatus.Contains(l_o.l.StatusCode))               
                .Where(l_o => l_o.o.OrderPlaced >= fromDate)
                .Where(l_o => l_o.o.OrderPlaced <= toDate)
                .Distinct()
                .Select(l_o => l_o.l)
                .ToList();

            return list;
        }
        
        public string GetBranchNameById(int id)
        {
            Branch branch = _branchRepository.Return(id);
            string name = null;

            if (branch != null)            
                name = branch.Name;
            
            return name;
        }

        public IList<Branch> GetAllBranches()
        {
            string key = CacheKey.BRANCH_ALL_KEY;

            return _cacheManager.Get(key, delegate()
            {
                return _branchRepository.Table.OrderBy(x => x.DisplayOrder).ToList();
            });            
        }

        public IList<BranchStockStatus> GetAllBranchStockStatus()
        {
            var list = _branchStockStatusRepository.Table.ToList();
            return list;
        }
        
        public IList<LineStatus> GetLineStatusList()
        {
            List<LineStatus> list = _lineStatusRepository.Table.OrderBy(l => l.Status).ToList();
            return list;
        }
        
        public BranchPendingLine GetBranchPendingLineById(int id)
        {
            BranchPendingLine line = _branchPendingLineRepository.Return(id);

            if (line != null)
            {
                line.BranchItemStatus = _branchItemStatusRepository.Return(line.BranchItemStatusId);
            }

            return line;
        }

        public BranchPendingLine GetBranchPendingLineByBranchIdProductIdPriceIdBranchItemStatusId(int branchId,
                                                                                                  int productId,
                                                                                                  int productPriceId,
                                                                                                  int branchItemStatusId)
        {
            var item = _branchPendingLineRepository.Table
                .Where(b => b.BranchId == branchId)
                .Where(b => b.ProductId == productId)
                .Where(b => b.ProductPriceId == productPriceId)
                .Where(b => b.BranchItemStatusId == branchItemStatusId)
                .FirstOrDefault();

            return item;
        }

        public string GetBranchStockStatusByCode(string code)
        {
            var stockStatus = _branchStockStatusRepository.Table.Where(b => b.StatusCode == code).FirstOrDefault();
            string status = null;

            if (stockStatus != null) status = stockStatus.Status;

            return status;
        }

        public IList<BranchPendingLineDownload> GetPendingLineItemByBranchId(
            int? branchId,
            int day,
            int month,
            int year,
            bool displayPrice = false,
            string name = null,
            string option = null,
            string barcode = null,
            string statusCode = null)
        {
            var query = _branchAllocationRepository.Table
            .GroupJoin(_branchPendingAllocationRepository.Table,
                       ba => ba.Id, bpa => bpa.BranchAllocationId, (ba, bpa) => new { ba, bpa })
            .SelectMany(x => x.bpa.DefaultIfEmpty(), (x, bpa) => new { x.ba, bpa })
            .GroupJoin(_branchPendingLineRepository.Table,
                       x => x.bpa.BranchPendingLineId, bpl => bpl.Id, (x, bpl) => new { x.ba, x.bpa, bpl })
            .SelectMany(x => x.bpl.DefaultIfEmpty(), (x, bpl) => new { x.ba, x.bpa, bpl })
            .GroupJoin(_lineItemRepository.Table,
                       x => x.ba.LineItemId, l => l.Id, (x, l) => new { x.ba, x.bpa, x.bpl, l })
            .SelectMany(x => x.l.DefaultIfEmpty(), (x, l) => new { x.ba, x.bpa, x.bpl, l })
            .GroupJoin(_orderRepository.Table,
                       x => x.l.OrderId, o => o.Id, (x, o) => new { x.ba, x.bpa, x.bpl, x.l, o })
            .SelectMany(x => x.o.DefaultIfEmpty(), (x, o) => new { x.ba, x.bpa, x.bpl, x.l, o })
            .GroupJoin(_productPriceRepository.Table,
                       x => x.l.ProductPriceId, pp => pp.Id, (x, pp) => new { x.ba, x.bpa, x.bpl, x.l, x.o, pp })
             .SelectMany(x => x.pp.DefaultIfEmpty(), (x, pp) => new { x.ba, x.bpa, x.bpl, x.l, x.o, pp });

            var validLineStatus = new List<string> { LineStatusCode.AWAITING_STOCK,
                                                     LineStatusCode.PARTIAL_SHIPPING };

            var validOrderStatus = new List<string> { OrderStatusCode.ORDER_PLACED,
                                                      OrderStatusCode.STOCK_WARNING,
                                                      OrderStatusCode.PARTIAL_SHIPPING,
                                                      OrderStatusCode.STOCK_WARNING };

            query = query.Where(x => validLineStatus.Contains(x.l.StatusCode));
            query = query.Where(x => validOrderStatus.Contains(x.o.StatusCode));
            query = query.Where(x => x.bpl.BranchItemStatusId == 0);

            if (!string.IsNullOrEmpty(name))
                query = query.Where(x => x.l.Name.Contains(name));

            if (!string.IsNullOrEmpty(option))
                query = query.Where(x => x.l.Option.Contains(option));

            if (!string.IsNullOrEmpty(barcode))
                query = query.Where(x => x.pp.Barcode.Contains(barcode));

            if (branchId != null)
                query = query.Where(x => x.bpl.BranchId == branchId.Value);
            
            if (!string.IsNullOrEmpty(statusCode))
                query = query.Where(x =>
                    _branchItemStatusRepository.Table
                    .Where(y => y.StatusCode == statusCode)
                    .Select(y => y.Id)
                    .Contains(x.bpl.BranchItemStatusId));

            if (branchId.HasValue)
                query = query.Where(x => x.ba.BranchId == branchId.Value);

            query = query.Where(x => x.ba.Printed
                || x.ba.PrintedDateTime == null
                || (x.ba.PrintedDateTime.Value.Day == day && x.ba.PrintedDateTime.Value.Month == month && x.ba.PrintedDateTime.Value.Year == year));

            var list = query.GroupBy(x => new
            {
                x.bpl.ProductId,
                x.bpl.ProductPriceId,
                x.l.Name,
                x.bpl.LastUpdatedDateTime
            })
            .Select(x => new BranchPendingLineDownload
            {
                ProductId = x.Key.ProductId,
                ProductPriceId = x.Key.ProductPriceId,
                Name = x.Key.Name,
                Quantity = x.Sum(y => y.l.PendingQuantity - y.l.AllocatedQuantity)
            })
            .OrderBy(x => x.Name)
            .ToList();

            if (list.Count > 0)
            {
                var productPriceIds = list.Select(x => x.ProductPriceId).Distinct();
                var allBranchStocks = GetBranchProductStocks(productPriceIds.ToArray());
                var branchStocks = allBranchStocks.Where(x => x.BranchId == branchId.Value);

                foreach (var item in list)
                {
                    var product = _productBuilder.Build(item.ProductId);
                    if (product != null)
                    {
                        var price = product.ProductPrices.Where(x => x.Id == item.ProductPriceId).FirstOrDefault();
                        if (price != null)
                        {
                            item.Option = price.Option;
                            item.Barcode = price.Barcode;
                            if (displayPrice) item.Price = price.Price;
                        }
                    }

                    var branchStock = branchStocks
                        .Where(x => x.ProductPriceId == item.ProductPriceId)
                        .FirstOrDefault();

                    if (branchStock != null)
                        item.Stock = branchStock.Stock;
                }
            }

            return list;
        }

        public IList<BranchAllocation> GetBranchAllocationListByBranchPendingLineId(int branchPendingLineId)
        {
            var list = _branchAllocationRepository.Table
                .Join(_lineItemRepository.Table, ba => ba.LineItemId, li => li.Id, (ba, li) => new { ba, li })
                .Join(_branchPendingAllocationRepository.Table, ba_li => ba_li.ba.Id, bpa => bpa.BranchAllocationId, (ba_li, bpa) => new { ba_li.ba, ba_li.li, bpa })
                .Join(_branchPendingLineRepository.Table, ba_li_bpa => ba_li_bpa.bpa.BranchPendingLineId, bpl => bpl.Id, (ba_li_bpa, bpl) => new { ba_li_bpa.ba, bpl })
                .Where(ba_bpl => ba_bpl.bpl.Id == branchPendingLineId)
                .Select(ba_bpl => ba_bpl.ba)
                .ToList();

            return list;
        }

        public IList<LineItem> GetLineItemsByProductPrice(
            int productPriceId,
            string[] lineStatusFilter,
            string[] orderStatusFilter)
        {
            var query = _lineItemRepository.TableNoTracking
                .Join(_orderRepository.TableNoTracking, l => l.OrderId, o => o.Id, (l, o) => new { l, o })
                .Where(l_o => l_o.l.ProductPriceId == productPriceId);

            if (lineStatusFilter != null && lineStatusFilter.Length > 0)
                query = query.Where(l_o => lineStatusFilter.Contains(l_o.l.StatusCode));

            if (orderStatusFilter != null && orderStatusFilter.Length > 0)
                query = query.Where(l_o => orderStatusFilter.Contains(l_o.o.StatusCode));

            query = query.OrderByDescending(l_o => l_o.l.Quantity);
            query = query.OrderBy(l_o => _orderRepository.TableNoTracking.Count(o => o.Id == l_o.o.Id));

            var list = query.Select(l_o => l_o.l).ToList();

            return list;
        }

        public string GetDefaultBranchByBrandId(int brandId)
        {
            var defaultBranch = _defaultBranchRepository.Table.Where(d => d.BrandId == brandId).FirstOrDefault();

            string targetBranch = null;
            if (defaultBranch != null) targetBranch = defaultBranch.TargetBranch;

            return targetBranch;
        }

        public OrderOverviewModel GetOrderOverviewModelById(int id)
        {
            var order = GetCompleteOrderById(id);

            if (order != null)            
                return BuildOrderOverviewModel(order);
            
            return null;
        }
       
        public IList<LineItemOverviewModel> GetLineItemOverviewModelListByOrderId(int orderId)
        {
            var items = _lineItemRepository.TableNoTracking.Where(l => l.OrderId == orderId).ToList();

            if (items != null)
            {
                var models = items.Select(i => BuildLineItemOverviewModel(_lineItemBuilder.Build(i))).ToList();
                return models;
            }

            return null;
        }

        public string GetShippingOptionNameByOrderId(int orderId)
        {
            var shippingOptionId = _orderRepository.Table.Where(o => o.Id == orderId).Select(o => o.ShippingOptionId).FirstOrDefault();
            var name = _shippingOptionRepository.Table.Where(s => s.Id == shippingOptionId).Select(s => s.Name).FirstOrDefault();

            return name;
        }

        public ShippingOverviewModel GetShippingOverviewModelByOrderId(int orderId)
        {
            var model = _orderRepository.Table.Where(o => o.Id == orderId)
                .Select(o => new ShippingOverviewModel
                {
                    ShippingOptionId = o.ShippingOptionId,
                    ShippingCountryId = o.ShippingCountryId,
                    ShippingCost = o.ShippingCost,
                    Packing = o.Packing
                })
                .FirstOrDefault();

            return model;
        }

        public PaymentHeaderOverviewModel GetPaymentHeaderOverviewModelByOrderId(int orderId)
        {
            var model = _orderRepository.Table.Where(o => o.Id == orderId)
                .Select(o => new PaymentHeaderOverviewModel
                {
                    PaymentMethod = o.PaymentMethod,
                    StatusCode = o.StatusCode,
                    PaymentRef = o.PaymentRef
                })
                .FirstOrDefault();

            return model;
        }

        public IList<OrderCommentOverviewModel> GetOrderCommentOverviewModelListByOrderId(int orderId, int returnCount)
        {
            var list = _orderCommentRepository.Table.Where(o => o.OrderId == orderId)
                .OrderByDescending(o => o.Id)
                .Select(o => new OrderCommentOverviewModel()
                {
                    FullName = o.FullName,
                    CommentText = o.CommentText,
                    DateStamp = o.DateStamp
                })                
                .Take(returnCount)
                .ToList();

            return list;
        }

        public OrderHeaderOverviewModel GetOrderHeaderOverviewModelByOrderId(int orderId)
        {
            var model = _orderRepository.TableNoTracking.Where(o => o.Id == orderId)
                .Select(o => new OrderHeaderOverviewModel
                {
                    OrderPlaced = o.OrderPlaced,
                    StatusCode = o.StatusCode,
                    IssueCode = o.IssueCode,
                    InvoiceNumber = o.InvoiceNumber
                })
                .FirstOrDefault();

            return model;
        }

        public string GetOrderIssueByCode(string code)
        {
            var orderIssue = _orderIssueRepository.Table.Where(o => o.IssueCode == code).FirstOrDefault();

            string issue = null;
            if (orderIssue != null) issue = orderIssue.Issue;

            return issue;
        }

        public AddressOverviewModel GetBillingAddressViewModelByOrderId(int orderId)
        {
            var model = _orderRepository.Table.Where(o => o.Id == orderId)
                .Select(o => new AddressOverviewModel
                {
                    Name = o.BillTo,
                    AddressLine1 = o.AddressLine1,
                    AddressLine2 = o.AddressLine2,
                    City = o.City,
                    County = o.County,
                    CountryId = o.CountryId,
                    USStateId = o.USStateId,
                    PostCode = o.PostCode
                })
                .FirstOrDefault();

            if (model != null)
            {
                var country = _shippingService.GetCountryById(model.CountryId);
                if (country != null) model.CountryName = country.Name;

                if (model.USStateId > 0)
                {
                    var state = _shippingService.GetUSStateById(model.USStateId);
                    if (state != null) model.USStateName = state.State;
                }
            }

            return model;
        }

        public AddressOverviewModel GetShippingAddressViewModelByOrderId(int orderId)
        {
            var model = _orderRepository.Table.Where(o => o.Id == orderId)
                .Select(o => new AddressOverviewModel
                {
                    Name = o.ShipTo,
                    AddressLine1 = o.ShippingAddressLine1,
                    AddressLine2 = o.ShippingAddressLine2,
                    City = o.ShippingCity,
                    County = o.ShippingCounty,
                    CountryId = o.ShippingCountryId,
                    USStateId = o.ShippingUSStateId,
                    PostCode = o.ShippingPostCode
                })
                .FirstOrDefault();

            if (model != null)
            {
                var country = _shippingService.GetCountryById(model.CountryId);
                if (country != null) model.CountryName = country.Name;

                if (model.USStateId > 0)
                {
                    var state = _shippingService.GetUSStateById(model.USStateId);
                    if (state != null) model.USStateName = state.State;
                }
            }

            return model;
        }

        public int GetNumberOfPaidOrders(string promoCode, int profileId)
        {
            int count = _orderRepository.Table
                .Where(o => o.PromoCode == promoCode && o.ProfileId == profileId && o.Paid == true)
                .Count();

            return count;
        }

        public int GetBranchProductStockByBranchAndProductPriceId(int branchId, int productPriceId)
        {
            var productStock = _branchProductStockRepository.TableNoTracking
                .Where(b => b.BranchId == branchId)
                .Where(b => b.ProductPriceId == productPriceId)                
                .FirstOrDefault();

            if (productStock != null) return productStock.Stock;

            return 0;
        }

        public IDictionary<int, int> GetBranchProductStocksByProductPriceId(int productPriceId)
        {
            var branchStocks = _branchProductStockRepository.TableNoTracking
                .Where(b => b.ProductPriceId == productPriceId)
                .Select(b => new { b.BranchId, b.Stock })
                .ToDictionary(x => x.BranchId, x => x.Stock);
            
            return branchStocks;
        }

        public int GetBranchPendingLineSumQuantityById(int branchPendingLineId)
        {
            var validLineStatus = new List<string> { LineStatusCode.AWAITING_STOCK, LineStatusCode.PARTIAL_SHIPPING };
            var validOrderStatus = new List<string> { OrderStatusCode.ORDER_PLACED, OrderStatusCode.STOCK_WARNING, OrderStatusCode.SHIPPING, OrderStatusCode.PARTIAL_SHIPPING, OrderStatusCode.AWAITING_REPLY };

            var pendingQuantity = _branchPendingAllocationRepository.Table
                .Join(_branchAllocationRepository.Table, bpa => bpa.BranchAllocationId, ba => ba.Id, (bpa, ba) => new { bpa, ba })
                .Join(_lineItemRepository.Table, bpa_ba => bpa_ba.ba.LineItemId, li => li.Id, (bpa_ba, li) => new { bpa_ba.bpa, bpa_ba.ba, li })
                .Join(_orderRepository.Table, (bpa_ba_li) => bpa_ba_li.li.OrderId, o => o.Id, (bpa_ba_li, o) => new { bpa_ba_li.bpa, bpa_ba_li.ba, bpa_ba_li.li, o })
                .Where(bpa_ba_li_o => bpa_ba_li_o.bpa.BranchPendingLineId == branchPendingLineId)
                .Where(bpa_ba_li_o => validLineStatus.Contains(bpa_ba_li_o.li.StatusCode))
                .Where(bpa_ba_li_o => validOrderStatus.Contains(bpa_ba_li_o.o.StatusCode))
                .Select(bpa_ba_li_o => bpa_ba_li_o.li.PendingQuantity - bpa_ba_li_o.li.AllocatedQuantity - bpa_ba_li_o.li.InTransitQuantity)
                .DefaultIfEmpty(0)
                .Sum();

            return pendingQuantity;
        }
       
        public int GetRefundCountByTypeAndStatus(bool isCancellation, bool isCompleted, string fromDate, string toDate)
        {
            var query = _refundRepository.Table.Where(x => x.IsCancellation == isCancellation)
                .Where(x => x.IsCompleted == isCompleted);

            DateTime fromDateStamp;
            if (!string.IsNullOrEmpty(fromDate) 
                && DateTime.TryParseExact(fromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDateStamp))
            {
                query = query.Where(x => fromDateStamp <= x.DateStamp);
            }

            DateTime toDateStamp;
            if (!string.IsNullOrEmpty(toDate)
               && DateTime.TryParseExact(toDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out toDateStamp))
            {
                // To include the whole day itself. dd/MM/yyyy 23:59:59:999 is expected.
                toDateStamp = toDateStamp.AddDays(1).AddMilliseconds(-1);
                query = query.Where(x => toDateStamp >= x.DateStamp);
            }

            return query.Count();
        }
        
        public OrderShipment GetOrderShipmentById(int id)
        {
            var shipment = _orderShipmentRepository.Return(id);

            if (shipment != null)
            {
                var items = _itemShipmentRepository.Table.Where(i => i.OrderShipmentId == shipment.Id).ToList();

                if (items != null)
                {
                    foreach (var item in items)                    
                        item.LineItem = _lineItemRepository.Return(item.LineItemId);
                    
                    shipment.ItemShipmentList = items;
                }
            }

            return shipment;
        }

        public SagePayDirect GetSagePayDirectByVPSTxId(string vpsTxId)
        {
            var item = _sagePayDirectRepository.Table.Where(s => s.VPSTxId == vpsTxId).FirstOrDefault();
            return item;
        }

        public IList<int> GetOrderIdListByProfileIdAndStatusCode(int profileId, string code)
        {
            var list = _orderRepository.Table
                .Where(o => o.ProfileId == profileId)
                .Where(o => o.StatusCode == code)
                .OrderBy(o => o.Id)
                .Select(o => o.Id)
                .ToList();

            return list;
        }

        public int GetOrderCountForSpecialDelivery()
        {
            var ordercode = new string[] { OrderStatusCode.ORDER_PLACED, OrderStatusCode.PARTIAL_SHIPPING };
            var linecode = new string[] { LineStatusCode.ORDERED, LineStatusCode.GOODS_ALLOCATED };
            var count = _orderRepository.Table
                .Join(_lineItemRepository.Table, o => o.Id, l => l.OrderId, (o, l) => new { o, l })
                .Join(_shippingOptionRepository.Table, o_l => o_l.o.ShippingOptionId, s => s.Id, (o_l, s) => new { o_l.o, o_l.l, s })
                .Where(o_l_s => ordercode.Contains(o_l_s.o.StatusCode))
                .Where(o_l_s => linecode.Contains(o_l_s.l.StatusCode))
                .Where(o_l_s => o_l_s.s.Name.Contains("next day"))
                .Select(o_l_s => o_l_s.o)
                .Distinct()
                .Count();

            return count;
        }
        
        public int GetOrderCountByOrderStatus(string statusCode)
        {
            var count = _orderRepository.Table.Where(o => o.StatusCode == statusCode).Count();
            return count;
        }
        
        public int GetOrderCountByOrderStatusAndIssueCode(string statusCode, string issueCode)
        {
            var count = _orderRepository.Table
                .Where(o => o.StatusCode == statusCode)
                .Where(o => o.IssueCode == issueCode)
                .Count();

            return count;
        }

        public IList<BranchAllocation> GetBranchAllocationListByBranchIdAndProductPriceId(int branchId, int productPriceId)
        {
            var list = _branchAllocationRepository.Table
                .Join(_lineItemRepository.Table, ba => ba.LineItemId, li => li.Id, (ba, li) => new { ba, li })
                .Where(ba_li => ba_li.ba.BranchId == branchId && ba_li.li.ProductPriceId == productPriceId)
                .Select(ba_li => ba_li.ba)
                .ToList();

            return list;
        }
        
        public int GetWarehouseAllocationQuantityByBarcode(string barcode)
        {
            var quantity = _warehouseAllocationRepository.TableNoTracking
                .Where(x => x.Barcode == barcode)
                .Select(x => x.Quantity)
                .DefaultIfEmpty(0)
                .Sum();

            return quantity;
        }

        public IList<LineItemOverviewModel> GetLineItemOverviewModels(int[] lineItemIds)
        {
            var items = _lineItemRepository.Table
                .Where(x => lineItemIds.Contains(x.Id))
                .ToList()
                .Select(x => BuildLineItemOverviewModel(x))
                .ToList();

            return items;
        }

        #endregion

        #region Create

        public int InsertSystemCheck(SystemCheck systemCheck)
        {
            return _systemCheckRepository.Create(systemCheck);
        }

        public int InsertLineItem(LineItem lineItem)
        {
            return _lineItemRepository.Create(lineItem);
        }

        public int InsertOrderPayment(OrderPayment orderPayment)
        {
            return _orderPaymentRepository.Create(orderPayment);
        }

        public int InsertOrder(Order order)
        {
            return _orderRepository.Create(order);
        }

        public int InsertWarehouseAllocation(WarehouseAllocation warehouseAllocation)
        {
            return _warehouseAllocationRepository.Create(warehouseAllocation);
        }

        public int InsertLineItemsFromCartItems(IList<CartItem> list, int orderId, string currencyCode, decimal exchangeRate)
        {
            foreach (var item in list)
            {
                LineItem lineItem = new LineItem
                {
                    OrderId = orderId,
                    ProductId = item.ProductId,
                    ProductPriceId = item.ProductPriceId,
                    Name = item.Product.Name,
                    Option = item.ProductPrice.Option,
                    PriceInclTax = item.ProductPrice.OfferPriceInclTax,
                    PriceExclTax = item.ProductPrice.OfferPriceExclTax,
                    Quantity = item.Quantity,
                    Wrapped = false,
                    Note = item.ProductPrice.Note,
                    IsPharmaceutical = item.Product.IsPharmaceutical,
                    Weight = item.ProductPrice.Weight,
                    CostPrice = item.ProductPrice.CostPrice,
                    ExchangeRate = exchangeRate,
                    CurrencyCode = currencyCode
                };

                _lineItemRepository.Create(lineItem);
            }

            return list.Count;
        }

        public int InsertPharmItemList(IList<PharmItem> list)
        {
            foreach (var item in list)            
                _pharmItemRepository.Create(item);

            return 0;
        }

        public int InsertBranchItemStatus(BranchItemStatus branchItemStatus)
        {
            return _branchItemStatusRepository.Create(branchItemStatus);
        }

        #endregion

        #region Update

        public void UpdateOrderShipment(OrderShipment orderShipment)
        {
            _orderShipmentRepository.Update(orderShipment);
        }

        public void UpdateSystemCheck(SystemCheck systemCheck)
        {
            _systemCheckRepository.Update(systemCheck);
        }

        public void UpdateOrder(Order order)
        {
            _orderRepository.Update(order);
        }

        public void UpdateBranchAllocationNewBranch(IList<int> branchItems, int newBranchId)
        {
            var list = _branchAllocationRepository.Table
                        .Where(b => branchItems.Contains(b.LineItemId))
                        .ToList();

            if (list != null)
            {
                foreach (var item in list)
                {
                    item.BranchId = newBranchId;
                    _branchAllocationRepository.Update(item);
                }
            }

            throw new NotImplementedException();
        }

        public void UpdateLineItemStatusCodeByOrderId(int orderId, string code)
        {
            var items = _lineItemRepository.Table.Where(l => l.OrderId == orderId).ToList();

            foreach (var item in items)
            {
                item.StatusCode = code;
                item.UpdatedOnUtc = DateTime.Now;
                _lineItemRepository.Update(item);
            }
        }

        public void UpdateOrderIssueCodeByOrderId(int orderId, string code)
        {
            var order = _orderRepository.Return(orderId);

            if (order != null)
            {
                order.IssueCode = code;
                _orderRepository.Update(order);
            }
        }

        public void UpdateOrderStatusCodeByOrderId(int orderId, string code)
        {
            var order = _orderRepository.Return(orderId);

            if (order != null)
            {
                order.StatusCode = code;
                _orderRepository.Update(order);
            }
        }

        public void UpdateOrderPaidStatusByOrderId(int orderId, string code, DateTime datePaid)
        {
            var order = _orderRepository.Return(orderId);

            if (order != null)
            {
                order.Paid = true;
                order.StatusCode = code;
                order.DatePaid = datePaid;

                _orderRepository.Update(order);
            }
        }

        public void UpdatePaymentRefByEmailInvoiceId(int id, string paymentRef)
        {
            var invoice = _emailInvoiceRepository.Return(id);

            if (invoice != null)
            {
                invoice.PaymentRef = paymentRef;
                _emailInvoiceRepository.Update(invoice);
            }
        }

        public void UpdateBranchPendingLineForBranchItemStatusId(int branchPendingLineId, int branchItemStatusId, DateTime lastUpdated)
        {
            var pendingLine = _branchPendingLineRepository.Return(branchPendingLineId);

            if (pendingLine != null)
            {
                pendingLine.BranchItemStatusId = branchItemStatusId;
                pendingLine.LastUpdatedDateTime = lastUpdated;

                _branchPendingLineRepository.Update(pendingLine);
            }
        }

        public void UpdateBranchAllocationForBranchItemStatusIByBranchPendingLineId(int branchPendingLineId, int branchItemStatusId, DateTime lastUpdated)
        {
            var list = _branchAllocationRepository.Table
                .Join(_branchPendingAllocationRepository.Table, ba => ba.Id, bpa => bpa.BranchAllocationId, (ba, bpa) => new { ba, bpa })
                .Where(ba_bpa => ba_bpa.bpa.BranchPendingLineId == branchPendingLineId)
                .Select(ba_bap => ba_bap.ba)
                .ToList();

            foreach (var item in list)
            {
                item.BranchItemStatusId = branchItemStatusId;
                item.LastUpdatedDateTime = lastUpdated;

                _branchAllocationRepository.Update(item);
            }
        }

        public void UpdateBranchAllocationForBranchId(int branchPendingLineId, int branchId)
        {
            var pBranchPendingId = GetParameter("BranchPendingLineId", branchPendingLineId);
            var pNewBranchId = GetParameter("NewBranchId", branchId);

            _dbContext.ExecuteSqlCommand("EXEC BranchAllocation_UpdateBranchId @BranchPendingLineId, @NewBranchId", 
                pBranchPendingId, pNewBranchId);
        }

        public void UpdateBranchPendingLineForBranchId(int branchPendingLineId, int branchId)
        {
            var item = _branchPendingLineRepository.Return(branchPendingLineId);

            if (item != null)
            {
                item.BranchId = branchId;
                _branchPendingLineRepository.Update(item);
            }
        }

        public void UpdateLineItemInTransitQuantity(int lineItemId, int quantity)
        {
            var item = _lineItemRepository.Return(lineItemId);

            if (item != null)
            {
                item.InTransitQuantity = quantity;
                item.UpdatedOnUtc = DateTime.Now;
                _lineItemRepository.Update(item);
            }
        }

        public void UpdateLineItemPendingQuantity(int lineItemId, int quantity)
        {
            var item = _lineItemRepository.Return(lineItemId);

            if (item != null)
            {
                item.PendingQuantity = quantity;
                item.UpdatedOnUtc = DateTime.Now;
                _lineItemRepository.Update(item);
            }
        }

        public void UpdateLineItemShippedQuantity(int lineItemId, int quantity)
        {
            var item = _lineItemRepository.Return(lineItemId);

            if (item != null)
            {
                item.ShippedQuantity = quantity;
                item.UpdatedOnUtc = DateTime.Now;
                _lineItemRepository.Update(item);
            }
        }

        public void UpdateLineItemAllocatedQuantity(int lineItemId, int quantity)
        {
            var item = _lineItemRepository.Return(lineItemId);

            if (item != null)
            {
                item.AllocatedQuantity = quantity;
                item.UpdatedOnUtc = DateTime.Now;
                _lineItemRepository.Update(item);
            }
        }

        public void UpdateOrderLastActivityDateByOrderId(int orderId, DateTime lastActivityDate)
        {
            var order = _orderRepository.Return(orderId);

            if (order != null)
            {
                order.LastActivityDate = lastActivityDate;
                _orderRepository.Update(order);
            }
        }

        public void UpdateBranchAllocationPrintStatus(int branchId, DateTime printedDate)
        {
            var list = _branchAllocationRepository.Table.Where(ba => ba.BranchId == branchId).ToList();

            foreach (var item in list)
            {
                item.Printed = true;
                item.PrintedDateTime = printedDate;
                _branchAllocationRepository.Update(item);
            }
        }

        public void UpdateBranchAllocationLastUpdatedByBranchAllocationId(int branchAllocationId, DateTime lastUpdated)
        {
            var item = _branchAllocationRepository.Return(branchAllocationId);

            if (item != null)
            {
                item.LastUpdatedDateTime = lastUpdated;
                _branchAllocationRepository.Update(item);
            }
        }
        
        public void UpdateLineItemStatusCodeByLineItemId(int lineItemId, string code)
        {
            var item = _lineItemRepository.Return(lineItemId);

            if (item != null)
            {
                item.StatusCode = code;
                item.UpdatedOnUtc = DateTime.Now;
                _lineItemRepository.Update(item);
            }
        }

        public void UpdateLineItemStatusCodeByLineItemIdList(IList<int> list, string code)
        {
            foreach (var id in list)
            {
                UpdateLineItemStatusCodeByLineItemId(id, code);
            }
        }

        public void UpdateLineItemPriceByLineItemId(int lineItemId, decimal priceInclTax, decimal priceExclTax)
        {
            var item = _lineItemRepository.Return(lineItemId);

            if (item != null)
            {
                item.PriceInclTax = priceInclTax;
                item.PriceExclTax = priceExclTax;
                item.UpdatedOnUtc = DateTime.Now;
                _lineItemRepository.Update(item);
            }
        }

        public void UpdateLineItemFreeWrappedByLineItemId(int lineItemId, bool freeWrapped)
        {
            var item = _lineItemRepository.Return(lineItemId);

            if (item != null)
            {
                item.Wrapped = freeWrapped;
                item.UpdatedOnUtc = DateTime.Now;
                _lineItemRepository.Update(item);
            }
        }

        public void UpdateLineItemOptionByLineItemId(int lineItemId, string option)
        {
            var item = _lineItemRepository.Return(lineItemId);

            if (item != null)
            {
                item.Option = option;
                item.UpdatedOnUtc = DateTime.Now;
                _lineItemRepository.Update(item);
            }
        }

        public void UpdateLineItemNoteByLineItemId(int lineItemId, string note)
        {
            var item = _lineItemRepository.Return(lineItemId);

            if (item != null)
            {
                item.Note = note;
                item.UpdatedOnUtc = DateTime.Now;
                _lineItemRepository.Update(item);
            }
        }
        
        public void UpdateLineItemQuantityByLineItemId(int lineItemId, int quantity)
        {
            var item = _lineItemRepository.Return(lineItemId);

            if (item != null)
            {
                item.Quantity = quantity;
                item.UpdatedOnUtc = DateTime.Now;
                _lineItemRepository.Update(item);
            }
        }

        public void UpdateOrderPackingByOrderId(int orderId, string packing)
        {
            var order = _orderRepository.Return(orderId);

            if (order != null)
            {
                order.Packing = packing;
                _orderRepository.Update(order);
            }
        }

        public void UpdateOrderDiscountByOrderId(int orderId, decimal discount)
        {
            var order = _orderRepository.Return(orderId);

            if (order != null)
            {
                order.DiscountAmount = discount;
                _orderRepository.Update(order);
            }
        }

        public void UpdateOrderShippingCostByOrderId(int orderId, decimal shippingCost)
        {
            var order = _orderRepository.Return(orderId);

            if (order != null)
            {
                order.ShippingCost = shippingCost;
                _orderRepository.Update(order);
            }
        }

        public void UpdateOrderShippingOptionIdByOrderId(int orderId, int shippingOptionId)
        {
            var order = _orderRepository.Return(orderId);

            if (order != null)
            {
                order.ShippingOptionId = shippingOptionId;
                _orderRepository.Update(order);
            }
        }

        public void UpdateOrderLastAlertDateByOrderId(int orderId, DateTime lastAlertDate)
        {
            var order = _orderRepository.Return(orderId);

            if (order != null)
            {
                order.LastAlertDate = lastAlertDate;
                _orderRepository.Update(order);
            }
        }

        public void UpdateSysCheckAddressEnabled(string address, bool enabled)
        {
            var check = _sysCheckAddressRepository.Table.Where(c => c.Address == address).FirstOrDefault();

            if (check != null)
            {
                check.Enabled = enabled;
                _sysCheckAddressRepository.Update(check);
            }
        }

        public void UpdateSysCheckNameEnabled(string name, bool enabled)
        {
            var check = _sysCheckNameRepository.Table.Where(c => c.FullName == name).FirstOrDefault();

            if (check != null)
            {
                check.Enabled = enabled;
                _sysCheckNameRepository.Update(check);
            }
        }

        public void UpdateSysCheckEmailEnabled(string email, bool enabled)
        {
            var check = _sysCheckEmailRepository.Table.Where(c => c.Email == email).FirstOrDefault();

            if (check != null)
            {
                check.Enabled = enabled;
                _sysCheckEmailRepository.Update(check);
            }
        }

        public void UpdateSysCheckPostCodeEnabled(string postCode, bool enabled)
        {
            var check = _sysCheckPostCodeRepository.Table.Where(c => c.PostCode == postCode).FirstOrDefault();

            if (check != null)
            {
                check.Enabled = enabled;
                _sysCheckPostCodeRepository.Update(check);
            }
        }

        public void UpdateOrderPaymentStatusById(int id, bool isCompleted)
        {
            var payment = _orderPaymentRepository.Return(id);

            if (payment != null)
            {
                payment.IsCompleted = isCompleted;
                _orderPaymentRepository.Update(payment);
            }
        }

        public void UpdateInvoiceForOrderIdListAndLineItems(IList<int> orderIdList)
        {
            for (int i = 0; i < orderIdList.Count; i++)
            {
                UpdateInvoiceForOrderAndLineItems(orderIdList[i]);
            }
        }

        public void UpdateInvoiceForOrderAndLineItems(int orderId)
        {
            var items = _lineItemRepository.Table.Where(l => l.OrderId == orderId)
                .Where(l => OrderStatusCode.SHIPPING == _orderRepository.Table.Where(o => o.Id == l.OrderId).Select(o => o.StatusCode).FirstOrDefault())
                .Where(l => l.StatusCode == LineStatusCode.DESPATCHED)
                .ToList();

            if (items != null)
            {
                foreach (var item in items)
                {
                    item.InvoicedQuantity = item.ShippedQuantity;
                    item.UpdatedOnUtc = DateTime.Now;
                    _lineItemRepository.Update(item);
                }
            }
            
            UpdateOrderStatusCodeByOrderId(orderId, OrderStatusCode.INVOICED);
        }

        public void UpdateCancelForOrderIdListAndLineItems(IList<int> orderIdList)
        {
            for(int i = 0; i < orderIdList.Count; i++)
            {
                UpdateCancelForOrderAndLineItems(orderIdList[i]);
            }
        }

        public void UpdateCancelForOrderAndLineItems(int orderId)
        {
            UpdateOrderStatusCodeByOrderId(orderId, OrderStatusCode.CANCELLED);
            UpdateLineItemStatusCodeByOrderId(orderId, LineStatusCode.CANCELLED);
        }

        public void UpdateRefundStatusById(int id, bool isCompleted)
        {
            var refund = _refundRepository.Return(id);

            if (refund != null)
            {
                refund.IsCompleted = isCompleted;
                _refundRepository.Update(refund);
            }
        }

        public void UpdateBranchAllocationNewBranchByLineItemIdList(List<int> lineItemIdList, int branchId)
        {
            var list = _branchAllocationRepository.Table.Where(b => lineItemIdList.Contains(b.LineItemId)).ToList();

            if (list != null)
            {
                foreach (var item in list)
                {
                    item.BranchId = branchId;
                    _branchAllocationRepository.Update(item);
                }
            }
        }

        public void UpdateBranchAllocationItemStatusIdByLineItemIdList(List<int> lineItemIdList, int branchItemStatusId)
        {
            var list = _branchAllocationRepository.Table.Where(b => lineItemIdList.Contains(b.LineItemId)).ToList();

            if (list != null)
            {
                foreach (var item in list)
                {
                    item.BranchItemStatusId = branchItemStatusId;
                    _branchAllocationRepository.Update(item);
                }
            }
        }

        public void UpdateOrderBillingAddressByOrderId(int orderId, Address address)
        {
            var order = _orderRepository.Return(orderId);

            if (order != null)
            {
                order.BillTo = address.Name;
                order.AddressLine1 = address.AddressLine1;
                order.AddressLine2 = address.AddressLine2;
                order.City = address.City;
                order.County = address.County;
                order.CountryId = address.CountryId;
                order.PostCode = address.PostCode;
                order.USStateId = address.USStateId;

                _orderRepository.Update(order);
            }
        }

        public void UpdateOrderShippingAddressByOrderId(int orderId, Address address)
        {
            var order = _orderRepository.Return(orderId);

            if (order != null)
            {
                order.ShipTo = address.Name;
                order.ShippingAddressLine1 = address.AddressLine1;
                order.ShippingAddressLine2 = address.AddressLine2;
                order.ShippingCity = address.City;
                order.ShippingCounty = address.County;
                order.ShippingCountryId = address.CountryId;
                order.ShippingPostCode = address.PostCode;
                order.ShippingUSStateId = address.USStateId;

                _orderRepository.Update(order);
            }
        }

        public void UpdateOrderAwardedPoint(int orderId, int newAwardedPoint)
        {
            var order = _orderRepository.Return(orderId);

            if (order != null)
            {
                order.AwardedPoint = newAwardedPoint;
                _orderRepository.Update(order);
            }
        }

        public void UpdateOrderEarnedPoint(int orderId, int newEarnedPoint)
        {
            var order = _orderRepository.Return(orderId);

            if (order != null)
            {
                order.EarnedPoint = newEarnedPoint;
                _orderRepository.Update(order);
            }
        }

        public void UpdateOrderAllocatedPoint(int orderId, int newAllocatedPoint)
        {
            var order = _orderRepository.Return(orderId);

            if (order != null)
            {
                order.AllocatedPoint = newAllocatedPoint;
                _orderRepository.Update(order);
            }
        }

        public void UpdateOrderPharmOrderAndItems(PharmOrder pharm)
        {
            var existingPharmOrder = GetPharmOrderByOrderId(pharm.OrderId);

            if (existingPharmOrder != null)
            {
                existingPharmOrder.TakenByOwner = pharm.TakenByOwner;
                existingPharmOrder.Allergy = pharm.Allergy;
                existingPharmOrder.OwnerAge = pharm.OwnerAge;
                existingPharmOrder.HasOtherCondMed = pharm.HasOtherCondMed;
                existingPharmOrder.OtherCondMed = pharm.OtherCondMed;

                _pharmOrderRepository.Update(existingPharmOrder);

                for(int i = 0; i < existingPharmOrder.Items.Count; i++)
                {
                    var existingItem = existingPharmOrder.Items[i];
                    var item = pharm.Items.Where(x => x.Id == existingItem.Id).FirstOrDefault();

                    if (item != null)
                    {
                        existingItem.Symptoms = item.Symptoms;
                        existingItem.MedForSymptom = item.MedForSymptom;
                        existingItem.Age = item.Age;
                        existingItem.HasOtherCondMed = item.HasOtherCondMed;
                        existingItem.OtherCondMed = item.OtherCondMed;
                        existingItem.PersistedInDays = item.PersistedInDays;
                        existingItem.ActionTaken = item.ActionTaken;
                        existingItem.HasTaken = item.HasTaken;
                        existingItem.TakenQuantity = item.TakenQuantity;
                        existingItem.LastTimeTaken = item.LastTimeTaken;

                        _pharmItemRepository.Update(existingItem);
                    }
                }
            }
        }
        
        #endregion

        #region Delete

        public void DeleteBranchItemStatusByBranchIdAndProductPriceId(int branchId, int productPriceId)
        {
            var item = _branchItemStatusRepository.Table.Where(i => i.BranchId == branchId && i.ProductPriceId == productPriceId).FirstOrDefault();

            if (item != null)
            {
                _branchItemStatusRepository.Delete(item);
            }
        }

        public void DeleteBranchItemAllocationByLineItemId(int lineItemId)
        {
            var item = _branchAllocationRepository.Table.Where(b => b.LineItemId == lineItemId).FirstOrDefault();
            if (item != null) _branchAllocationRepository.Delete(item);
        }

        public void DeleteBranchItemAllocationByLineItemIds(int[] lineItemIds)
        {
            for (int i = 0; i < lineItemIds.Length; i++)
            {
                DeleteBranchItemAllocationByLineItemId(lineItemIds[i]);
            }
        }

        public void DeleteWarehouseAllocationByLineItemId(int lineItemId)
        {
            var item = _warehouseAllocationRepository.Table.Where(w => w.LineItemId == lineItemId).FirstOrDefault();

            if (item != null)
                _warehouseAllocationRepository.Delete(item);
        }

        public void DeleteLineItemsByOrderId(int orderId)
        {
            var items = _lineItemRepository.Table.Where(l => l.OrderId == orderId).ToList();

            if (items != null)
            {
                foreach (var item in items)
                {
                    _lineItemRepository.Delete(item);
                }
            }
        }

        public void DeletePharmItemsByPharmOrderId(int pharmOrderId)
        {
            var items = _pharmItemRepository.Table.Where(i => i.PharmOrderId == pharmOrderId).ToList();

            if (items != null)
            {
                foreach (var item in items)
                {
                    _pharmItemRepository.Delete(item);
                }
            }
        }

        public void DeletePharmOrderAndItemsByOrderId(int orderId)
        {
            var pharmOrder = _pharmOrderRepository.Table.Where(p => p.OrderId == orderId).FirstOrDefault();
            DeletePharmItemsByPharmOrderId(pharmOrder.Id);
            _pharmOrderRepository.Delete(pharmOrder);
        }

        public void DeleteBranchItemStatus(int branchItemStatusId)
        {
            _branchItemStatusRepository.Delete(branchItemStatusId);
        }

        public void DeleteLineItem(int lineItemId)
        {
            _lineItemRepository.Delete(lineItemId);
        }

        public void DeleteRefund(int refundId)
        {
            _refundRepository.Delete(refundId);
        }

        #endregion

        #region Command

        public byte[] ExportOrdersCsv(IList<int> orderIds)
        {
            var orders = new List<Order>();

            foreach (var orderId in orderIds)
            {
                var order = GetCompleteOrderById(orderId);
                if (order != null) orders.Add(order);
            }

            return _csvService.ExportOrdersCsv(orders);
        }

        public decimal CalculateLastValidOrderTotalByProfileId(int profileId, bool useDefaultCurrency = false)
        {
            var orderId = _orderRepository.Table
                .Where(x => x.ProfileId == profileId)
                .Where(x => x.StatusCode == OrderStatusCode.ORDER_PLACED || x.StatusCode == OrderStatusCode.ON_HOLD)
                .OrderByDescending(x => x.OrderPlaced)
                .Select(x => x.Id)
                .FirstOrDefault();

            if (orderId > 0)
                return _orderCalculator.GetOrderTotal(orderId, useDefaultCurrency);

            return 0M;
        }

        public decimal CalculateOrderTotalByOrderId(int orderId)
        {
            return _orderCalculator.GetOrderTotal(orderId);
        }

        public byte[] PrintBranchPendingLinePdf(IList<BranchPendingLineDownload> list)
        {
            return _pdfService.PrintBranchPendingLinePdf(list);
        }

        public byte[] PrintInventoryPendingLinePdf()
        {
            var result = GetInventoryPendingLinesLoadPaged();            
            return _pdfService.PrintInventoryPendingLinePdf(result.Items);
        }

        public byte[] PrintOrderToInvoicePdf(int orderId, int? profileId = null)
        {
            var order = GetCompleteOrderById(orderId, profileId);
            return _pdfService.PrintOrderToInvoicePdf(order);
        }

        public PharmOrder PreparePharmOrderFromCartPharmOrder(int profileId, int orderId)
        {
            // Create pharm order
            var cartPharmForm = _cartPharmOrderRepository.TableNoTracking.Where(x => x.ProfileId == profileId).FirstOrDefault();
            if (cartPharmForm == null) return null;

            var pharmOrder = new PharmOrder
            {
                OrderId = orderId,
                TakenByOwner = cartPharmForm.TakenByOwner,
                OwnerAge = cartPharmForm.OwnerAge,
                HasOtherCondMed = cartPharmForm.HasOtherCondMed,
                OtherCondMed = cartPharmForm.OtherCondMed,
                Allergy = cartPharmForm.Allergy
            };

            int pharmOrderId = _pharmOrderRepository.Create(pharmOrder);

            cartPharmForm.Items = _cartPharmItemRepository.Table.Where(x => x.CartPharmOrderId == cartPharmForm.Id).ToList();

            foreach (var item in cartPharmForm.Items)
            {
                var pharmItem = new PharmItem
                {
                    PharmOrderId = pharmOrderId,
                    ProductId = item.ProductId,
                    ProductPriceId = item.ProductPriceId,
                    Symptoms = item.Symptoms,
                    MedForSymptom = item.MedForSymptom,
                    Age = item.Age,
                    HasOtherCondMed = item.HasOtherCondMed,
                    OtherCondMed = item.OtherCondMed,
                    PersistedInDays = item.PersistedInDays,
                    HasTaken = item.HasTaken,
                    TakenQuantity = item.TakenQuantity,
                    LastTimeTaken = item.LastTimeTaken,
                    ActionTaken = item.ActionTaken
                };

                _pharmItemRepository.Create(pharmItem);
            }

            return pharmOrder;
        }

        public int ProcessRefundInsertion(Refund refund)
        {
            // create refund
            var id = _refundRepository.Create(refund);
            
            // only for cancellation
            if (refund.IsCancellation)
            {
                var items = _lineItemRepository.Table.Where(x => x.OrderId == refund.OrderId).Select(x => new { x.Id, x.Quantity }).ToList();

                if (items.Count > 0)
                {
                    foreach (var item in items)
                    {
                        ResetLineItemForBranchAllocation(item.Id, item.Quantity);
                    }
                }

                UpdateOrderStatusCodeByOrderId(refund.OrderId, OrderStatusCode.SCHEDULED_FOR_CANCEL);
                UpdateLineItemStatusCodeByLineItemIdList(items.Select(x => x.Id).ToList(), LineStatusCode.SCHEDULED_FOR_CANCEL);
            }

            return id;
        }

        public string ProcessPaymentForFulfillment(int orderId, int profileId, string fullName)
        {
            var totalAmount = _orderCalculator.GetOrderTotal(orderId);
            var totalRefund = CalculateTotalRefundedAmountByOrderId(orderId);
            var totalPaid = CalculateTotalPaidAmountByOrderId(orderId);
            var outstanding = totalAmount - totalRefund - totalPaid;

            if (outstanding > 0M)
            {
                var currency = _orderRepository.Table.Where(o => o.Id == orderId)
                                .Select(o => new
                                {
                                    CurrencyCode = o.CurrencyCode,
                                    ExchangeRate = o.ExchangeRate
                                })
                                .FirstOrDefault();

                var payment = new OrderPayment
                {
                    OrderId = orderId,
                    Amount = outstanding / currency.ExchangeRate,
                    CurrencyCode = currency.CurrencyCode,
                    ExchangeRate = currency.ExchangeRate,
                    IsCompleted = false,
                    TimeStamp = DateTime.Now
                };

                var output = _paymentSystemService.ProcessOrderCharging(payment);

                if (output.Status)
                {
                    payment.IsCompleted = true;
                    
                    ProcessOrderCommentInsertion(orderId,
                                                 profileId,
                                                 fullName,
                                                 "Payment",
                                                 "Payment was authorised successfully. <br/>Release amount: " + currency.CurrencyCode.ToUpper() + " " + RoundPrice(outstanding),
                                                 string.Empty);
                }
                else
                {
                    ProcessOrderCommentInsertion(orderId,
                                                 profileId,
                                                 fullName,
                                                 "Payment",
                                                 "Payment was FAILED to authorise. " + output.Message,
                                                 string.Empty);

                    UpdateOrderStatusCodeByOrderId(orderId, OrderStatusCode.ON_HOLD);
                    UpdateOrderIssueCodeByOrderId(orderId, OrderIssueCode.FAILED_TO_CHARGE);
                }

                _orderPaymentRepository.Create(payment);

                return output.Message;
            }

            return "There is no outstanding balance for payment.";
        }

        public void ResetLineItemForBranchAllocation(int lineItemId, int pendingQuantity)
        {
            // delete branch item allocation
            DeleteBranchItemAllocationByLineItemId(lineItemId);

            // reset allocated quantity
            UpdateLineItemAllocatedQuantity(lineItemId, 0);

            // reset pending quantity
            UpdateLineItemPendingQuantity(lineItemId, pendingQuantity);

            // remove item (if any) from warehouse allocation
            DeleteWarehouseAllocationByLineItemId(lineItemId);
        }

        public void ProcessOrderInvoice(int orderId)
        {
            var items = _lineItemRepository.Table.Where(l => l.OrderId == orderId).ToList();

            if (items != null)
            {
                foreach (var item in items)
                {
                    item.InvoicedQuantity = item.ShippedQuantity;
                    _lineItemRepository.Update(item);
                }
            }

            var order = _orderRepository.Return(orderId);

            if (order != null)
            {
                order.StatusCode = OrderStatusCode.INVOICED;
                order.InvoiceNumber = orderId;
                _orderRepository.Update(order);
            }
        }

        public void ProcessBranchItemInsertion(IList<int> branchItems, int branchId, int profileId, string fullName)
        {
            var list = new List<BranchAllocation>();
            var statusAwaitingStock = GetLineStatusByCode(LineStatusCode.AWAITING_STOCK);

            for (int i = 0; i < branchItems.Count; i++)
            {
                int lineItemId = branchItems[i];
                var lineItem = _lineItemRepository.Return(lineItemId);
                var foundItem = GetBranchAllocationByLineItemId(lineItemId);

                if (foundItem == null)
                {
                    var branchAllocation = new BranchAllocation
                    {
                        BranchId = branchId,                        
                        LineItemId = lineItemId,
                        Enabled = true,
                        LastUpdatedDateTime = DateTime.Now,
                        CreatedDateTime = DateTime.Now
                    };
                
                    _branchAllocationRepository.Create(branchAllocation);

                    var branchPending = new BranchPendingLine
                    {
                        BranchId = branchId,
                        ProductId = lineItem.ProductId,
                        ProductPriceId = lineItem.ProductPriceId,
                        LastUpdatedDateTime = DateTime.Now
                    };
                
                    _branchPendingLineRepository.Create(branchPending);
                    var branchPendingAllocation = new BranchPendingAllocation
                    {
                        BranchPendingLineId = branchPending.Id,
                        BranchAllocationId = branchAllocation.Id
                    };

                    _branchPendingAllocationRepository.Create(branchPendingAllocation);
                }

                var lineStatusCode = lineItem.StatusCode;
                UpdateLineItemStatusCodeByLineItemId(lineItemId, LineStatusCode.AWAITING_STOCK);

                // save to comments
                StringBuilder sb = new StringBuilder("Line item was updated successfully.<br/><br/>");
                sb.AppendFormat("<u>{0} {1}</u>", lineItem.Name, lineItem.Option);
                sb.AppendFormat("<br/>Line Status <i>{0}</i> -> <i>{1}</i><br/>", GetLineStatusByCode(lineStatusCode), statusAwaitingStock);
                
                ProcessOrderCommentInsertion(lineItem.OrderId,
                                             profileId,
                                             fullName,
                                             "Line Item (Pick In Progress)",
                                             sb.ToString(),
                                             string.Empty);
            }
        }

        public int ProcessCompleteOrderInsertion(Order order)
        {
            _orderRepository.Create(order);

            foreach (var item in order.LineItemCollection)
            {
                item.OrderId = order.Id;
                _lineItemRepository.Create(item);
            }

            if (order.PharmOrder != null)
            {
                order.PharmOrder.OrderId = order.Id;
                _pharmOrderRepository.Create(order.PharmOrder);

                foreach (var item in order.PharmOrder.Items)
                {
                    item.PharmOrderId = order.PharmOrder.Id;
                    _pharmItemRepository.Create(item);
                }
            }

            _systemCheckRepository.Create(order.SystemCheck);

            return order.Id;
        }

        public string CheckThirdManResultByOrderId(int orderId)
        {
            string result = _paymentSystemService.GetThirdManResult(orderId);
            return result;
        }

        public string CheckTransactionDetailByOrderId(int orderId)
        {
            return _paymentSystemService.GetPaymentDetails(orderId);
        }

        public string CheckIPLocationByOrderId(int orderId)
        {
            string result = _paymentSystemService.GetIPLocation(orderId);
            return result;
        }

        public void ProcessBranchTransfer(int branchPendingLineId, int branchItemStatusId, int newBranchId)
        {
            // Remove branch status
            var status = _branchItemStatusRepository.Return(branchItemStatusId);
            if (status != null) _branchItemStatusRepository.Delete(status);

            // Update branch item status id for branch pending line
            UpdateBranchPendingLineForBranchItemStatusId(branchPendingLineId, 0, DateTime.Now);

            // Update branch id for branch allocation, it resets printed flag and printeddatetime too.
            UpdateBranchAllocationForBranchId(branchPendingLineId, newBranchId);

            // Update branch id for branch pending line
            UpdateBranchPendingLineForBranchId(branchPendingLineId, newBranchId);
        }

        public void ProcessBranchTransferForItems(IList<int> branchItems, int newBranchId, int oldBranchId, int productPriceId, int oldBranchItemStatusId)
        {
            // Update branch allocation
            UpdateBranchAllocationNewBranch(branchItems, newBranchId);

            // Update branch item status id
            //dalInventory.UpdateBranchAllocationNewBranchItemStatus(branchItems, 0);

            // Delete old brand item status            
            _branchItemStatusRepository.Delete(oldBranchItemStatusId);
        }

        public bool ProcessSystemChecking(int orderId)
        {
            var order = _orderRepository.Return(orderId);
            var account = _accountRepository.Return(order.ProfileId);

            if (account != null && order != null)
            {
                bool billingAddressCheck = _sysCheckAddressRepository.Table
                    .Where(s => s.Address.Contains(order.AddressLine1) || s.Address.Contains(order.AddressLine2))
                    .Where(s => s.Enabled == true)
                    .Any();

                bool shippingAddressCheck = _sysCheckAddressRepository.Table
                    .Where(s => s.Address.Contains(order.ShippingAddressLine1) || s.Address.Contains(order.ShippingAddressLine2))
                    .Where(s => s.Enabled == true)
                    .Any();

                bool emailCheck = _sysCheckEmailRepository.Table
                    .Where(s => s.Email.Contains(account.Email))
                    .Where(s => s.Enabled == true)
                    .Any();

                bool billingPostCodeCheck = _sysCheckPostCodeRepository.Table
                    .Where(s => s.PostCode.Contains(order.PostCode))
                    .Where(s => s.Enabled == true)
                    .Any();

                bool shippingPostCodeCheck = _sysCheckPostCodeRepository.Table
                    .Where(s => s.PostCode.Contains(order.ShippingPostCode))
                    .Where(s => s.Enabled == true)
                    .Any();

                bool billingNameCheck = _sysCheckNameRepository.Table
                    .Where(s => s.FullName.Contains(order.BillTo))
                    .Where(s => s.Enabled == true)
                    .Any();

                bool shippingNameCheck = _sysCheckNameRepository.Table
                    .Where(s => s.FullName.Contains(order.ShipTo))
                    .Where(s => s.Enabled == true)
                    .Any();

                bool nameCheck = _sysCheckNameRepository.Table
                    .Where(s => s.FullName.Contains(account.Name))
                    .Where(s => s.Enabled == true)
                    .Any();

                if (!string.IsNullOrWhiteSpace(order.AddressLine2) && !string.IsNullOrWhiteSpace(order.ShippingAddressLine2)
                    && (order.AddressLine2 != order.ShippingAddressLine2))
                {
                    billingAddressCheck = false;
                    shippingAddressCheck = false;
                }

                if ((order.AddressLine1 != order.ShippingAddressLine1)
                    || (order.PostCode != order.ShippingPostCode)
                    || (order.City != order.ShippingCity))
                {
                    billingAddressCheck = false;
                    shippingAddressCheck = false;
                }

                if (!AllItemsHavingSameCurrencyCode(order.Id) || GetFirstCurrencyCodeFromLineItems(order.Id) != order.CurrencyCode)
                {
                    billingAddressCheck = false;
                    shippingAddressCheck = false;
                    emailCheck = false;
                    billingPostCodeCheck = false;
                    shippingPostCodeCheck = false;
                    nameCheck = false;
                    billingNameCheck = false;
                    shippingNameCheck = false;

                    ProcessOrderCommentInsertion(order.Id,
                                                 0,
                                                 "Apollo WebStore - System Check",
                                                 "System Check",
                                                 "Incorrect currency",
                                                 string.Empty);
                }

                // Update system check on this order
                var sysCheck = _systemCheckRepository.TableNoTracking.Where(s => s.OrderId == order.Id).FirstOrDefault();

                if (sysCheck != null)
                {
                    sysCheck.BillingAddressCheck = billingAddressCheck;
                    sysCheck.ShippingAddressCheck = shippingAddressCheck;
                    sysCheck.EmailCheck = emailCheck;
                    sysCheck.BillingPostCodeCheck = billingPostCodeCheck;
                    sysCheck.ShippingPostCodeCheck = shippingPostCodeCheck;
                    sysCheck.NameCheck = billingNameCheck;
                    sysCheck.BillingNameCheck = billingNameCheck;
                    sysCheck.ShippingNameCheck = shippingNameCheck;

                    _systemCheckRepository.Update(sysCheck);

                    return sysCheck.AvsCheck && billingAddressCheck && shippingAddressCheck && emailCheck &&
                           billingPostCodeCheck && shippingPostCodeCheck && nameCheck &&
                           billingNameCheck && shippingNameCheck;
                }
            }

            return false;
        }

        public bool ProcessSystemCheckVerification(int orderId)
        {
            var check = _systemCheckRepository.Table.Where(s => s.OrderId == orderId).FirstOrDefault();

            if (check != null)
            {
                bool systemCheck = check.AvsCheck &&
                                   check.BillingAddressCheck && check.BillingNameCheck && check.BillingPostCodeCheck &&
                                   check.EmailCheck && check.NameCheck &&
                                   check.ShippingAddressCheck && check.ShippingNameCheck && check.ShippingPostCodeCheck;

                if (systemCheck == false)
                {
                    UpdateOrderStatusCodeByOrderId(orderId, OrderStatusCode.ON_HOLD);
                    UpdateOrderIssueCodeByOrderId(orderId, OrderIssueCode.SYSTEM_CHECK_FAILED);                    
                }
            }

            return false;
        }

        public PagedList<InventoryPendingLine> GetInventoryPendingLinesLoadPaged(
            int pageIndex = 0,
            int pageSize = 2147483647,
            IList<int> productIds = null,
            string name = null)
        {
            var statusCodes = new string[] { LineStatusCode.AWAITING_STOCK, LineStatusCode.STOCK_WARNING };
            var query = _lineItemRepository.Table
                .GroupJoin(_productPriceRepository.Table, l => l.ProductPriceId, pp => pp.Id, (l, pp) => new { l, pp = pp.DefaultIfEmpty() })
                .SelectMany(l_pp => l_pp.pp.Select(x => new { l = l_pp.l, pp = x }))
                .GroupJoin(_colourRepository.Table, x => x.pp.ColourId, c => c.Id, (x, c) => new { l_p = x, c = c.DefaultIfEmpty() })
                .SelectMany(l_pp_c => l_pp_c.c.Select(x => new { l = l_pp_c.l_p.l, pp = l_pp_c.l_p.pp, c = x }))
                .Select(x => new { x.l, x.pp, x.c })
                .Where(x => statusCodes.Contains(x.l.StatusCode))
                .GroupBy(x => new { x.l.ProductId, x.l.Name, x.pp.Size, x.c.Value, x.pp.Stock })
                .Select(x => new { Group = x.Key, OrderPlaced = x.Min(y => y.l.CreatedOnUtc), OrderCount = x.Select(z => z.l.OrderId).Distinct().Count(), RelatedOrderIds = x.Select(z => z.l.OrderId).Distinct(), Quantity = x.Sum(w => w.l.PendingQuantity) })
                .OrderBy(x => x.OrderPlaced)
                .ToList()
                .Select(x => new InventoryPendingLine
                {
                    ProductId = x.Group.ProductId,
                    Name = x.Group.Name,
                    Option = string.IsNullOrEmpty(x.Group.Value) ? x.Group.Size : x.Group.Value,
                    Quantity = x.Quantity,
                    Stock = x.Group.Stock,
                    OrderPlaced = x.OrderPlaced,
                    OrderCount = x.OrderCount,
                    RelatedOrderIds = x.RelatedOrderIds.ToList()
                });

            if (productIds != null && productIds.Count > 0)
                query = query.Where(x => productIds.Contains(x.ProductId));

            if (!string.IsNullOrEmpty(name))
                query = query.Where(x => x.Name.Contains(name));

            int totalRecords = query.Count();

            query = query.Skip(pageIndex * pageSize).Take(pageSize);

            var list = query.ToList();

            return new PagedList<InventoryPendingLine>(list, pageIndex, pageSize, totalRecords);
        }

        public void ResetOrderStatusAccordingToLineStatusByOrderIds(IList<int> orderIds, int profileId, string fullName)
        {
            for (int i = 0; i < orderIds.Count; i++)
                ResetOrderStatusAccordingToLineStatus(orderIds[i], profileId, fullName);            
        }

        //TODO: Find a quicker way to read order status code
        public void ResetOrderStatusAccordingToLineStatus(int orderId, int profileId, string fullName)
        {
            string orderStatus = GetOrderStatusCodeByOrderId(orderId);
            string message = string.Empty;

            // Flag order status only if it is not Shipping or Invoiced
            if (orderStatus != OrderStatusCode.SHIPPING && orderStatus != OrderStatusCode.INVOICED)
            {
                // if one of the line statuses below is found, flag order status accordingly
                if (HasThisLineStatusInOrder(orderId, LineStatusCode.STOCK_WARNING))
                {
                    if (orderStatus != OrderStatusCode.STOCK_WARNING)
                    {
                        UpdateOrderStatusCodeByOrderId(orderId, OrderStatusCode.STOCK_WARNING);
                        message = string.Format("Status was updated to <i>{0}</i>.", GetOrderStatusByCode(OrderStatusCode.STOCK_WARNING));
                    }
                }
                else if (HasAllLineWithThisStatusInOrder(orderId, LineStatusCode.GOODS_ALLOCATED, LineStatusCode.DESPATCHED))
                {
                    if (orderStatus != OrderStatusCode.ORDER_PLACED)
                    {
                        UpdateOrderStatusCodeByOrderId(orderId, OrderStatusCode.ORDER_PLACED);
                        message = string.Format("Status was updated to <i>{0}</i>.", GetOrderStatusByCode(OrderStatusCode.ORDER_PLACED));
                    }
                }
                else if (HasThisLineStatusInOrder(orderId, LineStatusCode.AWAITING_STOCK))
                {
                    if (orderStatus != OrderStatusCode.STOCK_WARNING)
                    {
                        UpdateOrderStatusCodeByOrderId(orderId, OrderStatusCode.STOCK_WARNING);
                        message = string.Format("Status was updated to <i>{0}</i>.", GetOrderStatusByCode(OrderStatusCode.STOCK_WARNING));
                    }
                }                
                else if (HasThisLineStatusInOrder(orderId, LineStatusCode.CANCELLED))
                {
                    if (orderStatus == OrderStatusCode.PARTIAL_SHIPPING)
                    {
                        UpdateOrderStatusCodeByOrderId(orderId, OrderStatusCode.SHIPPING);
                        message = string.Format("Status was updated to <i>{0}</i>.", GetOrderStatusByCode(OrderStatusCode.SHIPPING));
                    }
                }
            }

            if (!string.IsNullOrEmpty(message))
            {
                ProcessOrderCommentInsertion(orderId,
                                             profileId,
                                             fullName,
                                             "Order",
                                             message,
                                             string.Empty);
            }
        }

        public IList<KeyValuePair<LineItem, int>> ProcessPickingLineItems(IList<LinePickingStatus> ilist)
        {
            List<LinePickingStatus> list = ilist.ToList();
            // format: product price id, colour value, size, quantity owned
            var stockWarnItems = new List<int>();
            var goodAllocItems = new List<int>();
            var stockWarnOrders = new List<int>();
            var orderedItems = new List<int>();

            var excludedItems = new List<KeyValuePair<LineItem, int>>();
            
            for (int i = 0; i < list.Count; i++)
            {
                // Get all line items with the product price id                
                var items = GetLineItemsByProductPrice(list[i].ProductPriceId,
                                                      new string[] { LineStatusCode.PICK_IN_PROGRESS },
                                                      new string[] { OrderStatusCode.ORDER_PLACED, OrderStatusCode.PARTIAL_SHIPPING });

                int owed = list[i].OwedQty;
                
                for (int j = 0; j < items.Count; j++)
                {
                    var currentItems = _lineItemRepository.Table.Where(l => l.OrderId == items[j].OrderId).ToList();

                    if (owed > 0)
                    {
                        // If owed quantity is less than the line quantity
                        if (owed < items[j].PendingQuantity)
                        {
                            // Put this left over item in stock
                            int leftOver = items[j].PendingQuantity - owed;

                            // Add this left over item to excluded list
                            excludedItems = (List<KeyValuePair<LineItem, int>>)AddToExcludedItems(excludedItems, items[j], leftOver);

                            // Update product price's stock here
                            var price = _productPriceRepository.Return(items[j].ProductPriceId);
                            price.Stock = price.Stock + leftOver;
                            _productPriceRepository.Update(price);
                        }

                        // Reduce owed quantity accordingly
                        owed = owed - items[j].Quantity;

                        // Since this order is incomplete due to current stock warning item, 
                        // other items in the order should put back to stock too
                        for (int k = 0; k < currentItems.Count; k++)
                        {
                            LinePickingStatus foundOwedItem = list.Find(delegate(LinePickingStatus owedItem)
                            {
                                return owedItem.ProductPriceId == currentItems[k].ProductPriceId && owedItem.Option == currentItems[k].Option;
                            });

                            bool isGoodAlloc = goodAllocItems.Exists(delegate(int lineItemId)
                            {
                                return lineItemId == currentItems[k].Id;
                            });

                            if (currentItems[k].ProductPriceId != items[j].ProductPriceId && foundOwedItem != null && (currentItems[k].StatusCode != LineStatusCode.PICK_IN_PROGRESS || isGoodAlloc))
                            {
                                // Update owed item list
                                list[i].OwedQty = foundOwedItem.OwedQty - currentItems[k].PendingQuantity;

                                // Update product price's stock here
                                var price = _productPriceRepository.Return(currentItems[k].ProductPriceId);
                                price.Stock = price.Stock + currentItems[k].PendingQuantity;
                                _productPriceRepository.Update(price);

                                // Update this line item as ordered status too
                                //subOrder.UpdateLineItemStatusCode(LineStatus.ORDERED_CODE, currentItems[k].LineItemId);
                                if (!orderedItems.Exists(delegate(int lineItemId)
                                {
                                    return lineItemId == currentItems[k].Id;
                                }))
                                    orderedItems.Add(currentItems[k].Id);

                                // Add this left over item to excluded list
                                excludedItems = (List<KeyValuePair<LineItem, int>>)AddToExcludedItems(excludedItems, currentItems[k], currentItems[k].PendingQuantity);
                            }
                        }

                        // Check to see if there are other stock warning items in this order
                        bool isOrdered = currentItems.Exists(delegate(LineItem item)
                        {
                            return item.StatusCode == LineStatusCode.STOCK_WARNING;
                        });

                        if (isOrdered)
                            orderedItems.Add(items[j].Id);
                        else
                        {
                            // If something owed, put in stock warning list
                            stockWarnItems.Add(items[j].Id);
                        }

                        bool found = stockWarnItems.Exists(delegate(int orderId)
                        {
                            return orderId == items[j].OrderId;
                        });

                        if (!stockWarnItems.Exists(delegate(int orderId)
                        {
                            return orderId == items[j].OrderId;
                        }))
                            stockWarnOrders.Add(items[j].OrderId);
                    }
                    else
                    {
                        bool isOrdered = false;

                        for (int l = 0; l < currentItems.Count; l++)
                        {
                            if (currentItems[l].StatusCode == LineStatusCode.STOCK_WARNING)
                            {
                                isOrdered = true;
                                break;
                            }

                            isOrdered = stockWarnItems.Exists(delegate(int lineItemId)
                            {
                                return lineItemId == currentItems[l].Id;
                            });

                            if (isOrdered) break;
                        }

                        if (isOrdered)
                        {
                            if (!orderedItems.Exists(delegate(int lineItemId)
                            {
                                return lineItemId == items[j].Id;
                            }))
                                orderedItems.Add(items[j].Id);
                        }
                        else
                            // Put the rest in goods allocated list
                            goodAllocItems.Add(items[j].Id);
                    }
                }
            }

            // Update order of the line item as 'Stock Warning'            
            for (int i = 0; i < stockWarnOrders.Count; i++)                
                UpdateOrderStatusCodeByOrderId(stockWarnOrders[i], OrderStatusCode.STOCK_WARNING);                
            
            // Update line 'Stock Warning'            
            UpdateLineItemStatusCodeByLineItemIdList(stockWarnItems, LineStatusCode.STOCK_WARNING);
            
            // Update line 'Goods Allocated'
            UpdateLineItemStatusCodeByLineItemIdList(goodAllocItems, LineStatusCode.GOODS_ALLOCATED);
            
            // Update line 'Ordered'
            UpdateLineItemStatusCodeByLineItemIdList(orderedItems, LineStatusCode.ORDERED);

            return excludedItems;
        }

        public bool HasFullyPaid(int orderId)
        {
            // Order which is paid by phone is fully paid by default.
            bool foundPaidByPhone = _orderRepository.Table.Where(o => o.Id == orderId)
                .Where(o => o.PaymentMethod == PaymentType.PAID_BY_PHONE)
                .Where(o => o.Paid == true)
                .Any();

            if (foundPaidByPhone) return true;
            
            // Calculate total amount based on number of refunds and payments.
            decimal orderTotal = _orderCalculator.GetOrderTotal(orderId);
            decimal refundTotal = CalculateTotalRefundedAmountByOrderId(orderId);
            decimal prevPaidAmount = CalculateTotalPaidAmountByOrderId(orderId);
            decimal pendingAmount = RoundPrice(orderTotal - refundTotal - prevPaidAmount);

            // Due to exchange conversion, it could have 0.01 difference.
            return pendingAmount <= 0.01M;
        }

        /// <summary>
        /// To process order and line item by making sure quantities and statuses are updated correctly.
        /// </summary>        
        public void ProcessOrderShipment(int orderId, 
                                         int profileId,
                                         string fullName,
                                         string carrier,
                                         string trackingNumber,
                                         bool sendEmail)
        {
            #region 1. Retrieve shippable items

            // Format for listItem: LineItemId, (new) LineStatus
            var listItem = new List<KeyValuePair<int, string>>();

            var cartItems = RetrieveShippableItems(orderId);
            StringBuilder productString = new StringBuilder();
            StringBuilder sbComment = new StringBuilder("Item(s) that was shipped.<br/><br/>");

            foreach (LineItem item in cartItems)
            {
                if (productString.ToString() != string.Empty)
                    productString.Append("<br/><br/>");

                #region Pharmaceutical check (codeine)
                var codeineMessage = string.Empty;
                var count = _productGroupMappingRepository.Table
                    .Join(_productGroupRepository.Table, pgm => pgm.ProductGroupId, pg => pg.Id, (pgm, pg) => new { pgm, pg })
                    .Where(x => x.pgm.ProductId == item.ProductId)
                    .Where(x => x.pg.Id == (int)ProductGroupType.Codeine)
                    .Count();

                if (count > 0)
                    codeineMessage = "<br/><small>This item contains codeine. It should not be used for longer than 3 days, if symptoms persist please contact your doctor. Please be aware codeine can be addictive.</small>";

                #endregion

                productString.AppendFormat("{0}<br/>Quantity: {1}{2}",  item.Name, item.AllocatedQuantity, codeineMessage);
                
                string lineStatusCode = LineStatusCode.DESPATCHED;
                if ((item.AllocatedQuantity + item.ShippedQuantity) < item.Quantity)
                    lineStatusCode = LineStatusCode.AWAITING_STOCK;

                listItem.Add(new KeyValuePair<int, string>(item.Id, lineStatusCode));

                sbComment.Append(item.AllocatedQuantity + " x ");
                sbComment.Append("<u>" + item.Name + "</u>");
                sbComment.AppendFormat("<br/>");               
            }

            #endregion

            #region 2. Process each line item

            // Format for listItemQty: LineItemId, pending quantity
            var listItemQty = new List<KeyValuePair<int, int>>();

            for (int i = 0; i < listItem.Count; i++)
            {
                var item = _lineItemRepository.Return(listItem[i].Key);                

                if (item != null)
                {
                    // Update line item with new status
                    item.StatusCode = listItem[i].Value;

                    // Get Line Item Id and Pending Quantity only for next process
                    listItemQty.Add(new KeyValuePair<int, int>(item.Id, item.PendingQuantity));

                    int newShippedQty = item.ShippedQuantity + item.AllocatedQuantity;  // Shipped + Allocated
                    int newPendingQty = item.PendingQuantity - item.AllocatedQuantity;  // Pending - Allocated

                    if (newPendingQty < 0) newPendingQty = 0; // Reset negative value to 0

                    // Update shipped quantity in line item
                    item.ShippedQuantity = newShippedQty;

                    // Update pending quantity in line item
                    item.PendingQuantity = newPendingQty;                    

                    // Update allocated quantity in line item
                    item.AllocatedQuantity = 0;

                    _lineItemRepository.Update(item);

                    // Remove item (if any) from warehouse allocation
                    DeleteWarehouseAllocationByLineItemId(item.Id);

                    // Remove item from branch allocation
                    DeleteBranchItemAllocationByLineItemId(item.Id);
                }
            }

            #endregion

            #region 3. Process order

            var isPartial = _lineItemRepository.Table
                .Where(l => l.OrderId == orderId)
                .Where(l => l.StatusCode != LineStatusCode.DESPATCHED)
                .Where(l => l.StatusCode != LineStatusCode.CANCELLED)
                .Where(l => l.StatusCode != LineStatusCode.SCHEDULED_FOR_CANCEL)
                .Any();

            // Update order accordingly
            var newOrderStatus = OrderStatusCode.SHIPPING;
            if (isPartial)
            {
                newOrderStatus = OrderStatusCode.PARTIAL_SHIPPING;
                
                // Clear order issue code
                UpdateOrderIssueCodeByOrderId(orderId, string.Empty);
            }

            UpdateOrderStatusCodeByOrderId(orderId, newOrderStatus);

            #endregion

            #region 4. Create order shipment

            var shippingAddress = GetShippingAddressViewModelByOrderId(orderId);

            // Construct order shipment
            OrderShipment newShipment = new OrderShipment
            {
                OrderId = orderId,
                Carrier = carrier,
                TrackingRef = trackingNumber,
                TimeStamp = DateTime.Now,
                UserId = profileId,
                Name = shippingAddress.Name,
                AddressLine1 = shippingAddress.AddressLine1,
                AddressLine2 = shippingAddress.AddressLine2,
                City = shippingAddress.City,
                County = shippingAddress.County,
                PostCode = shippingAddress.PostCode,
                CountryId = shippingAddress.CountryId,
                USStateId = shippingAddress.USStateId
            };

            int id = _orderShipmentRepository.Create(newShipment);

            // Add item shipment with new order shipment id
            foreach (var item in listItemQty)
            {
                _itemShipmentRepository.Create(new ItemShipment
                {
                    OrderShipmentId = id,
                    LineItemId = item.Key,
                    Quantity = item.Value
                });
            }

            #endregion

            #region 5. Create order comment

            ProcessOrderCommentInsertion(orderId,
                                         profileId,
                                         fullName,
                                         "Despatch Order",
                                         sbComment.ToString(),
                                         string.Empty);

            #endregion

            #region 6. Send despatch email & process loyalty points

            int customerProfileId = GetProfileIdByOrderId(orderId);

            // Except for anonymous order which has user id as 0
            if (customerProfileId > 0)
            {
                // Add loyalty points                
                var totalAwardedPoints = _orderRepository.Table.Where(x => x.Id == orderId).Select(x => x.EarnedPoint + x.AwardedPoint).FirstOrDefault();

                if (totalAwardedPoints > 0)
                {
                    var customerAccountId = _accountService.GetAccountIdByProfileId(customerProfileId);

                    _accountService.InsertRewardPointHistory(
                        customerAccountId,
                        points: totalAwardedPoints,
                        message: string.Format("Earned points for order ID {0}.", orderId),
                        orderId: orderId);
                }

                #region Send email

                if (sendEmail)
                {
                    var account = _accountRepository.Table.Where(x => x.ProfileId == customerProfileId).FirstOrDefault();

                    if (account != null)
                    {
                        _emailManager.SendDespatchConfirmationEmail(account.Email,
                                                                    account.Name.Trim(),
                                                                    orderId.ToString(),
                                                                    productString.ToString());                        
                    }
                }

                #endregion
            }

            #endregion
        }

        public void ProcessPendingLineItems(int profileId, string fullName, DateTime[] fromDates)
        {
            var items = new List<LineItem>();

            // Get line items by dates (from orders)
            foreach (var date in fromDates)
            {
                items.AddRange(GetLineItemListByStatusCodeAndDate(date, date.AddDays(1D).AddMilliseconds(-1D),
                                                                  new string[] { OrderStatusCode.ORDER_PLACED, OrderStatusCode.PARTIAL_SHIPPING },
                                                                  new string[] { LineStatusCode.ORDERED, LineStatusCode.PENDING }));
            }

            var statusPickInProgress = GetLineStatusByCode(LineStatusCode.PICK_IN_PROGRESS);

            for (int i = 0; i < items.Count; i++)
            {
                var lineStatusCode = items[i].StatusCode;
                UpdateLineItemStatusCodeByLineItemId(items[i].Id, LineStatusCode.PICK_IN_PROGRESS);

                // Add comment
                StringBuilder sb = new StringBuilder("List item was updated successfully.<br/><br/>");
                sb.AppendFormat("<u>{0} {1}</u>", items[i].Name, items[i].Option);
                sb.AppendFormat("<br/>Line Status <i>{0}</i> -> <i>{1}</i><br/>", GetLineStatusByCode(lineStatusCode), statusPickInProgress);

                ProcessOrderCommentInsertion(items[i].OrderId,
                                             profileId,
                                             fullName,
                                             "Line Item (Ordered Line Items)",
                                             sb.ToString(),
                                             string.Empty);
            }
        }
        
        public void SendOrderConfirmationEmail(int orderId, string name, string email)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(email)) throw new ArgumentNullException("email");

            var order = GetCompleteOrderById(orderId);
            if (order == null) throw new InvalidOperationException("Order is not found.");
            
            SendEmail(order, name, email);            
        }

        public void SendOrderConfirmationEmail(Order order, string name, string email)
        {
            if (order == null) throw new ArgumentNullException("order");
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(email)) throw new ArgumentNullException("email");

            SendEmail(order, name, email);
        }

        public void SetAllSystemCheckItemsByOrderId(int orderId, bool enabled)
        {
            var order = _orderRepository.Return(orderId);
            var account = _accountRepository.Return(order.ProfileId);

            string email = account.Email;
            string billName = order.BillTo;
            string shipName = order.ShipTo;
            string billAddr = order.AddressLine1 + (string.IsNullOrWhiteSpace(order.AddressLine2) ? string.Empty : " " + order.AddressLine2);
            string shipAddr = order.ShippingAddressLine1 + (string.IsNullOrWhiteSpace(order.ShippingAddressLine2) ? string.Empty : " " + order.ShippingAddressLine2);
            string billPostCode = order.PostCode;
            string shipPostCode = order.ShippingPostCode;

            UpdateSysCheckNameEnabled(billName, enabled);
            UpdateSysCheckNameEnabled(shipName, enabled);
            UpdateSysCheckAddressEnabled(billAddr, enabled);
            UpdateSysCheckAddressEnabled(shipAddr, enabled);
            UpdateSysCheckPostCodeEnabled(billPostCode, enabled);
            UpdateSysCheckPostCodeEnabled(shipPostCode, enabled);
            UpdateSysCheckEmailEnabled(email, enabled);
        }

        public void ProcessOrderForPayByPhone(int orderId, string paymentReference)
        {
            var order = _orderRepository.Return(orderId);

            if (order != null)
            {
                // Update order details
                order.Paid = true;
                order.DatePaid = DateTime.Now;
                order.StatusCode = OrderStatusCode.ORDER_PLACED;
                order.PaymentMethod = PaymentType.PAID_BY_PHONE;
                order.PaymentRef = paymentReference;

                _orderRepository.Update(order);

                // Update line items details
                UpdateLineItemStatusCodeByOrderId(orderId, LineStatusCode.ORDERED);

                // Update stock
                var items = _lineItemRepository.Table.Where(l => l.OrderId == orderId).ToList();
                TakeStock(items);

                // Process system check on this order
                bool status = ProcessSystemChecking(orderId);

                if (status == false)
                {
                    UpdateOrderStatusCodeByOrderId(orderId, OrderStatusCode.ON_HOLD);
                    UpdateOrderIssueCodeByOrderId(orderId, OrderIssueCode.SYSTEM_CHECK_FAILED);
                }
            }
        }

        /// <summary>
        /// Calculate VAT using exchange rate.
        /// </summary>
        public decimal CalculateVATByOrderId(int orderId)
        {
            var order = GetCompleteOrderById(orderId);
            var vat = _orderCalculator.GetVAT(order);

            return vat;
        }

        public void SendEmailInvoiceRequest(int orderId, 
                                            string firstName, 
                                            string email, 
                                            string message, 
                                            string currencyCode,
                                            string contactNumber, 
                                            decimal amountInCurrency, 
                                            string endDate)
        {
            var userId = _orderRepository.TableNoTracking.Where(o => o.Id == orderId).Select(o => o.ProfileId).FirstOrDefault();
            var currency = _currencyRepository.Table.Where(c => c.CurrencyCode == currencyCode).FirstOrDefault();

            // Save email invoice into database
            EmailInvoice emailInvoice = new EmailInvoice
            {
                OrderId = orderId,
                FirstName = firstName,
                Email = email,
                Message = message,
                ProfileId = userId,
                ContactNumber = contactNumber,
                Amount = amountInCurrency / currency.ExchangeRate,
                CurrencyCode = currency.CurrencyCode,
                ExchangeRate = currency.ExchangeRate,
                EndDate = DateTime.ParseExact(endDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)
            };
            
            emailInvoice.Id = _emailInvoiceRepository.Create(emailInvoice);

            // TODO: Encrypted key couldn't work in URL. We use double GUID instead. Format is [GUID]@@[GUID].
            // Generate encoded key
            // Format: EmailInvoiceId@@OrderId@@UserId
            //string encodedKey = _crypto.Encrypt(emailInvoice.Id + "@@" + emailInvoice.OrderId + "@@" + emailInvoice.ProfileId);
            //emailInvoice.EncodedKey = HttpUtility.UrlEncode(encodedKey);

            emailInvoice.EncodedKey = Guid.NewGuid().ToString().ToLower() + "@@" + Guid.NewGuid().ToString().ToLower();

            // Generate payment ref
            emailInvoice.PaymentRef = "Email-Invoice-" + DateTime.Now.ToString("ddMMyyyyHHmmss");

            // Update EncodedKey & PaymentRef in database
            _emailInvoiceRepository.Update(emailInvoice);

            // Send email
            string storeFrontSecuredUrl = _storeInformationSettings.StoreFrontSecuredLink;
            _emailManager.SendPaymentInvoiceEmail(
                emailInvoice.Email,
                emailInvoice.FirstName,
                emailInvoice.Message,
                GetFormattedPrice(emailInvoice.Amount * emailInvoice.ExchangeRate, emailInvoice.CurrencyCode),
                string.Format(storeFrontSecuredUrl + "checkout/invoicepayment?k=" + emailInvoice.EncodedKey));            
        }
        
        /// <summary>
        /// Process order cancellation.
        /// </summary>
        /// <param name="refundAmount">Amount in current currency</param>
        public OrderCancellationResults ProcessOrderCancellation(int refundId, decimal refundAmount, int refundPoint, string reason)
        {
            var refund = _refundRepository.Return(refundId);
            if (refund == null)
            {
                _logger.InsertLog(LogLevel.Error, string.Format("Refund could not be found. Refund ID={{{0}}}.", refundId));
                return OrderCancellationResults.CancellationNotFound;
            }

            refund.ValueToRefund = refundAmount / refund.ExchangeRate;
            refund.PointToRefund = refundPoint;
            refund.Reason = reason;
            refund.DateStamp = DateTime.Now;

            _refundRepository.Update(refund);

            var output = new TransactionOutput
            {
                TransactionResult = TransactionResults.Success
            };

            if (refund.ValueToRefund > 0M) output = _paymentSystemService.ProcessCancel(refund);
            
            switch (output.TransactionResult)
            {
                case TransactionResults.Success:
                case TransactionResults.PaymentTransactionNotFound:
                    // We can still proceed to cancel the order if payment is not found.
                    refund.IsCompleted = true;
                    _refundRepository.Update(refund);

                    UpdateOrderStatusCodeByOrderId(refund.OrderId, OrderStatusCode.CANCELLED);
                    UpdateLineItemStatusCodeByOrderId(refund.OrderId, LineStatusCode.CANCELLED);

                    var list = _lineItemRepository.Table.Where(l => l.OrderId == refund.OrderId).Select(l => l.Id).ToList();

                    foreach (var id in list)
                    {
                        DeleteBranchItemAllocationByLineItemId(id);
                        DeleteWarehouseAllocationByLineItemId(id);
                    }

                    if (refund.PointToRefund > 0)
                    {
                        var profileId = _orderRepository.Table.Where(o => o.Id == refund.OrderId).Select(o => o.ProfileId).FirstOrDefault();
                        var accountId = _accountService.GetAccountIdByProfileId(profileId);
                        
                        _accountService.InsertRewardPointHistory(accountId, 
                            points: refund.PointToRefund, 
                            message: string.Format("Refund for order ID {0}.", refund.OrderId), 
                            orderId: refund.OrderId);
                    }

                    break;
                default:
                    return OrderCancellationResults.Error;
            }

            return OrderCancellationResults.Success;
        }

        /// <summary>
        /// Process order refund.
        /// </summary>
        /// <param name="refundAmount">Amount in current currency</param>
        public OrderRefundResults ProcessOrderRefund(int refundId, decimal refundAmount, int refundPoint, string reason)
        {
            var refund = _refundRepository.Return(refundId);
            if (refund == null)
            {
                _logger.InsertLog(LogLevel.Error, string.Format("Refund could not be found. Refund ID={{{0}}}.", refundId));
                return OrderRefundResults.RefundNotFound;
            }

            refund.ValueToRefund = refundAmount / refund.ExchangeRate;
            refund.PointToRefund = refundPoint;
            refund.Reason = reason;
            refund.DateStamp = DateTime.Now;

            _refundRepository.Update(refund);

            var output = new TransactionOutput
            {
                TransactionResult = TransactionResults.Success
            };

            if (refund.ValueToRefund > 0M) output = _paymentSystemService.ProcessRefund(refund);

            switch (output.TransactionResult)
            {
                case TransactionResults.Success:
                case TransactionResults.PaymentTransactionNotFound:
                    refund.IsCompleted = true;
                    _refundRepository.Update(refund);

                    decimal refundTotal = CalculateTotalRefundedAmountByOrderId(refund.OrderId);
                    decimal previousAmount = CalculateTotalPaidAmountByOrderId(refund.OrderId);
                    
                    if (previousAmount != 0M && refundTotal >= previousAmount)
                    {
                        UpdateOrderStatusCodeByOrderId(refund.OrderId, OrderStatusCode.CANCELLED);
                        UpdateLineItemStatusCodeByOrderId(refund.OrderId, LineStatusCode.CANCELLED);

                        var list = _lineItemRepository.Table.Where(l => l.OrderId == refund.OrderId).Select(l => l.Id).ToList();

                        foreach (var id in list)
                        {
                            DeleteBranchItemAllocationByLineItemId(id);
                            DeleteWarehouseAllocationByLineItemId(id);
                        }
                    }

                    if (refund.PointToRefund > 0)
                    {
                        var profileId = _orderRepository.Table.Where(o => o.Id == refund.OrderId).Select(o => o.ProfileId).FirstOrDefault();
                        var accountId = _accountService.GetAccountIdByProfileId(profileId);

                        _accountService.InsertRewardPointHistory(accountId,
                            points: refund.PointToRefund,
                            message: string.Format("Refund for order ID {0}.", refund.OrderId),
                            orderId: refund.OrderId);
                    }
                    break;
                default:                
                    return OrderRefundResults.Error;
            }

            return OrderRefundResults.Success;
        }

        public void ProcessOrderCommentInsertion(int orderId, int profileId, string fullName, string title, string message, string comment)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("<b>{0}</b>", title);

            if (message != string.Empty)
                sb.AppendFormat("<br/>{0}", message);

            if (comment != string.Empty)
            {
                if (message != string.Empty) sb.Append("<br/>");
                sb.AppendFormat("<br/><q>{0}</q>", comment);
            }

            var orderComment = new OrderComment
            {
                OrderId = orderId,
                ProfileId = profileId,
                CommentText = sb.ToString(),
                DateStamp = DateTime.Now,
                FullName = fullName
            };

            _orderCommentRepository.Create(orderComment);
        }

        public Order CreateOrderFromCart(int profileId, string paymentMethod, string clientIPAddress, Address billingAddress, Address shippingAddress, string orderStatus = OrderStatusCode.PENDING, string lineStatus = LineStatusCode.PENDING)
        {
            var order = new Order();
            order.ProfileId = profileId;

            var gas = _genericAttributeService.GetAttributesForEntity(profileId, "Profile");

            // Get allocated points
            var gaAllocatedPoint = gas.FirstOrDefault(ga => ga.Key == SystemCustomerAttributeNames.AllocatedPoint);
            if (gaAllocatedPoint != null) order.AllocatedPoint = Convert.ToInt32(gaAllocatedPoint.Value);

            // Get promocode
            var gaDiscountCouponCode = gas.FirstOrDefault(ga => ga.Key == SystemCustomerAttributeNames.DiscountCouponCode);
            if (gaDiscountCouponCode != null) order.PromoCode = gaDiscountCouponCode.Value;

            // Get shipping option
            var gaSelectedShippingOption = gas.FirstOrDefault(ga => ga.Key == SystemCustomerAttributeNames.SelectedShippingOption);
            if (gaSelectedShippingOption == null) throw new ApolloException("Shipping option is not selected. Please go to basket page to select your shipping option.");
            var shippingOption = _shippingService.GetShippingOptionById(Convert.ToInt32(gaSelectedShippingOption.Value));
            if (shippingOption.CountryId != shippingAddress.CountryId) throw new ApolloException("Shipping option is not valid. Please go to basket page to select your shipping option.");
                
            order.ShippingOptionId = shippingOption.Id;

            var orderTotals = _cartService.CalculateOrderTotals(profileId, true);

            order.CurrencyCode = orderTotals.CurrencyCode;
            order.ExchangeRate = orderTotals.ExchangeRate;
            order.EarnedPoint = orderTotals.EarnedPoints;
            order.AwardedPoint = orderTotals.AwardedPoints;
            order.ShippingCost = orderTotals.ShippingCost;
            order.DiscountAmount = orderTotals.Discount;
            order.GrandTotal = orderTotals.Total * orderTotals.ExchangeRate;

            order.PaymentMethod = paymentMethod;
            order.IPAddress = clientIPAddress;
            order.StatusCode = orderStatus;
            order.OrderPlaced = DateTime.Now;
            order.LastActivityDate = DateTime.Now;
            order.LastAlertDate = DateTime.Now;

            #region Billing Address
            if (billingAddress != null)
            {
                order.BillTo = billingAddress.Name;
                order.AddressLine1 = billingAddress.AddressLine1;
                order.AddressLine2 = billingAddress.AddressLine2;
                order.County = billingAddress.County;
                order.City = billingAddress.City;
                order.PostCode = billingAddress.PostCode;
                order.CountryId = billingAddress.CountryId;
                order.Country = billingAddress.Country;
                order.USStateId = billingAddress.USStateId;
                order.USState = billingAddress.USState;
            }
            #endregion

            #region Shipping Address
            if (shippingAddress != null)
            {
                order.ShipTo = shippingAddress.Name;
                order.ShippingAddressLine1 = shippingAddress.AddressLine1;
                order.ShippingAddressLine2 = shippingAddress.AddressLine2;
                order.ShippingCounty = shippingAddress.County;
                order.ShippingCity = shippingAddress.City;
                order.ShippingPostCode = shippingAddress.PostCode;
                order.ShippingCountryId = shippingAddress.CountryId;
                order.ShippingCountry = shippingAddress.Country;
                order.ShippingUSStateId = shippingAddress.USStateId;
                order.ShippingUSState = shippingAddress.USState;
            }
            #endregion

            // Create order to get new order id
            int orderId = _orderRepository.Create(order);

            #region Line Items

            // Get cart items by profile id
            var cartItems = _cartItemRepository.Table.Where(c => c.ProfileId == profileId).ToList();

            // Create line item from cart item
            for (int i = 0; i < cartItems.Count; i++)
            {
                var defaultLineStatus = lineStatus;
                
                var lineItem = cartItems[i].ToLineItem(orderId, orderTotals.CurrencyCode, orderTotals.ExchangeRate, defaultLineStatus);

                _lineItemRepository.Create(lineItem);
                order.LineItemCollection.Add(_lineItemBuilder.Build(lineItem));
            }

            #endregion

            #region Pharm Order

            // Create pharm order
            order.PharmOrder = PreparePharmOrderFromCartPharmOrder(profileId, orderId);

            // If there is a pharm form, update issue code
            if (order.PharmOrder != null)
            {
                order.IssueCode = OrderIssueCode.OTC_ORDER;
                _orderRepository.Update(order);
            }

            #endregion

            return order;
        }

        public Order CreatePrescriptionOrder(int profileId, string clientIPAddress)
        {
            var account = _accountService.GetAccountOverviewModelByProfileId(profileId);            
            var shippingAddress = _accountService.GetShippingAddressByAccountId(account.Id);

            var order = CreateOrderFromCart(
                profileId: profileId,
                paymentMethod: string.Empty,
                clientIPAddress: clientIPAddress,
                billingAddress: null,
                shippingAddress: shippingAddress,
                orderStatus: OrderStatusCode.ORDER_PLACED,
                lineStatus: LineStatusCode.ON_HOLD);

            SendPrescriptionOrderEmail(order, account.Name, account.Email);

            return order;
        }

        public Dictionary<string, int> CalculateSystemCheckScore(int orderId)
        {
            return _systemCheckService.CalculateSystemCheckScore(orderId);
        }
        
        #endregion

        #region Private methods
        
        private bool ProcessWarehouseStockAllocation(LineItem item, string barcode, ref int stockLevel, ref int pendingQty, ref List<AllocatedItem> allocatedItems)
        {
            if (pendingQty <= 0 || stockLevel <= 0) return false;

            int warehouseAllocated = 0;

            if (stockLevel >= pendingQty)
            {
                warehouseAllocated = pendingQty;
                stockLevel = stockLevel - pendingQty;
                pendingQty = 0;
            }
            else
            {
                warehouseAllocated = stockLevel;
                pendingQty = pendingQty - stockLevel;
                stockLevel = 0;
            }

            // Delete any existing items from warehouse allocation 
            DeleteWarehouseAllocationByLineItemId(item.Id);

            // Create warehouse allocation item
            var allocation = new WarehouseAllocation
            {
                LineItemId = item.Id,
                ProductPriceId = item.ProductPriceId,
                Quantity = warehouseAllocated,
                Barcode = barcode
            };

            InsertWarehouseAllocation(allocation);

            AllocatedItem allocatedItem = new AllocatedItem
            {
                OrderId = item.OrderId,
                Name = (item.Name + " " + item.Option).Trim(),
                Quantity = warehouseAllocated
            };

            allocatedItems.Add(allocatedItem);
            allocatedItems = allocatedItems.OrderBy(x => x.Name).ToList();

            return pendingQty <= 0;
        }

        private List<int> AppendToBranchItems(int[] arrayLineItem, List<int> branchItems)
        {
            for (int j = 0; j < arrayLineItem.Length; j++)
            {
                int lineItemId = arrayLineItem[j];

                if (!branchItems.Contains(lineItemId))
                    branchItems.Add(lineItemId);
            }

            return branchItems;
        }

        private void SendPrescriptionOrderEmail(Order order, string name, string email)
        {
            StringBuilder sbProductString = new StringBuilder();
            string productString = string.Empty;
            Currency currency = _currencyService.GetCurrencyByCurrencyCode(order.CurrencyCode);

            foreach (var item in order.LineItemCollection)
            {
                if (sbProductString.ToString() != string.Empty)
                    sbProductString.Append("<br/>");

                sbProductString.AppendFormat(string.Format("<a href='#'>{0}</a><br/>", item.Name));
                sbProductString.AppendFormat("Option: {0}<br/>", item.Option);
                sbProductString.AppendFormat("Quantity: {0}<br/>", item.Quantity);
                sbProductString.AppendFormat("Unit price: {0}<br/>", GetFormattedPrice(item.PriceInclTax, currency.HtmlEntity));
                sbProductString.AppendFormat("Subtotal: {0}<br/>", GetFormattedPrice(item.PriceInclTax * item.Quantity, currency.HtmlEntity));
            }

            string shipUSState = string.Empty;
            
            if ((order.ShippingUSState != null) && (order.ShippingUSState.Code != string.Empty))
            {
                shipUSState = order.ShippingUSState.State + ", ";
            }

            string shippingAddress = order.ShipTo
                + "<br/>" +
                order.ShippingAddressLine1 + "<br/>" +
                (string.IsNullOrWhiteSpace(order.ShippingAddressLine2) ? string.Empty : order.ShippingAddressLine2) + "<br/>" +
                order.ShippingCity + "<br/>" +
                order.ShippingPostCode + "<br/>" +
                shipUSState + order.ShippingCountry.Name;

            decimal itemTotal = _orderCalculator.GetLineTotalExclTax(order.LineItemCollection);

            decimal vat = 0M;
            if (order.ShippingCountry.IsEC) vat = _orderCalculator.GetVAT(order);
            
            string shippingCost = order.ShippingCost == 0M ? "FREE" : GetFormattedPrice(order.ShippingCost, currency.HtmlEntity);

            _emailManager.SendOrderConfirmationEmail(
                email,
                name,
                order.Id.ToString(),
                "-",
                shippingAddress,
                order.ShippingOption.Name,
                sbProductString.ToString(),
                GetFormattedPrice(itemTotal, currency.HtmlEntity),
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                GetFormattedPrice(vat, currency.HtmlEntity),
                shippingCost,
                GetFormattedPrice(_orderCalculator.GetOrderTotal(order), currency.HtmlEntity),
                string.Empty);
        }

        private IList<int> GetRelatedOrderIdListByBranchAndStatusAndProduct(int branchId,
                                                                           int branchItemStatusId,
                                                                           int productPriceId)
        {
            IList<BranchAllocation> list = _branchAllocationRepository.Table
                .Join(_lineItemRepository.Table, ba => ba.LineItemId, li => li.Id, (ba, li) => new { ba, li })
                .Join(_branchPendingAllocationRepository.Table, ba_li => ba_li.ba.Id, bpa => bpa.BranchAllocationId, (ba_li, bpa) => new { ba_li.ba, ba_li.li, bpa })
                .Join(_branchPendingLineRepository.Table, ba_li_bpa => ba_li_bpa.bpa.BranchPendingLineId, bpl => bpl.Id, (ba_li_bpa, bpl) => new { ba_li_bpa.ba, bpl, ba_li_bpa.li })
                .Where(ba_bpl_li => ba_bpl_li.ba.BranchId == branchId
                    && ba_bpl_li.bpl.BranchItemStatusId == branchItemStatusId
                    && ba_bpl_li.li.ProductPriceId == productPriceId)
                .Select(ba_bpl_li => ba_bpl_li.ba)
                .ToList();

            IList<int> orderList = null;

            if (list != null)
            {
                orderList = new List<int>();

                foreach (var item in list)
                {
                    var lineItem = _lineItemRepository.Return(item.LineItemId);

                    if (lineItem != null)
                        if (!orderList.Contains(lineItem.OrderId))
                            orderList.Add(lineItem.OrderId);
                }
            }

            return orderList;
        }

        private string ProcessDeduplicationLineItemArray(string rawList, string targetId)
        {
            string[] arrayList = rawList.Split(',');

            int foundIndex = Array.IndexOf(arrayList, targetId);

            if (foundIndex < 0) // not found then it should be -1
            {
                Array.Resize<string>(ref arrayList, arrayList.Length + 1);
                arrayList[arrayList.Length - 1] = targetId;

                Array.Sort(arrayList);
            }

            return string.Join(",", arrayList);
        }
        
        private void TakeStock(ICollection<LineItem> lineItems)
        {
            foreach (var item in lineItems)
            {
                var price = _productPriceRepository.Return(item.ProductPriceId);
                price.Stock = price.Stock - item.Quantity;
                _productPriceRepository.Update(price);
            }
        }
        
        private OrderOverviewModel BuildOrderOverviewModel(Order order)
        {
            return new OrderOverviewModel
            {
                Id = order.Id,
                LastAlertDate = order.LastAlertDate,
                OrderPlaced = order.OrderPlaced,
                LastActivityDate = order.LastActivityDate,
                OrderStatus = GetOrderStatusByCode(order.StatusCode),
                StatusCode = order.StatusCode,
                Email = _accountService.GetEmailByProfileId(order.ProfileId),
                GrandTotal = order.GrandTotal,
                CurrencyCode = order.CurrencyCode,
                ShippingOptionId = order.ShippingOptionId,
                ShippingOptionName = order.ShippingOption == null ? string.Empty : order.ShippingOption.Name,
                ShippingCountryId = order.ShippingCountryId,
                IssueCode = order.IssueCode,
                OrderIssue = GetOrderIssueByCode(order.IssueCode),
                LastUpdatedBy = GetLastNameForLastUpdatedByOrderId(order.Id),
                InvoiceNumber = order.InvoiceNumber,
                BillTo = order.BillTo,
                ShipTo = order.ShipTo,
                ProfileId = order.ProfileId,
                AccountId = _accountService.GetAccountIdByProfileId(order.ProfileId),
                ExchangeRate = order.ExchangeRate,
                DiscountValue = order.DiscountAmount,
                AllocatedPoint = order.AllocatedPoint,
                PromoCode = order.PromoCode,
                PointValue = order.EarnedPoint,
                Paid = order.Paid,
                AwardedPoint = order.AwardedPoint,
                EarnedPoint = order.EarnedPoint,
                ShippingCost = order.ShippingCost,
                Packing = order.Packing
            };
        }

        private OrderShipmentOverviewModel BuildOrderShipmentOverviewModel(OrderShipment shipment)
        {
            return new OrderShipmentOverviewModel
            {
                Id = shipment.Id,
                OrderId = shipment.OrderId,
                Carrier = shipment.Carrier,
                TrackingRef = shipment.TrackingRef,
                TimeStamp = shipment.TimeStamp
            };
        }

        private BranchPendingLineOverviewModel BuildBranchPendingLineOverviewModel(BranchPendingLine item)
        {
            var branchItemStatus = _branchItemStatusRepository.Return(item.BranchItemStatusId);

            var statusCode = string.Empty;
            var status = string.Empty;
            if (branchItemStatus != null)
            {
                statusCode = branchItemStatus.StatusCode;
                status = GetBranchStockStatusByCode(branchItemStatus.StatusCode);
            }

            string branchName = GetBranchNameById(item.BranchId);
            IList<int> orderList = GetRelatedOrderIdListByBranchAndStatusAndProduct(item.BranchId, item.BranchItemStatusId, item.ProductPriceId);
            
            var priceCode = string.Empty;
            var option = string.Empty;
            var barcode = string.Empty;
            var price = 0M;

            if (item.ProductPrice == null)
            {
                _logger.InsertLog(LogLevel.Error, string.Format("Product price could not be loaded. Branch Pending Line ID={{{0}}}, Product Price ID={{{1}}}", item.Id, item.ProductPriceId));                
            }
            else
            {
                priceCode = item.ProductPrice.PriceCode;
                option = item.ProductPrice.Option;
                barcode = item.ProductPrice.Barcode;
                price = item.ProductPrice.Price;
            }

            return new BranchPendingLineOverviewModel
            {
                Id = item.Id,
                Name = item.Product.Name,
                Option = option,
                Barcode = barcode,
                StatusCode = statusCode,
                Status = status,
                LastUpdatedDateTime = item.LastUpdatedDateTime,
                BranchName = branchName,
                RelatedOrders = orderList,
                UrlRewrite = item.Product.UrlRewrite,
                ProductId = item.ProductId,
                ProductPriceId = item.ProductPriceId,
                PriceCode = priceCode,
                Price = price,
                BranchItemStatusId = item.BranchItemStatusId,
                BranchId = item.BranchId
            };
        }

        private LineItemOverviewModel BuildLineItemOverviewModel(LineItem item)
        {
            var orderStatusCode = GetOrderStatusCodeByOrderId(item.OrderId);
            var orderStatus = GetOrderStatusByCode(orderStatusCode);

            var productGroupId = Convert.ToInt32(ProductGroupType.Codeine);
            var codeinFound = _productGroupMappingRepository.Table
                .Join(_productGroupRepository.Table, pgm => pgm.ProductGroupId, pg => pg.Id, (pgm, pg) => new { pgm, pg })
                .Where(x => x.pgm.ProductId == item.ProductId)
                .Where(x => x.pg.Id == productGroupId)
                .Any();
            
            return new LineItemOverviewModel
            {
                Id = item.Id,
                OrderId = item.OrderId,
                ProductId = item.ProductId,
                ProductPriceId = item.ProductPriceId,
                OrderStatus = orderStatus,
                StatusCode = item.StatusCode,
                Name = item.Name,
                Barcode = item.ProductPrice == null ? string.Empty : item.ProductPrice.Barcode,
                Option = item.Option,
                Quantity = item.Quantity,
                ShippedQuantity = item.ShippedQuantity,
                AllocatedQuantity = item.AllocatedQuantity,
                PendingQuantity = item.PendingQuantity,
                InitialPriceInclTax = item.InitialPriceInclTax,
                InitialPriceExclTax = item.InitialPriceExclTax,
                PriceInclTax = item.PriceInclTax,
                PriceExclTax = item.PriceExclTax,
                InvoicedQuantity = item.InvoicedQuantity,
                InTransitQuantity = item.InTransitQuantity,
                UrlRewrite = item.Product == null ? string.Empty : item.Product.UrlRewrite,
                Note = item.Note,
                CurrencyCode = item.CurrencyCode,
                ExchangeRate = item.ExchangeRate,
                FreeWrapped = item.Wrapped,
                Weight = item.Weight,
                RestrictedGroup = 
                    (item.Product != null && item.Product.RestrictedGroups.Count > 0) ? 
                    string.Join(", ", item.Product.RestrictedGroups.Select(x => x.Name).ToArray()) : string.Empty,
                CodeinMessage = codeinFound ? "This item contains codeine. It should not be used for longer than 3 days, if symptoms persist please contact your doctor. Please be aware codeine can be addictive." : null
            };
        }

        private bool HasThisLineStatusInOrder(int orderId, string lineStatusCode)
        {
            var count = _lineItemRepository.Table
                .Where(l => l.OrderId == orderId)
                .Where(l => l.StatusCode == lineStatusCode)
                .Count();

            return count > 0;
        }

        private bool HasAllLineWithThisStatusInOrder(int orderId, params string[] lineStatusCode)
        {
            var count = _lineItemRepository.Table
                .Where(l => l.OrderId == orderId)
                .Where(l => lineStatusCode.Contains(l.StatusCode) == false)
                .Count();

            return !(count > 0);
        }
        
        private bool AllItemsHavingSameCurrencyCode(int orderId)
        {
            var count = _lineItemRepository.Table
                .Where(l => l.OrderId == orderId)
                .Select(l => l.CurrencyCode)
                .Distinct()
                .Count();

            return count == 1;
        }

        private string GetFirstCurrencyCodeFromLineItems(int orderId)
        {
            var currencyCode = _lineItemRepository.Table
                .Where(l => l.OrderId == orderId)
                .Select(l => l.CurrencyCode)
                .FirstOrDefault();

            return currencyCode;
        }

        private string BuildXmlString(string xmlRootName, string[] values)
        {
            StringBuilder xmlString = new StringBuilder();

            xmlString.AppendFormat("<{0}>", xmlRootName);
            for (int i = 0; i < values.Length; i++)
            {
                xmlString.AppendFormat("<value>{0}</value>", values[i]);
            }
            xmlString.AppendFormat("</{0}>", xmlRootName);

            return xmlString.ToString();
        }

        private decimal RoundPrice(decimal value)
        {
            return Math.Round(value, 2, MidpointRounding.AwayFromZero);
        }

        private string GetFormattedPrice(decimal price, string symbol)
        {
            return string.Format(symbol + "{0:0.00}", RoundPrice(price));
        }

        private IList<KeyValuePair<LineItem, int>> AddToExcludedItems(List<KeyValuePair<LineItem, int>> excludedItems, LineItem targetItem, int quantity)
        {
            var foundItem = excludedItems.Find(delegate(KeyValuePair<LineItem, int> item)
            {
                return item.Key.ProductPriceId == targetItem.ProductPriceId;
            });
            int existingQty = 0;

            // Due to KeyValuePair's value is read-only, existing pair will be removed and new pair will added later on
            if (foundItem.Key != null)
            {
                existingQty = foundItem.Value;
                excludedItems.Remove(foundItem);
            }

            excludedItems.Add(new KeyValuePair<LineItem, int>(targetItem, quantity + existingQty));

            return excludedItems;
        }
        
        private IList<LineItem> RetrieveShippableItems(int orderId)
        {
            var items = _lineItemRepository.Table
                .Where(l => l.OrderId == orderId)
                .Where(l => l.StatusCode == LineStatusCode.GOODS_ALLOCATED)
                .ToList();

            return items;
        }

        private int GetBranchIdByName(string name)
        {
            Branch branch = _branchRepository.TableNoTracking.Where(x => x.Name.ToLower() == name.ToLower()).FirstOrDefault();
            var id = 0;

            if (branch != null)
                id = branch.Id;

            return id;
        }

        private int GetBranchIdByVectorBranchId(string vectorBranchId)
        {
            Branch branch = _branchRepository.TableNoTracking.Where(x => x.VectorBranchId == vectorBranchId).FirstOrDefault();
            var id = 0;

            if (branch != null)
                id = branch.Id;

            return id;
        }
        
        private void SendEmail(Order order, string name, string email)
        {
            StringBuilder sbProductString = new StringBuilder();
            string productString = string.Empty;
            Currency currency = _currencyService.GetCurrencyByCurrencyCode(order.CurrencyCode);
            var currencyHtmlEntity = currency.HtmlEntity;

            foreach (var item in order.LineItemCollection)
            {
                if (sbProductString.ToString() != string.Empty)
                    sbProductString.Append("<br/>");

                sbProductString.AppendFormat("<a href='{0}product/{1}'>{2}</a><br/>", _storeInformationSettings.StoreFrontLink, item.Product.UrlRewrite, item.Name);

                #region Pharmaceutical check (codeine)

                var count = _productGroupMappingRepository.Table
                    .Join(_productGroupRepository.Table, pgm => pgm.ProductGroupId, pg => pg.Id, (pgm, pg) => new { pgm, pg })
                    .Where(x => x.pgm.ProductId == item.ProductId)
                    .Where(x => x.pg.Id == (int)ProductGroupType.Codeine)
                    .Count();

                if (count > 0)
                    sbProductString.Append("<small>This item contains codeine. It should not be used for longer than 3 days, if symptoms persist please contact your doctor. Please be aware codeine can be addictive.</small><br/>");

                #endregion

                sbProductString.AppendFormat("Product code: {0}<br/>", item.ProductId);
                
                sbProductString.AppendFormat("Option: {0}<br/>", item.Option);
                sbProductString.AppendFormat("Quantity: {0}<br/>", item.Quantity);
                sbProductString.AppendFormat("Unit price: {0}<br/>", GetFormattedPrice(item.PriceExclTax * order.ExchangeRate, currencyHtmlEntity));
                sbProductString.AppendFormat("Subtotal: {0}<br/>", GetFormattedPrice(item.PriceExclTax * item.Quantity * order.ExchangeRate, currencyHtmlEntity));
            }

            string billUSState = string.Empty;
            string shipUSState = string.Empty;

            if ((order.USState != null) && (order.USState.Code != string.Empty))
            {
                billUSState = order.USState.State + ", ";
            }

            if ((order.ShippingUSState != null) && (order.ShippingUSState.Code != string.Empty))
            {
                shipUSState = order.ShippingUSState.State + ", ";
            }

            var billingAddress = order.BillTo + "<br/>" +
                order.AddressLine1 + "<br/>" +
                (string.IsNullOrWhiteSpace(order.AddressLine2) ? string.Empty : order.AddressLine2 + "<br/>") +
                order.City + "<br/>" +
                (string.IsNullOrWhiteSpace(order.PostCode) ? string.Empty : order.PostCode + "<br/>") +
                billUSState + order.Country.Name;

            var shippingAddress = order.ShipTo + "<br/>" +
                order.ShippingAddressLine1 + "<br/>" +
                (string.IsNullOrWhiteSpace(order.ShippingAddressLine2) ? string.Empty : order.ShippingAddressLine2 + "<br/>") +
                order.ShippingCity + "<br/>" +
                (string.IsNullOrWhiteSpace(order.ShippingPostCode) ? string.Empty : order.ShippingPostCode + "<br/>") +
                shipUSState + order.ShippingCountry.Name;

            decimal itemTotal = _orderCalculator.GetLineTotalExclTax(order.LineItemCollection);

            #region Discount
            string discountTitle = string.Empty;
            string discount = string.Empty;
            if (order.DiscountAmount != 0M)
            {
                discountTitle = "Discount:";
                discount = "-" + GetFormattedPrice(order.DiscountAmount * order.ExchangeRate, currencyHtmlEntity);
            }
            #endregion

            #region Loyalty point
            string loyaltyPointTitle = string.Empty;
            string loyaltyPoint = string.Empty;

            if (order.AllocatedPoint != 0)
            {
                loyaltyPointTitle = string.Format("Points Used({0}):", order.AllocatedPoint);
                loyaltyPoint = "-" + GetFormattedPrice((Convert.ToDecimal(order.AllocatedPoint) / 100M) * order.ExchangeRate, currencyHtmlEntity);
            }
            #endregion

            #region Tax

            decimal vat = 0M;            
            if (order.ShippingCountry.IsEC) vat = _orderCalculator.GetVAT(order);
            
            #endregion

            string shippingCost = order.ShippingCost == 0M ? "FREE" : GetFormattedPrice(order.ShippingCost * order.ExchangeRate, currencyHtmlEntity);
            
            _emailManager.SendOrderConfirmationEmail(
                email,
                name,
                order.Id.ToString(),
                billingAddress,
                shippingAddress,
                order.ShippingOption.Name,
                sbProductString.ToString(),
                GetFormattedPrice(itemTotal, currencyHtmlEntity),
                discountTitle,
                discount,
                loyaltyPointTitle,
                loyaltyPoint,
                GetFormattedPrice(vat, currencyHtmlEntity),
                shippingCost,
                GetFormattedPrice(_orderCalculator.GetOrderTotal(order), currencyHtmlEntity),
                string.Empty);
        }

        private IList<LineItemOverviewModel> GetLineItemOverviewModelsByLineStatusAndOrderStatus(string[] lineStatusCodes, string[] orderStatusCodes)
        {
            var list = _lineItemRepository.Table
                .Join(_orderRepository.Table, l => l.OrderId, o => o.Id, (l, o) => new { l, o })
                .GroupJoin(_productPriceRepository.Table, l_o => l_o.l.ProductPriceId, pp => pp.Id, (l_o, pp) => new { l_o.l, l_o.o, pp = pp.DefaultIfEmpty() })
                .SelectMany(l_o_pp => l_o_pp.pp.Select(x => new { l_o_pp.l, l_o_pp.o, pp = x }))
                .GroupJoin(_productRepository.Table, l_o_pp => l_o_pp.l.ProductId, p => p.Id, (l_o_pp, p) => new { l_o_pp.l, l_o_pp.o, l_o_pp.pp, p = p.DefaultIfEmpty() })
                .SelectMany(l_o_pp_p => l_o_pp_p.p.Select(x => new { l_o_pp_p.l, l_o_pp_p.o, l_o_pp_p.pp, p = x }))
                .Where(x => lineStatusCodes.Contains(x.l.StatusCode))
                .Where(x => orderStatusCodes.Contains(x.o.StatusCode))
                .Select(x => new
                {
                    x.l.Id,
                    x.l.OrderId,
                    x.l.Name,
                    x.l.Option,
                    x.l.PendingQuantity,
                    x.l.ProductId,
                    x.l.ProductPriceId,
                    UrlRewrite = x.p != null ? x.p.UrlRewrite : null,
                    Barcode = x.pp != null ? x.pp.Barcode : null
                })
                .ToList()
                .Select(x => new LineItemOverviewModel
                {
                    Id = x.Id,
                    OrderId = x.OrderId,
                    Name = x.Name,
                    Option = x.Option,
                    PendingQuantity = x.PendingQuantity,
                    ProductId = x.ProductId,
                    ProductPriceId = x.ProductPriceId,
                    UrlRewrite = x.UrlRewrite,
                    Barcode = x.Barcode
                })
                .ToList();

            return list;
        }

        private IList<LineItemOverviewModel> GetLineItemOverviewModelsByStatusAndBranch(string[] orderStatusCodes,
                                                                                        string[] lineStatusCodes,
                                                                                        int branchId,
                                                                                        AdmissionLineSortingType orderBy = AdmissionLineSortingType.Name)
        {
            var query = _lineItemRepository.Table
                .Join(_orderRepository.Table, l => l.OrderId, o => o.Id, (l, o) => new { l, o })
                .Join(_branchAllocationRepository.Table, l_o => l_o.l.Id, ba => ba.LineItemId, (l_o, ba) => new { l_o.l, l_o.o, ba })
                .Where(l_o_ba => orderStatusCodes.Contains(l_o_ba.o.StatusCode))
                .Where(l_o_ba => lineStatusCodes.Contains(l_o_ba.l.StatusCode))
                .Where(l_o_ba => l_o_ba.ba.BranchId == branchId);

            switch (orderBy)
            {
                default:
                case AdmissionLineSortingType.Name:
                    query = query.OrderBy(l_o_ba => l_o_ba.l.Name);
                    break;
                case AdmissionLineSortingType.LineItemId:
                    query = query.OrderBy(l_o_ba => l_o_ba.l.Id);
                    break;
            }


            var list = query.Select(l_o_ba => new LineItemOverviewModel
                {
                    Id = l_o_ba.l.Id,
                    ProductPriceId = l_o_ba.l.ProductPriceId
                })
                .ToList();

            return list;
        }

        private IList<BranchProductStock> GetBranchProductStocks(int productPriceId)
        {
            var productStocks = _branchProductStockRepository.TableNoTracking
                .Where(b => b.ProductPriceId == productPriceId)
                .ToList();

            return productStocks ?? new List<BranchProductStock>();
        }

        private IList<BranchProductStock> GetBranchProductStocks(int[] productPriceIds)
        {
            var productStocks = _branchProductStockRepository.Table
                .Where(b => productPriceIds.Contains(b.ProductPriceId))
                .ToList();

            return productStocks ?? new List<BranchProductStock>();
        }

        #endregion      
    }
}