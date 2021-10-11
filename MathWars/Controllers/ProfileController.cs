using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MathWars.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace MathWars.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);
            var userTasks = _context.WarTasks.Where(t => t.AuthorId == user.Id);
            ViewData["userTasks"] = await userTasks.ToListAsync();
            return View();
        }
    }
}