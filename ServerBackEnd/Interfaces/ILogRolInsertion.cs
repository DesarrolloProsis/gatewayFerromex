using ApiGateway.Services;
using Shared;

namespace ApiGateway.Interfaces
{
    public interface ILogRolInsertion
    {
        void InsertLogAddOrRemoveRole(UserAddRolesCommand addRolesCommand);
        void InsertLogAddRole(AddRolesCommand addRolesCommand);
        void InsertLogEditRole(Rol rol);
    }
}
