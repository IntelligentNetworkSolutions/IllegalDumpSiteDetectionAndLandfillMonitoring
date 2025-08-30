using DAL.ApplicationStorage;
using DAL.Interfaces.Repositories.RegisteredDumpsiteRepositories;
using Entities.RegisteredDumpsiteEntities;
using System;

namespace DAL.Repositories.RegisteredDumpsiteRepositories;
public class RegisteredDumpsiteInspectionRepository : BaseResultRepository<RegisteredDumpsiteInspection, Guid>, IRegisteredDumpsiteInspectionRepository
{
    public RegisteredDumpsiteInspectionRepository(ApplicationDbContext db) : base(db) { }
}