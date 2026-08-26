using System.Security.Claims;
using System.Text.Json;
using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
public static class Authorisation
{
    public static IServiceCollection AddAuthorisation(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthorization();
        
        services.AddAuthentication(options =>
        {
            // Read incoming requests via cookies
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            
            // Redirect unauthenticated requests to Discord
            options.DefaultChallengeScheme = "Discord";
        })
        .AddCookie()
        .AddOAuth("Discord", options =>
        {
            var discordClientId = configuration["Discord:ClientId"];
            var discordClientSecret = configuration["Discord:ClientSecret"];

            options.ClientId = discordClientId;
            options.ClientSecret = discordClientSecret;

            options.CallbackPath = "/auth/callback";
            options.AuthorizationEndpoint = "https://discord.com/api/oauth2/authorize";
            options.TokenEndpoint = "https://discord.com/api/oauth2/token";
            options.UserInformationEndpoint = "https://discord.com/api/users/@me";

            options.Scope.Add("identify");

            options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
            options.ClaimActions.MapJsonKey(ClaimTypes.Name, "username");
            options.ClaimActions.MapJsonKey("urn:discord:global_name", "global_name");

            options.Events = new OAuthEvents
            {
                OnCreatingTicket = async context =>
                {
                    // The handler already validated state and exchanged the authorization code for tokens before
                    // this event. Fetch the Discord profile with its access token, then apply the mappings above.
                    using var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                    request.Headers.Authorization = new("Bearer", context.AccessToken);

                    using var response = await context.Backchannel.SendAsync(
                        request,
                        HttpCompletionOption.ResponseHeadersRead,
                        context.HttpContext.RequestAborted);
                    response.EnsureSuccessStatusCode();

                    // RequestAborted stops the network operation if the browser disconnects before it completes.
                    await using var stream = await response.Content.ReadAsStreamAsync(context.HttpContext.RequestAborted);
                    using var user = await JsonDocument.ParseAsync(stream, cancellationToken: context.HttpContext.RequestAborted);
                    context.RunClaimActions(user.RootElement);

                    //get User's Discord Id
                    var discordUserId = user.RootElement.GetProperty("id").GetString();
                    var global_name = user.RootElement.GetProperty("global_name").GetString();  

                    if (discordUserId == null)
                    {
                        throw new Exception("Discord user ID was not returned.");
                    }

                    var dbContext = context.HttpContext.RequestServices.GetRequiredService<ChatContext>();

                    var existingUser = await dbContext.Users.FindAsync(discordUserId);

                    if (existingUser == null)
                    {
                        dbContext.Users.Add(new User{ 
                            UserId = discordUserId,
                            GlobalName = global_name
                        });
                        await dbContext.SaveChangesAsync();
                    }
                    else if (existingUser.GlobalName == null)
                    {
                        existingUser.GlobalName = global_name;
                        await dbContext.SaveChangesAsync();
                    }

                    context.HttpContext.RequestServices
                        .GetRequiredService<ILoggerFactory>()
                        .CreateLogger("DiscordOAuth")
                        .LogInformation(
                        "Discord OAuth callback invoked for Discord user {discordUserId}.",
                        user.RootElement.GetProperty("id").GetString());
                }
            };
        });    

        return services;
    }
}