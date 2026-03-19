namespace MVCWEB.Models.Entities
{
    public class Discussions
    {
        public int? Topic_id { get; set; }
        public int? Project_id { get; set; }
        public int? Creator_id { get; set; }
        public string? CreatorName { get; set; }
        public string? Title { get; set; }
        public string? ProjectTitle { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; }

    }
}
