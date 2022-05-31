using ReportesData.Models;
using Shared;

namespace ApiGateway.Interfaces
{
    public interface IFerromexService
    {
        Task<ApiResponse<Module>> GetModuleAsync(int id);
        Task<ApiResponse<List<Module>>> GetModulesAsync(string? role = null);
        Task<ApiResponse<Module>> PostModuleAsync(Module module);
    }
}
