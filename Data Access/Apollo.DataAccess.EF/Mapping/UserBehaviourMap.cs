using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class UserBehaviourMap : EntityTypeConfiguration<UserBehaviour>
    {
        public UserBehaviourMap()
        {
            this.ToTable("UserBehaviour");
            this.HasKey(u => u.Id);
        }
    }
}
