using MainApp.MVC.Areas.IntranetPortal.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.MainAppMVCTests.Areas.IntranetPortal.Controllers
{
    public class LandingPageControllerTests
    {
        private readonly LandingPageController _controller;

        public LandingPageControllerTests()
        {
            _controller = new LandingPageController();
        }

        [Fact]
        public void Index_ReturnsViewResult()
        {
            // Act
            var result = _controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void ContactForm_Post_ValidData_RedirectsToIndex()
        {
            // Arrange
            var formData = new { name = "Test Name", email = "test@example.com", message = "Hello" };

            // Act
            var result = _controller.ContactForm(formData.name, formData.email, formData.message);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        
    }
}
