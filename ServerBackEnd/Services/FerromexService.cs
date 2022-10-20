using ApiGateway.Interfaces;
using ApiGateway.Models;
using ApiGateway.Proxies;
using ReportesData.Models;
using Shared;
using System.Text.Json;
using System.Threading.Tasks;
using static ApiGateway.Controllers.FerromexController;

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

        public async Task<ApiResponse<List<TagList>>> GetTagsAsync(int? paginaActual, int? numeroDeFilas, string? tag, bool? estatus, DateTime? fecha, string? noDePlaca, string? noEconomico)
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
            if (!string.IsNullOrEmpty(noDePlaca))
            {
                parameters.Add("noDePlaca", noDePlaca);
            }
            if (!string.IsNullOrEmpty(noEconomico))
            {
                parameters.Add("noEconomico", noEconomico);
            }

            return await GetAsync<List<TagList>>(path: "tags", parameters: parameters);
        }

        public async Task<ApiResponse<int>> GetTagsCountAsync(string? tag, bool? estatus, DateTime? fecha, string? noDePlaca, string? noEconomico)
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
            if (!string.IsNullOrEmpty(noDePlaca))
            {
                parameters.Add("noDePlaca", noDePlaca);
            }
            if (!string.IsNullOrEmpty(noEconomico))
            {
                parameters.Add("noEconomico", noEconomico);
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

        public async Task<ApiResponse<List<TypeClass>>> GetClassAsync()
        {
            return await GetAsync<List<TypeClass>>(path: "class");
        }

        public async Task<ApiResponse<int?[]>> GetTurnosAsync(DateTime date)
        {
            Dictionary<string, string> parameters = new()
            {
                { "fecha", date.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'") }
            };
            return await GetAsync<int?[]>(path: "turnos", parameters: parameters);
        }

        public async Task<ApiResponse<List<Cruce>>> GetTransactionsAsync(int? paginaActual, int? numeroDeFilas, string? tag, string? carril, string? cuerpo, DateTime? fecha, string? noDePlaca, string? noEconomico, string? clase)
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
            if (!string.IsNullOrEmpty(noDePlaca))
            {
                parameters.Add("noDePlaca", noDePlaca);
            }
            if (!string.IsNullOrEmpty(noEconomico))
            {
                parameters.Add("noEconomico", noEconomico);
            }
            if (!string.IsNullOrEmpty(clase))
            {
                parameters.Add("clase", clase);
            }

            return await GetAsync<List<Cruce>>(path: "cruces", parameters: parameters);
        }
        public async Task<ApiResponse<int>> GetTransactionsCountAsync(string? tag, string? carril, string? cuerpo, DateTime? fecha, string? noDePlaca, string? noEconomico, string? clase)
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
            if (!string.IsNullOrEmpty(noDePlaca))
            {
                parameters.Add("noDePlaca", noDePlaca);
            }
            if (!string.IsNullOrEmpty(noEconomico))
            {
                parameters.Add("noEconomico", noEconomico);
            }
            if (!string.IsNullOrEmpty(clase))
            {
                parameters.Add("clase", clase);
            }

            return await GetAsync<int>(path: "transactionsCount", parameters: parameters);
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

            return await GetAsync<byte[]>(parameters: parameters, path: "/Reportes/TransaccionesCrucesTotales");
        }
        public async Task<ApiResponse<byte[]>> DownloadReporteCrucesFerromexDescuentoDetalleAsync(string? dia, string? mes, string? semana, string? tag, string? noDePlaca, string? noEconomico)
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
            if (!string.IsNullOrEmpty(tag))
            {
                parameters.Add("tag", tag);
            }
            if (!string.IsNullOrEmpty(noDePlaca))
            {
                parameters.Add("noDePlaca", noDePlaca);
            }
            if (!string.IsNullOrEmpty(noEconomico))
            {
                parameters.Add("noEconomico", noEconomico);
            }

            return await GetAsync<byte[]>(parameters: parameters, path: "/Reportes/ReporteCrucesFerromex");
        }
        public async Task<ApiResponse<byte[]>> DownloadReporteCrucesFerromexDescuentoResumenAsync(string? dia, string? mes, string? semana, string? tag, string? noDePlaca, string? noEconomico)
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
            if (!string.IsNullOrEmpty(tag))
            {
                parameters.Add("tag", tag);
            }
            if (!string.IsNullOrEmpty(noDePlaca))
            {
                parameters.Add("noDePlaca", noDePlaca);
            }
            if (!string.IsNullOrEmpty(noEconomico))
            {
                parameters.Add("noEconomico", noEconomico);
            }

            return await GetAsync<byte[]>(parameters: parameters, path: "/Reportes/ReporteCrucesFerromexResumen");
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
        public async Task<ApiResponse<byte[]>> DownloadMantenimientoTagsAsync(string? tag, bool? estatus, DateTime? fecha, string? noDePlaca, string? noEconomico)
        {
            Dictionary<string, string> parameters = new();

            if (!string.IsNullOrEmpty(tag))
            {
                parameters.Add("tag", tag);
            }
            if (estatus != null)
            {
                parameters.Add("estatus", Convert.ToString(estatus));
            }
            if (fecha != null)
            {
                parameters.Add("fecha", Convert.ToString(fecha));
            }
            if (!string.IsNullOrEmpty(noDePlaca))
            {
                parameters.Add("noDePlaca", noDePlaca);
            }
            if (!string.IsNullOrEmpty(noEconomico))
            {
                parameters.Add("noEconomico", noEconomico);
            }

            return await GetAsync<byte[]>(parameters: parameters, path: "/Reportes/MantenimientoTags");
        }
        public async Task<ApiResponse<byte[]>> DownloadReporteOperativoCajeroConcentradoAsync(int? IdBolsa)
        {
            Dictionary<string, string> parameters = new();

            if (IdBolsa != null)
            {
                parameters.Add("IdBolsa", Convert.ToString(IdBolsa));
            }

            return await GetAsync<byte[]>(parameters: parameters, path: "/Reportes/ConcentradoCajero");
        }
        public async Task<ApiResponse<byte[]>> DownloadReporteOperativoCajeroTransaccionesAsync(int? IdBolsa)
        {
            Dictionary<string, string> parameters = new();

            if (IdBolsa != null)
            {
                parameters.Add("IdBolsa", Convert.ToString(IdBolsa));
            }

            return await GetAsync<byte[]>(parameters: parameters, path: "/Reportes/TransaccionesCajero");
        }

        public async Task<ApiResponse<byte[]>> DownloadReporteOperativoDeatelleAsync(string? carril, string? fecha)
        {
            Dictionary<string, string> parameters = new();

            if (!string.IsNullOrEmpty(carril))
            {
                parameters.Add("carril", carril);
            }
            if (!string.IsNullOrEmpty(fecha))
            {
                parameters.Add("fecha", fecha);
            }

            return await GetAsync<byte[]>(parameters: parameters, path: "/Reportes/TransaccionesTurno");
        }

        public async Task<ApiResponse<byte[]>> DownloadReporteOperativoTurnoConcentradoAsync(int? turno, string? fecha)
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


            return await GetAsync<byte[]>(parameters: parameters, path: "/Reportes/ConcentradoTurno");
        }
        public async Task<ApiResponse<byte[]>> DownloadReporteOperativoTurnoTransaccionesAsync(int? turno, string? fecha)
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


            return await GetAsync<byte[]>(parameters: parameters, path: "/Reportes/TransaccionesTurno");
        }

        public async Task<ApiResponse<byte[]>> DownloadReporteOperativoConcentradoAsync(string? carril, string? fecha)
        {
            Dictionary<string, string> parameters = new();

            if (!string.IsNullOrEmpty(carril))
            {
                parameters.Add("carril", carril);
            }
            if (!string.IsNullOrEmpty(fecha))
            {
                parameters.Add("fecha", fecha);
            }

            return await GetAsync<byte[]>(parameters: parameters, path: "/Reportes/ConcentradoTurno");
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

        public async Task<ApiResponse<List<Viapasstags>>> GetTagsOrTagAsync(string? tag)
        {
            Dictionary<string, string> parameters = new();

            if (!string.IsNullOrEmpty(tag))
            {
                parameters.Add("tag", tag);
            }
            return await GetAsync<List<Viapasstags>>(parameters: parameters, path: "/ViaPassTags");
        }

        public async Task<ApiResponse<byte[]>> GetReporteActividadUsuariosAsync(string? dia, string? semana, string? mes, string? nombre, string? rol, string? accion)
        {
            Dictionary<string, string> parameters = new();

            if (!string.IsNullOrEmpty(dia))
            {
                parameters.Add("dia", dia);
            }
            if (semana != null)
            {
                parameters.Add("semana", Convert.ToString(semana));
            }
            if (mes != null)
            {
                parameters.Add("mes", Convert.ToString(mes));
            }
            if (!string.IsNullOrEmpty(nombre))
            {
                parameters.Add("nombre", nombre);
            }
            if (!string.IsNullOrEmpty(rol))
            {
                parameters.Add("rol", rol);
            }
            if (!string.IsNullOrEmpty(accion))
            {
                parameters.Add("accion", accion);
            }

            return await GetAsync<byte[]>(parameters: parameters, path: "/Reportes/ActividadUsuario");
        }

        public async Task<ApiResponse<List<ActividadUsuarios>>> GetActividadUsuariosAsync(int? paginaActual, int? numeroDeFilas, string? dia, string? semana, string? mes, string? nombre, string? rol, string? accion)
        {
            Dictionary<string, string> parameters = new();

            if (paginaActual != null)
            {
                parameters.Add("paginaActual", Convert.ToString(paginaActual));
            }
            if (numeroDeFilas != null)
            {
                parameters.Add("numeroDeFilas", Convert.ToString(numeroDeFilas));
            }
            if (!string.IsNullOrEmpty(dia))
            {
                parameters.Add("dia", dia);
            }
            if (semana != null)
            {
                parameters.Add("semana", Convert.ToString(semana));
            }
            if (mes != null)
            {
                parameters.Add("mes", Convert.ToString(mes));
            }
            if (!string.IsNullOrEmpty(nombre))
            {
                parameters.Add("nombre", nombre);
            }
            if (!string.IsNullOrEmpty(rol))
            {
                parameters.Add("rol", rol);
            }
            if (!string.IsNullOrEmpty(accion))
            {
                parameters.Add("accion", accion);
            }

            return await GetAsync<List<ActividadUsuarios>>(parameters: parameters, path: "/ActividadUsuario");
        }

        public async Task<ApiResponse<int>> GetActividadUsuariosCountAsync(string? dia, string? semana, string? mes, string? nombre, string? rol, string? accion)
        {
            Dictionary<string, string> parameters = new();

            if (!string.IsNullOrEmpty(dia))
            {
                parameters.Add("dia", dia);
            }
            if (semana != null)
            {
                parameters.Add("semana", Convert.ToString(semana));
            }
            if (mes != null)
            {
                parameters.Add("mes", Convert.ToString(mes));
            }
            if (!string.IsNullOrEmpty(nombre))
            {
                parameters.Add("nombre", nombre);
            }
            if (!string.IsNullOrEmpty(rol))
            {
                parameters.Add("rol", rol);
            }
            if (!string.IsNullOrEmpty(accion))
            {
                parameters.Add("accion", accion);
            }

            return await GetAsync<int>(parameters: parameters, path: "/ActividadUsuarioCount");
        }
    }
}


