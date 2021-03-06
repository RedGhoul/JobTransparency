using System.Collections.Generic;
using Xunit;

namespace AJobBoard.Tests.Controller.Tests
{
    public class HomeControllerTests
    {
        //[Fact]
        //public async Task Index_ReturnsAViewResult_WithAListOfJobPostings()
        //{
        //    // Arrange
        //    var mockRepo = new Mock<IJobPostingRepository>();
        //    mockRepo.Setup(repo => repo.GetRandomSet())
        //        .ReturnsAsync(GetRandomSetOfJobPostings());
        //    mockRepo.Setup(repo => repo.GetTotal())
        //       .ReturnsAsync(GetTotalJobs());
        //    var mockLogger = new Mock<ILogger<HomeController>>();
        //    var controller = new HomeController(mockRepo.Object, mockLogger.Object);

        //    // Act
        //    var result = await controller.Index();

        //    // Assert
        //    var viewResult = Assert.IsType<ViewResult>(result);
        //    var model = Assert.IsAssignableFrom<HomeIndexViewModel>(
        //        viewResult.ViewData.Model);
        //    Assert.Equal(10, model.JobPostings.Count());
        //}

        [Fact]
        public void RegisterType_ReturnsAViewResult_WithTheRightPage()
        {
            //// Arrange
            //var mockRepo = new Mock<IJobPostingRepository>();
            //var mockLogger = new Mock<ILogger<HomeController>>();
            //var controller = new HomeController(mockRepo.Object, mockLogger.Object);

            //// Act
            //var result = controller.RegisterType();

            //// Assert
            //var viewResult = Assert.IsType<ViewResult>(result);

            //Assert.Null(viewResult.ViewName);
        }

        private List<JobPostingDTO> GetRandomSetOfJobPostings()
        {
            List<JobPostingDTO> jobs = new List<JobPostingDTO>();

            for (int i = 0; i < 10; i++)
            {
                jobs.Add(new JobPostingDTO
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
