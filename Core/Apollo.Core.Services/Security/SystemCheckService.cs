using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Model.OverviewModel;
using Apollo.Core.Services.Payment;
using Apollo.DataAccess;
using Apollo.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;

namespace Apollo.Core.Services.Security
{
    public class SystemCheckService : ISystemCheckService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<SystemCheck> _systemCheckRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<Country> _countryRepository;
        private readonly IRepository<Account> _accountRepository;
        private readonly IRepository<ShippingOption> _shippingOptionRepository;
        private readonly IRepository<SagePayDirect> _sagePayDirectRepository;
        private readonly IPaymentSystemService _paymentSystem;

        public SystemCheckService(
            IDbContext dbContext,
            IRepository<SystemCheck> systemCheckRepository,
            IRepository<Order> orderRepository,
            IRepository<Country> countryRepository,
            IRepository<Account> accountRepository,
            IRepository<ShippingOption> shippingOptionRepository,
            IRepository<SagePayDirect> sagePayDirectRepository,
            IPaymentSystemService paymentSystem)
        {
            _dbContext = dbContext;
            _systemCheckRepository = systemCheckRepository;
            _orderRepository = orderRepository;
            _countryRepository = countryRepository;
            _accountRepository = accountRepository;
            _shippingOptionRepository = shippingOptionRepository;
            _sagePayDirectRepository = sagePayDirectRepository;
            _paymentSystem = paymentSystem;
        }

        public SystemCheck ProcessSystemChecking(Order order, string email, string name, bool avsCheck)
        {
            var pBillingAddress = GetParameter("BillingAddress", string.Format("{0} {1}", order.AddressLine1, order.AddressLine2));
            var pShippingAddress = GetParameter("ShippingAddress", string.Format("{0} {1}", order.ShippingAddressLine1, order.ShippingAddressLine2));
            var pEmail = GetParameter("Email", email);
            var pBillingPostCode = GetParameter("BillingPostCode", order.PostCode);
            var pShippingPostCode = GetParameter("ShippingPostCode", order.ShippingPostCode);
            var pName = GetParameter("Name", name);
            var pBillTo = GetParameter("BillTo", order.BillTo);
            var pShipTo = GetParameter("ShipTo", order.ShipTo);

            var pBillingAddressFound = GetParameterBooleanOutput("BillingAddressFound");
            var pShippingAddressFound = GetParameterBooleanOutput("ShippingAddressFound");
            var pEmailFound = GetParameterBooleanOutput("EmailFound");
            var pBillingPostCodeFound = GetParameterBooleanOutput("BillingPostCodeFound");
            var pShippingPostCodeFound = GetParameterBooleanOutput("ShippingPostCodeFound");
            var pNameFound = GetParameterBooleanOutput("NameFound");
            var pBillToFound = GetParameterBooleanOutput("BillToFound");
            var pShipToFound = GetParameterBooleanOutput("ShipToFound");

            _dbContext.ExecuteSqlCommand(@"EXEC SystemCheck_Find 
                @BillingAddress,
                @ShippingAddress,
                @Email,
                @BillingPostCode,
                @ShippingPostCode,
                @Name,
                @BillTo,
                @ShipTo,
                @BillingAddressFound = @BillingAddressFound OUTPUT,
                @ShippingAddressFound = @ShippingAddressFound OUTPUT,
                @EmailFound = @EmailFound OUTPUT,
                @BillingPostCodeFound = @BillingPostCodeFound OUTPUT,
                @ShippingPostCodeFound = @ShippingPostCodeFound OUTPUT,
                @NameFound = @NameFound OUTPUT,
                @BillToFound = @BillToFound OUTPUT,
                @ShipToFound = @ShipToFound OUTPUT",
                pBillingAddress,
                pShippingAddress,
                pEmail,
                pBillingPostCode,
                pShippingPostCode,
                pName,
                pBillTo,
                pShipTo,
                pBillingAddressFound,
                pShippingAddressFound,
                pEmailFound,
                pBillingPostCodeFound,
                pShippingPostCodeFound,
                pNameFound,
                pBillToFound,
                pShipToFound);

            bool billingAddressFound = (pBillingAddressFound.Value != DBNull.Value) ? Convert.ToBoolean(pBillingAddressFound.Value) : false;
            bool shippingAddressFound = (pShippingAddressFound.Value != DBNull.Value) ? Convert.ToBoolean(pShippingAddressFound.Value) : false;
            bool emailFound = (pEmailFound.Value != DBNull.Value) ? Convert.ToBoolean(pEmailFound.Value) : false;
            bool billingPostCodeFound = (pBillingPostCodeFound.Value != DBNull.Value) ? Convert.ToBoolean(pBillingPostCodeFound.Value) : false;
            bool shippingPostCodeFound = (pShippingPostCodeFound.Value != DBNull.Value) ? Convert.ToBoolean(pShippingPostCodeFound.Value) : false;
            bool nameFound = (pNameFound.Value != DBNull.Value) ? Convert.ToBoolean(pNameFound.Value) : false;
            bool billToFound = (pBillToFound.Value != DBNull.Value) ? Convert.ToBoolean(pBillToFound.Value) : false;
            bool shipToFound = (pShipToFound.Value != DBNull.Value) ? Convert.ToBoolean(pShipToFound.Value) : false;

            // Address matching
            bool addressMatched = AreBothAddressMatched(order.BillTo, order.AddressLine1, order.AddressLine2, order.PostCode, order.CountryId,
                                                        order.ShipTo, order.ShippingAddressLine1, order.ShippingAddressLine2, order.ShippingPostCode, 
                                                        order.ShippingCountryId);

            if (addressMatched == false)
            {
                // As both billing and shipping addresses are not matched, 
                // reset every address related check
                billToFound = true;
                shipToFound = true;
                billingAddressFound = true;
                shippingAddressFound = true;
                billingPostCodeFound = true;
                shippingPostCodeFound = true;
            }

            return new SystemCheck
            {
                OrderId = order.Id,
                AvsCheck = avsCheck,
                BillingAddressCheck = !billingAddressFound,
                ShippingAddressCheck = !shippingAddressFound,
                EmailCheck = !emailFound,
                BillingPostCodeCheck = !billingPostCodeFound,
                ShippingPostCodeCheck = !shippingPostCodeFound,
                NameCheck = !nameFound,
                BillingNameCheck = !billToFound,
                ShippingNameCheck = !shipToFound
            };
        }
        
