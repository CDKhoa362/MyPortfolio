using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Models.Portfolio;

namespace MyPortfolio.Repositories.Portfolio
{
    public interface IPortfolioRepository<T> where T : class
    {
        Task<T> GetAsync();
        Task<T> PostAsync(T newInfor);
        Task<T> PutAsync(Introduction updatedIntroduction);
        Task<ActionResult> DeleteAsync(string id);

    }
}
