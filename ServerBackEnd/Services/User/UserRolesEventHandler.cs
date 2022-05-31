using ApiGateway.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ApiGateway.Services
{
    public class UserAddRolesEventHandler : IRequestHandler<UserAddRolesCommand, IdentityResult>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserAddRolesEventHandler(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IdentityResult> Handle(UserAddRolesCommand addRolesCommand, CancellationToken cancellationToken)
        {
            var res = IdentityResult.Failed();
            var user = await _userManager.FindByIdAsync(addRolesCommand.UserId);
            if (user == null)
            {
                res = IdentityResult.Failed(_userManager.ErrorDescriber.InvalidUserName(addRolesCommand.UserId));
                return res;
            }

            if (addRolesCommand.Role != null)
            {
                var roleExists = await _roleManager.RoleExistsAsync(addRolesCommand.Role);
                if (!roleExists)
                {
                    res = IdentityResult.Failed(_userManager.ErrorDescriber.InvalidRoleName(addRolesCommand.Role));
                    return res;
                }

                res = await _userManager.AddToRoleAsync(user, addRolesCommand.Role);
            }

            if (addRolesCommand.Roles != null)
            {
                foreach (var role in addRolesCommand.Roles)
                {
                    var roleExists = await _roleManager.RoleExistsAsync(role);
                    if (!roleExists)
                    {
                        res = IdentityResult.Failed(_userManager.ErrorDescriber.InvalidRoleName(role));
                        return res;
                    }
                }
                res = await _userManager.AddToRolesAsync(user, addRolesCommand.Roles);
            }

            return res;
        }
    }

    public class UserRemoveRolesEventHandler : IRequestHandler<UserRemoveRolesCommand, IdentityResult>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserRemoveRolesEventHandler(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IdentityResult> Handle(UserRemoveRolesCommand removeRolesCommand, CancellationToken cancellationToken)
        {
            var res = IdentityResult.Failed();
            var user = await _userManager.FindByIdAsync(removeRolesCommand.UserId);
            if (user == null)
            {
                res = IdentityResult.Failed(_userManager.ErrorDescriber.InvalidUserName(removeRolesCommand.UserId));
                return res;
            }

            if (removeRolesCommand.Role != null)
            {
                var roleExists = await _roleManager.RoleExistsAsync(removeRolesCommand.Role);
                if (!roleExists)
                {
                    res = IdentityResult.Failed(_userManager.ErrorDescriber.InvalidRoleName(removeRolesCommand.Role));
                    return res;
                }

                res = await _userManager.RemoveFromRoleAsync(user, removeRolesCommand.Role);
            }

            if (removeRolesCommand.Roles != null)
            {
                foreach (var role in removeRolesCommand.Roles)
                {
                    var roleExists = await _roleManager.RoleExistsAsync(role);
                    if (!roleExists)
                    {
                        res = IdentityResult.Failed(_userManager.ErrorDescriber.InvalidRoleName(role));
                        return res;
                    }
                }
                res = await _userManager.RemoveFromRolesAsync(user, removeRolesCommand.Roles);
            }
            return res;
        }
    }

    public class AddRolesEventHandler : IRequestHandler<AddRolesCommand, IdentityResult>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AddRolesEventHandler(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IdentityResult> Handle(AddRolesCommand removeRolesCommand, CancellationToken cancellationToken)
        {
            var res = IdentityResult.Failed();

            if (removeRolesCommand.RoleName != null)
            {
                var roleExists = await _roleManager.RoleExistsAsync(removeRolesCommand.RoleName);
                if (roleExists)
                {
                    res = IdentityResult.Failed(_userManager.ErrorDescriber.DuplicateRoleName(removeRolesCommand.RoleName));
                    return res;
                }
                res = await _roleManager.CreateAsync(new IdentityRole(removeRolesCommand.RoleName));
            }

            if (removeRolesCommand.RoleNames != null)
            {
                foreach (var role in removeRolesCommand.RoleNames)
                {
                    var roleExists = await _roleManager.RoleExistsAsync(role);
                    if (roleExists)
                    {
                        res = IdentityResult.Failed(_userManager.ErrorDescriber.DuplicateRoleName(role));
                        return res;
                    }
                    res = await _roleManager.CreateAsync(new IdentityRole(role));
                }
                foreach (var role in removeRolesCommand.RoleNames)
                {
                    res = await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }
            return res;
        }
    }

    public class UserAddRolesCommand : IRequest<IdentityResult>
    {
        public string UserId { get; set; }
        public string? Role { get; set; }
        public IEnumerable<string>? Roles { get; set; }
    }
    public class UserRemoveRolesCommand : IRequest<IdentityResult>
    {
        public string UserId { get; set; }
        public string? Role { get; set; }
        public IEnumerable<string>? Roles { get; set; }
    }
    public class AddRolesCommand : IRequest<IdentityResult>
    {
        public string? RoleName { get; set; }
        public IEnumerable<string>? RoleNames { get; set; }
    }
}
