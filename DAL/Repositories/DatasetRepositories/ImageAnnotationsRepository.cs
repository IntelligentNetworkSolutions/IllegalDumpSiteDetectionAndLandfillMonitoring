using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.ApplicationStorage;
using DAL.Interfaces.Repositories.DatasetRepositories;
using Entities.DatasetEntities;

namespace DAL.Repositories.DatasetRepositories
{
    public class ImageAnnotationsRepository : BaseResultRepository<ImageAnnotation, Guid>, IImageAnnotationsRepository
    {
        public ImageAnnotationsRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
