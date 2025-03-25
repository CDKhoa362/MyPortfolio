using MyPortfolio.Models;
namespace MyPortfolio.ViewModels
{
    public class IntroductionViewModel
    {
        public string? FirstName { get; set; } = null!;
        public string? LastName { get; set; } = null!;
        public DateOnly? DOB { get; set; } = null!;
        public bool Gender { get; set; } = true;
        public string Major { get; set; } = null!;
        public string Description { get; set; } = null!;

        // ADDRESS
        public string? HouseNumber { get; set; } = null!;
        public string? Address { get; set; } = null!;

        // AVATAR
        public IFormFile? Avatar { get; set; }
    }
}
