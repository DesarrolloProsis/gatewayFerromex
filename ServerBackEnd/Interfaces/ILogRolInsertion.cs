using ApiGateway.Services;
using Microsoft.AspNetCore.Identity;
using Shared;

namespace ApiGateway.Interfaces
{
    public interface ILogRolInsertion
    {
        Task InsertLogAddOrRemoveRole(UserAddRolesCommand addRolesCommand);
        Task InsertLogAddRole(AddRolesCommand addRolesCommand);
        Task InsertLogEditRole(Rol rol, IdentityRole rolOld);
    }
}
