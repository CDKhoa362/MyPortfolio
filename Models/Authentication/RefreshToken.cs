using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
namespace MyPortfolio.Models.Authentication
{
    public class RefreshToken
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        // User ID
        [ForeignKey("User")]
        public string UserId { get; set; } = null!;
        public IdentityUser? User { get; set; }

        public string Token { get; set; } = null!;
        public string JwtId { get; set; } = null!;

        public bool IsUsed { get; set; }
        public bool IsRevoked { get; set; }

        public DateTime IssueAt { get; set; }
        public DateTime ExpireAt { get; set; }



    }
}
