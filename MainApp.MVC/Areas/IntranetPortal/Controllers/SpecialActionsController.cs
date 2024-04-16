using Azure;
using DAL.Interfaces.Helpers;
using DTOs.MainApp.BL;
using MainApp.MVC.Filters;
using MainApp.MVC.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SD;
using System.Security.Policy;
using System.Text;
using Westwind.Globalization;

namespace MainApp.MVC.Areas.IntranetPortal.Controllers
{
    [Area("IntranetPortal")]
    [HasAppModule(nameof(Modules.SpecialActions))]
    public class SpecialActionsController : Controller
    {
        private readonly ModulesAndAuthClaimsHelper _modulesAndAuthClaims;
        private readonly IAppSettingsAccessor _appSettingsAccessor;
        private readonly IConfiguration _configuration;

        public SpecialActionsController(ModulesAndAuthClaimsHelper modulesAndAuthClaims, IConfiguration configuration, IAppSettingsAccessor appSettingsAccessor)
        {
            _modulesAndAuthClaims = modulesAndAuthClaims;
            _configuration = configuration;
            _appSettingsAccessor = appSettingsAccessor;
        }

        [HasAuthClaim(nameof(SD.AuthClaims.SpecialActions))]
        public async Task<IActionResult> Index()
        {
            return await Task.FromResult(View());
        }

        [HttpPost]
        [HasAuthClaim(nameof(SD.AuthClaims.SpecialActions))]
        public async Task<IActionResult> ResetTranslationCache()
        {
            var translationClearCacheIntranetPortalUrl = await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("TranslationClearCacheIntranetPortal", "TranslationClearCacheIntranetPortal");
            var translationClearCachePublicPortalUrl = await _appSettingsAccessor.GetApplicationSettingValueByKey<string>("TranslationClearCachePublicPortal", "TranslationClearCachePublicPortal");

            StringBuilder stringBuilderSuccess = new StringBuilder();
            StringBuilder stringBuilderError = new StringBuilder();
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage responseIntranetPortal = await client.GetAsync(translationClearCacheIntranetPortalUrl.Data);
                HttpResponseMessage responsePublicPortal = await client.GetAsync(translationClearCachePublicPortalUrl.Data);

                var resultIntranetPortal = StringBuilderAppend(responseIntranetPortal, "Intranet portal");
                if(resultIntranetPortal.StringBuilderSuccess != null)
                {
                    stringBuilderSuccess.Append(resultIntranetPortal.StringBuilderSuccess);
                }
                else
                {
                    stringBuilderError.Append(resultIntranetPortal.StringBuilderError);
                }
                
                var resultPublicPortal = StringBuilderAppend(responsePublicPortal, "Public portal");
                if (resultPublicPortal.StringBuilderSuccess != null)
                {
                    stringBuilderSuccess.Append(resultPublicPortal.StringBuilderSuccess);
                }
                else
                {
                    stringBuilderError.Append(resultPublicPortal.StringBuilderError);
                }

                return Json(new { stringBuilderSuccess = stringBuilderSuccess.ToString(), stringBuilderError = stringBuilderError.ToString() });

            }
            catch (Exception ex)
            {
                var generalError = $"An error occurred while resetting translation cache: {ex.Message}";
                return Json(new {generalError = generalError});
                
            }
        }

        [HasAuthClaim(nameof(SD.AuthClaims.SpecialActions))]
        private StringBuilderResultDTO StringBuilderAppend(HttpResponseMessage response, string portalName)
        {
            StringBuilderResultDTO result = new StringBuilderResultDTO();
            if (response.IsSuccessStatusCode)
            {
                result.StringBuilderSuccess = new StringBuilder();
                result.StringBuilderSuccess.Append(DbResHtml.T($"{portalName} translation cache cleared successfully. ", "Resources"));
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                result.StringBuilderError = new StringBuilder();
                result.StringBuilderError.Append(DbResHtml.T($"{portalName} - The requested resource could not be found. ", "Resources"));
            }
            else
            {
                result.StringBuilderError = new StringBuilder();
                result.StringBuilderError.Append(DbResHtml.T($"{portalName} - An error occurred while processing your request. ", "Resources"));
            }
            return result;
        }
        
    }
}
