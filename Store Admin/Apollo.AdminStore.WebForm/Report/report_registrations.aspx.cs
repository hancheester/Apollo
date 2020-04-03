using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using System;
using System.Collections.Generic;

namespace Apollo.AdminStore.WebForm.Report
{
    public partial class report_registrations : BasePage
    {
        public IReportService ReportService { get; set; }

        protected void Page_Load()
        {
            if (!Page.IsPostBack)
            {
                txtDateTo.Text = DateTime.Today.ToString(AppConstant.DATE_FORM1);
                txtDateFrom.Text = DateTime.Today.AddDays(-7).ToString(AppConstant.DATE_FORM1);
            }
            LoadOrderReport();
        }

        protected void LoadOrderReport()
        {
            string fromDate = txtDateFrom.Text.Trim().Length != 0 ? DateTime.ParseExact(txtDateFrom.Text, "dd/MM/yyyy", null).ToString(AppConstant.DATE_FORM1) : string.Empty;
            string toDate = txtDateTo.Text.Trim().Length != 0 ? DateTime.ParseExact(txtDateTo.Text, "dd/MM/yyyy", null).AddDays(1).ToString(AppConstant.DATE_FORM1) : string.Empty;

            IList<spReport_GetRegistrations_Result> rows = new List<spReport_GetRegistrations_Result>();

            if (fromDate != string.Empty && toDate != string.Empty)
            {
                gvOrdersByDay.DataSource = ReportService.GetTotalRegisteredGroupByDay(fromDate, toDate);
                gvOrdersByDay.DataBind();
            }

            double totalOrders = 0;
            double totalRegistered = 0;
            foreach (var r in rows)
            {
                try
                {
                    totalOrders += Convert.ToDouble(r.TotalOrders);
                }
                catch
                {
                    //r["TotalOrders"] = 0;
                }

                totalRegistered += Convert.ToDouble(r.TotalRegistered);
            }

            ltlTotalOrders.Text = totalOrders.ToString();
            ltlTotalRegistered.Text = totalRegistered.ToString();
            try
            {
                ltlRegisterOrder.Text = Convert.ToInt32((totalOrders / totalRegistered) * 100).ToString() + "%";
            }
            catch
            {
                ltlRegisterOrder.Text = "0%";
            }            
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadOrderReport();
        }
    }
}