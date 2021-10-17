using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MathWars.CloudStorage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MathWars.Data;
using MathWars.Models;
using MathWars.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace MathWars.Controllers
{
    public class WarTaskController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly ICloudStorage _cloudStorage;

        public WarTaskController(ApplicationDbContext context, UserManager<AppUser> userManager, ICloudStorage cloudStorage)
        {
            _context = context;
            _userManager = userManager;
            _cloudStorage = cloudStorage;
        }
        
        private static string FormFileName(string title, string fileName)
        {
            var fileExtension = Path.GetExtension(fileName);
            var fileNameForStorage = $"{title}-{DateTime.Now.ToString("yyyyMMddHHmmss")}{fileExtension}";
            return fileNameForStorage;
        }
        
        // GET: WarTask
        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["NameSortParam"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["TopicSortParam"] = String.IsNullOrEmpty(sortOrder) ? "topic_desc" : "";
            ViewData["RatingSortParam"] = String.IsNullOrEmpty(sortOrder) ? "rating_desc" : "";
            ViewData["SearchString"] = searchString;
            var warTasks = from wt in _context.WarTasks select wt;
            if (!String.IsNullOrEmpty(searchString))
            {
                warTasks = warTasks.Where(s => s.Title.Contains(searchString)
                                               || s.Topic.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    warTasks = warTasks.OrderByDescending(s => s.Title);
                    break;
                case "topic_desc":
                    warTasks = warTasks.OrderByDescending(s => s.Topic);
                    break;
                case "rating_desc":
                    warTasks = warTasks.OrderByDescending(s => s.Rating);
                    break;
                default:
                    warTasks = warTasks.OrderBy(s => s.Created);
                    break;
            }
            return View(await warTasks.AsNoTracking().ToListAsync());
        }
        
        [HttpPost]
        [ActionName("AddComment")]
        public async Task<IActionResult> AddComment(int? taskId, string body)
        {
            if (taskId == null) NotFound();
            var warTask = await _context.WarTasks.FindAsync(taskId);
            if (warTask == null) return NotFound();
            var user = await _userManager.FindByIdAsync(_userManager.GetUserId(User));
            _context.Comments.Add(
                new Comment
                {
                    Body = body,
                    WarTask = warTask,
                    Author = user,
                    TaskId = warTask.Id,
                    Created = DateTime.Now
                }
            );
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "WarTask", new {id = taskId});
        }
        
        public WarTask GetWarTask(int? id)
        {
            return _context.WarTasks
                .Include(w => w.Tags)
                .Include(c => c.Images)
                .Include(a => a.Comments)
                .ThenInclude(x => x.Author)
                .FirstOrDefault(w => w.Id == id);
        }

        // GET: WarTask/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var warTask = GetWarTask(id);
            if(warTask == null) return NotFound();
            var user = await _userManager.FindByIdAsync(_userManager.GetUserId(User));
            if (GetUserSolvedWarTasks(user).Contains(warTask))
            {
                ViewData["solveStatus"] = "solved";
            }
            else
            {
                ViewData["solveStatus"] = "notSolved";
            }

            return View(warTask);
        }

        // GET: WarTask/Create
        public async Task<IActionResult> Create(string userName)
        {
            ViewData["userName"] = userName;
            return View();
        }
        
        [HttpPost]
        public List<Tag> AddTags(WarTask warTask, List<string> tags)  
        {
            var tagsText = tags.Select(t => 
                new Tag { Name = t.ToString(), WarTask = warTask }).ToList();
            return tagsText;
        }
        
        [HttpPost]
        public List<RightAnswer> AddRightAnswers(WarTask warTask, List<string> answers)  
        {
            var answersText = answers.Select(t => 
                new RightAnswer { Answer = t, WarTask = warTask}).ToList();
            return answersText;
        }
        
        [HttpPost]
        public async Task<List<Image>> CookImages(WarTask warTask, IFormFile[] images)
        {
            var warTaskImages = new List<Image>();
            foreach (var image in images)
            {
                var fileNameForStorage = FormFileName(warTask.Title, image.Name);
                warTaskImages.Add(new Image { WarTask = warTask, Url = await _cloudStorage.UploadFileAsync(image, fileNameForStorage)} );
            }
            return warTaskImages;
        }

        // POST: WarTask/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WarTask warTask, 
            string taskTags, string taskRightAnswers, IFormFile[] photos, string userName)
        {
            if (userName == null)
            {
                return NotFound();
            }
            // TODO file ext validation + file name random
            if (ModelState.IsValid)
            {
                warTask.Body = warTask.Body.Replace(Environment.NewLine, @"\r\n");
                _context.WarTasks.Add(warTask);
                warTask.Author = _context.Users.FirstOrDefault(u => u.UserName == userName);
                if(taskTags != null) {
                    var taskTagsList = taskTags.Split(";").ToList();
                    _context.Tags.AddRange(AddTags(warTask, taskTagsList));
                }

                if (taskRightAnswers != null)
                {
                    var rightAnswersList = taskRightAnswers.Split(";").ToList();
                    _context.RightAnswers.AddRange(AddRightAnswers(warTask, rightAnswersList));
                }
                _context.Images.AddRange(await CookImages(warTask, photos));
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
        public async Task<IActionResult> DeleteConfirmed(int id, string userName)
        {
            var warTask = await _context.WarTasks.FindAsync(id);
            _context.WarTasks.Remove(warTask);
            await _context.SaveChangesAsync();
            return Redirect($"/Profile/Index?userName={userName}");
        }

        [HttpGet, ActionName("GetTagsList")]
        public ActionResult GetTagsList()
        {
            var tagsList = _context.Tags.Select(t => t.Name).Distinct().ToList();
            var list = JsonConvert.SerializeObject(tagsList,
                Formatting.None,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                });
            return Content(list, "application/json");
        }
        
        private bool WarTaskExists(int id)
        {
            return _context.WarTasks.Any(e => e.Id == id);
        }

        public List<WarTask> GetUserSolvedWarTasks(AppUser user) =>
            (from w in _context.SolveHistory where w.Author == user select w.WarTask).ToList();

        [HttpPost]
        public async Task<IActionResult> SubmitSolve(int? taskId, string answerText)
        {
            var user = await _userManager.FindByIdAsync(_userManager.GetUserId(User));
            if (taskId == null) return NotFound();
            var warTask = await _context.WarTasks.FindAsync(taskId);
            if (warTask == null) return NotFound();
            var taskRightAnswersList = (from a in _context.RightAnswers
                where a.WarTask == warTask
                select a.Answer).ToList();
            if (!taskRightAnswersList.Contains(answerText))
                return RedirectToAction("Details", "WarTask", new {id = taskId});
            _context.SolveHistory.Add(new SolveObj
            {
                Author = user,
                UserAnswer = answerText,
                WarTask = warTask,
                Status = "completed",
                Created = DateTime.Now
            });
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "WarTask", new {id = taskId});
        }

        [HttpPost]
        public async Task<IActionResult> Search(string searchQuery)
        {
            var resultList = _context.WarTasks
                .Where(p => EF.Functions.ToTsVector("english", p.Title + " " + p.Body)
                    .Matches(searchQuery))
                .ToList();
            return View(resultList);
        }

    }
}
