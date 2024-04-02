using MainApp.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MainApp.MVC.Filters
{
    public class HasAppModuleAttribute : TypeFilterAttribute
    {
        public HasAppModuleAttribute(string moduleName) : base(typeof(TenantModuleAuthorizationFilter))
        {
            Arguments = new object[] { moduleName };
        }
    }

    public class TenantModuleAuthorizationFilter : IAuthorizationFilter
    {
        private readonly ModulesAndAuthClaimsHelper _modulesAndClaimsHelper;
        private string _moduleName;

        public TenantModuleAuthorizationFilter(ModulesAndAuthClaimsHelper modulesAndClaimsHelper, string moduleName)
        {
            _modulesAndClaimsHelper = modulesAndClaimsHelper;
            _moduleName = moduleName;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                var hasModule = _modulesAndClaimsHelper.HasModule(_moduleName);
                if (hasModule == false)
                    context.Result = new StatusCodeResult(403);
            }
            catch (Exception ex)
            {
                context.Result = new StatusCodeResult(403);
            }            
        }
    }
}
