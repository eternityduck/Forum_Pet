using DAL.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class ForumContext : IdentityDbContext<User>
    {
        public ForumContext(DbContextOptions<ForumContext> options)
            : base(options)
        {
           
            Database.EnsureCreated();
        }
       
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        
        public DbSet<Topic> Topics { get; set; }
    }
}