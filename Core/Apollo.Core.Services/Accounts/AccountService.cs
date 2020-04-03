using Apollo.Core.Caching;
using Apollo.Core.Logging;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Model.OverviewModel;
using Apollo.Core.Services.Accounts.Identity;
using Apollo.Core.Services.Common;
using Apollo.Core.Services.Interfaces;
using Apollo.Core.Services.Interfaces.DataBuilder;
using Apollo.DataAccess;
using Apollo.DataAccess.Interfaces;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Claims;

namespace Apollo.Core.Services.Accounts
{
    public class AccountService : BaseRepository, IAccountService
    {
        #region Fields

        private readonly IDbContext _dbContext;
        private readonly IRepository<Account> _accountRepository;
        private readonly IRepository<Profile> _profileRepository;
        private readonly IRepository<Address> _addressRepository;
        private readonly IRepository<RewardPointHistory> _rewardPointRepository;
        private readonly IRepository<ActivityLog> _activityLogRepository;
        private readonly IRepository<Country> _countryRepository;
        private readonly IRepository<USState> _usStateRepository;
        private readonly IRepository<CartItem> _cartItemRepository;
        private readonly IRepository<Subscriber> _subscriberRepository;
        private readonly IRepository<UserBehaviour> _userBehaviourRepository;
        private readonly IRepository<EmailMessage> _emailMessageRepository;        
        private readonly ICacheManager _cacheManager;
        private readonly IEmailManager _emailManager;
        private readonly IWebMembership _webMembership;
        private readonly IAddressBuilder _addressBuilder;
        private readonly IAccountBuilder _accountBuilder;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public AccountService(IDbContext dbContext,
                              IRepository<Account> accountRepository,
                              IRepository<Profile> profileRepository,
                              IRepository<Address> addressRepository,
                              IRepository<RewardPointHistory> rewardPointRepository,
                              IRepository<ActivityLog> activityLogRepository,
                              IRepository<Country> countryRepository,
                              IRepository<USState> usStateRepository,
                              IRepository<CartItem> cartItemRepository,
                              IRepository<Subscriber> subscriberRepository,
                              IRepository<UserBehaviour> userBehaviourRepository,
                              IRepository<EmailMessage> emailMessageRepository,
                              ICacheManager cacheManager,
                              IEmailManager emailManager,
                              ILogBuilder logBuilder,
                              IWebMembership webMembership,
                              IAddressBuilder addressBuilder,
                              IAccountBuilder accountBuilder)
        {
            _dbContext = dbContext;
            _accountRepository = accountRepository;
            _profileRepository = profileRepository;
            _addressRepository = addressRepository;
            _rewardPointRepository = rewardPointRepository;
            _activityLogRepository = activityLogRepository;
            _countryRepository = countryRepository;
            _usStateRepository = usStateRepository;
            _cartItemRepository = cartItemRepository;
            _subscriberRepository = subscriberRepository;
            _userBehaviourRepository = userBehaviourRepository;
            _emailMessageRepository = emailMessageRepository;
            _cacheManager = cacheManager;
            _emailManager = emailManager;
            _webMembership = webMembership;
            _addressBuilder = addressBuilder;
            _accountBuilder = accountBuilder;
            _logger = logBuilder.CreateLogger(GetType().FullName);
        }

        #endregion

        #region Return

        public Profile GetProfileBySystemName(string systemName)
        {
            return _profileRepository.Table.Where(x => x.SystemName == systemName).FirstOrDefault();
        }

        public PagedList<RewardPointHistory> GetPagedRewardPointHistory(
            int pageIndex = 0,
            int pageSize = 2147483647,
            int? accountId = null)
        {
            var query = _rewardPointRepository.Table;
            
            if (accountId.HasValue)
                query = query.Where(x => x.AccountId == accountId.Value);
            
            int totalRecords = query.Count();

            query = query.OrderByDescending(x => x.CreatedOnDate);
            
            query = query.Skip(pageIndex * pageSize).Take(pageSize);

            var list = query.ToList();

            return new PagedList<RewardPointHistory>(list, pageIndex, pageSize, totalRecords);
        }

        public string GetContactNumberIfOptedToDisplay(int accountId)
        {
            var number = _accountRepository.TableNoTracking
                .Where(x => x.Id == accountId)
                .Where(x => x.DisplayContactNumberInDespatch == true)
                .Select(x => x.ContactNumber)
                .FirstOrDefault();

            return number;
        }

        public bool GetDisplayContactNumberInDespatchStatus(int accountId)
        {
            var account = _accountRepository.Return(accountId);
            if (account != null) return account.DisplayContactNumberInDespatch;

            return false;
        }

        public int GetAccountIdByProfileId(int profileId)
        {
            return _accountRepository.Table.Where(x => x.ProfileId == profileId).Select(x => x.Id).FirstOrDefault();
        }

