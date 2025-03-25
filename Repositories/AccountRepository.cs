using Microsoft.AspNetCore.Identity;
using MyPortfolio.Models.Account;
using MyWebsite.Repositories;
using Microsoft.Extensions.Configuration;

using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MyPortfolio.Constants;

namespace MyPortfolio.Repositories
{
    public class AccountRepository : IAccountRepository
    {   
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        public AccountRepository(UserManager<IdentityUser> userManager,
                                 SignInManager<IdentityUser> signInManager,
                                 RoleManager<IdentityRole> roleManager,
                                 IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public async Task<IdentityResult> SignUpAsync(SignUp model)
        {
            var user = new IdentityUser
            {
                PhoneNumber = model.Phone,
                UserName = model.Email,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, Roles.USER);
            }

            return result; 
        }

        public async Task<string> SignInAsync(SignIn model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return string.Empty;
            }

            // Kiểm tra password hợp lệ
            var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!passwordValid)
            {
                return string.Empty;
            }

            // Tạo danh sách Claim
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Thêm Role vào danh sách Claim
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }


            var jwtSecret = _configuration["JWT:Secret"];
            if (string.IsNullOrEmpty(jwtSecret))
            {
                throw new InvalidOperationException("JWT Secret is not configured.");
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(20),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
