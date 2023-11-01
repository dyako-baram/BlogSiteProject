using API.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class BloggingDbContext : IdentityDbContext<ApplicationUser>
{
    public BloggingDbContext(DbContextOptions<BloggingDbContext> options) : base(options) { }
    public DbSet<ApplicationUser> ApplicationUser { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<ApplicationUser>()
            .Ignore(x=>x.PhoneNumber)
            .Ignore(x=>x.PhoneNumberConfirmed);
    }
}