using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class TestimonialMap : EntityTypeConfiguration<Testimonial>
    {
        public TestimonialMap()
        {
            this.ToTable("Testimonials");
            this.HasKey(t => t.Id);
        }
    }
}
