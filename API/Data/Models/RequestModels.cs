using System.ComponentModel.DataAnnotations;

namespace API.Data.Models;

public record RegisterRequest(
    [Required]
    [EmailAddress]
    string Email,
    [Required]
    string UserName, 
    [Required]
    string Password);
public record LoginRequest(
    [Required]
    string Email, 
    [Required]
    string Password);