        public Dictionary<string, int> CalculateSystemCheckScore(int orderId)
        {
            var scores = new Dictionary<string, int>();

            #region Standard Check (130)

            // System check
            var systemCheck = _systemCheckRepository.Table.Where(x => x.OrderId == orderId).FirstOrDefault();
            if (systemCheck != null)
            {
                var standardSystemCheckScore = 0;

                if (!systemCheck.AvsCheck) standardSystemCheckScore += 10;
                if (!systemCheck.EmailCheck) standardSystemCheckScore += 20;
                if (!systemCheck.NameCheck) standardSystemCheckScore += 20;
                if (!systemCheck.BillingNameCheck) standardSystemCheckScore += 20;
                if (!systemCheck.ShippingNameCheck) standardSystemCheckScore += 20;
                if (!systemCheck.BillingAddressCheck) standardSystemCheckScore += 10;
                if (!systemCheck.BillingPostCodeCheck) standardSystemCheckScore += 10;
                if (!systemCheck.ShippingAddressCheck) standardSystemCheckScore += 10;
                if (!systemCheck.ShippingPostCodeCheck) standardSystemCheckScore += 10;

                if (standardSystemCheckScore > 0) scores.Add("Standard Check", standardSystemCheckScore);

            }

            #endregion

            var order = _orderRepository.Return(orderId);
            if (order != null)
            {
                #region Country Combination (60)

                #region Card Country
                var cardCountry = string.Empty;
                var paymentDetails = _paymentSystem.GetPaymentDetails(orderId);
                if (string.IsNullOrEmpty(paymentDetails) == false)
                {
                    var cardCountryCode = paymentDetails.Substring(paymentDetails.Length - 2);
                    cardCountry = _countryRepository.Table.Where(x => x.ISO3166Code == cardCountryCode).Select(x => x.Name).FirstOrDefault(); 
                }
                #endregion

                #region IP Country
                // Get IP location country
                var ipCountry = string.Empty;
                var ipLocation = _paymentSystem.GetIPLocation(orderId);
                if (string.IsNullOrEmpty(ipLocation) == false)
                {
                    var regex = new Regex(@"Country: ([A-Za-z0-9\s]*)");
                    Match match = regex.Match(ipLocation);
                    if (match.Success)
                    {
                        ipCountry = match.Groups[1].Value;
                    }
                }
                #endregion

                var billingCountry = _countryRepository.Return(order.CountryId);
                var shippingCountry = _countryRepository.Return(order.ShippingCountryId);

                var card_ip = AreEqual(cardCountry, ipCountry);
                var card_billing = AreEqual(cardCountry, billingCountry.Name);
                var card_shipping = AreEqual(cardCountry, shippingCountry.Name);
                var ip_billing = AreEqual(ipCountry, billingCountry.Name);
                var ip_shipping = AreEqual(ipCountry, shippingCountry.Name);
                var billing_shipping = AreEqual(billingCountry.Name, shippingCountry.Name);

                if (card_billing == false)
                {
                    scores.Add("Country Combination", 60);
                }
                else if (ip_shipping == false)
                {
                    scores.Add("Country Combination", 45);
                }
                else if (billing_shipping == false)
                {
                    scores.Add("Country Combination", 35);
                }
                else if (card_ip == false || card_shipping == false || ip_billing == false)
                {
                    scores.Add("Country Combination", 30);
                }
                
                #endregion

                #region Name Combination (30)

                // Name combination
                var billTo = order.BillTo;
                var shipTo = order.ShipTo;
                var account = _accountRepository.Table.Where(x => x.ProfileId == order.ProfileId).FirstOrDefault();

                if (account != null)
                {
                    if (AreEqual(account.Name, billTo) == false &&
                    AreEqual(account.Name, shipTo) == false &&
                    AreEqual(billTo, shipTo) == false)
                    {
                        scores.Add("Name Combination", 30);
                    }
                    else if (AreEqual(account.Name, billTo) == false &&
                             AreEqual(account.Name, shipTo) == false &&
                             AreEqual(billTo, shipTo))
                    {
                        scores.Add("Name Combination", 20);
                    }
                    else if (AreEqual(account.Name, shipTo) == false &&
                             AreEqual(account.Name, billTo))
                    {
                        scores.Add("Name Combination", 20);
                    }
                    else if (AreEqual(account.Name, billTo) == false &&
                             AreEqual(account.Name, shipTo))
                    {
                        scores.Add("Name Combination", 10);
                    }
                }
                
                #endregion

                #region Next Day Delivery (10)
                var shippingOption = _shippingOptionRepository.Return(order.ShippingOptionId);
                if (shippingOption != null && shippingOption.Name.Contains("Next Day"))
                {
                    scores.Add("Next Day Delivery", 10);
                }
                #endregion

                #region Allocated Points Usage (10)
                if (order.AllocatedPoint < 0)
                {
                    scores.Add("Allocated Points Usage", 10);
                }
                #endregion

                #region Account Created Date (10)
                if ((DateTime.Now - account.CreationDate).Days < 7)
                {
                    scores.Add("Account Created Date", 10);
                }
                #endregion

                #region Number Of Orders (10)
                var orderCount = _orderRepository.Table
                    .Where(x => x.ProfileId == order.ProfileId)
                    .Where(x => x.Paid == true)
                    .Count();
                if (orderCount <= 3)
                {
                    scores.Add("Number Of Orders", 10);
                }
                #endregion
            }

            #region Third Man Score (100)            
            var t3mResult = _paymentSystem.GetThirdManResult(orderId);
            if (string.IsNullOrEmpty(t3mResult) == false)
            {
                var regex = new Regex(@"\s([+-]?[0-9]+)");
                Match match = regex.Match(t3mResult);
                if (match.Success)
                {
                    var t3Score = Convert.ToInt32(match.Groups[1].Value);
                    if (t3Score > 75)
                    {
                        scores.Add("Third Man Score", 100);
                    }
                    else if (t3Score > 30)
                    {
                        scores.Add("Third Man Score", 50);
                    }                    
                }
            }
            #endregion

            #region Payment System Check (150)

            var spModel = _sagePayDirectRepository.Table
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

            var spScores = 0;

            if (spModel != null)
            {
                spScores += CheckSagePayStatus(spModel.AddressResult, "address");
                spScores += CheckSagePayStatus(spModel.PostCodeResult, "postcode");
                spScores += CheckSagePayStatus(spModel.CV2Result, "cv2");
                spScores += CheckSagePayStatus(spModel.ThreeDSecureStatus, "3d");
            }

            if (spScores > 0)
            {
                scores.Add("Payment System Check", spScores);
            }

            #endregion

            var total = scores.Select(x => x.Value).Sum();            
            scores.Add("Total Score (Lower is better)", total);

            return scores;            
        }

