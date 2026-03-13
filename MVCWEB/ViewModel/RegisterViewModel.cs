using System.ComponentModel.DataAnnotations;

namespace MVCWEB.ViewModel
{
    public class RegisterViewModel
    {
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        [Required]
        public string? Username { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string? ConfirmPassword {  get; set; }
        [Required]
        public DateOnly Birthdate { get; set; }
        public string? Country { get; set; }

    }
}
