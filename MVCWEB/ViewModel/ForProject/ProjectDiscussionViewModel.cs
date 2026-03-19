using MVCWEB.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace MVCWEB.ViewModel.ForProject
{
    public class ProjectDiscussionViewModel
    {
        [Required]
        public int? Topic_id { get; set; }
        public string? Title { get; set; }
        [Required]
        public int? Project_id { get; set; }
        public string? ProjectTitle { get; set; }
        public string? CreatedBy { get; set; }
        public string? Description { get; set; }
        public bool? IsClosed { get; set; }
        public DateTime? CreatedAt { get; set; }

        [Required(ErrorMessage = "Comment cannot be empty.")]
        [MinLength(6, ErrorMessage = "Comment must be at least 6 characters.")]
        public string Reply { get; set; } = string.Empty; // non-nullable

        public List<TopicMessages>? Messages { get; set; }
    }
}
