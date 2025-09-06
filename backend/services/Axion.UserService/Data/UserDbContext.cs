using Microsoft.EntityFrameworkCore;
using Axion.Shared.Models;

namespace Axion.UserService.Data
{
  public class UserDbContext : DbContext
  {
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Organization> Organizations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      // User configuration
      modelBuilder.Entity<User>(entity =>
      {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
        entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
        entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
        entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
        entity.HasIndex(e => new { e.Email, e.OrganizationId }).IsUnique();
        entity.HasOne(e => e.Organization)
                  .WithMany(e => e.Users)
                  .HasForeignKey(e => e.OrganizationId)
                  .OnDelete(DeleteBehavior.Restrict);
      });

      // Organization configuration
      modelBuilder.Entity<Organization>(entity =>
      {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
        entity.Property(e => e.Description).HasMaxLength(500);
        entity.Property(e => e.Industry).HasMaxLength(100);
        entity.Property(e => e.Size).HasMaxLength(50);
        entity.HasIndex(e => e.Name).IsUnique();
      });
    }
  }
}
