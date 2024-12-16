using DAL.ApplicationStorage;
using DAL.ApplicationStorage.SeedDatabase.TestSeedData;
using DAL.Repositories;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Tests.DalTests.Repositories
{
    [Trait("Category", "Integration")]
    [Collection("Shared TestDatabaseFixture")]
    public class UserManagementDaTests
    {
        private readonly TestDatabaseFixture _fixture;

        public UserManagementDaTests(TestDatabaseFixture fixture) => _fixture = fixture;

        [Fact]
        public async Task GetAllIntanetPortalUsers_ReturnsUsers()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();
            var repository = new UserManagementDa(dbContext, logger);

            // Act
            var users = await repository.GetAllIntanetPortalUsers();

            // Assert
            Assert.NotNull(users);
            Assert.NotEmpty(users);
            transaction.Rollback();
        }

        [Fact]
        public async Task GetAllIntanetPortalUsers_NoUsers_ReturnsEmptyList()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();
            var repository = new UserManagementDa(dbContext, logger);

            // Act
            var users = await repository.GetAllIntanetPortalUsers();

            // Assert
            Assert.NotNull(users);
            Assert.Empty(users);
            transaction.Rollback();
        }

        [Fact]
        public async Task GetAllIntanetPortalUsers_DatabaseThrowsExceptionLogsError_ThrowsException()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();
            var repository = new UserManagementDa(dbContext, logger);

            transaction.Rollback();
            dbContext.Dispose();

            // Act & Assert
            await Assert.ThrowsAsync<ObjectDisposedException>(async () => await repository.GetAllIntanetPortalUsers());
        }


        [Fact]
        public async void GetUserBySpecificClaim_UserExists_ReturnsUser()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();
            var repository = new UserManagementDa(dbContext, logger);

            // Act
            var user = await repository.GetUserBySpecificClaim();

            // Assert
            Assert.NotNull(user);
            Assert.Equal(UserSeedData.ThirdUser.Id, user.Id);
            transaction.Rollback();
        }

        [Fact]
        public async void GetUserBySpecificClaim_UserNotExists_ReturnsNull()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();
            var repository = new UserManagementDa(dbContext, logger);

            // Act
            var user = await repository.GetUserBySpecificClaim();

            // Assert
            Assert.Null(user);
            transaction.Rollback();
        }

        [Fact]
        public async Task GetUserBySpecificClaim_DatabaseThrowsExceptionLogsError_ThrowsException()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();
            var repository = new UserManagementDa(dbContext, logger);

            transaction.Rollback();
            dbContext.Dispose();

            // Act & Assert
            await Assert.ThrowsAsync<ObjectDisposedException>(async () => await repository.GetUserBySpecificClaim());
        }


        [Fact]
        public async void GetAllIntranetPortalUsersExcludingCurrent_UsersExist_ReturnsUsers()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();
            var repository = new UserManagementDa(dbContext, logger);

            string currentUserId = UserSeedData.FirstUser.Id;

            // Act
            var users = await repository.GetAllIntanetPortalUsersExcludingCurrent(currentUserId);

            // Assert
            Assert.NotNull(users);
            Assert.DoesNotContain(users, user => user.Id == currentUserId);
            transaction.Rollback();
        }

        [Fact]
        public async void GetAllIntranetPortalUsersExcludingCurrent_NoUsers_ReturnsEmptyList()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();

            // Do not seed the database to simulate no users.
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();
            var repository = new UserManagementDa(dbContext, logger);

            string currentUserId = Guid.NewGuid().ToString();

            // Act
            var users = await repository.GetAllIntanetPortalUsersExcludingCurrent(currentUserId);

            // Assert
            Assert.NotNull(users);
            Assert.Empty(users);
            transaction.Rollback();
        }

        [Fact]
        public async Task GetAllIntranetPortalUsersExcludingCurrent_DatabaseThrowsExceptionLogsError_ThrowsException()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();
            var repository = new UserManagementDa(dbContext, logger);

            transaction.Rollback();
            dbContext.Dispose(); // Dispose to simulate database exception.

            // Act & Assert
            await Assert.ThrowsAsync<ObjectDisposedException>(async () => await repository.GetAllIntanetPortalUsersExcludingCurrent(Guid.NewGuid().ToString()));
        }


        [Fact]
        public async void GetUserById_UserExists_ReturnsUser()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();

            // Act
            var user = await new UserManagementDa(dbContext, logger).GetUserById(UserSeedData.FirstUser.Id);

            // Assert
            Assert.NotNull(user);
            Assert.Equal(UserSeedData.FirstUser.Id, user.Id);
            transaction.Rollback();
        }

        [Fact]
        public async void GetUserById_UserNotExists_ReturnsNull()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();
            // Act
            var user = await new UserManagementDa(dbContext, logger).GetUserById(null);

            // Assert
            Assert.Null(user);
            transaction.Rollback();

        }

        [Fact]
        public async Task GetUserById_DatabaseThrowsExceptionLogsError_ThrowsException()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            //_fixture.SeedDatabase(dbContext);

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();
            UserManagementDa repository = new(dbContext, logger);

            transaction.Rollback();
            dbContext.Dispose();

            // Act & Assert
            await Assert.ThrowsAsync<ObjectDisposedException>(async () => await repository.GetUserById(Guid.NewGuid().ToString()));
        }

        [Fact]
        public async void GetUserByUsername_UserNameExists_ReturnsUser()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();

            // Act
            Entities.ApplicationUser? user = await new UserManagementDa(dbContext, logger).GetUserByUsername(UserSeedData.FirstUser.UserName);

            // Assert
            Assert.NotNull(user);
            Assert.Equal(UserSeedData.FirstUser.UserName, user.UserName);
            transaction.Rollback();
        }

        [Fact]
        public async void GetUserByUsername_UserNameNotExists_ReturnsNull()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();
            // Act
            var user = await new UserManagementDa(dbContext, logger).GetUserByUsername(null);

            // Assert
            Assert.Null(user);
            transaction.Rollback();

        }

        [Fact]
        public async Task GetUserByUsername_DatabaseThrowsExceptionLogsError_ThrowsException()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();
            UserManagementDa repository = new(dbContext, logger);

            transaction.Rollback();
            dbContext.Dispose();

            var testUsername = "test-username";

            // Act & Assert
            await Assert.ThrowsAsync<ObjectDisposedException>(async () => await repository.GetUserByUsername(testUsername));

        }

        [Fact]
        public async void GetAllRoles_ReturnsRolesList()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();

            // Act
            var roles = await new UserManagementDa(dbContext, logger).GetAllRoles();

            // Assert
            Assert.NotNull(roles);
            Assert.Equal(dbContext.Roles, roles);
            transaction.Rollback();
        }

        [Fact]
        public async Task GetAllRoles_DatabaseThrowsExceptionLogsError_ThrowsException()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();
            var repository = new UserManagementDa(dbContext, logger);

            transaction.Rollback();
            dbContext.Dispose();

            // Act & Assert
            await Assert.ThrowsAsync<ObjectDisposedException>(async () => await repository.GetAllRoles());
        }

        [Fact]
        public async void GetRole_RoleExists_ReturnsRole()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();

            // Act
            var role = await new UserManagementDa(dbContext, logger).GetRole(UserSeedData.FirstRole.Id);

            // Assert
            Assert.NotNull(role);
            Assert.Equal(UserSeedData.FirstRole.Id, role.Id);
            transaction.Rollback();
        }

        [Fact]
        public async void GetRole_RoleNotExists_ReturnsNull()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();
            // Act
            var role = await new UserManagementDa(dbContext, logger).GetRole(null);

            // Assert
            Assert.Null(role);
            transaction.Rollback();

        }
        [Fact]
        public async Task GetRole_DatabaseDisposed_LogsErrorAndThrowsException()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();

            var repository = new UserManagementDa(dbContext, logger);

            dbContext.Dispose();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ObjectDisposedException>(async () => await repository.GetRole(UserSeedData.FirstRole.Id));

        }


        [Fact]
        public async void GetAllRoleClaims_ReturnsRoleClaimsList()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();

            // Act
            var roleClaims = await new UserManagementDa(dbContext, logger).GetAllRoleClaims();

            // Assert
            Assert.NotNull(roleClaims);
            Assert.Equal(dbContext.RoleClaims, roleClaims);
            transaction.Rollback();
        }

        [Fact]
        public async void GetAllRoleClaims_DatabaseThrowsExceptionLogsError_ThrowsException()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();

            transaction.Rollback();
            dbContext.Dispose();

            // Act & Assert
            await Assert.ThrowsAsync<ObjectDisposedException>(async () => await new UserManagementDa(dbContext, logger).GetAllRoleClaims());
        }

        [Fact]
        public async void GetClaimsForUserByUserIdAndClaimType_UserExists_ReturnsClaimsList()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();
            // Act
            var claims = await new UserManagementDa(dbContext, logger).GetClaimsForUserByUserIdAndClaimType(UserSeedData.FirstUser.Id, UserSeedData.FirstRoleFirstClaim.ClaimType);

            // Assert
            Assert.NotNull(claims);
            Assert.NotEmpty(claims);
            Assert.Equal(claims.Count, 4);
            transaction.Rollback();
        }

        [Fact]
        public async void GetClaimsForUserByUserIdAndClaimType_UserNotExists_ReturnsEmptyList()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();
            // Act
            var claims = await new UserManagementDa(dbContext, logger).GetClaimsForUserByUserIdAndClaimType(null, null);

            // Assert
            Assert.Empty(claims);
            transaction.Rollback();

        }

        [Fact]
        public async Task GetClaimsForUserByUserIdAndClaimType_DatabaseThrowsExceptionLogsError_ThrowsException()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();
            UserManagementDa repository = new(dbContext, logger);

            transaction.Rollback();
            dbContext.Dispose();

            // Act & Assert
            await Assert.ThrowsAsync<ObjectDisposedException>(async () =>
                await repository.GetClaimsForUserByUserIdAndClaimType(Guid.NewGuid().ToString(), Guid.NewGuid().ToString())
            );
        }

        [Fact]
        public async void GetRolesForUser_UserExists_ReturnsUserRoles()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();

            List<string> userRoles = new List<string>
                {
                    UserSeedData.FirstUserFirstRole.RoleId
                };

            // Act
            var roles = await new UserManagementDa(dbContext, logger).GetRolesForUser(userRoles);

            // Assert
            Assert.NotNull(roles);
            Assert.NotEmpty(roles);
            transaction.Rollback();
        }

        [Fact]
        public async void GetRolesForUser_UserNotExists_ReturnsEmptyList()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();
            // Act
            var roles = await new UserManagementDa(dbContext, logger).GetRolesForUser(null);

            // Assert
            Assert.Empty(roles);
            transaction.Rollback();

        }

        [Fact]
        public async Task GetRolesForUser_DatabaseThrowsExceptionLogsError_ThrowsException()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();
            UserManagementDa repository = new(dbContext, logger);
            var roles = new List<string>();

            transaction.Rollback();
            dbContext.Dispose();

            // Act & Assert
            await Assert.ThrowsAsync<ObjectDisposedException>(async () =>
                await repository.GetRolesForUser(roles)
            );
        }

        [Fact]
        public async void GetUserRoles_ReturnsUserRoles()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();

            // Act
            var userRoles = await new UserManagementDa(dbContext, logger).GetUserRoles();

            // Assert
            Assert.NotNull(userRoles);
            Assert.Equal(userRoles.Count, 3);
            transaction.Rollback();
        }

        [Fact]
        public async Task GetUserRoles_DatabaseThrowsExceptionLogsError_ThrowsException()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();
            UserManagementDa repository = new(dbContext, logger);

            transaction.Rollback();
            dbContext.Dispose();

            // Act & Assert
            await Assert.ThrowsAsync<ObjectDisposedException>(async () =>
                await repository.GetUserRoles()
            );
        }

        [Fact]
        public async void GetUserRolesByUserId_UserExists_ReturnsRolesList()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();

            // Act
            var roles = await new UserManagementDa(dbContext, logger).GetUserRolesByUserId(UserSeedData.FirstUser.Id);

            // Assert
            Assert.NotNull(roles);
            Assert.Single(roles);
            transaction.Rollback();
        }

        [Fact]
        public async void GetUserRolesByUserId_UserNotExists_ReturnsEmptyList()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();
            // Act
            var roles = await new UserManagementDa(dbContext, logger).GetUserRolesByUserId(null);

            // Assert
            Assert.Empty(roles);
            transaction.Rollback();

        }

        [Fact]
        public async Task GetUserRolesByUserId_DatabaseThrowsExceptionLogsError_ThrowsException()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();
            UserManagementDa repository = new(dbContext, logger);

            transaction.Rollback();
            dbContext.Dispose();

            // Act & Assert
            await Assert.ThrowsAsync<ObjectDisposedException>(async () =>
                await repository.GetUserRolesByUserId(Guid.NewGuid().ToString())
            );
        }

        [Fact]
        public async void GetClaimsForRoleByRoleIdAndClaimType_RoleExistsAndClaimTypeExists_ReturnsClaimsList()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();

            // Act
            var claims = await new UserManagementDa(dbContext, logger).GetClaimsForRoleByRoleIdAndClaimType(UserSeedData.FirstRole.Id, UserSeedData.FirstRoleFirstClaim.ClaimType);

            // Assert
            Assert.NotNull(claims);

            Assert.Equal(claims.Count, 4);

            transaction.Rollback();
        }

        [Fact]
        public async void GetClaimsForRoleByRoleIdAndClaimType_RoleNotExistsAndClaimTypeNotExists_ReturnsEmptyList()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();
            // Act
            var claims = await new UserManagementDa(dbContext, logger).GetClaimsForRoleByRoleIdAndClaimType(null, null);

            // Assert
            Assert.Empty(claims);
            transaction.Rollback();

        }

        [Fact]
        public async Task GetClaimsForRoleByRoleIdAndClaimType_DatabaseThrowsExceptionLogsError_ThrowsException()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();
            UserManagementDa repository = new(dbContext, logger);

            transaction.Rollback();
            dbContext.Dispose();

            // Act & Assert
            await Assert.ThrowsAsync<ObjectDisposedException>(async () =>
                await repository.GetClaimsForRoleByRoleIdAndClaimType(Guid.NewGuid().ToString(), "SomeClaimType")
            );
        }


        [Fact]
        public async Task AddUser_ValidUser_ReturnsAddedUser()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();

            var userManagementDa = new UserManagementDa(dbContext, logger);
            var newUser = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "newtestuser",
                NormalizedUserName = "newtestuser".ToUpper(),
                FirstName = "New",
                LastName = "Test User",
                Email = "newtestuser@test.com",
                NormalizedEmail = "newtestuser@test.com".ToUpper(),
                IsActive = true,
                EmailConfirmed = true,
            };

            // Act
            var addedUser = await userManagementDa.AddUser(newUser);

            // Assert
            Assert.NotNull(addedUser);
            Assert.Equal(newUser.UserName, addedUser.UserName);
            Assert.Equal(newUser.Email, addedUser.Email);
            var userInDb = await dbContext.Users.FindAsync(addedUser.Id);
            Assert.NotNull(userInDb);
            Assert.Equal(newUser.UserName, userInDb.UserName);

            transaction.Rollback();
        }

        [Fact]
        public async Task AddUser_NullUser_ThrowsArgumentNullException()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();
            var userManagementDa = new UserManagementDa(dbContext, logger);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await userManagementDa.AddUser(null));

            transaction.Rollback();
        }

        [Fact]
        public async Task AddRole_ValidRole_AddsRoleSuccessfully()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();
            var userManagementDa = new UserManagementDa(dbContext, logger);

            var newRole = new IdentityRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = "New Role",
                NormalizedName = "NEW ROLE".ToUpper()
            };

            // Act
            var result = await userManagementDa.AddRole(newRole);

            // Assert
            Assert.NotNull(result.Data);
            Assert.Equal(newRole.Name, result.Data.Name);
            Assert.Equal(newRole.NormalizedName, result.Data.NormalizedName);

            transaction.Rollback();
        }

        [Fact]
        public async Task AddRole_NullRole_ReturnsFailureResult()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();
            var userManagementDa = new UserManagementDa(dbContext, logger);

            // Act
            var result = await userManagementDa.AddRole(null);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
            Assert.NotNull(result.ErrMsg);
            Assert.Contains("Value cannot be null", result.ErrMsg);

            transaction.Rollback();
        }

        [Fact]
        public async Task AddRolesForUserRange_ValidUserRole_AddsUserRoleSuccessfully()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = await dbContext.Database.BeginTransactionAsync();

            // Assuming that seeding is necessary for the test.
            _fixture.SeedDatabase(dbContext);

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();

            var userManagementDa = new UserManagementDa(dbContext, logger);

            var list = new List<IdentityUserRole<string>>();

            var newUserRole = new IdentityUserRole<string>
            {
                RoleId = UserSeedData.SecondRole.Id,
                UserId = UserSeedData.FirstUser.Id
            };
            list.Add(newUserRole);

            // Act
            List<IdentityUserRole<string>> result = await userManagementDa.AddRolesForUserRange(list, transaction);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);  // Make sure only one role is added

            var addedUserRole = result.First();
            Assert.Equal(newUserRole.UserId, addedUserRole.UserId);
            Assert.Equal(newUserRole.RoleId, addedUserRole.RoleId);

            transaction.Rollback();
        }


        [Fact]
        public async Task AddRolesForUserRange_NullUserRole_ThrowsNullReferenceException()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();
            var userManagementDa = new UserManagementDa(dbContext, logger);
            List<IdentityUserRole<string>>? roles = null;

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(async () => await userManagementDa.AddRolesForUserRange(roles, transaction));

            transaction.Rollback();
        }

        [Fact]
        public async Task AddClaimForRole_ValidRoleClaim_AddsClaimSuccessfully()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            UserSeedData.SeedData(dbContext);

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();
            var userManagementDa = new UserManagementDa(dbContext, logger);


            var newRoleClaim = new IdentityRoleClaim<string>
            {
                Id = 7,
                RoleId = UserSeedData.FirstRole.Id,
                ClaimType = "NewClaimType",
                ClaimValue = "NewClaimValue"
            };


            // Act
            await userManagementDa.AddClaimForRole(newRoleClaim);

            // Assert
            var addedClaim = await dbContext.RoleClaims.FindAsync(newRoleClaim.Id);
            Assert.NotNull(addedClaim);
            Assert.Equal(newRoleClaim.ClaimType, addedClaim.ClaimType);

            transaction.Rollback();
        }


        [Fact]
        public async Task AddClaimForRole_NullRoleClaim_ThrowsArgumentNullException()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();
            var userManagementDa = new UserManagementDa(dbContext, logger);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await userManagementDa.AddClaimForRole(null));

            transaction.Rollback();
        }

        [Fact]
        public async Task AddClaimsForUserRange_ValidUserClaim_AddsClaimSuccessfully()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = await dbContext.Database.BeginTransactionAsync();
            _fixture.SeedDatabase(dbContext);

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();

            var userManagementDa = new UserManagementDa(dbContext, logger);

            // Ensure the user exists
            var userExists = await userManagementDa.GetUserById(UserSeedData.FirstUser.Id);
            Assert.NotNull(userExists);

            var list = new List<IdentityUserClaim<string>>();

            IdentityUserClaim<string> newUserClaim = new IdentityUserClaim<string>
            {
                Id = 7,
                UserId = UserSeedData.FirstUser.Id,
                ClaimType = "NewUserClaimType",
                ClaimValue = "NewUserClaimValue"
            };
            list.Add(newUserClaim);

            // Act
            await userManagementDa.AddClaimsForUserRange(list, transaction);

            // Assert
            var addedClaim = await dbContext.UserClaims.FindAsync(newUserClaim.Id);
            Assert.NotNull(addedClaim);
            Assert.Equal(newUserClaim.ClaimType, addedClaim.ClaimType);
            Assert.Equal(newUserClaim.ClaimValue, addedClaim.ClaimValue);

            await transaction.RollbackAsync();
        }

        [Fact]
        public async Task AddClaimsForUserRange_DatabaseThrowsException_LogsErrorAndRollsBackTransaction()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();

            var repository = new UserManagementDa(dbContext, logger);

            var userClaims = new List<IdentityUserClaim<string>>
                    {
                        new IdentityUserClaim<string> { UserId = "validUserId", ClaimType = "PreferedLanguageClaim", ClaimValue = "en" }
                    };
            dbContext.Dispose();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ObjectDisposedException>(async () =>
                await repository.AddClaimsForUserRange(userClaims, transaction)
            );
        }

        [Fact]
        public async Task AddLanguageClaimForUser_ValidUserClaim_AddsClaimSuccessfully()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = await dbContext.Database.BeginTransactionAsync();
            _fixture.SeedDatabase(dbContext);

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();
            var userManagementDa = new UserManagementDa(dbContext, logger);

            var newLanguageClaim = new IdentityUserClaim<string>
            {
                Id = 7,
                UserId = UserSeedData.FirstUser.Id,
                ClaimType = "Language",
                ClaimValue = "English"
            };

            // Act
            await userManagementDa.AddLanguageClaimForUser(newLanguageClaim);

            // Assert
            var addedClaim = await dbContext.UserClaims.SingleOrDefaultAsync(c => c.UserId == newLanguageClaim.UserId && c.ClaimType == newLanguageClaim.ClaimType);
            Assert.NotNull(addedClaim);
            Assert.Equal(newLanguageClaim.ClaimType, addedClaim.ClaimType);
            Assert.Equal(newLanguageClaim.ClaimValue, addedClaim.ClaimValue);

            transaction.Rollback();
        }

        [Fact]
        public async Task AddLanguageClaimForUser_NullUserClaim_ThrowsArgumentNullException()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();
            var userManagementDa = new UserManagementDa(dbContext, logger);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await userManagementDa.AddLanguageClaimForUser(null));

            transaction.Rollback();
        }

        [Fact]
        public async Task UpdateUser_ValidUser_UpdatesSuccessfully()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();
            var userManagementDa = new UserManagementDa(dbContext, logger);

            ApplicationUser userToUpdate = UserSeedData.FirstUser;
            userToUpdate.FirstName = "First";

            // Act
            bool result = await userManagementDa.UpdateUser(userToUpdate);

            // Assert
            Assert.True(result);

            var updatedUser = await dbContext.Users.FindAsync(userToUpdate.Id);
            Assert.Equal("First", updatedUser.FirstName);

            transaction.Rollback();
        }

        [Fact]
        public async Task UpdateUser_NullUser_ThrowsArgumentNullException()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();
            var userManagementDa = new UserManagementDa(dbContext, logger);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await userManagementDa.UpdateUser(null));

            transaction.Rollback();
        }

        [Fact]
        public async Task UpdateRole_ValidRole_UpdatesSuccessfully()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();
            var userManagementDa = new UserManagementDa(dbContext, logger);

            var roleToUpdate = UserSeedData.FirstRole;
            roleToUpdate.Name = "First Role";

            // Act
            var result = await userManagementDa.UpdateRole(roleToUpdate);

            // Assert
            Assert.True(result);
            var updatedRole = await dbContext.Roles.FindAsync(roleToUpdate.Id);
            Assert.Equal("First Role", updatedRole.Name);

            transaction.Rollback();
        }

        [Fact]
        public async Task UpdateRole_NullRole_ThrowsArgumentNullException()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();
            var userManagementDa = new UserManagementDa(dbContext, logger);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await userManagementDa.UpdateRole(null));

            transaction.Rollback();
        }

        [Fact]
        public async Task UpdateLanguageClaimForUser_ValidClaim_UpdatesSuccessfully()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();

            var userManagementDa = new UserManagementDa(dbContext, logger);

            var claimToUpdate = UserSeedData.FirstUserFirstUserClaim;
            claimToUpdate.ClaimValue = "Updated Claim Value";

            // Act
            await userManagementDa.UpdateLanguageClaimForUser(claimToUpdate);

            // Assert
            var updatedClaim = await dbContext.UserClaims.FindAsync(claimToUpdate.Id);
            Assert.Equal("Updated Claim Value", updatedClaim.ClaimValue);

            transaction.Rollback();
        }

        [Fact]
        public async Task UpdateLanguageClaimForUser_NullClaim_ThrowsArgumentNullException()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();
            var userManagementDa = new UserManagementDa(dbContext, logger);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await userManagementDa.UpdateLanguageClaimForUser(null));

            transaction.Rollback();
        }

        [Fact]
        public async Task DeleteUser_ValidUser_DeletesSuccessfully()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();
            var userManagementDa = new UserManagementDa(dbContext, logger);

            var userToDelete = UserSeedData.FirstUser;

            // Act
            var deletedUser = await userManagementDa.DeleteUser(userToDelete);

            // Assert
            Assert.NotNull(deletedUser);
            var foundUser = await dbContext.Users.FindAsync(userToDelete.Id);
            Assert.Null(foundUser);

            transaction.Rollback();
        }

        [Fact]
        public async Task DeleteUser_NullUser_ThrowsArgumentNullException()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();
            var userManagementDa = new UserManagementDa(dbContext, logger);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await userManagementDa.DeleteUser(null));

            transaction.Rollback();
        }

        [Fact]
        public async Task DeleteRole_ValidRole_DeletesSuccessfully()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);


            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();
            var userManagementDa = new UserManagementDa(dbContext, logger);
            var roleToDelete = UserSeedData.FirstRole;

            // Act
            var deletedRole = await userManagementDa.DeleteRole(roleToDelete);

            // Assert
            Assert.NotNull(deletedRole);
            var foundRole = await dbContext.Roles.FindAsync(roleToDelete.Id);
            Assert.Null(foundRole);

            transaction.Rollback();
        }

        [Fact]
        public async Task DeleteRole_NullRole_ThrowsArgumentNullException()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();
            var userManagementDa = new UserManagementDa(dbContext, logger);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await userManagementDa.DeleteRole(null));

            transaction.Rollback();
        }

        [Fact]
        public async Task DeleteClaimsRolesForUser_ValidClaimsAndRoles_DeletesSuccessfully()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();
            var userManagementDa = new UserManagementDa(dbContext, logger);

            var userClaims = new List<IdentityUserClaim<string>>
                {
                    UserSeedData.FirstUserFirstUserClaim,
                    UserSeedData.FirstUserSecondUserClaim
                };

            var userRoles = new List<IdentityUserRole<string>>
                {
                    UserSeedData.FirstUserFirstRole
                };

            // Act
            await userManagementDa.DeleteClaimsRolesForUser(userClaims, userRoles, transaction);

            // Assert
            foreach (var claim in userClaims)
            {
                var foundClaim = await dbContext.UserClaims.FindAsync(claim.Id);
                Assert.Null(foundClaim);
            }

            foreach (var role in userRoles)
            {
                var foundRole = await dbContext.UserRoles.FindAsync(role.UserId, role.RoleId);
                Assert.Null(foundRole);
            }

            transaction.Rollback();
        }

        [Fact]
        public async Task DeleteClaimsRolesForUser_NullClaims_ThrowsNullReferenceException()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();
            var userManagementDa = new UserManagementDa(dbContext, logger);

            var userRoles = new List<IdentityUserRole<string>>
                {
                    UserSeedData.FirstUserFirstRole
                };

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(async () => await userManagementDa.DeleteClaimsRolesForUser(null, userRoles, transaction));

            transaction.Rollback();
        }

        [Fact]
        public async Task DeleteClaimsRolesForUser_NullRoles_ThrowsNullReferenceException()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();
            var userManagementDa = new UserManagementDa(dbContext, logger);

            var userClaims = new List<IdentityUserClaim<string>>
                {
                    UserSeedData.FirstUserFirstUserClaim
                };

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(async () => await userManagementDa.DeleteClaimsRolesForUser(userClaims, null, transaction));

            transaction.Rollback();
        }

        [Fact]
        public async Task DeleteClaimsForRole_ValidClaims_DeletesSuccessfully()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();
            var userManagementDa = new UserManagementDa(dbContext, logger);

            var claimsToDelete = new List<IdentityRoleClaim<string>>
                {
                    UserSeedData.FirstRoleFirstClaim,
                    UserSeedData.FirstRoleSecondClaim
                };

            // Act
            await userManagementDa.DeleteClaimsForRole(claimsToDelete, transaction);

            // Assert
            foreach (var claim in claimsToDelete)
            {
                var foundClaim = await dbContext.RoleClaims.FindAsync(claim.Id);
                Assert.Null(foundClaim);
            }

            transaction.Rollback();
        }

        [Fact]
        public async Task DeleteClaimsForRole_NullClaims_ThrowsArgumentNullException()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();
            var userManagementDa = new UserManagementDa(dbContext, logger);

            List<IdentityRoleClaim<string>>? claims = null;

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(async () => await userManagementDa.DeleteClaimsForRole(claims, transaction));

            await transaction.RollbackAsync();
        }

        [Fact]
        public async Task DeleteClaimsForRole_EmptyClaims_DeletesNothing()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();
            var userManagementDa = new UserManagementDa(dbContext, logger);

            var claimsToDelete = new List<IdentityRoleClaim<string>>();

            // Act
            await userManagementDa.DeleteClaimsForRole(claimsToDelete, transaction);

            // Assert
            var allClaims = await dbContext.RoleClaims.ToListAsync();
            Assert.NotNull(allClaims);
            Assert.True(allClaims.Count > 0);

            transaction.Rollback();
        }
        [Fact]
        public async Task GetPreferredLanguage_ClaimExists_ReturnsClaimValue()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();

            var repository = new UserManagementDa(dbContext, logger);

            // Act
            var language = await repository.GetPreferredLanguage(UserSeedData.ThirdUser.Id);

            // Assert
            Assert.NotNull(language);
            Assert.Equal(UserSeedData.ThirdUserSecondUserClaim.ClaimType, language);
            transaction.Rollback();
        }
        [Fact]
        public async Task GetPreferredLanguage_ClaimNotExists_ReturnsEmptyString()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();

            var repository = new UserManagementDa(dbContext, logger);

            // Act
            var language = await repository.GetPreferredLanguage(UserSeedData.FirstUser.Id);

            // Assert
            Assert.Equal("", language);
            transaction.Rollback();
        }

        [Fact]
        public async Task GetPreferredLanguage_DatabaseDisposed_LogsErrorAndThrowsException()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();

            var repository = new UserManagementDa(dbContext, logger);

            dbContext.Dispose();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ObjectDisposedException>(async () => await repository.GetPreferredLanguage(UserSeedData.FirstUser.Id));
        }

        [Fact]
        public void CheckUserBeforeDelete_DatabaseDisposed_LogsErrorAndThrowsException()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<UserManagementDa> logger = loggerFactory.CreateLogger<UserManagementDa>();

            var repository = new UserManagementDa(dbContext, logger);

            dbContext.Dispose();

            // Act & Assert
            var exception = Assert.Throws<ObjectDisposedException>(() =>
                repository.CheckUserBeforeDelete(UserSeedData.FirstUser.Id)
            );
        }


    }
}
