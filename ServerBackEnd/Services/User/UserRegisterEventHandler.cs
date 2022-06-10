using ApiGateway.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ApiGateway.Services
{
    public class UserRegisterEventHandler : IRequestHandler<UserCreateCommand, IdentityResult>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserRegisterEventHandler(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IdentityResult> Handle(UserCreateCommand createCommand, CancellationToken cancellationToken)
        {
            var roleExists = await _roleManager.RoleExistsAsync(createCommand.RoleName);
            if (!roleExists)
                return IdentityResult.Failed(_userManager.ErrorDescriber.InvalidRoleName(createCommand.RoleName));
            var entry = new ApplicationUser
            {
                UserName = createCommand.Nombre[..3] + Regex.Replace(createCommand.Apellidos, @"\s+", ""),
                Name = createCommand.Nombre,
                LastName = createCommand.Apellidos,
                Matricule = 1,
                Active = true
            };
            entry.Email = entry.UserName + "@mail.com";
            var res = await _userManager.CreateAsync(entry, createCommand.Password);
            if (res.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(entry.UserName);
                res = await _userManager.AddToRoleAsync(user, createCommand.RoleName);
            }

            return res;
        }
    }

    public class UserCreateCommand : IRequest<IdentityResult>
    {
        [Required]
        public string Password { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string RoleName { get; set; }
    }
}
