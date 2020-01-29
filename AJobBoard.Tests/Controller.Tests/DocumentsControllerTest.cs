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
using ViewResult = Microsoft.AspNetCore.Mvc.ViewResult;

namespace AJobBoard.Tests.Controller.Tests
{
    public class DocumentsControllerTest
    {
        [Fact]
        public async Task Index_ReturnViewResult_With_Documents()
        {
            // Arrange
            ApplicationUser user = new ApplicationUser();
            user.Id = "ids";
            var mockUserRepo = new Mock<IUserRepository>();
            HttpContext temp = null;
            mockUserRepo.Setup(repo => repo.getUserFromHttpContextAsync(temp))
                .ReturnsAsync(user);

            var mockDocRepo = new Mock<IDocumentRepository>();
            mockDocRepo.Setup(repo => repo.GetDocumentsOfCurrentUser(user.Id))
                .Returns(GetDocumentsOfCurrentUser());

            var controller = new DocumentsController(
                mockUserRepo.Object, mockDocRepo.Object);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Document>>(
                viewResult.ViewData.Model);
            Assert.Equal(10, model.Count());
        }

        [Fact]
        public async Task Details_ReturnViewResult_With_Document()
        {
            // Arrange
            var tempDoc = new Document
            {
                DocumentId = 1,
                DocumentName = "Meh"
            };
            var mockUserRepo = new Mock<IUserRepository>();

            var mockDocRepo = new Mock<IDocumentRepository>();
            mockDocRepo.Setup(repo => repo.GetDocumentByIdAsync(1))
                .ReturnsAsync(tempDoc);

            var controller = new DocumentsController(mockUserRepo.Object,
                mockDocRepo.Object);

            // Act
            var result = await controller.Details(tempDoc.DocumentId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Document>(
                viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Details_ReturnNotFoundViewResult_When_NoValidIdGiven()
        {
            // Arrange
            var tempDoc = new Document
            {
                DocumentId = 100000,
                DocumentName = "Meh"
            };
            var mockUserRepo = new Mock<IUserRepository>();

            var mockDocRepo = new Mock<IDocumentRepository>();
            mockDocRepo.Setup(repo => repo.GetDocumentByIdAsync(1))
                .ReturnsAsync(tempDoc);

            var controller = new DocumentsController(mockUserRepo.Object,
                mockDocRepo.Object);

            // Act
            var result = await controller.Details(tempDoc.DocumentId);

            // Assert

            var viewResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsReDirectResult_When_ValidModelIsGiven()
        {
            // Arrange
            var tempDocumentVM = new DocumentViewModel
            {
               DocumentName = "new DocumentViewModel"
            };


            ApplicationUser user = new ApplicationUser();
            user.Id = "ids";
            var mockUserRepo = new Mock<IUserRepository>();
            HttpContext temp = null;
            mockUserRepo.Setup(repo => repo.getUserFromHttpContextAsync(temp))
                .ReturnsAsync(user);

            var mockDocumentRepo = new Mock<IDocumentRepository>();

            mockDocumentRepo.Setup(repo => repo.SaveDocumentToUser(tempDocumentVM, user))
                .ReturnsAsync(true);

            var controller = new DocumentsController(mockUserRepo.Object,
            mockDocumentRepo.Object);

            // Act
            var result = await controller.Create(tempDocumentVM);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", viewResult.ActionName);
        }

        [Fact]
        public async Task Create_ReturnsViewResult_When_InValidModelIsGiven()
        {

            // Arrange
            var mockResumeFormFile = new Mock<IFormFile>();
            mockResumeFormFile.Setup(file => file.Length)
                .Returns(0);


            var tempDocumentVM = new DocumentViewModel
            {
                DocumentName = "new DocumentViewModel",
                Resume = mockResumeFormFile.Object
            };


            ApplicationUser user = new ApplicationUser();
            user.Id = "ids";
            var mockUserRepo = new Mock<IUserRepository>();
            HttpContext temp = null;
            mockUserRepo.Setup(repo => repo.getUserFromHttpContextAsync(temp))
                .ReturnsAsync(user);

            var mockDocumentRepo = new Mock<IDocumentRepository>();

            mockDocumentRepo.Setup(repo => repo.SaveDocumentToUser(tempDocumentVM, user))
                .ReturnsAsync(false);

            var controller = new DocumentsController(mockUserRepo.Object,
            mockDocumentRepo.Object);

            // Act
            var result = await controller.Create(tempDocumentVM);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);

        }

        [Fact]
        public async Task Edit_Get_ReturnsNotFoundResult_When_InValidIDIsGiven()
        {

        }

        [Fact]
        public async Task Edit_Get_ReturnsViewResult_When_ValidIDIsGiven()
        {

        }

        [Fact]
        public async Task Edit_Post_ReturnsViewResult_When_ValidModelIsGiven()
        {

        }

        [Fact]
        public async Task Edit_Post_ReturnsViewResult_When_InValidModelIsGiven()
        {

        }

        [Fact]
        public async Task Edit_Post_ReturnsNotFoundResult_When_InValidIDIsGiven()
        {

        }

        [Fact]
        public async Task Delete_Get_ReturnsNotFoundResult_When_InValidIDIsGiven()
        {

        }

        [Fact]
        public async Task Delete_Get_ReturnsViewResult_When_ValidIDIsGiven()
        {

        }

        [Fact]
        public async Task Delete_Post_ReturnsRedirectResult_When_ValidIDIsGiven()
        {

        }

        public List<Document> GetDocumentsOfCurrentUser()
        {
            List<Document> TempDocuments = new List<Document>();

            for(int i = 0; i < 10; i++)
            {
                TempDocuments.Add(new Document
                {
                    DocumentName = "Yolo " + i
                });
            }
            return TempDocuments;
        }


    }
}
