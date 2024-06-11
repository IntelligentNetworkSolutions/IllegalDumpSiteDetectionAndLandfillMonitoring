using DTOs.MainApp.BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.DTOsTests.MainAppBL
{
    public class SendMailModelDTOTests
    {
        [Fact]
        public void SendMailModelDTO_Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var fromUsername = "fromUser";
            var fromEmail = "from@example.com";
            var toUsername = "toUser";
            var toEmail = "to@example.com";
            var messageSubject = "Test Subject";
            var messageBodyPlain = "This is a plain text message.";
            var messageBody = "This is an HTML message.";
            var smtpClientModel = new SmtpClientModelDTO();

            // Act
            var dto = new SendMailModelDTO
            {
                FromUsername = fromUsername,
                FromEmail = fromEmail,
                ToUsername = toUsername,
                ToEmail = toEmail,
                MessageSubject = messageSubject,
                MessageBodyPlain = messageBodyPlain,
                MessageBody = messageBody,
                SmtpClientModel = smtpClientModel
            };

            // Assert
            Assert.Equal(fromUsername, dto.FromUsername);
            Assert.Equal(fromEmail, dto.FromEmail);
            Assert.Equal(toUsername, dto.ToUsername);
            Assert.Equal(toEmail, dto.ToEmail);
            Assert.Equal(messageSubject, dto.MessageSubject);
            Assert.Equal(messageBodyPlain, dto.MessageBodyPlain);
            Assert.Equal(messageBody, dto.MessageBody);
            Assert.Equal(smtpClientModel, dto.SmtpClientModel);
        }

        [Fact]
        public void SendMailModelDTO_DefaultConstructor_ShouldInitializePropertiesToDefaultValues()
        {
            // Act
            var dto = new SendMailModelDTO();

            // Assert
            Assert.Null(dto.FromUsername);
            Assert.Null(dto.FromEmail);
            Assert.Null(dto.ToUsername);
            Assert.Null(dto.ToEmail);
            Assert.Null(dto.MessageSubject);
            Assert.Null(dto.MessageBodyPlain);
            Assert.Null(dto.MessageBody);
            Assert.Null(dto.SmtpClientModel);
        }

        [Fact]
        public void SendMailModelDTO_Properties_ShouldBeSettable()
        {
            // Arrange
            var dto = new SendMailModelDTO();

            // Act
            dto.FromUsername = "fromUser";
            dto.FromEmail = "from@example.com";
            dto.ToUsername = "toUser";
            dto.ToEmail = "to@example.com";
            dto.MessageSubject = "Test Subject";
            dto.MessageBodyPlain = "This is a plain text message.";
            dto.MessageBody = "This is an HTML message.";
            dto.SmtpClientModel = new SmtpClientModelDTO();

            // Assert
            Assert.Equal("fromUser", dto.FromUsername);
            Assert.Equal("from@example.com", dto.FromEmail);
            Assert.Equal("toUser", dto.ToUsername);
            Assert.Equal("to@example.com", dto.ToEmail);
            Assert.Equal("Test Subject", dto.MessageSubject);
            Assert.Equal("This is a plain text message.", dto.MessageBodyPlain);
            Assert.Equal("This is an HTML message.", dto.MessageBody);
            Assert.NotNull(dto.SmtpClientModel);
        }

        [Fact]
        public void SendMailModelDTO_NullableProperties_ShouldAcceptNullValues()
        {
            // Arrange
            var dto = new SendMailModelDTO
            {
                FromUsername = null,
                FromEmail = null,
                ToUsername = null,
                ToEmail = null,
                MessageSubject = null,
                MessageBodyPlain = null,
                MessageBody = null,
                SmtpClientModel = null
            };

            // Assert
            Assert.Null(dto.FromUsername);
            Assert.Null(dto.FromEmail);
            Assert.Null(dto.ToUsername);
            Assert.Null(dto.ToEmail);
            Assert.Null(dto.MessageSubject);
            Assert.Null(dto.MessageBodyPlain);
            Assert.Null(dto.MessageBody);
            Assert.Null(dto.SmtpClientModel);
        }
    }
}
