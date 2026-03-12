using System.ComponentModel.DataAnnotations;

namespace MVCWEB.ViewModel
{
    public class RegisterViewModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string ConfirmPassword {  get; set; }
        [Required]
        public DateOnly Birthdate { get; set; }

    }
}
