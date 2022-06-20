using ApiGateway.Interfaces;
using ApiGateway.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReportesData.Models;
using Shared;
using System.Net.Mime;

namespace ApiGateway.Controllers
{
    [Route("api/[controller]")]
    //[Authorize]
    [ApiController]
    public class FerromexController : ControllerBase
    {
        private readonly IFerromexService _ferromexService;

        public FerromexController(IFerromexService FerromexService)
        {
            _ferromexService = FerromexService;
        }
        #region Modulos

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
            //int idInt = 0;

            //if (!string.IsNullOrWhiteSpace(id))
            //{
            //    if (!int.TryParse(id.Trim(), out int i))
            //    {
            //        throw new ValidationException($"Id incorrecto");
            //    }
            //    idInt = i;
            //}
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

        /// <summary>
        /// Registrar roles de usuario 
        /// </summary>
        /// <param name="addRolesCommand">Parametros de registro</param>
        /// <returns>Regresa el codigo de estado.</returns>
        /// <response code="204">Se agregaron correctamento los roles al usuario</response>
        /// <response code="400">Alguno de los datos requeridos es incorrecto</response>
        /// <response code="500">Error por excepcion no controlada en el Gateway</response>
        [HttpPost("addRoleModules")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces("application/json", "application/problem+json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddRoleModules(RoleModules roleModules)
        {
            if (ModelState.IsValid)
            {
                var result = await _ferromexService.PostRoleModulesAsync(roleModules);
                if (!result.Succeeded)
                {
                    return BadRequest(result.ErrorMessage);
                }
                return NoContent();
            }
            return BadRequest();
        }

        #endregion
        #region Mantenimiento Tags

        [HttpGet("mantenimientotags/{paginaActual}/{numeroDeFilas}/{tag}/{estatus}/{fecha}")]
        public async Task<IActionResult> GetTags(string? paginaActual, string? numeroDeFilas, string? tag, string? estatus, string? fecha)
        {
            paginaActual = GetNullableString(paginaActual);
            numeroDeFilas = GetNullableString(numeroDeFilas);
            tag = GetNullableString(tag);
            estatus = GetNullableString(estatus);
            fecha = GetNullableString(fecha);

            bool? estatusBool = null;
            DateTime? fechaDt = null;
            int? paginaActualInt = null;
            int? numeroDeFilasInt = null;

            if (!string.IsNullOrWhiteSpace(paginaActual) || !string.IsNullOrWhiteSpace(numeroDeFilas))
            {
                if (!int.TryParse(paginaActual.Trim(), out int pa) || !int.TryParse(numeroDeFilas.Trim(), out int ndf))
                {
                    return BadRequest($"La paginacion se encuentra en un formato incorrecto");
                }
                paginaActualInt = pa;
                numeroDeFilasInt = ndf;
            }
            if (!string.IsNullOrWhiteSpace(estatus) && estatus.Trim().ToUpper().Equals("ACTIVO")
                || !string.IsNullOrWhiteSpace(estatus) && estatus.Trim().ToUpper().Equals("TRUE"))
            {
                estatusBool = true;
            }
            if (!string.IsNullOrWhiteSpace(estatus) && estatus.Trim().ToUpper().Equals("INACTIVO")
                || !string.IsNullOrWhiteSpace(estatus) && estatus.Trim().ToUpper().Equals("FALSE"))
            {
                estatusBool = false;
            }
            if (!string.IsNullOrWhiteSpace(fecha))
            {
                if (!DateTime.TryParse(fecha, out DateTime dt))
                    return BadRequest($"La fecha '{fecha}' se encuentra en un formato incorrecto");
                fechaDt = dt;
            }
            var tags = await _ferromexService.GetTagsAsync(paginaActualInt, numeroDeFilasInt, tag, estatusBool, fechaDt);
            var tagsCountResponse = await _ferromexService.GetTagsCountAsync(tag, estatusBool, fechaDt);
            if (!tagsCountResponse.Succeeded)
            {
                return BadRequest(tagsCountResponse.ErrorMessage);
            }
            MantenimientoTags res = new()
            {
                PaginasTotales = paginaActualInt
                == null ? null : (int?)Math.Ceiling(decimal.Divide(tagsCountResponse.Content, numeroDeFilasInt ?? 1)),
                PaginaActual = paginaActualInt,
                Tags = tags.Content
            };
            return Ok(res);
        }


        [HttpPost("agregartag")]
        public async Task<IActionResult> CreateTag(TagList tag)
        {
            if (ModelState.IsValid)
            {
                _ferromexService.CreateTagAsync(tag);
                return NoContent();
            }
            return BadRequest();
        }

        [HttpPut("editartag")]
        public async Task<IActionResult> UpdateTag(TagList tag)
        {
            if (ModelState.IsValid)
            {
                _ferromexService.UpdateTagAsync(tag);
                return NoContent();
            }
            return BadRequest();
        }

        [HttpDelete("eliminartag/{tag}")]
        public async Task<IActionResult> DeleteTag(string tag)
        {
            if (!string.IsNullOrWhiteSpace(tag))
            {
                _ferromexService.DeleteTagAsync(tag);
                return NoContent();
            }
            return BadRequest();
        }
        #endregion
        #region Reportes

        [HttpGet("Download/pdf/crucestotales/reporteCruces/{dia}/{mes}/{semana}")]
        [Produces("application/json", "application/problem+json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DownloadReporteCrucesTotales(string? dia, string? mes, string? semana)
        {
            dia = GetNullableString(dia);
            mes = GetNullableString(mes);
            semana = GetNullableString(semana);

            var result = await _ferromexService.DownloadReporteCrucesTotalesAsync(dia, mes, semana);
            if (!result.Succeeded)
            {
                return BadRequest(result.ErrorMessage);
            }
            else
            {
                return File(result.Content, "application/pdf", "CrucesTotales.pdf");
            }
            return NoContent();
        }

        [HttpGet("Download/pdf/crucesferromex/{dia}/{mes}/{semana}")]
        [Produces("application/json", "application/problem+json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DownloadReporteCrucesFerromex(string? dia, string? mes, string? semana)
        {
            dia = GetNullableString(dia);
            mes = GetNullableString(mes);
            semana = GetNullableString(semana);

            var result = await _ferromexService.DownloadReporteCrucesFerromexAsync(dia, mes, semana);
            if (!result.Succeeded)
            {
                return BadRequest(result.ErrorMessage);
            }
            else
            {
                return File(result.Content, "application/pdf", "CrucesFerromex.pdf");
            }
            return NoContent();
        }

        [HttpGet("Download/pdf/concentradosferromex/{dia}/{mes}/{semana}")]
        [Produces("application/json", "application/problem+json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DownloadConcentradosFerromex(string? dia, string? mes, string? semana)
        {
            dia = GetNullableString(dia);
            mes = GetNullableString(mes);
            semana = GetNullableString(semana);

            var result = await _ferromexService.DownloadConcentradosFerromexAsync(dia, mes, semana);
            if (!result.Succeeded)
            {
                return BadRequest(result.ErrorMessage);
            }
            else
            {
                return File(result.Content, "application/pdf", "ConcentradosFerromex.pdf");
            }
            return NoContent();
        }

        [HttpGet("Download/pdf/mantenimientotags/{tag}/{estatus}/{fecha}")]
        [Produces("application/json", "application/problem+json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DownloadMantenimientoTags(string tag, bool estatus, string fecha)
        {
            tag = GetNullableString(tag);

            DateTime? fechaDt = null;

            if (!string.IsNullOrWhiteSpace(fecha))
            {
                if (!DateTime.TryParse(fecha, out DateTime dt))
                    return BadRequest($"La fecha '{fecha}' se encuentra en un formato incorrecto");
                fechaDt = dt;
            }

            var result = await _ferromexService.DownloadMantenimientoTagsAsync(tag, estatus, fechaDt);
            if (!result.Succeeded)
            {
                return BadRequest(result.ErrorMessage);
            }
            else
            {
                return File(result.Content, "application/pdf", "MantenimientoTags.pdf");
            }

            return NoContent();
        }

        [HttpGet("Download/pdf/reporteOperativo/reporteCajero/{idBolsa}/{numeroCajero}/{turno}/{fecha}")]
        [Produces("application/json", "application/problem+json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DownloadReporteOperativoCajero(string? idBolsa, string? numeroCajero, string? turno, string? fecha)
        {
            fecha = GetNullableString(fecha);

            int idBolsaIn = 0, numeroCajeroIn = 0, turnoIn = 0;

            if (!string.IsNullOrWhiteSpace(turno))
            {
                turnoIn = Convert.ToInt16(turno);
            }
            if (!string.IsNullOrWhiteSpace(numeroCajero))
            {
                numeroCajeroIn = Convert.ToInt16(numeroCajero);
            }
            if (!string.IsNullOrWhiteSpace(idBolsa))
            {
                idBolsaIn = Convert.ToInt16(idBolsa);
            }

            var result = await _ferromexService.DownloadReporteOperativoCajeroAsync(idBolsaIn, numeroCajeroIn, turnoIn, fecha);
            if (!result.Succeeded)
            {
                return BadRequest(result.ErrorMessage);
            }
            else
            {
                return File(result.Content, "application/pdf", "ReporteCajero.pdf");
            }
            return NoContent();
        }

        [HttpGet("Download/pdf/reporteOperativo/reporteTurno/{turno}/{fecha}")]
        [Produces("application/json", "application/problem+json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DownloadReporteOperativoTurno(string? turno, string? fecha)
        {
            turno = GetNullableString(turno);
            fecha = GetNullableString(fecha);

            int turnoI = 0;

            if (!string.IsNullOrWhiteSpace(turno))
            {
                turnoI = Convert.ToInt16(turno);
            }


            if (ModelState.IsValid)
            {
                var result = await _ferromexService.DownloadReporteOperativoTurnoAsync(turnoI, fecha);
                if (!result.Succeeded)
                {
                    return BadRequest(result.ErrorMessage);
                }
                else
                {
                    return File(result.Content, "application/pdf", "ReporteTurno.pdf");
                }
                return NoContent();
            }
            return BadRequest();
        }

        [HttpGet("Download/reportecajero/bolsascajero/{numeroCajero}/{turno}/{fecha}")]
        [Produces("application/json", "application/problem+json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GeneracionBolsas(string numeroCajero, int turno, DateTime fecha)
        {
            var result = await _ferromexService.GeneracionBolsasAsync(numeroCajero, turno, fecha);

            if (!result.Succeeded)
            {
                return BadRequest(result.ErrorMessage);
            }
            else
            {
                return Ok(result);
            }

            return NoContent();
        }

        #endregion

        static string? GetNullableString(string value) => !string.IsNullOrWhiteSpace(value) && value.ToUpper().Contains("NULL") ? null : value;
    }
}
