using System.ComponentModel.DataAnnotations;

namespace MyPortfolio.Models.Authentication
{
    public class SignUp
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required, Phone]
        public string Phone { get; set; } = null!;

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Required, DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = null!;
    }
}