        private int CheckSagePayStatus(string check, string item)
        {
            int score;

            switch (check)
            {
                case "SECURITY CODE MATCH ONLY": score = 10; break;
                case "ADDRESS MATCH ONLY": score = 10; break;
                case "NO DATA MATCHES": score = 30; break;
                case "DATA NOT CHECKED": score = 20; break;
                case "ALL MATCH": score = 0; break;
                case "NOTPROVIDED": score = 20; break;
                case "NOTCHECKED": score = 20; break;
                case "MATCHED": score = 0; break;
                case "NOTMATCHED": score = 30; break;

                case "OK": score = 0; break;
                case "NOAUTH": score = 20; break;
                case "CANTAUTH": score = 20; break;
                case "NOTAUTH": score = 30; break;
                case "ATTEMPTONLY": score = 10; break;
                case "INCOMPLETE": score = 10; break;
                case "MALFORMED": score = 10; break;
                case "INVALID": score = 10; break;
                case "ERROR": score = 10; break;

                default:
                    if (string.IsNullOrWhiteSpace(check)) check = "no data found";
                    score = 10;
                    break;
            }

            // 3D Secure is more important.
            if (item == "3d" && score > 0)
                return score + 30;                

            return score;
        }

        private bool AreBothAddressMatched(string billTo, string addressLine1, string addressLine2, string postCode, int countryId,
            string shipTo, string shippingAddressLine1, string shippingAddressLine2, string shippingPostCode, int shippingCountryId)
        {
            return AreEqual(billTo, shipTo) 
                && AreEqual(addressLine1, shippingAddressLine1)
                && AreEqual(addressLine2, shippingAddressLine2)
                && AreEqual(postCode, shippingPostCode)
                && countryId == shippingCountryId;            
        }

