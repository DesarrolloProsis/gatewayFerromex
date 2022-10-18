using ApiGateway.Data;
using ApiGateway.Interfaces;
using ApiGateway.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace ApiGateway.Services
{
    public class UsuariosService : IUsuariosService
    {
        private readonly BackOfficeFerromexContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogUserActivity _logUserInsertion;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogRolInsertion _logRolInsertion;

        public UsuariosService(ILogRolInsertion logRolInsertion, BackOfficeFerromexContext dbContext, IHttpContextAccessor httpContextAccessor, ILogUserActivity logUserActivity, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _logRolInsertion = logRolInsertion;
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _logUserInsertion = logUserActivity;
            _httpContextAccessor =  httpContextAccessor;
        }

        public async Task<int> CountRolesAsync(string nombreRol, bool? estatus)
        {
            IQueryable<AspNetRole>? res = _dbContext.AspNetRoles;

            if (!string.IsNullOrWhiteSpace(nombreRol))
            {
                res = res.Where(x => x.NormalizedName.Contains(nombreRol.Trim().ToUpper()));
            }
            if (estatus != null)
            {
                res = res.Where(x => x.Active == estatus);
            }
            return await res.CountAsync();
        }

        public async Task<int> CountUsuariosAsync(string nombre, bool? estatus)
        {
            IQueryable<AspNetUser>? res = _dbContext.AspNetUsers;

            if (!string.IsNullOrWhiteSpace(nombre))
            {
                res = res.Where(x => x.Name.Contains(nombre.ToUpper().Trim()));
            }
            if (estatus != null)
            {
                res = res.Where(x => x.Active == estatus);
            }
            Console.WriteLine(res.ToQueryString());
            return await res.CountAsync();
        }

        public Task<Usuario> CreateUsuarioAsync(NuevoUsuario nuevoUsuario)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Rol>> GetRolesAsync(int? paginaActual, int? numeroDeFilas, string? nombreRol, bool? estatus)
        {
            IQueryable<AspNetRole>? res = _dbContext.AspNetRoles;

            if (!string.IsNullOrWhiteSpace(nombreRol))
            {
                res = res.Where(x => x.NormalizedName.Contains(nombreRol.ToUpper().Trim()));
            }
            if (estatus != null)
            {
                res = res.Where(x => x.Active == estatus);
            }
            if (paginaActual != null && numeroDeFilas != null)
            {
                res = res.Skip((int)((paginaActual - 1) * numeroDeFilas)).Take((int)numeroDeFilas);
            }
            var aspNetRoles = await res.ToListAsync();
            List<Rol> roles = new();
            foreach (var role in aspNetRoles)
            {
                roles.Add(new() { IdRol = role.Id, NombreRol = role.Name, Estatus = role.Active });
            }
            return roles;
        }

        public async Task<List<Usuario>> GetUsuariosAsync(int? paginaActual, int? numeroDeFilas, string? nombre, bool? estatus)
        {
            IQueryable<AspNetUser>? res = _dbContext.AspNetUsers;

            if (!string.IsNullOrWhiteSpace(nombre))
            {
                res = res.Where(x => x.Name.Contains(nombre.ToUpper().Trim()));
            }
            if (estatus != null)
            {
                res = res.Where(x => x.Active == estatus);
            }
            if (paginaActual != null && numeroDeFilas != null)
            {
                res = res.Skip((int)((paginaActual - 1) * numeroDeFilas)).Take((int)numeroDeFilas);
            }
            var aspNetUsers = await res.ToListAsync();

            List<Usuario> usuarios = new();                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 
            foreach (var user in aspNetUsers)
            {
                var applicationUser = await _userManager.FindByIdAsync(user.Id);
                var roles = await _userManager.GetRolesAsync(applicationUser);
                usuarios.Add(new()
                {
                    UsuarioId = user.Id,
                    NombreUsuario = user.UserName,
                    Nombre = user.Name,
                    Apellidos = user.LastName,
                    NombreCompleto = user.Name + " " + user.LastName,
                    Rol = roles.FirstOrDefault(),
                    Estatus = user.Active
                });
            }
            return usuarios;
        }

        public async Task<bool> UpdateRoleAsync(Rol rol)
        {
            IdentityRole roleOld = await _roleManager.FindByIdAsync(rol.IdRol);

            var entity = await _dbContext.AspNetRoles.FirstOrDefaultAsync(e => e.Id == rol.IdRol);
            if(entity == null) return false;

            entity.Name = rol.NombreRol.Trim();
            entity.NormalizedName = rol.NombreRol.Trim().ToUpper();
            entity.Active = rol.Estatus;

            _dbContext.Entry(entity).State = EntityState.Modified;            

            try
            {
                await _dbContext.SaveChangesAsync();
                await _logRolInsertion.InsertLogEditRole(rol, roleOld);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            return true;
        }

        public async Task<bool> UpdateUsuarioAsync(Usuario usuario)
        {
            var entity = await _dbContext.AspNetUsers.FirstOrDefaultAsync(e => e.Id == usuario.UsuarioId);
            if (entity == null) return false;

            _logUserInsertion.InsertarLogEditUser(usuario, entity);

            entity.UserName = usuario.NombreUsuario.Trim();
            entity.NormalizedUserName = usuario.NombreUsuario.Trim().ToUpper();
            entity.Name = usuario.Nombre;
            entity.LastName = usuario.Apellidos;
            entity.Active = usuario.Estatus;     

            if (!string.IsNullOrWhiteSpace(usuario.Rol) && await _roleManager.RoleExistsAsync(usuario.Rol))
            {
                ApplicationUser user = await _userManager.FindByIdAsync(entity.Id);
                var roles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, roles);
                await _userManager.AddToRoleAsync(user, usuario.Rol);
            }

            _dbContext.Entry(entity).State = EntityState.Modified;

            try
            {                
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            return true;
        }

        public async Task<bool> UpdateUsuarioPasswordAsync(UsuarioUpdatePassword usuario)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(usuario.UsuarioId);
            var entity = await _dbContext.AspNetUsers.FirstOrDefaultAsync(e => e.Id == usuario.UsuarioId);

            if (user == null) return false;

            await _userManager.RemovePasswordAsync(user);
            await _userManager.AddPasswordAsync(user, usuario.Password);
            _logUserInsertion.InsertarLogUpdatePass(user, usuario, entity);
            await _dbContext.SaveChangesAsync();

            return true;
        }
    }

    public class LogUserInsertion : ILogUserActivity
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<LogUserInsertion> _logger;
        private readonly BackOfficeFerromexContext _dbContext;

        public LogUserInsertion(IHttpContextAccessor httpContextAccessor, BackOfficeFerromexContext dbContext, ILogger<LogUserInsertion> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _dbContext = dbContext;
        }

        public void InsertarLogEditUser(Usuario usuario, AspNetUser aspNetUser)
        {
            try
            {
                var idHttpContext = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier).Value;
                if (usuario.Nombre != null)
                {
                    LogUserActivity logUser = new LogUserActivity()
                    {
                        IdModifiedUser = idHttpContext,
                        UpdatedDate = DateTime.Now,
                        IdUpdatedUser = aspNetUser.Id,
                        TypeAction = "8",
                        OldName = aspNetUser.Name,
                        NewName = usuario.Nombre,
                        OldLastName = aspNetUser.LastName,
                        NewLastName = usuario.Apellidos,
                        Active = usuario.Estatus,
                    };
                    _dbContext.LogUserActivities.Add(logUser);
                }
                else
                {

                    LogUserActivity logUser = new LogUserActivity()
                    {
                        IdModifiedUser = idHttpContext,
                        UpdatedDate = DateTime.Now,
                        IdUpdatedUser = aspNetUser.Id,
                        TypeAction = "7",
                        Active = usuario.Estatus,

                    };
                    _dbContext.LogUserActivities.Add(logUser);
                }                               
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
        public void InsertarLogUpdatePass(ApplicationUser usuario, UsuarioUpdatePassword usuarioPass, AspNetUser aspNetUser)
        {
            try
            {
                var idHttpContext = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier).Value;
                LogUserActivity logUser = new LogUserActivity()
                {
                    IdModifiedUser = idHttpContext,
                    UpdatedDate = DateTime.Now,
                    IdUpdatedUser = usuario.Id,
                    TypeAction = "9",
                    OldPass = usuario.PasswordHash,
                    NewePass = usuarioPass.Password,
                };
                _dbContext.LogUserActivities.Add(logUser);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        public void InsertarLogCreateUser(ApplicationUser usuario, UserCreateCommand usuarioPass, string? idRole)
        {
            try
            {
                var idHttpContext = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier).Value;
                var logUser = new LogUserActivity()
                {
                    IdModifiedUser = idHttpContext,
                    UpdatedDate = DateTime.Now,
                    IdUpdatedUser = usuario.Id,
                    TypeAction = "6",
                    AspNetRolesIdNew = idRole,
                    NewName = usuario.Name,
                    NewLastName = usuario.LastName,
                    NewePass = usuarioPass.Password,
                    Active = true
                };
                _dbContext.LogUserActivities.Add(logUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

    }
}
