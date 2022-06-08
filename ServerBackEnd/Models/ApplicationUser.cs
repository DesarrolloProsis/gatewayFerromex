﻿using Microsoft.AspNetCore.Identity;

namespace ApiGateway.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? RefreshToken { get; set; }
        public string? Nombre { get; set; }
        public string? Apellidos { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}