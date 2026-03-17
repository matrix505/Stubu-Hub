namespace MVCWEB.Models.Entities
{
    public class TeamMembers
    {
        public int? Team_member_id { get; set; }
        public int? Project_id { get; set; }
        public int? User_id { get; set; }
        public string? Role { get; set; }
        public string? Fullname { get; set; }
        public DateTime? JoinedAt { get; set; }

        public string? Email { get; set; }
    }
}
