namespace MVCWEB.Models.Entities
{
    public class Project
    {
        public int Project_id { get; set; }
        public int Owner_id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int? MemberSize { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? Status { get; set; }

        public string? OwnerName { get; set; }
        public int? TotalMembers {  get; set; }
        public string? CategoryNames { get; set; } = string.Empty;
        public string? MemberNames { get; set; } = string.Empty;

        public IEnumerable<ProjectCategories>? Categories {  get; set; }
        public IEnumerable<ProjectSkills>? Skills { get; set; }


    }
}
