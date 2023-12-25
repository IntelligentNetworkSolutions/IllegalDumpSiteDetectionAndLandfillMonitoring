using MainApp.MVC.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Entities;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using SD.Helpers;
using Dal;
using Services;
using Services.Interfaces;
using Dal.Helpers;
using Microsoft.AspNetCore.Localization;
using Westwind.Globalization;
using Microsoft.EntityFrameworkCore.Metadata;
using MainApp.MVC.ViewModels.IntranetPortal.Account;
using DAL.Repositories;

namespace MainApp.MVC.Areas.IntranetPortal.Controllers
{
    [Area("IntranetPortal")]
	public class AccountController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly AddClaimsForIntranetPortalUserHelper _addClaimsForIntranetPortalUserHelper;
		private readonly UserManagementDa _userManagementDa;
		private readonly IntranetPortalUsersTokenDa _intranetPortalUsersTokenDa;
		private readonly IForgotResetPasswordService _forgotResetPasswordService;
		private readonly ApplicationSettingsHelper _applicationSettingsHelper;
		private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public AccountController(UserManager<ApplicationUser> userManager, AddClaimsForIntranetPortalUserHelper addClaimsForIntranetPortalUserHelper, UserManagementDa userManaagementDa, IntranetPortalUsersTokenDa intranetPortalUsersTokenDa, IForgotResetPasswordService forgotResetPasswordService, ApplicationSettingsHelper applicationSettingsHelper, IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
		{
			_userManager = userManager;
			_addClaimsForIntranetPortalUserHelper = addClaimsForIntranetPortalUserHelper;
			_userManagementDa = userManaagementDa;
			_intranetPortalUsersTokenDa = intranetPortalUsersTokenDa;
            _forgotResetPasswordService = forgotResetPasswordService;
			_applicationSettingsHelper = applicationSettingsHelper;
			_configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
		}
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }
        [AllowAnonymous]
		public IActionResult Login()
		{
            if (User.Identity.IsAuthenticated)
            {
                return Redirect(Url.Content("~"));
            }
            return View();
		}

        /*
        //TODO
        [HttpPost]
        [AllowAnonymous]	
		public async Task<IActionResult> Login(string username, string password, bool remember, string returnUrl)
		{
			var user = await _userManager.FindByNameAsync(username);
			if(user == null)
			{
				ViewData["MessageError"] = DbResHtml.T("Wrong username", "Resources");
				ModelState.AddModelError("msgError", ViewData["MessageError"].ToString());
				return View();
			}			

            var passwordCheck = await _userManager.CheckPasswordAsync(user, password);
            if (passwordCheck)
			{
                if (user.IsActive == false)
                {
                    ViewData["MessageError"] = DbRes.T("Unable to log in because this user account is currently disabled!", "Resources");
                    ModelState.AddModelError("msgError", ViewData["MessageError"].ToString());
                    return View();
                }
                var claims = new List<Claim>();
				_addClaimsForIntranetPortalUserHelper.AddClaims(claims, user);
				var userClaimsDb = await _userManagementDa.GetIntranetUserClaimsDb(user.Id);
                foreach (var item in userClaimsDb)
                {
                    claims.Add(new Claim(item.ClaimType, item.ClaimValue));

                }

                var claimsIdentity = new ClaimsIdentity(
			   claims, CookieAuthenticationDefaults.AuthenticationScheme);

				var authProperties = new AuthenticationProperties
				{
					//AllowRefresh = <bool>,
					// Refreshing the authentication session should be allowed.

					ExpiresUtc = DateTimeOffset.UtcNow.AddDays(14),
					// The time at which the authentication ticket expires. A 
					// value set here overrides the ExpireTimeSpan option of 
					// CookieAuthenticationOptions set with AddCookie.

					IsPersistent = remember,
					// Whether the authentication session is persisted across 
					// multiple requests. When used with cookies, controls
					// whether the cookie's lifetime is absolute (matching the
					// lifetime of the authentication ticket) or session-based.

					IssuedUtc = DateTimeOffset.UtcNow
					// The time at which the authentication ticket was issued.

					//RedirectUri = <string>
					// The full path or absolute URI to be used as an http 
					// redirect response value.
				};

				await HttpContext.SignInAsync(
					CookieAuthenticationDefaults.AuthenticationScheme,
					new ClaimsPrincipal(claimsIdentity),
					authProperties);

               if(!string.IsNullOrEmpty(returnUrl))
			   {
                    return Redirect(returnUrl);
               }
			   return RedirectToAction("Index", "Home", new {area="Common"});

            }
            else
            {
                ViewData["MessageError"] = DbResHtml.T("Wrong password", "Resources");
                ModelState.AddModelError("msgError", ViewData["MessageError"].ToString());
                return View();
            }			
		}
        */

