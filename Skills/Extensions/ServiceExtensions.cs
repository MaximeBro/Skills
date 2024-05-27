using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.HttpResults;
using Skills.Services;
using WebApplication = Microsoft.AspNetCore.Builder.WebApplication;

namespace Skills.Extensions;

public static class ServiceExtensions
{
    public static void UseLogin(this WebApplication @this)
    {
        @this.Map("/api/login/{token:guid}", async (HttpContext context, UserTokenHoldingService service, Guid token) =>
        {
            if (!service.PendingIdentities.TryGetValue(token, out var claims))
            {
                return Results.Unauthorized();
            }
            
            var authProperties = new AuthenticationProperties { AllowRefresh = true, IsPersistent = true };
            await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claims, authProperties);
            
            return Results.Redirect("/");
        });
    }
    
    public static void UseLogout(this WebApplication @this)
    {
        @this.Map("/api/logout", async (HttpContext context) =>
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Results.Redirect("/");
        });
    }
    
    public static void UseFileUpload(this WebApplication @this)
    {
        @this.MapGet("/api/files/{name:required}", (HttpResponse response, string name) =>
        {
            response.Headers.ContentDisposition = "inline";
            var stream = new StreamReader($"wwwroot/_content/docs/{name}");
            return Results.Stream(stream.BaseStream, MimeTypes.GetMimeTypeOf(Path.GetExtension(name)));
        });
    }
    
    // public static void UseWebSocketsHandler(this WebApplication @this, string path)
    // {
    //     @this.Use((next) => async (context) =>
    //     {
    //         if (context.Request.Path == path)
    //         {
    //             if (context.WebSockets.IsWebSocketRequest)
    //             {
    //                 var circuitId = context.Request.Query["CircuitId"].FirstOrDefault();
    //                 if (!string.IsNullOrWhiteSpace(circuitId))
    //                 {
    //                     var webSocket = await context.WebSockets.AcceptWebSocketAsync();
    //                     var webSocketService = context.RequestServices.GetRequiredService<WebSocketService>();
    //                     await webSocketService.HandleAsync(webSocket, circuitId);
    //                 }
    //             }
    //             else
    //             {
    //                 context.Response.StatusCode = StatusCodes.Status400BadRequest; // 400
    //             }
    //         }
    //         else
    //         {
    //             await next(context);
    //         }
    //     });
    // }
}