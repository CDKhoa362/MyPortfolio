using Microsoft.AspNetCore.Identity;
using MyPortfolio.Models.Account;

namespace MyPortfolio.Repositories
{
    public interface IAccountRepository
    {
        public Task<IdentityResult> SignUpAsync(SignUp model);
        public Task<string> SignInAsync(SignIn model);
    }
}
