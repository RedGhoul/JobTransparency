using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AJobBoard.Tests.Controller.Tests
{
    public class DocumentsControllerTest
    {
        [Fact]
        public async Task Index_ReturnViewResult_With_Documents()
        {
            //// Arrange
            //var mockRepo = new Mock<IJobPostingRepository>();
            //mockRepo.Setup(repo => repo.GetRandomSetOfJobPostings())
            //    .ReturnsAsync(GetRandomSetOfJobPostings());
            //mockRepo.Setup(repo => repo.GetTotalJobs())
            //   .ReturnsAsync(GetTotalJobs());
            //var controller = new HomeController(mockRepo.Object);

            //// Act
            //var result = await controller.Index();

            //// Assert
            //var viewResult = Assert.IsType<ViewResult>(result);
            //var model = Assert.IsAssignableFrom<HomeIndexViewModel>(
            //    viewResult.ViewData.Model);
            //Assert.Equal(10, model.jobPostings.Count());
        }

    }
}
