using MVCWEB.Models.Entities;

namespace MVCWEB.ViewModel.ForProject
{
    public class ProjectMainViewModel
    {
        public int? Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? Status { get; set; }
        public int? MemberSize { get; set; }
        public int? TotalMembers { get; set; }
        public DateTime? CreatedAt { get; set; }
        public List<string> Categories { get; set; } = null!;
        public List<string>? Skills { get; set; }
        public List<TeamMembers>? Members { get; set; }
        public bool IsUserProjectOwner { get; set; }
        public bool IsUserProjectMember { get; set; }
        public string? tab { get; set; } 
    }
}
