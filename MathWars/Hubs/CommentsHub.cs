using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MathWars.Data;
using MathWars.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace MathWars.Hubs
{
    public class CommentsHub : Hub
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        public CommentsHub(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task Send(string message)
        {
            await this.Clients.All.SendAsync("Send", message);
        }
        
        public async Task<AppUser> GetLoggedUser() => await _userManager.FindByIdAsync(_userManager.GetUserId(this.Context.User));

        public List<Like> GetUserLikes(AppUser user) =>
            (from l in _context.Likes where l.User == user select l).ToList();
        public List<Dislike> GetUserDislikes(AppUser user) =>
            (from d in _context.Dislikes where d.User == user select d).ToList();
        
         public async Task LikeComment(string id)
         {
             int commentId = Int32.Parse(id);
             var user = await GetLoggedUser();
             if (user == null) return;
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null) return;
            var userLikes = GetUserLikes(user)
                .Where(l => l.Comment == comment).ToList();
            var userDislikes = GetUserDislikes(user)
                .Where(d => d.Comment == comment).ToList();
            if (userLikes.Count != 0)
            {
                await DeleteLike(userLikes);
                await this.Clients.All.SendAsync("DislikeComment", comment.Id, comment.Likes.Count, comment.Dislikes.Count);
                return;
            }
            
            if (userDislikes.Count != 0)
            {
                await DeleteDislike(comment, userDislikes);
            }
            await PutLike(comment);
            await this.Clients.All.SendAsync("ReceiveLike", commentId, comment.Likes.Count, comment.Dislikes.Count ,user.Id.ToString() , true);
        }

        public async Task PutLike(Comment comment)
        {
            _context.Likes.Add(new Like {User = await GetLoggedUser(), Comment = comment});
            await _context.SaveChangesAsync();
        }
        public async Task PutDislike(Comment comment)
        {
            _context.Dislikes.Add(new Dislike {User = await GetLoggedUser(), Comment = comment});
            await _context.SaveChangesAsync();
        }
        
        public async Task DeleteLike(List<Like> likes)
        {
            _context.Likes.RemoveRange(likes);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteDislike(Comment comment, List<Dislike> dislikes)
        {
            _context.Dislikes.RemoveRange(dislikes);
            await _context.SaveChangesAsync();
        }

        public async Task DislikeComment(string id)
        {
            if (id == null) return;
            int commentId = Int32.Parse(id);
            var user = await GetLoggedUser();
            if (user == null) return;
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null) return;
            var userLikes = GetUserLikes(await GetLoggedUser())
                .Where(l => l.Comment == comment).ToList();
            var userDislikes = GetUserDislikes(await GetLoggedUser())
                .Where(d => d.Comment == comment).ToList();
            if (userLikes.Count != 0)
            {
                await DeleteLike(userLikes);
            }

            if (userDislikes.Count != 0)
            {
                await DeleteDislike(comment, userDislikes);
                await this.Clients.All.SendAsync("RemoveDislikeComment", comment.Id, comment.Dislikes.Count);
                return;
            }
            await PutDislike(comment);
            await this.Clients.All.SendAsync("ReceiveDislike", commentId, comment.Dislikes.Count, comment.Likes.Count, user.Id.ToString() , true);
        }

        private IActionResult NotFound()
        {
            throw new NotImplementedException();
        }
        
        
        public async Task AddComment(string taskId, string body)
        {
            if (String.IsNullOrEmpty(body)) return;
            int warTaskId = Int32.Parse(taskId);
            var warTask = _context.WarTasks.FirstOrDefault(t => t.Id == warTaskId);
            // if (warTask == null) return;
            var user = await _userManager.FindByIdAsync(_userManager.GetUserId(this.Context.User));
            if (user == null) return;
            var newComment = new Comment();
            newComment.Body = body;
            newComment.WarTask = warTask;
            newComment.Author = user;
            newComment.TaskId = warTask.Id;
            newComment.Created = DateTime.UtcNow.AddHours(3);
            _context.Comments.Add(newComment);
            await _context.SaveChangesAsync();
            await this.Clients.All.SendAsync("ReceiveComment", user.UserName, user.Id, body, newComment.Created, newComment.TaskId, newComment.Id);
        }
    }
}