using DAL.ApplicationStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Interfaces.Repositories;

namespace DAL.Repositories
{
    public class AuditLogsDa : IAuditLogsDa
    {
        private readonly ApplicationDbContext _db;
        private static ILogger<AuditLogsDa> _logger;
        public AuditLogsDa(ApplicationDbContext db, ILogger<AuditLogsDa> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<ICollection<AuditLog>> GetAll()
        {
            try
            {
                return await _db.AuditLog.OrderByDescending(x => x.AuditDate)
                                            .AsNoTracking()
                                            .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }

        // TODO: Change to Nullable ? if first or default
        public async Task<AuditLog> GetAuditData(int auditLogId)
        {
            try
            {
                return await _db.AuditLog.Where(z => z.AuditLogId == auditLogId)
                                            .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }

        // TODO: Test
        public async Task<ICollection<AuditLog>> GetAllFromToDate(DateTime dateFrom, DateTime dateTo)
        {
            var query = _db.AuditLog.AsQueryable();

            if (dateFrom.Date != DateTime.MinValue)
            {
                query = query.Where(x => x.AuditDate.Date >= dateFrom.Date);
            }
            if (dateTo.Date != DateTime.MinValue)
            {
                query = query.Where(x => x.AuditDate.Date <= dateTo.Date);
            }

            return await query.AsNoTracking().ToListAsync();
        }

        // TODO: Test
        public async Task<ICollection<AuditLog>> GetAllByUsernameFromToDate(string internalUsername, DateTime? dateFrom, DateTime? dateTo, string action, string type)
        {
            var query = _db.AuditLog.AsQueryable();

            if (!string.IsNullOrWhiteSpace(internalUsername))
            {
                query = query.Where(x => x.AuditInternalUser == internalUsername);
            }
            if (dateFrom.HasValue)
            {
                query = query.Where(x => x.AuditDate.Date >= dateFrom);
            }
            if (dateTo.HasValue)
            {
                query = query.Where(x => x.AuditDate.Date <= dateTo);
            }
            if (!string.IsNullOrWhiteSpace(action))
            {
                query = query.Where(x => x.AuditAction == action);
            }
            if (!string.IsNullOrWhiteSpace(type))
            {
                query = query.Where(x => x.EntityType.Contains(type));
            }
            return await query.AsNoTracking().Take(5000).ToListAsync();
        }

        public async Task<List<string>> GetAuditActions()
        {
            try
            {
                return await _db.AuditLog.GroupBy(z => z.AuditAction)
                                            .Select(z => z.Key)
                                            .AsNoTracking()
                                            .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }
    }
}
