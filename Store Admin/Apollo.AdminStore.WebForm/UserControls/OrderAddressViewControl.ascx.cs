using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Model.OverviewModel;
using Apollo.Core.Services.Interfaces;
using System;
using System.Text;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.UserControls
{
    public partial class UserControls_OrderAddressViewControl : BaseUserControl
    {
        private const string US = "US";
        private int _countryId;
        private string _countryCode;
        private bool _nameCheckPassed;
        private bool _addressCheckPassed;
        private bool _postCodeCheckPassed;
        private bool _hidePanel;
        private bool _hidePrint;
        private bool _hideSystemCheck = false;
        private bool _hasUSState;
        private AddressType _type;

        public delegate void AddressVerifiedEventHandler(AddressType addrType, SysCheckType checkType, bool verified);
        public delegate void AddressEventHandler(string oldAddr, Address newAddr);

        public event AddressVerifiedEventHandler Verified;
        public event AddressEventHandler AddressChanged;

        public IOrderService OrderService { get; set; }
        public IShippingService ShippingService { get; set; }

        public AddressType AddressType
        {
            set { _type = value; }
        }

        public string Title
        {
            set { ltlTitle.Text = value; }
        }

        public int OrderId
        {
            set
            {
                int orderId = value;
                AddressOverviewModel address;

                switch (_type)
                {
                    case AddressType.Billing:
                        address = OrderService.GetBillingAddressViewModelByOrderId(orderId);
                        break;
                    case AddressType.Shipping:
                    default:
                        address = OrderService.GetShippingAddressViewModelByOrderId(orderId);
                        break;
                }

                string countryName = string.Empty;
                PopulateAddress(address, out countryName);

                SystemCheck checkInfo = OrderService.GetSystemCheckByOrderId(orderId);

                if (checkInfo != null)
                    PerformSystemCheckView(checkInfo);
                else
                {
                    _nameCheckPassed = true;
                    _addressCheckPassed = true;
                    _postCodeCheckPassed = true;
                }
            }
        }

        public string Email
        {
            set
            {
                if (value != string.Empty)
                    ltlEmail.Text = string.Format("<tr class='printHide'><th>Email</th><td>{0}</td></tr>", value);
                else
                    ltlEmail.Text = string.Empty;
            }
        }

        public string Phone
        {
            set
            {
                if (value != string.Empty)
                    ltlPhone.Text = string.Format("<tr class='printHide'><th>Phone</th><td>{0}</td></tr>", value);
                else
                    ltlPhone.Text = string.Empty;
            }
        }

        public bool DisplayPhone
        {
            set
            {
                var flag = value ? "check" : "times";
                ltlDisplayPhone.Text = string.Format("<tr class='printHide'><th>Display phone number on label</th><td><i class='fa fa-{0}' aria-hidden='true'></i></td></tr>", flag);                
            }
        }

        public bool HidePanel
        {
            set { _hidePanel = value; }
        }

        public bool HidePrint
        {
            set { _hidePrint = value; }
        }

        public bool HideSystemCheck
        {
            set { _hideSystemCheck = value; }
        }

        protected string PrintHideFlag
        {
            get
            {
                if (_hidePrint) return "printHide"; else return string.Empty;
            }
        }

        protected string Visibility
        {
            get
            {
                if (_hidePanel) return "hidden"; else return "visibile";
            }
        }

        protected string Display
        {
            get
            {
                if (_hidePanel) return "none"; else return "block";
            }
        }

        private void PerformSystemCheckView(SystemCheck checkInfo)
        {
            _nameCheckPassed = true;
            _addressCheckPassed = true;
            _postCodeCheckPassed = true;

            switch (_type)
            {
                case AddressType.Billing:
                    if (!checkInfo.BillingNameCheck)
                    {
                        _nameCheckPassed = false;
                        if (!_hideSystemCheck) phName.Visible = true;
                    }

                    if (!checkInfo.BillingAddressCheck)
                    {
                        _addressCheckPassed = false;
                        if (!_hideSystemCheck) phAddr.Visible = true;
                    }

                    if (!checkInfo.BillingPostCodeCheck)
                    {
                        _postCodeCheckPassed = false;
                        if (!_hideSystemCheck) phPostCode.Visible = true;
                    }
                    break;
                case AddressType.Shipping:
                    if (!checkInfo.ShippingNameCheck)
                    {
                        _nameCheckPassed = false;
                        if (!_hideSystemCheck) phName.Visible = true;
                    }

                    if (!checkInfo.ShippingAddressCheck)
                    {
                        _addressCheckPassed = false;
                        if (!_hideSystemCheck) phAddr.Visible = true;
                    }

                    if (!checkInfo.ShippingPostCodeCheck)
                    {
                        _postCodeCheckPassed = false;
                        if (!_hideSystemCheck) phPostCode.Visible = true;
                    }
                    break;
                case AddressType.Both:                    
                default:
                    break;
            }           
        }

        public bool EditDisabled
        {
            set { phEditItem.Visible = !value; }
        }

        protected void lbEditItem_Click(object sender, EventArgs e)
        {
            phEdit.Visible = true;
            txtName.Text = ltlName.Text;
            txtAddrLine1.Text = ltlAddr1.Text;
            txtAddrLine2.Text = ltlAddr2.Text;
            txtCity.Text = ltlCity.Text;
            txtCounty.Text = ltlCounty.Text;

            ddlCountry.SelectedIndex = -1;
            ListItem found = ddlCountry.Items.FindByValue(Convert.ToString(_countryId)) as ListItem;
            
            if (found != null) found.Selected = true;

            txtPostCode.Text = ltlPostCode.Text;
            phAddress.Visible = false;
        }

        protected void lbCancel_Click(object sender, EventArgs e)
        {
            phEdit.Visible = false;
            phAddress.Visible = true;
        }

        protected void ddlCountry_Init(object sender, EventArgs e)
        {
            ddlCountry.DataSource = ShippingService.GetActiveCountries();
            ddlCountry.DataBind();
        }

        protected void ddlState_Init(object sender, EventArgs e)
        {
            ddlState.DataSource = ShippingService.GetUSStates();
            ddlState.DataBind();
        }
        
        protected void lbSave_Click(object sender, EventArgs e)
        {
            Address address = new Address();
            address.Name = txtName.Text.Trim();
            address.AddressLine1 = txtAddrLine1.Text.Trim();
            address.AddressLine2 = txtAddrLine2.Text.Trim();
            address.City = txtCity.Text.Trim();
            address.County = txtCounty.Text.Trim();
            address.PostCode = txtPostCode.Text.Trim();
            address.Country = ShippingService.GetCountryById(Convert.ToInt32(ddlCountry.SelectedValue));
            address.CountryId = address.Country.Id;

            if (address.Country != null && address.Country.ISO3166Code == "US")
            {
                var usState = ShippingService.GetUSStateByCode(ddlState.SelectedValue);
                address.USState = usState;
                address.USStateId = usState.Id;
            }
            
            phEdit.Visible = false;
            phAddress.Visible = true;
            InvokeChanged(address);
        }

        protected bool GetNameCheckStatus()
        {
            return _hideSystemCheck || _nameCheckPassed;
        }

        protected bool GetAddrCheckStatus()
        {
            return _hideSystemCheck || _addressCheckPassed;
        }

        protected bool GetPostCodeCheckStatus()
        {
            return _hideSystemCheck || _postCodeCheckPassed;
        }

        protected bool HasUSState()
        {
            return _hasUSState;
        }

        protected void lbVerifyName_Click(object sender, EventArgs e)
        {
            InvokeVerified(SysCheckType.Name, true);
            phName.Visible = false;
        }

        protected void lbVerifyAddr_Click(object sender, EventArgs e)
        {
            InvokeVerified(SysCheckType.Address, true);
            phAddr.Visible = false;
        }

        protected void lbVerifyPostCode_Click(object sender, EventArgs e)
        {
            InvokeVerified(SysCheckType.PostCode, true);
            phPostCode.Visible = false;
        }

        private void InvokeChanged(Address newAddr)
        {
            AddressEventHandler handler = AddressChanged;
            if (handler != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("{0}<br/>", ltlName.Text);
                sb.AppendFormat("{0}<br/>", ltlAddr1.Text);
                sb.AppendFormat("{0}<br/>", ltlAddr2.Text);

                if (ltlCounty.Text != string.Empty)
                    sb.AppendFormat("{0}<br/>", ltlCounty.Text);

                sb.AppendFormat("{0}<br/>", ltlCity.Text);
                sb.AppendFormat("{0}<br/>", ltlPostCode.Text);

                if (ltlState.Text != string.Empty)
                    sb.AppendFormat("{0}<br/>", ltlState.Text);

                sb.AppendFormat("{0}", ltlCountry.Text);

                handler(sb.ToString(), newAddr);
            }
        }

        private void InvokeVerified(SysCheckType checkType, bool verified)
        {
            Verified?.Invoke(_type, checkType, verified);
        }

        private void PopulateAddress(AddressOverviewModel address, out string countryName)
        {
            countryName = string.Empty;

            if (address.Name != string.Empty)
                ltlName.Text = address.Name;

            if (address.AddressLine1 != string.Empty)
                ltlAddr1.Text = address.AddressLine1;

            if (address.AddressLine2 != string.Empty)
                ltlAddr2.Text = address.AddressLine2;

            if (address.City != string.Empty)
                ltlCity.Text = address.City;

            if (address.County != string.Empty)
            {
                phCountryField.Visible = true;
                ltlCounty.Text = address.County;
            }
            else
            {
                phCountryField.Visible = false;
            }

            if (address.PostCode != string.Empty)
                ltlPostCode.Text = address.PostCode;
            
            if (address.CountryId != 0)
            {
                Country country = ShippingService.GetCountryById(address.CountryId);

                countryName = country.Name;
                ltlCountry.Text = country.Name;
                _countryCode = country.ISO3166Code;
                _countryId = country.Id;
            }
            else
            {
                _countryCode = string.Empty;
            }

            phStateField.Visible = false;
            phState.Visible = false;
            _hasUSState = false;
            ltlState.Text = string.Empty;
            ltlStateCode.Text = "-";

            if (_countryCode == US && address.USStateId != 0)
            {
                USState usState = ShippingService.GetUSStateById(address.USStateId);

                if (usState != null)
                {
                    phStateField.Visible = true;
                    phState.Visible = true;
                    _hasUSState = true;
                    ltlState.Text = usState.State;
                    ltlStateCode.Text = usState.Code;
                }
            }
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
    }
}