using Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.EntitiesTests
{
    public class IntranetPortalUsersTokenTests
    {
        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var applicationUserId = "user123";
            var token = "sampletoken";
            var isTokenUsed = true;

            // Act
            var userToken = new IntranetPortalUsersToken
            {
                ApplicationUserId = applicationUserId,
                Token = token,
                isTokenUsed = isTokenUsed
            };

            // Assert
            Assert.Equal(applicationUserId, userToken.ApplicationUserId);
            Assert.Equal(token, userToken.Token);
            Assert.True(userToken.isTokenUsed);
        }

        [Fact]
        public void ShouldInitializeIsTokenUsedToFalseByDefault()
        {
            // Act
            var userToken = new IntranetPortalUsersToken();

            // Assert
            Assert.False(userToken.isTokenUsed);
        }

        [Fact]
        public void ShouldAllowAssigningNullToApplicationUsers()
        {
            // Arrange
            var userToken = new IntranetPortalUsersToken();

            // Act
            userToken.ApplicationUsers = null;

            // Assert
            Assert.Null(userToken.ApplicationUsers);
        }
    }
}
