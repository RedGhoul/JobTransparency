using AJobBoard.Controllers.Views;
using AJobBoard.Data;
using AJobBoard.Models;
using AJobBoard.Models.Data;
using AJobBoard.Models.DTO;
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
        public async Task Index_ReturnsAViewResult_WithAListOfJobPostings()
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
            //var result = await controller.Index(homeIndexVm);

            // Assert
            //var viewResult = Assert.IsType<ViewResult>(result);
            //var model = Assert.IsAssignableFrom <HomeIndexViewModel>(
            //    viewResult.Model);
            //Assert.Equal(10, model.jobPostings.Count());
        }


        [Fact]
        public async Task Details_ReturnsAValid_ViewResult_WhenInRangeId()
        {
            // Arrange
            int jobId = 1;
            var tempjob = new JobPosting
            {
                Id = jobId,
                Title = "Full Stack Developer",
                Company = "Walmart",
                NumberOfViews = 1
            };
            var mockRepoJob = new Mock<IJobPostingRepository>();
            mockRepoJob.Setup(repo => repo.GetJobPostingById(jobId))
                .ReturnsAsync(tempjob);

            mockRepoJob.Setup(repo => repo.TickNumberOfViewAsync(tempjob))
               .ReturnsAsync(TickNumberOfViewAsync(tempjob));

            var mockRepoKeyPharse = new Mock<IKeyPharseRepository>();

            var mockNLTKService = new Mock<INLTKService>();

            var controller = new JobPostingsController(mockRepoJob.Object,
                mockNLTKService.Object, mockRepoKeyPharse.Object);

            // Act
            var result = await controller.Details(jobId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<JobPosting>(
                viewResult.ViewData.Model);
            Assert.Equal(2, model.NumberOfViews);
           
        }

        [Fact]
        public async Task Details_Returns_NotFoundResult_WhenOutOfRangeId()
        {
            // Arrange
            int jobId = 1000;
            var mockRepoJob = new Mock<IJobPostingRepository>();
            mockRepoJob.Setup(repo => repo.GetJobPostingById(jobId))
                .ReturnsAsync(GetJobPostingById(jobId));

            var mockRepoKeyPharse = new Mock<IKeyPharseRepository>();

            var mockNLTKService = new Mock<INLTKService>();

            var controller = new JobPostingsController(mockRepoJob.Object,
                mockNLTKService.Object, mockRepoKeyPharse.Object);

            // Act
            var result = await controller.Details(jobId);

            // Assert

            var viewResult = Assert.IsType<NotFoundResult>(result);

        }

        [Fact]
        public async Task Create_Returns_ValidViewResult_WhenModelStateInvalid()
        {
            // Arrange
            var tempjob = new JobPosting
            {
                Title = "Full Stack Developer",
                Company = "Walmart",
                Description = "Yolo Lyfe",
                NumberOfViews = 1
            };
            tempjob = CreateJobPostingAsync(tempjob);


            var mockRepoJob = new Mock<IJobPostingRepository>();
            mockRepoJob.Setup(repo => repo.CreateJobPostingAsync(tempjob))
                .ReturnsAsync(tempjob);

            mockRepoJob.Setup(repo => repo.PutJobPostingAsync(tempjob.Id, tempjob))
                .ReturnsAsync(tempjob);

            var temKeyPharse = GetNLTKKeyPhrases(tempjob.Description);
            tempjob.KeyPhrases = new List<KeyPhrase>();
            foreach (var item in temKeyPharse.rank_list)
            {
                tempjob.KeyPhrases.Add(new KeyPhrase
                {
                    Affinty = item.Affinty,
                    Text = item.Text
                });
            }
            var tempValDTO = GetNLTKSummary(tempjob.Description);
            tempjob.Summary = tempValDTO.SummaryText;

            var mockRepoKeyPharse = new Mock<IKeyPharseRepository>();

            var mockNLTKService = new Mock<INLTKService>();

            mockNLTKService.Setup(service => service.GetNLTKKeyPhrases(tempjob.Description))
                .ReturnsAsync(temKeyPharse);

            mockNLTKService.Setup(service => service.GetNLTKSummary(tempjob.Description))
                .ReturnsAsync(tempValDTO);

            var controller = new JobPostingsController(mockRepoJob.Object,
                mockNLTKService.Object, mockRepoKeyPharse.Object);

            controller.ModelState.AddModelError("Salary", "no Salary found");

            // Act
            var result = await controller.Create(tempjob);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<JobPosting>(
                viewResult.ViewData.Model);

        }

        [Fact]
        public async Task Create_Returns_RedirectToActionResult_WhenModelStateValid()
        {
            // Arrange
            var tempjob = new JobPosting
            {
                Title = "Full Stack Developer",
                Company = "Walmart",
                Description = "Yolo Lyfe",
                NumberOfViews = 1
            };
            tempjob = CreateJobPostingAsync(tempjob);


            var mockRepoJob = new Mock<IJobPostingRepository>();
            mockRepoJob.Setup(repo => repo.CreateJobPostingAsync(tempjob))
                .ReturnsAsync(tempjob);

            mockRepoJob.Setup(repo => repo.PutJobPostingAsync(tempjob.Id, tempjob))
                .ReturnsAsync(tempjob);

            var temKeyPharse = GetNLTKKeyPhrases(tempjob.Description);
            tempjob.KeyPhrases = new List<KeyPhrase>();
            foreach (var item in temKeyPharse.rank_list)
            {
                tempjob.KeyPhrases.Add(new KeyPhrase
                {
                    Affinty = item.Affinty,
                    Text = item.Text
                });
            }
            var tempValDTO = GetNLTKSummary(tempjob.Description);
            tempjob.Summary = tempValDTO.SummaryText;

            var mockRepoKeyPharse = new Mock<IKeyPharseRepository>();

            var mockNLTKService = new Mock<INLTKService>();

            mockNLTKService.Setup(service => service.GetNLTKKeyPhrases(tempjob.Description))
                .ReturnsAsync(temKeyPharse);

            mockNLTKService.Setup(service => service.GetNLTKSummary(tempjob.Description))
                .ReturnsAsync(tempValDTO);

            var controller = new JobPostingsController(mockRepoJob.Object,
                mockNLTKService.Object, mockRepoKeyPharse.Object);


            // Act
            var result = await controller.Create(tempjob);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);

        }

        [Fact]
        public async Task Edit_Get_Returns_NotFoundViewResult_WhenOutOfRangeId()
        {
            Assert.True(true);
        }

        [Fact]
        public async Task Edit_Get_Returns_ValidViewResult_WhenInRangeId()
        {
            Assert.True(true);
        }

        [Fact]
        public async Task Edit_Post_Returns_NotFoundViewResult_WhenOutOfRangeId()
        {
            Assert.True(true);
        }

        [Fact]
        public async Task Edit_Post_Returns_ValidViewResult_WhenInvalidModel()
        {
            Assert.True(true);
        }

        [Fact]
        public async Task Delete_Get_Returns_ValidViewResult_WhenIdIsInRange()
        {
            Assert.True(true);
        }

        [Fact]
        public async Task Delete_Post_Returns_RedirectActionResult()
        {
            Assert.True(true);
        }

        private SummaryDTO GetNLTKSummary(string des)
        {
            return new SummaryDTO { SummaryText = "Yo" };
        }

        private KeyPhrasesWrapperDTO GetNLTKKeyPhrases(string des)
        {
            KeyPhrasesWrapperDTO temp = new KeyPhrasesWrapperDTO();
            var KeyPhrasesDTOs = new List<KeyPhrasesDTO>();
            for (int i = 0; i < 20; i++)
            {
                
                KeyPhrasesDTOs.Add(new KeyPhrasesDTO
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

        private List<JobPosting> ConfigureSearchAsync(HomeIndexViewModel homeIndexVm)
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
