using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication1.Data;
using WebApplication1.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly IAuthorizationService _authService;


        public HomeController(ApplicationContext context, IAuthorizationService authService)
        {
            _context = context;
            _authService = authService;
        }


        public async Task<IActionResult> Index()
        {
            return View();
        }


        public async Task<IActionResult> GetRecipe(int id)
        {
            var currentRecipe = await _context.Recipes.FirstOrDefaultAsync(e => e.Id == id);
            var authResult = await _authService.AuthorizeAsync(User, currentRecipe, "CanManageRecipe");
            if (!authResult.Succeeded)
            {
                return new ForbidResult();
            }
            return Content($"Can manage {currentRecipe.Name}");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