        public PagedList<Subscriber> GetSubscriberLoadPaged(
            int pageIndex = 0,
            int pageSize = 2147483647,
            string email = null,
            bool? isActive = null,
            SubscriberSortingType orderBy = SubscriberSortingType.IdAsc)
        {
            var pEmail = GetParameter("Email", email);
            var pIsActive = GetParameter("IsActive", isActive);
            var pOrderBy = GetParameter("OrderBy", (int)orderBy, true);
            var pPageIndex = GetParameter("PageIndex", pageIndex, true);
            var pPageSize = GetParameter("PageSize", pageSize);
            var pTotalRecords = GetParameterIntegerOutput("TotalRecords");
            
            var items = _dbContext.ExecuteStoredProcedureList<Subscriber>(
                "Subscriber_LoadPaged",
                pEmail,
                pIsActive,
                pOrderBy,
                pPageIndex,
                pPageSize,
                pTotalRecords);

            //return accounts
            int totalRecords = (pTotalRecords.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecords.Value) : 0;

            return new PagedList<Subscriber>(items, pageIndex, pageSize, totalRecords);
        }

        public PagedList<AccountOverviewModel> GetPagedAccountOverviewModels(
            int pageIndex = 0,
            int pageSize = 2147483647,
            IList<int> accountIds = null,
            string name = null,            
            string email = null,
            string contactNumber = null,
            string dob = null,
            AccountSortingType orderBy = AccountSortingType.IdAsc)
        {
            var list = GetAccountLoadPaged(
                pageIndex,
                pageSize,
                accountIds,
                name,
                email,
                contactNumber,
                dob,
                orderBy);

            var items = new Collection<AccountOverviewModel>();

            for (int i = 0; i < list.Items.Count; i++)
            {
                var account = BuildAccountOverviewModel(list.Items[i]);
                items.Add(account);
            }

            return new PagedList<AccountOverviewModel>(items, pageIndex, pageSize, list.TotalCount);
        }

        public PagedList<Account> GetAccountLoadPaged(
            int pageIndex = 0,
            int pageSize = 2147483647,
            IList<int> accountIds = null,
            string name= null,
            string email = null,
            string contactNumber = null,
            string dob = null,
            AccountSortingType orderBy = AccountSortingType.IdAsc)
        {
            var pName = GetParameter("Name", name);            
            var pEmail = GetParameter("Email", email);
            var pContactNumber = GetParameter("ContactNumber", contactNumber);
            var pDOB = GetParameter("DOB", dob);
            var pOrderBy = GetParameter("OrderBy", (int)orderBy, true);
            var pPageIndex = GetParameter("PageIndex", pageIndex, true);
            var pPageSize = GetParameter("PageSize", pageSize);
            var pTotalRecords = GetParameterIntegerOutput("TotalRecords");

            if (accountIds != null && accountIds.Contains(0))
                accountIds.Remove(0);
            string commaSeparatedUserIds = accountIds == null ? "" : string.Join(",", accountIds);
            var pAccountIds = GetParameter("AccountIds", commaSeparatedUserIds);

            var accounts = _dbContext.ExecuteStoredProcedureList<Account>(
                    "Account_LoadPaged",
                    pAccountIds,
                    pName,
                    pEmail,
                    pContactNumber,
                    pDOB,
                    pOrderBy,
                    pPageIndex,
                    pPageSize,
                    pTotalRecords);

            //return accounts
            int totalRecords = (pTotalRecords.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecords.Value) : 0;

            return new PagedList<Account>(accounts, pageIndex, pageSize, totalRecords);
        }
        
        public string[] GetRolesByUsername(string username)
        {
            return _webMembership.GetRolesForUser(username);
        }

        public DateTime? GetAccountCreationDateByUsername(string username)
        {
            var member = _webMembership.GetUser(username);
            
            if (member != null) return member.CreationDate;
               
            return null;
        }

        public int GetProfileIdByUsername(string username)
        {
            string key = string.Format(CacheKey.PROFILE_BY_USERNAME_KEY, username);

            var profileId = _cacheManager.GetWithExpiry(key, () =>
            {
                return _profileRepository.Table.Where(p => p.Username == username).Select(p => p.Id).FirstOrDefault();
            }, expiredEndOfDay: true);

            return profileId;            
        }

        public Account GetAccountById(int accountId)
        {
            var account = _accountRepository.Return(accountId);
            if (account == null)
            {
                _logger.InsertLog(LogLevel.Error, "Account could not be found. Account ID={{{0}}}", accountId);
                throw new ApolloException("Account could not be found. Account ID={{{0}}}", accountId);
            }

            var profile = _profileRepository.Return(account.ProfileId);
            if (profile == null)
            {
                _logger.InsertLog(LogLevel.Error, "Profile could not be found. Profile ID={{{0}}}", account.ProfileId);
                throw new ApolloException("Profile could not be found. Profile ID={{{0}}}", account.ProfileId);
            }

            account.Username = profile.Username;
            account.LastActvitityDate = profile.LastActivityDate;

            if (!profile.IsAnonymous)
                account = _accountBuilder.Build(account);

            return account;
        }

        public Account GetAccountByUsername(string username)
        {
            var result = Retry.Do(delegate ()
            {
                Profile profile = _profileRepository.Table.Where(p => p.Username == username).FirstOrDefault();
                if (profile == null)
                {
                    _logger.InsertLog(LogLevel.Error, "Profile could not be found. Username={{{0}}}", username);
                    throw new ApolloException("Profile could not be found. Username={{{0}}}", username);
                }

                if (profile.IsAnonymous)
                    return new Account { ProfileId = profile.Id, Username = profile.Username, LastActvitityDate = profile.LastActivityDate };

                var account = _accountRepository.Table.Where(x => x.ProfileId == profile.Id).FirstOrDefault();
                if (account == null)
                {
                    _logger.InsertLog(LogLevel.Error, "Account could not be found. Profile ID={{{0}}}", profile.Id);
                    throw new ApolloException("Account could not be found. Profile ID={{{0}}}", profile.Id);
                }

                account.Username = profile.Username;
                account.LastActvitityDate = profile.LastActivityDate;
                account = _accountBuilder.Build(account);

                return account;

            }, TimeSpan.FromSeconds(3));

            return result;
        }

