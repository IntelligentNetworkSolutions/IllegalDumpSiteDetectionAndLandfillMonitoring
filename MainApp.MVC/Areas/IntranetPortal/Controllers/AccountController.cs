﻿using DAL.Interfaces.Helpers;
using DAL.Interfaces.Repositories;
using Entities;
using MainApp.MVC.Helpers;
using MainApp.MVC.ViewModels.IntranetPortal.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using SD;
using Services.Interfaces;
using Services.Interfaces.Services;
using System.Security.Claims;
using Westwind.Globalization;

namespace MainApp.MVC.Areas.IntranetPortal.Controllers
{
    [Area("IntranetPortal")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IIntranetPortalUsersTokenDa _intranetPortalUsersTokenDa;
        private readonly IForgotResetPasswordService _forgotResetPasswordService;
        private readonly IAppSettingsAccessor _appSettingsAccessor;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IUserManagementService _userManagementService;

        public AccountController(UserManager<ApplicationUser> userManager,
            IIntranetPortalUsersTokenDa intranetPortalUsersTokenDa,
            IForgotResetPasswordService forgotResetPasswordService,
            IConfiguration configuration,
            IWebHostEnvironment hostingEnvironment,
            IUserManagementService userManagementService,
            IAppSettingsAccessor appSettingsAccessor)

        {
            _userManager = userManager;
            _intranetPortalUsersTokenDa = intranetPortalUsersTokenDa;
            _forgotResetPasswordService = forgotResetPasswordService;
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
            _userManagementService = userManagementService;
            _appSettingsAccessor = appSettingsAccessor;
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
                return Redirect(Url.Action("Index", "Home", new { area = "Common" }));

            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password, bool remember, string returnUrl)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewData["MessageError"] = DbResHtml.T("All fields are required", "Resources");
                ModelState.AddModelError("msgError", ViewData["MessageError"].ToString());
                return View();
            }

            ApplicationUser user = await _userManager.FindByNameAsync(username);
            if (user is null)
            {
                ViewData["MessageError"] = DbResHtml.T("Wrong username", "Resources");
                ModelState.AddModelError("msgError", ViewData["MessageError"].ToString());
                return View();
            }

            bool passwordCheck = await _userManager.CheckPasswordAsync(user, password);
            if (passwordCheck == false)
            {
                ViewData["MessageError"] = DbResHtml.T("Wrong password", "Resources");
                ModelState.AddModelError("msgError", ViewData["MessageError"].ToString());
                return View();
            }

            if (user.IsActive == false)
            {
                ViewData["MessageError"] = DbRes.T("Unable to log in because this user account is currently disabled!", "Resources");
                ModelState.AddModelError("msgError", ViewData["MessageError"].ToString());
                return View();
            }

            var claims = new List<Claim>();
            List<Claim> userIdentityClaims = GetUserIdentityClaims(user);
            claims.AddRange(userIdentityClaims);

            var userClaimsDb = await _userManagementService.GetUserClaims(user.Id);
            foreach (var item in userClaimsDb)
                claims.Add(new Claim(item.ClaimType, item.ClaimValue));

            // TODO: look over
            //if (user.UserName == "superadmin")
            //    claims.Add(new Claim("SpecialAuthClaim", "superadmin"));
            var superAdmin = await _userManagementService.GetSuperAdminUserBySpecificClaim();
            if (user.UserName == superAdmin.UserName)
                claims.Add(new Claim("SpecialAuthClaim", "superadmin"));

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                //AllowRefresh = <bool>, Refreshing the authentication session should be allowed.
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(14), // The time at which the authentication ticket expires.
                                                                // A value set here overrides the ExpireTimeSpan option of CookieAuthenticationOptions set with AddCookie.

                IsPersistent = remember, // Whether the authentication session is persisted across multiple requests.
                                         // When used with cookies, controls whether the cookie's lifetime is absolute
                                         // (matching the lifetime of the authentication ticket) or session-based.

                IssuedUtc = DateTimeOffset.UtcNow // The time at which the authentication ticket was issued.

