using System.ComponentModel.DataAnnotations;

namespace Web_Application_PFE.Models
{
    public class EntityClient
    {
        [Key]
        public int Id { get; set; }
        public string? Sales { get; set; }
        public string? EmailClient { get; set; }
        public string? Customer { get; set; }
        public ICollection<AddRFQ>? AddRFQs { get; set; }
        public ICollection<Brouillon>? Brouillons { get; set; }
        public ICollection<Epingle>? Epingles { get; set; }
        public ICollection<VersionEntity>? VersionEntities { get; set; }

    }
}
