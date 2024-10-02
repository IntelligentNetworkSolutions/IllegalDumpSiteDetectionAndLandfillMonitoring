using System;
using System.Collections.Generic;
using Entities;
using Microsoft.AspNetCore.Identity;

namespace DAL.ApplicationStorage.SeedDatabase.TestSeedData
{
    public static class UserSeedData
    {
        public static void SeedData(ApplicationDbContext dbContext)
        {
            ArgumentNullException.ThrowIfNull(dbContext);

            dbContext.Roles.AddRange([FirstRole, SecondRole]);
            dbContext.RoleClaims.AddRange([FirstRoleFirstClaim, FirstRoleSecondClaim, SecondRoleFirstClaim, SecondRoleSecondClaim]);

            dbContext.Users.AddRange([FirstUser, SecondUser]);
            dbContext.UserRoles.AddRange([FirstUserFirstRole, SecondUserSecondRole]);

            dbContext.UserClaims.AddRange([FirstUserFirstUserClaim, FirstUserSecondUserClaim]);
            dbContext.UserClaims.AddRange([SecondUserFirstUserClaim, SecondUserSecondUserClaim]);
        }

        public static readonly ApplicationUser FirstUser = new()
        {
            Id = Guid.Parse("3369eab9-4c84-4f7d-aa5b-c5acd4b949b6").ToString(),
            UserName = "firsttestuser",
            NormalizedUserName = "firsttestuser".ToUpper(),
            FirstName = "First",
            LastName = "Test User",
            Email = "firsttestuser@test.com",
            NormalizedEmail = "firsttestuser@test.com".ToUpper(),
            IsActive = true,
            EmailConfirmed = true,
        };

        public static readonly ApplicationUser SecondUser = new()
        {
            Id = Guid.Parse("86a7c85a-faa5-444d-bbdc-c2d26962d8e8").ToString(),
            UserName = "secondtestuser",
            NormalizedUserName = "secondtestuser".ToUpper(),
            FirstName = "Second",
            LastName = "Test User",
            Email = "secondtestuser@test.com",
            NormalizedEmail = "secondtestuser@test.com".ToUpper(),
            IsActive = true,
            EmailConfirmed = true,
        };

        public static readonly IdentityRole FirstRole = new()
        {
            Id = Guid.Parse("907a8bfb-ce4d-4ff1-9c56-7b8708a7c5f0").ToString(),
            Name = "First Role",
            NormalizedName = "First Role".ToUpper()
        };

        public static readonly IdentityRole SecondRole = new()
        {
            Id = Guid.Parse("ac8e23ca-d15e-4df8-8a9e-4e0e07d2483c").ToString(),
            Name = "Second Role",
            NormalizedName = "Second Role".ToUpper()
        };

        public static readonly IdentityUserRole<string> FirstUserFirstRole = new() { RoleId = FirstRole.Id, UserId = FirstUser.Id,};

        public static readonly IdentityUserRole<string> SecondUserSecondRole = new() { RoleId = SecondRole.Id, UserId = SecondUser.Id,};

        public static readonly IdentityRoleClaim<string> FirstRoleFirstClaim = 
            new() { Id = 1, RoleId = FirstRole.Id, ClaimType = "AuthClaim", ClaimValue = "First Role First Claim" };
        public static readonly IdentityRoleClaim<string> FirstRoleSecondClaim =
            new() { Id = 2, RoleId = FirstRole.Id, ClaimType = "AuthClaim", ClaimValue = "First Role Second Claim" };

        public static readonly IdentityRoleClaim<string> SecondRoleFirstClaim =
            new() { Id = 3, RoleId = FirstRole.Id, ClaimType = "AuthClaim", ClaimValue = "Second Role First Claim" };
        public static readonly IdentityRoleClaim<string> SecondRoleSecondClaim =
            new() { Id = 4, RoleId = FirstRole.Id, ClaimType = "AuthClaim", ClaimValue = "Second Role Second Claim" };

        public static readonly IdentityUserClaim<string> FirstUserFirstUserClaim = 
            new() { Id = 1, UserId = FirstUser.Id, ClaimValue = "First User First Claim", ClaimType = "AuthClaim" };
        public static readonly IdentityUserClaim<string> FirstUserSecondUserClaim = 
            new() { Id = 2, UserId = FirstUser.Id, ClaimValue = "First User Second Claim", ClaimType = "AuthClaim" };

        public static readonly IdentityUserClaim<string> SecondUserFirstUserClaim =
            new() { Id = 3, UserId = FirstUser.Id, ClaimValue = "Second User First Claim", ClaimType = "AuthClaim" };
        public static readonly IdentityUserClaim<string> SecondUserSecondUserClaim =
            new() { Id = 4, UserId = FirstUser.Id, ClaimValue = "Second User Second Claim", ClaimType = "AuthClaim" };
    }
}
