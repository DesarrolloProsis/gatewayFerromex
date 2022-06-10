using Microsoft.AspNetCore.Identity;

namespace ApiGateway.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? RefreshToken { get; set; }
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public int Matricule { get; set; }
        public bool? Active { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}