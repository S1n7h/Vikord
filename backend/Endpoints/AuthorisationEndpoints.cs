using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth");

        // 1. Trigger Discord Login
        group.MapGet("/login", () =>
        {
            // Challenge the "Discord" scheme and set RedirectUri to your frontend (e.g. "http://localhost:5173")
            return Results.Challenge(
            new AuthenticationProperties { RedirectUri = "http://localhost:6969/" },
            authenticationSchemes: ["Discord"]);
        });

        // 2. Fetch User Profile
        group.MapGet("/me", (ClaimsPrincipal user) =>
        {
            if (user.Identity?.IsAuthenticated != true)
            {
                return Results.Unauthorized();
            }

            var id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var name = user.FindFirst(ClaimTypes.Name)?.Value;
            var globalName = user.FindFirst("urn:discord:global_name")?.Value;

            return Results.Ok(new { id, name, globalName });
        });

        // 3. Logout
        group.MapPost("/logout", async (HttpContext httpContext) =>
        {
            // This removes only this application's cookie. It does not sign the person out of Discord itself.
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Results.Ok(new { message = "Logged out successfully" });

        });

        return app;
    }
}