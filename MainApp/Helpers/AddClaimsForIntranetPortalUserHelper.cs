using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Security.Claims;
using Models;
using SD;

namespace MainApp.Helpers
{
	public class AddClaimsForIntranetPortalUserHelper
	{
		public void AddClaims(List<Claim> claims, ApplicationUser user)
		{
			claims.Add(new Claim(ClaimTypes.Name, user.Email));
			claims.Add(new Claim("FirsName", user.FirstName));
			claims.Add(new Claim("Username", user.UserName));
			claims.Add(new Claim("UserId", user.Id));
			claims.Add(new Claim("LastName", user.LastName));			
        }
	}
}
