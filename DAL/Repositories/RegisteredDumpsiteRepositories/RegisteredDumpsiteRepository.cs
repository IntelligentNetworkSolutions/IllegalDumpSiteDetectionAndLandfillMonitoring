using DAL.ApplicationStorage;
using DAL.Interfaces.Repositories.RegisteredDumpsiteRepositories;
using Entities.RegisteredDumpsiteEntities;
using System;

namespace DAL.Repositories.RegisteredDumpsiteRepositories;

public class RegisteredDumpsiteRepository : BaseResultRepository<RegisteredDumpsite, Guid>, IRegisteredDumpsiteRepository
{
    public RegisteredDumpsiteRepository(ApplicationDbContext db) : base(db) { }
}
