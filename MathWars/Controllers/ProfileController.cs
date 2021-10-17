using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MathWars.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace MathWars.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Authorize]
        public async Task<IActionResult> Index(string sortOrder, string searchString, string userName)
        {
            if (userName == null)
            {
                return NotFound();
            }

            ViewData["userName"] = userName;
            ViewData["NameSortParam"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["TopicSortParam"] = String.IsNullOrEmpty(sortOrder) ? "topic_desc" : "";
            var user = _context.Users
                .Include(u => u.CreatedWarTasks)
                .ThenInclude(w => w.SolvedWarTasks)
                .Include(u => u.SolvedWarTasks)
                .FirstOrDefault(x => x.UserName == userName);
            switch (sortOrder)
            {
                case "name_desc":
                    user.SolvedWarTasks = user.SolvedWarTasks.OrderByDescending(s => s.Title).ToList();
                    break;
                case "topic_desc":
                    user.SolvedWarTasks = user.SolvedWarTasks.OrderByDescending(s => s.Topic).ToList();
                    break;
                case "rating_desc":
                    user.SolvedWarTasks = user.SolvedWarTasks.OrderByDescending(s => s.Rating).ToList();
                    break;
                default:
                    user.SolvedWarTasks = user.SolvedWarTasks.OrderByDescending(s => s.Created).ToList();
                    break;
            }
            ViewData["userId"] = user.Id;
            return View(user);
        }
    }
}