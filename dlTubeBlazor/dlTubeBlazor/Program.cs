using Blazored.LocalStorage;
using dlTubeBlazor.Client.Services;
using dlTubeBlazor.Components;
using dlTubeBlazor.Features.Authentication;
using dlTubeBlazor.Features.Youtube;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddLogging();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<AuthenticatorRepository>();
builder.Services.AddScoped<AuthenticatorService>();
builder.Services.AddScoped<Youtube>();
builder.Services.AddScoped<YoutubeBrowser>();
builder.Services.AddScoped<YoutubeStreamer>();

WebApplication app = builder.Build();

app.UseMiddleware<AuthenticatorMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts(); // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
}

app.MapYoutubeEndpoints();

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(dlTubeBlazor.Client._Imports).Assembly);

app.Run();
