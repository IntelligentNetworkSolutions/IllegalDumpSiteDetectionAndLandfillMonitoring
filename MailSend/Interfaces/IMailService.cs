using DTOs.MainApp.BL;

namespace MailSend.Interfaces
{
    public interface IMailService
    {
        bool SendMail(SendMailModelDTO sendMailModel);
    }
}
