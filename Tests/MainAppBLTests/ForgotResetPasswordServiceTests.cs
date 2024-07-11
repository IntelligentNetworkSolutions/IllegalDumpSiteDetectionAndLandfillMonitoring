using DTOs.MainApp.BL;
using MailSend.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.MainAppBLTests
{
    public class ForgotResetPasswordServiceTests
    {
        [Fact]
        public async Task SendPasswordResetEmail_Failure()
        {
            // Arrange
            var mockMailService = new Mock<IMailService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            mockConfiguration.Setup(c => c["MailSettings:Server"]).Returns("test_server");
            mockConfiguration.Setup(c => c["MailSettings:Port"]).Returns("25");
            mockConfiguration.Setup(c => c["MailSettings:Email"]).Returns("test_email@example.com");
            mockConfiguration.Setup(c => c["MailSettings:Password"]).Returns("test_password");

            var service = new ForgotResetPasswordService(mockMailService.Object, mockConfiguration.Object, mockHttpContextAccessor.Object);

            var userEmail = "test@example.com";
            var username = "test_user";
            var url = "http://example.com/reset";
            var root = "/wwwroot";

            // Act
            var result = await service.SendPasswordResetEmail(userEmail, username, url, root);

            // Assert
            Assert.False(result);

            // Verify that SendMail method was not called
            mockMailService.Verify(m => m.SendMail(It.IsAny<SendMailModelDTO>()), Times.Never);
        }

        [Fact]
        public async Task SendPasswordResetEmail_ThrowsException_WhenConfigMissing()
        {
            // Arrange
            var mockMailService = new Mock<IMailService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            var service = new ForgotResetPasswordService(mockMailService.Object, mockConfiguration.Object, mockHttpContextAccessor.Object);

            var userEmail = "test@example.com";
            var username = "test_user";
            var url = "http://example.com/reset";
            var root = "/wwwroot";

            // Act
            var result = await service.SendPasswordResetEmail(userEmail, username, url, root);

            // Assert
            Assert.False(result);

            // Verify that SendMail method was not called
            mockMailService.Verify(m => m.SendMail(It.IsAny<SendMailModelDTO>()), Times.Never);
        }

        [Fact]
        public async Task SendPasswordResetEmail_ThrowsException_WhenTemplateFileNotFound()
        {
            // Arrange
            var mockMailService = new Mock<IMailService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            mockConfiguration.Setup(c => c["MailSettings:Server"]).Returns("test_server");
            mockConfiguration.Setup(c => c["MailSettings:Port"]).Returns("25");
            mockConfiguration.Setup(c => c["MailSettings:Email"]).Returns("test_email@example.com");
            mockConfiguration.Setup(c => c["MailSettings:Password"]).Returns("test_password");

            var service = new ForgotResetPasswordService(mockMailService.Object, mockConfiguration.Object, mockHttpContextAccessor.Object);

            var userEmail = "test@example.com";
            var username = "test_user";
            var url = "http://example.com/reset";
            var root = "/wwwroot";

            // Act
            var result = await service.SendPasswordResetEmail(userEmail, username, url, root);

            // Assert
            Assert.False(result);

            // Verify that SendMail method was not called
            mockMailService.Verify(m => m.SendMail(It.IsAny<SendMailModelDTO>()), Times.Never);
        }       

    }
}
