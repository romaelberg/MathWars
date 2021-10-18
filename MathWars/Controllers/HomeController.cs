using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MathWars.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MathWars.Models;
using MathWars.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;


namespace MathWars.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        
        [HttpPost]
        public IActionResult SetCulture(string culture, string returnUrl)
        {
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });
            return LocalRedirect(returnUrl);
        }
        
        public IActionResult Index(string sortOrder, string searchString, string searchFor)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["TitleSortParm"] = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["RatingSortParam"] = String.IsNullOrEmpty(sortOrder) ? "rating_desc" : "";
            ViewData["SearchString"] = searchString;

            var tagsList = _context.Tags.Select(t => t.Name).Distinct().ToList();
            var latestTasks = _context.WarTasks.OrderByDescending(wt => wt.Created).ToList();
            var topRatedTasks = _context.WarTasks.OrderByDescending(wt => wt.Rating).ToList();
            if (!String.IsNullOrEmpty(searchString))
            {
                topRatedTasks = topRatedTasks.Where(s => s.Title.Contains(searchString)
                                                         || s.Topic.Contains(searchString)).ToList();
                if (!String.IsNullOrEmpty(searchFor))
                {
                    if (searchFor == "tags")
                    {
                        topRatedTasks = _context.Tags.Where(t => t.Name == searchString).Select(t => t.WarTask).Distinct().ToList();
                        ViewData["SearchFor"] = "tags";
                    }
                }
            }
            switch (sortOrder)
            {
                case "name_desc":
                    topRatedTasks = topRatedTasks.OrderByDescending(s => s.Title).ToList();
                    break;
                case "topic_desc":
                    topRatedTasks = topRatedTasks.OrderByDescending(s => s.Topic).ToList();
                    break;
                case "rating_desc":
                    topRatedTasks = topRatedTasks.OrderByDescending(s => s.Rating).ToList();
                    break;
                default:
                    topRatedTasks = topRatedTasks.OrderByDescending(s => s.Rating).ToList();
                    break;
            }
            var vm = new HomeViewModel();
            vm.Tags = tagsList;
            vm.LatestTasks = latestTasks;
            vm.TopRatedTasks = topRatedTasks;
            return View(vm);
        }

        [HttpPost]
        public IActionResult Cookie(string culture)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddMonths(1) }
            );
 
            return RedirectToAction("Cookie");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}