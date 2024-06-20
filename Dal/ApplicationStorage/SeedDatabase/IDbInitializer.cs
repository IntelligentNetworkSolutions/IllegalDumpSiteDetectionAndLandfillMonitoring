using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ApplicationStorage.SeedDatabase
{
    public interface IDbInitializer
    {
        void Initialize(bool? runMigrations, bool? loadModules, List<string> modulesToLoad);
    }
}
