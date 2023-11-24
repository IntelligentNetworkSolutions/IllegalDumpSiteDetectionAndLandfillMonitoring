using Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailSend.Interfaces
{
    public interface IMailService
    {
        bool SendMail(SendMailModelDTO sendMailModel);
    }
}
