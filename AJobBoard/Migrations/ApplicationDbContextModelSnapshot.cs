﻿// <auto-generated />
using System;
using AJobBoard.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using NpgsqlTypes;

namespace Jobtransparency.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.6")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("AJobBoard.Models.Entity.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("integer");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("FirstName")
                        .HasColumnType("text");

                    b.Property<bool>("IsJobSeeker")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsRecruiter")
                        .HasColumnType("boolean");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("text");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("boolean");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("AJobBoard.Models.Entity.Apply", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ApplierId")
                        .HasColumnType("text");

                    b.Property<DateTime>("DateAddedToApplies")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("JobPostingId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ApplierId");

                    b.HasIndex("JobPostingId");

                    b.ToTable("Applies");
                });

            modelBuilder.Entity("AJobBoard.Models.Entity.Document", b =>
                {
                    b.Property<int?>("DocumentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("DocumentName")
                        .HasColumnType("text");

                    b.Property<bool>("IsOtherDoc")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsResume")
                        .HasColumnType("boolean");

                    b.Property<string>("OwnerId")
                        .HasColumnType("text");

                    b.Property<string>("URL")
                        .HasColumnType("text");

                    b.HasKey("DocumentId");

                    b.HasIndex("OwnerId");

                    b.ToTable("Document");
                });

            modelBuilder.Entity("AJobBoard.Models.Entity.JobPosting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Company")
                        .HasColumnType("text");

                    b.Property<string>("CompanyLogoUrl")
                        .HasColumnType("text");

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<bool>("Expried")
                        .HasColumnType("boolean");

                    b.Property<string>("JobSource")
                        .HasColumnType("text");

                    b.Property<string>("Location")
                        .HasColumnType("text");

                    b.Property<int>("NumberOfApplies")
                        .HasColumnType("integer");

                    b.Property<int>("NumberOfViews")
                        .HasColumnType("integer");

                    b.Property<string>("PostDate")
                        .HasColumnType("text");

                    b.Property<string>("PosterId")
                        .HasColumnType("text");

                    b.Property<string>("Posters")
                        .HasColumnType("text");

                    b.Property<string>("Salary")
                        .HasColumnType("text");

                    b.Property<NpgsqlTsVector>("SearchVector")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("tsvector")
                        .HasAnnotation("Npgsql:TsVectorConfig", "english")
                        .HasAnnotation("Npgsql:TsVectorProperties", new[] { "Title", "Company", "Description", "Location" });

                    b.Property<string>("Slug")
                        .HasColumnType("text");

                    b.Property<string>("Summary")
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .HasColumnType("text");

                    b.Property<string>("URL")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("DateAdded");

                    b.HasIndex("PosterId");

                    b.HasIndex("SearchVector")
                        .HasMethod("GIN");

                    b.HasIndex("Slug");

                    b.HasIndex("Title", "Company", "Location", "DateAdded")
                        .IsUnique();

                    b.ToTable("JobPostings");
                });

            modelBuilder.Entity("AJobBoard.Models.Entity.KeyPhrase", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<float>("Affinty")
                        .HasColumnType("real");

                    b.Property<int?>("JobPostingId")
                        .HasColumnType("integer");

                    b.Property<string>("Text")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Affinty");

                    b.HasIndex("JobPostingId");

                    b.ToTable("KeyPhrase");
                });

            modelBuilder.Entity("Jobtransparency.Models.Entity.HangfireConfig", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("AffinityThreshold")
                        .HasColumnType("integer");

                    b.Property<int>("MinKeyPhraseLengthThreshold")
                        .HasColumnType("integer");

                    b.Property<int>("SQLCommandTimeOut")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("HangfireConfigs");
                });

            modelBuilder.Entity("Jobtransparency.Models.Entity.JobGetter.PositionCities", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("JobGettingConfigId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("JobGettingConfigId");

                    b.ToTable("PositionCities");
                });

            modelBuilder.Entity("Jobtransparency.Models.Entity.JobGettingConfig", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Host")
                        .HasColumnType("text");

                    b.Property<string>("LinkAzureFunction")
                        .HasColumnType("text");

                    b.Property<string>("LinkAzureFunction2")
                        .HasColumnType("text");

                    b.Property<string>("LinkCheckIfJobExists")
                        .HasColumnType("text");

                    b.Property<string>("LinkJobPostingCreation")
                        .HasColumnType("text");

                    b.Property<int>("MaxAge")
                        .HasColumnType("integer");

                    b.Property<string>("MaxNumber")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("JobGettingConfig");
                });

            modelBuilder.Entity("Jobtransparency.Models.Entity.JobPostingTag", b =>
                {
                    b.Property<int>("JobPostingId")
                        .HasColumnType("integer");

                    b.Property<int>("TagId")
                        .HasColumnType("integer");

                    b.Property<int>("Id")
                        .HasColumnType("integer");

                    b.HasKey("JobPostingId", "TagId");

                    b.HasIndex("TagId");

                    b.ToTable("JobPostingTags");
                });

            modelBuilder.Entity("Jobtransparency.Models.Entity.PositionName", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("JobGettingConfigId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("JobGettingConfigId");

                    b.ToTable("PositionName");
                });

            modelBuilder.Entity("Jobtransparency.Models.Entity.Sentiment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("JobPostingId")
                        .HasColumnType("integer");

                    b.Property<float>("compound")
                        .HasColumnType("real");

                    b.Property<float>("neg")
                        .HasColumnType("real");

                    b.Property<float>("neu")
                        .HasColumnType("real");

                    b.Property<float>("pos")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.HasIndex("JobPostingId")
                        .IsUnique();

                    b.HasIndex("compound");

                    b.HasIndex("neg");

                    b.HasIndex("pos");

                    b.ToTable("Sentiment");
                });

            modelBuilder.Entity("Jobtransparency.Models.Entity.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("CreateAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Text")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("text");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("RoleId")
                        .HasColumnType("text");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("AJobBoard.Models.Entity.Apply", b =>
                {
                    b.HasOne("AJobBoard.Models.Entity.ApplicationUser", "Applier")
                        .WithMany("Applies")
                        .HasForeignKey("ApplierId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("AJobBoard.Models.Entity.JobPosting", "JobPosting")
                        .WithMany("Applies")
                        .HasForeignKey("JobPostingId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Applier");

                    b.Navigation("JobPosting");
                });

            modelBuilder.Entity("AJobBoard.Models.Entity.Document", b =>
                {
                    b.HasOne("AJobBoard.Models.Entity.ApplicationUser", "Owner")
                        .WithMany("Documents")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("AJobBoard.Models.Entity.JobPosting", b =>
                {
                    b.HasOne("AJobBoard.Models.Entity.ApplicationUser", "Poster")
                        .WithMany("CreatedJobPostings")
                        .HasForeignKey("PosterId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Poster");
                });

            modelBuilder.Entity("AJobBoard.Models.Entity.KeyPhrase", b =>
                {
                    b.HasOne("AJobBoard.Models.Entity.JobPosting", "JobPosting")
                        .WithMany("KeyPhrases")
                        .HasForeignKey("JobPostingId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("JobPosting");
                });

            modelBuilder.Entity("Jobtransparency.Models.Entity.JobGetter.PositionCities", b =>
                {
                    b.HasOne("Jobtransparency.Models.Entity.JobGettingConfig", "JobGettingConfig")
                        .WithMany("PositionCities")
                        .HasForeignKey("JobGettingConfigId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("JobGettingConfig");
                });

            modelBuilder.Entity("Jobtransparency.Models.Entity.JobPostingTag", b =>
                {
                    b.HasOne("AJobBoard.Models.Entity.JobPosting", "JobPosting")
                        .WithMany("JobPostingTags")
                        .HasForeignKey("JobPostingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Jobtransparency.Models.Entity.Tag", "Tag")
                        .WithMany("JobPostingTags")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("JobPosting");

                    b.Navigation("Tag");
                });

            modelBuilder.Entity("Jobtransparency.Models.Entity.PositionName", b =>
                {
                    b.HasOne("Jobtransparency.Models.Entity.JobGettingConfig", "JobGettingConfig")
                        .WithMany("PositionName")
                        .HasForeignKey("JobGettingConfigId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("JobGettingConfig");
                });

            modelBuilder.Entity("Jobtransparency.Models.Entity.Sentiment", b =>
                {
                    b.HasOne("AJobBoard.Models.Entity.JobPosting", "JobPosting")
                        .WithOne("Sentiment")
                        .HasForeignKey("Jobtransparency.Models.Entity.Sentiment", "JobPostingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("JobPosting");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("AJobBoard.Models.Entity.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("AJobBoard.Models.Entity.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AJobBoard.Models.Entity.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("AJobBoard.Models.Entity.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AJobBoard.Models.Entity.ApplicationUser", b =>
                {
                    b.Navigation("Applies");

                    b.Navigation("CreatedJobPostings");

                    b.Navigation("Documents");
                });

            modelBuilder.Entity("AJobBoard.Models.Entity.JobPosting", b =>
                {
                    b.Navigation("Applies");

                    b.Navigation("JobPostingTags");

                    b.Navigation("KeyPhrases");

                    b.Navigation("Sentiment");
                });

            modelBuilder.Entity("Jobtransparency.Models.Entity.JobGettingConfig", b =>
                {
                    b.Navigation("PositionCities");

                    b.Navigation("PositionName");
                });

            modelBuilder.Entity("Jobtransparency.Models.Entity.Tag", b =>
                {
                    b.Navigation("JobPostingTags");
                });
#pragma warning restore 612, 618
        }
    }
}
