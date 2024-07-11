using MainApp.MVC.Areas.IntranetPortal.Controllers;
using Microsoft.AspNetCore.Authorization;
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

        [Fact]
        public void ContactForm_Post_EmptyMessage_ReturnsRedirectToIndex()
        {
            // Arrange
            var formData = new { name = "Test Name", email = "test@example.com", message = "" };

            // Act
            var result = _controller.ContactForm(formData.name, formData.email, formData.message);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

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
