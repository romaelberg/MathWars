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
        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            ViewData["NameSortParam"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["TopicSortParam"] = String.IsNullOrEmpty(sortOrder) ? "topic_desc" : "";
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);
            var userTasks = _context.WarTasks.Where(t => t.AuthorId == user.Id);
            var userSolvedTasks = from st in _context.SolveHistory where st.UserId == user.Id select st;
            switch (sortOrder)
            {
                case "name_desc":
                    userTasks = userTasks.OrderByDescending(s => s.Title);
                    break;
                case "topic_desc":
                    userTasks = userTasks.OrderByDescending(s => s.Topic);
                    break;
                case "rating_desc":
                    userTasks = userTasks.OrderByDescending(s => s.Rating);
                    break;
                default:
                    userTasks = userTasks.OrderBy(s => s.Created);
                    break;
            }
            
            ViewData["userTasks"] = await userTasks.ToListAsync();
            ViewData["userSolvedTasks"] = await userSolvedTasks.ToListAsync();
            ViewData["userId"] = user.Id;
            return View();
        }
    }
}