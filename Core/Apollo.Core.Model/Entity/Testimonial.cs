
namespace Apollo.Core.Model.Entity
{
    public class Testimonial : BaseEntity
    {
        public string Comment { get; set; }
        public string Name { get; set; }
        public int Priority { get; set; }
    }
}
