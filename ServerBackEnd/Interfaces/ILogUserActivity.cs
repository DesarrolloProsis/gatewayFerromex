using ApiGateway.Models;
using ApiGateway.Services;
using Shared;

namespace ApiGateway.Interfaces
{
    public interface ILogUserActivity
    {
        void InsertarLogEditUser(Usuario usuario, AspNetUser aspNetUser);
        void InsertarLogUpdatePass(ApplicationUser usuario, UsuarioUpdatePassword usuarioPass, AspNetUser aspNetUser);
        void InsertarLogCreateUser(ApplicationUser usuario, UserCreateCommand usuarioPass, string? idRole);
    }
}
