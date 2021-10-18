using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MathWars.Data;
using MathWars.Models;
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
        
        public List<WarTask> GetUserSolvedWarTasks(AppUser user) =>
            (from w in _context.SolveHistory where w.Author == user select w.WarTask).ToList();
        [Authorize]
        public async Task<IActionResult> Index(string sortOrder, string userName)
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
                .ThenInclude(a => a.Author)
                .FirstOrDefault(x => x.UserName == userName);
            if (user == null) return NotFound();
            user.SolvedWarTasks = GetUserSolvedWarTasks(user) ?? new List<WarTask>();
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