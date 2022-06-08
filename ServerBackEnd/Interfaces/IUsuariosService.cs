using Shared;

namespace ApiGateway.Interfaces
{
    public interface IUsuariosService
    {
        Task<List<Usuario>> GetUsuariosAsync(int? paginaActual, int? numeroDeFilas, string? nombre, bool? estatus);
        Task<List<Rol>> GetRolesAsync(int? paginaActual, int? numeroDeFilas, string? nombreRol, bool? estatus);
        Task<List<Rol>> UpdateRoleAsync(Rol rol);
        Task<List<Rol>> CreateUsuarioAsync(NuevoUsuario nuevoUsuario);
        Task<List<Rol>> UpdateUsuarioAsync(Usuario usuario);
        Task<int> CountUsuariosAsync(string nombre, bool? estatus);
        Task<int> CountRolesAsync(string nombreRol, bool? estatus);
    }
}
