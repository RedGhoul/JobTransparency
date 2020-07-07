using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jobtransparency.Controllers.Views
{
    [Authorize]
    public class DataAdminActionsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

    }
}