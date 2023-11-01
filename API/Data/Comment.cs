using System.ComponentModel.DataAnnotations;

namespace API.Data;
public class Comment
{
    public int CommentId { get; set; }
    [Required]
    public int PostId { get; set; }

    [Required]
    public Post? Post { get; set; }

    [Required]
    public required string AuthorId { get; set; }

    public ApplicationUser? Author { get; set; }

    [Required]
    public required string CommentText { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime CreationDate { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime? Deleted { get; set; }
}