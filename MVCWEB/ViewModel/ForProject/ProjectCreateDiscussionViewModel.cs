using System.ComponentModel.DataAnnotations;

namespace MVCWEB.ViewModel.ForProject
{
    public class ProjectCreateDiscussionViewModel
    {
        [Required]
        public int ProjectId { get; set; }
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, MinimumLength = 10, ErrorMessage = "Title must be between 10 and 100 characters.")]
        [Display(Name = "Discussion Title")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(600, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 600 characters.")]
        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; }

        [Required]
        public string ProjectTitle { get; set; }

    }
}