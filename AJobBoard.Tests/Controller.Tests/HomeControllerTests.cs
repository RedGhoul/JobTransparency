using AJobBoard.Controllers.Views;
using AJobBoard.Data;
using AJobBoard.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AJobBoard.Tests.Controller.Tests
{
    public class HomeControllerTests
    {
        [Fact]
        public async Task Index_ReturnsAViewResult_WithAListOfJobPostings()
        {
            // Arrange
            var mockRepo = new Mock<IJobPostingRepository>();
            mockRepo.Setup(repo => repo.GetRandomSetOfJobPostings())
                .ReturnsAsync(GetRandomSetOfJobPostings());
            mockRepo.Setup(repo => repo.GetTotalJobs())
               .ReturnsAsync(GetTotalJobs());
            var controller = new HomeController(mockRepo.Object);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<HomeIndexViewModel>(
                viewResult.ViewData.Model);
            Assert.Equal(10, model.jobPostings.Count());
        }

        [Fact]
        public void RegisterType_ReturnsAViewResult_WithTheRightPage()
        {
            // Arrange
            var mockRepo = new Mock<IJobPostingRepository>();
            var controller = new HomeController(mockRepo.Object);

            // Act
            var result = controller.RegisterType();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.Null(viewResult.ViewName);
        }

        private List<JobPosting> GetRandomSetOfJobPostings()
        {
            List<JobPosting> jobs = new List<JobPosting>();

            for (int i = 0; i < 10; i++)
            {
                jobs.Add(new JobPosting
                {
                    Title = "Car Mart Sales" + i
                });
            }

            return jobs;
        }
        private string GetTotalJobs()
        {
            return "1000";
        }

    }
}