		[AllowAnonymous]
        public async Task<IActionResult> ForgotPassword()
        {
            IntranetUsersForgotPasswordViewModel model = new IntranetUsersForgotPasswordViewModel();
            return View(model);
        }

		[HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> ForgotPassword(IntranetUsersForgotPasswordViewModel model)
		{     
            if (ModelState.IsValid)
			{
                var domain = _configuration["DomainSettings:MainDomain"];
                var webRootPath = _hostingEnvironment.WebRootPath;
                string url;
                var token = Guid.NewGuid().ToString();
                bool isEmailSend;

                var userByEmail = await _userManager.FindByEmailAsync(model.UsernameOrEmail);
				var userByUsername = await _userManager.FindByNameAsync(model.UsernameOrEmail);

				if (userByEmail != null)
				{
					await _intranetPortalUsersTokenDa.CreateIntranetPortalUserToken(token, userByEmail.Id);
                    url = string.Format("https://{0}/IntranetPortal/Account/ResetPassword?userId={1}&token={2}", domain, userByEmail.Id, token);
                    isEmailSend = await _forgotResetPasswordService.SendPasswordResetEmail(userByEmail.Email, userByEmail.UserName, url, webRootPath);
                    if (isEmailSend)
                    {
                        return View("ResetPasswordConfirmation");
                    }
                    else
                    {
                        var errorPath = _configuration["ErrorViewsPath:Error"];
                        if (!string.IsNullOrEmpty(errorPath))
                        {
                            return Redirect(errorPath);
                        }
                        else
                        {
                            return BadRequest();
                        }
                    }
                } 
                else if(userByUsername != null)
				{
                    await _intranetPortalUsersTokenDa.CreateIntranetPortalUserToken(token, userByUsername.Id);                    
                    url = string.Format("https://{0}/IntranetPortal/Account/ResetPassword?userId={1}&token={2}", domain, userByUsername.Id, token);
                    isEmailSend = await _forgotResetPasswordService.SendPasswordResetEmail(userByUsername.Email, userByUsername.UserName, url, webRootPath);
                    if (isEmailSend)
                    {
                        return View("ResetPasswordConfirmation");
                    }
                    else
                    {
                        var errorPath = _configuration["ErrorViewsPath:Error"];
                        if (!string.IsNullOrEmpty(errorPath))
                        {
                            return Redirect(errorPath);
                        }
                        else
                        {
                            return BadRequest();
                        }
                    }
                        
                }
				else
				{
                    var errorPath = _configuration["ErrorViewsPath:Error404"];
                    if (!string.IsNullOrEmpty(errorPath))
                    {                        
                        return Redirect(errorPath);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
			}
			return View(model);
		}

        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(string userId, string token)
        {
			var tokenIsNotUsed = await _intranetPortalUsersTokenDa.IsTokenNotUsed(token, userId);
            if (tokenIsNotUsed)
            {
                IntranetUsersResetPasswordViewModel model = new IntranetUsersResetPasswordViewModel();
                model.Token = token;
                model.UserId = userId;
                model.PasswordMinLength = _applicationSettingsHelper.GetApplicationSettingInteger("PasswordMinLength", "10");
                model.PasswordMustHaveLetters = _applicationSettingsHelper.GetApplicationSettingBool("PasswordMustHaveLetters", "true");
                model.PasswordMustHaveNumbers = _applicationSettingsHelper.GetApplicationSettingBool("PasswordMustHaveNumbers", "true");
                return View(model);
            }
            var errorPath = _configuration["ErrorViewsPath:Error403"];
            if (!string.IsNullOrEmpty(errorPath))
            {
                return Redirect(errorPath);
            }
            else
            {
                return StatusCode(403);
            }

        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(IntranetUsersResetPasswordViewModel model)
        {           
            if (ModelState.IsValid)
			{
                var user = await _intranetPortalUsersTokenDa.GetUser(model.UserId);
                await _intranetPortalUsersTokenDa.UpdateAndHashUserPassword(user, model.NewPassword);
                await _intranetPortalUsersTokenDa.UpdateIsTokenUsedForUser(model.Token, model.UserId);
                return RedirectToAction(nameof(Login));
            }
            model.PasswordMinLength = _applicationSettingsHelper.GetApplicationSettingInteger("PasswordMinLength", "10");
            model.PasswordMustHaveLetters = _applicationSettingsHelper.GetApplicationSettingBool("PasswordMustHaveLetters", "true");
            model.PasswordMustHaveNumbers = _applicationSettingsHelper.GetApplicationSettingBool("PasswordMustHaveNumbers", "true");
            return View(model);           
        }
        
        public async Task<IActionResult> MyProfile()
        {
            var id = User.FindFirstValue("UserId");
            if(id != null)
            {
                var appUser = await _userManagementDa.GetUser(id);
                MyProfileViewModel model = new MyProfileViewModel();
                model.FirstName = appUser.FirstName;
                model.LastName = appUser.LastName;
                model.Email = appUser.Email;
                model.Username = appUser.UserName;
                model.UserId = appUser.Id;
                model.PasswordMinLength = _applicationSettingsHelper.GetApplicationSettingInteger("PasswordMinLength", "10");
                model.PasswordMustHaveLetters = _applicationSettingsHelper.GetApplicationSettingBool("PasswordMustHaveLetters", "true");
                model.PasswordMustHaveNumbers = _applicationSettingsHelper.GetApplicationSettingBool("PasswordMustHaveNumbers", "true");      
                model.PreferredLanguage = await _userManagementDa.GetPreferredLanguage(appUser.Id);
                return View(model);
            }
            else
            {
                var errorPath = _configuration["ErrorViewsPath:Error404"];
                if (!string.IsNullOrEmpty(errorPath))
                {
                    return Redirect(errorPath);
                }
                else
                {
                    return NotFound();
                }
            }
           
        }
        public async Task<string> GetAllLanguages()
        {
            return _applicationSettingsHelper.GetApplicationSettingString("MainApplicationLanguages");
        }

        [HttpPost]
        public async Task<IActionResult> SetCulture(string culture, string returnUrl)
        {
            var id = User.FindFirstValue("UserId");
            if(id == null)
            {
                var errorPath = _configuration["ErrorViewsPath:Error404"];
                if (!string.IsNullOrEmpty(errorPath))
                {
                    return Redirect(errorPath);
                }
                else
                {
                    return NotFound();
                }
            }
            var appUser = await _userManagementDa.GetUser(id);
            if(appUser == null)
            {
                var errorPath = _configuration["ErrorViewsPath:Error404"];
                if (!string.IsNullOrEmpty(errorPath))
                {
                    return Redirect(errorPath);
                }
                else
                {
                    return NotFound();
                }
            }

            await _userManagementDa.AddLanguageClaimForUser(appUser.Id, culture);

            HttpContext.Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture("en", culture)),
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    SameSite = SameSiteMode.Strict
                });

            return Redirect(returnUrl);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePassword(string currentPassword, string password, string confirmNewPassword, string userId)
        {            

            if (!string.IsNullOrEmpty(userId))
            {
                MyProfileViewModel model = new MyProfileViewModel();
                model.PasswordMinLength = _applicationSettingsHelper.GetApplicationSettingInteger("PasswordMinLength", "10");
                model.PasswordMustHaveLetters = _applicationSettingsHelper.GetApplicationSettingBool("PasswordMustHaveLetters", "true");
                model.PasswordMustHaveNumbers = _applicationSettingsHelper.GetApplicationSettingBool("PasswordMustHaveNumbers", "true");
                var userDb = await _userManagementDa.GetUser(userId);  
                
                if(userDb != null)
                {
                    model.FirstName = userDb.FirstName;
                    model.LastName = userDb.LastName;
                    model.Email = userDb.Email;
                    model.Username = userDb.UserName;
                    model.UserId = userDb.Id;
                    model.PreferredLanguage = await _userManagementDa.GetPreferredLanguage(userDb.Id);
                    if (!string.IsNullOrEmpty(currentPassword) && !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(confirmNewPassword))
                    {
                        var passwordHasher = new PasswordHasher<ApplicationUser>();
                        var hashedPasswordViewModel = passwordHasher.HashPassword(userDb, password);
                        var result = passwordHasher.VerifyHashedPassword(userDb, userDb.PasswordHash, currentPassword);
                        if (result == PasswordVerificationResult.Failed)
                        {
                            return Json(new { currentPasswordFailed = true });
                        }
                        else
                        {
                            userDb.PasswordHash = hashedPasswordViewModel;
                            await _userManagementDa.UpdateUser(userDb);
                            return Json(new { passwordUpdatedSuccessfully = true });
                        }

                    }
                    else
                    {
                        return Json(new { paswwordFieldsEmpty = true });
                    }
                }
                else
                {
                    return Json(new { userDbNotFound = true });
                }

            }
            else
            {
                return Json(new { wrongUserId = true });
            }           
        }
    }
}
