using MainApp.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Entities;
using Services;
using MainApp.MVC.ViewModels.IntranetPortal.AuditLog;
using DAL.Repositories;
using DAL.Interfaces.Repositories;

namespace MainApp.MVC.Areas.IntranetPortal.Controllers
{
    [Area("IntranetPortal")]
    public class AuditLogsController : Controller
    {
        private IAuditLogsDa _auditLogsDa;
        private AuditLogBl _auditLogBl;
        private IUserManagementDa _userManagementDa;
        public AuditLogsController(IAuditLogsDa auditLogsDa,
            AuditLogBl auditLogBl,
            IUserManagementDa userManagementDa)
        {
            _auditLogsDa = auditLogsDa;
            _auditLogBl = auditLogBl;
            _userManagementDa = userManagementDa;
        }
        public async Task<IActionResult> Index()
        {
            // TODO: add check claim
            //if (!User.HasAuthClaim(SD.AuthClaims.AuditLog) || !_modulesAndAuthClaims.HasModule(SD.Modules.AuditLog))
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
            var internalUsers = await _userManagementDa.GetAllIntanetPortalUsers();
            var auditActions = new List<string> { DbResHtml.T("Insert", "Resources").ToString(), DbResHtml.T("Update", "Resources").ToString(), DbResHtml.T("Delete", "Resources").ToString() };

            var internalUserList = internalUsers.Select(x => new AuditLogUserViewModel
            {
                Username = x.UserName,
                FullName = x.UserName + " (" + x.LastName + " " + x.FirstName + ")"
            }).ToList();           

            var model = new AuditLogViewModel
            {
                InternalUsersList = internalUserList,
                AuditActionsList = auditActions
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> _AuditLogs(string internalUsername = null, DateTime? dateFrom = null, DateTime? dateTo = null, string actionType = null, string type = null)
        {
            var auditLogs = await _auditLogsDa.GetAllByUsernameFromToDate(internalUsername, dateFrom, dateTo, actionType, type);

            var auditLogsListVM = auditLogs.Select(x => new AuditLogListViewModel
            {
                AuditInternalUser = x.AuditInternalUser,
                AuditAction = x.AuditAction,
                EntityType = x.EntityType,
                AuditData = x.AuditData,
                AuditDate = x.AuditDate,
                AuditLogId = x.AuditLogId
            }).ToList();

            return PartialView(auditLogsListVM);
        }

        [HttpPost]
        public async Task<AuditLog> GetAuditData(int id)
        {
            // TODO: add check claim
            //if (!User.HasAuthClaim(SD.AuthClaims.AuditLog) || !_modulesAndAuthClaims.HasModule(SD.Modules.AuditLog))
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
        public List<string> GetAuditActions()
        {
            return _auditLogsDa.GetAuditActions().Result;
        }

        public async Task<IActionResult> RedirectToUserManagement(string username)
        {
            // TODO: add check claim
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
            var user = await _userManagementDa.GetUserByUsername(username);
            return RedirectToAction("EditUser", "UserManagement", new { id = user.Id, area = "IntranetPortal" });
        }
               
    }
}
