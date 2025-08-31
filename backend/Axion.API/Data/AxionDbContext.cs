using Microsoft.EntityFrameworkCore;
using Axion.API.Models;

namespace Axion.API.Data;

public class AxionDbContext : DbContext
{
    public AxionDbContext(DbContextOptions<AxionDbContext> options) : base(options)
    {
    }

    public DbSet<Organization> Organizations { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<ChatRoom> ChatRooms { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<SSOProvider> SSOProviders { get; set; }
    public DbSet<SSOLogin> SSOLogins { get; set; }
    public DbSet<TwoFactorAuth> TwoFactorAuths { get; set; }
    public DbSet<TwoFactorCode> TwoFactorCodes { get; set; }
    public DbSet<BackupCode> BackupCodes { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<ProjectTask> Tasks { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Resource> Resources { get; set; }
    public DbSet<Booking> Bookings { get; set; }

    // Third-party integrations
    public DbSet<ThirdPartyIntegration> ThirdPartyIntegrations { get; set; }
    public DbSet<IntegrationEvent> IntegrationEvents { get; set; }
    public DbSet<IntegrationSync> IntegrationSyncs { get; set; }

    // GitHub integrations
    public DbSet<GitHubRepository> GitHubRepositories { get; set; }
    public DbSet<GitHubIssue> GitHubIssues { get; set; }
    public DbSet<GitHubPullRequest> GitHubPullRequests { get; set; }

    // Slack integrations
    public DbSet<SlackWorkspace> SlackWorkspaces { get; set; }
    public DbSet<SlackChannel> SlackChannels { get; set; }
    public DbSet<SlackUser> SlackUsers { get; set; }
    public DbSet<SlackMessage> SlackMessages { get; set; }

    // Google Workspace integrations
    public DbSet<GoogleWorkspaceDomain> GoogleWorkspaceDomains { get; set; }
    public DbSet<GoogleWorkspaceUser> GoogleWorkspaceUsers { get; set; }
    public DbSet<GoogleWorkspaceGroup> GoogleWorkspaceGroups { get; set; }

    // Analytics and reporting
    public DbSet<Dashboard> Dashboards { get; set; }
    public DbSet<DashboardWidget> DashboardWidgets { get; set; }
    public DbSet<AnalyticsMetric> AnalyticsMetrics { get; set; }
    public DbSet<MetricValue> MetricValues { get; set; }
    public DbSet<BurndownChart> BurndownCharts { get; set; }
    public DbSet<AgileMetrics> AgileMetrics { get; set; }
    public DbSet<ResourceUtilization> ResourceUtilizations { get; set; }
    public DbSet<ROITracking> ROITrackings { get; set; }
    public DbSet<PredictiveAnalytics> PredictiveAnalytics { get; set; }
    public DbSet<Prediction> Predictions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Role).IsRequired();
        });

        // Project configuration
        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.Priority).IsRequired();
            entity.Property(e => e.StartDate).IsRequired();

            entity.HasOne(e => e.CreatedBy)
                .WithMany(e => e.CreatedProjects)
                .HasForeignKey(e => e.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Task configuration
        modelBuilder.Entity<ProjectTask>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.Priority).IsRequired();

