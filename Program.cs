using sharpness_sharp.Components;
using sharpness_sharp.Config;

var builder = WebApplication.CreateBuilder(args);

// Habilita IHttpClientFactory y el cliente scoped para todas las instancias de la aplicacion.
builder.Services.UseHttpClient(builder.Configuration);

// El motor y sus datos salen de la sección "Database" de appsettings.json.
// Para cambiar de SQLite a Postgres no se toca una línea de C#: se cambia el JSON,
// o se exporta Database__Engine=POSTGRESQL antes de arrancar.
// UseAuth se encadena después: reutiliza el motor y la cadena que configuró UseDB.
builder.Services.UseDB(builder.Configuration).UseAuth();


// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
