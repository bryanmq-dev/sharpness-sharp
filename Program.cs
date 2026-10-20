using sharpness_sharp.Components;
using sharpness_sharp.Config;
using sharpness_sharp.Repositories;
using sharpness_sharp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.UseHttpClient(builder.Configuration);

builder.Services.UseAuth(builder.Configuration);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<ProductoRepository>();
builder.Services.AddScoped<ProductoService>();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapAdditionalIdentityEndpoints();

app.UseAPIEndpoints();

await app.SeedAuth();

app.Run();
