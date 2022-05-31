﻿using ApiGateway.Interfaces;
using ApiGateway.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReportesData.Models;
using Shared;
using System.Net.Mime;

namespace ApiGateway.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class FerromexController : ControllerBase
    {
        private readonly IFerromexService _ferromexService;

        public FerromexController(IFerromexService FerromexService)
        {
            _ferromexService = FerromexService;
        }

        // GET: api/<Controller>/module/1
        /// <summary>
        /// Obtener la informacion del usuario logueado en la plaza.
        /// </summary>
        /// <returns>Regresa un modelo con los detalles de registro del usuario autentificado.</returns>
        /// <response code="200">Regresa el objeto solicitado</response>
        /// <response code="400">Alguno de los datos requeridos es incorrecto</response>
        /// <response code="500">Error por excepcion no controlada en el Gateway</response>
        [HttpGet("module/{id}")]
        [Produces("application/json", "application/problem+json")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<Module>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<Module>>> GetModule(int id)
        {
            return Ok(await _ferromexService.GetModuleAsync(id));
        }

        // GET: api/<Controller>/modules
        /// <summary>
        /// Obtener la informacion del usuario logueado en la plaza.
        /// </summary>
        /// <returns>Regresa un modelo con los detalles de registro del usuario autentificado.</returns>
        /// <response code="200">Regresa el objeto solicitado</response>
        /// <response code="400">Alguno de los datos requeridos es incorrecto</response>
        /// <response code="500">Error por excepcion no controlada en el Gateway</response>
        [HttpGet("modules")]
        [Produces("application/json", "application/problem+json")]
        [ProducesResponseType(typeof(ApiResponse<List<Module>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<List<Module>>>> GetModules(string? role)
        {
            return Ok(await _ferromexService.GetModulesAsync(role));
        }

        // GET: api/<Controller>/module/1
        /// <summary>
        /// Obtener la informacion del usuario logueado en la plaza.
        /// </summary>
        /// <returns>Regresa un modelo con los detalles de registro del usuario autentificado.</returns>
        /// <response code="200">Regresa el objeto solicitado</response>
        /// <response code="400">Alguno de los datos requeridos es incorrecto</response>
        /// <response code="500">Error por excepcion no controlada en el Gateway</response>
        [HttpPost("module")]
        [Produces("application/json", "application/problem+json")]
        [ProducesResponseType(typeof(ApiResponse<Module>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<Module>>> PostModule(Module module)
        {
            return Ok(await _ferromexService.PostModuleAsync(module));
        }
    }
}
