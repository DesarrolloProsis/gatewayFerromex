using ApiGateway.Interfaces;
using ApiGateway.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReportesData.Models;
using Shared;
using System.Globalization;
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
        /// Obtiene los modulos asociados a un role
        /// </summary>        
        /// <param name="role">Nombre del rol a buscar</param>                
        /// <response code="200">Se encontro modulos para el role.</response>        
        /// <response code="400">No se encontro modulos para el role.</response>
        /// <response code="500">Error por excepcion no controlada en el Gateway.</response>
        /// <returns>Regresa coleccion de modulos asociados a un rol.</returns>     
        [HttpGet("modules")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<Module>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<Module>))]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]        
        public async Task<ActionResult<ApiResponse<List<Module>>>> GetModules(string? role)
        {
            return Ok(await _ferromexService.GetModulesAsync(role));
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
        /// Relaciona un modulo a un role
        /// </summary>        
        /// <param name="roleModules">Objeto necesario para relacionar un modula a un role</param>                   
        /// <response code="200">Se relaciono el moudulo a un rol.</response>        
        /// <response code="400">No se relaciono el moudulo a un rol.</response>
        /// <response code="500">Error por excepcion no controlada en el Gateway.</response>        
        [HttpPost("addRoleModules")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<Module>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<Module>))]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]        
        public async Task<IActionResult> AddRoleModules(RoleModules roleModules)
        {    
            if (ModelState.IsValid)
            {
                var result = await _ferromexService.PostRoleModulesAsync(roleModules);
                return Ok(result);
            }
            return BadRequest();
        }

        #endregion


        #region Mantenimiento Tags

        /// <summary>
        /// Obtine una pagiancion de tag aplicando filtros
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
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]        
        public async Task<IActionResult> CreateTag(TagList tag)
        {
            if (ModelState.IsValid)
            {
                var res = await _ferromexService.CreateTagAsync(tag);
                if (res.Succeeded) return NoContent();
            }
            return BadRequest();
        }

        /// <summary>
        /// Edita un tag en especifico
        /// </summary>        
        /// <param name="tag">Objeto necesario para editar el tag</param>               
        /// <response code="204">Se inserto un nuevo tag.</response>        
        /// <response code="400">Alguno de los parametros no es valido.</response>
        /// <response code="500">Error por excepcion no controlada en el Gateway.</response>        
        [HttpPut("editartag")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]        
        public async Task<IActionResult> UpdateTag(TagList tag)
        {
            if (ModelState.IsValid)
            {
                var res = await _ferromexService.UpdateTagAsync(tag);
                if (res.Succeeded) return NoContent();               
            }
            return BadRequest();
        }

        /// <summary>
        /// Elimina un tag en especifico
        /// </summary>        
        /// <param name="tag">Objeto necesario para eliminar el tag</param>               
        /// <response code="204">Se inserto un nuevo tag.</response>        
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
            }
            return BadRequest();
        }

        #endregion

        #region Telepeaje

        [HttpGet("registroInformacion/{paginaActual}/{numeroDeFilas}/{fecha}/{tag}/{carril}/{cuerpo}")]
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
            var tags = await _ferromexService.GetTransactionsAsync(paginaActualInt, numeroDeFilasInt, tag, carril, cuerpo, fechaDt);
            var tagsCountResponse = await _ferromexService.GetTransactionsCountAsync(tag, carril, cuerpo, fechaDt);

            if (!tagsCountResponse.Succeeded)
            {
                return BadRequest(tagsCountResponse.ErrorMessage);
            }

            CrucesPaginacion res = new()
            {
                PaginasTotales = paginaActualInt
                == null ? null : (int?)Math.Ceiling(decimal.Divide(tagsCountResponse.Content, numeroDeFilasInt ?? 1)),
                PaginaActual = paginaActualInt,
                Cruces = tags.Content
            };
            return Ok(res);
        }

        [HttpGet("carriles")]
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
