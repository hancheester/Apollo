using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class AccountMap : EntityTypeConfiguration<Account>
    {
        public AccountMap()
        {
            this.ToTable("Account");
            this.HasKey(a => a.Id);
            this.Ignore(a => a.IsLockedOut);
            this.Ignore(a => a.IsApproved);
            this.Ignore(a => a.IsSubscribed);
            this.Ignore(a => a.Roles);
            this.Ignore(a => a.LastLoginDate);
            this.Ignore(a => a.LastActvitityDate);
            this.Ignore(a => a.CreationDate);
            this.Ignore(a => a.Username);
        }
    }
}
