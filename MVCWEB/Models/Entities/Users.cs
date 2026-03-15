namespace MVCWEB.Models.Entities
{
    public class Users
    {
        public int User_id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? HashedPassword { get; set; }
        public bool? IsEmailVerified { get; set; }
        public DateOnly? BirthDate { get; set; }
        public string? Country { get; set; }
        public string? AuthorizationRole { get; set; }
        public string? ProfilePath { get; set; }
        public DateTime? CreatedAt { get; set; }

        

    }
}
