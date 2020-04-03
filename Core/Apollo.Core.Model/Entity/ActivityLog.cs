
namespace Apollo.Core.Model.Entity
{
    public class ActivityLog : BaseEntity
    {  
        public string Description { get; set; }
        public string ClientIp { get; set; }
        public string RegionAndCountry { get; set; }
        public string UserAction { get; set; }
        public string SubModule { get; set; }
        public string Module { get; set; }
        public string EndTime { get; set; }
        public string StartTime { get; set; }
        public string PresentDate { get; set; }
        public int UserId { get; set; }

        public ActivityLog()
        {
            Description = string.Empty;
            ClientIp = string.Empty;
            RegionAndCountry = string.Empty;
            UserAction = string.Empty;
            SubModule = string.Empty;
            Module = string.Empty;
            EndTime = string.Empty;
            StartTime = string.Empty;
            PresentDate = string.Empty;
        }
    }
}
