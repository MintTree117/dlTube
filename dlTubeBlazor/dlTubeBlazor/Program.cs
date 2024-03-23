using Blazored.LocalStorage;
using dlTubeBlazor;
using dlTubeBlazor.Client.Services;
using dlTubeBlazor.Components;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddLogging();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<Authenticator>();
builder.Services.AddScoped<HttpService>();
builder.Services.AddScoped<YoutubeBrowser>();


WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts(); // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(dlTubeBlazor.Client._Imports).Assembly);

app.Run();
