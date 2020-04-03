using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Domain.Directory;
using Apollo.Core.Model;
using Apollo.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.Report
{
    public partial class report_orders : BasePage, ICallbackEventHandler
    {
        private const string SHOW_BY = "showBy";
        
        public IReportService ReportService { get; set; }
        public AdminStoreUtility AdminStoreUtility { get; set; }
        public CurrencySettings CurrencySettings { get; set; }

        //[PrincipalPermission(SecurityAction.Demand, Role = "Super Administrator")]
        protected void Page_Load()
        {
            if (!Page.IsPostBack)
            {
                txtDateTo.Text = DateTime.Today.ToString(AppConstant.DATE_FORM1);
                txtDateFrom.Text = DateTime.Today.AddDays(-6).ToString(AppConstant.DATE_FORM1);
            }

            LoadOrderReport();
        }

        protected void LoadOrderReport()
        {
            gvOrdersByHour.Visible = false;
            gvOrdersByDay.Visible = false;
            gvOrdersByWeek.Visible = false;
            gvOrdersByMonth.Visible = false;
            gvOrdersByQuarter.Visible = false;
            gvOrdersByYear.Visible = false;

            string sorting = ddlShowBy.SelectedValue;

            // Calculate days
            DateTime dtFrom;
            DateTime dtTo;

            if (txtDateFrom.Text.Trim().Length != 0 && txtDateTo.Text.Trim().Length != 0)
            {
                dtFrom = DateTime.ParseExact(txtDateFrom.Text, AppConstant.DATE_FORM1, CultureInfo.InvariantCulture);
                dtTo = DateTime.ParseExact(txtDateTo.Text, AppConstant.DATE_FORM1, CultureInfo.InvariantCulture);
            }
            else
            {
                txtDateTo.Text = DateTime.Today.ToString(AppConstant.DATE_FORM1);
                txtDateFrom.Text = DateTime.Today.AddDays(-6).ToString(AppConstant.DATE_FORM1);
                dtFrom = DateTime.ParseExact(txtDateFrom.Text, AppConstant.DATE_FORM1, CultureInfo.InvariantCulture);
                dtTo = DateTime.ParseExact(txtDateTo.Text, AppConstant.DATE_FORM1, CultureInfo.InvariantCulture);
            }
            DataTable dt;

            int beginMonth;
            int endMonth;
            int pointerMonth;
            int pointerYear;
            int beginWeek;
            int endWeek;
            int beginYear;
            int endYear;
            int pointerWeek;

            TimeSpan span;
            int totalDays;

            switch (sorting)
            {
                case "hour":
                    dt = new DataTable();
                    dt.Columns.Add("Hour");
                    dt.Columns.Add("Day");
                    dt.Columns.Add("Month");
                    dt.Columns.Add("Year");

                    span = dtTo - dtFrom;

                    totalDays = span.Days;

                    for (int i = totalDays; i >= 0; i--)
                    {
                        for (int j = 0; j < 24; j++)
                        {
                            DataRow dr = dt.NewRow();
                            dr["Hour"] = j;

                            DateTime temp = dtFrom.AddDays((double)(i));

                            dr["Day"] = temp.Day;
                            dr["Month"] = temp.Month;
                            dr["Year"] = temp.Year;

                            dt.Rows.Add(dr);
                        }
                    }

                    gvOrdersByHour.Visible = true;
                    gvOrdersByHour.DataSource = dt;
                    gvOrdersByHour.DataBind();

                    break;

                case "day":
                    List<DateTime> dates = new List<DateTime>();
                    span = dtTo - dtFrom;

                    totalDays = span.Days;

                    for (int i = totalDays; i >= 0; i--)
                    {
                        dates.Add(dtFrom.AddDays((double)(i)));
                    }

                    gvOrdersByDay.Visible = true;
                    gvOrdersByDay.DataSource = dates;
                    gvOrdersByDay.DataBind();
                    break;

                case "week":
                    CultureInfo culture = CultureInfo.CurrentCulture;

                    dt = new DataTable();
                    dt.Columns.Add("Week");
                    dt.Columns.Add("Year");

                    beginWeek = culture.Calendar.GetWeekOfYear(dtFrom, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
                    endWeek = culture.Calendar.GetWeekOfYear(dtTo, CalendarWeekRule.FirstDay, DayOfWeek.Monday);

                    beginMonth = dtFrom.Month;
                    endMonth = dtTo.Month;

                    beginYear = dtFrom.Year;
                    endYear = dtTo.Year;

                    pointerWeek = beginWeek;
                    pointerMonth = beginMonth;
                    pointerYear = dtFrom.Year;

                    int endWeekTemp = endYear > beginYear ? 52 : endWeek;

                    for (int i = pointerYear; i <= endYear; i++)
                    {
                        for (int j = pointerWeek; j <= endWeekTemp; j++)
                        {
                            DataRow dr = dt.NewRow();
                            dr["Week"] = j;
                            dr["Year"] = i;
                            dt.Rows.Add(dr);
                        }

                        endWeekTemp = (i + 1) < endYear ? 52 : endWeek;
                        pointerWeek = 1;
                    }

                    gvOrdersByWeek.Visible = true;
                    gvOrdersByWeek.RowDataBound += new GridViewRowEventHandler(gvOrdersByWeek_RowDataBound);
                    gvOrdersByWeek.DataSource = dt;
                    gvOrdersByWeek.DataBind();

                    break;

                case "month":
                    dt = new DataTable();
                    dt.Columns.Add("Month");
                    dt.Columns.Add("Year");

                    beginMonth = dtFrom.Month;
                    endMonth = dtTo.Month;
                    endYear = dtTo.Year;
                    pointerMonth = beginMonth;
                    pointerYear = dtFrom.Year;

                    while (pointerYear <= endYear)
                    {
                        if (pointerMonth <= 12)
                        {
                            DataRow dr = dt.NewRow();
                            dr["Month"] = pointerMonth;
                            dr["Year"] = pointerYear;

                            dt.Rows.Add(dr);

                            pointerMonth++;
                        }
                        else
                        {
                            pointerYear++;
                            pointerMonth = 1;
                        }

                        if (pointerMonth > endMonth && pointerYear == endYear)
                            break;
                    }

                    gvOrdersByMonth.Visible = true;
                    gvOrdersByMonth.DataSource = dt;
                    gvOrdersByMonth.DataBind();

                    break;

                case "quarter":
                    dt = new DataTable();
                    dt.Columns.Add("Quarter");
                    dt.Columns.Add("Year");

                    int beginQuarter = AdminStoreUtility.GetQuarter(dtFrom);
                    int endQuarter = AdminStoreUtility.GetQuarter(dtTo);

                    endYear = dtTo.Year;
                    int pointerQuarter = beginQuarter;
                    pointerYear = dtFrom.Year;

                    while (pointerYear <= endYear)
                    {
                        if (pointerQuarter <= 4)
                        {
                            DataRow dr = dt.NewRow();
                            dr["Quarter"] = pointerQuarter;
                            dr["Year"] = pointerYear;

                            dt.Rows.Add(dr);

                            pointerQuarter++;
                        }
                        else
                        {
                            pointerYear++;
                            pointerQuarter = 1;
                        }

                        if (pointerQuarter > endQuarter && pointerYear == endYear)
                            break;
                    }

                    gvOrdersByQuarter.Visible = true;
                    gvOrdersByQuarter.DataSource = dt;
                    gvOrdersByQuarter.DataBind();

                    break;

                case "year":
                    dt = new DataTable();
                    dt.Columns.Add("Year");

                    beginYear = dtFrom.Year;
                    endYear = dtTo.Year;
                    pointerYear = beginYear;

                    while (pointerYear <= endYear)
                    {
                        DataRow dr = dt.NewRow();
                        dr["Year"] = pointerYear;

                        dt.Rows.Add(dr);

                        pointerYear++;

                        if (pointerYear > endYear)
                            break;
                    }

                    gvOrdersByYear.Visible = true;
                    gvOrdersByYear.DataSource = dt;
                    gvOrdersByYear.DataBind();

                    break;
            }
        }

        void gvOrdersByWeek_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //DataRowView data = (DataRowView)e.Row.DataItem;
                //string test = data[0].ToString();
                //if (test.Equals("1"))
                //    e.Row.ControlStyle.CssClass = "firstWeek";
            }
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadOrderReport();
        }

        #region ICallbackEventHandler Members

        private string message;
        private const char splitter = '_';

        string ICallbackEventHandler.GetCallbackResult()
        {
            return message;
        }

        void ICallbackEventHandler.RaiseCallbackEvent(string eventArgument)
        {
            string[] args = eventArgument.Split(splitter);

            int hourFlag = 0;
            int dayFlag = 0;
            int weekFlag = 0;
            int monthFlag = 0;
            int quarterFlag= 0;
            int yearFlag = 0;
            int hour;
            int day;
            int week;
            int month;
            int quarter;
            int year;

            hour = Convert.ToInt32(args[1]);
            day = Convert.ToInt32(args[2]);
            week = Convert.ToInt32(args[3]);
            month = Convert.ToInt32(args[4]);
            quarter = Convert.ToInt32(args[5]);
            year = Convert.ToInt32(args[6]);

            string typeFlag = args[8];

            switch (typeFlag)
            {
                case "hour":
                    weekFlag = 1;
                    quarterFlag = 1;
                    break;
                case "day":
                    hourFlag = 1;
                    weekFlag = 1;
                    quarterFlag = 1;
                    break;
                case "week":
                    hourFlag = 1;
                    dayFlag = 1;
                    monthFlag = 1;
                    quarterFlag = 1;
                    break;
                case "month":
                    hourFlag = 1;
                    dayFlag = 1;
                    weekFlag = 1;
                    quarterFlag = 1;
                    break;
                case "quarter":
                    hourFlag = 1;
                    dayFlag = 1;
                    weekFlag = 1;
                    monthFlag = 1;
                    break;
                case "year":
                    hourFlag = 1;
                    dayFlag = 1;
                    weekFlag = 1;
                    monthFlag = 1;
                    quarterFlag = 1;
                    break;
            }

            bool noneu = Convert.ToBoolean(args[7]);
            int count = 0;

            switch (args[0])
            {
                case "noOrders":
                    count = ReportService.GetOrderCountSumByDate(AdminStoreUtility.BuildXmlString("status", ValidOrderStatus.VALID_STATUSES),
                                                              noneu,
                                                              hourFlag,
                                                              dayFlag,
                                                              weekFlag,
                                                              monthFlag,
                                                              quarterFlag,
                                                              yearFlag,
                                                              hour,
                                                              day,
                                                              week,
                                                              month,
                                                              quarter,
                                                              year);
                    message = count.ToString();

                    break;

                case "promoDiscount":
                    decimal promoDiscount = ReportService.GetOrderDiscountSumByDate(
                        AdminStoreUtility.BuildXmlString(AppConstant.XML_ROOT_STATUS, ValidOrderStatus.VALID_STATUSES),                                                                                          
                        noneu,
                        hourFlag,
                        dayFlag,
                        weekFlag,
                        monthFlag,
                        quarterFlag,
                        yearFlag,
                        hour,
                        day,
                        week,
                        month,
                        quarter,
                        year);
                    message = AdminStoreUtility.GetFormattedPrice(promoDiscount,
                                                                  CurrencySettings.PrimaryStoreCurrencyCode, 
                                                                  CurrencyType.HtmlEntity);
                    break;

                case "loyaltyDiscount":
                    decimal loyaltyDiscount = ReportService.GetOrderLoyaltyDiscountSumByDate(AdminStoreUtility.BuildXmlString(AppConstant.XML_ROOT_STATUS, ValidOrderStatus.VALID_STATUSES),
                                                                               noneu,
                                                                               hourFlag,
                                                                               dayFlag,
                                                                               weekFlag,
                                                                               monthFlag,
                                                                               quarterFlag,
                                                                               yearFlag,
                                                                               hour,
                                                                               day,
                                                                               week,
                                                                               month,
                                                                               quarter,
                                                                               year);
                    message = AdminStoreUtility.GetFormattedPrice(loyaltyDiscount,
                                                                  CurrencySettings.PrimaryStoreCurrencyCode, 
                                                                  CurrencyType.HtmlEntity);
                    break;

                case "lineCostPrice":
                    decimal lineCostPrice = ReportService.GetLineCostSumByDate(AdminStoreUtility.BuildXmlString(AppConstant.XML_ROOT_STATUS, ValidOrderStatus.VALID_STATUSES), 
                                                                           AdminStoreUtility.BuildXmlString(AppConstant.XML_ROOT_STATUS, ValidLineStatus.VALID_LINE_STATUSES),
                                                                           noneu,
                                                                           hourFlag,
                                                                           dayFlag,
                                                                           weekFlag,
                                                                           monthFlag,
                                                                           quarterFlag,
                                                                           yearFlag,
                                                                           hour,
                                                                           day,
                                                                           week,
                                                                           month,
                                                                           quarter,
                                                                           year);
                    message = AdminStoreUtility.GetFormattedPrice(lineCostPrice,
                                                                  CurrencySettings.PrimaryStoreCurrencyCode, 
                                                                  CurrencyType.HtmlEntity);
                    break;

                case "margin":
                    decimal margin = ReportService.GetMarginValueSumByDate(AdminStoreUtility.BuildXmlString(AppConstant.XML_ROOT_STATUS, ValidOrderStatus.VALID_STATUSES),
                                                                  AdminStoreUtility.BuildXmlString(AppConstant.XML_ROOT_STATUS, ValidLineStatus.VALID_LINE_STATUSES),
                                                                  noneu,
                                                                  20,
                                                                  hourFlag,
                                                                  dayFlag,
                                                                  weekFlag,
                                                                  monthFlag,
                                                                  quarterFlag,
                                                                  yearFlag,
                                                                  hour,
                                                                  day,
                                                                  week,
                                                                  month,
                                                                  quarter,
                                                                  year);
                    message = margin.ToString() + "%";
                    break;

                case "shipping":
                    decimal shipping = ReportService.GetShippingValueSumByDate(
                        AdminStoreUtility.BuildXmlString(AppConstant.XML_ROOT_STATUS, ValidOrderStatus.VALID_STATUSES),
                        AdminStoreUtility.BuildXmlString(AppConstant.XML_ROOT_STATUS, ValidLineStatus.VALID_LINE_STATUSES),
                        noneu, 
                        20,
                        hourFlag,
                        dayFlag,
                        weekFlag,
                        monthFlag,
                        quarterFlag,
                        yearFlag,
                        hour,
                        day,
                        week,
                        month,
                        quarter,
                        year);
                    message = AdminStoreUtility.GetFormattedPrice(shipping,
                                                                  CurrencySettings.PrimaryStoreCurrencyCode, 
                                                                  CurrencyType.HtmlEntity);
                    break;

                case "orderValue":
                    decimal orderValue = ReportService.GetOrderValueSumByDate(AdminStoreUtility.BuildXmlString(AppConstant.XML_ROOT_STATUS, ValidOrderStatus.VALID_STATUSES),
                                                                    AdminStoreUtility.BuildXmlString(AppConstant.XML_ROOT_STATUS, ValidLineStatus.VALID_LINE_STATUSES),                                                                    
                                                                    noneu,
                                                                    20,
                                                                    hourFlag,
                                                                    dayFlag,
                                                                    weekFlag,
                                                                    monthFlag,
                                                                    quarterFlag,
                                                                    yearFlag,
                                                                    hour,
                                                                    day,
                                                                    week,
                                                                    month,
                                                                    quarter,
                                                                    year);

                    message = AdminStoreUtility.GetFormattedPrice(orderValue, CurrencySettings.PrimaryStoreCurrencyCode, CurrencyType.HtmlEntity);

                    decimal nonTaxableOrderValue = ReportService.GetNonTaxableOrderValueSumByDate(AdminStoreUtility.BuildXmlString(AppConstant.XML_ROOT_STATUS, ValidOrderStatus.VALID_STATUSES),
                                                                                        AdminStoreUtility.BuildXmlString(AppConstant.XML_ROOT_STATUS, ValidLineStatus.VALID_LINE_STATUSES),
                                                                                        noneu,
                                                                                        20, 
                                                                                        hourFlag,
                                                                                        dayFlag,
                                                                                        weekFlag,
                                                                                        monthFlag,
                                                                                        quarterFlag,
                                                                                        yearFlag,
                                                                                        hour,
                                                                                        day,
                                                                                        week,
                                                                                        month,
                                                                                        quarter,
                                                                                        year);

                    decimal vatDiscount = nonTaxableOrderValue / (120M) * 20M;

                    decimal netOrderValue = AdminStoreUtility.RoundPrice(orderValue) - AdminStoreUtility.RoundPrice(vatDiscount);

                    message = AdminStoreUtility.GetFormattedPrice(netOrderValue, CurrencySettings.PrimaryStoreCurrencyCode, CurrencyType.HtmlEntity);

                    break;

                case "productDiscount":
                    decimal productDiscount = ReportService.GetProductDiscountSumByDate(
                        AdminStoreUtility.BuildXmlString(AppConstant.XML_ROOT_STATUS, ValidOrderStatus.VALID_STATUSES),
                        AdminStoreUtility.BuildXmlString(AppConstant.XML_ROOT_STATUS, ValidLineStatus.VALID_LINE_STATUSES),
                        noneu,
                        hourFlag,
                        dayFlag,
                        weekFlag,
                        monthFlag,
                        quarterFlag,
                        yearFlag,
                        hour,
                        day,
                        week,
                        month,
                        quarter,
                        year);
                    message = AdminStoreUtility.GetFormattedPrice(productDiscount,
                                                                  CurrencySettings.PrimaryStoreCurrencyCode,
                                                                  CurrencyType.HtmlEntity);
                    break;

                case "orderAverageValue":
                    decimal totalOrderValue = ReportService.GetOrderValueSumByDate(AdminStoreUtility.BuildXmlString(AppConstant.XML_ROOT_STATUS, ValidOrderStatus.VALID_STATUSES),
                                                                         AdminStoreUtility.BuildXmlString(AppConstant.XML_ROOT_STATUS, ValidLineStatus.VALID_LINE_STATUSES),                                                                         
                                                                         noneu,
                                                                         20,
                                                                         hourFlag,
                                                                         dayFlag,
                                                                         weekFlag,
                                                                         monthFlag,
                                                                         quarterFlag,
                                                                         yearFlag,
                                                                         hour,
                                                                         day,
                                                                         week,
                                                                         month,
                                                                         quarter,
                                                                         year);

                    decimal totalNonTaxableOrderValue = ReportService.GetNonTaxableOrderValueSumByDate(AdminStoreUtility.BuildXmlString(AppConstant.XML_ROOT_STATUS, ValidOrderStatus.VALID_STATUSES),
                                                                                             AdminStoreUtility.BuildXmlString(AppConstant.XML_ROOT_STATUS, ValidLineStatus.VALID_LINE_STATUSES),
                                                                                             noneu,
                                                                                             20,
                                                                                             hourFlag,
                                                                                             dayFlag,
                                                                                             weekFlag,
                                                                                             monthFlag,
                                                                                             quarterFlag,
                                                                                             yearFlag,
                                                                                             hour,
                                                                                             day,
                                                                                             week,
                                                                                             month,
                                                                                             quarter,
                                                                                             year);

                    decimal totalVatDiscount = totalNonTaxableOrderValue / (120M) * 20M;

                    decimal totalNetOrderValue = AdminStoreUtility.RoundPrice(totalOrderValue) - AdminStoreUtility.RoundPrice(totalVatDiscount);

                    int totalOrder = ReportService.GetOrderCountSumByDate(AdminStoreUtility.BuildXmlString("status", ValidOrderStatus.VALID_STATUSES),
                                                                          noneu,
                                                                          hourFlag,
                                                                          dayFlag,
                                                                          weekFlag,
                                                                          monthFlag,
                                                                          quarterFlag,
                                                                          yearFlag,
                                                                          hour,
                                                                          day,
                                                                          week,
                                                                          month,
                                                                          quarter,
                                                                          year);

                    if (totalNetOrderValue != 0M && totalOrder != 0)
                    {
                        message = AdminStoreUtility.GetFormattedPrice(totalNetOrderValue / totalOrder, CurrencySettings.PrimaryStoreCurrencyCode, CurrencyType.HtmlEntity);
                    }
                    else
                    {
                        message = AdminStoreUtility.GetFormattedPrice(0M, CurrencySettings.PrimaryStoreCurrencyCode, CurrencyType.HtmlEntity);
                    }

                    break;
            }
        }

        #endregion
    }
}