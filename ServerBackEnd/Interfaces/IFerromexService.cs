using ReportesData.Models;
using Shared;

namespace ApiGateway.Interfaces
{
    public interface IFerromexService
    {
        Task<ApiResponse<Module>> GetModuleAsync(int id);
        Task<ApiResponse<List<Module>>> GetModulesAsync(string? role = null);
        Task<ApiResponse<Module>> PostModuleAsync(Module module);
        Task<ApiResponse<bool>> PostRoleModulesAsync(RoleModules roleModules);

        ///EPs GD

        Task<ApiResponse<bool>> GetMantenimientoTags(string paginaActual, string numeroDeFilas, string tag, string estatus, string fecha);
        Task<ApiResponse<bool>> CreatePdfCruces(RoleModules roleModules);
        Task<ApiResponse<bool>> CreatePdfCrucesFerromex(RoleModules roleModules);
        Task<ApiResponse<bool>> CreatePdfConcentradosFerromex(RoleModules roleModules);
        Task<ApiResponse<bool>> CreatePdfMantenimientoTags(RoleModules roleModules);
        Task<ApiResponse<bool>> CreatePdfReporteCajero(RoleModules roleModules);
    }
}
