using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Consulcon.Domain.Common
{
    public abstract class AuditableEntity
    {
        [NotMapped]
        public DateTime? CreatedAt { get; set; }
        
        [NotMapped]
        public string? CreatedBy { get; set; }

        [NotMapped]
        public DateTime? UpdatedAt { get; set; }

        [NotMapped]
        public string? UpdatedBy { get; set; }
        
        // We will map these dynamically in OnModelCreating if corresponding columns exist
        // or use [Column("column_name")] in specific classes.
    }
}
