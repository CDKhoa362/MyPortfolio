using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using MyPortfolio.Constants;
using MyPortfolio.Models.Authentication;

namespace MyPortfolio.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenRepository _jwtTokenRepository;

        public AccountRepository(UserManager<IdentityUser> userManager,
                                 RoleManager<IdentityRole> roleManager,
                                 SignInManager<IdentityUser> signInManager,
                                 IJwtTokenRepository jwtTokenRepository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _jwtTokenRepository = jwtTokenRepository;
        }

        public async Task<IdentityResult> SignUpAsync(SignUp model)
        {
            var user = new IdentityUser
            {
                PhoneNumber = model.Phone,
                UserName = model.Email,
                Email = model.Email,
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                // Thêm roles mặc định là user khi đăng ký.
                await _userManager.AddToRoleAsync(user, Roles.USER);
            }

            return result;
        }
        
        public async Task<Tokens> SignInAsync(SignIn model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);

            // Generate Token.
            var token = await _jwtTokenRepository.GenerateTokenAsync(user);
            return token;
        }
    }
}
