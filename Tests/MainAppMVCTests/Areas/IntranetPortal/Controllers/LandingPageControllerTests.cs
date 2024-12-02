using MailSend.Interfaces;
using MainApp.MVC.Areas.IntranetPortal.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Tests.MainAppMVCTests.Areas.IntranetPortal.Controllers
{
    public class LandingPageControllerTests
    {
        private readonly LandingPageController _controller;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IMailService> _mockMailService;

        public LandingPageControllerTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockMailService = new Mock<IMailService>();
            _controller = new LandingPageController
                (
                _mockConfiguration.Object,
                _mockMailService.Object
                );
        }

        [Fact]
        public void Index_ReturnsViewResult()
        {
            // Act
            var result = _controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        //[Fact]
        //public void ContactForm_Post_ValidData_RedirectsToIndex()
        //{
        //    // Arrange
        //    var formData = new { name = "Test Name", email = "test@example.com", message = "Hello" };

        //    var mailSettings = new MailSettingsDTO
        //    {
        //        Email = "admin@example.com",
        //        Server = "smtp.example.com",
        //        Port = 587,
        //        Password = "testpassword"
        //    };

        //    // Mock configuration to return MailSettings values
        //    _mockConfiguration
        //        .Setup(c => c.GetSection("MailSettings").Get<MailSettingsDTO>())
        //        .Returns(mailSettings);

        //    // Mock _mailService to simulate successful email sending
        //    _mockMailService
        //        .Setup(m => m.SendMail(It.IsAny<SendMailModelDTO>()))
        //        .Returns(true);

        //    // Act
        //    var result = _controller.ContactForm(formData.name, formData.email, formData.message);

        //    // Assert
        //    var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        //    Assert.Equal("Index", redirectResult.ActionName);
        //    Assert.True(_controller.TempData.ContainsKey("SuccessMessage"));
        //    Assert.Equal("Your message has been sent successfully.", _controller.TempData["SuccessMessage"]);
        //}

        //[Fact]
        //public void ContactForm_Post_EmptyMessage_ReturnsRedirectToIndex()
        //{
        //    // Arrange
        //    var formData = new { name = "Test Name", email = "test@example.com", message = "" };

        //    // Act
        //    var result = _controller.ContactForm(formData.name, formData.email, formData.message);

        //    // Assert
        //    var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        //    Assert.Equal("Index", redirectResult.ActionName);
        //}

        [Fact]
        public void Index_ReturnsAllowAnonymousAttribute()
        {
            // Act
            var allowAnonymous = _controller.GetType().GetMethod("Index").GetCustomAttributes(typeof(AllowAnonymousAttribute), true);

            // Assert
            Assert.NotNull(allowAnonymous);
            Assert.True(allowAnonymous.Length > 0);
        }

        [Fact]
        public void Index_ReturnsNotNullViewResult()
        {
            // Act
            var result = _controller.Index();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
        }

    }
}
