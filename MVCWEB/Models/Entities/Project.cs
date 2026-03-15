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

    }
}
