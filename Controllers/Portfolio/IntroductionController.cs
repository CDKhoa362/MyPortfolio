using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Models.Authentication;
using MyPortfolio.Repositories.Portfolio;
using MyPortfolio.Models.Portfolio;
using Microsoft.AspNetCore.Authorization;
using MyPortfolio.Constants;

namespace MyPortfolio.Controllers.Portfolio
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = Roles.USER)]
    public class IntroductionController : ControllerBase
    {
        private readonly IPortfolioRepository<Introduction> _introduction;
        public IntroductionController(IPortfolioRepository<Introduction> introduction)
        {
            _introduction = introduction;
        }

        
        [HttpGet("GetIntroductionAsync")]
        public async Task<ActionResult> GetIntroductionAsync()
        {
            var userIntroduction = await _introduction.GetAsync();
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Get Introduction Successfully",
                Data = userIntroduction
            });
        }

        [HttpPost("CreateIntroductionAsync")]
        public async Task<ActionResult> CreateIntroductionAsync(Introduction newIntroduction)
        {
            var introduction = await _introduction.PostAsync(newIntroduction);
            return Ok(new ApiResponse
            {
                Success = true,
                Data = introduction
            });

        }

        [HttpPut("UpdateIntroductionAsync")]
        public async Task<ActionResult> UpdateIntroductionAsync(Introduction newIntroduction)
        {
            var introduction = await _introduction.PutAsync(newIntroduction);
            return Ok(new ApiResponse
            {
                Success = true,
                Data = introduction
            });
        }
    }
}
