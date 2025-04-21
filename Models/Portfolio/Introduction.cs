using Microsoft.AspNetCore.Identity;

namespace MyPortfolio.Models.Portfolio
{
    public class Introduction
    {
        public string IntroductionId { get; set; } = null!;
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
        public byte[]? Avatar { get; set; }

        // USER
        public string UserId { get; set; } = null!;
        public IdentityUser? User { get; set; }
    }
}
