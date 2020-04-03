using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model;
using Apollo.Core.Services.Interfaces;
using System;
using System.Collections.Generic;

namespace Apollo.AdminStore.WebForm.Sales
{
    public partial class order_pharm_form : BasePage
    {
        private const double BACKDATED_IN_DAYS = -60D;

        public IOrderService OrderService { get; set; }
        public IAccountService AccountService { get; set; }

        protected override void OnInit(EventArgs e)
        {
            var orderId = QueryOrderId;

            if (orderId <= 0)
                Response.Redirect("/sales/order_default.aspx");
            else
            {
                ltlTitle.Text = string.Format("<h3>Order # {0}</h3>", orderId.ToString());
                LoadPharmForm(orderId);
            }

            base.OnInit(e);
        }

        private void LoadPharmForm(int orderId)
        {
            var pharm = OrderService.GetPharmOrderByOrderId(orderId);

            ltlNotFound.Visible = false;
            //phPharmForm.Visible = false;
            phPharmOrder.Visible = false;

            if (pharm == null)
            {
                ltlNotFound.Visible = true;
                return;
            }

            phPharmOrder.Visible = true;

            ltTakenOwner.Text = pharm.TakenByOwner ? "Yes" : "No";

            phAllergy.Visible = !string.IsNullOrEmpty(pharm.Allergy);
            ltlAllergy.Text = pharm.Allergy;

            phOwnerAge.Visible = !string.IsNullOrEmpty(pharm.OwnerAge);
            ltlOwnerAge.Text = pharm.OwnerAge;

            ltHasOtherMed.Text = pharm.HasOtherCondMed ? "Yes" : "No";

            if (pharm.HasOtherCondMed)
            {
                phOwnerOtherCond.Visible = true;
                ltlOwnerOtherCond.Text = pharm.OtherCondMed;
            }
            else
            {
                phOwnerOtherCond.Visible = false;
            }

            rptPharmItem.DataSource = pharm.Items;
            rptPharmItem.DataBind();

            List<string> orderStatus = new List<string>(ValidOrderStatus.VALID_STATUSES);
            orderStatus.Add(OrderStatusCode.AWAITING_REPLY);

            var profileId = OrderService.GetProfileIdByOrderId(pharm.OrderId);

            var account = AccountService.GetAccountOverviewModelByProfileId(profileId);

            if (account != null)
            {
                ltName.Text = account.Name;
                ltEmail.Text = account.Email;
                ltContact.Text = string.IsNullOrEmpty(account.ContactNumber) ? string.Empty : account.ContactNumber;
            }

            // Get pharmaceutical lines from the last 60 days
            var items = OrderService.GetLastNDaysPharmaceauticalLines(
                profileId,
                DateTime.Now.AddDays(BACKDATED_IN_DAYS),
                DateTime.Now,
                ValidLineStatus.VALID_LINE_STATUSES,
                orderStatus.ToArray());

            if (items.Count > 0)
            {
                rptExistingItems.DataSource = items;
                rptExistingItems.DataBind();
            }
            else
            {
                phItemsFromThisClient.Visible = false;
            }
            
            // Get pharmaceutical lines from the last 60 days from this address
            var addressModel = OrderService.GetShippingAddressViewModelByOrderId(pharm.OrderId);

            if (addressModel != null)
            {
                var itemsFromPostCode = OrderService.GetLastNDaysPharmaceauticalLinesFromThisAddress(
                    addressModel.AddressLine1,
                    addressModel.PostCode,
                    DateTime.Now.AddDays(BACKDATED_IN_DAYS),
                    DateTime.Now,
                    ValidLineStatus.VALID_LINE_STATUSES,
                    orderStatus.ToArray());

                if (itemsFromPostCode.Count > 0)
                {
                    rptExistingItemsFromThisPostcode.DataSource = itemsFromPostCode;
                    rptExistingItemsFromThisPostcode.DataBind();
                }
                else
                {
                    phItemsFromThisPostCode.Visible = false;
                }
            }
            else
            {
                phItemsFromThisPostCode.Visible = false;
            }
        }
    }
}