                //RedirectUri = <string> The full path or absolute URI to be used as an http redirect response value.
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home", new { area = "Common" });
        }

        // TODO: In BL
        public List<Claim> GetUserIdentityClaims(ApplicationUser user)
        {
            List<Claim> claimsIdentity = new List<Claim>();

            if (user is null)
                return claimsIdentity;

            claimsIdentity.Add(new Claim(ClaimTypes.Name, user.Email));
            claimsIdentity.Add(new Claim("FirsName", user.FirstName));
            claimsIdentity.Add(new Claim("Username", user.UserName));
            claimsIdentity.Add(new Claim("UserId", user.Id));
            claimsIdentity.Add(new Claim("LastName", user.LastName));

            return claimsIdentity;
        }

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
            if (!ModelState.IsValid)
                return View(model);

            string? domain = _configuration["DomainSettings:MainDomain"];
            string? webRootPath = _hostingEnvironment.WebRootPath;
            string url;
            string? token = Guid.NewGuid().ToString();
            bool isEmailSend;

            var userByEmail = await _userManager.FindByEmailAsync(model.UsernameOrEmail);
            var userByUsername = await _userManager.FindByNameAsync(model.UsernameOrEmail);

            if (userByEmail != null)
            {
                await _intranetPortalUsersTokenDa.CreateIntranetPortalUserToken(token, userByEmail.Id);
                url = string.Format("https://{0}/IntranetPortal/Account/ResetPassword?userId={1}&token={2}", domain, userByEmail.Id, token);
                isEmailSend = await _forgotResetPasswordService.SendPasswordResetEmail(userByEmail.Email, userByEmail.UserName, url, webRootPath);
                if (isEmailSend)
                    return View("ResetPasswordConfirmation");

                var errorPath = _configuration["ErrorViewsPath:Error"];
                if (!string.IsNullOrEmpty(errorPath))
                    return Redirect(errorPath);

                return BadRequest();
            }
            else if (userByUsername != null)
            {
                await _intranetPortalUsersTokenDa.CreateIntranetPortalUserToken(token, userByUsername.Id);
                url = string.Format("https://{0}/IntranetPortal/Account/ResetPassword?userId={1}&token={2}", domain, userByUsername.Id, token);
                isEmailSend = await _forgotResetPasswordService.SendPasswordResetEmail(userByUsername.Email, userByUsername.UserName, url, webRootPath);
                if (isEmailSend)
                    return View("ResetPasswordConfirmation");

                var errorPath = _configuration["ErrorViewsPath:Error"];
                if (!string.IsNullOrEmpty(errorPath))
                    return Redirect(errorPath);

                return BadRequest();

            }
            else
            {
                var errorPath = _configuration["ErrorViewsPath:Error404"];
                if (!string.IsNullOrEmpty(errorPath))
                    return Redirect(errorPath);

                return NotFound();
            }
        }

        private async Task<(int minLength, bool mustLetters, bool mustNumbers)> GetBasicPasswordRequirements
            (int defultPassMinLength, bool defaultMustHaveLetters, bool defaultMustHaveNumbers)
        {
            ResultDTO<int> resPassMinLength =
                    await _appSettingsAccessor.GetApplicationSettingValueByKey<int>("PasswordMinLength", defultPassMinLength);
            ResultDTO<bool> resPassHasLetters =
                await _appSettingsAccessor.GetApplicationSettingValueByKey<bool>("PasswordMustHaveLetters", defaultMustHaveLetters);
            ResultDTO<bool> resPassHasNumbers =
                await _appSettingsAccessor.GetApplicationSettingValueByKey<bool>("PasswordMustHaveNumbers", defaultMustHaveNumbers);

            int resMinLength = resPassMinLength.IsSuccess ? resPassMinLength.Data : defultPassMinLength;
            bool resMustLetters = resPassHasLetters.IsSuccess ? resPassHasLetters.Data : defaultMustHaveLetters;
            bool resMustNumbers = resPassHasNumbers.IsSuccess ? resPassHasNumbers.Data : defaultMustHaveNumbers;

            return (resMinLength, resMustLetters, resMustNumbers);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(string userId, string token)
        {
            var tokenIsNotUsed = await _intranetPortalUsersTokenDa.IsTokenNotUsed(token, userId);
            if (tokenIsNotUsed)
            {
                (int passMinLength, bool passMustLetters, bool passMustNumbers) =
                    await GetBasicPasswordRequirements(10, true, true);

                IntranetUsersResetPasswordViewModel model = new IntranetUsersResetPasswordViewModel()
                {
                    Token = token,
                    UserId = userId,
                    PasswordMinLength = passMinLength,
                    PasswordMustHaveLetters = passMustLetters,
                    PasswordMustHaveNumbers = passMustNumbers
                };

                return View(model);
            }

            var errorPath = _configuration["ErrorViewsPath:Error403"];
            if (!string.IsNullOrEmpty(errorPath))
                return Redirect(errorPath);

            return StatusCode(403);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(IntranetUsersResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                (int passMinLength, bool passMustLetters, bool passMustNumbers) =
                    await GetBasicPasswordRequirements(10, true, true);

                model.PasswordMinLength = passMinLength;
                model.PasswordMustHaveLetters = passMustLetters;
                model.PasswordMustHaveNumbers = passMustNumbers;

                return View(model);
            }

            var user = await _intranetPortalUsersTokenDa.GetUser(model.UserId);

            await _intranetPortalUsersTokenDa.UpdateAndHashUserPassword(user, model.NewPassword);
            await _intranetPortalUsersTokenDa.UpdateIsTokenUsedForUser(model.Token, model.UserId);

            return RedirectToAction(nameof(Login));
        }

        public async Task<IActionResult> MyProfile()
        {
            var id = User.FindFirstValue("UserId");

            if (string.IsNullOrWhiteSpace(id))
            {
                var errorPath = _configuration["ErrorViewsPath:Error404"];
                if (!string.IsNullOrEmpty(errorPath))
                    return Redirect(errorPath);
                else
                    return NotFound();
            }

            var appUser = await _userManagementService.GetUserById(id);

            (int passMinLength, bool passMustLetters, bool passMustNumbers) =
                    await GetBasicPasswordRequirements(10, true, true);

            MyProfileViewModel model = new MyProfileViewModel()
            {
                FirstName = appUser.FirstName,
                LastName = appUser.LastName,
                Email = appUser.Email,
                Username = appUser.UserName,
                UserId = appUser.Id,
                PasswordMinLength = passMinLength,
                PasswordMustHaveLetters = passMustLetters,
                PasswordMustHaveNumbers = passMustNumbers,
                PreferredLanguage = await _userManagementService.GetPreferredLanguageForUser(appUser.Id)
            };
            return View(model);
        }

        public async Task<string?> GetAllLanguages()
        {
            ResultDTO<string?> resGetMainAppLangs =
                await _appSettingsAccessor.GetApplicationSettingValueByKey<string?>("MainApplicationLanguages");

            return resGetMainAppLangs.IsSuccess ? resGetMainAppLangs.Data : null;
        }

        [HttpPost]
        public async Task<IActionResult> SetCulture(string culture, string returnUrl)
        {
            var id = User.FindFirstValue("UserId");
            if (id == null)
            {
                var errorPath = _configuration["ErrorViewsPath:Error404"];
                if (!string.IsNullOrEmpty(errorPath))
                    return Redirect(errorPath);
                else
                    return NotFound();
            }

            var appUser = await _userManagementService.GetUserById(id);
            if (appUser == null)
            {
                var errorPath = _configuration["ErrorViewsPath:Error404"];
                if (!string.IsNullOrEmpty(errorPath))
                    return Redirect(errorPath);
                else
                    return NotFound();
            }

            await _userManagementService.AddLanguageClaimForUser(appUser.Id, culture);

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
            if (string.IsNullOrWhiteSpace(userId))
                return Json(new { wrongUserId = true });

            if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmNewPassword))
                return Json(new { passwordFieldsEmpty = true });

            if (!password.Equals(confirmNewPassword))
                return Json(new { passwordMissmatch = true });

            ResultDTO result = await _userManagementService.UpdateUserPassword(userId, currentPassword, password);
            if (!result.IsSuccess && ResultDTO.HandleError(result))
                return Json(new { currentPasswordFailed = true });

            return Json(new { passwordUpdatedSuccessfully = true });
        }
    }
}
