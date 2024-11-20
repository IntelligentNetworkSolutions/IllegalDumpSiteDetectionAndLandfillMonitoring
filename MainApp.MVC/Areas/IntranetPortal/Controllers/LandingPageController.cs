using DTOs.MainApp.BL;
using MailSend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MainApp.MVC.Areas.IntranetPortal.Controllers
{
    [Area("IntranetPortal")]
    public class LandingPageController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IMailService _mailService;
        public LandingPageController(IConfiguration configuration, IMailService mailService)
        {
            _configuration = configuration;
            _mailService = mailService;
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ContactForm(string name, string email, string message)
        {
            try
            {
                // Get MailSettings from appsettings.json
                var mailSettings = _configuration.GetSection("MailSettings").Get<MailSettingsDTO>();

                // Prepare the email model
                var mailModel = new SendMailModelDTO
                {
                    FromUsername = name,
                    FromEmail = email,
                    ToUsername = "Admin", // Replace with the recipient's name
                    ToEmail = mailSettings.Email, // Email of the admin
                    MessageSubject = "Contact Form Submission",
                    MessageBodyPlain = $"Name: {name}\nEmail: {email}\nMessage: {message}",
                    MessageBody = $"<p><strong>Name:</strong> {name}</p>" +
                                  $"<p><strong>Email:</strong> {email}</p>" +
                                  $"<p><strong>Message:</strong> {message}</p>",
                    SmtpClientModel = new SmtpClientModelDTO
                    {
                        Host = mailSettings.Server,
                        Port = mailSettings.Port,
                        UserName = mailSettings.Email,
                        Password = mailSettings.Password,
                        UseSsl = true
                    }
                };

                // Send the email
                var emailSent = _mailService.SendMail(mailModel);

                // Provide feedback to the user
                if (emailSent)
                {
                    TempData["SuccessMessage"] = "Your message has been sent successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to send your message. Please try again later.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while sending your message.";
                Console.WriteLine($"Error: {ex.Message}");
            }

            return RedirectToAction("Index");
        }
    }


}
