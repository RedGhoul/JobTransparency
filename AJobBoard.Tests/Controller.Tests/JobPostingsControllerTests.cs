using AJobBoard.Controllers.Views;
using AJobBoard.Data;
using AJobBoard.Models.View;
using AJobBoard.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace AJobBoard.Tests.Controller.Tests
{
    public class JobPostingsControllerTests
    {
        //[Fact]
        //public async Task Index_ReturnsAViewResult_WithAListOfJobPostings()
        //{
        //    // Arrange
        //    HomeIndexViewModel homeIndexVm = new HomeIndexViewModel();
        //    var mockRepoJob = new Mock<IJobPostingRepository>();
        //    mockRepoJob.Setup(repo => repo.ConfigureSearchAsync(homeIndexVm))
        //        .ReturnsAsync(ConfigureSearchAsync(homeIndexVm));
        //    mockRepoJob.Setup(repo => repo.GetTotalJobs())
        //       .ReturnsAsync(GetTotalJobs());

        //    var mockRepoKeyPharse = new Mock<IKeyPharseRepository>();

        //    var mockNLTKService = new Mock<INLTKService>();

        //    var controller = new JobPostingsController(mockRepoJob.Object,
        //        mockNLTKService.Object, mockRepoKeyPharse.Object); 

        //    //Act
        //   var result = await controller.Index();

        //    //Assert
        //   var viewResult = Assert.IsType<ViewResult>(result);
        //    var model = Assert.IsAssignableFrom<HomeIndexViewModel>(
        //        viewResult.Model);
        //    Assert.Equal(10, model.JobPostings.Count());
        //}


        [Fact]
        public async Task Details_ReturnsAValid_ViewResult_WhenInRangeId()
        {
            // Arrange
            int jobId = 1;
            JobPosting tempjob = new JobPosting
            {
                Id = jobId,
                Title = "Full Stack Developer",
                Company = "Walmart",
                NumberOfViews = 1
            };
            Mock<IJobPostingRepository> mockRepoJob = new Mock<IJobPostingRepository>();
            mockRepoJob.Setup(repo => repo.GetById(jobId))
                .ReturnsAsync(tempjob);

            mockRepoJob.Setup(repo => repo.AddView(tempjob))
               .ReturnsAsync(TickNumberOfViewAsync(tempjob));

            Mock<IKeyPharseRepository> mockRepoKeyPharse = new Mock<IKeyPharseRepository>();

            Mock<INLTKService> mockNLTKService = new Mock<INLTKService>();

            JobPostingsController controller = new JobPostingsController(mockRepoJob.Object,
                mockNLTKService.Object, mockRepoKeyPharse.Object);

            // Act
            IActionResult result = await controller.Details(jobId);

            // Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            JobPosting model = Assert.IsAssignableFrom<JobPosting>(
                viewResult.ViewData.Model);
            Assert.Equal(2, model.NumberOfViews);

        }

        [Fact]
        public async Task Details_Returns_NotFoundResult_WhenOutOfRangeId()
        {
            // Arrange
            int jobId = 1000;
            Mock<IJobPostingRepository> mockRepoJob = new Mock<IJobPostingRepository>();
            mockRepoJob.Setup(repo => repo.GetById(jobId))
                .ReturnsAsync(GetJobPostingById(jobId));

            Mock<IKeyPharseRepository> mockRepoKeyPharse = new Mock<IKeyPharseRepository>();

            Mock<INLTKService> mockNLTKService = new Mock<INLTKService>();

            JobPostingsController controller = new JobPostingsController(mockRepoJob.Object,
                mockNLTKService.Object, mockRepoKeyPharse.Object);

            // Act
            IActionResult result = await controller.Details(jobId);

            // Assert

            NotFoundResult viewResult = Assert.IsType<NotFoundResult>(result);

        }

        [Fact]
        public async Task Create_Returns_ValidViewResult_WhenModelStateInvalid()
        {
            // Arrange
            JobPosting tempjob = new JobPosting
            {
                Title = "Full Stack Developer",
                Company = "Walmart",
                Description = "Yolo Lyfe",
                NumberOfViews = 1
            };
            tempjob = CreateJobPostingAsync(tempjob);


            Mock<IJobPostingRepository> mockRepoJob = new Mock<IJobPostingRepository>();
            mockRepoJob.Setup(repo => repo.Create(tempjob))
                .ReturnsAsync(tempjob);

            mockRepoJob.Setup(repo => repo.Put(tempjob.Id, tempjob))
                .ReturnsAsync(tempjob);

            KeyPhrasesWrapperDTO temKeyPharse = GetNLTKKeyPhrases(tempjob.Description);
            tempjob.KeyPhrases = new List<KeyPhrase>();
            foreach (KeyPhraseDTO item in temKeyPharse.rank_list)
            {
                tempjob.KeyPhrases.Add(new KeyPhrase
                {
                    Affinty = item.Affinty,
                    Text = item.Text
                });
            }
            SummaryDTO tempValDTO = GetNLTKSummary(tempjob.Description);
            tempjob.Summary = tempValDTO.SummaryText;

            Mock<IKeyPharseRepository> mockRepoKeyPharse = new Mock<IKeyPharseRepository>();

            Mock<INLTKService> mockNLTKService = new Mock<INLTKService>();

            mockNLTKService.Setup(service => service.GetNLTKKeyPhrases(tempjob.Description))
                .ReturnsAsync(temKeyPharse);

            mockNLTKService.Setup(service => service.GetNLTKSummary(tempjob.Description))
                .ReturnsAsync(tempValDTO);

            JobPostingsController controller = new JobPostingsController(mockRepoJob.Object,
                mockNLTKService.Object, mockRepoKeyPharse.Object);

            controller.ModelState.AddModelError("Salary", "no Salary found");

            // Act
            IActionResult result = await controller.Create(tempjob);

            // Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            JobPosting model = Assert.IsAssignableFrom<JobPosting>(
                viewResult.ViewData.Model);

        }

        [Fact]
        public async Task Create_Returns_RedirectToActionResult_WhenModelStateValid()
        {
            // Arrange
            JobPosting tempjob = new JobPosting
            {
                Title = "Full Stack Developer",
                Company = "Walmart",
                Description = "Yolo Lyfe",
                NumberOfViews = 1
            };
            tempjob = CreateJobPostingAsync(tempjob);


            Mock<IJobPostingRepository> mockRepoJob = new Mock<IJobPostingRepository>();
            mockRepoJob.Setup(repo => repo.Create(tempjob))
                .ReturnsAsync(tempjob);

            mockRepoJob.Setup(repo => repo.Put(tempjob.Id, tempjob))
                .ReturnsAsync(tempjob);

            KeyPhrasesWrapperDTO temKeyPharse = GetNLTKKeyPhrases(tempjob.Description);
            tempjob.KeyPhrases = new List<KeyPhrase>();
            foreach (KeyPhraseDTO item in temKeyPharse.rank_list)
            {
                tempjob.KeyPhrases.Add(new KeyPhrase
                {
                    Affinty = item.Affinty,
                    Text = item.Text
                });
            }
            SummaryDTO tempValDTO = GetNLTKSummary(tempjob.Description);
            tempjob.Summary = tempValDTO.SummaryText;

            Mock<IKeyPharseRepository> mockRepoKeyPharse = new Mock<IKeyPharseRepository>();

            Mock<INLTKService> mockNLTKService = new Mock<INLTKService>();

            mockNLTKService.Setup(service => service.GetNLTKKeyPhrases(tempjob.Description))
                .ReturnsAsync(temKeyPharse);

            mockNLTKService.Setup(service => service.GetNLTKSummary(tempjob.Description))
                .ReturnsAsync(tempValDTO);

            JobPostingsController controller = new JobPostingsController(mockRepoJob.Object,
                mockNLTKService.Object, mockRepoKeyPharse.Object);


            // Act
            IActionResult result = await controller.Create(tempjob);

            // Assert
            RedirectToActionResult redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);

        }

        //[Fact]
        //public async Task Edit_Get_Returns_NotFoundViewResult_WhenOutOfRangeId()
        //{
        //    Assert.True(true);
        //}

        //[Fact]
        //public async Task Edit_Get_Returns_ValidViewResult_WhenInRangeId()
        //{
        //    Assert.True(true);
        //}

        //[Fact]
        //public async Task Edit_Post_Returns_NotFoundViewResult_WhenOutOfRangeId()
        //{
        //    Assert.True(true);
        //}

        //[Fact]
        //public async Task Edit_Post_Returns_ValidViewResult_WhenInvalidModel()
        //{
        //    Assert.True(true);
        //}

        //[Fact]
        //public async Task Delete_Get_Returns_ValidViewResult_WhenIdIsInRange()
        //{
        //    Assert.True(true);
        //}

        //[Fact]
        //public async Task Delete_Post_Returns_RedirectActionResult()
        //{
        //    Assert.True(true);
        //}

        private SummaryDTO GetNLTKSummary(string des)
        {
            return new SummaryDTO { SummaryText = "Yo" };
        }

        private KeyPhrasesWrapperDTO GetNLTKKeyPhrases(string des)
        {
            KeyPhrasesWrapperDTO temp = new KeyPhrasesWrapperDTO();
            List<KeyPhraseDTO> KeyPhrasesDTOs = new List<KeyPhraseDTO>();
            for (int i = 0; i < 20; i++)
            {

                KeyPhrasesDTOs.Add(new KeyPhraseDTO
                {
                    Affinty = 1.ToString(),
                    Text = i.ToString()
                });

            }
            temp.rank_list = KeyPhrasesDTOs;
            return temp;
        }
        private JobPosting CreateJobPostingAsync(JobPosting jobPosting)
        {
            jobPosting.Id = 1;
            return jobPosting;
        }

        private JobPosting TickNumberOfViewAsync(JobPosting jobPosting)
        {
            jobPosting.NumberOfViews++;
            return jobPosting;
        }
        private JobPosting GetJobPostingById(int id)
        {
            return null;
        }

        private List<JobPostingDTO> ConfigureSearchAsync(HomeIndexViewModel homeIndexVm)
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
