﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using ReportesData.Models;
using TestingFrontEnd.Interfaces;
using TestingFrontEnd.Stores;

namespace TestingFrontEnd.Pages.Reportes
{
    [Authorize]
    public partial class CajeroReceptorIndex : ComponentBase
    {
        private readonly IReportesService _reportesService;
        private ApplicationContext _context;
        public EditContext EditContext;
        private CajeroReceptor ReporteCajeroReceptorModel { get; set; }

        private TypePlaza Plaza { get; set; }
        private TypeDelegacion Delegacion { get; set; }
        private UsuarioPlaza UsuarioPlaza { get; set; }
        private List<Personal> Administradores { get; set; }
        private List<Bolsa>? Bolsas { get; set; }
        private KeyValuePair<string, string>[] Turnos;
        private byte[]? PdfBlob { get; set; }
        private bool Render;
        private bool HideLoader = true;
        private bool HideError = true;

        public CajeroReceptorIndex(IReportesService reportesService, ApplicationContext context)
        {
            _reportesService = reportesService;
            _context = context;
        }

        protected override async Task OnInitializedAsync()
        {
            ReporteCajeroReceptorModel = new() { Fecha = DateTime.Now };
            EditContext = new EditContext(ReporteCajeroReceptorModel);

            UsuarioPlaza = await _reportesService.GetUsuarioPlazaAsync();
            ReporteCajeroReceptorModel.NumPlaza = UsuarioPlaza?.NumPlaza;
            ReporteCajeroReceptorModel.NumDelegacion = UsuarioPlaza?.NumDelegacion;

            if (UsuarioPlaza != null)
            {
                var plazas = await _reportesService.GetPlazasAsync();
                Plaza = plazas.FirstOrDefault(x => x.NumPlaza == UsuarioPlaza.NumPlaza);

                var delegaciones = await _reportesService.GetDelegacionesAsync();
                Delegacion = delegaciones.FirstOrDefault(x => x.NumDelegacion == UsuarioPlaza.NumDelegacion);

                Administradores = await _reportesService.GetAdministradoresAsync();
                ReporteCajeroReceptorModel.NumGeaAdministrador = Administradores?.FirstOrDefault().NumGea;

                Turnos = await _reportesService.GetTurnosAsync();
                ReporteCajeroReceptorModel.IdTurno = Turnos?.FirstOrDefault().Key;

                Render = true;
            }
        }

        private async Task ConsultarBolsas()
        {
            HideError = true;
            HideLoader = false;

            if (EditContext.Validate())
            {
                Bolsas = await _reportesService.CreateBolsasCajeroReceptorAsync(ReporteCajeroReceptorModel);
                if (Bolsas == null || Bolsas.Count <= 0)
                {
                    HideError = false;
                }
                PdfBlob = null;
            }
            else
            {
                Console.WriteLine("Form is Invalid");
            }

            HideLoader = true;
        }
        private async Task GenerarReporte(int? id)
        {
            HideLoader = false;
            Bolsas = null;

            ReporteCajeroReceptorModel.IdBolsa = id;
            PdfBlob = await _reportesService.CreateReporteCajeroReceptorAsync(ReporteCajeroReceptorModel);

            HideLoader = true;
        }
    }
}
