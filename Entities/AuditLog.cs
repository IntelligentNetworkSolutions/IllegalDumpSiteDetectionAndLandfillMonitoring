using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class AuditLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long AuditLogId { get; set; }
        public string AuditData { get; set; }
        public string AuditAction { get; set; }
        public string EntityType { get; set; }
        public DateTime AuditDate { get; set; }
        public string AuditInternalUser { get; set; }
        public string TablePk { get; set; }
    }
}
