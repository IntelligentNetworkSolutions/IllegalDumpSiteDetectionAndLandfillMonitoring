using Dal;
using Dal.Helpers;
using MainApp.MVC.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Entities;
using SD.Helpers;
using Services;
using Services.Interfaces.Services;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;
using System.Text.RegularExpressions;
using MainApp.MVC.ViewModels.IntranetPortal.UserManagement;

namespace MainApp.MVC.Areas.IntranetPortal.Controllers
{
    [Area("IntranetPortal")]
    public class UserManagementController : Controller
    {
        private readonly IUserManagementService _userManagementService;
        private readonly UserManagementDa _userManagementDa;
        private readonly ModulesAndAuthClaimsHelper _modulesAndAuthClaimsHelper;
        private readonly ApplicationSettingsHelper _applicationSettingsHelper;
        private readonly PasswordValidationHelper _passwordValidationHelper;
        private readonly IConfiguration _configuration;
        public UserManagementController(UserManagementDa userManagementDa, ModulesAndAuthClaimsHelper modulesAndAuthClaimsHelper, ApplicationSettingsHelper applicationSettingsHelper, PasswordValidationHelper passwordValidationHelper, IConfiguration configuration, IUserManagementService userManagementService)
        {
            _userManagementDa = userManagementDa;
            _modulesAndAuthClaimsHelper = modulesAndAuthClaimsHelper;
            _applicationSettingsHelper = applicationSettingsHelper;
            _passwordValidationHelper = passwordValidationHelper;
            _configuration = configuration;
            _userManagementService = userManagementService;
        }

