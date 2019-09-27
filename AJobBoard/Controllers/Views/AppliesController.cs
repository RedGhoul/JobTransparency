using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AJobBoard.Data;
using AJobBoard.Models;
using Microsoft.AspNetCore.Identity;

namespace AJobBoard.Controllers
{
    public class AppliesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AppliesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Applies
        public IActionResult Index()
        {
            return View();
        }

        
    }
}
