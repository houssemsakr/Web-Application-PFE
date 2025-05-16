using System.ComponentModel.DataAnnotations.Schema;
namespace Web_Application_PFE.Models
{
    public class AuditLog
    {
        public int Id { get; set; }
       
        public string? Action { get; set; }
        public string? EntityType { get; set; }
        public string? EntityId { get; set; }
        public DateTime? Timestamp { get; set; }
        public string? IpAddress { get; set; }
        public string? AdditionalInfo { get; set; }
        [ForeignKey("User")]
        public string? UserId { get; set; }  
        public User? User { get; set; }
    }
}
