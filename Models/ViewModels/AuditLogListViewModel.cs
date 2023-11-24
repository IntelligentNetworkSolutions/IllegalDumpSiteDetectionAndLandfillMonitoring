using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class AuditLogListViewModel
    {
        public long AuditLogId { get; set; }
        public string AuditData { get; set; }
        public string AuditAction { get; set; }
        public string EntityType { get; set; }
        public DateTime AuditDate { get; set; }
        public string AuditInternalUser { get; set; }
        public string TablePk { get; set; }
    }
}
