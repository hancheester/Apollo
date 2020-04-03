using Apollo.Core.Model.Entity;
using Apollo.DataAccess;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Web.Security;

namespace Apollo.Core.Services.Accounts.SqlMembership
{
    public class SqlMembership : IWebMembership
    {
        private readonly IDbContext _dbContext;

        public SqlMembership(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public string[] GetAllRoles()
        {
            return Roles.GetAllRoles();
        }

        public void AddUserToRole(string username, string roleName)
        {
            Roles.AddUserToRole(username, roleName);
        }

        public string[] GetRolesForUser(string username)
        {
            return Roles.GetRolesForUser(username);
        }

        public void RemoveUserFromRoles(string username, string[] roles)
        {
            Roles.RemoveUserFromRoles(username, Roles.GetRolesForUser(username));
        }

        public async Task<WebMembershipUser> CreateUserAsync(string username, string password, string email)
        {
            return await Task.Run(() => CreateUser(username, password, email));            
        }

        public WebMembershipUser CreateUser(string username, string password, string email)
        {
            var newMember = Membership.CreateUser(username, password, email);
            newMember.IsApproved = true;

            return newMember.PrepareWebMembershipUser();
        }

        public WebMembershipUser GetUser(string username)
        {
            var member = Membership.GetUser(username);

            return member.PrepareWebMembershipUser();
        }

        public bool UserExists(string username)
        {
            MembershipUserCollection existingUsernames = Membership.FindUsersByName(username);
            return existingUsernames == null ? false : existingUsernames.Count > 0;
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            return await Task.FromResult(UserExists(username));
        }

        public void UpdateUsername(string username, string newUsername)
        {
            SqlParameter OldUsername = new SqlParameter
            {
                ParameterName = "OldUsername",
                Value = username,
                DbType = DbType.String
            };

            SqlParameter Username = new SqlParameter
            {
                ParameterName = "Username",
                Value = newUsername,
                DbType = DbType.String
            };

            _dbContext.ExecuteSqlCommand("UPDATE aspnet_Users SET UserName = @Username, LoweredUserName = @Username WHERE LoweredUserName = @OldUsername",
                OldUsername, Username);

            OldUsername = new SqlParameter
            {
                ParameterName = "OldUsername",
                Value = username,
                DbType = DbType.String
            };

            Username = new SqlParameter
            {
                ParameterName = "Username",
                Value = newUsername,
                DbType = DbType.String
            };

            _dbContext.ExecuteSqlCommand("UPDATE aspnet_Membership SET Email = @Username, LoweredEmail = @Username WHERE  LoweredEmail = @OldUsername",
                OldUsername, Username);            
        }

        public bool ValidateUser(string username, string password)
        {
            return Membership.ValidateUser(username, password);
        }

        public string ResetPassword(string username)
        {
            MembershipUser user = Membership.GetUser(username);
            string newPassword = string.Empty;

            if (user != null)
            {
                string resetPassword = user.ResetPassword();
                string randomGuid = Guid.NewGuid().ToString("N");

                newPassword = randomGuid.Substring(0, 8);

                user.ChangePassword(resetPassword, newPassword);
            }

            return newPassword;
        }

        public void ChangePassword(string username, string newPassword)
        {
            var member = Membership.GetUser(username);

            if (member != null)
            {
                string resetPwd = member.ResetPassword();
                member.ChangePassword(resetPwd, newPassword);
            }
        }

        public async Task<WebMembershipUser> GetUserAsync(string username)
        {
            return await Task.FromResult(GetUser(username));
        }        

        public async Task<bool> ValidateUserAsync(string username, string password)
        {
            return await Task.FromResult(ValidateUser(username, password));            
        }

        public async Task<string> ResetPasswordAsync(string username)
        {
            return await Task.FromResult(ResetPassword(username));
        }

        public void ChangePasswordAsync(string username, string newPassword)
        {
            Task.Run(() => ChangePassword(username, newPassword));
        }

        public void AddUserToRoleAsync(string username, string roleName)
        {
            Task.Run(() => AddUserToRole(username, roleName));
        }

        public async Task<string[]> GetRolesForUserAsync(string username)
        {
            return await Task.FromResult(GetRolesForUser(username));            
        }

        public void RemoveUserFromRolesAsync(string username, string[] roles)
        {
            Task.Run(() => RemoveUserFromRoles(username, roles));
        }

        public void UnlockUser(string username)
        {
            MembershipUser user = Membership.GetUser(username);
            user.UnlockUser();
        }

        public void DisapproveUser(string username)
        {
            MembershipUser user = Membership.GetUser(username);
            user.IsApproved = false;
            Membership.UpdateUser(user);
        }

        public void ApproveUser(string username)
        {
            MembershipUser user = Membership.GetUser(username);
            user.IsApproved = true;
            Membership.UpdateUser(user);
        }
    }
}
