namespace MainApp.MVC.ViewModels.IntranetPortal.AuditLog
{
    public class AuditLogViewModel
    {
        public List<AuditLogUserViewModel> InternalUsersList { get; set; }
        public List<string> AuditActionsList { get; set; }
    }
}
