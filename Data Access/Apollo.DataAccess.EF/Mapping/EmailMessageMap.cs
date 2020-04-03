using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class EmailMessageMap : EntityTypeConfiguration<EmailMessage>
    {
        public EmailMessageMap()
        {
            this.ToTable("EmailMessage");
            this.HasKey(e => e.Id);
        }
    }
}
