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
using MainApp.MVC.ViewModels.IntranetPortal.AuditLog;
using DTOs.MainApp.MVC;
using DocumentFormat.OpenXml.Office2010.Excel;
using Humanizer;

namespace MainApp.MVC.Areas.IntranetPortal.Controllers
{
    [Area("IntranetPortal")]
    public class UserManagementController : Controller
    {
        private readonly IUserManagementService _userManagementService;
        private readonly ModulesAndAuthClaimsHelper _modulesAndAuthClaimsHelper;
        private readonly ApplicationSettingsHelper _applicationSettingsHelper;
        private readonly PasswordValidationHelper _passwordValidationHelper;
        private readonly IConfiguration _configuration;
        public UserManagementController(ModulesAndAuthClaimsHelper modulesAndAuthClaimsHelper, ApplicationSettingsHelper applicationSettingsHelper, PasswordValidationHelper passwordValidationHelper, IConfiguration configuration, IUserManagementService userManagementService)
        {
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
            var users = await _userManagementService.GetAllIntanetPortalUsers();
            if (users is null)
            {
                throw new Exception("Users not found");
            }
            var roles = await _userManagementService.GetAllRoles();
            if (roles is null)
            {
                throw new Exception("Roles not found");
            }
            var userRoles = await _userManagementService.GetAllUserRoles();
            if (userRoles is null)
            {
                throw new Exception("User roles not found");
            }

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
            //#region Get Pass App Settings
            //int passwordMinLength;
            //bool passwordMustHaveLetters;
            //bool passwordMustHaveNumbers;
            //try
            //{
            //    passwordMinLength = _applicationSettingsHelper.GetApplicationSettingInteger("PasswordMinLength");
            //    passwordMustHaveLetters = _applicationSettingsHelper.GetApplicationSettingBool("PasswordMustHaveLetters");
            //    passwordMustHaveNumbers = _applicationSettingsHelper.GetApplicationSettingBool("PasswordMustHaveNumbers");
            //}
            //catch (Exception ex)
            //{
            //    passwordMinLength = 3;
            //    passwordMustHaveLetters = false;
            //    passwordMustHaveNumbers = false;
            //}
            //#endregion

            UserManagementDTO dto = new();
            dto = await _userManagementService.FillUserManagementDto(dto);
            UserManagementCreateUserViewModel model = new()
            {                
                PasswordMinLength = dto.PasswordMinLength,
                PasswordMustHaveLetters = dto.PasswordMustHaveLetters,
                PasswordMustHaveNumbers = dto.PasswordMustHaveNumbers,
                Roles = dto.Roles,
                Claims = await _modulesAndAuthClaimsHelper.GetAuthClaims(),
                AllUsers = dto.AllUsers,
                RoleClaims = dto.RoleClaims
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(UserManagementCreateUserViewModel viewModel)
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
            if (!ModelState.IsValid)
            {             
                UserManagementDTO dto = new();
                dto = await _userManagementService.FillUserManagementDto(dto);
                viewModel.Roles = dto.Roles;
                viewModel.Claims = _modulesAndAuthClaimsHelper.GetAuthClaims().Result;
                viewModel.RoleClaims = dto.RoleClaims;
                viewModel.PasswordMinLength = dto.PasswordMinLength;
                viewModel.PasswordMustHaveLetters = dto.PasswordMustHaveLetters;
                viewModel.PasswordMustHaveNumbers = dto.PasswordMustHaveNumbers;
                viewModel.AllUsers = dto.AllUsers;

                return View(viewModel);
            }

            UserManagementDTO userManagementDTO = new()
            {
                Email = viewModel.Email,
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                IsActive = viewModel.IsActive,
                PhoneNumber = viewModel.PhoneNumber,
                UserName = viewModel.UserName,
                Password = viewModel.Password,
                ConfirmPassword = viewModel.ConfirmPassword,               
                RolesInsert = viewModel.RolesInsert,
                ClaimsInsert = viewModel.ClaimsInsert
            };

            await _userManagementService.AddUser(userManagementDTO);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> EditUser(string id)
        {
            if (id is null)
            {
                //var errorPath = _configuration["ErrorViewsPath:Error404"];
                //if (!string.IsNullOrEmpty(errorPath))
                //{
                //    return Redirect(errorPath);
                //}
                //else
                //{
                //    return NotFound();
                //}
            }
            UserManagementDTO dto = new()
            {
                Id = id
            };

            dto = await _userManagementService.FillUserManagementDto(dto);
            UserManagementEditUserViewModel model = new()
            {
                Id = dto.Id,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                UserName = dto.UserName,
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                IsActive = dto.IsActive,
                PasswordMinLength = dto.PasswordMinLength,
                PasswordMustHaveLetters = dto.PasswordMustHaveLetters,
                PasswordMustHaveNumbers = dto.PasswordMustHaveNumbers,
                RolesInsert = dto.RolesInsert,
                Roles = dto.Roles,
                Claims = await _modulesAndAuthClaimsHelper.GetAuthClaims(),
                ClaimsInsert = dto.ClaimsInsert,
                AllUsersExceptCurrent = dto.AllUsersExceptCurrent
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(UserManagementEditUserViewModel viewModel)
        {
            //var id = User.FindFirstValue("UserId");
            //var appUser = _userManagementDa.GetUser(id).Result;
            //if (!User.HasAuthClaim(SD.AuthClaims.UserManagementEditUsersAndRoles)
            //    || (user.UserName == "insadmin" && appUser.UserName != "insadmin"))
            //{
            //    var errorPath = _configuration["ErrorViewsPath:Error403"];
            //    if (!string.IsNullOrEmpty(errorPath))
            //    {
            //        return Redirect(errorPath);
            //    }
            //    else
            //    {
            //        return StatusCode(403);
            //    }
            //}
            if (!ModelState.IsValid)
            {
                UserManagementDTO dto = new()
                {
                    Id = viewModel.Id
                };
                dto = await _userManagementService.FillUserManagementDto(dto);
                viewModel.PasswordMinLength = dto.PasswordMinLength;
                viewModel.PasswordMustHaveLetters = dto.PasswordMustHaveLetters;
                viewModel.PasswordMustHaveNumbers = dto.PasswordMustHaveNumbers;
                viewModel.RolesInsert = dto.RolesInsert;
                viewModel.Roles = dto.Roles;
                viewModel.Claims = await _modulesAndAuthClaimsHelper.GetAuthClaims();
                viewModel.ClaimsInsert = dto.ClaimsInsert;
                viewModel.AllUsersExceptCurrent = dto.AllUsersExceptCurrent;  

                return View(viewModel);
            }

            UserManagementDTO userManagementDTO = new()
            {
                Id = viewModel.Id,
                Email = viewModel.Email,
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                IsActive = viewModel.IsActive,
                PhoneNumber = viewModel.PhoneNumber,
                UserName = viewModel.UserName,
                Password = viewModel.Password,
                ConfirmPassword = viewModel.ConfirmPassword,
                NormalizedUserName = viewModel.NormalizedUserName,
                EmailConfirmed = viewModel.EmailConfirmed,
                RolesInsert = viewModel.RolesInsert,
                ClaimsInsert = viewModel.ClaimsInsert
            };
            await _userManagementService.UpdateUser(userManagementDTO);
            return RedirectToAction(nameof(Index));                      
        }

        public async Task<IActionResult> CreateRole()
        {
            var model = new UserManagementCreateRoleViewModel();
            model.Claims = await _modulesAndAuthClaimsHelper.GetAuthClaims();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(UserManagementCreateRoleViewModel viewModel)
        {
            //if (!User.HasAuthClaim(SD.AuthClaims.UserManagementAddUsersAndRoles))
            //{
            //    var errorPath = _configuration["ErrorViewsPath:Error403"];
            //    if (!string.IsNullOrEmpty(errorPath))
            //    {
            //        return Redirect(errorPath);
            //    }
            //    else
            //    {
            //        return StatusCode(403);
            //    }
            //}

            if (!ModelState.IsValid)
            {
                viewModel.Claims = await _modulesAndAuthClaimsHelper.GetAuthClaims();
                return View(viewModel);
            }

            RoleManagementDTO roleManagementDTO = new()
            {
                Name = viewModel.Name,
                NormalizedName = viewModel.NormalizedName,
                ClaimsInsert = viewModel.ClaimsInsert
            };
            await _userManagementService.AddRole(roleManagementDTO);

            return RedirectToAction(nameof(Index));           
        }

        public async Task<IActionResult> EditRole(string id)
        {
            if (id is null)
            {
                //var errorPath = _configuration["ErrorViewsPath:Error404"];
                //if (!string.IsNullOrEmpty(errorPath))
                //{
                //    return Redirect(errorPath);
                //}
                //else
                //{
                //    return NotFound();
                //}
            }
            RoleManagementDTO dto = new()
            { 
                Id = id
            };
            dto = await _userManagementService.FillRoleManagementDto(dto);
            UserManagementEditRoleViewModel viewModel = new()
            {
                Id = dto.Id,
                Claims = await _modulesAndAuthClaimsHelper.GetAuthClaims(),
                ClaimsInsert = dto.ClaimsInsert,                
                Name = dto.Name,
                NormalizedName = dto.NormalizedName
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRole(UserManagementEditRoleViewModel viewModel)
        {
            //if (!User.HasAuthClaim(SD.AuthClaims.UserManagementEditUsersAndRoles))
            //{
            //    var errorPath = _configuration["ErrorViewsPath:Error403"];
            //    if (!string.IsNullOrEmpty(errorPath))
            //    {
            //        return Redirect(errorPath);
            //    }
            //    else
            //    {
            //        return StatusCode(403);
            //    }
            //}
            if (!ModelState.IsValid)
            {               
                RoleManagementDTO dto = new()
                {
                    Id = viewModel.Id
                };
                dto = await _userManagementService.FillRoleManagementDto(dto);
                viewModel.Claims = await _modulesAndAuthClaimsHelper.GetAuthClaims();
                viewModel.ClaimsInsert = dto.ClaimsInsert;
                return View(viewModel);
            }
            RoleManagementDTO roleManagementDTO = new()
            {
                Id = viewModel.Id,
                Name = viewModel.Name,
                NormalizedName = viewModel.NormalizedName,
                ClaimsInsert = viewModel.ClaimsInsert
            };
            await _userManagementService.UpdateRole(roleManagementDTO);
            return RedirectToAction(nameof(Index));
            
        }

        [HttpPost]
        public async Task<RoleDTO> DeleteRole(string id)
        {
            return await _userManagementService.GetRoleById(id);
            
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRoleConfirmed(string id)
        {
            //if (!User.HasAuthClaim(SD.AuthClaims.UserManagementDeleteUsersAndRoles))
            //{
            //    var errorPath = _configuration["ErrorViewsPath:Error403"];
            //    if (!string.IsNullOrEmpty(errorPath))
            //    {
            //        return Redirect(errorPath);
            //    }
            //    else
            //    {
            //        return StatusCode(403);
            //    }
            //}

            await _userManagementService.DeleteRole(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<UserDTO> DeleteUser(string id)
        {
            return await _userManagementService.GetUserById(id);
            
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUserConfirmed(string id)
        {
            //if (!User.HasAuthClaim(SD.AuthClaims.UserManagementDeleteUsersAndRoles))
            //{
            //    var errorPath = _configuration["ErrorViewsPath:Error403"];
            //    if (!string.IsNullOrEmpty(errorPath))
            //    {
            //        return Redirect(errorPath);
            //    }
            //    else
            //    {
            //        return StatusCode(403);
            //    }
            //}

            await _userManagementService.DeleteUser(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public List<RoleClaimDTO> GetRoleClaims(string roleId)
        {
            var roleClaims = _userManagementService.GetRoleClaims(roleId).GetAwaiter().GetResult().Select(x => x.ClaimValue).ToList();
            if (roleClaims is null)
            {
                throw new Exception("Role claims not found");
            }
            return _modulesAndAuthClaimsHelper.GetAuthClaims().Result.Where(z => roleClaims.Contains(z.Value))
            .Select(z => new RoleClaimDTO
            {
                ClaimType = z.Value,
                ClaimValue = z.Description
            }).ToList();

        }

        [HttpPost]
        public List<UserClaimDTO> GetUserClaims(string userId)
        {
            var userClaims = _userManagementService.GetUserClaims(userId).GetAwaiter().GetResult().Select(x => x.ClaimValue).ToList();
            if (userClaims is null)
            {
                throw new Exception("User claims not found");
            }
            return _modulesAndAuthClaimsHelper.GetAuthClaims().Result .Where(z => userClaims.Contains(z.Value))
            .Select(z => new UserClaimDTO
            {
                ClaimType = z.Value,
                ClaimValue = z.Description
            }).ToList();
        }

        [HttpPost]
        public async Task<List<RoleDTO>> GetUserRoles(string userId)
        {
            return await _userManagementService.GetRolesForUser(userId);
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
