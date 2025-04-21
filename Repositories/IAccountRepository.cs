using Microsoft.AspNetCore.Identity;
using MyPortfolio.Models.Authentication;

namespace MyPortfolio.Repositories
{
    public interface IAccountRepository
    {
        Task<IdentityResult> SignUpAsync(SignUp model);
        Task<Tokens> SignInAsync(SignIn model);
    }
}
