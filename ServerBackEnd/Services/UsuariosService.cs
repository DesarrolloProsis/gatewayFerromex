using ApiGateway.Data;
using ApiGateway.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace ApiGateway.Services
{
    public class UsuariosService : IUsuariosService
    {
        private readonly BackOfficeFerromexContext _dbContext;

        public UsuariosService(BackOfficeFerromexContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<int> CountRolesAsync(string nombreRol, bool? estatus)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountUsuariosAsync(string nombre, bool? estatus)
        {
            throw new NotImplementedException();
        }

        public Task<List<Rol>> CreateUsuarioAsync(NuevoUsuario nuevoUsuario)
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
                //res = res.Where(x => x. == estatus);
            }
            if (paginaActual != null && numeroDeFilas != null)
            {
                res = res.Skip((int)((paginaActual - 1) * numeroDeFilas)).Take((int)numeroDeFilas);
            }
            var aspNetRoles = await res.ToListAsync();
            List<Rol> roles = new();
            foreach (var role in aspNetRoles)
            {
                roles.Add(new() { IdRol = role.Id, NombreRol = role.Name, Estatus = "Activo" });
            }
            return roles;
        }

        public async Task<List<Usuario>> GetUsuariosAsync(int? paginaActual, int? numeroDeFilas, string? nombre, bool? estatus)
        {
            IQueryable<AspNetUser>? res = _dbContext.AspNetUsers;

            if (!string.IsNullOrWhiteSpace(nombre))
            {
                res = res.Where(x => x.UserName.Contains(nombre.ToUpper().Trim()));
            }
            if (estatus != null)
            {
                //res = res.Where(x => x. == estatus);
            }
            if (paginaActual != null && numeroDeFilas != null)
            {
                res = res.Skip((int)((paginaActual - 1) * numeroDeFilas)).Take((int)numeroDeFilas);
            }
            var aspNetUsers = await res.ToListAsync();
            List<Usuario> usuarios = new();                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 
            foreach (var user in aspNetUsers)
            {
                usuarios.Add(new()
                {
                    UsuarioId = user.Id,
                    NombreUsuario = user.UserName,
                    Nombre = user.UserName,
                    Apellidos = user.UserName,
                    NombreCompleto = user.UserName,
                    Rol = "",
                    Estatus = "Activo"
                });
            }
            return usuarios;
        }

        public Task<List<Rol>> UpdateRoleAsync(Rol rol)
        {
            throw new NotImplementedException();
        }

        public Task<List<Rol>> UpdateUsuarioAsync(Usuario usuario)
        {
            throw new NotImplementedException();
        }
    }
}
