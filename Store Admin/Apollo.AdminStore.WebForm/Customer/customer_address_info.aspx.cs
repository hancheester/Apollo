using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using System;
using System.Text;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.Customer
{
    public partial class customer_address_info : BasePage
    {
        public IAccountService AccountService { get; set; }
        public IShippingService ShippingService { get; set; }

        protected override void OnInit(EventArgs e)
        {
            int profileId = QueryUserId;

            if (profileId <= 0)
                Response.Redirect("/customer/customer_default.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.AccountIdInvalid);

            Account account = AccountService.GetAccountByProfileId(profileId);

            if (account != null)
                LoadUserInfo(account);
            else
                Response.Redirect("/customer/customer_default.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.AccountNotFound);

            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            enbNotice.Message = string.Empty;

            if (rptAddress.Items.Count <= 0)
                rptAddress.Visible = false;
            else
                rptAddress.Visible = true;
        }

        protected void rptAddress_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                int userId = QueryUserId;
                var address = e.Item.DataItem as Address;
                Literal ltl = e.Item.FindControl("ltlAddress") as Literal;

                StringBuilder sb = new StringBuilder();
                
                if (!string.IsNullOrEmpty(address.Name))
                {
                    sb.Append(address.Name).Append(HtmlElement.BR);
                }
                if (!string.IsNullOrEmpty(address.AddressLine1))
                {
                    sb.Append(address.AddressLine1).Append(HtmlElement.BR);
                }
                if (!string.IsNullOrEmpty(address.AddressLine2))
                {
                    sb.Append(address.AddressLine2).Append(HtmlElement.BR);
                }
                if (!string.IsNullOrEmpty(address.City))
                {
                    sb.Append(address.City).Append(HtmlElement.BR);
                }
                if (!string.IsNullOrEmpty(address.County))
                {
                    sb.Append(address.County).Append(HtmlElement.BR);
                }
                if (!string.IsNullOrEmpty(address.PostCode))
                {
                    sb.Append(address.PostCode).Append(HtmlElement.BR);
                }
                if (address.USState != null)
                {
                    sb.Append(address.USState.State).Append(HtmlElement.BR);
                }
                sb.Append(address.Country.Name);

                ltl.Text = sb.ToString();
            }
        }

        protected void lbEdit_Click(object sender, EventArgs e)
        {
            var button = sender as LinkButton;
            ltlAddressTitle.Text = "Edit customer's address";

            var address = AccountService.GetAddressById(Convert.ToInt32(button.CommandArgument));
            hfAddressId.Value = address.Id.ToString();

            lbCreateAddress.Visible = false;
            lbSaveAddress.Visible = true;
            lbCancel.Visible = true;

            LoadAddressInfo(address);
        }

        protected void lbDelete_Click(object sender, EventArgs e)
        {
            var button = sender as LinkButton;
            AccountService.DeleteAddressByAddressId(Convert.ToInt32(button.CommandArgument));

            enbNotice.Message = "Address was deleted successfully.";

            LoadUserInfo(AccountService.GetAccountByProfileId(QueryUserId));
        }
        
        protected void lbCancel_Click(object sender, EventArgs e)
        {
            ltlAddressTitle.Text = "New customer's address";
            ClearInfo();
            lbCancel.Visible = false;
            lbCreateAddress.Visible = true;
            lbSaveAddress.Visible = false;
        }

        protected void lbCreateAddress_Click(object sender, EventArgs e)
        {
            int profileId = QueryUserId;

            var address = new Address
            {
                Name = txtName.Text.Trim(),
                AddressLine1 = string.IsNullOrEmpty(txtAddr1.Text.Trim()) ? null : txtAddr1.Text.Trim(),
                AddressLine2 = string.IsNullOrEmpty(txtAddr2.Text.Trim()) ? null : txtAddr2.Text.Trim(),
                City = string.IsNullOrEmpty(txtCity.Text.Trim()) ? null : txtCity.Text.Trim(),
                County = string.IsNullOrEmpty(txtCounty.Text.Trim()) ? null : txtCounty.Text.Trim(),
                PostCode = string.IsNullOrEmpty(txtPostCode.Text.Trim()) ? null : txtPostCode.Text.Trim(),
                Country = ShippingService.GetCountryById(Convert.ToInt32(ddlCountry.SelectedValue)),
                CreatedOnDate = DateTime.Now,
                UpdatedOnDate = DateTime.Now,
                IsBilling = cbIsBiling.Checked,
                IsShipping = cbIsShipping.Checked
            };
            address.CountryId = address.Country.Id;
            
            if (address.Country.ISO3166Code == "US")
            {
                var state = ShippingService.GetUSStateByCode(ddlState.SelectedValue);

                if (state != null)
                {
                    address.USState = state;
                    address.USStateId = state.Id;
                }
            }

            var account = AccountService.GetAccountByProfileId(profileId);
            address.AccountId = account.Id;

            AccountService.InsertAddress(address);
            
            if (address.IsBilling)
            {
                AccountService.UpdatePrimaryBillingAddress(address.Id, accountId: account.Id, profileId: profileId);
            }

            if (address.IsShipping)
            {
                AccountService.UpdatePrimaryShippingAddress(address.Id, accountId: account.Id, profileId: profileId);
            }

            enbNotice.Message = "Address was added successfully.";

            ClearInfo();
            LoadUserInfo(account);
        }

        protected void lbSaveAddress_Click(object sender, EventArgs e)
        {
            int profileId = QueryUserId;
            var country = ShippingService.GetCountryById(Convert.ToInt32(ddlCountry.SelectedValue));

            var address = new Address
            {
                AccountId = AccountService.GetAccountIdByProfileId(profileId),
                Name = txtName.Text.Trim(),
                AddressLine1 = txtAddr1.Text.Trim(),
                AddressLine2 = txtAddr2.Text.Trim(),
                City = txtCity.Text.Trim(),
                County = txtCounty.Text.Trim(),
                PostCode = txtPostCode.Text.Trim(),
                Country = country,
                CountryId = country.Id,
                UpdatedOnDate = DateTime.Now,
                IsBilling = cbIsBiling.Checked,
                IsShipping = cbIsShipping.Checked
            };

            if (country.ISO3166Code == "US")
            {
                var state = ShippingService.GetUSStateByCode(ddlState.SelectedValue);

                if (state != null)
                {
                    address.USState = state;
                    address.USStateId = state.Id;
                }
            }
            else
            {
                address.USState = null;
                address.USStateId = 0;
            }

            int addressId = 0;
            if (int.TryParse(hfAddressId.Value, out addressId))
            {
                address.Id = addressId;
                AccountService.UpdateAddress(address);

                if (address.IsBilling)
                {
                    AccountService.UpdatePrimaryBillingAddress(address.Id, profileId: profileId);
                }

                if (address.IsShipping)
                {
                    AccountService.UpdatePrimaryShippingAddress(address.Id, profileId: profileId);
                }

                enbNotice.Message = "Address was updated successfully.";
            }
            else
                enbNotice.Message = "Address was not updated successfully. Please try again.";

            lbCancel.Visible = false;
            lbCreateAddress.Visible = true;
            lbSaveAddress.Visible = false;

            ClearInfo();
            LoadUserInfo(AccountService.GetAccountByProfileId(profileId));            
        }

        protected void ddlCountry_Init(object sender, EventArgs e)
        {
            var list = ShippingService.GetActiveCountries();            
            ddlCountry.DataSource = list;
            ddlCountry.DataTextField = "Name";
            ddlCountry.DataValueField = "Id";
            ddlCountry.DataBind();
            ddlCountry.Items.Insert(0, new ListItem(AppConstant.DEFAULT_SELECT, string.Empty));
        }

        protected void ddlCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            int countryId = Convert.ToInt32(ddlCountry.SelectedValue);
            var country = ShippingService.GetCountryById(countryId);

            if (country.ISO3166Code == "US")
                phState.Visible = true;
            else
                phState.Visible = false;
        }

        protected void ddlState_Init(object sender, EventArgs e)
        {
            ddlState.DataSource = ShippingService.GetUSStates();
            ddlState.DataBind();
        }

        protected void ectTogRightNav_ActionOccurred(string message, bool refresh)
        {
            enbNotice.Message = message;
            if (refresh) LoadUserInfo(AccountService.GetAccountByProfileId(QueryUserId));
        }

        private void LoadUserInfo(Account account)
        {
            ltlTitle.Text = string.Format("{0} (ID: {1}){2}{3}", account.Name, account.Id.ToString(), !account.IsApproved ? " <i class='fa fa-thumbs-down' aria-hidden='true'></i>" : null, account.IsLockedOut ? " <i class='fa fa-lock' aria-hidden='true'></i>" : null);

            rptAddress.DataSource = AccountService.GetAddressesByAccountId(account.Id);
            rptAddress.DataBind();
            rptAddress.Visible = rptAddress.Items.Count > 0;
            
            ltlAddressTitle.Text = "New customer's address";
        }

        private void ClearInfo()
        {
            hfAddressId.Value = string.Empty;
            txtName.Text = string.Empty;
            txtAddr1.Text = string.Empty;
            txtAddr2.Text = string.Empty;
            txtCity.Text = string.Empty;
            txtCounty.Text = string.Empty;
            ddlCountry.SelectedIndex = 0;
            txtPostCode.Text = string.Empty;
            cbIsBiling.Checked = false;
            cbIsShipping.Checked = false;
        }

        private void LoadAddressInfo(Address address)
        {
            txtName.Text = address.Name;
            txtAddr1.Text = address.AddressLine1;
            txtAddr2.Text = address.AddressLine2;
            txtCity.Text = address.City;
            txtCounty.Text = address.County;            
            txtPostCode.Text = address.PostCode;

            ddlCountry.SelectedIndex = -1;
            if (address.CountryId != 0)
                ddlCountry.Items.FindByValue(address.CountryId.ToString()).Selected = true;

            if (address.Country.ISO3166Code == "US")
            {
                phState.Visible = true;

                ddlState.SelectedIndex = -1;
                if (address.USState != null)
                    ddlState.Items.FindByValue(address.USState.Code).Selected = true;
            }
            else
                phState.Visible = false;

            cbIsBiling.Checked = address.IsBilling;
            cbIsShipping.Checked = address.IsShipping;
        }
    }
}