using ApiGateway.Interfaces;
using ApiGateway.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReportesData.Models;
using Shared;
using System.Globalization;
using System.Net.Mime;
using System.Security.Claims;

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
        #region Modulos

        /// <summary>
        /// Obtiene el modulo indicado por el id
        /// </summary>        
        /// <param name="id">id del modulo a buscar</param>     
        /// <returns>Regresa el modulo encontrado con el id.</returns>        
        /// <response code="200">Se encontro modulo para el id.</response>        
        /// <response code="400">No se encontro modulo para el id.</response>
        /// <response code="500">Error por excepcion no controlada en el Gateway.</response>
        /// <returns></returns>
        [HttpGet("module/{id}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<Module>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<Module>))]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]                
        public async Task<ActionResult<ApiResponse<Module>>> GetModule(int id)
        {

            return Ok(await _ferromexService.GetModuleAsync(id));
        }

        /// <summary>
        /// Obtiene los modulos asociados a un rol
        /// </summary>        
        /// <param name="roleName">Nombre del rol a buscar</param>                
        /// <response code="200">Se encontro modulos para el rol.</response>        
        /// <response code="400">No se encontro modulos para el rol.</response>
        /// <response code="500">Error por excepcion no controlada en el Gateway.</response>
        /// <returns>Regresa coleccion de modulos asociados a un rol.</returns>     
        [HttpGet("modules")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<Module>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<Module>))]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]        
        public async Task<ActionResult<ApiResponse<List<Module>>>> GetModules(string? roleName)
        {
            return Ok(await _ferromexService.GetModulesAsync(roleName));
        }

        /// <summary>
        /// Inserta un nuevo modulo
        /// </summary>        
        /// <param name="module">Objeto necesario para insertar un nuevo modulo</param>               
        /// <response code="200">Se inserto un nuevo modulo.</response>        
        /// <response code="400">No se inserto el modulo.</response>
        /// <response code="500">Error por excepcion no controlada en el Gateway.</response>
        /// <returns>Regresa el moudlo creado</returns>
        [HttpPost("module")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<Module>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<Module>))]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]        
        public async Task<ActionResult<ApiResponse<Module>>> PostModule(Module module)
        {
            return Ok(await _ferromexService.PostModuleAsync(module));
        }


        /// <summary>
        /// Relaciona un modulo a un rol
        /// </summary>        
        /// <param name="roleModules">Objeto necesario para relacionar un modulo a un role</param>                   
        /// <response code="200">Se relaciono el moudulo a un rol.</response>        
        /// <response code="400">No se relaciono el modulo a un rol.</response>
        /// <response code="500">Error por excepcion no controlada en el Gateway.</response>        
        [HttpPost("addRoleModules")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<Module>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddRoleModules(RoleModules roleModules)
        {    
            if (ModelState.IsValid)
            {
                var result = await _ferromexService.PostRoleModulesAsync(roleModules);
                if(result.Succeeded) return Ok(result);
                return Ok(result);
            }
            return BadRequest(ModelState);
        }

        #endregion

        #region Mantenimiento Tags

        /// <summary>
        /// Obtiene una paginacion de tag aplicando filtros
        /// </summary>        
        /// <param name="paginaActual">Desde donde quiere iniciar la paginacion</param>   
        /// <param name="numeroDeFilas">Numero de registros por pagina</param>   
        /// <param name="tag">Numero de tag</param>   
        /// <param name="estatus">Estatus de los tags</param>   
        /// <param name="fecha">Filtro fecha para los tags</param>   
        /// <response code="200">Se obtiene el objeto para la paginacion de Tags.</response>        
        /// <response code="400">Alguno de los parametros no es valido.</response>
        /// <response code="500">Error por excepcion no controlada en el Gateway.</response>  
        /// <returns>Regresa pagiancion de tags</returns>
        [HttpGet("mantenimientotags/{paginaActual}/{numeroDeFilas}/{tag}/{estatus}/{fecha}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MantenimientoTags))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]             
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

            if (!string.IsNullOrWhiteSpace(paginaActual) && !string.IsNullOrWhiteSpace(numeroDeFilas))
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
                if (!DateTime.TryParse(fecha, null, DateTimeStyles.RoundtripKind, out DateTime dt))
                    return BadRequest($"La fecha '{fecha}' se encuentra en un formato incorrecto");
                fechaDt = dt;
            }

            var tagsCountResponse = await _ferromexService.GetTagsCountAsync(tag, estatusBool, fechaDt);
            var tags = tagsCountResponse.Content < numeroDeFilasInt 
                ? await _ferromexService.GetTagsAsync(paginaActualInt, tagsCountResponse.Content, tag, estatusBool, fechaDt) 
                : await _ferromexService.GetTagsAsync(paginaActualInt, numeroDeFilasInt, tag, estatusBool, fechaDt);    

            if (!tagsCountResponse.Succeeded)
            {
                return BadRequest(tagsCountResponse.ErrorMessage);
            }
            MantenimientoTags res = new()
            {
                PaginasTotales = paginaActualInt == null 
                    ? null 
                    : (int?)Math.Ceiling(decimal.Divide(tagsCountResponse.Content, numeroDeFilasInt ?? 1)),
                PaginaActual = paginaActualInt,
                Tags = tags.Content
            };
            return Ok(res);
        }

        /// <summary>
        /// Inserta un nuevo tag
        /// </summary>        
        /// <param name="tag">Objeto necesario para insertar un nuevo tag</param>               
        /// <response code="204">Se inserto un nuevo tag.</response>        
        /// <response code="400">Alguno de los parametros no es valido.</response>
        /// <response code="500">Error por excepcion no controlada en el Gateway.</response>        
        [HttpPost("agregartag")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]        
        public async Task<IActionResult> CreateTag(TagList tag)
        {
            tag.IdUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ModelState.IsValid)
            {
                var res = await _ferromexService.CreateTagAsync(tag);
                if (res.Succeeded) return NoContent();
            }
            return BadRequest(ModelState);
        }

        /// <summary>
        /// Edita un tag en especifico
        /// </summary>        
        /// <param name="tag">Objeto necesario para editar el tag</param>               
        /// <response code="204">Se inserto un nuevo tag.</response>        
        /// <response code="400">Alguno de los parametros no es valido.</response>
        /// <response code="500">Error por excepcion no controlada en el Gateway.</response>        
        [HttpPut("editartag")]
        [Produces("application/json", "application/problem+json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]        
        public async Task<IActionResult> UpdateTag(TagList tag)
        {
            tag.IdUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ModelState.IsValid)
            {
                var res = await _ferromexService.UpdateTagAsync(tag);
                if (res.Succeeded) return NoContent();               
            }
            return BadRequest(ModelState);
        }

        /// <summary>
        /// Elimina un tag en especifico
        /// </summary>        
        /// <param name="tag">Objeto necesario para eliminar el tag</param>               
        /// <response code="204">Se elimino el tag.</response>        
        /// <response code="400">Alguno de los parametros no es valido.</response>
        /// <response code="500">Error por excepcion no controlada en el Gateway.</response>        
        [HttpDelete("eliminartag/{tag}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]        
        public async Task<IActionResult> DeleteTag(string tag)
        {
            if (!string.IsNullOrWhiteSpace(tag))
            {
                var res = await _ferromexService.DeleteTagAsync(tag);
                if (res.Succeeded) return NoContent();
                return BadRequest(res.ErrorMessage);
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

        #region Telepeaje

        /// <summary>
        /// Obtine una paginacion de tag aplicando filtros
        /// </summary>        
        /// <param name="paginaActual">Desde donde quiere iniciar la paginacion</param>   
        /// <param name="numeroDeFilas">Numero de registros por pagina</param>   
        /// <param name="tag">Numero de tag</param>   
        /// <param name="carril">Numero de carril</param>   
        /// <param name="cuerpo">Cuerpo del carril</param>   
        /// <param name="fecha">Filtro fecha para los tags</param>  
        /// <response code="200">Se obtiene el objeto para la paginacion de Transacciones.</response>        
        /// <response code="400">Alguno de los parametros no es valido.</response>
        /// <response code="500">Error por excepcion no controlada en el Gateway.</response>  
        /// <returns>Regresa paginacion de tags</returns>
        [HttpGet("registroInformacion/{paginaActual}/{numeroDeFilas}/{fecha}/{tag}/{carril}/{cuerpo}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CrucesPaginacion))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]        
        public async Task<IActionResult> GetTransactions(string? paginaActual, string? numeroDeFilas, string? tag, string? carril, string? cuerpo, string? fecha)
        {
            paginaActual = GetNullableString(paginaActual);
            numeroDeFilas = GetNullableString(numeroDeFilas);
            tag = GetNullableString(tag);
            carril = GetNullableString(carril);
            cuerpo = GetNullableString(cuerpo);
            fecha = GetNullableString(fecha);

            DateTime? fechaDt = null;
            int? paginaActualInt = null;
            int? numeroDeFilasInt = null;

            if (!string.IsNullOrWhiteSpace(paginaActual) && !string.IsNullOrWhiteSpace(numeroDeFilas))
            {
                if (!int.TryParse(paginaActual.Trim(), out int pa) || !int.TryParse(numeroDeFilas.Trim(), out int ndf))
                {
                    return BadRequest($"La paginacion se encuentra en un formato incorrecto");
                }
                paginaActualInt = pa;
                numeroDeFilasInt = ndf;
            }
            if (!string.IsNullOrWhiteSpace(cuerpo) && cuerpo.Trim().ToUpper().Equals("A")
                || !string.IsNullOrWhiteSpace(cuerpo) && cuerpo.Trim().ToUpper().Equals("B"))
            {
                cuerpo = $"Cuerpo {cuerpo.Trim().ToUpper()}";
            }
            if (!string.IsNullOrWhiteSpace(fecha))
            {
                if (!DateTime.TryParse(fecha, null, DateTimeStyles.RoundtripKind, out DateTime dt))
                    return BadRequest($"La fecha '{fecha}' se encuentra en un formato incorrecto");
                fechaDt = dt;
            }
            
            var tagsCountResponse = await _ferromexService.GetTransactionsCountAsync(tag, carril, cuerpo, fechaDt);            
            var tags = tagsCountResponse.Content < numeroDeFilasInt
                ? await _ferromexService.GetTransactionsAsync(paginaActualInt, tagsCountResponse.Content, tag, carril, cuerpo, fechaDt)
                : await _ferromexService.GetTransactionsAsync(paginaActualInt, numeroDeFilasInt, tag, carril, cuerpo, fechaDt);

            if (!tagsCountResponse.Succeeded)
            {
                return BadRequest(tagsCountResponse.ErrorMessage);
            }

            CrucesPaginacion res = new()
            {
                PaginasTotales = paginaActualInt == null 
                    ? null 
                    : (int?)Math.Ceiling(decimal.Divide(tagsCountResponse.Content, numeroDeFilasInt ?? 1)),
                PaginaActual = paginaActualInt,
                Cruces = tags.Content
            };
            return Ok(res);
        }

        /// <summary>
        /// Obtiene una lista de carriles
        /// </summary>                           
        /// <response code="200">Lista de carriles.</response>        
        /// <response code="400">Sin carriles para mostrar.</response>
        /// <response code="500">Error por excepcion no controlada en el Gateway.</response>        
        [HttpGet("carriles")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Carril>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]        
        public async Task<IActionResult> GetLanes()
        {
            var res = await _ferromexService.GetLanesAsync();
            if (res != null)
            {
                List<Carril> carriles = new();                
                foreach (var carril in res.Content)
                {
                    carriles.Add(new() { Id = carril.IdLane, Nombre = carril.Name });
                }
                return Ok(carriles);
            }
            return BadRequest();
        }

        #endregion
        static string? GetNullableString(string? value) => !string.IsNullOrWhiteSpace(value) && value.ToUpper().Contains("NULL") ? null : value;
    }
}
