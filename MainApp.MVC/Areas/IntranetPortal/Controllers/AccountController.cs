using DAL.Interfaces.Helpers;
using DAL.Interfaces.Repositories;
using DTOs.MainApp.BL;
using DTOs.MainApp.BL.TrainingDTOs;
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
            try
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    ViewData["MessageError"] = DbResHtml.T("All fields are required", "Resources");
                    ModelState.AddModelError("msgError", ViewData["MessageError"].ToString());
                    return View();
                }

                ApplicationUser? user = await _userManager.FindByNameAsync(username);
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

                ResultDTO<List<UserClaimDTO>>? resultGetUserClaims = await _userManagementService.GetUserClaims(user.Id);
                if (resultGetUserClaims.IsSuccess == false && resultGetUserClaims.HandleError())
                    return HandleErrorRedirect("ErrorViewsPath:Error", 400);
                if (resultGetUserClaims.Data == null)
                    return HandleErrorRedirect("ErrorViewsPath:Error404", 404);


                foreach (var item in resultGetUserClaims.Data)
                    claims.Add(new Claim(item.ClaimType, item.ClaimValue));

                // TODO: look over
                //if (user.UserName == "superadmin")
                //    claims.Add(new Claim("SpecialAuthClaim", "superadmin"));
                ResultDTO<UserDTO>? resultGetSuperAdmin = await _userManagementService.GetSuperAdminUserBySpecificClaim();
                if (resultGetSuperAdmin.IsSuccess == false && resultGetSuperAdmin.HandleError())
                    return HandleErrorRedirect("ErrorViewsPath:Error", 400);
                if (resultGetSuperAdmin.Data == null)
                    return HandleErrorRedirect("ErrorViewsPath:Error404", 404);

                if (user.UserName == resultGetSuperAdmin.Data.UserName)
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
            catch (Exception)
            {
                return HandleErrorRedirect("ErrorViewsPath:Error", 400);
            }
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

            ApplicationUser? userByEmail = await _userManager.FindByEmailAsync(model.UsernameOrEmail);
            ApplicationUser? userByUsername = await _userManager.FindByNameAsync(model.UsernameOrEmail);

            if (userByEmail != null)
            {
                await _intranetPortalUsersTokenDa.CreateIntranetPortalUserToken(token, userByEmail.Id);
                url = string.Format("https://{0}/IntranetPortal/Account/ResetPassword?userId={1}&token={2}", domain, userByEmail.Id, token);
                isEmailSend = await _forgotResetPasswordService.SendPasswordResetEmail(userByEmail.Email, userByEmail.UserName, url, webRootPath);
                if (isEmailSend)
                    return View("ResetPasswordConfirmation");
                else
                    return HandleErrorRedirect("ErrorViewsPath:Error", 400);

            }
            else if (userByUsername != null)
            {
                await _intranetPortalUsersTokenDa.CreateIntranetPortalUserToken(token, userByUsername.Id);
                url = string.Format("https://{0}/IntranetPortal/Account/ResetPassword?userId={1}&token={2}", domain, userByUsername.Id, token);
                isEmailSend = await _forgotResetPasswordService.SendPasswordResetEmail(userByUsername.Email, userByUsername.UserName, url, webRootPath);
                if (isEmailSend)
                    return View("ResetPasswordConfirmation");
                else
                    return HandleErrorRedirect("ErrorViewsPath:Error", 400);

            }
            else
            {
                return HandleErrorRedirect("ErrorViewsPath:Error404", 404);
            }
        }

        private async Task<(int minLength, bool mustLetters, bool mustNumbers)> GetBasicPasswordRequirements
            (int defultPassMinLength, bool defaultMustHaveLetters, bool defaultMustHaveNumbers)
        {
            ResultDTO<int>? resPassMinLength =
                    await _appSettingsAccessor.GetApplicationSettingValueByKey<int>("PasswordMinLength", defultPassMinLength);
            ResultDTO<bool>? resPassHasLetters =
                await _appSettingsAccessor.GetApplicationSettingValueByKey<bool>("PasswordMustHaveLetters", defaultMustHaveLetters);
            ResultDTO<bool>? resPassHasNumbers =
                await _appSettingsAccessor.GetApplicationSettingValueByKey<bool>("PasswordMustHaveNumbers", defaultMustHaveNumbers);

            int resMinLength = resPassMinLength.IsSuccess ? resPassMinLength.Data : defultPassMinLength;
            bool resMustLetters = resPassHasLetters.IsSuccess ? resPassHasLetters.Data : defaultMustHaveLetters;
            bool resMustNumbers = resPassHasNumbers.IsSuccess ? resPassHasNumbers.Data : defaultMustHaveNumbers;

            return (resMinLength, resMustLetters, resMustNumbers);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(string userId, string token)
        {
            bool tokenIsNotUsed = await _intranetPortalUsersTokenDa.IsTokenNotUsed(token, userId);
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

            return HandleErrorRedirect("ErrorViewsPath:Error403", 403);
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

            ApplicationUser? user = await _intranetPortalUsersTokenDa.GetUser(model.UserId);
            if (user == null)
                return HandleErrorRedirect("ErrorViewsPath:Error404", 404);

            await _intranetPortalUsersTokenDa.UpdateAndHashUserPassword(user, model.NewPassword);
            await _intranetPortalUsersTokenDa.UpdateIsTokenUsedForUser(model.Token, model.UserId);

            return RedirectToAction(nameof(Login));
        }

        public async Task<IActionResult> MyProfile()
        {
            try
            {
                string? id = User.FindFirstValue("UserId");

                if (string.IsNullOrWhiteSpace(id))
                    return HandleErrorRedirect("ErrorViewsPath:Error404", 404);

                ResultDTO<UserDTO>? resultGetAppUser = await _userManagementService.GetUserById(id);
                if (resultGetAppUser.IsSuccess == false && resultGetAppUser.HandleError())
                    return HandleErrorRedirect("ErrorViewsPath:Error", 400);

                if (resultGetAppUser.Data == null)
                    return HandleErrorRedirect("ErrorViewsPath:Error404", 404);


                (int passMinLength, bool passMustLetters, bool passMustNumbers) =
                        await GetBasicPasswordRequirements(10, true, true);

                ResultDTO<string>? resGetPrefLang = await _userManagementService.GetPreferredLanguageForUser(resultGetAppUser.Data.Id);
                if (resGetPrefLang.IsSuccess == false && resGetPrefLang.HandleError())
                    return HandleErrorRedirect("ErrorViewsPath:Error", 400);
                if (resGetPrefLang.Data == null)
                    return HandleErrorRedirect("ErrorViewsPath:Error404", 404);

                MyProfileViewModel model = new MyProfileViewModel()
                {
                    FirstName = resultGetAppUser.Data.FirstName!,
                    LastName = resultGetAppUser.Data.LastName!,
                    Email = resultGetAppUser.Data.Email!,
                    Username = resultGetAppUser.Data.UserName!,
                    UserId = resultGetAppUser.Data.Id,
                    PasswordMinLength = passMinLength,
                    PasswordMustHaveLetters = passMustLetters,
                    PasswordMustHaveNumbers = passMustNumbers,
                    PreferredLanguage = resGetPrefLang.Data
                };
                return View(model);
            }
            catch (Exception)
            {
                return HandleErrorRedirect("ErrorViewsPath:Error", 400);
            }
        }

        public async Task<string?> GetAllLanguages()
        {
            ResultDTO<string?> resGetMainAppLangs = await _appSettingsAccessor.GetApplicationSettingValueByKey<string?>("MainApplicationLanguages");

            return resGetMainAppLangs.IsSuccess ? resGetMainAppLangs.Data : null;
        }

        [HttpPost]
        public async Task<IActionResult> SetCulture(string culture, string returnUrl)
        {
            try
            {
                string? id = User.FindFirstValue("UserId");

                if (string.IsNullOrWhiteSpace(id))
                    return HandleErrorRedirect("ErrorViewsPath:Error404", 404);


                ResultDTO<UserDTO>? resultGetAppUser = await _userManagementService.GetUserById(id);
                if (resultGetAppUser.IsSuccess == false && resultGetAppUser.HandleError())
                    return HandleErrorRedirect("ErrorViewsPath:Error", 400);

                if (resultGetAppUser.Data == null)
                    return HandleErrorRedirect("ErrorViewsPath:Error404", 404);

                await _userManagementService.AddLanguageClaimForUser(resultGetAppUser.Data.Id, culture);

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
            catch (Exception)
            {
                return HandleErrorRedirect("ErrorViewsPath:Error", 400);
            }
        }

        [HttpPost]
        public async Task<ResultDTO> UpdatePassword(string currentPassword, string password, string confirmNewPassword, string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                    return ResultDTO.Fail("Wrong user id");

                if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmNewPassword))
                    return ResultDTO.Fail("Incorrect current password");

                if (!password.Equals(confirmNewPassword))
                    return ResultDTO.Fail("Passwords mismatch");

                ResultDTO? result = await _userManagementService.UpdateUserPassword(userId, currentPassword, password);
                if (!result.IsSuccess && result.HandleError())
                    return ResultDTO.Fail(result.ErrMsg!);

                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
        }

        private IActionResult HandleErrorRedirect(string configKey, int statusCode)
        {
            string? errorPath = _configuration[configKey];
            if (string.IsNullOrEmpty(errorPath))
            {
                return statusCode switch
                {
                    404 => NotFound(),
                    403 => Forbid(),
                    405 => StatusCode(405),
                    _ => BadRequest()
                };
            }
            return Redirect(errorPath);
        }
    }
}
