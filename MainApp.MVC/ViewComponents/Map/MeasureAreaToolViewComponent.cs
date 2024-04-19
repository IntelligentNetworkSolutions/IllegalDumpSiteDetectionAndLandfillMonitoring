using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SD.Helpers;
using MainApp.MVC.Helpers;

namespace MainApp.MVC.ViewComponents.Map
{
    public class MeasureAreaToolViewComponent : ViewComponent
    {
        private readonly ModulesAndAuthClaimsHelper _modulesAndAuthClaims;
        public MeasureAreaToolViewComponent(ModulesAndAuthClaimsHelper modulesAndAuthClaims)
        {
            _modulesAndAuthClaims = modulesAndAuthClaims;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {   
            if (_modulesAndAuthClaims.HasModule(SD.Modules.MapToolMeasureArea) && User.HasAuthClaim(SD.AuthClaims.MapToolMeasureArea))
            {
                return View();
            }
            else
            {
                return Task.FromResult<IViewComponentResult>(Content(string.Empty)).Result;
            }
        }
    }
}
