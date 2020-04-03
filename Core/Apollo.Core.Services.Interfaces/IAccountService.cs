using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Model.OverviewModel;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Apollo.Core.Services.Interfaces
{
    public interface IAccountService
    {
        Profile GetProfileBySystemName(string systemName);        
        int GetProfileIdByUsername(string username);
        Account GetAccountByUsername(string username);
        Account GetAccountByProfileId(int profileId);
        Account GetAccountById(int accountId);
        AccountOverviewModel GetAccountOverviewModelByProfileId(int profileId);
        string[] GetRolesByUsername(string username);
        string[] GetAllRoles();        
        IList<Address> GetAddressesByAccountId(int accountId);        
        Address GetAddressById(int addressId);
        Address GetShippingAddressByAccountId(int accountId);
        Address GetShippingAddressByProfileId(int profileId);
        int GetLoyaltyPointsBalanceByAccountId(int accountId);        
        PagedList<AccountOverviewModel> GetPagedAccountOverviewModels(
            int pageIndex = 0,
            int pageSize = 2147483647,  //Int32.MaxValue
            IList<int> accountIds = null,
            string name = null,
            string email = null,
            string contactNumber = null,
            string dob = null,
            AccountSortingType orderBy = AccountSortingType.IdAsc);
        PagedList<RewardPointHistory> GetPagedRewardPointHistory(
            int pageIndex = 0,
            int pageSize = 2147483647,
            int? accountId = null);
        string GetEmailByProfileId(int profileId);
        string GetNameByProfileId(int profileId);
        int GetAccountIdByProfileId(int profileId);
        int GetLoyaltyPointsBalanceByProfileId(int profileId);
        int DeleteGuestCustomers(DateTime? lastActivityFromUtc, DateTime? lastActivityToUtc);
        void DeleteAddressByAddressId(int addressId);
        void UpdateSubscribersStatus(IList<int> subscriberIds, bool isActive);
        void UpdateAddress(Address address);
        int InsertAddress(Address address);        
        CustomerLoginResults ValidateUser(string username, string password, bool isRestrictedArea = false);
        IdentityLoginResult ValidateIdentityUser(string username, string password, bool shouldLockout);
        IdentityLoginResult ValidateIdentityUser(string email, string loginProvider, string providerKey);        
        PagedList<Subscriber> GetSubscriberLoadPaged(
            int pageIndex = 0,
            int pageSize = 2147483647,
            string email = null,
            bool? isActive = null,
            SubscriberSortingType orderBy = SubscriberSortingType.IdAsc);
        Profile GetProfileById(int profileId);
        bool GetAnonymousStatusByProfileId(int profileId);
        int InsertProfile(string username, bool isAuthenticated);
        void DisableUser(int profileId);
        void EnableUser(int profileId);
        void UnlockUser(int profileId);
        DateTime? GetAccountCreationDateByUsername(string username);
        IList<Address> GetAddressesByProfileId(int profileId);
        void UpdatePrimaryBillingAddress(int addressId, int accountId = 0, int profileId = 0);
        void UpdatePrimaryShippingAddress(int addressId, int accountId = 0, int profileId = 0);
        Address GetBillingAddressByAccountId(int accountId);
        
        string GetContactNumberIfOptedToDisplay(int accountId);
        Profile GetProfileByUsername(string username);
        Profile GenerateVisitorProfileByUsername(string username, bool isAuthenticated, bool ignoreAuthenticationType = true);
        int InsertRewardPointHistory(int accountId, int? points = null, int? pointsBalance = null, string message = null, int? orderId = null, int? usedPoints = null);
        bool IdentityUserExistsById(string userId);
        bool SupportsUserSecurityStamp();
        string GetSecurityStamp(string userId);
        ClaimsIdentity CreateIdentity(string userId);
        bool ChangePassword(string username, string oldPassword, string newPassword, bool sendPassEmailFlag = false);
        bool ChangePassword(string username, string password, bool sendPassEmailFlag = false);
        bool SetPassword(string username, string password, bool sendPassEmailFlag = false);
        bool HasPassword(string username);
        IDictionary<string, string> GetLogins(string username);
        ClaimsIdentity RemoveLoginAndReturnIdentity(string username, string loginProvider, string providerKey);
        AccountUpdateResults ProcessAccountUpdate(Account account, int? loyaltyPoints = null, bool sendEmailFlag = false);
        RegistrationResult ProcessRegistration(Account account, string password, bool sendEmailFlag = false, bool sendPasswordEmailFlag = false);
        RegistrationResult ProcessRegistrationWithExternalLogin(Account account, string loginProvider, string providerKey, bool sendEmailFlag = false);
        SubscriptionResults ProcessNewEmailSubscription(string email);
        PasswordResetResults ProcessPasswordReset(string email, bool sendEmailFlag);
        SubscriptionResults ProcessEmailSubsciptionCancellation(int profileId);
    }
}
