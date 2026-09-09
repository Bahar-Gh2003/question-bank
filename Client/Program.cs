using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using System.Net.Http.Json;
using Blazored.LocalStorage;
using Client;
using Client.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// 1. Core Blazor services
builder.RootComponents.Add<App>("#app");

// 2. HttpClient for talking to the API
var apiBaseUrl = builder.Configuration["ApiBaseUrl"]
                 ?? throw new InvalidOperationException(
                     "ApiBaseUrl is not configured. See wwwroot/appsettings.json");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });
// 3. Third-party library services
builder.Services.AddMudServices();
builder.Services.AddBlazoredLocalStorage();

// 4. Authentication services
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

await builder.Build().RunAsync();