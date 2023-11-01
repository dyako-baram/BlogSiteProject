using System.ComponentModel.DataAnnotations;

namespace API.Data;
public class Post
{
    public int PostId { get; set; }

    [Required]
    [MaxLength(100)]
    public required string Title { get; set; }

    [Required]
    public required string Content { get; set; }

    [Required]
    public required string AuthorId { get; set; }

    public ApplicationUser? Author { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime CreationDate { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime? LastModifiedDate { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime? Deleted { get; set; }
    public int Upvotes { get; set; }
    public int DownVotes { get; set; }
    public List<string>? Tags { get; set; }   
    public ICollection<Comment>? Comments { get; set; }

}