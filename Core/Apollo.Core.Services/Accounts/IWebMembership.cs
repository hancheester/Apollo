using Apollo.Core.Model.Entity;
using System.Threading.Tasks;

namespace Apollo.Core.Services.Accounts
{
    public interface IWebMembership
    {
        WebMembershipUser GetUser(string username);
        Task<WebMembershipUser> GetUserAsync(string username);

        bool UserExists(string username);
        Task<bool> UserExistsAsync(string username);
        
        WebMembershipUser CreateUser(string username, string password, string email);
        Task<WebMembershipUser> CreateUserAsync(string username, string password, string email);

        void UpdateUsername(string username, string newUsername);

        bool ValidateUser(string username, string password);
        Task<bool> ValidateUserAsync(string username, string password);

        string ResetPassword(string username);
        Task<string> ResetPasswordAsync(string username);

        void ChangePassword(string username, string newPassword);
        void ChangePasswordAsync(string username, string newPassword);

        string[] GetAllRoles();

        void AddUserToRole(string username, string roleName);
        void AddUserToRoleAsync(string username, string roleName);

        string[] GetRolesForUser(string username);
        Task<string[]> GetRolesForUserAsync(string username);

        void RemoveUserFromRoles(string username, string[] roles);
        void RemoveUserFromRolesAsync(string username, string[] roles);

        void UnlockUser(string username);

        void DisapproveUser(string username);
        void ApproveUser(string username);
    }
}
