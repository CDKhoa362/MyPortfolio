using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using MyPortfolio.Models.Authentication;
using Microsoft.IdentityModel.Tokens;
namespace MyPortfolio.Repositories
{
    public interface IJwtTokenRepository
    {
        Task<Tokens> GenerateTokenAsync(IdentityUser user);
        Task<ApiResponse> RenewTokenAsync(Tokens model, SecurityToken securityToken);
        ClaimsPrincipal GetClaimsPrincipal(Tokens token, out SecurityToken validatedToken);

    }
}
