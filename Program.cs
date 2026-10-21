using sharpness_sharp.Components;
using sharpness_sharp.Config;

var builder = WebApplication.CreateBuilder(args);

builder.Services.UseHttpClient(builder.Configuration);

builder.Services.UseAuth(builder.Configuration);

builder.Services.AddAuthorization();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

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

app.UseWhen(
    contexto => !contexto.Request.Path.StartsWithSegments("/api"),
    rama => rama.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true));
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapAdditionalIdentityEndpoints();

app.UseAPIEndpoints();

await app.SeedAuth();

app.Run();
