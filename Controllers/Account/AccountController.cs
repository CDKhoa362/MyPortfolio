using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Models.Account;
using MyPortfolio.Repositories;

namespace MyPortfolio.Controllers.Account
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly IAccountRepository _accountRepository;
        public AccountController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        [HttpPost("SignUp")]
        public async Task<ActionResult> SignUp(SignUp model)
        {
            var result = await _accountRepository.SignUpAsync(model);
            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest();

        }

        [HttpPost("SignIn")]
        public async Task<ActionResult> SignIn(SignIn model)
        {
            var result = await _accountRepository.SignInAsync(model);
            if (string.IsNullOrEmpty(result))
            {
                return Unauthorized();
            }
            return Ok(result);
        }

    }
}
