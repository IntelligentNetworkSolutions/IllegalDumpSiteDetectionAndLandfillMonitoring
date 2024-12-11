using MainApp.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces.Services;
using System.Data;
using MainApp.MVC.ViewModels.IntranetPortal.UserManagement;
using AutoMapper;
using DTOs.MainApp.BL;
using DAL.Interfaces.Helpers;
using MainApp.MVC.Filters;
using SD;

namespace MainApp.MVC.Areas.IntranetPortal.Controllers
{
    [Area("IntranetPortal")]
    [HasAppModule(nameof(Modules.UserManagement))]
    public class UserManagementController : Controller
    {
        private readonly IUserManagementService _userManagementService;
        private readonly ModulesAndAuthClaimsHelper _modulesAndAuthClaimsHelper;
        private readonly IAppSettingsAccessor _appSettingsAccessor;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public UserManagementController(ModulesAndAuthClaimsHelper modulesAndAuthClaimsHelper, IConfiguration configuration,
            IUserManagementService userManagementService, IAppSettingsAccessor appSettingsAccessor, IMapper mapper)
        {
            _modulesAndAuthClaimsHelper = modulesAndAuthClaimsHelper;
            _configuration = configuration;
            _userManagementService = userManagementService;
            _appSettingsAccessor = appSettingsAccessor;
            _mapper = mapper;
        }
        
        [HttpGet]
        [HasAuthClaim(nameof(SD.AuthClaims.UserManagement))]
        public async Task<IActionResult> Index()
        {
            try
            {
                UserManagementViewModel model = new UserManagementViewModel();
                ResultDTO<List<UserDTO>>? resultGetUsers = await _userManagementService.GetAllIntanetPortalUsers();
                if (resultGetUsers.IsSuccess == false && resultGetUsers.HandleError())                
                    return HandleErrorRedirect("ErrorViewsPath:Error", 400);
                if (resultGetUsers.Data == null)
                    return HandleErrorRedirect("ErrorViewsPath:Error404", 404);

                ResultDTO<List<RoleDTO>>? resultGetRoles = await _userManagementService.GetAllRoles();
                if (resultGetRoles.IsSuccess == false && resultGetRoles.HandleError())
                    return HandleErrorRedirect("ErrorViewsPath:Error", 400);

                if (resultGetRoles.Data == null)
                    return HandleErrorRedirect("ErrorViewsPath:Error404", 404);

                ResultDTO<List<UserRoleDTO>>? resGetUserRoles = await _userManagementService.GetAllUserRoles();
                if (resGetUserRoles.IsSuccess == false && resGetUserRoles.HandleError())
                    return HandleErrorRedirect("ErrorViewsPath:Error", 400);
                if (resGetUserRoles.Data == null)
                    return HandleErrorRedirect("ErrorViewsPath:Error404", 404);

                List<UserManagementUserViewModel>? mappedList = _mapper.Map<List<UserManagementUserViewModel>>(resultGetUsers.Data);
                if (mappedList == null)
                    return HandleErrorRedirect("ErrorViewsPath:Error404", 404);

                model.Users = mappedList;

                foreach (var user in model.Users)
                {
                    List<string>? userRole = resGetUserRoles.Data.Where(z => z.UserId == user.Id).Select(z => z.RoleId).ToList();
                    if (userRole == null)
                        return HandleErrorRedirect("ErrorViewsPath:Error404", 404);

                    user.Roles = resultGetRoles.Data.Where(z => userRole.Contains(z.Id)).ToList();
                }
                model.Roles = resultGetRoles.Data.OrderBy(x => x.Name).ToList();
                return View(model);
            }
            catch (Exception)
            {
                return HandleErrorRedirect("ErrorViewsPath:Error", 400);
            }
        }

