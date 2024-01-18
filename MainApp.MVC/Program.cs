using MainApp.MVC.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SD;
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

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;
IServiceCollection services = builder.Services;

string? applicationStartMode = configuration["ApplicationStartupMode"];
if (string.IsNullOrEmpty(applicationStartMode))
    throw new Exception("ApplicationStartupMode not defined in appsettings.json");

services.RegisterWestwindLocalization(configuration);

services.RegisterMvc();

services.RegisterMicrosoftIdentity();

services.RegisterMainAppDb();

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

services.RegisterAuthorizationMiddleware();

services.AddHttpContextAccessor();

builder.Services.TryAddScoped<IApplicationSettingsRepo, ApplicationSettingsRepo>();
builder.Services.TryAddScoped<IAppSettingsAccessor, AppSettingsAccessor>();
builder.Services.TryAddScoped<ApplicationSettingsHelper>();
builder.Services.TryAddScoped<IApplicationSettingsService, ApplicationSettingsService>();
builder.Services.TryAddScoped<AddClaimsForIntranetPortalUserHelper>();
builder.Services.TryAddScoped<PasswordValidationHelper>();
builder.Services.TryAddScoped<ModulesAndAuthClaimsHelper>();
builder.Services.TryAddScoped<IUserManagementDa, UserManagementDa>();
builder.Services.TryAddScoped<IntranetPortalUsersTokenDa>();
builder.Services.TryAddScoped<ApplicationSettingsDa>();
//builder.Services.TryAddScoped<IApplicationSettingsDa, ApplicationSettingsDa>();
builder.Services.TryAddScoped<IUserManagementService, UserManagementService>();
builder.Services.TryAddScoped<AuditLogsDa>();
builder.Services.TryAddScoped<AuditLogBl>();
builder.Services.TryAddScoped<ILayoutService, LayoutService>();
builder.Services.TryAddScoped<IForgotResetPasswordService, ForgotResetPasswordService>();
builder.Services.TryAddScoped<IMailService, MailService>();
services.AddAutoMapper(typeof(Program).Assembly, typeof(UserManagementProfileBL).Assembly);
services.AddAutoMapper(typeof(Program).Assembly, typeof(UserManagementProfile).Assembly);

services.RegisterAuditNet();

var app = builder.Build();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

app.ConfigureAuditNet(applicationStartMode);

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
    
    if(applicationStartMode == ApplicationStartModes.IntranetPortal)
        app.ConfigureMissingCultureCookie(configuration);

    await next();
});

if (applicationStartMode == ApplicationStartModes.IntranetPortal)
    app.ConfigureIntranetPortalEndpoints();

if (applicationStartMode == ApplicationStartModes.PublicPortal)
    app.ConfigurePublicPortalEndpoints();

app.Run();
