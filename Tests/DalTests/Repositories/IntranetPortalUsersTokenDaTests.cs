using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.ApplicationStorage;
using DAL.ApplicationStorage.SeedDatabase.TestSeedData;
using DAL.Repositories;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Tests.DalTests.Repositories
{
    [Trait("Category", "Integration")]
    [Collection("Shared TestDatabaseFixture")]
    public class IntranetPortalUsersTokenDaTests
    {
        private readonly TestDatabaseFixture _fixture;

        public IntranetPortalUsersTokenDaTests(TestDatabaseFixture fixture) => _fixture = fixture;

        [Fact]
        public async Task CreateIntranetPortalUserToken_ShouldCreateNewTokenAndUpdateExisting()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<IntranetPortalUsersTokenDa> logger = loggerFactory.CreateLogger<IntranetPortalUsersTokenDa>();
            var repository = new IntranetPortalUsersTokenDa(dbContext, logger);

            string userId = UserSeedData.FirstUser.Id;
            string token = "newToken";
            List<IntranetPortalUsersToken> existingTokens =
                [ new() { ApplicationUserId = userId, isTokenUsed = false, Token = "oldToken1" },
                new() { ApplicationUserId = userId, isTokenUsed = true, Token = "oldToken2" }];

            dbContext.IntranetPortalUsersTokens.AddRange(existingTokens);

            // Act
            int result = await repository.CreateIntranetPortalUserToken(token, userId);

            // Assert
            Assert.Equal(3, result);
            IntranetPortalUsersToken? insertedToken = dbContext.IntranetPortalUsersTokens.AsNoTracking().FirstOrDefault(x => x.Token == token);
            Assert.NotNull(insertedToken);
            transaction.Rollback();
        }

        [Fact]
        public async Task CreateIntranetPortalUserToken_ShouldLogErrorAndThrowExceptionOnDatabaseError()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<IntranetPortalUsersTokenDa> logger = loggerFactory.CreateLogger<IntranetPortalUsersTokenDa>();
            var repository = new IntranetPortalUsersTokenDa(dbContext, logger);
            string userId = UserSeedData.FirstUser.Id;
            string token = "newToken";

            transaction.Rollback();
            // Simulate a database error
            dbContext.Dispose();

            // Act & Assert
            await Assert.ThrowsAsync<ObjectDisposedException>(() => repository.CreateIntranetPortalUserToken(token, userId));
        }

        [Fact]
        public async Task UpdateAndHashUserPassword_ShouldUpdatePasswordHash()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<IntranetPortalUsersTokenDa> logger = loggerFactory.CreateLogger<IntranetPortalUsersTokenDa>();
            var repository = new IntranetPortalUsersTokenDa(dbContext, logger);
            var user = await dbContext.Users.FindAsync(UserSeedData.FirstUser.Id);
            string oldPasswordHash = user.PasswordHash;
            string newPassword = "newTestPassword123!";

            // Act
            int result = await repository.UpdateAndHashUserPassword(user, newPassword);

            // Assert
            Assert.Equal(1, result);
            var updatedUser = await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == user.Id);
            Assert.NotNull(updatedUser);
            Assert.NotEqual(oldPasswordHash, updatedUser.PasswordHash);
            transaction.Rollback();
        }

        [Fact]
        public async Task UpdateIsTokenUsedForUser_ShouldUpdateTokenToUsed()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<IntranetPortalUsersTokenDa> logger = loggerFactory.CreateLogger<IntranetPortalUsersTokenDa>();
            var repository = new IntranetPortalUsersTokenDa(dbContext, logger);
            string userId = UserSeedData.FirstUser.Id;
            string token = "testToken";
            var intranetPortalUserToken = new IntranetPortalUsersToken { ApplicationUserId = userId, Token = token, isTokenUsed = false };
            dbContext.IntranetPortalUsersTokens.Add(intranetPortalUserToken);
            await dbContext.SaveChangesAsync();

            // Act
            int result = await repository.UpdateIsTokenUsedForUser(token, userId);

            // Assert
            Assert.Equal(1, result);
            var updatedToken = await dbContext.IntranetPortalUsersTokens.AsNoTracking().FirstOrDefaultAsync(t => t.Token == token && t.ApplicationUserId == userId);
            Assert.NotNull(updatedToken);
            Assert.True(updatedToken.isTokenUsed);
            transaction.Rollback();
        }

        [Fact]
        public async Task GetUser_ShouldReturnUser()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<IntranetPortalUsersTokenDa> logger = loggerFactory.CreateLogger<IntranetPortalUsersTokenDa>();
            var repository = new IntranetPortalUsersTokenDa(dbContext, logger);
            string userId = UserSeedData.FirstUser.Id;

            // Act
            var result = await repository.GetUser(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            transaction.Rollback();
        }

        [Fact]
        public async Task IsTokenNotUsed_ShouldReturnTrueForUnusedToken()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<IntranetPortalUsersTokenDa> logger = loggerFactory.CreateLogger<IntranetPortalUsersTokenDa>();
            var repository = new IntranetPortalUsersTokenDa(dbContext, logger);
            string userId = UserSeedData.FirstUser.Id;
            string token = "unusedToken";
            var intranetPortalUserToken = new IntranetPortalUsersToken { ApplicationUserId = userId, Token = token, isTokenUsed = false };
            dbContext.IntranetPortalUsersTokens.Add(intranetPortalUserToken);
            await dbContext.SaveChangesAsync();

            // Act
            bool result = await repository.IsTokenNotUsed(token, userId);

            // Assert
            Assert.True(result);
            transaction.Rollback();
        }

        [Fact]
        public async Task IsTokenNotUsed_ShouldReturnFalseForUsedToken()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<IntranetPortalUsersTokenDa> logger = loggerFactory.CreateLogger<IntranetPortalUsersTokenDa>();
            var repository = new IntranetPortalUsersTokenDa(dbContext, logger);
            string userId = UserSeedData.FirstUser.Id;
            string token = "usedToken";
            var intranetPortalUserToken = new IntranetPortalUsersToken { ApplicationUserId = userId, Token = token, isTokenUsed = true };
            dbContext.IntranetPortalUsersTokens.Add(intranetPortalUserToken);
            await dbContext.SaveChangesAsync();

            // Act
            bool result = await repository.IsTokenNotUsed(token, userId);

            // Assert
            Assert.False(result);
            transaction.Rollback();
        }

        [Fact]
        public async Task IsTokenNotUsed_ShouldLogErrorAndThrowExceptionOnDatabaseError()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<IntranetPortalUsersTokenDa> logger = loggerFactory.CreateLogger<IntranetPortalUsersTokenDa>();
            var repository = new IntranetPortalUsersTokenDa(dbContext, logger);
            string userId = UserSeedData.FirstUser.Id;
            string token = "testToken";

            transaction.Rollback();
            // Simulate a database error
            dbContext.Dispose();

            // Act & Assert
            await Assert.ThrowsAsync<ObjectDisposedException>(() => repository.IsTokenNotUsed(token, userId));
        }

        [Fact]
        public async Task CreateIntranetPortalUserToken_ShouldCreateNewTokenWhenNoExistingTokens()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<IntranetPortalUsersTokenDa> logger = loggerFactory.CreateLogger<IntranetPortalUsersTokenDa>();
            var repository = new IntranetPortalUsersTokenDa(dbContext, logger);
            string userId = UserSeedData.FirstUser.Id;
            string token = "newToken";

            // Act
            int result = await repository.CreateIntranetPortalUserToken(token, userId);

            // Assert
            Assert.Equal(1, result);
            IntranetPortalUsersToken? insertedToken = await dbContext.IntranetPortalUsersTokens.AsNoTracking().FirstOrDefaultAsync(x => x.Token == token);
            Assert.NotNull(insertedToken);
            Assert.False(insertedToken.isTokenUsed);
            transaction.Rollback();
        }

        [Fact]
        public async Task UpdateAndHashUserPassword_ShouldThrowExceptionWhenUserDoesNotExist()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<IntranetPortalUsersTokenDa> logger = loggerFactory.CreateLogger<IntranetPortalUsersTokenDa>();
            var repository = new IntranetPortalUsersTokenDa(dbContext, logger);
            var nonExistentUser = new ApplicationUser { Id = "nonexistent" };
            string newPassword = "newTestPassword123!";

            transaction.Rollback();
            // Act
            dbContext.Dispose();

            // Assert
            await Assert.ThrowsAsync<ObjectDisposedException>(() => repository.UpdateAndHashUserPassword(nonExistentUser, newPassword));
        }

        [Fact]
        public async Task UpdateAndHashUserPassword_ShouldLogErrorAndThrowExceptionOnDatabaseError()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<IntranetPortalUsersTokenDa> logger = loggerFactory.CreateLogger<IntranetPortalUsersTokenDa>();
            var repository = new IntranetPortalUsersTokenDa(dbContext, logger);
            var user = await dbContext.Users.FindAsync(UserSeedData.FirstUser.Id);
            string newPassword = "newTestPassword123!";

            transaction.Rollback();
            // Simulate a database error
            dbContext.Dispose();

            // Act & Assert
            await Assert.ThrowsAsync<ObjectDisposedException>(() => repository.UpdateAndHashUserPassword(user, newPassword));
            
        }

        [Fact]
        public async Task UpdateIsTokenUsedForUser_ShouldThrowExceptionWhenTokenDoesNotExist()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<IntranetPortalUsersTokenDa> logger = loggerFactory.CreateLogger<IntranetPortalUsersTokenDa>();
            var repository = new IntranetPortalUsersTokenDa(dbContext, logger);
            string userId = UserSeedData.FirstUser.Id;
            string nonExistentToken = "nonExistentToken";

            transaction.Rollback();
            // Act 
            dbContext.Dispose();

            // Assert
            await Assert.ThrowsAsync<ObjectDisposedException>(() => repository.UpdateIsTokenUsedForUser(nonExistentToken, userId));
        }

        [Fact]
        public async Task UpdateIsTokenUsedForUser_ShouldLogErrorAndThrowExceptionOnDatabaseError()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<IntranetPortalUsersTokenDa> logger = loggerFactory.CreateLogger<IntranetPortalUsersTokenDa>();
            var repository = new IntranetPortalUsersTokenDa(dbContext, logger);
            string userId = UserSeedData.FirstUser.Id;
            string token = "testToken";

            transaction.Rollback();
            // Simulate a database error
            dbContext.Dispose();

            // Act & Assert
            await Assert.ThrowsAsync<ObjectDisposedException>(() => repository.UpdateIsTokenUsedForUser(token, userId));
        }

        [Fact]
        public async Task GetUser_ShouldReturnNullWhenUserDoesNotExist()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<IntranetPortalUsersTokenDa> logger = loggerFactory.CreateLogger<IntranetPortalUsersTokenDa>();
            var repository = new IntranetPortalUsersTokenDa(dbContext, logger);
            string nonExistentUserId = "nonexistent";

            // Act
            var result = await repository.GetUser(nonExistentUserId);

            // Assert
            Assert.Null(result);
            transaction.Rollback();
        }

        [Fact]
        public async Task GetUser_ShouldLogErrorAndThrowExceptionOnDatabaseError()
        {
            // Arrange
            using ApplicationDbContext dbContext = _fixture.CreateDbContext();
            dbContext.AuditDisabled = true;
            using IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            _fixture.SeedDatabase(dbContext);
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger<IntranetPortalUsersTokenDa> logger = loggerFactory.CreateLogger<IntranetPortalUsersTokenDa>();
            var repository = new IntranetPortalUsersTokenDa(dbContext, logger);
            string userId = UserSeedData.FirstUser.Id;

            transaction.Rollback();
            // Simulate a database error
            dbContext.Dispose();

            // Act & Assert
            await Assert.ThrowsAsync<ObjectDisposedException>(() => repository.GetUser(userId));
        }
    }
}
