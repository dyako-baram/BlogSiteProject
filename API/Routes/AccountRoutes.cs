using API.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using API.Data.Models;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using API.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Http;
namespace API.Routes;

public static class AccountRoutes
{
    public static void MapIdentityRoutes(this IEndpointRouteBuilder app)
    {
        var account=app.MapGroup("/account");
        account.MapPost("/login", 
            async (
                [FromBody] LoginRequest req,
                [FromServices] BloggingDbContext dbContext, 
                [FromServices] SignInManager<ApplicationUser> signInManager,
                [FromServices] HttpContext ctx) =>
        {
            if(ctx.User.Identity!=null)
            {
                return Results.BadRequest("already signed in");
            }
            var user = await signInManager.UserManager.FindByEmailAsync(req.Email);
            if(user==null)
            {
                return Results.BadRequest("invalid email or password");
            }
            var result = await signInManager.PasswordSignInAsync(user , req.Password, true, lockoutOnFailure: true);
            if (result.Succeeded)
            {
                return Results.Ok();
            }
            else if (result.IsNotAllowed)
            {
                // User is not allowed to sign in (e.g., email confirmation required)
                // Handle accordingly (e.g., return a message to confirm email)
                return Results.BadRequest("User is not allowed to sign in. Email confirmation may be required.");
            }
            else if (result.RequiresTwoFactor)
            {
                //TODO: Send 2FA code
                //temp code
            return Results.BadRequest("2fa codes needed");
            }
            if (result.IsLockedOut)
            {
                return Results.Problem("User account is locked out due to too many failed attempts.",statusCode: StatusCodes.Status423Locked);
            }
            else
            {
                Console.WriteLine(req.Email+" "+ req.Password);
                return Results.Unauthorized();
            }
        }).AllowAnonymous();
        account.MapGet("/logout", async ([FromServices] SignInManager<ApplicationUser> signInManager) =>
        {
            await signInManager.SignOutAsync();
            return Results.Ok();
        });
        account.MapPost("/register", async ([FromBody] RegisterRequest req,[FromServices] SignInManager<ApplicationUser> signInManager,HttpContext ctx, [FromServices] UserManager<ApplicationUser> userManager,[FromServices] IEmailSender emailSender) =>
        {
            ApplicationUser user = new()
            {
                UserName = req.UserName,
                Email = req.Email
            };
            var result=await userManager.CreateAsync(user, req.Password);
            if(result.Succeeded)
            {
                var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                

                var uriBuilder = new UriBuilder(ctx.Request.Scheme, ctx.Request.Host.Host)
                {
                    Path = "/Account/ConfirmEmail",
                    Query = $"userId={user.Id}&code={code}"
                };

                var callbackUrl = uriBuilder.Uri.ToString();
                //var callbackUrl = $"/Account/ConfirmEmail?userId={user.Id}&code={code}";

                await emailSender.SendEmailAsync(req.Email, "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                if (userManager.Options.SignIn.RequireConfirmedAccount)
                {
                    return Results.Ok("Please confirm your account by clicking on the link in the email we sent you");
                }
                else
                {
                    await signInManager.SignInAsync(user, isPersistent: true);
                    return Results.LocalRedirect("/");
                }
            }
            else
            {
                return Results.BadRequest(result.Errors);
            }
        });
        account.MapGet("/ConfirmEmail",async (string userId, string code,[FromServices] UserManager<ApplicationUser> userManager)=>
        {
            if (userId == null || code == null)
            {
                return Results.RedirectToRoute("/");
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Results.NotFound($"Unable to load user with ID '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await userManager.ConfirmEmailAsync(user, code);
            if(result.Succeeded)
            {
                return Results.Ok();
            }
            else
            {
                return Results.BadRequest(result.Errors);
            }
        });

    }
}