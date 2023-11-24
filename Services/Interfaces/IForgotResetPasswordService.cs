using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IForgotResetPasswordService
    {
        Task<bool> SendPasswordResetEmail(string userEmail, string username, string url, string root);
    }
}
