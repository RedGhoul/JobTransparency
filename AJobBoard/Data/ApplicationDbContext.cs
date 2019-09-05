using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AJobBoard.Models;
using Microsoft.AspNetCore.Identity;
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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //List<JobData> liss = new List<JobData>();
            //using (StreamReader sr = new StreamReader(@"C:\Users\Avane\source\repos\AJobBoard\AJobBoard\Ontarioreactdeveloper30JobPostingIndeed04092019183835.csv"))
            //{
            //    String line;
            //    while ((line = sr.ReadLine()) != null)
            //    {
                    

            //        if (line.Contains("Synopsis")){
            //            continue;
            //        }
            //        //string[] list = line.Split(",,");
            //        //int Index = Int32.Parse(line.Split(",")[0]);
            //        //string Title = line.Split(",")[1];
            //        //string JobURL = line.Split(",")[2];

            //        //liss.Add(new JobData
            //        //{
            //        //    Index = Index,
            //        //    Title = list[1],
            //        //    JobURL = list[2],
            //        //    PostDate = list[3]
            //        //});
            //    }
            //}
            // To Read Use:
            //var result = engine.ReadFile(@"C:\Users\Avane\source\repos\AJobBoard\AJobBoard\Ontarioreactdeveloper30JobPostingIndeed04092019183835.csv");
            //builder.Entity<JobPosting>().HasData(new JobPosting
            //{

            //});
        }
    }

    public class JobData
    {
        public int Index { get; set; }
        public string Title { get; set; }
        public string JobURL { get; set; }
        public string PostDate { get; set; } 
        public string Location { get; set; }
        public string Company { get; set; }
        public string Salary { get; set; }
        public string Synopsis { get; set; }
    }
}
