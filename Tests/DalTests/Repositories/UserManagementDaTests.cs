namespace Tests.DalTests.Repositories
{
    [Trait("Category", "Integration")]
    public class UserManagementDaTests : IClassFixture<TestDatabaseFixture>
    {
        private readonly TestDatabaseFixture _fixture;

        public UserManagementDaTests(TestDatabaseFixture fixture) => _fixture = fixture;

        [Fact]
        public void GetUserById_UserExists_ReturnsUser()
        {

        }

        [Fact]
        public void GetUserById_UserNotExists_ReturnsNull()
        {

        }

        [Fact]
        public void GetUserById_DatabaseThrowsExceptionLogsError_ThrowsException()
        {

        }


    }
}
