using Apollo.Core.Services.Interfaces;
using System;
using System.Configuration;
using System.Web.Profile;

namespace Apollo.AdminStore.WebForm.Classes
{
    public class CustomProfileProvider : ProfileProvider
    {
        public IAccountService AccountService { get; set; }

        public CustomProfileProvider()
        {
        }

        public override int DeleteInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
        {
            throw new NotImplementedException();
        }

        public override int DeleteProfiles(string[] usernames)
        {
            throw new NotImplementedException();
        }

        public override int DeleteProfiles(ProfileInfoCollection profiles)
        {
            throw new NotImplementedException();
        }

        public override ProfileInfoCollection FindInactiveProfilesByUserName(ProfileAuthenticationOption authenticationOption, string usernameToMatch, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override ProfileInfoCollection FindProfilesByUserName(ProfileAuthenticationOption authenticationOption, string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override ProfileInfoCollection GetAllInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override ProfileInfoCollection GetAllProfiles(ProfileAuthenticationOption authenticationOption, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
        {
            throw new NotImplementedException();
        }

        private string _applicationName;
        public override string ApplicationName
        {
            get { return _applicationName; }
            set { _applicationName = value; }
        }

        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection collection)
        {
            string username = (string)context["UserName"];

            SettingsPropertyValueCollection newCollection = new SettingsPropertyValueCollection();
            foreach (SettingsProperty prop in collection)
            {
                SettingsPropertyValue pv = new SettingsPropertyValue(prop);

                switch (pv.Property.Name)
                {
                    case "ProfileId":
                        int profileId = AccountService.GetProfileIdByUsername(username);
                        pv.PropertyValue = profileId;
                        break;
                    case "FullName":
                        var account = AccountService.GetAccountByUsername(username);
                        pv.PropertyValue = account.Name;
                        break;
                    default:
                        break;
                }

                newCollection.Add(pv);
            }
            return newCollection;
        }

        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection)
        {
            throw new NotImplementedException();
        }
    }
}