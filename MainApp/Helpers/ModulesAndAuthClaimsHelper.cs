using SD;
using SD.Helpers;
using System.Reflection;

namespace MainApp.Helpers
{
    public class ModulesAndAuthClaimsHelper
    {
        private IConfiguration _configuration { get; }

        public ModulesAndAuthClaimsHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Gets available AuthClaims from Tenant module configuration
        /// </summary>
        /// <returns></returns>
        public async Task<ICollection<AuthClaim>> GetAuthClaims()
        {
            var modules = _configuration.GetSection("AppSettings:Modules").Get<string[]>();

            var result = SD.AuthClaims.GetAll().Where(x => modules.Any(y => y == x.FromModule.Value)).ToList();

            return result;
        }

        /// <summary>
        /// Gets available modules from Tenant configuration
        /// </summary>
        /// <returns></returns>
        public async Task<ICollection<SD.Module>> GetModules()
        {
            var modules = _configuration.GetSection("AppSettings:Modules").Get<string[]>();

            var result = SD.Modules.GetAll().Where(x => modules.Any(y => y == x.Value)).ToList();

            return null;
        }


        public bool HasModule(SD.Module module)
        {
            var modules = _configuration.GetSection("AppSettings:Modules").Get<string[]>();
            return modules.Contains(module.Value);
        }

        public bool HasModule(string module)
        {
            var modules = _configuration.GetSection("AppSettings:Modules").Get<string[]>();
            return modules.Contains(module);
        }
    }
}
