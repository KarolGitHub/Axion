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

    // Performance & Caching
    public DbSet<CacheEntry> CacheEntries { get; set; }
    public DbSet<PerformanceMetric> PerformanceMetrics { get; set; }
    public DbSet<DatabaseOptimization> DatabaseOptimizations { get; set; }
    public DbSet<ApiPerformanceLog> ApiPerformanceLogs { get; set; }
            public DbSet<MemoryUsage> MemoryUsages { get; set; }
        public DbSet<CpuUsage> CpuUsages { get; set; }

        // Load Balancing & Auto-scaling
        public DbSet<LoadBalancer> LoadBalancers { get; set; }
        public DbSet<LoadBalancerInstance> LoadBalancerInstances { get; set; }
        public DbSet<LoadBalancerRule> LoadBalancerRules { get; set; }
        public DbSet<ScalingPolicy> ScalingPolicies { get; set; }
        public DbSet<ScalingEvent> ScalingEvents { get; set; }
        public DbSet<AutoScalingGroup> AutoScalingGroups { get; set; }
        public DbSet<AutoScalingInstance> AutoScalingInstances { get; set; }
        public DbSet<LoadBalancerMetrics> LoadBalancerMetrics { get; set; }
        public DbSet<ScalingMetrics> ScalingMetrics { get; set; }

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

        // Performance & Caching configurations
        modelBuilder.Entity<CacheEntry>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Key).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Value).IsRequired().HasColumnType("nvarchar(max)");
            entity.Property(e => e.CacheType).HasMaxLength(50);
            entity.HasIndex(e => new { e.Key, e.OrganizationId }).IsUnique();
            entity.HasIndex(e => e.ExpiresAt);
            entity.HasOne(e => e.Organization)
                .WithMany()
                .HasForeignKey(e => e.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PerformanceMetric>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MetricName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Category).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Value).HasPrecision(18, 4);
            entity.Property(e => e.Unit).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Context).HasMaxLength(500);
            entity.Property(e => e.Tags).HasColumnType("nvarchar(max)");
            entity.HasIndex(e => new { e.MetricName, e.Category, e.RecordedAt });
            entity.HasOne(e => e.Organization)
                .WithMany()
                .HasForeignKey(e => e.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<DatabaseOptimization>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TableName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.OptimizationType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Query).IsRequired().HasColumnType("nvarchar(max)");
            entity.Property(e => e.ExecutionTime).HasPrecision(18, 4);
            entity.Property(e => e.ImprovementPercentage).HasPrecision(18, 2);
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.HasIndex(e => new { e.TableName, e.OptimizationType, e.ExecutedAt });
            entity.HasOne(e => e.Organization)
                .WithMany()
                .HasForeignKey(e => e.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ApiPerformanceLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.HttpMethod).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Endpoint).IsRequired().HasMaxLength(500);
            entity.Property(e => e.ResponseTime).HasPrecision(18, 4);
            entity.Property(e => e.UserAgent).HasMaxLength(50);
            entity.Property(e => e.IpAddress).HasMaxLength(50);
            entity.Property(e => e.RequestBody).HasColumnType("nvarchar(max)");
            entity.Property(e => e.ResponseBody).HasColumnType("nvarchar(max)");
            entity.Property(e => e.RequestSize).HasPrecision(18, 2);
            entity.Property(e => e.ResponseSize).HasPrecision(18, 2);
            entity.HasIndex(e => new { e.Endpoint, e.RequestedAt });
            entity.HasIndex(e => e.ResponseTime);
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.Organization)
                .WithMany()
                .HasForeignKey(e => e.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<MemoryUsage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TotalMemory).HasPrecision(18, 2);
            entity.Property(e => e.UsedMemory).HasPrecision(18, 2);
            entity.Property(e => e.AvailableMemory).HasPrecision(18, 2);
            entity.Property(e => e.MemoryUsagePercentage).HasPrecision(18, 2);
            entity.Property(e => e.ServerInstance).HasMaxLength(50);
            entity.HasIndex(e => new { e.OrganizationId, e.RecordedAt });
            entity.HasOne(e => e.Organization)
                .WithMany()
                .HasForeignKey(e => e.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<CpuUsage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CpuUsagePercentage).HasPrecision(18, 2);
            entity.Property(e => e.LoadAverage).HasPrecision(18, 2);
            entity.Property(e => e.ServerInstance).HasMaxLength(50);
            entity.HasIndex(e => new { e.OrganizationId, e.RecordedAt });
            entity.HasOne(e => e.Organization)
                .WithMany()
                .HasForeignKey(e => e.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Load Balancing & Auto-scaling configurations
        modelBuilder.Entity<LoadBalancer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Type).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Algorithm).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Configuration).HasColumnType("nvarchar(max)");
            entity.HasIndex(e => new { e.Name, e.OrganizationId }).IsUnique();
            entity.HasOne(e => e.Organization)
                .WithMany()
                .HasForeignKey(e => e.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<LoadBalancerInstance>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.InstanceId).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.IpAddress).IsRequired().HasMaxLength(15);
            entity.Property(e => e.ResponseTime).HasPrecision(10, 2);
            entity.HasIndex(e => new { e.InstanceId, e.LoadBalancerId }).IsUnique();
            entity.HasOne(e => e.LoadBalancer)
                .WithMany(e => e.Instances)
                .HasForeignKey(e => e.LoadBalancerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<LoadBalancerRule>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Type).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Condition).IsRequired().HasColumnType("nvarchar(max)");
            entity.Property(e => e.Action).IsRequired().HasColumnType("nvarchar(max)");
            entity.HasIndex(e => new { e.Name, e.LoadBalancerId }).IsUnique();
            entity.HasOne(e => e.LoadBalancer)
                .WithMany(e => e.Rules)
                .HasForeignKey(e => e.LoadBalancerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ScalingPolicy>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Type).IsRequired().HasMaxLength(50);
            entity.Property(e => e.MetricName).IsRequired();
            entity.Property(e => e.Threshold).HasPrecision(10, 2);
            entity.Property(e => e.Conditions).HasColumnType("nvarchar(max)");
            entity.HasIndex(e => new { e.Name, e.OrganizationId }).IsUnique();
            entity.HasOne(e => e.Organization)
                .WithMany()
                .HasForeignKey(e => e.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ScalingEvent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Action).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Reason).IsRequired();
            entity.Property(e => e.MetricValue).HasPrecision(10, 2);
            entity.Property(e => e.Threshold).HasPrecision(10, 2);
            entity.Property(e => e.ErrorMessage).HasColumnType("nvarchar(max)");
            entity.HasOne(e => e.ScalingPolicy)
                .WithMany(e => e.ScalingEvents)
                .HasForeignKey(e => e.ScalingPolicyId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AutoScalingGroup>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.LaunchTemplate).HasColumnType("nvarchar(max)");
            entity.Property(e => e.VpcConfig).HasColumnType("nvarchar(max)");
            entity.Property(e => e.Tags).HasColumnType("nvarchar(max)");
            entity.HasIndex(e => new { e.Name, e.OrganizationId }).IsUnique();
            entity.HasOne(e => e.Organization)
                .WithMany()
                .HasForeignKey(e => e.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<AutoScalingInstance>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.InstanceId).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.IpAddress).IsRequired().HasMaxLength(15);
            entity.Property(e => e.PrivateIpAddress).HasMaxLength(15);
            entity.Property(e => e.InstanceType).HasMaxLength(50);
            entity.Property(e => e.AvailabilityZone).HasMaxLength(50);
            entity.Property(e => e.TerminationReason).HasColumnType("nvarchar(max)");
            entity.Property(e => e.CpuUtilization).HasPrecision(5, 2);
            entity.Property(e => e.MemoryUtilization).HasPrecision(5, 2);
            entity.HasIndex(e => new { e.InstanceId, e.AutoScalingGroupId }).IsUnique();
            entity.HasOne(e => e.AutoScalingGroup)
                .WithMany(e => e.Instances)
                .HasForeignKey(e => e.AutoScalingGroupId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<LoadBalancerMetrics>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RequestCount).HasPrecision(10, 2);
            entity.Property(e => e.TargetResponseTime).HasPrecision(10, 2);
            entity.Property(e => e.HealthyHostCount).HasPrecision(5, 2);
            entity.Property(e => e.UnhealthyHostCount).HasPrecision(5, 2);
            entity.Property(e => e.TargetConnectionErrorCount).HasPrecision(10, 2);
            entity.Property(e => e.TargetTLSNegotiationErrorCount).HasPrecision(10, 2);
            entity.Property(e => e.RequestCountPerTarget).HasPrecision(10, 2);
            entity.Property(e => e.UnHealthyHostCount).HasPrecision(5, 2);
            entity.HasIndex(e => new { e.LoadBalancerId, e.RecordedAt });
            entity.HasOne(e => e.LoadBalancer)
                .WithMany()
                .HasForeignKey(e => e.LoadBalancerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ScalingMetrics>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CpuUtilization).HasPrecision(5, 2);
            entity.Property(e => e.MemoryUtilization).HasPrecision(5, 2);
            entity.Property(e => e.NetworkIn).HasPrecision(10, 2);
            entity.Property(e => e.NetworkOut).HasPrecision(10, 2);
            entity.Property(e => e.DiskReadOps).HasPrecision(10, 2);
            entity.Property(e => e.DiskWriteOps).HasPrecision(10, 2);
            entity.HasIndex(e => new { e.AutoScalingGroupId, e.RecordedAt });
            entity.HasOne(e => e.AutoScalingGroup)
                .WithMany()
                .HasForeignKey(e => e.AutoScalingGroupId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