        private bool AreEqual(string a, string b)
        {
            if (string.IsNullOrEmpty(a))            
                return string.IsNullOrEmpty(b);
            
            return string.Equals(a, b, StringComparison.InvariantCultureIgnoreCase);            
        }

        #region Sql Helper

        static Dictionary<string, SqlParameter> _cachedParams = new Dictionary<string, SqlParameter>();
        static readonly object _object = new object();

        private SqlParameter GetParameterWithNull(string name)
        {
            lock (_object)
            {
                if (!_cachedParams.ContainsKey(name))
                {
                    SqlParameter param = new SqlParameter(name, DBNull.Value);
                    _cachedParams.Add(name, param);
                }

                return (SqlParameter)((ICloneable)_cachedParams[name]).Clone();
            }
        }

        private SqlParameter GetParameterWithValue(string name, object value, DbType type)
        {
            SqlParameter param = GetParameterWithNull(name);
            param.Value = value;
            param.DbType = type;

            return param;
        }

        private SqlParameter GetParameter(string name, string value)
        {
            if (value == null) return GetParameterWithNull(name);
            return GetParameterWithValue(name, value, DbType.String);
        }

        private SqlParameter GetParameterBooleanOutput(string name)
        {
            SqlParameter param = new SqlParameter();
            param.ParameterName = name;
            param.DbType = DbType.Boolean;
            param.Direction = ParameterDirection.Output;

            return param;
        }

        #endregion
    }
}
