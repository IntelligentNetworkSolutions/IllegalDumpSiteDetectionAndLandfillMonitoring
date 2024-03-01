using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SD
{
    public class AuthClaim
    {
        /// <summary>
        /// Values must be unique across all claims
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// This will be shown to the end admin user when configuring access right to users/roles
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Do not provide value for common functionalities that are not part of a module     
        /// </summary>
        public Module FromModule { get; set; }

    }

    /// <summary>
    /// All Authorization claims are defined here
    /// </summary>
    public static class AuthClaims
    {
        public static ICollection<AuthClaim> GetAll()
        {
            Type t = typeof(AuthClaims);
            FieldInfo[] fields = t.GetFields();
            Collection<AuthClaim> claims = new Collection<AuthClaim>();
            foreach (FieldInfo fi in fields)
            {
                claims.Add((AuthClaim)fi.GetValue(null));
            }
            return claims;
        }

        public static void CheckAuthClaimsValuesForDuplicates()
        {
            var valuesList = GetAll().Select(s => s.Value);
            if (valuesList.Count() != valuesList.Distinct().Count())
            {
                throw new Exception("SD.AuthClaims have value duplicates. Rename the duplicated values!");
            }
        }

        public static void CheckAuthClaimsForInvalidCharacters()
        {
            var valuesList = GetAll().Select(s => s.Value);
            if (valuesList.Where(s => s.Contains(',')).Count() > 0)
            {
                throw new Exception("There are SD.AuthClaims with invalid characters in the Value param!");
            }
        }

        public static readonly AuthClaim UserManagement = new AuthClaim
        {
            Value = "1:1",
            FromModule = Modules.UserManagement,
            Description = "UserManagement: Manage all intranet portal users"
        };
        public static readonly AuthClaim UserManagementEditUsersAndRoles = new AuthClaim
        {
            Value = "1:2",
            FromModule = Modules.UserManagement,
            Description = "UserManagement: Edit users and roles"
        };
        public static readonly AuthClaim UserManagementAddUsersAndRoles = new AuthClaim
        {
            Value = "1:3",
            FromModule = Modules.UserManagement,
            Description = "UserManagement: Add users and roles"
        };
        public static readonly AuthClaim UserManagementDeleteUsersAndRoles = new AuthClaim
        {
            Value = "1:4",
            FromModule = Modules.UserManagement,
            Description = "UserManagement: Delete users and roles"
        };
        public static readonly AuthClaim UserManagementViewUserClaims = new AuthClaim
        {
            Value = "1:5",
            FromModule = Modules.UserManagement,
            Description = "UserManagement: View user claims"
        };
        public static readonly AuthClaim UserManagementViewUserRoles = new AuthClaim
        {
            Value = "1:6",
            FromModule = Modules.UserManagement,
            Description = "UserManagement: View user roles"
        };
        public static readonly AuthClaim UserManagementViewRoleClaims = new AuthClaim
        {
            Value = "1:7",
            FromModule = Modules.UserManagement,
            Description = "UserManagement: View role claims"
        };
        public static readonly AuthClaim AuditLog = new AuthClaim
        {
            Value = "2:1",
            FromModule = Modules.AuditLog,
            Description = "Audit Log: View Audit Log"
        };
        public static readonly AuthClaim Admin = new AuthClaim
        {
            Value = "3:1",
            FromModule = Modules.Admin,
            Description = "Admin: Admin access"
        };
        public static readonly AuthClaim SpecialActions = new AuthClaim
        {
            Value = "4:1",
            FromModule = Modules.SpecialActions,
            Description = "SpecialActions: Special Actions"
        };
        public static readonly AuthClaim SpecialActionsResetTranslationCache = new AuthClaim
        {
            Value = "4:2",
            FromModule = Modules.SpecialActions,
            Description = "SpecialActions: Reset translation cache"
        };
    } 
}


     
