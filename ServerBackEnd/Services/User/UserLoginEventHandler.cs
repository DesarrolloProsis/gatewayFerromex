using ApiGateway.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

namespace ApiGateway.Services
{
    public class UserLoginEventHandler : IRequestHandler<UserLoginCommand, IdentityAccess>
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public UserLoginEventHandler(SignInManager<ApplicationUser> signInManager,
                                     UserManager<ApplicationUser> userManager,
                                     IConfiguration configuration)
        {
            _signInManager = signInManager;
            _configuration = configuration;
            _userManager = userManager;
        }

        public async Task<IdentityAccess> Handle(UserLoginCommand loginCommand, CancellationToken cancellationToken)
        {
            var result = new IdentityAccess
            {
                Succeeded = false
            };

            if (loginCommand.UserName == null)
            {
                result.Error = "invalid_request";
                result.ErrorDescription = "usuario invalido";
                return result;
            }
            ApplicationUser? user = null;
            if (loginCommand.UserName != null) user = await _userManager.FindByEmailAsync(loginCommand.UserName);
            if (loginCommand.UserName != null && user == null) user = await _userManager.FindByNameAsync(loginCommand.UserName);
            if (user == null)
            {
                result.Error = "invalid_request";
                result.ErrorDescription = "usuario o password invalido";
                return result;
            }

            if (loginCommand.RefreshToken != null && !loginCommand.RefreshToken.Equals(user.RefreshToken) || loginCommand.RefreshToken != null && user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                result.Error = "unauthorized_client";
                result.ErrorDescription = "refresh_token invalido";
                return result;
            }
            if (loginCommand.Password != null)
            {
                if (!(await _signInManager.CheckPasswordSignInAsync(user, loginCommand.Password, false)).Succeeded)
                {
                    result.Error = "invalid_request";
                    result.ErrorDescription = "usuario o password invalido";
                    return result;
                }
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            }

            result.Succeeded = true;
            result.User = user;
            //result.Scope = loginCommand.Scope;
            if (!loginCommand.IsRfc6749)
            {
                result.ClaimsPrincipal = await _signInManager.CreateUserPrincipalAsync(user);
                CreateJwtToken(user, result);
                if(loginCommand.Scope == "offline_access") CreateRefreshToken(user, result);
                await _userManager.UpdateAsync(user);
            }
            return result;
        }

        private void CreateJwtToken(ApplicationUser user, IdentityAccess identity)
        {
            var secretKey = _configuration.GetValue<string>("SecretKey");
            var key = Encoding.ASCII.GetBytes(secretKey);
            IDictionary<string, object> claims = new Dictionary<string, object>();
            claims.Add("nombreCompleto", $"{user.Name} {user.LastName}");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                
                Subject = new ClaimsIdentity(identity.ClaimsPrincipal?.Claims, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme),
                Claims = claims,
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var createdToken = tokenHandler.CreateToken(tokenDescriptor);

            identity.TokenType = "Bearer";
            identity.Expires = (int)Math.Truncate((tokenDescriptor.Expires - DateTime.Now).Value.TotalSeconds);
            identity.AccessToken = tokenHandler.WriteToken(createdToken);
        }

        private static void CreateRefreshToken(ApplicationUser user, IdentityAccess identity)
        {
            using var rngCryptoServiceProvider = RandomNumberGenerator.Create();
            var randomBytes = new byte[512];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            var refreshToken = Convert.ToBase64String(randomBytes);

            user.RefreshToken = refreshToken;
            identity.RefreshTokenExpiryTime = user.RefreshTokenExpiryTime;
            identity.RefreshToken = refreshToken;
        }
    }

    public class UserLoginCommand : IRequest<IdentityAccess>
    {
        [JsonPropertyName("grant_type")]
        public string? GrantType { get; set; }
        public string? UserName { get; set; }
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        public string? Scope { get; set; }
        [JsonPropertyName("client_id")]
        public string? ClientId { get; set; }
        [JsonPropertyName("client_secret")]
        public string? ClientSecret { get; set; }
        [JsonIgnore]
        public string? RefreshToken { get; set; }
        [JsonIgnore]
        public bool IsRfc6749 { get; set; }
    }

    public class IdentityAccess
    {
        [JsonPropertyName("access_token")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? AccessToken { get; set; }

        [JsonPropertyName("token_type")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? TokenType { get; set; }

        [JsonPropertyName("expires_in")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int Expires { get; set; }

        [JsonPropertyName("scope")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Scope { get; set; }

        [JsonPropertyName("error")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Error { get; set; }

        [JsonPropertyName("error_description")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ErrorDescription { get; set; }

        [JsonIgnore]
        public bool Succeeded { get; set; }

        [JsonIgnore]
        public ApplicationUser? User { get; set; }

        [JsonIgnore]
        public ClaimsPrincipal? ClaimsPrincipal { get; set; }

        [JsonPropertyName("refresh_token")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? RefreshToken { get; set; }
        [JsonIgnore]
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }

}
