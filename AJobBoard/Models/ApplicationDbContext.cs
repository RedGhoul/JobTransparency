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
            //.HasForeignKey(x => x.ApplierId)
            .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<JobPosting>()
            .HasOne(x => x.Sentiment)
            .WithOne(x => x.JobPosting)
            .HasForeignKey<Sentiment>(x => x.JobPostingId);
        }


    }


}
