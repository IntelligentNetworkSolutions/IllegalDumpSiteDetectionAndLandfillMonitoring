using MainApp.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Entities;
using Services;
using MainApp.MVC.ViewModels.IntranetPortal.AuditLog;
using DAL.Repositories;
using DAL.Interfaces.Repositories;
using MainApp.MVC.Filters;

namespace MainApp.MVC.Areas.IntranetPortal.Controllers
{
    [Area("IntranetPortal")]
    public class AuditLogsController : Controller
    {
        private IAuditLogsDa _auditLogsDa;
        private AuditLogBl _auditLogBl;
        private IUserManagementDa _userManagementDa;
        private readonly IConfiguration _configuration;
        public AuditLogsController(IAuditLogsDa auditLogsDa,
            AuditLogBl auditLogBl,
            IUserManagementDa userManagementDa,
            IConfiguration configuration)
        {
            _auditLogsDa = auditLogsDa;
            _auditLogBl = auditLogBl;
            _userManagementDa = userManagementDa;
            _configuration = configuration;
        }

        [HasAuthClaim(nameof(SD.AuthClaims.AuditLog))]
        public async Task<IActionResult> Index()
        {
            try
            {
                var internalUsers = await _userManagementDa.GetAllIntanetPortalUsers();
                if (internalUsers == null)
                    return HandleErrorRedirect("ErrorViewsPath:Error404", 404);

                var auditActions = new List<string> { DbResHtml.T("Insert", "Resources").ToString(), DbResHtml.T("Update", "Resources").ToString(), DbResHtml.T("Delete", "Resources").ToString() };
                if (auditActions == null)
                    return HandleErrorRedirect("ErrorViewsPath:Error404", 404);

                var internalUserList = internalUsers.Select(x => new AuditLogUserViewModel
                {
                    Username = x.UserName,
                    FullName = x.UserName + " (" + x.LastName + " " + x.FirstName + ")"
                }).ToList();

                if (internalUserList == null)
                    return HandleErrorRedirect("ErrorViewsPath:Error404", 404);

                var model = new AuditLogViewModel
                {
                    InternalUsersList = internalUserList,
                    AuditActionsList = auditActions
                };

                if (model == null)
                    return HandleErrorRedirect("ErrorViewsPath:Error404", 404);

                return View(model);
            }
            catch (Exception)
            {
                return HandleErrorRedirect("ErrorViewsPath:Error", 400);
            }
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.AuditLog))]
        public async Task<IActionResult> _AuditLogs(string internalUsername = null, DateTime? dateFrom = null, DateTime? dateTo = null, string actionType = null, string type = null)
        {
            try
            {
                var auditLogs = await _auditLogsDa.GetAllByUsernameFromToDate(internalUsername, dateFrom, dateTo, actionType, type);
                if (auditLogs == null)
                    return HandleErrorRedirect("ErrorViewsPath:Error404", 404);

                var auditLogsListVM = auditLogs.Select(x => new AuditLogListViewModel
                {
                    AuditInternalUser = x.AuditInternalUser,
                    AuditAction = x.AuditAction,
                    EntityType = x.EntityType,
                    AuditData = x.AuditData,
                    AuditDate = x.AuditDate,
                    AuditLogId = x.AuditLogId
                }).ToList();

                if (auditLogsListVM == null)
                    return HandleErrorRedirect("ErrorViewsPath:Error404", 404);

                return PartialView(auditLogsListVM);
            }
            catch (Exception)
            {
                return HandleErrorRedirect("ErrorViewsPath:Error", 400);
            }
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.AuditLog))]
        public async Task<AuditLog> GetAuditData(int id)
        {
            var role = await _auditLogsDa.GetAuditData(id);
            return role;
        }

        /*
        TODO
        public async Task<IActionResult> ExportAuditLog(string internalUsername = null, DateTime? dateFrom = null, DateTime? dateTo = null, string actionType = null, string type = null)
        {
            var auditLogs = await _auditLogsDa.GetAllByUsernameFromToDate(internalUsername, dateFrom, dateTo, actionType, type);

            var auditLogsList = auditLogs.Select(x => new AuditLogListViewModel
            {
                AuditInternalUser = x.AuditInternalUser,
                AuditAction = x.AuditAction,
                EntityType = x.EntityType,
                AuditData = x.AuditData,
                AuditDate = x.AuditDate,
                AuditLogId = x.AuditLogId,
                TablePk = x.TablePk
            }).ToList();
            var fileResult = _auditLogBl.ExportExcelFile(auditLogsList);
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(fileResult, contentType, "ExportAuditLogs.xlsx");
        }
        */
        [HasAuthClaim(nameof(SD.AuthClaims.AuditLog))]
        public List<string> GetAuditActions()
        {
            return _auditLogsDa.GetAuditActions().Result;
        }

        [HasAuthClaim(nameof(SD.AuthClaims.UserManagementEditUsersAndRoles))]
        public async Task<IActionResult> RedirectToUserManagement(string username)
        {
            try
            {
                var user = await _userManagementDa.GetUserByUsername(username);
                if(user == null)
                    return HandleErrorRedirect("ErrorViewsPath:Error404", 404);

                return RedirectToAction("EditUser", "UserManagement", new { id = user.Id, area = "IntranetPortal" });
            }
            catch (Exception)
            {
                return HandleErrorRedirect("ErrorViewsPath:Error", 400);
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
