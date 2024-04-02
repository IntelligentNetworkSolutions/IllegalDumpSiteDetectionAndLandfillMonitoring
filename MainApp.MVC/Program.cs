using MainApp.MVC.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SD;
using SD.Helpers;
using Microsoft.IdentityModel.Logging;
using Services.Interfaces;
using Services;
using DAL.Helpers;
using MailSend.Interfaces;
using MailSend;
using Services.Interfaces.Services;
using DAL.Repositories;
using MainApp.MVC.Infrastructure.Register;
using MainApp.MVC.Infrastructure.Configure;
using DAL.Interfaces.Repositories;
using DAL.Interfaces.Helpers;
using MainApp.BL.Interfaces.Services;
using MainApp.BL.Services;
using MainApp.BL.Mappers;
using MainApp.MVC.Mappers;
using Westwind.Globalization.AspnetCore;
using Microsoft.AspNetCore.Mvc.Localization;
using DAL.ApplicationStorage;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Audit.EntityFramework;
using SD.Enums;
using Audit.Core;
using Microsoft.Extensions.Localization;
using Westwind.Globalization;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using DAL.Repositories.DatasetRepositories;
using DAL.Interfaces.Repositories.DatasetRepositories;
using MainApp.BL.Interfaces.Services.DatasetServices;
using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;
using MainApp.BL.Services.DatasetServices;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;
IServiceCollection services = builder.Services;

string? applicationStartMode = configuration["ApplicationStartupMode"];
if (string.IsNullOrEmpty(applicationStartMode))
    throw new Exception("ApplicationStartupMode not defined in appsettings.json");

// TODO: Whole code has to stay here Code Generation does not get the indirection here,
// remove implementation with commented one liners above them

//services.RegisterWestwindLocalization(configuration);
services.AddLocalization(options =>
{
    options.ResourcesPath = "Properties";
});

services.AddSingleton(typeof(IStringLocalizerFactory),
          typeof(DbResStringLocalizerFactory));
services.AddSingleton(typeof(IHtmlLocalizerFactory),
                      typeof(DbResHtmlLocalizerFactory));

services.AddWestwindGlobalization(opt =>
{
    opt.ResourceAccessMode = ResourceAccessMode.DbResourceManager;
    opt.DbResourceDataManagerType = typeof(DbResourcePostgreSqlDataManager);
    opt.ConnectionString = configuration.GetConnectionString("MasterDatabase");
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

services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("en", "en");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedUiCultures;
    options.RequestCultureProviders =
        new List<IRequestCultureProvider>
        {
                        new LocalizationQueryProvider { QureyParamterName = "culture" },
                        new CookieRequestCultureProvider()
        };
});

//services.RegisterMvcBuilder();
services.AddMvc()
                    .AddNewtonsoftJson(options =>
                    {
                        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                        // required for LocalizationAdmin interface for dynamic serialization
                        // (options part required for GeoJson serialization)
                    })
                    .AddViewLocalization() // for MVC/RazorPages localization
                    .AddDataAnnotationsLocalization() // for ViewModel Error Annotation Localization
                    ;

// This *has to go here* after view localization has been initialized so that Pages can localize
// Note required even if you're not using the DbResource manager.
services.AddTransient<IViewLocalizer, DbResViewLocalizer>();

//services.RegisterMicrosoftIdentity();
services.AddDefaultIdentity<ApplicationUser>()
                    .AddEntityFrameworkStores<ApplicationDbContext>();

//services.RegisterMainAppDb();
services.AddDbContext<ApplicationDbContext>();

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

//services.RegisterAuthorizationMiddleware();
services.AddAuthorization(options => new AuthorizationOptions()
{

});

services.AddHttpContextAccessor();

builder.Services.TryAddScoped<IApplicationSettingsRepo, ApplicationSettingsRepository>();
builder.Services.TryAddScoped<IAppSettingsAccessor, AppSettingsAccessor>();
builder.Services.TryAddScoped<IApplicationSettingsService, ApplicationSettingsService>();
builder.Services.TryAddScoped<AddClaimsForIntranetPortalUserHelper>();
builder.Services.TryAddScoped<PasswordValidationHelper>();
builder.Services.TryAddScoped<ModulesAndAuthClaimsHelper>();
builder.Services.TryAddScoped<IUserManagementDa, UserManagementDa>();
builder.Services.TryAddScoped<IntranetPortalUsersTokenDa>();
builder.Services.TryAddScoped<IUserManagementService, UserManagementService>();
builder.Services.TryAddScoped<IAuditLogsDa, AuditLogsDa>();
builder.Services.TryAddScoped<AuditLogBl>();
builder.Services.TryAddScoped<ILayoutService, LayoutService>();
builder.Services.TryAddScoped<IForgotResetPasswordService, ForgotResetPasswordService>();
builder.Services.TryAddScoped<IMailService, MailService>();
builder.Services.TryAddScoped<IDatasetsRepository, DatasetsRepository>();
builder.Services.TryAddScoped<IImageAnnotationsRepository, ImageAnnotationsRepository>();
builder.Services.TryAddScoped<IDatasetService, DatasetService>(); 
builder.Services.TryAddScoped<IDatasetClassesRepository, DatasetClassesRepository>(); 
builder.Services.TryAddScoped<IDataset_DatasetClassRepository, Dataset_DatasetClassRepository>(); 
builder.Services.TryAddScoped<IDatasetClassesService, DatasetClassesService>(); 
builder.Services.TryAddScoped<IDatasetImagesRepository, DatasetImagesRepository>(); 
builder.Services.TryAddScoped<IDatasetImagesService, DatasetImagesService>(); 
builder.Services.TryAddScoped<IDataset_DatasetClassService, Dataset_DatasetClassService>(); 


