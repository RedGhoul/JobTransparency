using AJobBoard.Models.Entity;
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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

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
            .HasForeignKey(x => x.ApplierId)
            .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<JobPosting>().Property(n => n.Id);

            builder.Entity<JobPosting>().HasIndex(n => n.URL);
            builder.Entity<JobPosting>().HasIndex(n => n.Title);

            builder.Entity<KeyPhrase>().HasIndex(n => n.Affinty);
        }


    }


}
