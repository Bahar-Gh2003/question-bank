using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using System.Net.Http.Json;
using Blazored.LocalStorage;
using Client;
using Client.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// ۱. ثبت سرویس‌های اصلی Blazor
builder.RootComponents.Add<App>("#app");

// ۲. ثبت HttpClient برای ارتباط با API
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7210") }); // آدرس API شما

// ۳. ثبت سرویس‌های کتابخانه‌های جانبی
builder.Services.AddMudServices();
builder.Services.AddBlazoredLocalStorage();

// ۴. ثبت سرویس‌های احراز هویت
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

await builder.Build().RunAsync();