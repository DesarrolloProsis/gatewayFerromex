using ApiGateway.Data;
using ApiGateway.Interfaces;
using ApiGateway.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace ApiGateway.Services
{
    public class UserRegisterEventHandler : IRequestHandler<UserCreateCommand, IdentityResult>
    {
        private readonly BackOfficeFerromexContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogUserActivity _logUserInsertion;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserRegisterEventHandler(BackOfficeFerromexContext dbContext, ILogUserActivity logUserActivity, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
            _logUserInsertion = logUserActivity;
            _httpContextAccessor = httpContextAccessor;
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
                Active = true
            };
            entry.Email = entry.UserName + "@mail.com";                      

            var res = await _userManager.CreateAsync(entry, createCommand.Password);
            if (res.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(entry.UserName);
                res = await _userManager.AddToRoleAsync(user, createCommand.RoleName);                               
                var idRole = await _roleManager.FindByNameAsync(createCommand.RoleName);
                //_logUserInsertion.InsertarLogCreateUser(entry, createCommand, idRole.Id);
                await _dbContext.SaveChangesAsync();
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
