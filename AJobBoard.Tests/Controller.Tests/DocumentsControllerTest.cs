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

        }

        [Fact]
        public async Task Details_ReturnViewResult_With_Document()
        {

        }

        [Fact]
        public async Task Details_ReturnNotFoundViewResult_When_NoValidIdGiven()
        {

        }

        [Fact]
        public async Task Create_ReturnsReDirectResult_When_ValidModelIsGiven()
        {

        }

        [Fact]
        public async Task Create_ReturnsViewResult_When_InValidModelIsGiven()
        {

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


    }
}
