using Apollo.Core.Logging;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using Apollo.DataAccess;
using Apollo.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace Apollo.Core.Services.Report
{
    public class ReportService : BaseRepository, IReportService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<ProductPrice> _productPriceRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<Country> _countryRepository;
        private readonly IRepository<LineItem> _lineItemRepository;
        private readonly ILogger _logger;

        public ReportService(IDbContext dbContext,
                             IRepository<ProductPrice> productPriceRepository,
                             IRepository<Order> orderRepository,
                             IRepository<Country> countryRepository,
                             IRepository<LineItem> lineItemRepository,
                             ILogBuilder logBuilder)
        {
            _dbContext = dbContext;
            _productPriceRepository = productPriceRepository;
            _orderRepository = orderRepository;
            _countryRepository = countryRepository;
            _lineItemRepository = lineItemRepository;
            _logger = logBuilder.CreateLogger(GetType().FullName);
        }

        public decimal GetProductDiscountSumByDate(string inOrderStatusXml,
            string inLineStatusXml,
            bool? nonEuOnly,
            int hourFlag,
            int dayFlag,
            int weekFlag,
            int monthFlag,
            int quarterFlag,
            int yearFlag,
            int hour,
            int day,
            int week,
            int month,
            int quarter,
            int year)
        {
            var OrderStatus = GetParameter("InOrderStatusXml", inOrderStatusXml);
            var LineStatus = GetParameter("InLineStatusXml", inLineStatusXml);
            var NonEuOnly = GetParameter("NonEuOnly", nonEuOnly);
            var HourFlag = GetParameter("HourFlag", hourFlag);
            var DayFlag = GetParameter("DayFlag", dayFlag);
            var WeekFlag = GetParameter("WeekFlag", weekFlag);
            var MonthFlag = GetParameter("MonthFlag", monthFlag);
            var QuarterFlag = GetParameter("QuarterFlag", quarterFlag);
            var YearFlag = GetParameter("YearFlag", yearFlag);
            var Hour = GetParameter("Hour", hour);
            var Day = GetParameter("Day", day);
            var Week = GetParameter("Week", week);
            var Month = GetParameter("Month", month);
            var Quarter = GetParameter("Quarter", quarter);
            var Year = GetParameter("Year", year);

            var discount = _dbContext.SqlQuery<decimal>("EXEC DiscountValueSum_GetInfo @InLineStatusXml, @InOrderStatusXml, @HourFlag, @DayFlag, @WeekFlag, @MonthFlag, @QuarterFlag, @YearFlag, @Hour, @Day, @Week, @Month, @Quarter, @Year, @NonEuOnly",
                LineStatus, OrderStatus, HourFlag, DayFlag, WeekFlag, MonthFlag, QuarterFlag, YearFlag, Hour, Day, Week, Month, Quarter, Year, NonEuOnly).Single();

            return discount;
        }

        public decimal GetNonTaxableOrderValueSumByDate(string inOrderStatusXml,
            string inLineStatusXml,
            bool? nonEuOnly,
            int? vat,
            int hourFlag,
            int dayFlag,
            int weekFlag,
            int monthFlag,
            int quarterFlag,
            int yearFlag,
            int hour,
            int day,
            int week,
            int month,
            int quarter,
            int year)
        {
            object Vat = GetParameter("VAT", Convert.ToInt32(vat));
            object LineStatus = GetParameter("InLineStatusXml", inLineStatusXml);
            object OrderStatus = GetParameter("InOrderStatusXml", inOrderStatusXml);
            object NonEuOnly = GetParameter("NonEuOnly", nonEuOnly);
            object HourFlag = GetParameter("HourFlag", hourFlag);
            object DayFlag = GetParameter("DayFlag", dayFlag);
            object WeekFlag = GetParameter("WeekFlag", weekFlag);
            object MonthFlag = GetParameter("MonthFlag", monthFlag);
            object QuarterFlag = GetParameter("QuarterFlag", quarterFlag);
            object YearFlag = GetParameter("YearFlag", yearFlag);
            object Hour = GetParameter("Hour", hour);
            object Day = GetParameter("Day", day);
            object Week = GetParameter("Week", week);
            object Month = GetParameter("Month", month);
            object Quarter = GetParameter("Quarter", quarter);
            object Year = GetParameter("Year", year);

            var value = _dbContext.SqlQuery<decimal>("NonTaxableOrderValueSum_GetInfo @InLineStatusXml, @InOrderStatusXml, @HourFlag, @DayFlag, @WeekFlag, @MonthFlag, @QuarterFlag, @YearFlag, @Hour, @Day, @Week, @Month, @Quarter, @Year, @VAT",
                LineStatus, OrderStatus, HourFlag, DayFlag, WeekFlag, MonthFlag, QuarterFlag, YearFlag, Hour, Day, Week, Month, Quarter, Year, Vat).Single();

            return value;
        }

        public decimal GetOrderValueSumByDate(string inOrderStatusXml,
            string inLineStatusXml,
            bool? nonEuOnly,
            int? vat,
            int hourFlag,
            int dayFlag,
            int weekFlag,
            int monthFlag,
            int quarterFlag,
            int yearFlag,
            int hour,
            int day,
            int week,
            int month,
            int quarter,
            int year)
        {
            object Vat = GetParameter("VAT", Convert.ToInt32(vat));
            object LineStatus = GetParameter("InLineStatusXml", inLineStatusXml);
            object OrderStatus = GetParameter("InOrderStatusXml", inOrderStatusXml);
            object NonEuOnly = GetParameter("NonEuOnly", nonEuOnly);
            object HourFlag = GetParameter("HourFlag", hourFlag);
            object DayFlag = GetParameter("DayFlag", dayFlag);
            object WeekFlag = GetParameter("WeekFlag", weekFlag);
            object MonthFlag = GetParameter("MonthFlag", monthFlag);
            object QuarterFlag = GetParameter("QuarterFlag", quarterFlag);
            object YearFlag = GetParameter("YearFlag", yearFlag);
            object Hour = GetParameter("Hour", hour);
            object Day = GetParameter("Day", day);
            object Week = GetParameter("Week", week);
            object Month = GetParameter("Month", month);
            object Quarter = GetParameter("Quarter", quarter);
            object Year = GetParameter("Year", year);

            var returnValue = _dbContext.SqlQuery<decimal?>("EXEC OrderValueSum_GetInfo @InLineStatusXml, @InOrderStatusXml, @HourFlag, @DayFlag, @WeekFlag, @MonthFlag, @QuarterFlag, @YearFlag, @Hour, @Day, @Week, @Month, @Quarter, @Year, @NonEuOnly, @VAT",
                LineStatus, OrderStatus, HourFlag, DayFlag, WeekFlag, MonthFlag, QuarterFlag, YearFlag, Hour, Day, Week, Month, Quarter, Year, NonEuOnly, Vat).Single();

            return returnValue ?? 0M;
        }

        // TODO: It's a replacement method for 'GetShippingValueSumByDate'
        public decimal GetTotalShippingValueGroupByDateType(
            IList<string> orderStatus,
            bool? nonEUOnly = null,
            int? hour = null,
            int? day = null,
            int? week = null,
            int? month = null,
            int? quarter = null,
            int? year = null)
        {
            var query = _orderRepository.Table
                .Join(_countryRepository.Table, o => o.ShippingCountryId, c => c.Id, (o, c) => new { o, c })
                .Where(o_c => orderStatus.Contains(o_c.o.StatusCode));

            if (nonEUOnly.HasValue) query = query.Where(o_c => o_c.c.IsEC == nonEUOnly.Value);
            if (hour.HasValue) query = query.Where(o_c => o_c.o.OrderPlaced.Value.Hour == hour.Value);
            if (day.HasValue) query = query.Where(o_c => o_c.o.OrderPlaced.Value.Day == day.Value);
            if (week.HasValue) query = query.Where(o_c => WeekOfYearISO8601(o_c.o.OrderPlaced.Value) == week.Value);
            if (month.HasValue) query = query.Where(o_c => o_c.o.OrderPlaced.Value.Month == month.Value);
            if (quarter.HasValue) query = query.Where(o_c => GetQuarter(o_c.o.OrderPlaced.Value) == quarter.Value);
            if (year.HasValue) query = query.Where(o_c => o_c.o.OrderPlaced.Value.Year == year.Value);

            var value = query.Select(o_c => o_c.o.ShippingCost / o_c.o.ExchangeRate).Sum();

            return value;
        }

        public decimal GetShippingValueSumByDate(string inOrderStatusXml,
            string inLineStatusXml,
            bool? nonEuOnly,
            int? vat,
            int hourFlag,
            int dayFlag,
            int weekFlag,
            int monthFlag,
            int quarterFlag,
            int yearFlag,
            int hour,
            int day,
            int week,
            int month,
            int quarter,
            int year)
        {
            var pLineStatus = GetParameter("InLineStatusXml", inLineStatusXml);
            var pOrderStatus = GetParameter("InOrderStatusXml", inOrderStatusXml);
            var pNonEuOnly = GetParameter("NonEuOnly", nonEuOnly);
            var pHourFlag = GetParameter("HourFlag", hourFlag);
            var pDayFlag = GetParameter("DayFlag", dayFlag);
            var pWeekFlag = GetParameter("WeekFlag", weekFlag);
            var pMonthFlag = GetParameter("MonthFlag", monthFlag);
            var pQuarterFlag = GetParameter("QuarterFlag", quarterFlag);
            var pYearFlag = GetParameter("YearFlag", yearFlag);
            var pHour = GetParameter("Hour", hour);
            var pDay = GetParameter("Day", day);
            var pWeek = GetParameter("Week", week);
            var pMonth = GetParameter("Month", month);
            var pQuarter = GetParameter("Quarter", quarter);
            var pYear = GetParameter("Year", year);

            var returnValue = _dbContext.SqlQuery<decimal?>(@"EXEC ShippingValueSum_GetInfo 
                @InOrderStatusXml, 
                @HourFlag, 
                @DayFlag, 
                @WeekFlag, 
                @MonthFlag, 
                @QuarterFlag, 
                @YearFlag, 
                @Hour, 
                @Day, 
                @Week, 
                @Month, 
                @Quarter, 
                @Year, 
                @NonEuOnly",
                pOrderStatus, 
                pHourFlag, 
                pDayFlag, 
                pWeekFlag, 
                pMonthFlag, 
                pQuarterFlag, 
                pYearFlag, 
                pHour, 
                pDay, 
                pWeek, 
                pMonth, 
                pQuarter, 
                pYear, 
                pNonEuOnly)
                .Single();

            return returnValue.HasValue ? returnValue.Value : 0M;
        }

        public decimal GetMarginValueSumByDate(string inOrderStatusXml,
            string inLineStatusXml,
            bool? nonEuOnly,
            int? vat,
            int hourFlag,
            int dayFlag,
            int weekFlag,
            int monthFlag,
            int quarterFlag,
            int yearFlag,
            int hour,
            int day,
            int week,
            int month,
            int quarter,
            int year)
        {
            var Vat = GetParameter("VAT", vat);
            var LineStatus = GetParameter("InLineStatusXml", inLineStatusXml);
            var OrderStatus = GetParameter("InOrderStatusXml", inOrderStatusXml);
            var NonEuOnly = GetParameter("NonEuOnly", nonEuOnly);
            var HourFlag = GetParameter("HourFlag", hourFlag);
            var DayFlag = GetParameter("DayFlag", dayFlag);
            var WeekFlag = GetParameter("WeekFlag", weekFlag);
            var MonthFlag = GetParameter("MonthFlag", monthFlag);
            var QuarterFlag = GetParameter("QuarterFlag", quarterFlag);
            var YearFlag = GetParameter("YearFlag", yearFlag);
            var Hour = GetParameter("Hour", hour);
            var Day = GetParameter("Day", day);
            var Week = GetParameter("Week", week);
            var Month = GetParameter("Month", month);
            var Quarter = GetParameter("Quarter", quarter);
            var Year = GetParameter("Year", year);

            var returnValue = _dbContext.SqlQuery<decimal>("EXEC MarginValueSum_GetInfo @InLineStatusXml, @InOrderStatusXml, @HourFlag, @DayFlag, @WeekFlag, @MonthFlag, @QuarterFlag, @YearFlag, @Hour, @Day, @Week, @Month, @Quarter, @Year,@VAT, @NonEuOnly",
                LineStatus, OrderStatus, HourFlag, DayFlag, WeekFlag, MonthFlag, QuarterFlag, YearFlag, Hour, Day, Week, Month, Quarter, Year, Vat, NonEuOnly).Single();

            return returnValue;
        }

        public decimal GetLineCostSumByDate(string inOrderStatusXml,
            string inLineStatusXml,
            bool? nonEuOnly,
            int hourFlag,
            int dayFlag,
            int weekFlag,
            int monthFlag,
            int quarterFlag,
            int yearFlag,
            int hour,
            int day,
            int week,
            int month,
            int quarter,
            int year)
        {
            var OrderStatus = GetParameter("InOrderStatusXml", inOrderStatusXml);
            var LineStatus = GetParameter("InLineStatusXml", inLineStatusXml);
            var NonEuOnly = GetParameter("NonEuOnly", nonEuOnly);
            var HourFlag = GetParameter("HourFlag", hourFlag);
            var DayFlag = GetParameter("DayFlag", dayFlag);
            var WeekFlag = GetParameter("WeekFlag", weekFlag);
            var MonthFlag = GetParameter("MonthFlag", monthFlag);
            var QuarterFlag = GetParameter("QuarterFlag", quarterFlag);
            var YearFlag = GetParameter("YearFlag", yearFlag);
            var Hour = GetParameter("Hour", hour);
            var Day = GetParameter("Day", day);
            var Week = GetParameter("Week", week);
            var Month = GetParameter("Month", month);
            var Quarter = GetParameter("Quarter", quarter);
            var Year = GetParameter("Year", year);

            var returnValue = _dbContext.SqlQuery<decimal>("EXEC LineCostValueSum_GetInfo @InLineStatusXml, @InOrderStatusXml, @HourFlag, @DayFlag, @WeekFlag, @MonthFlag, @QuarterFlag, @YearFlag, @Hour, @Day, @Week, @Month, @Quarter, @Year, @NonEuOnly",
                                                        LineStatus, OrderStatus, HourFlag, DayFlag, WeekFlag, MonthFlag, QuarterFlag, YearFlag, Hour, Day, Week, Month, Quarter, Year, NonEuOnly).Single();

            return returnValue;
        }

        public decimal GetOrderLoyaltyDiscountSumByDate(string inOrderStatusXml,
            bool? nonEuOnly,
            int hourFlag,
            int dayFlag,
            int weekFlag,
            int monthFlag,
            int quarterFlag,
            int yearFlag,
            int hour,
            int day,
            int week,
            int month,
            int quarter,
            int year)
        {
            var pOrderStatus = GetParameter("InOrderStatusXml", inOrderStatusXml);
            var pNonEuOnly = GetParameter("NonEuOnly", nonEuOnly);
            var pHourFlag = GetParameter("HourFlag", hourFlag);
            var pDayFlag = GetParameter("DayFlag", dayFlag);
            var pWeekFlag = GetParameter("WeekFlag", weekFlag);
            var pMonthFlag = GetParameter("MonthFlag", monthFlag);
            var pQuarterFlag = GetParameter("QuarterFlag", quarterFlag);
            var pYearFlag = GetParameter("YearFlag", yearFlag);
            var pHour = GetParameter("Hour", hour);
            var pDay = GetParameter("Day", day);
            var pWeek = GetParameter("Week", week);
            var pMonth = GetParameter("Month", month);
            var pQuarter = GetParameter("Quarter", quarter);
            var pYear = GetParameter("Year", year);

            var discount = _dbContext.SqlQuery<decimal?>(@"EXEC LoyaltyDiscountSum_GetInfo 
                @InOrderStatusXml, 
                @HourFlag, 
                @DayFlag, 
                @WeekFlag, 
                @MonthFlag, 
                @QuarterFlag, 
                @YearFlag, 
                @Hour, 
                @Day, 
                @Week, 
                @Month, 
                @Quarter, 
                @Year,
                @NonEuOnly",
                pOrderStatus,
                pHourFlag,
                pDayFlag,
                pWeekFlag,
                pMonthFlag,
                pQuarterFlag,
                pYearFlag,
                pHour,
                pDay,
                pWeek,
                pMonth,
                pQuarter,
                pYear,
                pNonEuOnly)
                .Single();

            return discount.HasValue ? discount.Value : 0M;
        }

        public decimal GetOrderDiscountSumByDate(string inOrderStatusXml,
            bool? nonEuOnly,
            int hourFlag,
            int dayFlag,
            int weekFlag,
            int monthFlag,
            int quarterFlag,
            int yearFlag,
            int hour,
            int day,
            int week,
            int month,
            int quarter,
            int year)
        {
            var pOrderStatus = GetParameter("InOrderStatusXml", inOrderStatusXml);
            var pNonEuOnly = GetParameter("NonEuOnly", nonEuOnly);
            var pHourFlag = GetParameter("HourFlag", hourFlag);
            var pDayFlag = GetParameter("DayFlag", dayFlag);
            var pWeekFlag = GetParameter("WeekFlag", weekFlag);
            var pMonthFlag = GetParameter("MonthFlag", monthFlag);
            var pQuarterFlag = GetParameter("QuarterFlag", quarterFlag);
            var pYearFlag = GetParameter("YearFlag", yearFlag);
            var pHour = GetParameter("Hour", hour);
            var pDay = GetParameter("Day", day);
            var pWeek = GetParameter("Week", week);
            var pMonth = GetParameter("Month", month);
            var pQuarter = GetParameter("Quarter", quarter);
            var pYear = GetParameter("Year", year);

            var returnValue = _dbContext.SqlQuery<decimal?>(@"EXEC OrderDiscountSum_GetInfo 
                @InOrderStatusXml, 
                @HourFlag, 
                @DayFlag, 
                @WeekFlag, 
                @MonthFlag, 
                @QuarterFlag, 
                @YearFlag, 
                @Hour, 
                @Day, 
                @Week, 
                @Month, 
                @Quarter, 
                @Year, 
                @NonEuOnly",
                pOrderStatus, 
                pHourFlag, 
                pDayFlag, 
                pWeekFlag, 
                pMonthFlag, 
                pQuarterFlag, 
                pYearFlag, 
                pHour, 
                pDay, 
                pWeek, 
                pMonth, 
                pQuarter, 
                pYear, 
                pNonEuOnly)
                .Single();

            return returnValue.HasValue ? returnValue.Value : 0M;
        }

        public int GetOrderCountSumByDate(string inOrderStatusXml,
            bool? nonEuOnly,
            int hourFlag,
            int dayFlag,
            int weekFlag,
            int monthFlag,
            int quarterFlag,
            int yearFlag,
            int hour,
            int day,
            int week,
            int month,
            int quarter,
            int year)
        {
            var OrderStatus = GetParameter("InOrderStatusXml", inOrderStatusXml);
            var NonEuOnly = GetParameter("NonEuOnly", nonEuOnly);
            var HourFlag = GetParameter("HourFlag", hourFlag);
            var DayFlag = GetParameter("DayFlag", dayFlag);
            var WeekFlag = GetParameter("WeekFlag", weekFlag);
            var MonthFlag = GetParameter("MonthFlag", monthFlag);
            var QuarterFlag = GetParameter("QuarterFlag", quarterFlag);
            var YearFlag = GetParameter("YearFlag", yearFlag);
            var Hour = GetParameter("Hour", hour);
            var Day = GetParameter("Day", day);
            var Week = GetParameter("Week", week);
            var Month = GetParameter("Month", month);
            var Quarter = GetParameter("Quarter", quarter);
            var Year = GetParameter("Year", year);

            var count = _dbContext.SqlQuery<int>("EXEC OrderCount_GetInfo @InOrderStatusXml, @HourFlag, @DayFlag, @WeekFlag, @MonthFlag, @QuarterFlag, @YearFlag, @Hour, @Day, @Week, @Month, @Quarter, @Year, @NonEuOnly",
                                                     OrderStatus, HourFlag, DayFlag, WeekFlag, MonthFlag, QuarterFlag, YearFlag, Hour, Day, Week, Month, Quarter, Year, NonEuOnly).Single();

            return count;
        }

        public DataTable GetIncompleteOrders()
        {
            var dt = new DataTable("IncompleteOrders");
            dt.Columns.Add("Item", typeof(string));
            dt.Columns.Add("Count", typeof(int));

            var notYetShippedOrderStatus = new string[] { OrderStatusCode.ORDER_PLACED,
                                                          OrderStatusCode.STOCK_WARNING,
                                                          OrderStatusCode.AWAITING_REPLY,
                                                          OrderStatusCode.ON_HOLD };

            var notYetShippedOrderCount = _orderRepository.Table
                .Where(x => notYetShippedOrderStatus.Contains(x.StatusCode))
                .Count();

            var drNotYetShippedItem = dt.NewRow();
            drNotYetShippedItem["Item"] = "Total not yet shipped orders";
            drNotYetShippedItem["Count"] = notYetShippedOrderCount;
            dt.Rows.Add(drNotYetShippedItem);

            var stockAllocationOrderCount = _orderRepository.Table
                .Where(x => x.StatusCode == OrderStatusCode.STOCK_WARNING)
                .Count();

            var drStockAllocationItem = dt.NewRow();
            drStockAllocationItem["Item"] = "Total stock allocation orders";
            drStockAllocationItem["Count"] = stockAllocationOrderCount;
            dt.Rows.Add(drStockAllocationItem);

            var onHoldOrderCount = _orderRepository.Table
                .Where(x => x.StatusCode == OrderStatusCode.ON_HOLD)
                .Count();

            var drOnHoldItem = dt.NewRow();
            drOnHoldItem["Item"] = "Total on hold orders";
            drOnHoldItem["Count"] = onHoldOrderCount;
            dt.Rows.Add(drOnHoldItem);

            var awaitingReplyOrderCount = _orderRepository.Table
                .Where(x => x.StatusCode == OrderStatusCode.AWAITING_REPLY)
                .Count();

            var drAwaitingReplyItem = dt.NewRow();
            drAwaitingReplyItem["Item"] = "Total awaiting reply orders";
            drAwaitingReplyItem["Count"] = awaitingReplyOrderCount;
            dt.Rows.Add(drAwaitingReplyItem);

            return dt;
        }
        
        public IList<spReport_GetRegistrations_Result> GetTotalRegisteredGroupByDay(string fromDate, string toDate)
        {
            object FromDate = GetParameter("DateFrom", fromDate);
            object ToDate = GetParameter("DateTo", toDate);
            var result = _dbContext.SqlQuery<spReport_GetRegistrations_Result>("EXEC Report_GetRegistrations @DateFrom, @DateTo", FromDate, ToDate).ToList();

            return result;
        }
        
        public IList<spRevenue_currentmonth_Result> GetRevenueReportForCurrentMonth()
        {
            var data = _dbContext.SqlQuery<spRevenue_currentmonth_Result>("EXEC Revenue_currentmonth").ToList();
            return data;
        }

        public IList<spRevenue_lastweek_Result> GetRevenueReportForLastWeek()
        {
            var data = _dbContext.SqlQuery<spRevenue_lastweek_Result>("EXEC Revenue_lastweek").ToList();
            return data;
        }

        public IList<spRevenue_lastday_Result> GetRevenueReportForLastDay()
        {
            var data = _dbContext.SqlQuery<spRevenue_lastday_Result>("EXEC Revenue_lastday").ToList();
            return data;
        }

        public IList<spOrderslastday_graph_Result> GetOrderCountLastDayForGraph()
        {
            var data = _dbContext.SqlQuery<spOrderslastday_graph_Result>("EXEC Orderslastday_graph").ToList();
            return data;
        }

        public IList<spOrderslastweek_graph_Result> GetOrderCountLastWeekForGraph()
        {
            var data = _dbContext.SqlQuery<spOrderslastweek_graph_Result>("EXEC Orderslastweek_graph").ToList();
            return data;
        }

        public IList<spOrderslastmonth_graph_Result> GetOrderCountLastMonthForGraph()
        {
            var data = _dbContext.SqlQuery<spOrderslastmonth_graph_Result>("EXEC Orderslastmonth_graph").ToList();
            return data;
        }

        public IList<spLastfive_Result> GetLastFiveTerms()
        {
            var data = _dbContext.SqlQuery<spLastfive_Result>("EXEC Lastfive").ToList();
            return data;
        }

        public IList<spRegistered_Customers_Result> GetRegisteredCustomersReport()
        {
            var data = _dbContext.SqlQuery<spRegistered_Customers_Result>("EXEC Registered_Customers").ToList();
            return data;
        }
        
        public IList<spBestsellers_Dashboard_Result> GetBestsellerForDashboardByType(string type)
        {
            var Type = GetParameter("Type", type);
            var result = _dbContext.SqlQuery<spBestsellers_Dashboard_Result>("EXEC Bestsellers_Dashboard @Type", Type).ToList();

            return result;
        }

        public PagedList<ProductAnalysis> GetProductAnalysis(
            int pageIndex = 0, 
            int pageSize = 2147483647,
            IList<int> productIds = null,
            string name = null,
            string fromDateStamp = null,
            string toDateStamp = null)
        {
            string[] orderStatus = new string[] {
                OrderStatusCode.SHIPPING,
                OrderStatusCode.STOCK_WARNING,
                OrderStatusCode.INVOICED,
                OrderStatusCode.ON_HOLD,
                OrderStatusCode.CANCELLED,
                OrderStatusCode.SCHEDULED_FOR_CANCEL};

            string[] lineStatus = new string[]
            {
                LineStatusCode.ORDERED,
                LineStatusCode.GOODS_ALLOCATED,
                LineStatusCode.PARTIAL_SHIPPING,
                LineStatusCode.PICK_IN_PROGRESS,
                LineStatusCode.STOCK_WARNING,
                LineStatusCode.DESPATCHED,
                LineStatusCode.AWAITING_STOCK
            };

            var query = _lineItemRepository.Table
                        .Join(_orderRepository.Table, l => l.OrderId, o => o.Id, (l, o) => new { l, o })
                        .Where(x => orderStatus.Contains(x.o.StatusCode));
                        
            if (productIds != null && productIds.Count > 0)
                query = query.Where(x => productIds.Contains(x.l.ProductId));

            if (!string.IsNullOrEmpty(name))
                query = query.Where(x => x.l.Name.Contains(name));

            if (!string.IsNullOrEmpty(fromDateStamp)
                && DateTime.TryParseExact(fromDateStamp, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fromDate))
            {
                query = query.Where(x => fromDate <= x.o.OrderPlaced);
            }

            if (!string.IsNullOrEmpty(toDateStamp)
                && DateTime.TryParseExact(toDateStamp, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime toDate))
            {
                toDate = toDate.AddDays(1).AddMilliseconds(-1);
                query = query.Where(x => toDate >= x.o.OrderPlaced);
            }

            var results = query.GroupBy(
                            x => new { x.l.ProductId, x.l.Name, x.l.StatusCode },
                            x => x.l.Quantity,
                            (x, y) => new ProductAnalysis
                            {
                                ProductId = x.ProductId,
                                Name = x.Name,
                                Sold = lineStatus.Contains(x.StatusCode) ? y.Sum(q => q) : 0,
                                Cancelled = !lineStatus.Contains(x.StatusCode) ? y.Sum(q => q) : 0
                            });
            
            var results2 = results.GroupBy(
                x => new { x.ProductId, x.Name },
                x => new { x.Sold, x.Cancelled },
                (x, y) => new ProductAnalysis
                {
                    ProductId = x.ProductId,
                    Name = x.Name,
                    Sold = y.Sum(q => q.Sold),
                    Cancelled = y.Sum(q => q.Cancelled)
                });

            int totalRecords = results2.Count();

            results2 = results2.OrderBy(x => x.Name);

            var items = results2.Skip(pageIndex * pageSize).Take(pageSize).ToList();

            foreach (var item in items)
            {
                var itemQuery = _lineItemRepository.Table
                    .Join(_orderRepository.Table, l => l.OrderId, o => o.Id, (l, o) => new { l, o })
                    .Join(_countryRepository.Table, l_o => l_o.o.ShippingCountryId, c => c.Id, (l_o, c) => new { l_o.l, l_o.o, c })
                    .Where(x => x.l.ProductId == item.ProductId)
                    .Where(x => orderStatus.Contains(x.o.StatusCode));

                if (!string.IsNullOrEmpty(fromDateStamp)
                    && DateTime.TryParseExact(fromDateStamp, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fromOrderDate))
                {
                    itemQuery = itemQuery.Where(x => fromOrderDate <= x.o.OrderPlaced);
                }

                if (!string.IsNullOrEmpty(toDateStamp)
                    && DateTime.TryParseExact(toDateStamp, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime toOrderDate))
                {
                    toDate = toOrderDate.AddDays(1).AddMilliseconds(-1);
                    itemQuery = itemQuery.Where(x => toDate >= x.o.OrderPlaced);
                }

                var groups = itemQuery.GroupBy(
                    x => new { x.o.ShippingCountryId, x.c.ISO3166Code },
                    x => x.l.Quantity,
                    (x, y) => new { countryCode = x.ISO3166Code, quantity = y.Sum(q => q) })
                    .OrderByDescending(x => x.quantity)
                    .ToList();
                
                foreach (var group in groups)
                {
                    item.TopCountries.Add(group.countryCode, group.quantity);
                }

                var stock = _productPriceRepository.Table
                    .Where(x => x.ProductId == item.ProductId)
                    .Where(x => x.Enabled == true)
                    .Select(x => x.Stock)
                    .DefaultIfEmpty()
                    .Sum();

                item.CurrentStock = stock;
            }
            
            return new PagedList<ProductAnalysis>(items, pageIndex, pageSize, totalRecords);
        }

        private int WeekOfYearISO8601(DateTime date)
        {
            var day = (int)CultureInfo.CurrentCulture.Calendar.GetDayOfWeek(date);
            return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(date.AddDays(4 - (day == 0 ? 7 : day)), CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        private int GetQuarter(DateTime date)
        {
            if (date.Month >= 1 && date.Month <= 3)
                return 1;
            else if (date.Month >= 4 && date.Month <= 6)
                return 2;
            else if (date.Month >= 7 && date.Month <= 9)
                return 3;
            else
                return 4;

        }
    }
}
