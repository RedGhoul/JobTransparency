using AJobBoard.Controllers.Views;
using AJobBoard.Data;
using AJobBoard.Models;
using AJobBoard.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AJobBoard.Tests.Controller.Tests
{
    public class JobPostingsControllerTests
    {
        [Fact]
        public async Task Index_ReturnsAViewResult_WithAListOfJobPostings_AndTimeSpan()
        {
            // Arrange
            HomeIndexViewModel homeIndexVm = new HomeIndexViewModel();
            var mockRepoJob = new Mock<IJobPostingRepository>();
            mockRepoJob.Setup(repo => repo.ConfigureSearchAsync(homeIndexVm))
                .ReturnsAsync(ConfigureSearchAsync(homeIndexVm));
            mockRepoJob.Setup(repo => repo.GetTotalJobs())
               .ReturnsAsync(GetTotalJobs());

            var mockRepoKeyPharse = new Mock<IKeyPharseRepository>();

            var mockNLTKService = new Mock<INLTKService>();

            var controller = new JobPostingsController(mockRepoJob.Object,
                mockNLTKService.Object, mockRepoKeyPharse.Object);

            // Act
            var result = await controller.Index(homeIndexVm);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom <IEnumerable<JobPosting>>(
                viewResult.ViewData.Model);
            Assert.Equal(10, model.Count());
            Assert.True(viewResult.ViewData.ContainsKey("SecsToQuery"));
        }

        private (List<JobPosting>, TimeSpan) ConfigureSearchAsync(HomeIndexViewModel homeIndexVm)
        {
            List<JobPosting> jobs = new List<JobPosting>();

            for (int i = 0; i < 10; i++)
            {
                jobs.Add(new JobPosting
                {
                    Title = "Car Mart Sales" + i
                });
            }

            return (jobs, new TimeSpan());
        }
        private string GetTotalJobs()
        {
            return "1000";
        }
    }
}
