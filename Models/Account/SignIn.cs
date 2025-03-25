using System.ComponentModel.DataAnnotations;

namespace MyPortfolio.Models.Account
{
    public class SignIn
    {
        [Required, EmailAddress]   
        
        public string Email { get; set; } = null!;

        [Required, DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        public string Password { get; set; } = null!;
    }
}
