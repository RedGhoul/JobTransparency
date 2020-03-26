using AJobBoard.Models;
using AJobBoard.Models.Data;
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
        public DbSet<ETLStatus> ETLStatus { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<JobPosting>()
            .HasMany(c => c.KeyPhrases)
            .WithOne(e => e.JobPosting)
            .IsRequired();

            builder.Entity<JobPosting>()
            .HasMany(c => c.Applies)
            .WithOne(e => e.JobPosting);

            builder.Entity<ApplicationUser>()
            .HasMany(c => c.Documents)
            .WithOne(e => e.Owner);

            builder.Entity<ApplicationUser>()
            .HasMany(c => c.JobPostings)
            .WithOne(e => e.Poster);

            builder.Entity<ApplicationUser>()
            .HasMany(c => c.Applies)
            .WithOne(e => e.Applier);

        }

        
    }


}