        #region Create User
        [HttpGet]
        [HasAuthClaim(nameof(SD.AuthClaims.UserManagementAddUsersAndRoles))]
        public async Task<ResultDTO<UserManagementCreateUserViewModel>> FillUserManagementCreateUserViewModel()
        {
            try
            {
                ResultDTO<UserManagementDTO>? resultFillDTO = await _userManagementService.FillUserManagementDto();
                if (resultFillDTO.IsSuccess == false && resultFillDTO.HandleError())
                    return ResultDTO<UserManagementCreateUserViewModel>.Fail(resultFillDTO.ErrMsg!);
                if (resultFillDTO.Data == null)
                    return ResultDTO<UserManagementCreateUserViewModel>.Fail("User managment dto not found");

                UserManagementDTO dto = resultFillDTO.Data!;

                UserManagementCreateUserViewModel? model = _mapper.Map<UserManagementCreateUserViewModel>(dto);
                if (model is null)
                    return ResultDTO<UserManagementCreateUserViewModel>.Fail("Model not found");

                model.Claims = await _modulesAndAuthClaimsHelper.GetAuthClaims();

                return ResultDTO<UserManagementCreateUserViewModel>.Ok(model);
            }
            catch (Exception ex)
            {
                return ResultDTO<UserManagementCreateUserViewModel>.ExceptionFail(ex.Message, ex);
            }
        }

        public async Task<IActionResult> GetViewWithUserManagementCreateVM(string? errMsg = null)
        {
            try
            {
                ResultDTO<UserManagementCreateUserViewModel>? resultFillModel = await FillUserManagementCreateUserViewModel();
                if (resultFillModel.IsSuccess == false && resultFillModel.HandleError())
                    ModelState.AddModelError("ModelOnly", resultFillModel.ErrMsg!);

                return View(resultFillModel.Data);
            }
            catch (Exception)
            {
                return HandleErrorRedirect("ErrorViewsPath:Error", 400);
            }
           
        }

        [HttpGet]
        [HasAuthClaim(nameof(SD.AuthClaims.UserManagementAddUsersAndRoles))]
        public async Task<IActionResult> CreateUser()
        {
            return await GetViewWithUserManagementCreateVM();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasAuthClaim(nameof(SD.AuthClaims.UserManagementAddUsersAndRoles))]
        public async Task<IActionResult> CreateUser(UserManagementCreateUserViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid == false)
                    return await GetViewWithUserManagementCreateVM();

                UserManagementDTO? userManagementDTO = _mapper.Map<UserManagementDTO>(viewModel);
                if (userManagementDTO is null)
                    return await GetViewWithUserManagementCreateVM("User management DTO not found");

