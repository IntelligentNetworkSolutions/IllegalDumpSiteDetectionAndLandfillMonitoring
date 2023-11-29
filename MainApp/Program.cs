using Dal.ApplicationStorage;
using MainApp;
using MainApp.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using Models;
using System.Globalization;
using System;
using Microsoft.Extensions.Options;
using SD;
using Microsoft.IdentityModel.Logging;
using Services.Interfaces;
using Services;
using Dal;
using Dal.Helpers;
using MailSend.Interfaces;
using MailSend;
using Audit.Core;
using Audit.EntityFramework;
using Microsoft.AspNetCore.Http;
using SD.Enums;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Westwind.Globalization.AspnetCore;
using SD.Helpers;
using Westwind.Globalization;
using Services.Interfaces.Repositories;

var builder = WebApplication.CreateBuilder(args);
string applicationStartMode = builder.Configuration["ApplicationStartupMode"];
if (applicationStartMode == null)
{
    throw new Exception("ApplicationStartupMode not defined in appsettings.json");
}

builder.Services.AddLocalization(options =>
{
    options.ResourcesPath = "Properties";
});

builder.Services.AddSingleton(typeof(IStringLocalizerFactory),
                      typeof(DbResStringLocalizerFactory));
builder.Services.AddSingleton(typeof(IHtmlLocalizerFactory),
                      typeof(DbResHtmlLocalizerFactory));

builder.Services.AddWestwindGlobalization(opt =>
{
    opt.ResourceAccessMode = ResourceAccessMode.DbResourceManager;
    opt.DbResourceDataManagerType = typeof(DbResourcePostgreSqlDataManager);
    opt.ConnectionString = builder.Configuration.GetConnectionString("MasterDatabase");
    opt.DataProvider = DbResourceProviderTypes.PostgreSql;
    opt.ResourceTableName = "Localizations";
    opt.AddMissingResources = true;
    opt.ResxBaseFolder = "~/Properties/";
    opt.ConfigureAuthorizeLocalizationAdministration(actionContext =>
    {
        return actionContext.HttpContext.User.HasCustomClaim("SpecialAuthClaim", "insadmin");
    });

});
CultureInfo[] supportedCultures = new[]
{
                new CultureInfo("en")
            };

CultureInfo[] supportedUiCultures = new[]
{
                new CultureInfo("mk"),
                new CultureInfo("sq"),
                new CultureInfo("en")
            };

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("en", "en");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedUiCultures;
    options.RequestCultureProviders = new List<IRequestCultureProvider>
                        {
                            new LocalizationQueryProvider {
                                QureyParamterName = "culture"
                            },
                            new CookieRequestCultureProvider()
                        };

});

builder.Services.AddMvc()            
            .AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore)
            .AddViewLocalization()
            .AddDataAnnotationsLocalization();

builder.Services.AddDefaultIdentity<ApplicationUser>().AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddDbContext<ApplicationDbContext>();

if (applicationStartMode == ApplicationStartModes.IntranetPortal)
{
    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                   .AddCookie((options =>
                   {
                       options.Cookie.HttpOnly = true;
                       options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                       options.Cookie.SameSite = SameSiteMode.None;
                       options.LoginPath = "/IntranetPortal/Account/Login";
                   }));
}

builder.Services.AddAuthorization();
builder.Services.AddTransient<IViewLocalizer, DbResViewLocalizer>();
builder.Services.AddHttpContextAccessor();

builder.Services.TryAddScoped<ApplicationSettingsHelper>();
builder.Services.TryAddScoped<AddClaimsForIntranetPortalUserHelper>();
builder.Services.TryAddScoped<PasswordValidationHelper>();
builder.Services.TryAddScoped<ModulesAndAuthClaimsHelper>();
builder.Services.TryAddScoped<UserManagementDa>();
builder.Services.TryAddScoped<IntranetPortalUsersTokenDa>();
builder.Services.TryAddScoped<IApplicationSettingsDa, ApplicationSettingsDa>();
builder.Services.TryAddScoped<AuditLogsDa>();
builder.Services.TryAddScoped<AuditLogBl>();
builder.Services.TryAddScoped<ILayoutService, LayoutService>();
builder.Services.TryAddScoped<IForgotResetPasswordService, ForgotResetPasswordService>();
builder.Services.TryAddScoped<IMailService, MailService>();

Audit.Core.Configuration.Setup()
               .UseEntityFramework(_ => _
                   .AuditTypeMapper(t => typeof(AuditLog))
                   .AuditEntityAction<AuditLog>((ev, entry, entity) =>
                   {

                       entity.AuditData = entry.ToJson();
                       entity.EntityType = entry.EntityType.Name;
                       entity.AuditDate = DateTime.Now;
                       entity.TablePk = entry.PrimaryKey.First().Value.ToString();
                       entity.AuditAction = entry.Action;
                       entity.AuditInternalUser = (ev.CustomFields["audit_internal_user"] != null) ? ev.CustomFields["audit_internal_user"]?.ToString() : "none";
                   })
                   .IgnoreMatchedProperties(true));


