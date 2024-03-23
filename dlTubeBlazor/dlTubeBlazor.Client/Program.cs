using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazored.LocalStorage;
using dlTubeBlazor.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddLogging();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<Authenticator>();

await builder.Build().RunAsync();
