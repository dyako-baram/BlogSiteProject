using API.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<BloggingDbContext>(x=>x.UseNpgsql(builder.Configuration.GetConnectionString("postgres")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<BloggingDbContext>()
    .AddDefaultTokenProviders();
builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings.
    options.User.AllowedUserNameCharacters ="abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.MapGet("/login",async (HttpContext ctx) => {
    // Clear the existing external cookie to ensure a clean login process
    await ctx.SignOutAsync(IdentityConstants.ExternalScheme);
});
app.MapPost("/login",async (RegisterRequest Input, SignInManager<ApplicationUser> _signInManager,
BloggingDbContext _db)=>
{
    // var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, true, lockoutOnFailure: true);
    // if (result.Succeeded)
    // {
    //     var user = _db. ApplicationUser.FirstOrDefault(u => u.Email.ToLower() == Input.Email.ToLower());
    //     var claim = await _userManager.GetClaimsAsync(user);
    //     if (claim.Count > 0)
    //     {
    //         try
    //         {
    //             await _userManager.RemoveClaimAsync(user, claim.FirstOrDefault(u => u.Type == "FirstName"));
    //         }
    //         catch(Exception)
    //         {

    //         }
    //     }
    //     await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("FirstName", user.Name));


    //     _logger.LogInformation("User logged in.");
    //     return LocalRedirect(returnUrl);
    // }
    // if (result.RequiresTwoFactor)
    // {
    //     return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
    // }
    // if (result.IsLockedOut)
    // {
    //     _logger.LogWarning("User account locked out.");
    //     return RedirectToPage("./Lockout");
    // }
    // else
    // {
    //     ModelState.AddModelError(string.Empty, "Invalid login attempt.");
    //     return Page();
    // }
});
app.MapGet("/seed",async (UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager)=>{
    var u=await userManager.FindByEmailAsync("admin@admin");
    if(u!=null)
    {
        return Results.BadRequest("Admin already exists");
    }
    var user = new ApplicationUser
    {
        UserName = "admin",
        Email = "admin@admin",
        EmailConfirmed = true,
        FullName = "Admin",
        ProfilePicture = "https://api.dicebear.com/7.x/adventurer-neutral/svg?seed=Cuddles",
        Bio = "Admin account",
        SocialMediaLinks = new string[] { "https://www.facebook.com/", "https://www.instagram.com/", "https://www.twitter.com/" }
    };
    var result = await userManager.CreateAsync(user, "adminPassword123!");
    if(await roleManager.RoleExistsAsync("Admin")==false)
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }
    if (result.Succeeded)
    {
        await userManager.AddToRoleAsync(user, "Admin");
    }
    return Results.Ok("Admin created");
});
app.MapGet("users",async (UserManager<ApplicationUser> userManager)=>Results.Ok(await userManager.Users.ToListAsync()));
app.MapPost("/register", async ([FromBody] RegisterRequest req, [FromServices] UserManager<ApplicationUser> userManager) =>
{
    // ApplicationUser user = new()
    // {
    //     UserName = req.Username,
    //     Email = req.Username
    // };
    // await userManager.SetUserNameAsync(user, req.Username);
    // await userManager.CreateAsync(user, req.Password);
});
app.Run();
public record RegisterRequest(string Email, string Password);