        public Account GetAccountByProfileId(int profileId)
        {
            Profile profile = _profileRepository.Return(profileId);

            if (profile == null)
            {
                _logger.InsertLog(LogLevel.Error, "Profile could not be found. Profile ID={{{0}}}", profileId);
                throw new ApolloException("Profile could not be found. Profile ID={{{0}}}", profileId);
            }

            if (profile.IsAnonymous)
                return new Account { ProfileId = profile.Id, Username = profile.Username, LastActvitityDate = profile.LastActivityDate };

            var account = _accountRepository.Table.Where(x => x.ProfileId == profile.Id).FirstOrDefault();
            if (account == null)
            {
                _logger.InsertLog(LogLevel.Error, "Account could not be found. Profile ID={{{0}}}", profile.Id);
                throw new ApolloException("Account could not be found. Profile ID={{{0}}}", profile.Id);
            }

            account.Username = profile.Username;
            account.LastActvitityDate = profile.LastActivityDate;

            account = _accountBuilder.Build(account);
            
            return account;
        }

        public Profile GetProfileById(int profileId)
        {
            Profile profile = _profileRepository.Return(profileId);
            return profile;
        }

        public Profile GetProfileByUsername(string username)
        {
            Profile profile = _profileRepository.TableNoTracking
                .Where(x => x.Username.ToLower() == username.ToLower())
                .FirstOrDefault();
            return profile;
        }

        public Address GetAddressById(int id)
        {
            Address address = _addressRepository.Return(id);
            if (address != null) address = _addressBuilder.Build(address);            
            return address;
        }
        
        public Address GetBillingAddressByAccountId(int accountId)
        {
            var item = _addressRepository.TableNoTracking
                .Where(a => a.AccountId == accountId && a.IsBilling == true)
                .FirstOrDefault();
            if (item != null ) item = _addressBuilder.Build(item);
            return item;
        }

        public Address GetShippingAddressByAccountId(int accountId)
        {
            var item = _addressRepository.TableNoTracking
                .Where(a => a.AccountId == accountId && a.IsShipping == true)
                .FirstOrDefault();
            if (item != null) item = _addressBuilder.Build(item);
            return item;
        }

        public Address GetShippingAddressByProfileId(int profileId)
        {            
            var item = _addressRepository.TableNoTracking
                .Join(_accountRepository.Table, ad => ad.AccountId, ac => ac.Id, (ad, ac) => new { ad, ac })
                .Where(x => x.ac.ProfileId == profileId && x.ad.IsShipping == true)
                .Select(x => x.ad)
                .DefaultIfEmpty()
                .FirstOrDefault();

            if (item != null) item = _addressBuilder.Build(item);
            return item;
        }

        public IList<Address> GetAddressesByAccountId(int accountId)
        {
            var list = _addressRepository.TableNoTracking
                .Where(a => a.AccountId == accountId)
                .OrderByDescending(a => a.UpdatedOnDate)
                .ToList();

            for(int i = 0; i < list.Count; i++)            
                list[i] = _addressBuilder.Build(list[i]);
            
            return list;
        }

        public IList<Address> GetAddressesByProfileId(int profileId)
        {
            var accountId = GetAccountIdByProfileId(profileId);

            if (accountId != 0)
            {
                var list = _addressRepository.TableNoTracking
                    .Where(a => a.AccountId == accountId)
                    .OrderByDescending(a => a.UpdatedOnDate)
                    .ToList();

                for (int i = 0; i < list.Count; i++)
                    list[i] = _addressBuilder.Build(list[i]);

                return list;
            }

            return new List<Address>();
        }

        public AccountOverviewModel GetAccountOverviewModelByProfileId(int profileId)
        {
            var account = _accountRepository.TableNoTracking
                .Where(a => a.ProfileId == profileId)                
                .FirstOrDefault();

            if (account != null)
            {
                return BuildAccountOverviewModel(account);
            }

            return null;
        }

        public int GetProfileId(string username, bool isAuthenticated, bool ignoreAuthenticationType = true)
        {
            int profileId = 0;

            var query = _profileRepository.TableNoTracking.Where(x => x.Username == username);

            if (!ignoreAuthenticationType)            
                query = query.Where(x => x.IsAnonymous == !isAuthenticated);

            profileId = query.Select(x => x.Id).FirstOrDefault();

            if (profileId == 0)
                profileId = InsertProfile(username, isAuthenticated);
            
            return profileId;
        }

        public string GetEmailByProfileId(int profileId)
        {
            if (profileId <= 0) return null;
            return _accountRepository.Table
                .Where(x => x.ProfileId == profileId)
                .Select(x => x.Email).FirstOrDefault();
        }
        
        public string GetNameByProfileId(int profileId)
        {
            if (profileId <= 0) return null;
            return _accountRepository.Table
                .Where(x => x.ProfileId == profileId)
                .Select(x => x.Name)
                .FirstOrDefault();
        }

