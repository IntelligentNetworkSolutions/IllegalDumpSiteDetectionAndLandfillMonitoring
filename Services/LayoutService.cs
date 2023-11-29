using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Services
{
    public class LayoutService : ILayoutService
    {
        
        private IConfiguration _configuration;
        // TODO: Remove Context Accessor ?
        private readonly IHttpContextAccessor _contextAccessor;

        public LayoutService(IConfiguration configuration,
           
            IHttpContextAccessor contextAccessor)
        {
            _configuration = configuration;
            
            _contextAccessor = contextAccessor;
        }

        public async Task<string> GetLayout()
        {
            var appStartupMode = _configuration["ApplicationStartupMode"];
            var portalLayout = "";

            if (appStartupMode == SD.ApplicationStartModes.IntranetPortal)
            {
                portalLayout = _configuration["PortalLayout:IntranetPortalLayout"];
            }


            else if (appStartupMode == SD.ApplicationStartModes.PublicPortal)
            {
                portalLayout = _configuration["PortalLayout:PublicPortalLayout"];
            }

            return portalLayout;
        }
    }
}
