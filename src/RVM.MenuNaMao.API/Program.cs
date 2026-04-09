using RVM.MenuNaMao.API.Middleware;
using RVM.MenuNaMao.Application;
using RVM.MenuNaMao.Infrastructure;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddMenuNaMaoApplication();
builder.Services.AddMenuNaMaoInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddHealthChecks();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseCors();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapControllers();
app.MapHealthChecks("/health");
app.MapRazorComponents<RVM.MenuNaMao.API.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
