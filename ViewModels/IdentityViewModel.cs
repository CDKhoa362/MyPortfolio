using Azure.Identity;
using Microsoft.AspNetCore.Identity;

namespace MyPortfolio.ViewModels
{
    public class IdentityViewModel
    {
        public string UserName = null!;
        public string Email = null!;
        public string PhoneNumber = null!;
    }
}
