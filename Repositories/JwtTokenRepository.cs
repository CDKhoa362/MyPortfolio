using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyPortfolio.DependencyInjection.Options;
using MyPortfolio.Models.Authentication;
using MyPortfolio.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MyPortfolio.Repositories
{
    public class JwtTokenRepository : IJwtTokenRepository
    {
        private readonly JwtOptions _jwtOptions = new JwtOptions();
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<JwtTokenRepository> _logger;
        private readonly MyPortfolioDbContext _dbContext;

        public JwtTokenRepository(IConfiguration configuration,
                                  UserManager<IdentityUser> userManager,
                                  MyPortfolioDbContext dbContext,
                                  ILogger<JwtTokenRepository> logger)
        {
            
            configuration.GetSection(nameof(JwtOptions)).Bind(_jwtOptions);
            _userManager = userManager;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Tokens> GenerateTokenAsync(IdentityUser user)
        {

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey)); // secret Key.
    
            var userRoles = await _userManager.GetRolesAsync(user); // User Role
            var roleClaims = userRoles.Select(role => new Claim(ClaimTypes.Role, role)).ToList(); // userRoles => RoleClaim List to concat Token Descriptor
            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("UserName", user.Email),
                    new Claim("Id", user.Id.ToString())
                }.Concat(roleClaims)),

                Issuer = _jwtOptions.Issuer,
                Audience = _jwtOptions.Audience,
                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha512Signature)
            };
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var token = jwtTokenHandler.CreateToken(tokenDescription);
            var accessToken = jwtTokenHandler.WriteToken(token);

            // Refresh token.
            var refreshToken = GenerateRefreshToken();
            var refreshTokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid().ToString(),
                JwtId = token.Id,
                UserId = user.Id,
                Token = refreshToken,
                IsUsed = false,
                IsRevoked = false,
                IssueAt = DateTime.UtcNow,
                ExpireAt = DateTime.UtcNow.AddHours(1)
            };

            await _dbContext.AddAsync(refreshTokenEntity);
            await _dbContext.SaveChangesAsync();

            return new Tokens
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            };
        }


        // Renew Token
        private string GenerateRefreshToken()
        {
            var random = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);
                return Convert.ToBase64String(random);
            }
        }

        public ClaimsPrincipal GetClaimsPrincipal(Tokens token, out SecurityToken validatedToken)
        {
            var tokenParams = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey))
            };
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var principal = jwtTokenHandler.ValidateToken(token.AccessToken, tokenParams, out validatedToken);

            return principal;
        }

        // Renew Token
        private DateTime ConvertUnixTimeToDateTime(long utcExpireDate)
        {
            var dateTimeInterval = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return dateTimeInterval.AddSeconds(utcExpireDate);
        }

        public async Task<ApiResponse> RenewTokenAsync(Tokens model, SecurityToken validateToken)
        {
            var principal = GetClaimsPrincipal(model, out var validatedToken);
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }
            
            // Check algorithm
            if (validateToken is JwtSecurityToken jwtSecurityToken)
            {
                var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase);
                if (!result)//false
                {
                    return (new ApiResponse
                    {
                        Success = false,
                        Message = "Invalid token"
                    });
                }

            }

            // Check Expired Token
            var utcExpireDate = long.Parse(principal.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
            if (ConvertUnixTimeToDateTime(utcExpireDate) > DateTime.UtcNow)
            {
                return (new ApiResponse
                {
                    Success = false,
                    Message = "Access token has not yet expired"
                });
            }

            //check 4: Check refreshtoken exist in DB
            var storedToken = _dbContext.RefreshTokens.FirstOrDefault(x => x.Token == model.RefreshToken);
            if (storedToken == null)
            {
                return (new ApiResponse
                {
                    Success = false,
                    Message = "Refresh token does not exist"
                });
            }

            //check 5: check refreshToken is used/revoked?
            if (storedToken.IsUsed)
            {
                return (new ApiResponse
                {
                    Success = false,
                    Message = "Refresh token has been used"
                });
            }
            if (storedToken.IsRevoked)
            {
                return (new ApiResponse
                {
                    Success = false,
                    Message = "Refresh token has been revoked"
                });
            }

            //check 6: AccessToken id == JwtId in RefreshToken
            var jti = principal.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
            if (storedToken.JwtId != jti)
            {
                return (new ApiResponse
                {
                    Success = false,
                    Message = "Token doesn't match"
                });
            }

            //Update token is used
            storedToken.IsRevoked = true;
            storedToken.IsUsed = true;
            _dbContext.Update(storedToken);
            await _dbContext.SaveChangesAsync();

            var user = await _dbContext.Users.SingleOrDefaultAsync(nd => nd.Id == storedToken.UserId);
            var token = await GenerateTokenAsync(user);


            return (new ApiResponse
            {
                Success = true,
                Message = "Token renewed successfully",
                Data = token
            });
        }
    }
}
