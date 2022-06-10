﻿using ReportesData.Models;
using Shared;

namespace ApiGateway.Interfaces
{
    public interface IFerromexService
    {
        Task<ApiResponse<Module>> GetModuleAsync(int id);
        Task<ApiResponse<List<Module>>> GetModulesAsync(string? role = null);
        Task<ApiResponse<Module>> PostModuleAsync(Module module);
        Task<ApiResponse<bool>> PostRoleModulesAsync(RoleModules roleModules);
        Task<ApiResponse<List<TagList>>> GetTagsAsync(int? paginaActual, int? numeroDeFilas, string? tag, bool? estatus, DateTime? fecha);
        Task<ApiResponse<int>> GetTagsCountAsync(string? tag, bool? estatus, DateTime? fecha);
        Task<ApiResponse<bool>> UpdateTagAsync(TagList tag);
        Task<ApiResponse<TagList>> CreateTagAsync(TagList tag);
        Task<ApiResponse<bool>> DeleteTagAsync(string? tag);

        ///EPs GD

        //Task<ApiResponse<bool>> GetMantenimientoTags(string paginaActual, string numeroDeFilas, string tag, string estatus, string fecha);
        //Task<ApiResponse<bool>> CreatePdfCruces(RoleModules roleModules);
        //Task<ApiResponse<bool>> CreatePdfCrucesFerromex(RoleModules roleModules);
        //Task<ApiResponse<bool>> CreatePdfConcentradosFerromex(RoleModules roleModules);
        //Task<ApiResponse<bool>> CreatePdfMantenimientoTags(RoleModules roleModules);
        //Task<ApiResponse<bool>> CreatePdfReporteCajero(RoleModules roleModules);
    }
}