        public int GetLoyaltyPointsBalanceByProfileId(int profileId)
        {
            var accountId = GetAccountIdByProfileId(profileId);
            return GetLoyaltyPointsBalanceByAccountId(accountId);
        }

        public int GetLoyaltyPointsBalanceByAccountId(int accountId)
        {
            int result = 0;
            var lastBalance = _rewardPointRepository.Table
                .Where(x => x.AccountId == accountId)
                .OrderByDescending(x => x.CreatedOnDate)
                .ThenByDescending(x => x.Id)
                .FirstOrDefault();

            if (lastBalance != null)
                result = lastBalance.PointsBalance;

            return result;
        }
        
        public bool GetAnonymousStatusByProfileId(int profileId)
        {
            return _profileRepository.Table.Where(p => p.Id == profileId).Select(p => p.IsAnonymous).FirstOrDefault();
        }
        
        public DateTime GetLastActivityDateByProfileId(int profileId)
        {
            return _profileRepository.Table.Where(p => p.Id == profileId).Select(p => p.LastActivityDate).FirstOrDefault();
        }

        public string[] GetAllRoles()
        {
            return _webMembership.GetAllRoles();
        }

        #endregion

        #region Update

        public void UpdateAccount(Account account)
        {
            _accountRepository.Update(account);
        }

        public void UpdateProfileLastActivityDate(int profileId, DateTime lastActivityDate)
        {
            Profile profile = _profileRepository.Table.Where(p => p.Id == profileId).FirstOrDefault();

            if (profile != null)
            {
                profile.LastActivityDate = lastActivityDate;
                _profileRepository.Update(profile);
            }
        }

        public void UpdateProfileUsernameById(int profileId, string username)
        {
            Profile profile = _profileRepository.Return(profileId);

            if (profile != null)
            {
                profile.Username = username;
                _profileRepository.Update(profile);
            }
        }

        public void UpdatePrimaryBillingAddress(int addressId, int accountId = 0, int profileId = 0)
        {
            if (addressId == 0) return;
            if (accountId == 0 && profileId == 0) return;

            List<Address> list = null;

            if (accountId != 0)            
                list = _addressRepository.TableNoTracking.Where(a => a.AccountId == accountId).ToList();
            
            if (list == null && profileId != 0)
            {
                var accountIdByProfileId = GetAccountIdByProfileId(profileId);
                list = _addressRepository.TableNoTracking.Where(a => a.AccountId == accountIdByProfileId).ToList();
            }
                
            if (list != null)
            {
                foreach (var address in list)
                {
                    address.IsBilling = (address.Id == addressId);
                    _addressRepository.Update(address);
                }
            }
        }

        public void UpdatePrimaryShippingAddress(int addressId, int accountId = 0, int profileId = 0)
        {
            if (addressId == 0) return;
            if (accountId == 0 && profileId == 0) return;

            List<Address> list = null;

            if (accountId != 0)
                list = _addressRepository.TableNoTracking.Where(a => a.AccountId == accountId).ToList();

            if (list == null && profileId != 0)
            {
                var accountIdByProfileId = GetAccountIdByProfileId(profileId);
                list = _addressRepository.TableNoTracking.Where(a => a.AccountId == accountIdByProfileId).ToList();
            }

            if (list != null)
            {
                foreach (var address in list)
                {
                    address.IsShipping = (address.Id == addressId);
                    _addressRepository.Update(address);
                }
            }
        }
        
        public void UpdateSubscribersStatus(IList<int> subscriberIds, bool isActive)
        {
            foreach (var id in subscriberIds)
            {
                var subscriber = _subscriberRepository.Return(id);
                if (subscriber != null)
                {
                    subscriber.IsActive = isActive;
                    _subscriberRepository.Update(subscriber);
                }
            }
        }

        public void UpdateAddress(Address address)
        {
            var oldAdress = _addressRepository.Return(address.Id);

            if (oldAdress != null)
            {
                address.CreatedOnDate = oldAdress.CreatedOnDate;
                _addressRepository.Update(address);
            }            
        }

        #endregion

        #region Create

        public int InsertAddress(Address address)
        {
            return _addressRepository.Create(address);
        }

        public int InsertProfile(string username, bool isAuthenticated)
        {
            Profile profile = new Profile
            {
                Username = username,
                LastActivityDate = DateTime.Now,
                IsAnonymous = !isAuthenticated
            };

            int id = _profileRepository.Create(profile);
            return id;
        }
        
        public int InsertSubscriber(string email)
        {
            Subscriber subscriber = new Subscriber { Email = email, IsActive = true };
            return _subscriberRepository.Create(subscriber);
        }

        public int InsertAccount(Account account)
        {
            return _accountRepository.Create(account);
        }

