using AJobBoard.Models.Entity;
using Jobtransparency.Models.Entity;
using Jobtransparency.Models.Entity.JobGetter;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace AJobBoard.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            
        }

        public DbSet<JobPosting> JobPostings { get; set; }
        public DbSet<Apply> Applies { get; set; }
        public DbSet<KeyPhrase> KeyPhrase { get; set; }
        public DbSet<Document> Document { get; set; }
        public DbSet<PositionName> PositionName { get; set; }
        public DbSet<JobGettingConfig> JobGettingConfig { get; set; }
        public DbSet<PositionCities> PositionCities { get; set; }
        public DbSet<Sentiment> Sentiment { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<JobPostingTag> JobPostingTags { get; set; }
        public DbSet<HangfireConfig> HangfireConfigs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<PositionCities>().HasOne(x => x.JobGettingConfig).WithMany(x => x.PositionCities)
                .HasForeignKey(x => x.JobGettingConfigId).OnDelete(DeleteBehavior.Cascade);

            builder.Entity<PositionName>().HasOne(x => x.JobGettingConfig).WithMany(x => x.PositionName)
                .HasForeignKey(x => x.JobGettingConfigId).OnDelete(DeleteBehavior.Cascade);

            builder.Entity<JobPosting>()
            .HasMany(c => c.KeyPhrases)
            .WithOne(e => e.JobPosting)
            .HasForeignKey(x => x.JobPostingId)
            .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<JobPosting>()
            .HasMany(c => c.Applies)
            .WithOne(e => e.JobPosting)
            .HasForeignKey(x => x.JobPostingId)
            .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<ApplicationUser>()
            .HasMany(c => c.CreatedJobPostings)
            .WithOne(e => e.Poster)
            .HasForeignKey(x => x.PosterId)
            .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<ApplicationUser>()
            .HasMany(c => c.Documents)
            .WithOne(e => e.Owner)
            .HasForeignKey(x => x.OwnerId)
            .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<ApplicationUser>()
            .HasMany(c => c.Applies)
            .WithOne(e => e.Applier)
            .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<JobPosting>()
            .HasOne(x => x.Sentiment)
            .WithOne(x => x.JobPosting)
            .HasForeignKey<Sentiment>(x => x.JobPostingId);

            builder.Entity<JobPosting>()
           .HasMany(p => p.Tags)
           .WithMany(p => p.JobPostings)
           .UsingEntity<JobPostingTag>(
               j => j
                   .HasOne(pt => pt.Tag)
                   .WithMany(t => t.JobPostingTags)
                   .HasForeignKey(pt => pt.TagId),
               j => j
                   .HasOne(pt => pt.JobPosting)
                   .WithMany(p => p.JobPostingTags)
                   .HasForeignKey(pt => pt.JobPostingId),
               j =>
               {
                   j.HasKey(t => new { t.JobPostingId, t.TagId });
               });

            builder.Entity<JobPosting>()
                .HasGeneratedTsVectorColumn(
                    p => p.SearchVector,
                    "english", 
                    p => new { p.Title, p.Company, p.Description, p.Location })
                .HasIndex(p => p.SearchVector)
                .HasMethod("GIN");

            builder.Entity<Sentiment>().HasIndex(x => x.neg);
            builder.Entity<Sentiment>().HasIndex(x => x.compound);
            builder.Entity<Sentiment>().HasIndex(x => x.pos);
            builder.Entity<JobPosting>().HasIndex(x => x.DateAdded);
            builder.Entity<KeyPhrase>().HasIndex(x => x.Affinty);
            builder.Entity<JobPosting>().HasIndex(x => x.Slug);
            builder.Entity<JobPosting>().HasIndex(x => new { x.Title, x.Company, x.Location, x.DateAdded}).IsUnique(true);

        }


    }


}
