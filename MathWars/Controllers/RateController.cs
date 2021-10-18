using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MathWars.Data;
using MathWars.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis;

namespace MathWars.Controllers
{
    public class RateController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RateController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(int rateValue, int taskId)
        {
            var taskObj = await _context.WarTasks.AsNoTracking().FirstOrDefaultAsync(t => t.Id == taskId);
            if (taskObj == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            var userTaskRatingList = (from rt in _context.Ratings where (rt.TaskId == taskId && rt.UserId == user.Id) select rt).ToList();
            if (userTaskRatingList.Count != 0)
            {
                return RedirectToAction("Details", "WarTask", new {id=taskId});
            }
            var rateObj = new Rate();
            rateObj.Value = rateValue;
            rateObj.TaskId = taskId;
            rateObj.UserId = user.Id;
            _context.Ratings.Add(rateObj);
            // await _context.SaveChangesAsync();
            // TODO sum in db query
            var ratingsList = (from rt in _context.Ratings where rt.TaskId == taskId select rt.Value).ToList();
            int ratesSum = 0;
            foreach (var rt in ratingsList)
            {
                ratesSum += rt;
            }

            taskObj.Rating = ratesSum / ratingsList.Count;
            _context.Update(taskObj);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "WarTask", new {id=taskId});
        }
    }
}