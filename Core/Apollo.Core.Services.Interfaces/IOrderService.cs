using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Model.OverviewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Apollo.Core.Services.Interfaces
{
    public interface IOrderService
    {
        int InsertOrderPayment(OrderPayment payment);
        int InsertLineItem(LineItem lineItem);
        int InsertSystemCheck(SystemCheck systemCheck);
        int InsertLineItemsFromCartItems(IList<CartItem> cartItems, int orderId, string currencyCode, decimal exchangeRate);
        int InsertOrder(Order order);
        int InsertBranchItemStatus(BranchItemStatus item);
        int InsertWarehouseAllocation(WarehouseAllocation warehouseAllocation);
        Order CreatePrescriptionOrder(int profileId, string clientIPAddress);

        Order GetCompleteOrderByIdAndProfileId(int orderId, int profileId);
        EmailInvoice GetEmailInvoiceByEncodedKey(string encodedKey);
        EmailInvoice GetEmailInvoiceById(int emailInvoiceId);
        int[] GetPrevNextOrderId(int orderId);
        IList<EmailInvoice> GetPaidEmailInvoicesByOrderId(int orderId);
        PaymentHeaderOverviewModel GetPaymentHeaderOverviewModelByOrderId(int orderId);
        SagePayDirectOverviewModel GetLastSagePayDirectViewModelByOrderId(int orderId);
        string GetOfferNameByOrderId(int orderId);
        string GetLatestStatusFromLog(int orderId);
        string GetOrderIssueByCode(string issueCode);
        OrderHeaderOverviewModel GetOrderHeaderOverviewModelByOrderId(int orderId);
        AddressOverviewModel GetBillingAddressViewModelByOrderId(int orderId);
        SystemCheck GetSystemCheckByOrderId(int orderId);
        int GetPaidOrderCountByProfileId(int profileId);
        OrderShipment GetOrderShipmentById(int orderShipmentId);
        PagedList<OrderShipmentOverviewModel> GetPagedOrderShipmentOverviewModel(
            int pageIndex = 0,
            int pageSize = 2147483647,  //Int32.MaxValue
            IList<int> orderIds = null,
            string shippingName = null,
            string trackingRef = null,
            OrderShipmentSortingType orderBy = OrderShipmentSortingType.OrderIdDesc);
        IList<LineItem> GetLastNDaysPharmaceauticalLines(int profileId, DateTime dateFrom, DateTime dateTo, string[] lineStatusFilter, string[] orderStatusFilter);
        IList<LineItem> GetLastNDaysPharmaceauticalLinesFromThisAddress(string addressLine1, string postcode, DateTime dateFrom, DateTime dateTo, string[] lineStatusFilter, string[] orderStatusFilter);
        PharmOrder GetPharmOrderByOrderId(int orderId);
        SagePayLog GetSagePayLogById(int sagepayLogId);
        IList<SagePayLog> GetSagePayLogsByOrderId(int orderId);
        IList<int> GetOrderIdListByProfileIdAndStatusCode(int profileId, string statusCode);
        int GetProfileIdByOrderId(int orderId);
        AccountOverviewModel GetAccountOverviewModelByOrderId(int orderId);
        void SendEmailInvoiceRequest(
            int orderId,
            string firstName,
            string email,
            string message,
            string currencyCode,
            string contactNumber,
            decimal amount,
            string endDate);
        PagedList<OrderComment> GetOrderCommentLoadPaged(
            int pageIndex = 0,
            int pageSize = 2147483647,  //Int32.MaxValue
            IList<int> orderIds = null,
            string fromDateStamp = null,
            string toDateStamp = null,
            string commentText = null,
            string fullName = null,
            OrderCommentSortingType orderBy = OrderCommentSortingType.IdAsc);
        string GetCurrencyCodeByOrderId(int orderId);
        Refund GetRefundById(int refundId);
        PagedList<Refund> GetRefundLoadPaged(
            int pageIndex = 0,
            int pageSize = 2147483647,  //Int32.MaxValue
            IList<int> orderIds = null,
            bool isCancellation = false,
            bool? isCompleted = null,
            string fromDateStamp = null,
            string toDateStamp = null,
            RefundSortingType orderBy = RefundSortingType.IdAsc);
        ShippingOverviewModel GetShippingOverviewModelByOrderId(int orderId);
        IList<BranchAllocation> GetBranchAllocationListByBranchPendingLineId(int branchPendingLineId);
        IList<BranchPendingLineDownload> GetPendingLineItemByBranchId(
            int? branchId,
            int day,
            int month,
            int year,
            bool displayPrice = false,
            string name = null,
            string option = null,
            string barcode = null,
            string statusCode = null);
        decimal GetOrderTotal(int orderId);
        string GetBranchStockStatusByCode(string statusCode);
        decimal GetVAT(int orderId);
        BranchPendingLine GetBranchPendingLineById(int branchPendingId);
        IList<BranchAllocation> GetBranchAllocationListByBranchIdAndProductPriceId(int branchId, int productPriceId);
        IList<OrderCommentOverviewModel> GetOrderCommentOverviewModelListByOrderId(int orderId, int topCount);
        AddressOverviewModel GetShippingAddressViewModelByOrderId(int orderId);
        string GetOrderStatusByCode(string code);
        Order GetCompleteOrderById(int id, int? profileId = null);
        IList<IGrouping<int, LineItem>> GetLastThreeMonthsLineItems();
        string GetOrderStatusCodeByOrderId(int id);
        int GetNumberOfPaidOrders(string promoCode, int userId);
        int GetLastNumberOfDaysOrdersByProfileId(int profileId, int days);
        Order GetLastValidOrderByProfileId(int profileId, bool useDefaultCurrency = false);
        IList<Order> GetRecentOrderListByProfileId(int profileId, int limit);
        IList<OrderStatus> GetOrderStatusList();
        int GetOrderCountByOrderStatusAndIssueCode(string statusCode, string issueCode);
        int GetOrderCountByOrderStatus(string statusCode);
        int GetRefundCountByTypeAndStatus(bool isCancellation, bool isCompleted, string fromDate = null, string toDate = null);
        DataTable GetOrderCountGroupByDate(string inLineStatusXml, string inOrderStatusXml);
        int GetOrderCountForPacking();
        int GetShipmentRate(string chosenDate, int notLessThanDays, int notMoreThanDays);
        IList<LineItemOverviewModel> GetLineItemOverviewModelListByOrderId(int orderId);
        string GetShippingOptionNameByOrderId(int orderId);
        int GetOrderCountForSpecialDelivery();
        string GetLineStatusByCode(string code);
        int GetBranchProductStockByBranchAndProductPriceId(int branchId, int productPriceId);
        IDictionary<int, int> GetBranchProductStocksByProductPriceId(int productPriceId);
        string GetDefaultBranchByBrandId(int brandId);
        IList<BranchStockStatus> GetAllBranchStockStatus();
        string GetBranchNameById(int branchId);
        IList<Branch> GetAllBranches();
        int GetBranchPendingLineSumQuantityById(int branchPendingLineId);
        IList<LineStatus> GetLineStatusList();
        LineItemOverviewModel GetLineItemOverviewModel(int lineItemId);
        int GetWarehouseAllocationQuantityByBarcode(string barcode);
        LineItem GetLineItemById(int id);
        OrderOverviewModel GetOrderOverviewModelById(int orderId);
        IList<OrderIssue> GetOrderIssueList();
        PagedList<LineItemOverviewModel> GetPagedLineItemOverviewModel(
            int pageIndex = 0,
            int pageSize = 2147483647,  //Int32.MaxValue
            IList<int> orderIds = null,
            IList<int> productIds = null,
            string name = null,
            string barcode = null,
            string option = null,
            string statusCode = null,
            LineItemSortingType orderBy = LineItemSortingType.OrderIdAsc);
        PagedList<BranchPendingLineOverviewModel> GetPagedBranchPendingLineOverviewModel(
            int pageIndex = 0,
            int pageSize = 2147483647,  //Int32.MaxValue
            string name = null,
            string option = null,
            string barcode = null,
            string statusCode = null,
            int? branchId = null,
            bool showEmptyStatus = false,
            BranchPendingLineSortingType orderBy = BranchPendingLineSortingType.NameAsc);
        IList<LineItem> GetLineItemsByProductPrice(
            int productPriceId,
            string[] lineStatusFilter = null,
            string[] orderStatusFilter = null);
        IList<LineItem> GetLineItemListByStatusCodeAndDate(DateTime fromDate,
                                                           DateTime toDate,
                                                           string[] lineStatusFilter,
                                                           string[] orderStatusFilter);
        PagedList<OrderOverviewModel> GetPagedOrderOverviewModel(
            int pageIndex = 0,
            int pageSize = 2147483647,  //Int32.MaxValue
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
            OrderSortingType orderBy = OrderSortingType.IdAsc);
        PagedList<Order> GetOrderLoadPaged(
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
            OrderSortingType orderBy = OrderSortingType.IdAsc);
        PagedList<InventoryPendingLine> GetInventoryPendingLinesLoadPaged(
            int pageIndex = 0,
            int pageSize = 2147483647,
            IList<int> productIds = null,
            string name = null);
        IList<LineItemOverviewModel> GetLineItemOverviewModels(int[] lineItemIds);

        void UpdateOrderShipment(OrderShipment shipment);
        void UpdateLineItemStatusCodeByOrderId(int orderId, string statusCode);
        void UpdateLineItemFreeWrappedByLineItemId(int lineItemId, bool freeWrapped);
        void UpdateLineItemOptionByLineItemId(int lineItemId, string option);
        void UpdateLineItemNoteByLineItemId(int lineItemId, string note);
        void UpdateLineItemQuantityByLineItemId(int lineItemId, int quantity);
        void UpdateLineItemPendingQuantity(int lineItemId, int quantity);
        void UpdateLineItemPriceByLineItemId(int lineItemId, decimal priceInclTax, decimal priceExclTax);
        void UpdateOrderAllocatedPoint(int orderId, int newAllocatedPoint);
        void UpdateOrderBillingAddressByOrderId(int orderId, Address address);
        void UpdateOrderEarnedPoint(int orderId, int newEarnedPoint);
        void UpdateOrderPackingByOrderId(int orderId, string packingNote);
        void UpdateOrderShippingCostByOrderId(int orderId, decimal shippingCost);
        void UpdateOrderDiscountByOrderId(int orderId, decimal discount);
        void UpdateOrderShippingOptionIdByOrderId(int orderId, int shippingOptionId);
        void UpdateSystemCheck(SystemCheck systemCheck);
        void UpdateOrderShippingAddressByOrderId(int orderId, Address address);
        void UpdateOrderLastAlertDateByOrderId(int orderId, DateTime lastAlertDate);
        void UpdateOrderPharmOrderAndItems(PharmOrder pharm);
        void UpdateOrder(Order order);
        void UpdateBranchAllocationLastUpdatedByBranchAllocationId(int branchAllocationId, DateTime lastUpdated);
        void UpdateBranchAllocationPrintStatus(int branchId, DateTime printedDate);
        void UpdateBranchAllocationForBranchItemStatusIByBranchPendingLineId(int branchPendingLineId, int branchItemStatusId, DateTime lastUpdated);
        void UpdateBranchPendingLineForBranchItemStatusId(int branchPendingLineId, int branchItemStatusId, DateTime lastUpdated);
        void UpdateOrderIssueCodeByOrderId(int orderId, string issueCode);
        void UpdateLineItemStatusCodeByLineItemIdList(IList<int> list, string code);
        void UpdateOrderStatusCodeByOrderId(int orderId, string statusCode);
        void UpdateLineItemAllocatedQuantity(int lineItemId, int quantity);
        void UpdateLineItemStatusCodeByLineItemId(int lineItemId, string statusCode);
        void UpdateOrderLastActivityDateByOrderId(int orderId, DateTime lastActivityDate);
        void UpdateLineItemInTransitQuantity(int lineItemId, int quantity);

        void DeleteBranchItemAllocationByLineItemId(int lineItemId);
        void DeleteBranchItemAllocationByLineItemIds(int[] lineItemIds);
        void DeleteRefund(int refundId);
        void DeleteLineItem(int lineItemId);
        void DeleteLineItemsByOrderId(int orderId);
        void DeletePharmOrderAndItemsByOrderId(int orderId);
        void DeleteWarehouseAllocationByLineItemId(int lineItemId);
        void DeleteBranchItemStatus(int branchItemStatusId);        

        decimal CalculateTotalPaidAmountByOrderId(int orderId);
        decimal CalculateTotalRefundedAmountByOrderId(int orderId);
        decimal CalculateOrderTotalByOrderId(int orderId);
        decimal CalculateVATByOrderId(int orderId);
        decimal CalculateLastValidOrderTotalByProfileId(int profileId, bool useDefaultCurrency = false);
        Dictionary<string, int> CalculateSystemCheckScore(int orderId);
        byte[] PrintOrderToInvoicePdf(int orderId, int? profileId = null);
        byte[] PrintBranchPendingLinePdf(IList<BranchPendingLineDownload> list);
        byte[] PrintInventoryPendingLinePdf();
        byte[] ExportOrdersCsv(IList<int> orderIds);
        void SendOrderConfirmationEmail(int orderId, string name, string email);
        void SendOrderConfirmationEmail(Order order, string name, string email);
        void ProcessOrderForPayByPhone(int orderId, string paymentRef);
        void ProcessOrderShipment(
            int orderId,
            int profileId,
            string fullName,
            string carrier,
            string trackingNumber,
            bool sendEmail);
        bool ProcessSystemChecking(int orderId);
        void ProcessOrderInvoice(int orderId);
        string ProcessPaymentForFulfillment(int orderId, int profileId, string fullName);
        int ProcessRefundInsertion(Refund refund);
        void ProcessBranchTransfer(int branchPendingLineId, int branchItemStatusId, int newBranchId);
        void ProcessBranchItemInsertion(IList<int> branchItems, int branchId, int profileId, string fullName);
        void ProcessOrderCommentInsertion(int orderId, int profileId, string fullName, string title, string message, string comment);
        IList<KeyValuePair<LineItem, int>> ProcessPickingLineItems(IList<LinePickingStatus> list);
        OrderRefundResults ProcessOrderRefund(int refundId, decimal refundAmount, int refundPoint, string reason);
        OrderCancellationResults ProcessOrderCancellation(int refundId, decimal refundAmount, int refundPoint, string reason);
        bool ProcessSystemCheckVerification(int orderId);
        void ProcessPendingLineItems(int profileId, string fullName, DateTime[] fromDates);
        void SetAllSystemCheckItemsByOrderId(int orderId, bool enabled);        
        string CheckIPLocationByOrderId(int orderId);
        string CheckTransactionDetailByOrderId(int orderId);
        string CheckThirdManResultByOrderId(int orderId);        
        PharmOrder PreparePharmOrderFromCartPharmOrder(int profileId, int orderId);
        void ResetLineItemForBranchAllocation(int lineItemId, int pendingQuantity);
        void ResetOrderStatusAccordingToLineStatusByOrderIds(IList<int> orderIdList, int profileId, string fullName);
        void ResetOrderStatusAccordingToLineStatus(int orderId, int profileId, string fullName);
        Order CreateOrderFromCart(int profileId, string paymentMethod, string clientIPAddress, Address billingAddress, Address shippingAddress, string orderStatus = OrderStatusCode.PENDING, string lineStatus = LineStatusCode.PENDING);
        bool HasFullyPaid(int orderId);
    }
}
