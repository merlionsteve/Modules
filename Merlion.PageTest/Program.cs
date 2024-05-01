using Merlion.PageTest.Components;
using Merlion.PageTest.Resources;
using Merlion.Component;
using Merlion.Component.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using MudBlazor;
using MudBlazor.Services;
using Serilog;
using System.Net;
using System.Security.Claims;
using Merlion.Database;
using Merlion.Base.Access;
using Merlion.Base.DataConfig;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// log
Environment.CurrentDirectory = AppContext.BaseDirectory;
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .WriteTo.Console());

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// mudblazor
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopCenter;
    config.SnackbarConfiguration.VisibleStateDuration = 3000;
    config.SnackbarConfiguration.HideTransitionDuration = 200;
    config.SnackbarConfiguration.ShowTransitionDuration = 200;
});
builder.Services.AddMudMarkdownServices();
builder.Services.AddServerSideBlazor()
    .AddHubOptions(options =>
    {
        options.MaximumReceiveMessageSize = 32 * 1024 * 1000;
    });

builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, BlazorAuthorizationMiddlewareResultHandler>();

// data configuration
builder.Services.AddScoped<IDataConfigHelper, CustomDataConfigHelper>();

// some service
//builder.Services.AddCascadingAuthenticationState();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<IAccessService, AccessController>();

// locallization
builder.Services.AddLocalization();

// get ip and agent only for record login log 
builder.Services.AddHttpContextAccessor();

// multiple languages
builder.Services.AddSingleton(typeof(IStringLocalizer), typeof(StringLocalizer<LangResources>));

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);                     //to meet the UTC time in postgreSQL
DbContextSettings.CONNECTION_STRING = "server=localhost;database=AOM2;uid=postgres;pwd=postgres";
builder.Services.AddScoped<IPartialDbContextFactory, PartialDbContextFactoryDev>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseRequestLocalization(new RequestLocalizationOptions()
    .AddSupportedCultures(new[] { "", "en" })
    .AddSupportedUICultures(new[] { "", "en" }));

var avatarDirectory = Path.Combine(AppContext.BaseDirectory, "Avatars");
if (!Directory.Exists(avatarDirectory))
{
    Directory.CreateDirectory(avatarDirectory);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(avatarDirectory),
    RequestPath = "/Avatars"
});


app.UseStaticFiles();
app.UseAntiforgery();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Web API V1");
});

app.MapControllers();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
    //.AddAdditionalAssemblies(new[] { typeof(Merlion.Report.Pages.Index).Assembly });

app.Run();

// https://github.com/dotnet/aspnetcore/issues/52063
// AuthorizeRouteView
public class BlazorAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
{
    public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
    {
        await next(context);
    }
}
