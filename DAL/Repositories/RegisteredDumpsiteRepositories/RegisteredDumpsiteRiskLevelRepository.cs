using DAL.ApplicationStorage;
using DAL.Interfaces.Repositories.RegisteredDumpsiteRepositories;
using Entities.RegisteredDumpsiteEntities;
using System;

namespace DAL.Repositories.RegisteredDumpsiteRepositories;

public class RegisteredDumpsiteRiskLevelRepository : BaseResultRepository<RegisteredDumpsiteRiskLevel, Guid>, IRegisteredDumpsiteRiskLevelRepository
{
    public RegisteredDumpsiteRiskLevelRepository(ApplicationDbContext db) : base(db) { }
}
