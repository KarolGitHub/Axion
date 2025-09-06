using Microsoft.EntityFrameworkCore;
using Axion.Shared.Models;

namespace Axion.ProjectService.Data
{
  public class ProjectDbContext : DbContext
  {
    public ProjectDbContext(DbContextOptions<ProjectDbContext> options) : base(options)
    {
    }

    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectTask> Tasks { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Organization> Organizations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      // Project configuration
      modelBuilder.Entity<Project>(entity =>
      {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
        entity.Property(e => e.Description).HasMaxLength(1000);
        entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
        entity.Property(e => e.Priority).HasMaxLength(50);
        entity.Property(e => e.Budget).HasPrecision(18, 2);
        entity.HasIndex(e => new { e.Name, e.OrganizationId }).IsUnique();
        entity.HasOne(e => e.Organization)
                  .WithMany()
                  .HasForeignKey(e => e.OrganizationId)
                  .OnDelete(DeleteBehavior.Restrict);
        entity.HasOne(e => e.CreatedByUser)
                  .WithMany()
                  .HasForeignKey(e => e.CreatedBy)
                  .OnDelete(DeleteBehavior.Restrict);
      });

      // Task configuration
      modelBuilder.Entity<ProjectTask>(entity =>
      {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
        entity.Property(e => e.Description).HasMaxLength(1000);
        entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
        entity.Property(e => e.Priority).HasMaxLength(50);
        entity.Property(e => e.EstimatedHours).HasPrecision(8, 2);
        entity.Property(e => e.ActualHours).HasPrecision(8, 2);
        entity.HasIndex(e => new { e.Title, e.ProjectId }).IsUnique();
        entity.HasOne(e => e.Project)
                  .WithMany()
                  .HasForeignKey(e => e.ProjectId)
                  .OnDelete(DeleteBehavior.Cascade);
        entity.HasOne(e => e.AssignedToUser)
                  .WithMany()
                  .HasForeignKey(e => e.AssignedTo)
                  .OnDelete(DeleteBehavior.Restrict);
        entity.HasOne(e => e.CreatedByUser)
                  .WithMany()
                  .HasForeignKey(e => e.CreatedBy)
                  .OnDelete(DeleteBehavior.Restrict);
      });

      // User configuration (simplified for cross-service reference)
      modelBuilder.Entity<User>(entity =>
      {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
        entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
        entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
        entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
      });

      // Organization configuration (simplified for cross-service reference)
      modelBuilder.Entity<Organization>(entity =>
      {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
      });
    }
  }
}