        /// <summary>
        /// Add reward point history
        /// </summary>
        /// <param name="accountId">Target account ID</param>
        /// <param name="points">Added points</param>
        /// <param name="pointsBalance">Points balance to change. If points balance is null, only added points is considered. If points balance is not null, added points will be ignored.</param>
        /// <param name="message">Message</param>
        /// <param name="orderId">Related order ID</param>
        /// <param name="usedPoints">Deducted points</param>
        /// <returns>Reward point history ID</returns>
        public int InsertRewardPointHistory(int accountId, int? points = null, int? pointsBalance = null, string message = null, int? orderId = null, int? usedPoints = null)
        {
            if (accountId <= 0) return 0;

            var newPointsBalance = 0;
            
            if (pointsBalance.HasValue == false)
                newPointsBalance = GetLoyaltyPointsBalanceByAccountId(accountId) + (points ?? 0);
            else
                newPointsBalance = pointsBalance.Value;

            var allocatedPoints = 0;
            if (usedPoints.HasValue)
            {
                if (usedPoints.Value == 0)
                    usedPoints = null;
                else
                    allocatedPoints = usedPoints.Value;
            }

            newPointsBalance = newPointsBalance - allocatedPoints;

            var history = new RewardPointHistory
            {
                AccountId = accountId,
                Points = points,
                PointsBalance = newPointsBalance,
                UsedPoints = usedPoints,
                Message = message,
                UsedWithOrderId = orderId
            };

            return _rewardPointRepository.Create(history);
        }

        #endregion

        #region Delete

        public int DeleteGuestCustomers(DateTime? lastActivityFromUtc, DateTime? lastActivityToUtc)
        {
            var pLastActivityFromUtc = GetParameter("LastActivityFromUtc", lastActivityFromUtc);
            var plastActivityToUtc = GetParameter("LastActivityToUtc", lastActivityToUtc);
            var pTotalRecordsDeleted = GetParameterIntegerOutput("TotalRecordsDeleted");
            
            _dbContext.ExecuteSqlCommand(
                "EXEC [DeleteGuests] @LastActivityFromUtc, @LastActivityToUtc, @TotalRecordsDeleted OUTPUT",
                pLastActivityFromUtc,
                plastActivityToUtc,
                pTotalRecordsDeleted);

            int totalRecordsDeleted = (pTotalRecordsDeleted.Value != DBNull.Value) ? Convert.ToInt32(pTotalRecordsDeleted.Value) : 0;
            return totalRecordsDeleted;
        }

        public void DeleteAddressByAddressId(int addressId)
        {
            _addressRepository.Delete(addressId);
        }

        #endregion

        #region Command

        public Profile GenerateVisitorProfileByUsername(string username, bool isAuthenticated, bool ignoreAuthenticationType = true)
        {
            int profileId = GetProfileId(username, isAuthenticated, ignoreAuthenticationType);
            return GetProfileById(profileId);
        }

        public void ProcessUsernameUpdate(string username, string oldUsername)
        {
            _webMembership.UpdateUsername(oldUsername, username);

            var profile = _profileRepository.Table.Where(p => p.Username == oldUsername).FirstOrDefault();
            if (profile != null)
            {
                profile.Username = username;
                _profileRepository.Update(profile);
            }

            var account = _accountRepository.Table.Where(a => a.Email == oldUsername).FirstOrDefault();
            if (account != null)
            {
                account.Email = username;
                _accountRepository.Update(account);
            }

            var subscriber = _subscriberRepository.Table.Where(s => s.Email == oldUsername).FirstOrDefault();
            if (subscriber != null)
            {
                subscriber.Email = username;
                _subscriberRepository.Update(subscriber);
            }
        }

        public RegistrationResult ProcessRegistrationWithExternalLogin(Account account, string loginProvider, string providerKey, bool sendEmailFlag = false)
        {
            if (account == null) throw new ArgumentNullException("account");
            if (string.IsNullOrEmpty(loginProvider)) throw new ArgumentNullException("loginProvider");
            if (string.IsNullOrEmpty(providerKey)) throw new ArgumentNullException("providerKey");

            var result = RegisterNewAccount(account, () =>
            {
                var identityUserManager = ThrowIfUnableToCast<IIdentityUserManager>(_webMembership);
                identityUserManager.CreateUserAndUserLogin(account.Username, account.Email, loginProvider, providerKey);
            });
            if (result != null) return result;

            if (sendEmailFlag) _emailManager.SendAccountRegistrationEmail(account.Email, account.Name);
            
            return new RegistrationResult
            {
                UserId = account.Id,
                ProfileId = account.ProfileId
            };
        }

        public RegistrationResult ProcessRegistration(Account account, string password, bool sendEmailFlag = false, bool sendPasswordEmailFlag = false)
        {
            if (account == null) throw new ArgumentNullException("account");
            if (string.IsNullOrEmpty(password)) throw new ArgumentNullException("password");

            var result = RegisterNewAccount(account, () =>
            {
                _webMembership.CreateUser(account.Username, password, account.Email);
            });
            if (result != null) return result;

            if (sendEmailFlag) _emailManager.SendAccountRegistrationEmail(account.Email, account.Name);

            // Send new password email
            if (sendPasswordEmailFlag) _emailManager.SendAccountRegistrationWithPasswordEmail(account.Email, account.Name, password);

            return new RegistrationResult
            {
                UserId = account.Id,
                ProfileId = account.ProfileId
            };
        }
        
        public bool IdentityUserExistsById(string userId)
        {
            var identityUserManager = ThrowIfUnableToCast<IIdentityUserManager>(_webMembership);
            return identityUserManager.UserExistsById(userId);
        }

        public bool SupportsUserSecurityStamp()
        {
            var identityUserManager = ThrowIfUnableToCast<IIdentityUserManager>(_webMembership);
            return identityUserManager.SupportsUserSecurityStamp;
        }

