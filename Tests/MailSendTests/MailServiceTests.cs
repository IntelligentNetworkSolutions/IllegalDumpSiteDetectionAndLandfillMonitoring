using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs.MainApp.BL;

namespace Tests.MailSendTests
{
    public class MailServiceTests
    {
        // TEST: Integration Tests
        //[Fact]
        //public void SendMail_Successful()
        //{
        //    // Arrange
        //    var sendMailModel = new SendMailModelDTO
        //    {
        //        FromUsername = "sender",
        //        FromEmail = "sender@example.com",
        //        ToUsername = "recipient",
        //        ToEmail = "recipient@example.com",
        //        MessageSubject = "Test Subject",
        //        MessageBody = "<html><body>This is a test email.</body></html>",
        //        SmtpClientModel = new SmtpClientModel
        //        {
        //            Host = "smtp.example.com",
        //            Port = 587, // or any port number you are using
        //            UserName = "smtp_user",
        //            Password = "smtp_password"
        //        }
        //    };

        //    var mailService = new MailSend.MailService();

        //    // Act
        //    var result = mailService.SendMail(sendMailModel, "smtp.example.com", 587, "smtp_user", "smtp_password");

        //    // Assert
        //    Assert.True(result);
        //}

        //[Fact]
        //public void SendMail_Exception()
        //{
        //    // Arrange
        //    var sendMailModel = new SendMailModelDTO
        //    {
        //        // Fill in the properties for the mail model
        //    };

        //    var mailService = new MailSend.MailService();

        //    // Act & Assert
        //    Assert.False(mailService.SendMail(sendMailModel, "invalid_host", 587, "smtp_user", "smtp_password"));
        //}
    }
}
