using SD;

namespace MainApp.MVC.Helpers
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
            var allClaims = SD.AuthClaims.GetAll();
            var result = allClaims.Where(x => modules.Any(y => y == x.FromModule.Value)).ToList();

            return result;
        }

        /// <summary>
        /// Gets available modules from Tenant configuration
        /// </summary>
        /// <returns></returns>
        public async Task<ICollection<SD.Module>> GetModules()
        {
            var activeModules = _configuration.GetSection("AppSettings:Modules").Get<string[]>();
            var allModules = SD.Modules.GetAll();

            var result = allModules.Where(x => activeModules.Any(y => y == x.Value)).ToList();

            return result;
        }

        public bool HasModule(SD.Module module)
        {
            var activeModules = _configuration.GetSection("AppSettings:Modules").Get<string[]>();
            if(activeModules is null || activeModules.Length == 0)
                return false;
            
            return activeModules.Contains(module.Value);
        }

        public bool HasModule(string module)
        {
            //TODO: Review add null check is used once inside try catch
            var activeModules = _configuration.GetSection("AppSettings:Modules").Get<string[]>();
            return activeModules.Contains(module);
        }
    }
}
