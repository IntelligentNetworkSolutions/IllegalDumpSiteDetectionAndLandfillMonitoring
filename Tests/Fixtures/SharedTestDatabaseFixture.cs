using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Fixtures
{
    [CollectionDefinition("Shared TestDatabaseFixture")]
    public class SharedTestDatabaseFixture : ICollectionFixture<TestDatabaseFixture>
    {
    }
}
