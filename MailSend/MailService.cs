﻿using DTOs.MainApp.BL;
using MailKit.Net.Smtp;
using MailSend.Interfaces;
using MimeKit;

namespace MailSend
{
    public class MailService : IMailService
    {
        public bool SendMail(SendMailModelDTO sendMailModel)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(sendMailModel.FromUsername, sendMailModel.FromEmail));
                message.To.Add(new MailboxAddress(sendMailModel.ToUsername, sendMailModel.ToEmail));
                message.Subject = sendMailModel.MessageSubject;

                var builder = new BodyBuilder
                {
                    HtmlBody = sendMailModel.MessageBody,
                };
                message.Body = builder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    client.Connect(sendMailModel.SmtpClientModel.Host, sendMailModel.SmtpClientModel.Port, false);
                    client.Authenticate(sendMailModel.SmtpClientModel.UserName, sendMailModel.SmtpClientModel.Password);
                    client.Send(message);
                    client.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                // TODO: 🤔 Review to see if exception should be thrown 🤔🤔🤔
                throw;
            }

            return true;
        }
    }
}
