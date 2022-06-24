using ApiGateway.Models;
using ReportesData.Models;
using Shared;

namespace ApiGateway.Interfaces
{
    public interface IFerromexService
    {
        Task<ApiResponse<Module>> GetModuleAsync(int id);
        Task<ApiResponse<List<Module>>> GetModulesAsync(string? role = null);
        Task<ApiResponse<Module>> PostModuleAsync(Module module);
        Task<ApiResponse<RoleModules>> PostRoleModulesAsync(RoleModules roleModules);

        Task<ApiResponse<List<TagList>>> GetTagsAsync(int? paginaActual, int? numeroDeFilas, string? tag, bool? estatus, DateTime? fecha);
        Task<ApiResponse<int>> GetTagsCountAsync(string? tag, bool? estatus, DateTime? fecha);
        Task<ApiResponse> UpdateTagAsync(TagList tag);
        Task<ApiResponse<TagList>> CreateTagAsync(TagList tag);
        Task<ApiResponse<bool>> DeleteTagAsync(string? tag);

        Task<ApiResponse<byte[]>> DownloadReporteCrucesTotalesAsync(string? dia, string? mes, string? semana);
        Task<ApiResponse<byte[]>> DownloadReporteCrucesFerromexDescuentoDetalleAsync(string? dia, string? mes, string? semana);
        Task<ApiResponse<byte[]>> DownloadReporteCrucesFerromexDescuentoResumenAsync(string? dia, string? mes, string? semana);
        Task<ApiResponse<byte[]>> DownloadConcentradosFerromexAsync(string? dia, string? mes, string? semana);
        Task<ApiResponse<byte[]>> DownloadMantenimientoTagsAsync(string? tag, bool? estatus, DateTime? fecha);
        Task<ApiResponse<byte[]>> DownloadReporteOperativoCajeroConcentradoAsync(int? IdBolsa, string? numeroBolsa, int? turno, string? fecha);
        Task<ApiResponse<byte[]>> DownloadReporteOperativoCajeroTransaccionesAsync(int? IdBolsa, string? numeroBolsa, int? turno, string? fecha);
        Task<ApiResponse<byte[]>> DownloadReporteOperativoTurnoConcentradoAsync(int? turno, string? fecha);
        Task<ApiResponse<byte[]>> DownloadReporteOperativoTurnoTransaccionesAsync(int? turno, string? fecha);
        Task<ApiResponse<List<Bolsas>>> GeneracionBolsasAsync(string? numeroCajero, int? turno, DateTime? fecha);

        Task<ApiResponse<List<LaneCatalog>>> GetLanesAsync();
        Task<ApiResponse<List<Cruce>>> GetTransactionsAsync(int? paginaActual, int? numeroDeFilas, string? tag, string? carril, string? cuerpo, DateTime? fecha);
        Task<ApiResponse<int>> GetTransactionsCountAsync(string? tag, string? carril, string? cuerpo, DateTime? fecha);

    }
}
