using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyPortfolio.Models.Authentication;
using MyPortfolio.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace MyPortfolio.Controllers.Account
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly IJwtTokenRepository _jwtTokenRepository;
        private readonly IAccountRepository _accountRepository;

        public AccountController(IJwtTokenRepository jwtTokenRepository,
                                 IAccountRepository accountRepository)
        {
            _jwtTokenRepository = jwtTokenRepository;
            _accountRepository = accountRepository;
        }

        [HttpPost("SignUp")]
        public async Task<ActionResult> SignUp(SignUp model)
        {
            var result = await _accountRepository.SignUpAsync(model);
            if (result.Succeeded)
            {
                return Ok(result.Succeeded);
            }
            return StatusCode(500);

        }

        [HttpPost("SignIn")]
        public async Task<ActionResult> SignIn(SignIn model)
        {
            var result = await _accountRepository.SignInAsync(model);
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Successful Authentication",
                Data = result
            });

        }

        [HttpPost("RenewToken")]
        public async Task<IActionResult> RenewToken(Tokens model)
        {
            // Check valid format token
            SecurityToken validateToken;
            var principal = _jwtTokenRepository.GetClaimsPrincipal(model, out validateToken);
            var response = await _jwtTokenRepository.RenewTokenAsync(model, validateToken);
            return Ok(response);
        }

    }
}
