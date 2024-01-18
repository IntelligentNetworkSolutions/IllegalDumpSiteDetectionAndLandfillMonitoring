using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entities;

namespace DAL.Interfaces.Repositories
{
    public interface IAuditLogsDa
    {
        Task<ICollection<AuditLog>> GetAll();

        Task<AuditLog> GetAuditData(int auditLogId);

        Task<ICollection<AuditLog>> GetAllFromToDate(DateTime dateFrom, DateTime dateTo);

        Task<ICollection<AuditLog>> GetAllByUsernameFromToDate(string internalUsername, 
                                                                DateTime? dateFrom, DateTime? dateTo, 
                                                                string action, string type);

        Task<List<string>> GetAuditActions();
    }
}
