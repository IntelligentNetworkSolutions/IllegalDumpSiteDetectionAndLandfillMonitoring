using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SD.Helpers;

namespace MainApp.MVC.Filters
{
    /// <summary>
    /// Claim attribute authorization filter
    /// </summary>
    public class HasAuthClaimAttribute : TypeFilterAttribute
    {
        public HasAuthClaimAttribute(string claim)
        : base(typeof(AuthorizeActionFilter))
        {
            Arguments = new object[] { claim };
        }
    }

    /// <summary>
    /// Action authorization filter
    /// </summary>
    public class AuthorizeActionFilter : IAuthorizationFilter
    {
        private readonly string _claim;
        public AuthorizeActionFilter(string claim)
        {
            _claim = claim;
        }

        /// <summary>
        /// Check if logged in user has given claim
        /// </summary>
        /// <param name="context"></param>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                var claimName = (SD.AuthClaim)typeof(SD.AuthClaims).GetField(_claim).GetValue(this);
                if (!context.HttpContext.User.HasAuthClaim(claimName))
                    context.Result = new StatusCodeResult(403);
            }
            catch (Exception ex)
            {
                context.Result = new StatusCodeResult(403);
            }
        }
    }
}
