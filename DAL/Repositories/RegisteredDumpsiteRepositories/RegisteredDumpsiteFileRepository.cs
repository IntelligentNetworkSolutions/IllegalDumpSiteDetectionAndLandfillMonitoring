using DAL.ApplicationStorage;
using DAL.Interfaces.Repositories.RegisteredDumpsiteRepositories;
using Entities.RegisteredDumpsiteEntities;
using System;

namespace DAL.Repositories.RegisteredDumpsiteRepositories;
public class RegisteredDumpsiteFileRepository : BaseResultRepository<RegisteredDumpsiteFile, Guid>, IRegisteredDumpsiteFileRepository
{
    public RegisteredDumpsiteFileRepository(ApplicationDbContext db) : base(db) { }
}