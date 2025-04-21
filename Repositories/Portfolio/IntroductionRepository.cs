using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using MyPortfolio.Data;
using MyPortfolio.Models.Authentication;
using MyPortfolio.Models.Portfolio;
using MyPortfolio.Repositories.Portfolio;
namespace MyPortfolio.Repositories.Portfolio
{
    public class IntroductionRepository : IPortfolioRepository<Introduction>
    {
        private readonly MyPortfolioDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<IntroductionRepository> _logger;

        public IntroductionRepository(MyPortfolioDbContext dbContext, IHttpContextAccessor httpContextAccessor, ILogger<IntroductionRepository> logger)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        // Get _userId
        public string _userId => _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;


        public async Task<Introduction> GetAsync()
        {
            var introduction = await _dbContext.Introductions.FirstOrDefaultAsync(x => x.UserId == _userId);
            if(introduction == null)
            {
                return new Introduction();
            }
            return introduction;
        }

        public async Task<Introduction> PostAsync(Introduction newIntroduction)
        {
            var introduction = new Introduction
            {
                IntroductionId = Guid.NewGuid().ToString(),
                Avatar = newIntroduction.Avatar,
                FirstName = newIntroduction.FirstName,
                LastName = newIntroduction.LastName,
                Gender = newIntroduction.Gender,
                DOB = newIntroduction.DOB,
                Description = newIntroduction.Description,
                HouseNumber = newIntroduction.HouseNumber,
                Address = newIntroduction.Address,
                Major = newIntroduction.Major,
                UserId = _userId
            };

            await _dbContext.Introductions.AddAsync(introduction);
            await _dbContext.SaveChangesAsync();
            return introduction;    
        }

        public async Task<Introduction> PutAsync(Introduction updatedIntroduction)
        {
            var existingIntroduction = await _dbContext.Introductions
                .FirstOrDefaultAsync(x => x.UserId == _userId && x.IntroductionId == updatedIntroduction.IntroductionId);

            if (existingIntroduction == null)
            {
                throw new InvalidOperationException("Introduction not found for this user.");
            }

            // Update the properties of the existing introduction with the new values
            existingIntroduction.Avatar = updatedIntroduction.Avatar;
            existingIntroduction.FirstName = updatedIntroduction.FirstName;
            existingIntroduction.LastName = updatedIntroduction.LastName;
            existingIntroduction.Gender = updatedIntroduction.Gender;
            existingIntroduction.DOB = updatedIntroduction.DOB;
            existingIntroduction.Description = updatedIntroduction.Description;
            existingIntroduction.HouseNumber = updatedIntroduction.HouseNumber;
            existingIntroduction.Address = updatedIntroduction.Address;
            existingIntroduction.Major = updatedIntroduction.Major;

            // Update the entity in the DbContext
            _dbContext.Introductions.Update(existingIntroduction);
            await _dbContext.SaveChangesAsync();

            return existingIntroduction!;
        }

        public Task<ActionResult> DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }
    }
}
