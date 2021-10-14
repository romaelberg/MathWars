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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace MathWars.Controllers
{
    public class WarTaskController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICloudStorage _cloudStorage;

        public WarTaskController(ApplicationDbContext context, ICloudStorage cloudStorage)
        {
            _context = context;
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

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            // var TaskTagsList = from tg in _context.Tags where tg.
            var taskCommentsList = (from cm in _context.Comments where cm.TaskId == id select cm).OrderByDescending(cm => cm.Created).ToList();
            var taskImages = (from ti in _context.Images where ti.TaskId == warTask.Id select ti).ToList();

            // var TaskSolveStatus = (from sst in _context.SolveHistory where (sst.TaskId == id && sst.UserId == user.Id) select sst).ToList();
            var dvm = new DetailsViewModel();
            dvm.WarTaskModel = warTask;
            if (user != null)
            {
                var taskSolveStatusObj =
                    _context.SolveHistory.FirstOrDefault(sst => sst.TaskId == id && sst.UserId == user.Id);
                if (taskSolveStatusObj == null)
                {
                    dvm.SolveStatus = "not solved";
                }
                else
                {
                    dvm.SolveStatus = taskSolveStatusObj.Status;
                }
            }

            dvm.TaskComments = taskCommentsList;
            dvm.TaskImages = taskImages;
            return View(dvm);
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
            string taskTags, string taskRightAnswers, IFormFile[] photos)
        {
            // TODO file ext validation + file name random
            if (ModelState.IsValid)
            {
                var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);
                warTask.AuthorId = user.Id;
                _context.Add(warTask);
                await _context.SaveChangesAsync();
                if (taskTags != null)
                {
                    var taskTagsList = taskTags.Split(";");
                    foreach (var tag in taskTagsList)
                    {
                        Tag tagObj = new Tag();
                        tagObj.Name = tag;
                        _context.Tags.Add(tagObj);
                    }
                }

                if (taskRightAnswers != null)
                {
                    var taskRightAnswersList = taskRightAnswers.Split(";");
                    foreach (var rga in taskRightAnswersList)
                    {
                        RightAnswer rgaObj = new RightAnswer();
                        rgaObj.Answer = rga;
                        rgaObj.TaskId = warTask.Id;
                        _context.RightAnswers.Add(rgaObj);
                    }
                }

                foreach (IFormFile photo in photos)
                {
                    string fileNameForStorage = FormFileName(warTask.Title, photo.FileName);
                    Image imageModel = new Image();
                    imageModel.TaskId = warTask.Id;
                    imageModel.Url = await _cloudStorage.UploadFileAsync(photo, fileNameForStorage);
                    _context.Images.Add(imageModel);
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

        [HttpGet, ActionName("GetTagsList")]
        public ActionResult GetTagsList()
        {
            var tagsList = _context.Tags.Select(t => t.Name).ToList();
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
    }
}
