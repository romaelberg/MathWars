using System;
using System.Threading.Tasks;
using MathWars.Data;
using System.Linq;
using MathWars.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;


namespace MathWars.Controllers
{
    public class SolveController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<AppUser> _signInManager;

        public SolveController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        [HttpPost]
        public async Task<IActionResult> SubmitSolve(int taskId, string userAnswer)
        {
            var user = _context.Users.First(u => u.UserName == User.Identity.Name);
            var userAttemptsList = (from so in _context.SolveHistory where (so.TaskId == taskId && so.UserId == user.Id) select so).ToList();
            if (userAttemptsList.Count == 0)
            {
                var sobj = new SolveObj();
                sobj.TaskId = taskId;
                sobj.UserAnswer = userAnswer;
                sobj.UserId = user.Id;
                sobj.Created = DateTime.Now;
                var RightAnswersList =
                    (from rga in _context.RightAnswers where rga.TaskId == taskId && rga.Answer == userAnswer select rga).ToList();
                if (RightAnswersList.Count == 0)
                {
                    sobj.Status = "error";
                }
                else
                {
                    sobj.Status = "completed";
                }

                _context.Add(sobj);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Details", "WarTask", new {id = taskId});
        }
    }
}