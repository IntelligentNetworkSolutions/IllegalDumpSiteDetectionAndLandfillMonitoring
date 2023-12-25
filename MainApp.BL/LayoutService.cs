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

        public LayoutService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> GetLayout()
        {
            string? portalLayout = string.Empty;
            var appStartupMode = _configuration["ApplicationStartupMode"];

            if (appStartupMode == SD.ApplicationStartModes.IntranetPortal)
                portalLayout = _configuration["PortalLayout:IntranetPortalLayout"];
            
            if (appStartupMode == SD.ApplicationStartModes.PublicPortal)
                portalLayout = _configuration["PortalLayout:PublicPortalLayout"];

            if(portalLayout is null)
                portalLayout = string.Empty;

            return await Task.FromResult(portalLayout);
        }
    }
}
