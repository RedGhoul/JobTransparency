using AJobBoard.Controllers.Views;
using AJobBoard.Data;
using AJobBoard.Models;
using AJobBoard.Models.View;
using Microsoft.AspNetCore.Http;
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
    public class AppliesControllerTest
    {
        [Fact]
        public async Task Index_ReturnViewResult_With_Applies()
        {
            // Arrange
            var mockAppliesRepo = new Mock<IAppliesRepository>();

            ApplicationUser user = new ApplicationUser();
            mockAppliesRepo.Setup(repo => repo.GetUsersAppliesAsync(user))
                .ReturnsAsync(GetUsersAppliesAsync(user));

            HttpContext temp = null;
            var mockUserRepo = new Mock<IUserRepository>();
            mockUserRepo.Setup(repo => repo.getUserFromHttpContextAsync(temp))
                .ReturnsAsync(user);

            var controller = new AppliesController(
                mockAppliesRepo.Object, mockUserRepo.Object);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<AppliesIndexViewModel>(
                viewResult.ViewData.Model);
            Assert.Equal(20, model.Applies.Count());
        }

        public List<Apply> GetUsersAppliesAsync(ApplicationUser User)
        {
            List<Apply> temp = new List<Apply>();

            for (int i = 0; i < 20; i++)
            {
                temp.Add(new Apply()
                {
                    JobPosting = new JobPosting()
                    {
                        Title = "FSD" + i
                    }
                });
            }


            return temp;
        }

    }
}
