using System;
using System.Threading.Tasks;
using MathWars.Data;
using MathWars.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MathWars.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CommentsController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(string body, int taskId)
        {
            if (String.IsNullOrEmpty(body))
            {
                return RedirectToAction("Details", "WarTask", new {id = taskId});
            }
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            var cm = new Comment();
            cm.Body = body;
            cm.Created = DateTime.Now;
            cm.TaskId = taskId;
            cm.AuthorId = user.Id;
            cm.AuthorName = user.UserName;
            _context.Add(cm);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "WarTask", new {id = taskId});
        }
    }
}