        public string GetSecurityStamp(string userId)
        {
            var identityUserManager = ThrowIfUnableToCast<IIdentityUserManager>(_webMembership);
            return identityUserManager.GetSecurityStamp(userId);
        }

        public ClaimsIdentity CreateIdentity(string userId)
        {
            var identityUserManager = ThrowIfUnableToCast<IIdentityUserManager>(_webMembership);
            return identityUserManager.CreateIdentityById(userId);
        }

        public bool AddUserLogin(string userId, string loginProvider, string providerKey)
        {
            var identityUserManager = ThrowIfUnableToCast<IIdentityUserManager>(_webMembership);
            return identityUserManager.AddUserLogin(userId, loginProvider, providerKey);
        }

        public IdentityLoginResult ValidateIdentityUser(string email, string loginProvider, string providerKey)
        {
            var identityUserManager = ThrowIfUnableToCast<IIdentityUserManager>(_webMembership);

            var user = identityUserManager.Find(new UserLoginInfo(loginProvider, providerKey));
            if (user == null)
            {
                var foundAccountAndProfile = ValidateAccountAndProfile(email);
                if (foundAccountAndProfile == null)
                {
                    var addResult = identityUserManager.AddLogin(email, loginProvider, providerKey);
                    if (addResult.Succeeded)
                    {
                        user = identityUserManager.FindByEmail(email);
                    }
                    else
                    {
                        return new IdentityLoginResult { CustomerLoginResults = CustomerLoginResults.Error };
                    }                    
                }
                else
                {
                    return new IdentityLoginResult { CustomerLoginResults = CustomerLoginResults.MemberNotExists };
                }                
            }
            
            if (identityUserManager.IsLockedOut(user.Id))
                return new IdentityLoginResult { CustomerLoginResults = CustomerLoginResults.IsLockedOut };

            var result = ValidateAccountAndProfile(user.Email);
            if (result != null) return result;
            
            var userIdentity = identityUserManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);

            return new IdentityLoginResult
            {
                ClaimsIdentity = userIdentity,
                CustomerLoginResults = CustomerLoginResults.Successful
            };
        }

        public IdentityLoginResult ValidateIdentityUser(string username, string password, bool shouldLockout)
        {
            var identityUserManager = ThrowIfUnableToCast<IIdentityUserManager>(_webMembership);

            var user = identityUserManager.FindByName(username);
            if (user == null)
                return new IdentityLoginResult { CustomerLoginResults = CustomerLoginResults.MemberNotExists };

            if (identityUserManager.IsLockedOut(user.Id))
                return new IdentityLoginResult { CustomerLoginResults = CustomerLoginResults.IsLockedOut };

            var result = ValidateAccountAndProfile(username);
            if (result != null) return result;

            if (identityUserManager.CheckPassword(user, password))
            {
                identityUserManager.ResetAccessFailedCount(user.Id);

                var userIdentity = identityUserManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);

                return new IdentityLoginResult
                {
                    ClaimsIdentity = userIdentity,
                    CustomerLoginResults = CustomerLoginResults.Successful
                };
            }

            if (shouldLockout)
            {
                // If lockout is requested, increment access failed count which might lock out the user
                identityUserManager.AccessFailed(user.Id);
                if (identityUserManager.IsLockedOut(user.Id))
                {
                    return new IdentityLoginResult { CustomerLoginResults = CustomerLoginResults.IsLockedOut };
                }
            }

