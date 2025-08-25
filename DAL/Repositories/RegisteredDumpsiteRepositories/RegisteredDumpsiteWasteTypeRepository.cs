using DAL.ApplicationStorage;
using DAL.Interfaces.Repositories.RegisteredDumpsiteRepositories;
using Entities.RegisteredDumpsiteEntities;
using System;

namespace DAL.Repositories.RegisteredDumpsiteRepositories;

public class RegisteredDumpsiteWasteTypeRepository : BaseResultRepository<RegisteredDumpsiteWasteType, Guid>, IRegisteredDumpsiteWasteTypeRepository
{
    public RegisteredDumpsiteWasteTypeRepository(ApplicationDbContext db) : base(db) { }
}
