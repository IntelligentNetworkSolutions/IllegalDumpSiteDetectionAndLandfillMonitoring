using DAL.ApplicationStorage;
using DAL.Interfaces.Repositories.RegisteredDumpsiteRepositories;
using Entities.RegisteredDumpsiteEntities;
using System;

namespace DAL.Repositories.RegisteredDumpsiteRepositories;
public class RegisteredDumpsiteInspectionFileRepository : BaseResultRepository<RegisteredDumpsiteInspectionFile, Guid>, IRegisteredDumpsiteInspectionFileRepository
{
    public RegisteredDumpsiteInspectionFileRepository(ApplicationDbContext db) : base(db) { }
}