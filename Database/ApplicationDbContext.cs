using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace WAP_PIS.Database;
public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<Account> Account {get; set;}
    public DbSet<Manager> Manager {get; set;}
    public DbSet<Secretary> Secretary {get; set;}
    public DbSet<Meeting> Meeting {get; set;}
    public DbSet<Notification> Notification {get; set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Manager>()
            .HasNoKey();
        
        modelBuilder.Entity<Secretary>()
            .HasNoKey();
    }
}
