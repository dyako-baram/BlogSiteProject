using System.ComponentModel.DataAnnotations;

namespace API.Data;
public class Notification
{
    public int NotificationId { get; set; }

    [Required]
    public required string UserId { get; set; }

    public ApplicationUser? User { get; set; }

    [Required]
    [MaxLength(500)]
    public required string Content { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime CreationDate { get; set; }

    [Required]
    public bool IsRead { get; set; }
}