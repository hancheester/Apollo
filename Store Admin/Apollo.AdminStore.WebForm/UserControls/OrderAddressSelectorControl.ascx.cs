using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Domain.Shipping;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace Apollo.AdminStore.WebForm.UserControls
{
    public partial class UserControls_OrderAddressSelectorControl : BaseUserControl, ICallbackEventHandler
    {
        private const string COMMA = ", ";
        private const string ADDRESS_ID = "AddressId";
        private const string ADDRESS = "Address";
        private const string PLEASE_SELECT = "Please select";

        public IAccountService AccountService { get; set; }
        public IShippingService ShippingService { get; set; }
        public ShippingSettings ShippingSettings { get; set; }

        public delegate void AddressCountryEventHandler(int countryId);
        public event AddressCountryEventHandler AddressCountryChanged;

        public delegate void AddressChangeEventHandler(int addressId);
        public event AddressChangeEventHandler AddressChanged;

        private bool _forceAddressLoading;
        private int _countryId;
        private Address _address;
        private AddressType _type;
        
        public Address Address
        {
            set { _address = value; }
            get { return BuildAddress(); }
        }

        public int QueryUserId
        {
            get { return GetIntQuery("userid"); }
        }
        
        public bool ForceAddressLoading
        {
            set { _forceAddressLoading = value; }
        }

        public int SetCountryId
        {
            set {
                _countryId = value;
                ddlCountry.SelectedIndex = -1;

                var item = ddlCountry.Items.FindByValue(_countryId.ToString());

                if (item != null) item.Selected = true;
            }
        }
        
        public bool SaveAddress
        {
            get { return chkSaveAddress.Checked; }
        }

        public AddressType AddressType
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
                switch (_type)
                {
                    case AddressType.Billing:
                        rfName.ErrorMessage = "Billing name is required.";
                        rfAddressLine1.ErrorMessage = "Billing address line 1 is required.";
                        rfCity.ErrorMessage = "Billing city is required.";
                        break;
                    default:
                    case AddressType.Shipping:
                        rfName.ErrorMessage = "Shipping name is required.";
                        rfAddressLine1.ErrorMessage = "Shipping address line 1 is required.";
                        rfCity.ErrorMessage = "Shipping city is required.";
                        break;
                }
            }
        }

        public UserControls_OrderAddressSelectorControl()
        {
            _forceAddressLoading = false;
        }

        public void ClearAddress()
        {
            _address = null;
            txtName.Text = string.Empty;
            txtAddrLine1.Text = string.Empty;
            txtAddrLine2.Text = string.Empty;
            txtCounty.Text = string.Empty;
            txtCity.Text = string.Empty;
            txtPostCode.Text = string.Empty;
            ddlCountry.SelectedIndex = 0;
            ddlState.SelectedIndex = 0;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            var profileId = QueryUserId;

            if (!Page.IsPostBack && profileId != 0)
            {
                var isAnonymous = AccountService.GetAnonymousStatusByProfileId(profileId);

                if (isAnonymous == true)
                    phSaveAddress.Visible = false;
                else
                    LoadAddressesByProfileId(profileId);

                if (_address != null) LoadAddressFields(_address);
            }

            if (_forceAddressLoading)
            {
                LoadAddressFields(_address);
                _forceAddressLoading = false;
            }
        }

        protected void ddlExistingAddresses_SelectedIndexChanged(object sender, EventArgs e)
        {
            int addressId = Convert.ToInt32(ddlExistingAddresses.SelectedValue);

            if (addressId != 0)
            {
                var address = AccountService.GetAddressById(addressId);                
                LoadAddressFields(address);               
            }
            else
                ClearAddress();

            InvokeAddressChanged(addressId);
        }

        protected void ddlCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            int countryId = Convert.ToInt32(ddlCountry.SelectedValue);
            var country = ShippingService.GetCountryById(countryId);

            if (country.ISO3166Code == "US")
                phState.Visible = true;
            else
                phState.Visible = false;

            InvokeCountryChanged(countryId);
        }

        private void LoadAddressFields(Address address)
        {
            ltAddressId.Text = "<input type='hidden' class='addressId' value='" + address.Id + "'/>";
            txtName.Text = address.Name;
            txtAddrLine1.Text = address.AddressLine1;
            txtAddrLine2.Text = address.AddressLine2;
            txtCounty.Text = address.County;
            txtCity.Text = address.City;
            txtPostCode.Text = address.PostCode;
            ddlCountry.SelectedIndex = ddlCountry.Items.IndexOf(ddlCountry.Items.FindByValue(address.CountryId.ToString()));
            if (address.Country.ISO3166Code == "US")
            {
                phState.Visible = true;

                if (address.USState != null)
                    ddlState.SelectedIndex = ddlState.Items.IndexOf(ddlState.Items.FindByValue(address.USState.Code));
                else
                    ddlState.SelectedIndex = 0;

            }
            else
                phState.Visible = false;
        }

        /// <summary>
        /// Load existing addresses by profile id. If addresses are available, display ddlExistingAddresses and vice versa.
        /// </summary>
        /// <param name="profileId"></param>
        private void LoadAddressesByProfileId(int profileId)
        {
            var addresses = AccountService.GetAddressesByProfileId(profileId);

            DataTable dt = new DataTable();
            dt.Columns.Add(ADDRESS_ID);
            dt.Columns.Add(ADDRESS);

            DataRow firstRow = dt.NewRow();
            firstRow[ADDRESS_ID] = 0;
            firstRow[ADDRESS] = PLEASE_SELECT;
            dt.Rows.Add(firstRow);

            for (int i = 0; i < addresses.Count; i++)
            {
                DataRow row = dt.NewRow();
                row[ADDRESS_ID] = addresses[i].Id;

                StringBuilder sb = new StringBuilder();

                if (addresses[i].Name != string.Empty) { sb.Append(addresses[i].Name); sb.Append(COMMA); }
                if (addresses[i].AddressLine1 != string.Empty) { sb.Append(addresses[i].AddressLine1); sb.Append(COMMA); }
                if (addresses[i].AddressLine2 != string.Empty) { sb.Append(addresses[i].AddressLine2); sb.Append(COMMA); }
                if (addresses[i].City != string.Empty) { sb.Append(addresses[i].City); sb.Append(COMMA); }
                if (addresses[i].County != string.Empty) { sb.Append(addresses[i].County); sb.Append(COMMA); }
                if (addresses[i].USState != null) { sb.Append(addresses[i].USState.State); sb.Append(COMMA); }
                if (addresses[i].PostCode != string.Empty) { sb.Append(addresses[i].PostCode); sb.Append(COMMA); }
                if (addresses[i].Country != null) { sb.Append(addresses[i].Country.Name); }

                row[ADDRESS] = sb.ToString();
                dt.Rows.Add(row);
            }

            phExistingAddresses.Visible = addresses.Count > 0;
            ddlExistingAddresses.Visible = addresses.Count > 0;
            ddlExistingAddresses.DataTextField = ADDRESS;
            ddlExistingAddresses.DataValueField = ADDRESS_ID;
            ddlExistingAddresses.DataSource = dt;
            ddlExistingAddresses.DataBind();

            for (int i = 0; i < addresses.Count; i++)
            {
                if (BothAddressesAreSame(Address, addresses[i]))
                {
                    ddlExistingAddresses.SelectedIndex = ddlExistingAddresses.Items.IndexOf(ddlExistingAddresses.Items.FindByValue(addresses[i].Id.ToString()));
                    break;
                }
                else
                    ddlExistingAddresses.SelectedIndex = 0;
            }

            switch (_type)
            {
                case AddressType.Billing:
                    var billingAddress = addresses.Where(x => x.IsBilling == true).FirstOrDefault();
                    if (billingAddress != null)
                        LoadAddressFields(billingAddress);
                    else
                    {
                        if (addresses.Count > 0)
                        {
                            LoadAddressFields(addresses[0]);
                        }
                    }

                    break;
                case AddressType.Shipping:
                    var shippingAddress = addresses.Where(x => x.IsShipping == true).FirstOrDefault();
                    if (shippingAddress != null)
                        LoadAddressFields(shippingAddress);
                    else
                    {
                        if (addresses.Count > 0)
                        {
                            LoadAddressFields(addresses[0]);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private Address BuildAddress()
        {
            Address address = new Address
            {
                Name = txtName.Text,
                AddressLine1 = string.IsNullOrEmpty(txtAddrLine1.Text.Trim()) ? null : txtAddrLine1.Text.Trim(),
                AddressLine2 = string.IsNullOrEmpty(txtAddrLine2.Text.Trim()) ? null : txtAddrLine2.Text.Trim(),
                City = string.IsNullOrEmpty(txtCity.Text.Trim()) ? null : txtCity.Text.Trim(),
                County = string.IsNullOrEmpty(txtCounty.Text.Trim()) ? null : txtCounty.Text.Trim(),
                PostCode = string.IsNullOrEmpty(txtPostCode.Text.Trim()) ? null : txtPostCode.Text.Trim(),
                CreatedOnDate = DateTime.Now,
                UpdatedOnDate = DateTime.Now
            };

            Country country = ShippingService.GetCountryById(Convert.ToInt32(ddlCountry.SelectedItem.Value));
            address.Country = country;
            address.CountryId = country.Id;

            if (address.Country.ISO3166Code == "US")
            {
                USState usState = ShippingService.GetUSStateByCode(ddlState.SelectedValue);
                address.USState = usState;
                address.USStateId = usState.Id;
            }

            var profileId = QueryUserId;
            var isAnonymous = AccountService.GetAnonymousStatusByProfileId(profileId);

            if (isAnonymous == false)
            {
                var addressList = AccountService.GetAddressesByProfileId(profileId);
                var id = addressList.Where(a => BothAddressesAreSame(a, address)).Select(a => a.Id).FirstOrDefault();
                address.Id = id;
            }

            return address;
        }

        private bool BothAddressesAreSame(Address addressA, Address addressB)
        {
            if (addressA.Country == null) throw new ArgumentException("should not be null", "addressA.Country");
            if (addressB.Country == null) throw new ArgumentException("should not be null", "addressB.Country");

            bool compareResult =
                addressA.Name == addressB.Name &&
                addressA.AddressLine1 == addressB.AddressLine1 &&
                addressA.AddressLine2 == addressB.AddressLine2 &&
                addressA.City == addressB.City &&
                addressA.County == addressB.County &&
                addressA.PostCode == addressB.PostCode &&
                addressA.Country.Id == addressB.Country.Id;

            if (addressA.Country.ISO3166Code == "US" && addressB.Country.ISO3166Code == "US")
                return compareResult && addressA.USState.State == addressB.USState.State;
            else
                return compareResult;
        }

        protected void ddlCountry_Init(object sender, EventArgs e)
        {
            ddlCountry.DataSource = ShippingService.GetActiveCountries();
            ddlCountry.DataBind();

            ddlCountry.Items.FindByValue(ShippingSettings.PrimaryStoreCountryId.ToString()).Selected = true;
        }

        protected void ddlState_Init(object sender, EventArgs e)
        {
            ddlState.DataSource = ShippingService.GetUSStates();
            ddlState.DataBind();
        }

        private void InvokeCountryChanged(int countryId)
        {
            AddressCountryEventHandler handler = AddressCountryChanged;
            if (handler != null)
            {
                handler(countryId);
            }
        }

        private void InvokeAddressChanged(int addressId)
        {
            AddressChangeEventHandler handler = AddressChanged;
            if (handler != null)
            {
                handler(addressId);
            }
        }

        #region ICallbackEventHandler Members

        private string _result;

        void ICallbackEventHandler.RaiseCallbackEvent(string eventArgument)
        {
            // Format "id + '_' + name + '_' + line1 + '_' + line2 + '_' + county + '_' + city + '_' + postcode + '_' + state + '_' + country"
            const int ID = 0;
            const int NAME = 1;
            const int LINE1 = 2;
            const int LINE2 = 3;
            const int COUNTY = 4;
            const int CITY = 5;
            const int POSTCODE = 6;
            const int STATE = 7;
            const int COUNTRY = 8;

            var args = eventArgument.Split('_');
            int addressId = 0;
            if (int.TryParse(args[ID], out addressId))
            {
                var address = AccountService.GetAddressById(addressId);
                if (address != null)
                {
                    address.Name = args[NAME].Trim();
                    address.AddressLine1 = args[LINE1].Trim();
                    address.AddressLine2 = string.IsNullOrEmpty(args[LINE2].Trim()) ? null : args[LINE2].Trim();
                    address.County = string.IsNullOrEmpty(args[COUNTY].Trim()) ? null : args[COUNTY].Trim();
                    address.City = string.IsNullOrEmpty(args[CITY].Trim()) ? null : args[CITY].Trim();
                    address.PostCode = string.IsNullOrEmpty(args[POSTCODE].Trim()) ? null : args[POSTCODE].Trim();

                    var state = ShippingService.GetUSStateByCode(args[STATE]);

                    if (state != null)
                    {
                        address.USStateId = state.Id;
                    }

                    int countryId = 0;
                    if (int.TryParse(args[COUNTRY], out countryId))
                    {
                        address.CountryId = countryId;
                    }

                    AccountService.UpdateAddress(address);

                    _result = "success!";
                    return;
                }
            }
            else
            {
                var profileId = QueryUserId;

                var address = new Address
                {
                    Name = args[NAME].Trim(),
                    AddressLine1 = args[LINE1].Trim(),
                    AddressLine2 = string.IsNullOrEmpty(args[LINE2].Trim()) ? null : args[LINE2].Trim(),
                    County = string.IsNullOrEmpty(args[COUNTY].Trim()) ? null : args[COUNTY].Trim(),
                    City = string.IsNullOrEmpty(args[CITY].Trim()) ? null : args[CITY].Trim(),
                    PostCode = string.IsNullOrEmpty(args[POSTCODE].Trim()) ? null : args[POSTCODE].Trim(),
                    CreatedOnDate = DateTime.Now,
                    UpdatedOnDate = DateTime.Now,
                };

                var state = ShippingService.GetUSStateByCode(args[STATE]);
                if (state != null)
                {
                    address.USStateId = state.Id;
                }

                int countryId = 0;
                if (int.TryParse(args[COUNTRY], out countryId))
                {
                    address.CountryId = countryId;
                }

                address.AccountId = AccountService.GetAccountIdByProfileId(profileId);
                AccountService.InsertAddress(address);

                _result = "success!";
                return;
            }

            _result = "failed, try again!";
        }

        string ICallbackEventHandler.GetCallbackResult()
        {            
            return _result;
        }

        #endregion
    }
}