using AJobBoard.Controllers.Views;
using AJobBoard.Data;
using AJobBoard.Models.View;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
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
            ApplicationUser user = new ApplicationUser
            {
                Id = "ids"
            };
            Mock<IUserRepository> mockUserRepo = new Mock<IUserRepository>();
            HttpContext temp = null;
            mockUserRepo.Setup(repo => repo.getUserFromHttpContextAsync(temp))
                .ReturnsAsync(user);

            Mock<IDocumentRepository> mockDocRepo = new Mock<IDocumentRepository>();
            mockDocRepo.Setup(repo => repo.GetDocumentsOfCurrentUser(user.Id))
                .Returns(GetDocumentsOfCurrentUser());

            DocumentsController controller = new DocumentsController(
                mockUserRepo.Object, mockDocRepo.Object);

            // Act
            IActionResult result = await controller.Index();

            // Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            IEnumerable<Document> model = Assert.IsAssignableFrom<IEnumerable<Document>>(
                viewResult.ViewData.Model);
            Assert.Equal(10, model.Count());
        }

        [Fact]
        public async Task Details_ReturnViewResult_With_Document()
        {
            // Arrange
            Document tempDoc = new Document
            {
                DocumentId = 1,
                DocumentName = "Meh"
            };
            Mock<IUserRepository> mockUserRepo = new Mock<IUserRepository>();

            Mock<IDocumentRepository> mockDocRepo = new Mock<IDocumentRepository>();
            mockDocRepo.Setup(repo => repo.GetDocumentByIdAsync(1))
                .ReturnsAsync(tempDoc);

            DocumentsController controller = new DocumentsController(mockUserRepo.Object,
                mockDocRepo.Object);

            // Act
            IActionResult result = await controller.Details(tempDoc.DocumentId);

            // Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            Document model = Assert.IsAssignableFrom<Document>(
                viewResult.ViewData.Model);
        }

        [Fact]
        public async Task Details_ReturnNotFoundViewResult_When_NoValidIdGiven()
        {
            // Arrange
            Document tempDoc = new Document
            {
                DocumentId = 100000,
                DocumentName = "Meh"
            };
            Mock<IUserRepository> mockUserRepo = new Mock<IUserRepository>();

            Mock<IDocumentRepository> mockDocRepo = new Mock<IDocumentRepository>();
            mockDocRepo.Setup(repo => repo.GetDocumentByIdAsync(1))
                .ReturnsAsync(tempDoc);

            DocumentsController controller = new DocumentsController(mockUserRepo.Object,
                mockDocRepo.Object);

            // Act
            IActionResult result = await controller.Details(tempDoc.DocumentId);

            // Assert

            NotFoundResult viewResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsReDirectResult_When_ValidModelIsGiven()
        {
            // Arrange
            DocumentViewModel tempDocumentVM = new DocumentViewModel
            {
                DocumentName = "new DocumentViewModel"
            };


            ApplicationUser user = new ApplicationUser
            {
                Id = "ids"
            };
            Mock<IUserRepository> mockUserRepo = new Mock<IUserRepository>();
            HttpContext temp = null;
            mockUserRepo.Setup(repo => repo.getUserFromHttpContextAsync(temp))
                .ReturnsAsync(user);

            Mock<IDocumentRepository> mockDocumentRepo = new Mock<IDocumentRepository>();

            mockDocumentRepo.Setup(repo => repo.SaveDocumentToUser(tempDocumentVM, user))
                .ReturnsAsync(true);

            DocumentsController controller = new DocumentsController(mockUserRepo.Object,
            mockDocumentRepo.Object);

            // Act
            IActionResult result = await controller.Create(tempDocumentVM);

            // Assert
            RedirectToActionResult viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", viewResult.ActionName);
        }

        [Fact]
        public async Task Create_ReturnsViewResult_When_InValidModelIsGiven()
        {

            // Arrange
            Mock<IFormFile> mockResumeFormFile = new Mock<IFormFile>();
            mockResumeFormFile.Setup(file => file.Length)
                .Returns(0);


            DocumentViewModel tempDocumentVM = new DocumentViewModel
            {
                DocumentName = "new DocumentViewModel",
                Resume = mockResumeFormFile.Object
            };


            ApplicationUser user = new ApplicationUser
            {
                Id = "ids"
            };
            Mock<IUserRepository> mockUserRepo = new Mock<IUserRepository>();
            HttpContext temp = null;
            mockUserRepo.Setup(repo => repo.getUserFromHttpContextAsync(temp))
                .ReturnsAsync(user);

            Mock<IDocumentRepository> mockDocumentRepo = new Mock<IDocumentRepository>();

            mockDocumentRepo.Setup(repo => repo.SaveDocumentToUser(tempDocumentVM, user))
                .ReturnsAsync(false);

            DocumentsController controller = new DocumentsController(mockUserRepo.Object,
            mockDocumentRepo.Object);

            // Act
            IActionResult result = await controller.Create(tempDocumentVM);

            // Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);

        }

        //[Fact]
        //public async Task Edit_Get_ReturnsNotFoundResult_When_InValidIDIsGiven()
        //{

        //}

        //[Fact]
        //public async Task Edit_Get_ReturnsViewResult_When_ValidIDIsGiven()
        //{

        //}

        //[Fact]
        //public async Task Edit_Post_ReturnsViewResult_When_ValidModelIsGiven()
        //{

        //}

        //[Fact]
        //public async Task Edit_Post_ReturnsViewResult_When_InValidModelIsGiven()
        //{

        //}

        //[Fact]
        //public async Task Edit_Post_ReturnsNotFoundResult_When_InValidIDIsGiven()
        //{

        //}

        //[Fact]
        //public async Task Delete_Get_ReturnsNotFoundResult_When_InValidIDIsGiven()
        //{

        //}

        //[Fact]
        //public async Task Delete_Get_ReturnsViewResult_When_ValidIDIsGiven()
        //{

        //}

        //[Fact]
        //public async Task Delete_Post_ReturnsRedirectResult_When_ValidIDIsGiven()
        //{

        //}

        public List<Document> GetDocumentsOfCurrentUser()
        {
            List<Document> TempDocuments = new List<Document>();

            for (int i = 0; i < 10; i++)
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
