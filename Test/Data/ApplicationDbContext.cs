using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Test.Models;

namespace Test.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Incident> Incidents => Set<Incident>();
        public DbSet<Problem> Problems => Set<Problem>();
        public DbSet<ChangeRequest> ChangeRequests => Set<ChangeRequest>();
        public DbSet<ServiceRequest> ServiceRequests => Set<ServiceRequest>();
        public DbSet<Asset> Assets => Set<Asset>();
        public DbSet<KnowledgeArticle> KnowledgeArticles => Set<KnowledgeArticle>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<Attachment> Attachments => Set<Attachment>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Incident>(e =>
            {
                e.HasIndex(i => i.TicketNumber).IsUnique();
                e.HasMany(i => i.Comments).WithOne().HasForeignKey(c => c.IncidentId);
                e.HasMany(i => i.Attachments).WithOne().HasForeignKey(a => a.IncidentId);
            });

            builder.Entity<Problem>(e =>
            {
                e.HasIndex(p => p.TicketNumber).IsUnique();
                e.HasMany(p => p.Comments).WithOne().HasForeignKey(c => c.ProblemId);
                e.HasMany(p => p.RelatedIncidents).WithOne(i => i.LinkedProblem).HasForeignKey(i => i.LinkedProblemId);
            });

            builder.Entity<ChangeRequest>(e =>
            {
                e.HasIndex(c => c.TicketNumber).IsUnique();
                e.HasMany(c => c.Comments).WithOne().HasForeignKey(cm => cm.ChangeRequestId);
            });

            builder.Entity<ServiceRequest>(e =>
            {
                e.HasIndex(s => s.TicketNumber).IsUnique();
                e.HasMany(s => s.Comments).WithOne().HasForeignKey(c => c.ServiceRequestId);
            });

            builder.Entity<Asset>(e =>
            {
                e.HasIndex(a => a.AssetTag).IsUnique();
                e.Property(a => a.PurchaseCost).HasColumnType("decimal(18,2)");
            });

            builder.Entity<KnowledgeArticle>(e =>
            {
                e.HasIndex(k => k.ArticleNumber).IsUnique();
            });
        }
    }
}
