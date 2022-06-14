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

        public async Task<ApiResponse<TagList>> CreateTagAsync(TagList tag)
        {
            return await PostAsync(tag, path: "tag");
        }

        public async Task<ApiResponse<bool>> DeleteTagAsync(string? tag)
        {
            return await DeleteAsync<bool>(tag, path: "tag");
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

        public async Task<ApiResponse<List<TagList>>> GetTagsAsync(int? paginaActual, int? numeroDeFilas, string? tag, bool? estatus, DateTime? fecha)
        {
            Dictionary<string, string> parameters = new();
            if (paginaActual != null && paginaActual != 0)
            {
                parameters.Add("paginaActual", paginaActual.ToString());
            }
            if (numeroDeFilas != null && numeroDeFilas != 0)
            {
                parameters.Add("numeroDeFilas", numeroDeFilas.ToString());
            }
            if (!string.IsNullOrEmpty(tag))
            {
                parameters.Add("tag", tag);
            }
            if (estatus != null)
            {
                parameters.Add("estatus", estatus.Value.ToString());
            }
            if (fecha != null)
            {
                parameters.Add("fecha", fecha.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"));
            }
            return await GetAsync<List<TagList>>(path: "tags", parameters: parameters);
        }

        public async Task<ApiResponse<int>> GetTagsCountAsync(string? tag, bool? estatus, DateTime? fecha)
        {
            Dictionary<string, string> parameters = new();
            if (!string.IsNullOrEmpty(tag))
            {
                parameters.Add("tag", tag);
            }
            if (estatus != null)
            {
                parameters.Add("estatus", estatus.Value.ToString());
            }
            if (fecha != null)
            {
                parameters.Add("fecha", fecha.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"));
            }
            return await GetAsync<int>(path: "tagsCount", parameters: parameters);
        }

        public async Task<ApiResponse<Module>> PostModuleAsync(Module module)
        {
            return await PostAsync(module, path: "module");
        }

        public async Task<ApiResponse<RoleModules>> PostRoleModulesAsync(RoleModules roleModules)
        {
            return await PostAsync(roleModules, path: "modulesrole");
        }

        public async Task<ApiResponse> UpdateTagAsync(TagList tag)
        {
            return await PutAsync(tag.Tag, tag, path: "tag");
        }

        public async Task<ApiResponse<List<LaneCatalog>>> GetLanesAsync()
        {
            return await GetAsync<List<LaneCatalog>>(path: "lanes");
        }

        public async Task<ApiResponse<List<Cruce>>> GetTransactionsAsync(int? paginaActual, int? numeroDeFilas, string? tag, string? carril, string? cuerpo, DateTime? fecha)
        {
            Dictionary<string, string> parameters = new();
            if (paginaActual != null && paginaActual != 0)
            {
                parameters.Add("paginaActual", paginaActual.ToString());
            }
            if (numeroDeFilas != null && numeroDeFilas != 0)
            {
                parameters.Add("numeroDeFilas", numeroDeFilas.ToString());
            }
            if (!string.IsNullOrEmpty(tag))
            {
                parameters.Add("tag", tag);
            }
            if (!string.IsNullOrEmpty(carril))
            {
                parameters.Add("carril", carril);
            }
            if (!string.IsNullOrEmpty(cuerpo))
            {
                parameters.Add("cuerpo", cuerpo);
            }
            if (fecha != null)
            {
                parameters.Add("fecha", fecha.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"));
            }
            return await GetAsync<List<Cruce>>(path: "cruces", parameters: parameters);
        }
        public async Task<ApiResponse<int>> GetTransactionsCountAsync(string? tag, string? carril, string? cuerpo, DateTime? fecha)
        {
            Dictionary<string, string> parameters = new();
            if (!string.IsNullOrEmpty(tag))
            {
                parameters.Add("tag", tag);
            }
            if (!string.IsNullOrEmpty(carril))
            {
                parameters.Add("carril", carril);
            }
            if (!string.IsNullOrEmpty(cuerpo))
            {
                parameters.Add("cuerpo", cuerpo);
            }
            if (fecha != null)
            {
                parameters.Add("fecha", fecha.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"));
            }
            return await GetAsync<int>(path: "transactionsCount", parameters: parameters);
        }

    }
}
