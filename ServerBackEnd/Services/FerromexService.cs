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

        public async Task<ApiResponse<bool>> PostRoleModulesAsync(RoleModules roleModules)
        {
            return await PostAsync<bool>(roleModules, path: "modulesrole");
        }

        public async Task<ApiResponse<bool>> UpdateTagAsync(TagList tag)
        {
            var res = await PutAsync<TagList>(tag.Tag, tag, path: "tag");
            return new ApiResponse<bool>() { Succeeded = res.Succeeded};
        }

        public async Task<ApiResponse<byte[]>> DownloadReporteCrucesTotalesAsync(string? dia, string? mes, string? semana)
        {
            Dictionary<string, string> parameters = new();

            if (!string.IsNullOrEmpty(dia))
            {
                parameters.Add("dia", dia.ToString());
            }
            if (!string.IsNullOrEmpty(mes))
            {
                parameters.Add("mes", mes);
            }
            if (!string.IsNullOrEmpty(semana))
            {
                parameters.Add("semana", semana.ToString());
            }

            return await GetAsync<byte[]>(parameters: parameters, path: "/Reportes/TransaccionesTurno");
        }
        public async Task<ApiResponse<byte[]>> DownloadReporteCrucesFerromexAsync(string? dia, string? mes, string? semana)
        {
            Dictionary<string, string> parameters = new();

            if (!string.IsNullOrEmpty(dia))
            {
                parameters.Add("dia", dia.ToString());
            }
            if (!string.IsNullOrEmpty(mes))
            {
                parameters.Add("mes", mes);
            }
            if (!string.IsNullOrEmpty(semana))
            {
                parameters.Add("semana", semana.ToString());
            }

            return await GetAsync<byte[]>(parameters: parameters, path: "/Reportes/TransaccionesTurno");
        }
        public async Task<ApiResponse<byte[]>> DownloadConcentradosFerromexAsync(string? dia, string? mes, string? semana)
        {
            Dictionary<string, string> parameters = new();

            if (!string.IsNullOrEmpty(dia))
            {
                parameters.Add("dia", dia.ToString());
            }
            if (!string.IsNullOrEmpty(mes))
            {
                parameters.Add("mes", mes);
            }
            if (!string.IsNullOrEmpty(semana))
            {
                parameters.Add("semana", semana.ToString());
            }

            return await GetAsync<byte[]>(parameters: parameters, path: "/Reportes/ReporteIngresosResumen");
        }
        public async Task<ApiResponse<byte[]>> DownloadMantenimientoTagsAsync(string? tag, string? estatus, DateTime? fecha)
        {
            Dictionary<string, string> parameters = new();

            if (!string.IsNullOrEmpty(tag))
            {
                parameters.Add("tag", tag);
            }
            if (!string.IsNullOrEmpty(estatus))
            {
                parameters.Add("estatus", estatus);
            }
            if (fecha != null)
            {
                parameters.Add("fecha", Convert.ToString(fecha));
            }

            return await GetAsync<byte[]>(parameters: parameters, path: "");
        }
        public async Task<ApiResponse<byte[]>> DownloadReporteOperativoCajeroAsync(int? IdBolsa, int? numeroBolsa, int? turno, string? fecha)
        {
            Dictionary<string, string> parameters = new();

            if (IdBolsa != null)
            {
                parameters.Add("IdBolsa", Convert.ToString(IdBolsa));
            }
            if (numeroBolsa != null)
            {
                parameters.Add("numeroBolsa", Convert.ToString(numeroBolsa));
            }
            if (turno != null)
            {
                parameters.Add("turno", Convert.ToString(turno));
            }
            if (fecha != null)
            {
                parameters.Add("fecha", Convert.ToString(fecha));
            }


            return await GetAsync<byte[]>(parameters: parameters, path: "/Reportes/TransaccionesCajero");
        }
        public async Task<ApiResponse<byte[]>> DownloadReporteOperativoTurnoAsync(int? turno, string? fecha)
        {
            Dictionary<string, string> parameters = new();

            if (turno != null)
            {
                parameters.Add("turno", Convert.ToString(turno));
            }
            if (fecha != null)
            {
                parameters.Add("fecha", Convert.ToString(fecha));
            }


            return await GetAsync<byte[]>(parameters: parameters, path: "/Reportes/TransaccionesDetalle");
        }
        public async Task<ApiResponse<List<Bolsas>>> GeneracionBolsasAsync(string? numeroCajero, int? turno, DateTime? fecha)
        {
            Dictionary<string, string> parameters = new();

            if (!string.IsNullOrEmpty(numeroCajero))
            {
                parameters.Add("NumCajero", numeroCajero);
            }
            if (turno != null)
            {
                parameters.Add("turno", Convert.ToString(turno));
            }
            if (fecha != null)
            {
                parameters.Add("fecha", Convert.ToString(fecha));
            }

            return await GetAsync<List<Bolsas>>(parameters: parameters, path: "/GetBolsas");
        }
    }
}

