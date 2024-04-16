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
            UserManagementViewModel model = new UserManagementViewModel();
            var users = await _userManagementService.GetAllIntanetPortalUsers() ?? throw new Exception("Users not found");
            var roles = await _userManagementService.GetAllRoles() ?? throw new Exception("Roles not found");
            var userRoles = await _userManagementService.GetAllUserRoles() ?? throw new Exception("User roles not found");

            model.Users = _mapper.Map<List<UserManagementUserViewModel>>(users);

            foreach (var user in model.Users)
            {
                var userRole = userRoles.Where(z => z.UserId == user.Id).Select(z => z.RoleId).ToList();
                user.Roles = roles.Where(z => userRole.Contains(z.Id)).ToList();
            }
            model.Roles = roles.OrderBy(x => x.Name).ToList();
            return View(model);
        }

        #region Create User
        public async Task<ResultDTO<UserManagementCreateUserViewModel>> FillUserManagementCreateUserViewModel()
        {
            try
            {
                ResultDTO<UserManagementDTO> resultFillDTO = await _userManagementService.FillUserManagementDto();
                if (resultFillDTO.IsSuccess == false)
                    return ResultDTO<UserManagementCreateUserViewModel>.Fail(resultFillDTO.ErrMsg!);

                UserManagementDTO dto = resultFillDTO.Data!;

                var model = _mapper.Map<UserManagementCreateUserViewModel>(dto);
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
            ResultDTO<UserManagementCreateUserViewModel> resultFillModel = await FillUserManagementCreateUserViewModel();
            if (resultFillModel.IsSuccess == false && ResultDTO<UserManagementCreateUserViewModel>.HandleError(resultFillModel))
                ModelState.AddModelError("ModelOnly", resultFillModel.ErrMsg!);

            return View(resultFillModel.Data);
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
            if (ModelState.IsValid == false)
                return await GetViewWithUserManagementCreateVM();

            var userManagementDTO = _mapper.Map<UserManagementDTO>(viewModel);
            if (userManagementDTO is null)
                return await GetViewWithUserManagementCreateVM("User management DTO not found");

            ResultDTO resultAdd = await _userManagementService.AddUser(userManagementDTO);
            if (resultAdd.IsSuccess == false)
                return await GetViewWithUserManagementCreateVM(resultAdd.ErrMsg);

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Edit User
        public async Task<ResultDTO<UserManagementEditUserViewModel>> FillUserManagementEditUserViewModelFromDto(UserManagementDTO dto)
        {
            try
            {
                ResultDTO<UserManagementDTO> resultFillDTO = await _userManagementService.FillUserManagementDto(dto);
                if (resultFillDTO.IsSuccess == false)
                    return ResultDTO<UserManagementEditUserViewModel>.Fail(resultFillDTO.ErrMsg!);

                dto = resultFillDTO.Data!;

                var model = _mapper.Map<UserManagementEditUserViewModel>(dto);
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

        public async Task<IActionResult> GetViewWithUserManagementEditVM(UserManagementDTO dto, string? errMsg = null)
        {
            ResultDTO<UserManagementEditUserViewModel> resultFillModel = await FillUserManagementEditUserViewModelFromDto(dto);
            if (resultFillModel.IsSuccess == false && ResultDTO<UserManagementEditUserViewModel>.HandleError(resultFillModel))
                ModelState.AddModelError("ModelOnly", resultFillModel.ErrMsg!);

            return View(resultFillModel.Data);
        }

        [HttpGet]
        [HasAuthClaim(nameof(SD.AuthClaims.UserManagementEditUsersAndRoles))]
        public async Task<IActionResult> EditUser(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            UserManagementDTO dto = new() { Id = id! };

            return await GetViewWithUserManagementEditVM(dto);
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.UserManagementEditUsersAndRoles))]
        public async Task<IActionResult> EditUser(UserManagementEditUserViewModel viewModel)
        {
            UserManagementDTO dto = new() { Id = viewModel.Id };
            if (ModelState.IsValid == false)
                return await GetViewWithUserManagementEditVM(dto);

            var userManagementDTO = _mapper.Map<UserManagementDTO>(viewModel);
            if (userManagementDTO is null)
                    return await GetViewWithUserManagementEditVM(dto, "User management DTO not found");

            ResultDTO resultUpdate = await _userManagementService.UpdateUser(userManagementDTO);
            if (resultUpdate.IsSuccess == false)
                return await GetViewWithUserManagementEditVM(dto, resultUpdate.ErrMsg);

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Create Role
        [HasAuthClaim(nameof(SD.AuthClaims.UserManagementAddUsersAndRoles))]
        public async Task<IActionResult> CreateRole()
        {
            var model = new UserManagementCreateRoleViewModel { Claims = await _modulesAndAuthClaimsHelper.GetAuthClaims() };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasAuthClaim(nameof(SD.AuthClaims.UserManagementAddUsersAndRoles))]
        public async Task<IActionResult> CreateRole(UserManagementCreateRoleViewModel viewModel)
        {
            if (ModelState.IsValid == false)
            {
                viewModel.Claims = await _modulesAndAuthClaimsHelper.GetAuthClaims();
                return View(viewModel);
            }

            var roleManagementDTO = _mapper.Map<RoleManagementDTO>(viewModel) ?? throw new Exception("Role Management DTO not found");
            await _userManagementService.AddRole(roleManagementDTO);

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Edit Role
        private async Task<ResultDTO<UserManagementEditRoleViewModel>> FillUserManagementEditRoleViewModelFromDTO(RoleManagementDTO dto)
        {
            try
            {
                ResultDTO<RoleManagementDTO> resultFillDTO = await _userManagementService.FillRoleManagementDto(dto);
                if (resultFillDTO.IsSuccess == false)
                    return ResultDTO<UserManagementEditRoleViewModel>.Fail(resultFillDTO.ErrMsg!);

                dto = resultFillDTO.Data!;

                var viewModel = _mapper.Map<UserManagementEditRoleViewModel>(dto);
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

        [HasAuthClaim(nameof(SD.AuthClaims.UserManagementEditUsersAndRoles))]
        public async Task<IActionResult> EditRole(string id)
        {
            if (id is null)
            {
                // TODO
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
            RoleManagementDTO dto = new() { Id = id! };
            ResultDTO<UserManagementEditRoleViewModel> resultFillModel = await FillUserManagementEditRoleViewModelFromDTO(dto);
            if (resultFillModel.IsSuccess == false)
                throw new Exception(resultFillModel.ErrMsg);

            return View(resultFillModel.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasAuthClaim(nameof(SD.AuthClaims.UserManagementEditUsersAndRoles))]
        public async Task<IActionResult> EditRole(UserManagementEditRoleViewModel viewModel)
        {
            if (ModelState.IsValid == false)
            {
                RoleManagementDTO dto = new() { Id = viewModel.Id };
                ResultDTO<UserManagementEditRoleViewModel> resultFillModel = await FillUserManagementEditRoleViewModelFromDTO(dto);
                if (resultFillModel.IsSuccess == false)
                    throw new Exception(resultFillModel.ErrMsg);

                return View(resultFillModel.Data);
            }

            var roleManagementDTO = _mapper.Map<RoleManagementDTO>(viewModel) ?? throw new Exception("Role Management DTO not found");
            
            ResultDTO resultUpdate = await _userManagementService.UpdateRole(roleManagementDTO);
            if (resultUpdate.IsSuccess == false)
            {
                ModelState.AddModelError("ModelOnly", resultUpdate.ErrMsg!);
                RoleManagementDTO dto = new() { Id = viewModel.Id };
                ResultDTO<UserManagementEditRoleViewModel> resultFillModel = await FillUserManagementEditRoleViewModelFromDTO(dto);
                if (resultFillModel.IsSuccess == false)
                    throw new Exception(resultFillModel.ErrMsg);

                return View(resultFillModel.Data);
            }

            return RedirectToAction(nameof(Index));
        }
        #endregion

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.UserManagementDeleteUsersAndRoles))]
        public async Task<RoleDTO?> DeleteRole(string id)
        {
            return await _userManagementService.GetRoleById(id);
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.UserManagementDeleteUsersAndRoles))]
        public async Task<IActionResult> DeleteRoleConfirmed(string id)
        {
            await _userManagementService.DeleteRole(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.UserManagementDeleteUsersAndRoles))]
        public async Task<UserDTO?> DeleteUser(string id)
        {
            return await _userManagementService.GetUserById(id);
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.UserManagementDeleteUsersAndRoles))]
        public async Task<IActionResult> DeleteUserConfirmed(string id)
        {
            await _userManagementService.DeleteUser(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.UserManagement))]
        public List<RoleClaimDTO> GetRoleClaims(string roleId)
        {
            var roleClaims = _userManagementService.GetRoleClaims(roleId).GetAwaiter().GetResult().Select(x => x.ClaimValue).ToList() ?? throw new Exception("Role claims not found");
            var listOfRoleAuthClaims = _modulesAndAuthClaimsHelper.GetAuthClaims().Result.Where(z => roleClaims.Contains(z.Value)).ToList();
            return _mapper.Map<List<RoleClaimDTO>>(listOfRoleAuthClaims);
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.UserManagement))]
        public List<UserClaimDTO> GetUserClaims(string userId)
        {
            var userClaims = _userManagementService.GetUserClaims(userId).GetAwaiter().GetResult().Select(x => x.ClaimValue).ToList() ?? throw new Exception("User claims not found");
            var listOfAuthClaims = _modulesAndAuthClaimsHelper.GetAuthClaims().Result.Where(z => userClaims.Contains(z.Value)).ToList();
            return _mapper.Map<List<UserClaimDTO>>(listOfAuthClaims);
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.UserManagement))]
        public async Task<List<RoleDTO>> GetUserRoles(string userId)
        {
            return await _userManagementService.GetRolesForUser(userId);
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
