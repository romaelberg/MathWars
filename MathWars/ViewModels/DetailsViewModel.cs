using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using MathWars.Models;

namespace MathWars.ViewModels
{
    [NotMapped]
    public class DetailsViewModel
    {
        public IList<Tag> TaskTags;
        public IList<Comment> TaskComments;
        public string SolveStatus;
        public WarTask WarTaskModel;
        public IList<Image> TaskImages;
    }
}