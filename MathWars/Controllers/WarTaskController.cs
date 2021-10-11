using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MathWars.Data;
using MathWars.Models;
using Microsoft.AspNetCore.Identity;

namespace MathWars.Controllers
{
    public class WarTaskController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public WarTaskController(ApplicationDbContext context, SignInManager<AppUser> signInManager, 
            UserManager<AppUser> userManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // GET: WarTask
        public async Task<IActionResult> Index()
        {
            return View(await _context.WarTasks.ToListAsync());
        }

        // GET: WarTask/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var warTask = await _context.WarTasks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (warTask == null)
            {
                return NotFound();
            }

            return View(warTask);
        }

        // GET: WarTask/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: WarTask/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AuthorId,Title,Topic,Body")] WarTask warTask, 
            string taskTags, string taskRightAnswers)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);
                warTask.AuthorId = user.Id;
                var taskTagsList = taskTags.Split(";");
                var taskRightAnswersList = taskRightAnswers.Split(";");
                foreach (var tag in taskTagsList)
                {
                    Tag tagObj = new Tag();
                    tagObj.Name = tag;
                    _context.Tags.Add(tagObj);
                }
                _context.Add(warTask);
                await _context.SaveChangesAsync();
                foreach (var rga in taskRightAnswersList)
                {
                    RightAnswer rgaObj = new RightAnswer();
                    rgaObj.Answer = rga;
                    rgaObj.TaskId = warTask.Id;
                    _context.RightAnswers.Add(rgaObj);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(warTask);
        }

        // GET: WarTask/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var warTask = await _context.WarTasks.FindAsync(id);
            if (warTask == null)
            {
                return NotFound();
            }
            return View(warTask);
        }

        // POST: WarTask/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AuthorId,Title,Topic,Body")] WarTask warTask)
        {
            if (id != warTask.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(warTask);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WarTaskExists(warTask.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(warTask);
        }

        // GET: WarTask/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var warTask = await _context.WarTasks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (warTask == null)
            {
                return NotFound();
            }

            return View(warTask);
        }

        // POST: WarTask/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var warTask = await _context.WarTasks.FindAsync(id);
            _context.WarTasks.Remove(warTask);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Profile");
        }

        private bool WarTaskExists(int id)
        {
            return _context.WarTasks.Any(e => e.Id == id);
        }
    }
}
