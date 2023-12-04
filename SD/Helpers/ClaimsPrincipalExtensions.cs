using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace SD.Helpers
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool HasAuthClaim(this IPrincipal user, AuthClaim authClaim)
        {
            // TODO: Return Real Value
            return true;
            var result = false;
            if (HasCustomClaim(user, "SpecialAuthClaim", "insadmin") ||
                HasCustomClaim(user, "AuthorizationClaim", authClaim.Value))
            {
                result = true;
            }
            return result;
        }

        public static bool HasAnyAuthClaim(this IPrincipal user, ICollection<AuthClaim> authClaims)
        {
            foreach (var claim in authClaims)
            {
                if (user.HasAuthClaim(claim))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool HasCustomClaim(this IPrincipal user, string type, string value)
        {
            bool result = false;

            var userPrincipal = (user as ClaimsPrincipal);

            result = userPrincipal.HasClaim(c => c.Type == type &&
                ((!c.Value.Contains(",") && c.Value.Contains(value)) ||  //Vo slucaj koga ima edna vrednost
                (c.Value.Contains(",") && c.Value.Contains("\"" + value + "\"")))); //Vo slucaj koga ima povekje vrednosti

            return result;
        }
    }
}
