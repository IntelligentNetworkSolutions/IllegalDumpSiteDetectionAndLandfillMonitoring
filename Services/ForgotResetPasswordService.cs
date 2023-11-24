using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using MailSend;
using MailSend.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Models.DTOs;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ForgotResetPasswordService : IForgotResetPasswordService
    {
        private readonly IMailService _mailService;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
       

        public ForgotResetPasswordService(IMailService mailService, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _mailService = mailService;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<bool> SendPasswordResetEmail(string userEmail, string username, string url, string root)
        {
            try
            {

                var server = _configuration["MailSettings:Server"];
                var port = Int32.Parse(_configuration["MailSettings:Port"]);
                var email = _configuration["MailSettings:Email"];
                var password = _configuration["MailSettings:Password"];

                var smtpClientModel = new SmtpClientModelDTO
                {
                    Host = server,
                    Port = port,
                    UseSsl = true,
                    UserName = email,
                    Password = password

                };
                var webRootPath = root;
                var filename = "ResetPasswordTemplate.html";
                var pathName = Path.Combine(webRootPath, "Templates", filename);
                string contents = System.IO.File.ReadAllText(pathName);
                contents = contents.Replace("{year}", DateTime.Now.Year.ToString());
                contents = contents.Replace("{username}", username);
                contents = contents.Replace("{link}", "<a href='" + url + "'>link</a>");
                var resetMessage = contents;            
                var mailSubject = "Reset Password";

                SendMailModelDTO sendMailModelDto = new SendMailModelDTO
                {                    
                    ToEmail = userEmail,
                    ToUsername = username,
                    MessageSubject = mailSubject,
                    MessageBody = resetMessage,
                    SmtpClientModel = smtpClientModel,
                    FromEmail = email,
                    FromUsername = "UNICEF Waste Detection",
                    MessageBodyPlain = resetMessage
                };

                _mailService.SendMail(sendMailModelDto);
                return true;

            }
            catch (Exception)
            {
                return false;

            }
        }
    }
}
