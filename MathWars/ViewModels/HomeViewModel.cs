using System.Collections.Generic;
using MathWars.Models;


namespace MathWars.ViewModels
{
    public class HomeViewModel
    {
        public IList<WarTask> TopRatedTasks;
        public IList<WarTask> LatestTasks;
        public IList<Tag> Tags;
    }
}