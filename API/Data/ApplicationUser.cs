using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace API.Data;
public class ApplicationUser : IdentityUser
{
    [MaxLength(100)]
    public string? ProfilePicture { get; set; }
    public string? FullName { get; set; }
    public string? Bio { get; set; }

    [DataType(DataType.Url)]
    public string[]? SocialMediaLinks { get; set; }
    public ICollection<ApplicationUser>? Followers { get; set; } // Users who follow this user
    public ICollection<ApplicationUser>? Following { get; set; } // Users this user follows

}