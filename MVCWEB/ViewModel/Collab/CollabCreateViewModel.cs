using MVCWEB.Models.Entities;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System.ComponentModel.DataAnnotations;

namespace MVCWEB.ViewModel.Collab
{
    public class CollabCreateViewModel
    {
        [Required]
        [MinLength(16, ErrorMessage = "Title must be at least 16 characters.")]
        public string Title { get; set; }
        [Required]
        [MinLength(40, ErrorMessage = "Description must be at least 40 characters.")]
        public string Description { get; set; }
        [Required]
        [Range(12,36,ErrorMessage ="Size of members must be in range of 12-36.")]
        public int MemberSize { get; set; }

        //[Required(ErrorMessage = "Select atleast two category.")]


        public List<string> SelectedCategoryIds { get; set; } = new();
        public List<int> SelectedSkillIds { get; set; } = new();

        public List<Skills>? Skills { get; set; }
        public List<Categories>? Categories { get; set; }

    }
}
