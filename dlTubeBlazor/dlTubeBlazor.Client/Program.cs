using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazored.LocalStorage;
using dlTubeBlazor.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddLogging();
builder.Services.AddScoped( http => new HttpClient
{
    BaseAddress = new Uri( builder.HostEnvironment.BaseAddress )
} );
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<ClientAuthenticator>();
builder.Services.AddScoped<Youtube>();

await builder.Build().RunAsync();