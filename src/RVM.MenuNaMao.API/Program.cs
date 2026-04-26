using Microsoft.Extensions.FileProviders;
using RVM.Common.Security;
using RVM.MenuNaMao.API.Middleware;
using RVM.MenuNaMao.Application;
using RVM.MenuNaMao.Infrastructure;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, loggerConfiguration) =>
{
    loggerConfiguration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console();

    var seqUrl = context.Configuration["Seq:ServerUrl"];
    if (!string.IsNullOrEmpty(seqUrl))
        loggerConfiguration.WriteTo.Seq(seqUrl);
});

builder.Services.AddMenuNaMaoApplication();
builder.Services.AddMenuNaMaoInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddHealthChecks();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RVM.MenuNaMao.Infrastructure.Data.MenuNaMaoDbContext>();
    await db.Database.EnsureCreatedAsync();
}

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<GlobalExceptionMiddleware>();

if (!app.Environment.IsDevelopment()) { app.UseHsts(); }
app.UseSecurityHeaders();

app.UseCors();

app.UseStaticFiles();

// Serve React client app (customer menu)
var clientAppPath = Path.Combine(app.Environment.ContentRootPath, "clientapp");
if (Directory.Exists(clientAppPath))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(clientAppPath),
        RequestPath = ""
    });
}

app.UseAntiforgery();

app.MapControllers();
app.MapHealthChecks("/health");

// SPA fallback for customer-facing React routes
if (Directory.Exists(clientAppPath))
{
    var indexPath = Path.Combine(clientAppPath, "index.html");
    app.MapGet("/menu/{**slug}", async context =>
    {
        context.Response.ContentType = "text/html";
        await context.Response.SendFileAsync(indexPath);
    });
    app.MapGet("/cart", async context =>
    {
        context.Response.ContentType = "text/html";
        await context.Response.SendFileAsync(indexPath);
    });
    app.MapGet("/checkout", async context =>
    {
        context.Response.ContentType = "text/html";
        await context.Response.SendFileAsync(indexPath);
    });
    app.MapGet("/order/{**slug}", async context =>
    {
        context.Response.ContentType = "text/html";
        await context.Response.SendFileAsync(indexPath);
    });
}

app.MapRazorComponents<RVM.MenuNaMao.API.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