Audit.Core.Configuration.AddOnSavingAction(scope =>
{
    var efEvent = scope.GetEntityFrameworkEvent();
    if (!efEvent.Success)
    {
        scope.Discard();
    }
    else
    {
        var notIncluded = Enum.GetNames(typeof(AuditLogIgnoredTables));
        efEvent.Entries.RemoveAll(e => notIncluded.Contains(e.Table));

        if (efEvent.Entries.Count == 0)
        {
            scope.Discard();
        }

        foreach (var entry in efEvent.Entries)
        {
            if (entry.Action == "Update")
            {

                foreach (var change in entry.Changes)
                {
                    if (change.OriginalValue is NetTopologySuite.Geometries.Geometry)
                    {
                        change.OriginalValue = Models.Helpers.GeoJsonHelpers.GeometryToGeoJson((NetTopologySuite.Geometries.Geometry)change.OriginalValue);
                        change.NewValue = Models.Helpers.GeoJsonHelpers.GeometryToGeoJson((NetTopologySuite.Geometries.Geometry)change.NewValue);
                    }
                }

                foreach (var columnValue in entry.ColumnValues)
                    entry.ColumnValues.Remove(columnValue);
            }

            var tempList = entry.ColumnValues.ToList();

            foreach (var colValue in tempList)
            {
                if (colValue.Value is NetTopologySuite.Geometries.Geometry)
                {
                    entry.ColumnValues[colValue.Key] = Models.Helpers.GeoJsonHelpers.GeometryToGeoJson((NetTopologySuite.Geometries.Geometry)colValue.Value);
                }
            }

        }

    }
});

var app = builder.Build();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
Audit.Core.Configuration.AddCustomAction(ActionType.OnScopeCreated, scope =>
{
    scope.SetCustomField("audit_internal_user", String.Empty);
    if (applicationStartMode == ApplicationStartModes.IntranetPortal)
    {
        var httpContextAccessor = app.Services.GetRequiredService<IHttpContextAccessor>();
        scope.SetCustomField("audit_internal_user", httpContextAccessor.HttpContext?.User?.Identities?.First()?.FindFirst("Username")?.Value);
    }
});

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    IdentityModelEventSource.ShowPII = true;
    Modules.CheckModuleValuesForDuplicates();
    AuthClaims.CheckAuthClaimsValuesForDuplicates();
    AuthClaims.CheckAuthClaimsForInvalidCharacters();

}
else
{
    app.UseExceptionHandler("/Error");
    app.UseStatusCodePagesWithRedirects("/Common/Error/Error/{0}");
    app.UseHsts();
}

if (applicationStartMode == null)
{
    throw new Exception("ApplicationStartupMode not defined in appsettings.json");
}

app.UseHttpsRedirection();
app.UseRequestLocalization();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    var area = context.Request.RouteValues["area"] == null ? "" : context.Request.RouteValues["area"].ToString();
    var controller = context.Request.RouteValues["controller"] == null ? "" : context.Request.RouteValues["controller"].ToString();
    var action = context.Request.RouteValues["action"] == null ? "" : context.Request.RouteValues["action"].ToString();


    if (!app.Environment.IsDevelopment())
    {
        context.Response.Redirect("/" + builder.Configuration.GetValue("DomainSettings:MainAppPath", ""));
    }

    if (area == ApplicationStartModes.IntranetPortal && applicationStartMode != ApplicationStartModes.IntranetPortal)
    {
        context.Response.Redirect("/" + builder.Configuration.GetValue("DomainSettings:MainAppPath", ""));        
    }
    if(applicationStartMode == ApplicationStartModes.IntranetPortal)
    {
        var cookie = context.Request.Cookies[".AspNetCore.Culture"];
        var cultureParam = context.Request.Query["culture"];

        if (string.IsNullOrEmpty(cookie) && string.IsNullOrEmpty(cultureParam))
        {
            var host_url = context.Request.Host;
            string localeId = builder.Configuration.GetValue<string>("DefaultLocale:" + host_url, "en");

            var newCookieValue = CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(localeId));
            context.Response.Cookies.Append(".AspNetCore.Culture", newCookieValue);
            context.Response.Redirect(context.Request.PathBase + context.Request.Path);
        }
    }

    await next();
});
if (applicationStartMode == ApplicationStartModes.IntranetPortal)
{
    app.MapControllers().RequireAuthorization();
    app.MapRazorPages();
    app.MapControllerRoute(
        name: "default",
        pattern: "{area=Common}/{controller=Home}/{action=Index}/{id?}");

}
else if (applicationStartMode == ApplicationStartModes.PublicPortal)
{
    app.MapControllerRoute(
        name: "default",
        pattern: "{area=Common}/{controller=Home}/{action=Index}/{id?}");
}

app.Run();