            entity.HasOne(e => e.Project)
                .WithMany(e => e.Tasks)
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.AssignedTo)
                .WithMany(e => e.AssignedTasks)
                .HasForeignKey(e => e.AssignedToId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.CreatedBy)
                .WithMany(e => e.CreatedTasks)
                .HasForeignKey(e => e.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Comment configuration
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Content).IsRequired().HasMaxLength(1000);

            entity.HasOne(e => e.Task)
                .WithMany(e => e.Comments)
                .HasForeignKey(e => e.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany(e => e.Comments)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Resource configuration
        modelBuilder.Entity<Resource>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Type).IsRequired();
            entity.Property(e => e.Location).IsRequired().HasMaxLength(100);
            entity.Property(e => e.IsAvailable).IsRequired();
        });

        // Booking configuration
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.StartTime).IsRequired();
            entity.Property(e => e.EndTime).IsRequired();
            entity.Property(e => e.Purpose).IsRequired().HasMaxLength(200);

            entity.HasOne(e => e.Resource)
                .WithMany(e => e.Bookings)
                .HasForeignKey(e => e.ResourceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany(e => e.Bookings)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Many-to-many relationship between Project and User
        modelBuilder.Entity<Project>()
            .HasMany(p => p.AssignedUsers)
            .WithMany(u => u.AssignedProjects)
            .UsingEntity(j => j.ToTable("ProjectUsers"));

        // Third-party integration configuration
        modelBuilder.Entity<ThirdPartyIntegration>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Provider).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ClientId).IsRequired();
            entity.Property(e => e.ClientSecret).IsRequired();
            entity.Property(e => e.SyncSettings).HasColumnType("nvarchar(max)");

            entity.HasOne(e => e.Organization)
                .WithMany()
                .HasForeignKey(e => e.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.CreatedBy)
                .WithMany(e => e.CreatedIntegrations)
                .HasForeignKey(e => e.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Integration event configuration
        modelBuilder.Entity<IntegrationEvent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EventType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Payload).IsRequired().HasColumnType("nvarchar(max)");
            entity.Property(e => e.Status).HasMaxLength(20);

            entity.HasOne(e => e.Integration)
                .WithMany(e => e.Events)
                .HasForeignKey(e => e.IntegrationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Integration sync configuration
        modelBuilder.Entity<IntegrationSync>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SyncType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.EntityType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(20);

            entity.HasOne(e => e.Integration)
                .WithMany(e => e.Syncs)
                .HasForeignKey(e => e.IntegrationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // GitHub repository configuration
        modelBuilder.Entity<GitHubRepository>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ExternalId).IsRequired();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Language).HasMaxLength(50);

            entity.HasOne(e => e.Integration)
                .WithMany()
                .HasForeignKey(e => e.IntegrationId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.IntegrationId, e.ExternalId }).IsUnique();
        });

        // GitHub issue configuration
        modelBuilder.Entity<GitHubIssue>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ExternalId).IsRequired();
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasColumnType("nvarchar(max)");
            entity.Property(e => e.State).HasMaxLength(20);
            entity.Property(e => e.Labels).HasColumnType("nvarchar(max)");

            entity.HasOne(e => e.Repository)
                .WithMany(e => e.Issues)
                .HasForeignKey(e => e.RepositoryId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.RepositoryId, e.ExternalId }).IsUnique();
        });

        // GitHub pull request configuration
        modelBuilder.Entity<GitHubPullRequest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ExternalId).IsRequired();
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasColumnType("nvarchar(max)");
            entity.Property(e => e.State).HasMaxLength(20);
            entity.Property(e => e.Reviewers).HasColumnType("nvarchar(max)");
            entity.Property(e => e.Labels).HasColumnType("nvarchar(max)");

            entity.HasOne(e => e.Repository)
                .WithMany(e => e.PullRequests)
                .HasForeignKey(e => e.RepositoryId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.RepositoryId, e.ExternalId }).IsUnique();
        });

        // Slack workspace configuration
        modelBuilder.Entity<SlackWorkspace>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ExternalId).IsRequired();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Domain).HasMaxLength(100);

            entity.HasOne(e => e.Integration)
                .WithMany()
                .HasForeignKey(e => e.IntegrationId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.IntegrationId, e.ExternalId }).IsUnique();
        });

        // Slack channel configuration
        modelBuilder.Entity<SlackChannel>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ExternalId).IsRequired();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Topic).HasMaxLength(500);
            entity.Property(e => e.Purpose).HasMaxLength(500);

            entity.HasOne(e => e.Workspace)
                .WithMany(e => e.Channels)
                .HasForeignKey(e => e.WorkspaceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.WorkspaceId, e.ExternalId }).IsUnique();
        });

        // Slack user configuration
        modelBuilder.Entity<SlackUser>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ExternalId).IsRequired();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.RealName).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(e => e.Workspace)
                .WithMany(e => e.Users)
                .HasForeignKey(e => e.WorkspaceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.WorkspaceId, e.ExternalId }).IsUnique();
        });

        // Slack message configuration
        modelBuilder.Entity<SlackMessage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ExternalId).IsRequired();
            entity.Property(e => e.Text).IsRequired().HasColumnType("nvarchar(max)");
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.Property(e => e.Subtype).HasMaxLength(50);
            entity.Property(e => e.Attachments).HasColumnType("nvarchar(max)");
            entity.Property(e => e.Reactions).HasColumnType("nvarchar(max)");

            entity.HasOne(e => e.Channel)
                .WithMany(e => e.Messages)
                .HasForeignKey(e => e.ChannelId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.ParentMessage)
                .WithMany(e => e.Replies)
                .HasForeignKey(e => e.ParentMessageId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => new { e.ChannelId, e.ExternalId }).IsUnique();
        });

        // Google Workspace domain configuration
        modelBuilder.Entity<GoogleWorkspaceDomain>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Domain).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(e => e.Integration)
                .WithMany()
                .HasForeignKey(e => e.IntegrationId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.IntegrationId, e.Domain }).IsUnique();
        });

        // Google Workspace user configuration
        modelBuilder.Entity<GoogleWorkspaceUser>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ExternalId).IsRequired();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.DisplayName).HasMaxLength(100);
            entity.Property(e => e.Department).HasMaxLength(100);
            entity.Property(e => e.JobTitle).HasMaxLength(100);
            entity.Property(e => e.Location).HasMaxLength(100);

            entity.HasOne(e => e.Domain)
                .WithMany(e => e.Users)
                .HasForeignKey(e => e.DomainId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.DomainId, e.ExternalId }).IsUnique();
        });

        // Google Workspace group configuration
        modelBuilder.Entity<GoogleWorkspaceGroup>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ExternalId).IsRequired();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Type).HasMaxLength(50);

            entity.HasOne(e => e.Domain)
                .WithMany(e => e.Groups)
                .HasForeignKey(e => e.DomainId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.DomainId, e.ExternalId }).IsUnique();
        });

        // Many-to-many relationship between Google Workspace users and groups
        modelBuilder.Entity<GoogleWorkspaceUser>()
            .HasMany(u => u.Groups)
            .WithMany(g => g.Users)
            .UsingEntity(j => j.ToTable("GoogleWorkspaceUserGroups"));

        // Dashboard configuration
        modelBuilder.Entity<Dashboard>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Layout).IsRequired().HasColumnType("nvarchar(max)");

            entity.HasOne(e => e.Organization)
                .WithMany()
                .HasForeignKey(e => e.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.CreatedBy)
                .WithMany(e => e.CreatedDashboards)
                .HasForeignKey(e => e.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Dashboard widget configuration
        modelBuilder.Entity<DashboardWidget>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Type).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Configuration).IsRequired().HasColumnType("nvarchar(max)");

            entity.HasOne(e => e.Dashboard)
                .WithMany(e => e.Widgets)
                .HasForeignKey(e => e.DashboardId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Analytics metric configuration
        modelBuilder.Entity<AnalyticsMetric>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Category).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Query).IsRequired().HasColumnType("nvarchar(max)");
            entity.Property(e => e.Parameters).IsRequired().HasColumnType("nvarchar(max)");

            entity.HasOne(e => e.Organization)
                .WithMany()
                .HasForeignKey(e => e.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Metric value configuration
        modelBuilder.Entity<MetricValue>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Value).HasPrecision(18, 4);
            entity.Property(e => e.Context).HasColumnType("nvarchar(max)");

            entity.HasOne(e => e.Metric)
                .WithMany(e => e.Values)
                .HasForeignKey(e => e.MetricId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Burndown chart configuration
        modelBuilder.Entity<BurndownChart>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.SprintData).IsRequired().HasColumnType("nvarchar(max)");

            entity.HasOne(e => e.Project)
                .WithMany()
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Agile metrics configuration
        modelBuilder.Entity<AgileMetrics>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SprintId).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Velocity).HasPrecision(18, 4);
            entity.Property(e => e.BurndownRate).HasPrecision(18, 4);

            entity.HasOne(e => e.Project)
                .WithMany()
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Resource utilization configuration
        modelBuilder.Entity<ResourceUtilization>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.HoursWorked).HasPrecision(18, 2);
            entity.Property(e => e.HoursAllocated).HasPrecision(18, 2);
            entity.Property(e => e.UtilizationRate).HasPrecision(18, 4);
            entity.Property(e => e.Notes).HasColumnType("nvarchar(max)");

            entity.HasOne(e => e.User)
                .WithMany(e => e.ResourceUtilizations)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Organization)
                .WithMany()
                .HasForeignKey(e => e.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => new { e.UserId, e.Date }).IsUnique();
        });

        // ROI tracking configuration
        modelBuilder.Entity<ROITracking>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProjectName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Investment).HasPrecision(18, 2);
            entity.Property(e => e.Return).HasPrecision(18, 2);
            entity.Property(e => e.ROI).HasPrecision(18, 4);
            entity.Property(e => e.LaborCost).HasPrecision(18, 2);
            entity.Property(e => e.InfrastructureCost).HasPrecision(18, 2);
            entity.Property(e => e.OtherCosts).HasPrecision(18, 2);
            entity.Property(e => e.Notes).HasColumnType("nvarchar(max)");

            entity.HasOne(e => e.Project)
                .WithMany()
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Organization)
                .WithMany()
                .HasForeignKey(e => e.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Predictive analytics configuration
        modelBuilder.Entity<PredictiveAnalytics>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ModelName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Type).IsRequired().HasMaxLength(50);
            entity.Property(e => e.TrainingData).IsRequired().HasColumnType("nvarchar(max)");
            entity.Property(e => e.ModelParameters).IsRequired().HasColumnType("nvarchar(max)");
            entity.Property(e => e.Accuracy).HasPrecision(18, 4);

            entity.HasOne(e => e.Organization)
                .WithMany()
                .HasForeignKey(e => e.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Prediction configuration
        modelBuilder.Entity<Prediction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.InputData).IsRequired().HasColumnType("nvarchar(max)");
            entity.Property(e => e.PredictionResult).IsRequired().HasColumnType("nvarchar(max)");
            entity.Property(e => e.Confidence).HasPrecision(18, 4);

            entity.HasOne(e => e.Model)
                .WithMany(e => e.Predictions)
                .HasForeignKey(e => e.ModelId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
