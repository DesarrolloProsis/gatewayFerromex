using ApiGateway.Interfaces;
using ApiGateway.Models;
using ApiGateway.Proxies;
using ReportesData.Models;
using Shared;
using System.Text.Json;

namespace ApiGateway.Services
{
    public class FerromexService : GenericProxy, IFerromexService
    {
        public FerromexService(IHttpContextAccessor? httpContextAccessor, IHttpClientFactory httpClientFactory) : base(httpContextAccessor, httpClientFactory, "Reportes")
        {
        }

        public async Task<ApiResponse<Module>> GetModuleAsync(int id)
        {
            return await GetAsync<Module>(id, path: "module");
        }

        public async Task<ApiResponse<List<Module>>> GetModulesAsync(string? role = null)
        {
            Dictionary<string, string> parameters = new();
            if (!string.IsNullOrEmpty(role))
            {
                parameters.Add("role", role);
            }
            return await GetAsync<List<Module>>(path: "modules", parameters: parameters);
        }

        public async Task<ApiResponse<Module>> PostModuleAsync(Module module)
        {
            return await PostAsync(module, path: "module");
        }

        public async Task<ApiResponse<bool>> PostRoleModulesAsync(RoleModules roleModules)
        {
            return await PostAsync<bool>(roleModules, path: "modulesrole");
        }
    }
}
