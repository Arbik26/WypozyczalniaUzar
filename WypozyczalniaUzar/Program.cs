using Microsoft.FluentUI.AspNetCore.Components;
using System.Text;
using WypozyczalniaUzar.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddFluentUIComponents();

// ==================== MongoDB Configuration ====================
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDb"));

builder.Services.AddScoped<MongoDbService>();
// ================================================================

// ==================== Authentication Services (Simplified) ====================
builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddCascadingAuthenticationState();
// ================================================================

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapGet("/healthz", () => Results.Ok("Healthy")).AllowAnonymous();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
