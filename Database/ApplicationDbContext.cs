using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace WAP_PIS.Database;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<Account> Account { get; set; }
    public DbSet<Manager> Manager { get; set; }
    public DbSet<Secretary> Secretary { get; set; }
    public DbSet<Meeting> Meeting { get; set; }
    public DbSet<Notification> Notification { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Manager>()
            .HasMany<Meeting>(m => m.Meetings).WithMany(m => m.Attendees);

        modelBuilder.Entity<Meeting>()
            .HasOne<Manager>(m => m.Owner);

        modelBuilder.Entity<Meeting>()
            .HasMany<Manager>(m => m.Attendees).WithMany(m => m.Meetings);

        modelBuilder.Entity<Meeting>()
            .HasMany<Notification>(m=>m.Notifications).WithOne(n => n.Meeting);

        modelBuilder.Entity<Manager>()
            .HasMany<Notification>(m=>m.Notifications).WithOne(n => n.Recipient);

        base.OnModelCreating(modelBuilder);
    }
}