                ResultDTO? resultAdd = await _userManagementService.AddUser(userManagementDTO);
                if (resultAdd.IsSuccess == false && resultAdd.HandleError())
                    return await GetViewWithUserManagementCreateVM(resultAdd.ErrMsg);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return HandleErrorRedirect("ErrorViewsPath:Error", 400);
            }
        }
        #endregion

        #region Edit User
        [HttpGet]
        [HasAuthClaim(nameof(SD.AuthClaims.UserManagementEditUsersAndRoles))]
        public async Task<ResultDTO<UserManagementEditUserViewModel>> FillUserManagementEditUserViewModelFromDto(UserManagementDTO dto)
        {
            try
            {
                ResultDTO<UserManagementDTO>? resultFillDTO = await _userManagementService.FillUserManagementDto(dto);
                if (resultFillDTO.IsSuccess == false && resultFillDTO.HandleError())
                    return ResultDTO<UserManagementEditUserViewModel>.Fail(resultFillDTO.ErrMsg!);
                if (resultFillDTO.Data == null)
                    return ResultDTO<UserManagementEditUserViewModel>.Fail("User managment dto not found");

                dto = resultFillDTO.Data;

                UserManagementEditUserViewModel? model = _mapper.Map<UserManagementEditUserViewModel>(dto);
                if (model is null)
                    return ResultDTO<UserManagementEditUserViewModel>.Fail("Unable to map dto to model");

                model.Claims = await _modulesAndAuthClaimsHelper.GetAuthClaims();

                return ResultDTO<UserManagementEditUserViewModel>.Ok(model);
            }
            catch (Exception ex)
            {
                return ResultDTO<UserManagementEditUserViewModel>.ExceptionFail(ex.Message, ex);
            }
        }

        [HttpGet]
        [HasAuthClaim(nameof(SD.AuthClaims.UserManagementEditUsersAndRoles))]
        public async Task<IActionResult> GetViewWithUserManagementEditVM(UserManagementDTO dto, string? errMsg = null)
        {
            try
            {
                ResultDTO<UserManagementEditUserViewModel>? resultFillModel = await FillUserManagementEditUserViewModelFromDto(dto);
                if (resultFillModel.IsSuccess == false && resultFillModel.HandleError())
                    ModelState.AddModelError("ModelOnly", resultFillModel.ErrMsg!);
                if (resultFillModel.Data is null)
                    return HandleErrorRedirect("ErrorViewsPath:Error404", 404);

                return View(resultFillModel.Data);
            }
            catch (Exception)
            {
                return HandleErrorRedirect("ErrorViewsPath:Error", 400);
            }           
        }

        [HttpGet]
        [HasAuthClaim(nameof(SD.AuthClaims.UserManagementEditUsersAndRoles))]
        public async Task<IActionResult> EditUser(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return HandleErrorRedirect("ErrorViewsPath:Error404", 404);

            UserManagementDTO dto = new() { Id = id! };

            return await GetViewWithUserManagementEditVM(dto);
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.UserManagementEditUsersAndRoles))]
        public async Task<IActionResult> EditUser(UserManagementEditUserViewModel viewModel)
        {
            try
            {
                UserManagementDTO dto = new() { Id = viewModel.Id };
                if (ModelState.IsValid == false)
                    return await GetViewWithUserManagementEditVM(dto);

                UserManagementDTO? userManagementDTO = _mapper.Map<UserManagementDTO>(viewModel);
                if (userManagementDTO is null)
                    return await GetViewWithUserManagementEditVM(dto, "User management DTO not found");

                ResultDTO resultUpdate = await _userManagementService.UpdateUser(userManagementDTO);
                if (resultUpdate.IsSuccess == false && resultUpdate.HandleError())
                    return await GetViewWithUserManagementEditVM(dto, resultUpdate.ErrMsg);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return HandleErrorRedirect("ErrorViewsPath:Error", 400);
            }            
        }
        #endregion

        #region Create Role
        [HttpGet]
        [HasAuthClaim(nameof(SD.AuthClaims.UserManagementAddUsersAndRoles))]
        public async Task<IActionResult> CreateRole()
        {
            UserManagementCreateRoleViewModel? model = new UserManagementCreateRoleViewModel { Claims = await _modulesAndAuthClaimsHelper.GetAuthClaims() };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasAuthClaim(nameof(SD.AuthClaims.UserManagementAddUsersAndRoles))]
        public async Task<IActionResult> CreateRole(UserManagementCreateRoleViewModel viewModel)
        {            
            try
            {
                if (ModelState.IsValid == false)
                {
                    viewModel.Claims = await _modulesAndAuthClaimsHelper.GetAuthClaims();
                    return View(viewModel);
                }

                RoleManagementDTO? roleManagementDTO = _mapper.Map<RoleManagementDTO>(viewModel);
                if (roleManagementDTO == null)
                    return HandleErrorRedirect("ErrorViewsPath:Error404", 404);

                ResultDTO? resultRoleAdd = await _userManagementService.AddRole(roleManagementDTO);
                if (resultRoleAdd.IsSuccess == false && resultRoleAdd.HandleError())
                    return HandleErrorRedirect("ErrorViewsPath:Error", 400);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return HandleErrorRedirect("ErrorViewsPath:Error", 400);
            }
        }
        #endregion

        #region Edit Role
        private async Task<ResultDTO<UserManagementEditRoleViewModel>> FillUserManagementEditRoleViewModelFromDTO(RoleManagementDTO dto)
        {
            try
            {
                ResultDTO<RoleManagementDTO>? resultFillDTO = await _userManagementService.FillRoleManagementDto(dto);
                if (resultFillDTO.IsSuccess == false && resultFillDTO.HandleError())
                    return ResultDTO<UserManagementEditRoleViewModel>.Fail(resultFillDTO.ErrMsg!);
                if (resultFillDTO.Data == null)
                    return ResultDTO<UserManagementEditRoleViewModel>.Fail("Role managment dto not found");

                dto = resultFillDTO.Data!;

                UserManagementEditRoleViewModel? viewModel = _mapper.Map<UserManagementEditRoleViewModel>(dto);
                if (viewModel is null)
                    return ResultDTO<UserManagementEditRoleViewModel>.Fail("Unable to map dto to model");

                viewModel.Claims = await _modulesAndAuthClaimsHelper.GetAuthClaims();

                return ResultDTO<UserManagementEditRoleViewModel>.Ok(viewModel);
            }
            catch (Exception ex)
            {
                return ResultDTO<UserManagementEditRoleViewModel>.ExceptionFail(ex.Message, ex);
            }
        }

        [HttpGet]
        [HasAuthClaim(nameof(SD.AuthClaims.UserManagementEditUsersAndRoles))]
        public async Task<IActionResult> EditRole(string id)
        {
            try
            {
                if (id is null)
                    return HandleErrorRedirect("ErrorViewsPath:Error404", 404);

                RoleManagementDTO dto = new() { Id = id! };
                ResultDTO<UserManagementEditRoleViewModel>? resultFillModel = await FillUserManagementEditRoleViewModelFromDTO(dto);
                if (resultFillModel.IsSuccess == false && resultFillModel.HandleError())
                    return HandleErrorRedirect("ErrorViewsPath:Error", 400);
                if (resultFillModel.Data == null)
                    return HandleErrorRedirect("ErrorViewsPath:Error404", 404);

                return View(resultFillModel.Data);
            }
            catch (Exception)
            {
                return HandleErrorRedirect("ErrorViewsPath:Error", 400);
            }            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasAuthClaim(nameof(SD.AuthClaims.UserManagementEditUsersAndRoles))]
        public async Task<IActionResult> EditRole(UserManagementEditRoleViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid == false)
                {
                    RoleManagementDTO dto = new() { Id = viewModel.Id };
                    ResultDTO<UserManagementEditRoleViewModel>? resultFillModel = await FillUserManagementEditRoleViewModelFromDTO(dto);
                    if (resultFillModel.IsSuccess == false && resultFillModel.HandleError())
                        return HandleErrorRedirect("ErrorViewsPath:Error", 400);
                    if (resultFillModel.Data == null)
                        return HandleErrorRedirect("ErrorViewsPath:Error404", 404);

                    return View(resultFillModel.Data);
                }

                RoleManagementDTO? roleManagementDTO = _mapper.Map<RoleManagementDTO>(viewModel);
                if (roleManagementDTO == null)
                    return HandleErrorRedirect("ErrorViewsPath:Error404", 404);

                ResultDTO? resultUpdate = await _userManagementService.UpdateRole(roleManagementDTO);
                if (resultUpdate.IsSuccess == false)
                {
                    ModelState.AddModelError("ModelOnly", resultUpdate.ErrMsg!);
                    RoleManagementDTO dto = new() { Id = viewModel.Id };
                    ResultDTO<UserManagementEditRoleViewModel> resultFillModel = await FillUserManagementEditRoleViewModelFromDTO(dto);
                    if (resultFillModel.IsSuccess == false && resultFillModel.HandleError())
                        return HandleErrorRedirect("ErrorViewsPath:Error", 400);
                    if (resultFillModel.Data == null)
                        return HandleErrorRedirect("ErrorViewsPath:Error404", 404);

                    return View(resultFillModel.Data);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return HandleErrorRedirect("ErrorViewsPath:Error", 400);
            }
        }
        #endregion

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.UserManagementDeleteUsersAndRoles))]
        public async Task<ResultDTO<RoleDTO>> DeleteRole(string id)
        {
            try
            {
                ResultDTO<RoleDTO>? resultGetRole = await _userManagementService.GetRoleById(id);
                if (resultGetRole.IsSuccess == false && resultGetRole.HandleError())
                    return ResultDTO<RoleDTO>.Fail(resultGetRole.ErrMsg!);
                if (resultGetRole.Data == null)
                    return ResultDTO<RoleDTO>.Fail("Role not found");

                return ResultDTO<RoleDTO>.Ok(resultGetRole.Data);
            }
            catch (Exception ex)
            {
                return ResultDTO<RoleDTO>.ExceptionFail(ex.Message, ex);
            }            
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.UserManagementDeleteUsersAndRoles))]
        public async Task<ResultDTO> DeleteRoleConfirmed(string id)
        {
            try
            {
                ResultDTO? resDeleteRole = await _userManagementService.DeleteRole(id);
                if (resDeleteRole.IsSuccess != false && resDeleteRole.HandleError())
                    return ResultDTO.Fail(resDeleteRole.ErrMsg!);

                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }            
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.UserManagementDeleteUsersAndRoles))]
        public async Task<ResultDTO<UserDTO>> DeleteUser(string id)
        {
            try
            {
                ResultDTO<UserDTO>? resultGetUser = await _userManagementService.GetUserById(id);
                if (resultGetUser.IsSuccess == false && resultGetUser.HandleError())
                    return ResultDTO<UserDTO>.Fail(resultGetUser.ErrMsg!);
                if (resultGetUser.Data == null)
                    return ResultDTO<UserDTO>.Fail("User not found");

                return ResultDTO<UserDTO>.Ok(resultGetUser.Data);
            }
            catch (Exception ex)
            {
                return ResultDTO<UserDTO>.ExceptionFail(ex.Message, ex);
            }
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.UserManagementDeleteUsersAndRoles))]
        public async Task<ResultDTO> DeleteUserConfirmed(string id)
        {
            try
            {
                ResultDTO<bool>? hasUserEntry = await _userManagementService.CheckUserBeforeDelete(id);
                if (hasUserEntry.IsSuccess == false && hasUserEntry.HandleError())
                    return ResultDTO.Fail(hasUserEntry.ErrMsg!);
                if (hasUserEntry.Data == true)
                    return ResultDTO.Fail("You can not delete this user because there are data entries connected with the same user!");

                ResultDTO? resDeleteUsr = await _userManagementService.DeleteUser(id);
                if (resDeleteUsr.IsSuccess == false && resDeleteUsr.HandleError())
                    return ResultDTO.Fail(resDeleteUsr.ErrMsg!);

                return ResultDTO.Ok();
            }
            catch (Exception ex)
            {
                return ResultDTO.ExceptionFail(ex.Message, ex);
            }
            
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.UserManagement))]
        public ResultDTO<List<RoleClaimDTO>> GetRoleClaims(string roleId)
        {
            try
            {
                ResultDTO<List<RoleClaimDTO>>? resultRoleClaimsList = _userManagementService.GetRoleClaims(roleId).GetAwaiter().GetResult();
                if (resultRoleClaimsList.IsSuccess == false && resultRoleClaimsList.HandleError())
                    return ResultDTO<List<RoleClaimDTO>>.Fail(resultRoleClaimsList.ErrMsg!);
                if (resultRoleClaimsList.Data == null)
                    return ResultDTO<List<RoleClaimDTO>>.Fail("Role claims not found");

                List<string?> roleClaims = resultRoleClaimsList.Data.Select(x => x.ClaimValue).ToList();
                List<AuthClaim> listOfRoleAuthClaims = _modulesAndAuthClaimsHelper.GetAuthClaims().Result.Where(z => roleClaims.Contains(z.Value)).ToList();
                List<RoleClaimDTO>? dtoList = _mapper.Map<List<RoleClaimDTO>>(listOfRoleAuthClaims);
                if (dtoList == null)
                    return ResultDTO<List<RoleClaimDTO>>.Fail("Mapping failed");

                return ResultDTO<List<RoleClaimDTO>>.Ok(dtoList);
            }
            catch (Exception ex)
            {
                return ResultDTO<List<RoleClaimDTO>>.ExceptionFail(ex.Message, ex);
            }            
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.UserManagement))]
        public ResultDTO<List<UserClaimDTO>> GetUserClaims(string userId)
        {
            try
            {
                ResultDTO<List<UserClaimDTO>>? resultUserClaimsList = _userManagementService.GetUserClaims(userId).GetAwaiter().GetResult();
                if (resultUserClaimsList.IsSuccess == false && resultUserClaimsList.HandleError())
                    return ResultDTO<List<UserClaimDTO>>.Fail(resultUserClaimsList.ErrMsg!);
                if (resultUserClaimsList.Data == null)
                    return ResultDTO<List<UserClaimDTO>>.Fail("User claims not found");


                List<string?> userClaims = resultUserClaimsList.Data.Select(x => x.ClaimValue).ToList();
                List<AuthClaim>? listOfAuthClaims = _modulesAndAuthClaimsHelper.GetAuthClaims().Result.Where(z => userClaims.Contains(z.Value)).ToList();
                List<UserClaimDTO>? dtoList = _mapper.Map<List<UserClaimDTO>>(listOfAuthClaims);
                if (dtoList == null)
                    return ResultDTO<List<UserClaimDTO>>.Fail("Mapping failed");

                return ResultDTO<List<UserClaimDTO>>.Ok(dtoList);
            }
            catch (Exception ex)
            {
                return ResultDTO<List<UserClaimDTO>>.ExceptionFail(ex.Message, ex);
            }            
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.UserManagement))]
        public async Task<ResultDTO<List<RoleDTO>>> GetUserRoles(string userId)
        {
            try
            {
                ResultDTO<List<RoleDTO>>? resultGetRoles = await _userManagementService.GetRolesForUser(userId);
                if (resultGetRoles.IsSuccess == false && resultGetRoles.HandleError())
                    return ResultDTO<List<RoleDTO>>.Fail(resultGetRoles.ErrMsg!);

                if (resultGetRoles.Data == null)
                    return ResultDTO<List<RoleDTO>>.Fail("No data found");

                return ResultDTO<List<RoleDTO>>.Ok(resultGetRoles.Data);
            }
            catch (Exception ex)
            {
                return ResultDTO<List<RoleDTO>>.ExceptionFail(ex.Message, ex);
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
        // POC:
        // TODO: maybe Clean up
        //[HttpGet]
        //[HasAuthorizeClaim(nameof(SD.AuthClaims.UserManagement))]
        //public async Task<List<IdentityRole>?> GetQueryBuiltRoles()
        //{
        //    IQueryable<IdentityRole> wholeDbSetQuery = _userManagementService.GetRolesAsQueriable();

        //    int numRows = 5;
        //    int pageNumber = 1;
        //    string orderByProp = "Name";
        //    bool orderIsAscending = true;

        //    var rolesNameAsc = await wholeDbSetQuery.OrderBy(x => x.Name).ToListAsync();
        //    var rolesNameDesc = await wholeDbSetQuery.OrderByDescending(x => x.Name).ToListAsync();

        //    var constructedQuery =
        //        ConstructPagedEFQuery<IdentityRole>(wholeDbSetQuery, numRows, pageNumber, orderByProp, orderIsAscending);

        //    List<IdentityRole> pagedRoles = await constructedQuery.ToListAsync();

        //    return null;
        //}

        // POC:
        // TODO: maybe Clean up
        //private IQueryable<DbSetType> ConstructPagedEFQuery<DbSetType>(IQueryable<DbSetType> queriableSet,
        //                                                int numRows = 5,
        //                                                int pageNumber = 0,
        //                                                string? orderByPropertyName = null,
        //                                                bool orderIsAscending = true)
        //{
        //    Type actualDbSetType = typeof(DbSetType);

        //    IQueryable<DbSetType> constructedQuery = queriableSet;

        //    if (!string.IsNullOrEmpty(orderByPropertyName))
        //    {
        //        PropertyInfo? actualOrderByPropertyName = actualDbSetType.GetProperty(orderByPropertyName);

        //        if (actualOrderByPropertyName is null)
        //            throw new Exception("orderByPropertyName Does Not Exist");

        //        // Create an expression representing the property access
        //        ParameterExpression parameter = Expression.Parameter(actualDbSetType, "x");
        //        Expression propertyAccess = Expression.Property(parameter, actualOrderByPropertyName);

        //        // Create an expression representing the lambda function: x => x.PropertyName
        //        LambdaExpression orderByExpression = Expression.Lambda(propertyAccess, parameter);

        //        // Determine if we should use OrderBy or OrderByDescending
        //        string methodName = !orderIsAscending ? "OrderByDescending" : "OrderBy";

        //        // Use reflection to call the appropriate OrderBy method
        //        MethodCallExpression methodCallExpression = Expression.Call(
        //            typeof(Queryable),
        //            methodName,
        //            new[] { actualDbSetType, actualOrderByPropertyName.PropertyType },
        //            constructedQuery.Expression,
        //            Expression.Quote(orderByExpression)
        //        );

        //        // Update the query with the new ordering
        //        constructedQuery = constructedQuery.Provider.CreateQuery<DbSetType>(methodCallExpression);
        //    }

        //    constructedQuery = constructedQuery.Skip(pageNumber * numRows).Take(numRows);

        //    return constructedQuery;
        //}
    }
}
