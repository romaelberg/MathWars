using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MathWars.Data;
using MathWars.Models;
using MathWars.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MathWars.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(ApplicationDbContext context,  RoleManager<IdentityRole> roleManager,UserManager<AppUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var allUsersExceptCurrentUser = await
                (from u in _context.Users where u.Id != currentUser.Id select u).ToListAsync();
            return View(allUsersExceptCurrentUser);
        }
        
        public async Task<IActionResult> EditRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.ToList();
            ChangeRoleViewModel crvm = new ChangeRoleViewModel
            {
                UserId = user.Id,
                UserEmail = user.Email,
                UserRoles = userRoles,
                AllRoles = allRoles
            };
            return View(crvm);
        }
        
        [HttpPost]
        public async Task<IActionResult> EditRoles(string userId, List<string> roles)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.ToList();
            var addedRoles = roles.Except(userRoles);
            var removedRoles = userRoles.Except(roles);
            await _userManager.AddToRolesAsync(user, addedRoles);
            await _userManager.RemoveFromRolesAsync(user, removedRoles);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> BanUser(string userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return NotFound();
            }
            user.LockoutEnd = System.DateTime.Now.AddHours(24 * 7);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Admin");
        }
        
        public async Task<IActionResult> UnBanUser(string userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return NotFound();
            }
            user.LockoutEnd = null;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Admin");
        }

        public async Task<IActionResult> RemoveUser(string userId)
        {
            var users = (from u in _context.Users where u.Id == userId select u).ToList();
            foreach (var user in users)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Admin");
        }
}
}