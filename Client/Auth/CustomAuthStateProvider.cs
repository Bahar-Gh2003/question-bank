using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace Client.Auth;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private readonly HttpClient _http;

    public CustomAuthStateProvider(ILocalStorageService localStorage, HttpClient http)
    {
        _localStorage = localStorage;
        _http = http;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var authToken = await _localStorage.GetItemAsync<string>("authToken");
        if (string.IsNullOrWhiteSpace(authToken))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        var claims = ParseClaimsFromJwt(authToken);
        var claimsIdentity = new ClaimsIdentity(claims, "jwtAuth", ClaimTypes.Name, ClaimTypes.Role);
        
        // Populate the service from the token
        
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", authToken);
        return new AuthenticationState(new ClaimsPrincipal(claimsIdentity));
    }

    public void NotifyUserAuthentication(string token)
    {
        var claims = ParseClaimsFromJwt(token);
        var claimsIdentity = new ClaimsIdentity(claims, "jwtAuth", ClaimTypes.Name, ClaimTypes.Role);
        var authenticatedUser = new ClaimsPrincipal(claimsIdentity);
        var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
        
        // Populate the service from the token
        
        NotifyAuthenticationStateChanged(authState);
    }

    public void NotifyUserLogout()
    {
        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
        var authState = Task.FromResult(new AuthenticationState(anonymousUser));
        _http.DefaultRequestHeaders.Authorization = null;
        NotifyAuthenticationStateChanged(authState);
    }

    public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);
        return token.Claims;
    }
}