            return new IdentityLoginResult { CustomerLoginResults = CustomerLoginResults.WrongPassword };
        }
        
        public CustomerLoginResults ValidateUser(string username, string password, bool isRestrictedArea = false)
        {
            if (string.IsNullOrEmpty(username)) throw new ArgumentNullException("username");
            if (string.IsNullOrEmpty(password)) throw new ArgumentNullException("password");

            try
            {
                var member = _webMembership.GetUser(username);
                if (member == null) return CustomerLoginResults.MemberNotExists;
                if (member.IsLockedOut) return CustomerLoginResults.IsLockedOut;
                if (member.IsApproved == false) return CustomerLoginResults.NotApproved;
                if (isRestrictedArea && _webMembership.GetRolesForUser(username).Length == 0) return CustomerLoginResults.NotPermitted;
                if (_webMembership.ValidateUser(username, password) == false) return CustomerLoginResults.WrongPassword;

                var profile = _profileRepository.Table.Where(p => p.Username == username).FirstOrDefault();
                if (profile == null) return CustomerLoginResults.ProfileNotExists;
                profile.LastActivityDate = DateTime.Now;
                _profileRepository.Update(profile);

                var account = _accountRepository.Table.Where(x => x.ProfileId == profile.Id).FirstOrDefault();
                if (account == null) return CustomerLoginResults.AccountNotExists;
                
                return CustomerLoginResults.Successful;
            }
            catch (Exception ex)
            {
                _logger.InsertLog(LogLevel.Error, string.Format("Failed to validate user. Username={{{0}}}", username), ex);
                return CustomerLoginResults.Error;
            }            
        }
        
        public string ResetPassword(string username)
        {
            return _webMembership.ResetPassword(username);
        }

        public PasswordResetResults ProcessPasswordReset(string username, bool sendEmailFlag)
        {
            var member = _webMembership.GetUser(username.ToLower());
            if (member == null) return PasswordResetResults.MemberNotExist;
            if (member.IsLockedOut) return PasswordResetResults.IsLockedOut;

            Account account = null;

            try
            {
                account = GetAccountByUsername(username);
                if (account == null) return PasswordResetResults.AccountNotExist;
            }
            catch (Exception ex)
            {
                _logger.InsertLog(LogLevel.Error, string.Format("Failed to get account by username. Username={{{0}}}", username), ex);
                return PasswordResetResults.AccountNotExist;
            }

            try
            {    
                string newPassword = ResetPassword(username);

                if (sendEmailFlag)
                {
                    _emailManager.SendPasswordRetrievalEmail(account.Email, account.Name, newPassword);
                }

                return PasswordResetResults.Successful;
            }
            catch (Exception ex)
            {
                _logger.InsertLog(LogLevel.Error, string.Format("Failed to reset password. Username={{{0}}}, Send Email Flag={{{1}}}", username, sendEmailFlag), ex);
                return PasswordResetResults.Error;
            }
        }

        public bool ChangePassword(string username, string password, bool sendPassEmailFlag = false)
        {
            var identityUserManager = ThrowIfUnableToCast<IIdentityUserManager>(_webMembership);
            var result = identityUserManager.RemovePassword(username);

            if (result.Succeeded)
            {
                var user = identityUserManager.FindByName(username);
                var addPasswordResult = identityUserManager.AddPassword(user.Id, password);

                if (addPasswordResult && sendPassEmailFlag)
                {
                    var account = GetAccountByUsername(username);
                    _emailManager.SendPasswordRetrievalEmail(account.Email, account.Name, password);
                }

                return addPasswordResult;
            }

            return false;
        }

        public bool ChangePassword(string username, string oldPassword, string newPassword, bool sendPassEmailFlag = false)
        {
            if (_webMembership.ValidateUser(username, oldPassword))
            {
                _webMembership.ChangePassword(username, newPassword);
                
                if (sendPassEmailFlag)
                {
                    var account = GetAccountByUsername(username);
                    _emailManager.SendPasswordRetrievalEmail(account.Email, account.Name, newPassword);
                }

                return true;
            }

            return false;
        }

        public bool SetPassword(string username, string password, bool sendPassEmailFlag = false)
        {
            var identityUserManager = ThrowIfUnableToCast<IIdentityUserManager>(_webMembership);

            var user = identityUserManager.FindByName(username);

            if (user != null)
            {
                if (identityUserManager.AddPassword(user.Id, password))
                {
                    if (sendPassEmailFlag)
                    {
                        var account = GetAccountByUsername(username);
                        _emailManager.SendPasswordRetrievalEmail(account.Email, account.Name, password);
                    }

                    return true;
                }
            }

            return false;
        }

        public bool HasPassword(string username)
        {
            var identityUserManager = ThrowIfUnableToCast<IIdentityUserManager>(_webMembership);
            return identityUserManager.HasPassword(username);
        }

        public void EnableUser(int profileId)
        {
            var member = GetWebMembershipUser(profileId);
            _webMembership.ApproveUser(member.Username);
        }

        public void DisableUser(int profileId)
        {
            var member = GetWebMembershipUser(profileId);
            _webMembership.DisapproveUser(member.Username);
        }

        public void UnlockUser(int profileId)
        {
            var member = GetWebMembershipUser(profileId);
            _webMembership.UnlockUser(member.Username);
        }
        
        public IDictionary<string, string> GetLogins(string username)
        {
            var identityUserManager = ThrowIfUnableToCast<IIdentityUserManager>(_webMembership);
            var logins = identityUserManager.GetLogins(username);

            return logins.ToDictionary(x => x.LoginProvider, x => x.ProviderKey);
        }

        public ClaimsIdentity RemoveLoginAndReturnIdentity(string username, string loginProvider, string providerKey)
        {
            var identityUserManager = ThrowIfUnableToCast<IIdentityUserManager>(_webMembership);
            var result = identityUserManager.RemoveLogin(username, loginProvider, providerKey);

            if (result.Succeeded)
            {
                return identityUserManager.CreateIdentityByName(username);
            }

            return null;
        }

        public AccountUpdateResults ProcessAccountUpdate(Account account, int? loyaltyPoints = null, bool sendEmailFlag = false)
        {
            if (account == null) throw new ArgumentNullException("account");

            var member = _webMembership.GetUser(account.Username);
            if (member == null)
            {
                _logger.InsertLog(LogLevel.Error, "Membership could not be found with this username. Username={{{0}}}", account.Username);
                return AccountUpdateResults.MemberNotExist;
            }
            
            // Update email
            if (member.Email.ToLower().CompareTo(account.Email.ToLower()) != 0)
            {
                string newEmail = account.Email.ToLower();
                string oldEmail = member.Email.ToLower();
                bool userFound = _webMembership.UserExists(newEmail);

                if (userFound)
                    return AccountUpdateResults.ExistingEmail;

                ProcessUsernameUpdate(newEmail, oldEmail);

                account.Username = newEmail;
                member = _webMembership.GetUser(account.Username);
            }

            // Update account
            UpdateAccount(account);

            // Update loyalty points
            if (loyaltyPoints.HasValue)
            {
                var pointsBalance = GetLoyaltyPointsBalanceByAccountId(account.Id);
                if (pointsBalance != loyaltyPoints.Value)
                {
                    InsertRewardPointHistory(account.Id, pointsBalance: loyaltyPoints.Value, message: "Point balance adjusted.");
                }
            }

            // Send email to customer
            if (sendEmailFlag)
            {
                _emailManager.SendNewUsernameEmail(account.Email, account.Name);
            }

            // Set roles
            _webMembership.RemoveUserFromRoles(member.Username, _webMembership.GetRolesForUser(member.Username));

            for (int i = 0; i < account.Roles.Length; i++)
            {
                _webMembership.AddUserToRole(member.Username, account.Roles[i]);
            }
            
            return AccountUpdateResults.Successful;
        }

        public SubscriptionResults ProcessNewEmailSubscription(string email)
        {
            try
            {
                email = email.ToLower().Trim();
                var existingSubscriber = _subscriberRepository.Table.Where(x => x.Email.ToLower() == email).FirstOrDefault();

                if (existingSubscriber != null)
                {
                    existingSubscriber.IsActive = true;
                    _subscriberRepository.Update(existingSubscriber);
                }
                else
                {
                    var subscriber = new Subscriber
                    {
                        Email = email,
                        IsActive = true
                    };

                    _subscriberRepository.Create(subscriber);
                }

                return SubscriptionResults.Successful;
            }
            catch
            {
                return SubscriptionResults.Error;
            }
        }

        public SubscriptionResults ProcessEmailSubsciptionCancellation(int profileId)
        {
            try
            {
                var email = GetEmailByProfileId(profileId);                
                var existingSubscriber = _subscriberRepository.Table.Where(x => x.Email.ToLower() == email).FirstOrDefault();

                if (existingSubscriber != null)
                {
                    existingSubscriber.IsActive = false;
                    _subscriberRepository.Update(existingSubscriber);
                }
                
                return SubscriptionResults.Successful;
            }
            catch
            {
                return SubscriptionResults.Error;
            }
        }

        #endregion

        #region Private methods

        private TTarget ThrowIfUnableToCast<TTarget>(object fromObject) 
            where TTarget : class
        {
            if (fromObject is TTarget)
            {
                return fromObject as TTarget;
            }

            throw new InvalidCastException($"Unable to cast to type '{typeof(TTarget).FullName}'");
        }

        private WebMembershipUser GetWebMembershipUser(int profileId)
        {
            var email = GetEmailByProfileId(profileId);

            if (string.IsNullOrEmpty(email))
            {
                _logger.InsertLog(LogLevel.Error, string.Format("Email could not be found with this profile ID. Profile ID={{{0}}}", profileId));
                throw new ApolloException("Email could not be found with this profile ID. Profile ID={{{0}}}", profileId);
            }

            var member = _webMembership.GetUser(email);
            if (member == null)
            {
                _logger.InsertLog(LogLevel.Error, string.Format("Membership could not be found with this profile ID. Profile ID={{{0}}}", profileId));
                throw new ApolloException("Membership could not be found with this profile ID. Profile ID={{{0}}}", profileId);
            }

            return member;
        }

        private RegistrationResult RegisterNewAccount(Account account, Action createUser)
        {
            if (account == null) throw new ArgumentNullException("account");
            
            // Make sure there is no duplication
            var found = _webMembership.UserExists(account.Username);
            if (found) return new RegistrationResult { Message = "The email is already in use. Please choose another email." };

            // Invoke to create membership user
            createUser();
            
            if (account.Roles != null && account.Roles.Length > 0)
            {
                for (int i = 0; i < account.Roles.Length; i++)
                    _webMembership.AddUserToRole(account.Username, account.Roles[i]);
            }
            
            if (account.ProfileId == 0)
            {
                // Create new profile to generate new profile id
                var profileId = InsertProfile(account.Username, isAuthenticated: true);
                account.ProfileId = profileId;
            }
            else
            {
                // Change profile to non anonymous
                var profile = _profileRepository.Return(account.ProfileId);
                profile.Username = account.Email.ToLower();
                profile.IsAnonymous = false;
                profile.LastActivityDate = DateTime.Now;
                _profileRepository.Update(profile);
            }

            InsertAccount(account);

            InsertSubscriber(account.Email);

            return null;
        }

        private IdentityLoginResult ValidateAccountAndProfile(string username)
        {
            var profile = _profileRepository.Table.Where(p => p.Username == username).FirstOrDefault();
            if (profile == null)
                return new IdentityLoginResult { CustomerLoginResults = CustomerLoginResults.ProfileNotExists };

            profile.LastActivityDate = DateTime.Now;
            _profileRepository.Update(profile);

            var account = _accountRepository.Table.Where(x => x.ProfileId == profile.Id).FirstOrDefault();
            if (account == null) return new IdentityLoginResult { CustomerLoginResults = CustomerLoginResults.AccountNotExists };

            return null;
        }

        private AccountOverviewModel BuildAccountOverviewModel(Account account)
        {
            return new AccountOverviewModel
            {
                Id = account.Id,
                Name = account.Name,
                Email = account.Email,
                ContactNumber = account.ContactNumber,
                DOB = account.DOB,
                LastActivityDate = GetLastActivityDateByProfileId(account.ProfileId),
                Note = account.Note,
                ProfileId = account.ProfileId
            };
        }

        #endregion
    }
}