        public async Task<IActionResult> Index()
        {
            // TODO: 🧹 Middleware Attribute -> AuthorizeClaim 🧹🧹🧹
            /*
            if (!User.HasAuthClaim(SD.AuthClaims.UserManagement) || !_modulesAndAuthClaimsHelper.HasModule(SD.Modules.UserManagement))
            {
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
            */
            UserManagementViewModel model = new UserManagementViewModel();
            var users = await _userManagementDa.GetAllIntanetPortalUsers();
            var roles = await _userManagementDa.GetRoles();
            var userRoles = await _userManagementDa.GetUserRoles();
            model.Users = users.Select(z => new UserManagementUserViewModel
            {
                FirstName = z.FirstName,
                LastName = z.LastName,
                Id = z.Id,
                UserName = z.UserName,
                PhoneNumber = z.PhoneNumber,
                Email = z.Email,
                IsActive = z.IsActive
            }).ToList();

            foreach (var user in model.Users)
            {
                var userRole = userRoles.Where(z => z.UserId == user.Id).Select(z => z.RoleId).ToList();
                user.Roles = roles.Where(z => userRole.Contains(z.Id)).ToList();
            }
            model.Roles = roles.OrderBy(x => x.Name).ToList();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> CreateUser()
        {
            // TODO: 🧹 Middleware Attribute -> AuthorizeClaim 🧹🧹🧹
            /*
            if (!User.HasAuthClaim(SD.AuthClaims.UserManagementAddUsersAndRoles) || !_modulesAndAuthClaimsHelper.HasModule(SD.Modules.UserManagement))
            {
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
            */

            // TODO: 🧹 Create Seed for initial load 🧹🧹🧹
            #region Get Pass App Settings
            int passwordMinLength;
            bool passwordMustHaveLetters;
            bool passwordMustHaveNumbers;
            try
            {
                passwordMinLength = _applicationSettingsHelper.GetApplicationSettingInteger("PasswordMinLength");
                passwordMustHaveLetters = _applicationSettingsHelper.GetApplicationSettingBool("PasswordMustHaveLetters");
                passwordMustHaveNumbers = _applicationSettingsHelper.GetApplicationSettingBool("PasswordMustHaveNumbers");
            }
            catch (Exception ex)
            {
                passwordMinLength = 3;
                passwordMustHaveLetters = false;
                passwordMustHaveNumbers = false;
            }
            #endregion

            UserManagementCreateUserViewModel model = new UserManagementCreateUserViewModel()
            {
                Roles = _userManagementDa.GetRoles().Result.ToList(),
                Claims = _modulesAndAuthClaimsHelper.GetAuthClaims().Result,
                RoleClaims = _userManagementDa.GetAllRoleClaims().Result,
                PasswordMinLength = passwordMinLength,
                PasswordMustHaveLetters = passwordMustHaveLetters,
                PasswordMustHaveNumbers = passwordMustHaveNumbers,
                AllUsers = await _userManagementDa.GetAllIntanetPortalUsers()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(UserManagementCreateUserViewModel user)
        {
            // TODO: 🧹 Middleware Attribute -> AuthorizeClaim 🧹🧹🧹
            /*
            if (!User.HasAuthClaim(SD.AuthClaims.UserManagementAddUsersAndRoles) || !_modulesAndAuthClaimsHelper.HasModule(SD.Modules.UserManagement))
            {
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
            */
            // TODO: ⚠️ !!! Password validation is only front end ⚠️⚠️⚠️
            if (ModelState.IsValid)
            {
                user.UserName = user.UserName?.Trim();
                user.Email = user.Email?.Trim();
                user.IsActive ??= false;
                user.Password = user.Password?.Trim();
                user.ConfirmPassword = user.ConfirmPassword?.Trim();

                var password = new PasswordHasher<ApplicationUser>();
                var hashed = password.HashPassword(user, user.Password);
                user.PasswordHash = hashed;
                var u = await _userManagementDa.AddUser(user);
                foreach (var role in user.RolesInsert)
                {
                    await _userManagementDa.AddRoleForUser(u.Id, role);
                }
                foreach (var claim in user.ClaimsInsert)
                {
                    _userManagementDa.AddClaimForUser(u.Id, claim);
                }
                return RedirectToAction(nameof(Index));
            }

            user.Roles = _userManagementDa.GetRoles().Result.ToList();
            user.Claims = _modulesAndAuthClaimsHelper.GetAuthClaims().Result;
            user.RoleClaims = _userManagementDa.GetAllRoleClaims().Result;
            user.PasswordMinLength = _applicationSettingsHelper.GetApplicationSettingInteger("PasswordMinLength");
            user.PasswordMustHaveLetters = _applicationSettingsHelper.GetApplicationSettingBool("PasswordMustHaveLetters");
            user.PasswordMustHaveNumbers = _applicationSettingsHelper.GetApplicationSettingBool("PasswordMustHaveNumbers");
            user.AllUsers = await _userManagementDa.GetAllIntanetPortalUsers();
            return View(user);
        }

        public async Task<IActionResult> EditUser(string id)
        {
            if (id == null)
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

            UserManagementEditUserViewModel model = new UserManagementEditUserViewModel();
            var user = await _userManagementDa.GetUser(id);
            var roles = await _userManagementDa.GetRoles();
            var userRoles = await _userManagementDa.GetUserRoles();
            var userRole = userRoles.Where(z => z.UserId == user.Id).Select(z => z.RoleId).ToList();
            var claims = await _userManagementDa.GetClaimsForUser(user.Id);

            model.FirstName = user.FirstName;
            model.LastName = user.LastName;
            model.UserName = user.UserName;
            model.PhoneNumber = user.PhoneNumber;
            model.Id = user.Id;
            model.Email = user.Email;
            model.IsActive = user.IsActive;
            model.PasswordMinLength = _applicationSettingsHelper.GetApplicationSettingInteger("PasswordMinLength");
            model.PasswordMustHaveLetters = _applicationSettingsHelper.GetApplicationSettingBool("PasswordMustHaveLetters");
            model.PasswordMustHaveNumbers = _applicationSettingsHelper.GetApplicationSettingBool("PasswordMustHaveNumbers");
            model.RolesInsert = roles.Where(z => userRole.Contains(z.Id)).Select(z => z.Id).ToList();
            model.Roles = roles.ToList();
            model.Claims = await _modulesAndAuthClaimsHelper.GetAuthClaims();
            model.ClaimsInsert = claims.Select(z => z.ClaimValue).ToList();
            model.AllUsersExceptCurrent = await _userManagementDa.GetAllIntanetPortalUsersExcludingCurrent(user.Id);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(UserManagementEditUserViewModel user)
        {
            var id = User.FindFirstValue("UserId");
            var appUser = _userManagementDa.GetUser(id).Result;
            if (!User.HasAuthClaim(SD.AuthClaims.UserManagementEditUsersAndRoles)
                || (user.UserName == "insadmin" && appUser.UserName != "insadmin"))
            {
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
            if (ModelState.IsValid)
            {
                user.UserName = user.UserName?.Trim();
                user.Email = user.Email?.Trim();
                user.Password = user.Password?.Trim();
                user.ConfirmPassword = user.ConfirmPassword?.Trim();

                var u = await _userManagementDa.UpdateUser(user);
                _userManagementDa.DeleteClaimsRolesForUser((ApplicationUser)user);
                foreach (var role in user.RolesInsert)
                {
                    await _userManagementDa.AddRoleForUser(u.Id, role);
                }

                foreach (var claim in user.ClaimsInsert)
                {
                    _userManagementDa.AddClaimForUser(u.Id, claim);
                }
                return RedirectToAction(nameof(Index));
            }

            var roles = await _userManagementDa.GetRoles();
            var userRoles = await _userManagementDa.GetUserRoles();
            var userRole = userRoles.Where(z => z.UserId == user.Id).Select(z => z.RoleId).ToList();
            var claims = await _userManagementDa.GetClaimsForUser(user.Id);
            user.PasswordMinLength = _applicationSettingsHelper.GetApplicationSettingInteger("PasswordMinLength");
            user.PasswordMustHaveLetters = _applicationSettingsHelper.GetApplicationSettingBool("PasswordMustHaveLetters");
            user.PasswordMustHaveNumbers = _applicationSettingsHelper.GetApplicationSettingBool("PasswordMustHaveNumbers");
            user.RolesInsert = roles.Where(z => userRole.Contains(z.Id)).Select(z => z.Id).ToList();
            user.Roles = roles.ToList();
            user.Claims = await _modulesAndAuthClaimsHelper.GetAuthClaims();
            user.ClaimsInsert = claims.Select(z => z.ClaimValue).ToList();
            user.AllUsersExceptCurrent = await _userManagementDa.GetAllIntanetPortalUsersExcludingCurrent(id);
            return View(user);

        }

        public async Task<IActionResult> CreateRole()
        {
            var model = new UserManagementCreateRoleViewModel();
            model.Claims = await _modulesAndAuthClaimsHelper.GetAuthClaims();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(UserManagementCreateRoleViewModel role)
        {
            if (!User.HasAuthClaim(SD.AuthClaims.UserManagementAddUsersAndRoles))
            {
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

            if (ModelState.IsValid)
            {
                if (role.Name != null)
                {
                    role.Name = role.Name.Trim();
                }
                var r = await _userManagementDa.AddRole((IdentityRole)role);

                foreach (var claim in role.ClaimsInsert)
                {
                    _userManagementDa.AddClaimForRole(r.Id, claim);
                }

                return RedirectToAction(nameof(Index));
            }
            return View(role);
        }

        public async Task<IActionResult> EditRole(string id)
        {
            if (id == null)
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
            var role = await _userManagementDa.GetRole(id);
            UserManagementEditRoleViewModel model = new UserManagementEditRoleViewModel();
            model.Name = role.Name;

            model.Claims = await _modulesAndAuthClaimsHelper.GetAuthClaims();
            var claims = await _userManagementDa.GetClaimsForRole(role.Id);
            model.ClaimsInsert = claims.Select(z => z.ClaimValue).ToList();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRole(UserManagementEditRoleViewModel role)
        {
            if (!User.HasAuthClaim(SD.AuthClaims.UserManagementEditUsersAndRoles))
            {
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
            if (ModelState.IsValid)
            {
                if (role.Name != null)
                {
                    role.Name = role.Name.Trim();
                }
                var u = await _userManagementDa.UpdateRole((IdentityRole)role);
                _userManagementDa.DeleteClaimsForRole((IdentityRole)role);
                foreach (var claim in role.ClaimsInsert)
                {
                    _userManagementDa.AddClaimForRole(u.Id, claim);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(role);
        }

        [HttpPost]
        public async Task<IdentityRole> DeleteRole(string id)
        {
            var role = await _userManagementDa.GetRole(id);
            return role;
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRoleConfirmed(string id)
        {
            if (!User.HasAuthClaim(SD.AuthClaims.UserManagementDeleteUsersAndRoles))
            {
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

            var userManagement = await _userManagementDa.DeleteRole(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<ApplicationUser> DeleteUser(string id)
        {
            var user = await _userManagementDa.GetUser(id);
            return user;
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUserConfirmed(string id)
        {
            if (!User.HasAuthClaim(SD.AuthClaims.UserManagementDeleteUsersAndRoles))
            {
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

            var userManagement = await _userManagementDa.DeleteUser(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public List<IdentityRoleClaim<string>> GetRoleClaims(string roleId)
        {
            var roleClaims = _userManagementDa.GetClaimsForRole(roleId).Result.Select(z => z.ClaimValue).ToList();
            return _modulesAndAuthClaimsHelper.GetAuthClaims().Result.Where(z => roleClaims.Contains(z.Value))
            .Select(z => new IdentityRoleClaim<string>
            {
                ClaimType = z.Value,
                ClaimValue = z.Description
            }).ToList();

        }

        [HttpPost]
        public List<IdentityUserClaim<string>> GetUserClaims(string userId)
        {
            var userClaims = _userManagementDa.GetClaimsForUser(userId).Result.Select(z => z.ClaimValue).ToList();
            return _modulesAndAuthClaimsHelper.GetAuthClaims().Result
                                                .Where(z => userClaims.Contains(z.Value))
                                                .Select(z => new IdentityUserClaim<string>
                                                {
                                                    ClaimType = z.Value,
                                                    ClaimValue = z.Description
                                                }).ToList();
        }

        [HttpPost]
        public async Task<List<IdentityRole>> GetUserRoles(string userId)
        {
            return await _userManagementDa.GetRolesForUser(userId);
        }

        [HttpGet]
        public async Task<List<IdentityRole>> GetQueryBuiltRoles()
        {
            IQueryable<IdentityRole> wholeDbSetQuery = _userManagementService.GetRolesAsQueriable();

            int numRows = 5;
            int pageNumber = 1;
            string orderByProp = "Name";
            bool orderIsAscending = true;

            var rolesNameAsc = await wholeDbSetQuery.OrderBy(x => x.Name).ToListAsync();
            var rolesNameDesc = await wholeDbSetQuery.OrderByDescending(x => x.Name).ToListAsync();

            var constructedQuery = 
                ConstructPagedEFQuery<IdentityRole>(wholeDbSetQuery, numRows, pageNumber, orderByProp, orderIsAscending);

            List<IdentityRole> pagedRoles = await constructedQuery.ToListAsync();

            return null;
        }

        private IQueryable<DbSetType> ConstructPagedEFQuery<DbSetType>(IQueryable<DbSetType> queriableSet,
                                                        int numRows = 5,
                                                        int pageNumber = 0,
                                                        string? orderByPropertyName = null,
                                                        bool orderIsAscending = true)
        {
            Type actualDbSetType = typeof(DbSetType);

            IQueryable<DbSetType> constructedQuery = queriableSet;

            if (!string.IsNullOrEmpty(orderByPropertyName))
            {
                PropertyInfo? actualOrderByPropertyName = actualDbSetType.GetProperty(orderByPropertyName);

                if (actualOrderByPropertyName is null)
                    throw new Exception("orderByPropertyName Does Not Exist");

                // Create an expression representing the property access
                ParameterExpression parameter = Expression.Parameter(actualDbSetType, "x");
                Expression propertyAccess = Expression.Property(parameter, actualOrderByPropertyName);

                // Create an expression representing the lambda function: x => x.PropertyName
                LambdaExpression orderByExpression = Expression.Lambda(propertyAccess, parameter);

                // Determine if we should use OrderBy or OrderByDescending
                string methodName = !orderIsAscending ? "OrderByDescending" : "OrderBy";

                // Use reflection to call the appropriate OrderBy method
                MethodCallExpression methodCallExpression = Expression.Call(
                    typeof(Queryable),
                    methodName,
                    new[] { actualDbSetType, actualOrderByPropertyName.PropertyType },
                    constructedQuery.Expression,
                    Expression.Quote(orderByExpression)
                );

                // Update the query with the new ordering
                constructedQuery = constructedQuery.Provider.CreateQuery<DbSetType>(methodCallExpression);
            }

            constructedQuery = constructedQuery.Skip(pageNumber * numRows).Take(numRows);

            return constructedQuery;
        }
    }
}