services.AddAutoMapper(typeof(Program).Assembly, typeof(UserManagementProfileBL).Assembly);
services.AddAutoMapper(typeof(Program).Assembly, typeof(UserManagementProfile).Assembly);
services.AddAutoMapper(typeof(Program).Assembly, typeof(DatasetProfileBL).Assembly);
services.AddAutoMapper(typeof(Program).Assembly, typeof(DatasetProfile).Assembly);

//services.RegisterAuditNet();
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
                       entity.AuditInternalUser = ev.CustomFields["audit_internal_user"] != null ? ev.CustomFields["audit_internal_user"]?.ToString() : "none";
                   })
                   .IgnoreMatchedProperties(true));


Audit.Core.Configuration.AddOnSavingAction(scope =>
{
    var efEvent = scope.GetEntityFrameworkEvent();
    if (!efEvent.Success)
        scope.Discard();
    var notIncluded = Enum.GetNames(typeof(AuditLogIgnoredTables));
    efEvent.Entries.RemoveAll(e => notIncluded.Contains(e.Table));

    if (efEvent.Entries.Count == 0)
        scope.Discard();

    foreach (var entry in efEvent.Entries)
    {
        if (entry.Action == "Update")
        {
            foreach (var change in entry.Changes)
            {
                if (change.OriginalValue is NetTopologySuite.Geometries.Geometry)
                {
                    change.OriginalValue = 
                        Entities.Helpers.GeoJsonHelpers.GeometryToGeoJson((NetTopologySuite.Geometries.Geometry)change.OriginalValue);
                    change.NewValue = 
                        Entities.Helpers.GeoJsonHelpers.GeometryToGeoJson((NetTopologySuite.Geometries.Geometry)change.NewValue);
                }
            }

            foreach (var columnValue in entry.ColumnValues)
                entry.ColumnValues.Remove(columnValue);
        }

        var tempList = entry.ColumnValues.ToList();

        foreach (var colValue in tempList)
        {
            if (colValue.Value is NetTopologySuite.Geometries.Geometry)
                entry.ColumnValues[colValue.Key] = 
                    Entities.Helpers.GeoJsonHelpers.GeometryToGeoJson((NetTopologySuite.Geometries.Geometry)colValue.Value);
        }
    }
});

var app = builder.Build();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

//app.ConfigureAuditNet(applicationStartMode);
Audit.Core.Configuration.AddCustomAction(ActionType.OnScopeCreated, scope =>
{
    scope.SetCustomField("audit_internal_user", String.Empty);
    if (applicationStartMode == ApplicationStartModes.IntranetPortal)
    {
        var httpContextAccessor = app.Services.GetRequiredService<IHttpContextAccessor>();
        scope.SetCustomField("audit_internal_user", httpContextAccessor.HttpContext?.User?.Identities?
                                                                        .First()?.FindFirst("Username")?.Value);
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

app.UseHttpsRedirection();

app.UseRequestLocalization();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    if (!app.Environment.IsDevelopment())
        context.Response.Redirect("/" + builder.Configuration.GetValue("DomainSettings:MainAppPath", ""));

    // Application Current Mode/Area and Start Mode/Area are NOT Equal
    var area = context.Request.RouteValues["area"] == null
                ? ""
                : context.Request.RouteValues["area"].ToString();
    if (area == ApplicationStartModes.IntranetPortal
        && applicationStartMode != ApplicationStartModes.IntranetPortal)
        context.Response.Redirect("/" + builder.Configuration.GetValue("DomainSettings:MainAppPath", ""));

    if (applicationStartMode == ApplicationStartModes.IntranetPortal)
    {
        app.Use(async (context, next) =>
        {
            var cookie = context.Request.Cookies[".AspNetCore.Culture"];
            var cultureParam = context.Request.Query["culture"];

            if (string.IsNullOrEmpty(cookie) && string.IsNullOrEmpty(cultureParam))
            {
                var host_url = context.Request.Host;
                string localeId = configuration.GetValue<string>("DefaultLocale:" + host_url, "en");

                var newCookieValue = CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(localeId));
                context.Response.Cookies.Append(".AspNetCore.Culture", newCookieValue);
                context.Response.Redirect(context.Request.PathBase + context.Request.Path);
            }

            await next();
        });
    }
        //app.ConfigureMissingCultureCookie(configuration);

    await next();
});

if (applicationStartMode == ApplicationStartModes.IntranetPortal)
//app.ConfigureIntranetPortalEndpoints();
{
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllerRoute(name: "default",
                                        pattern: "{area=Common}/{controller=Home}/{action=Index}/{id?}")
                .RequireAuthorization()
                ;
    });
}

if (applicationStartMode == ApplicationStartModes.PublicPortal)
//app.ConfigurePublicPortalEndpoints();
{
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllerRoute(
            name: "default",
            pattern: "{area=Common}/{controller=Home}/{action=Index}/{id?}");
    });
}

app.Run();
