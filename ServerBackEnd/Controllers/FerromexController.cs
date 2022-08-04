using ApiGateway.Interfaces;
using ApiGateway.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReportesData.Models;
using Shared;
using System.Globalization;
using System.Net.Mime;
using System.Security.Claims;
using System.Text.RegularExpressions;

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
                if (result.Succeeded) return Ok(result);
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
        [HttpGet("mantenimientotags/{paginaActual}/{numeroDeFilas}/{tag}/{estatus}/{fecha}/{noDePlaca}/{noEconomico}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MantenimientoTags))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTags(string? paginaActual, string? numeroDeFilas, string? tag, string? estatus, string? fecha, string? noDePlaca, string? noEconomico)
        {
            noDePlaca = GetNullableString(noDePlaca);
            noEconomico = GetNullableString(noEconomico);
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

            var tagsCountResponse = await _ferromexService.GetTagsCountAsync(tag, estatusBool, fechaDt, noDePlaca, noEconomico);
            var tags = tagsCountResponse.Content < numeroDeFilasInt
                ? await _ferromexService.GetTagsAsync(paginaActualInt, tagsCountResponse.Content, tag, estatusBool, fechaDt, noDePlaca, noEconomico)
                : await _ferromexService.GetTagsAsync(paginaActualInt, numeroDeFilasInt, tag, estatusBool, fechaDt, noDePlaca, noEconomico);

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

        /// <summary>
        /// Obtiene el Reporte de cruces totales en formato PDF
        /// </summary>
        /// <remarks>
        /// <para>Ejemplo de datos para obtener un PDF de ejemplo</para>
        /// <para>dia   2022-06-21</para>
        /// <para>mes   2022-06</para>
        /// <para>semana    2022-W26</para>
        /// </remarks>
        /// <param name="dia">Ej. 2022-06-21</param>
        /// <param name="mes">Ej. 2022-06</param>
        /// <param name="semana">Ej. 2022-W24</param>
        /// <response code="200">Se obtiene el PDF</response>        
        /// <response code="400">Alguno de los parametros no es validoo se encuentra en algun formato incorrecto</response>
        /// <response code="204">Error en el microServicio, no controlada por el gateway</response>  
        /// <returns>Se obtienen los cruces totales filtrandolos por los parametros pedidos anteriormente en formato PDF</returns>
        [HttpGet("Download/pdf/crucestotales/reporteCruces/{dia}/{mes}/{semana}")]
        [Produces("application/pdf")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DownloadReporteCrucesTotales(string? dia, string? mes, string? semana)
        {
            dia = GetNullableString(dia); //Se comprueba si lo que se obtuvo no es un espacio en blanco 
            mes = GetNullableString(mes); //Se comprueba si lo que se obtuvo no es un espacio en blanco 
            semana = GetNullableString(semana); //Se comprueba si lo que se obtuvo no es un espacio en blanco 

            string patternDia = @"(19|20)\d\d[-/.](0[1-9]|1[012])[-/.](0[1-9]|[1][0-9]|[2][0-9]|3[01])";

            if (dia != null)
            {
                if (Regex.IsMatch(dia, patternDia) == false)
                {
                    return BadRequest("El dia se encuentra en un formato incorrecto");
                }
            }

            string patternMes = @"(19|20)\d\d[-/.](0[1-9]|1[012])";

            if (mes != null)
            {
                if (Regex.IsMatch(mes, patternMes) == false)
                {
                    return BadRequest("El mes se encuentra en un formato incorrecto");
                }
            }

            string patternSemana = @"(19|20)\d\d[-/.](W0[1-9]|W1[0-9]|W2[0-9]|W3[0-9]|W4[0-9]|W5[0-3])";

            if (semana != null)
            {
                if (Regex.IsMatch(semana, patternSemana) == false)
                {
                    return BadRequest("La semana se encuentra en un formato incorrecto");
                }
            }

            var result = await _ferromexService.DownloadReporteCrucesTotalesAsync(dia, mes, semana); //Se llama al metodo asincrono de la interfaz, obteniendo el resultado del microServicio

            if (!result.Succeeded) //Se verifica que lo obtenido por el metodo no es nullo o erroneo a lo deseado
            {
                return StatusCode(result.Status, result.ErrorMessage); //Se devuelve al usuario el estatus del error del microServicio y el mensaje del error
            }
            else
            {
                return File(result.Content, "application/pdf", "Transacciones Ferromex Detalle.pdf"); //Se devuelve al usuario el PDF requerido
            }

            return NoContent(); //Si no se entro en ninguna de las anterior opciones, se devuelve un noContent
        }

        /// <summary>
        /// Obtiene el reporte de los cruces de feromex con descuento detalle en formato PDF
        /// </summary>
        /// <remarks>
        /// <para>Ejemplo de datos para obtener un PDF de ejemplo</para>
        /// <para>dia	2022-06-21</para>
        /// <para>mes   2022-06</para>
        /// <para>semana    2022-W26</para>
        /// </remarks>
        /// <param name="dia">Ej. 2022-06-21</param>
        /// <param name="mes">Ej. 2022-06</param>
        /// <param name="semana">Ej. 2022-W24</param>
        /// <response code="200">Se obtiene el PDF</response>        
        /// <response code="400">Alguno de los parametros no es validoo se encuentra en algun formato incorrecto</response>
        /// <response code="204">Error en el microServicio, no controlada por el gateway</response>  
        /// <returns>Se obtienen los cruces ferromex con descuento filtrandolos por los parametros pedidos anteriormente en formato PDF</returns>
        [HttpGet("Download/pdf/crucesferromex/descuentodetallesamarre/{dia}/{mes}/{semana}/{tag}/{noDePlaca}/{noEconomico}")]
        [Produces("application/pdf")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DownloadReporteCrucesFerromexDescuentoDetalle(string? dia, string? mes, string? semana, string? tag, string? noDePlaca, string? noEconomico)
        {
            dia = GetNullableString(dia); //Se comprueba si lo que se obtuvo no es un espacio en blanco 
            mes = GetNullableString(mes); //Se comprueba si lo que se obtuvo no es un espacio en blanco 
            semana = GetNullableString(semana); //Se comprueba si lo que se obtuvo no es un espacio en blanco 
            tag = GetNullableString(tag);
            noDePlaca = GetNullableString(noDePlaca);
            noEconomico = GetNullableString(noEconomico);

            string patternDia = @"(19|20)\d\d[-/.](0[1-9]|1[012])[-/.](0[1-9]|[1][0-9]|[2][0-9]|3[01])";

            if (dia != null)
            {
                if (Regex.IsMatch(dia, patternDia) == false)
                {
                    return BadRequest("El dia se encuentra en un formato incorrecto");
                }
            }

            string patternMes = @"(19|20)\d\d[-/.](0[1-9]|1[012])";

            if (mes != null)
            {
                if (Regex.IsMatch(mes, patternMes) == false)
                {
                    return BadRequest("El mes se encuentra en un formato incorrecto");
                }
            }

            string patternSemana = @"(19|20)\d\d[-/.](W0[1-9]|W1[0-9]|W2[0-9]|W3[0-9]|W4[0-9]|W5[0-3])";

            if (semana != null)
            {
                if (Regex.IsMatch(semana, patternSemana) == false)
                {
                    return BadRequest("La semana se encuentra en un formato incorrecto");
                }
            }

            var result = await _ferromexService.DownloadReporteCrucesFerromexDescuentoDetalleAsync(dia, mes, semana, tag, noDePlaca, noEconomico); //Se llama al metodo asincrono de la interfaz, obteniendo el resultado del microServicio

            if (!result.Succeeded) //Se verifica que lo obtenido por el metodo no es nullo o erroneo a lo deseado
            {
                return StatusCode(result.Status, result.ErrorMessage); //Se devuelve al usuario el estatus del error del microServicio y el mensaje del error
            }
            else
            {
                return File(result.Content, "application/pdf", "Descuentos Detalle Amarre.pdf"); //Se devuelve al usuario el PDF requerido
            }

            return NoContent();  //Si no se entro en ninguna de las anterior opciones, se devuelve un noContent
        }

        /// <summary>
        /// Obtiene el reporte de los cruces de feromex con descuento resumen en formato PDF
        /// </summary>
        /// <remarks>
        /// <para>Ejemplo de datos para obtener un PDF de ejemplo</para>
        /// <para>dia	2022-06-21</para>
        /// <para>mes   2022-06</para>
        /// <para>semana    2022-W26</para>
        /// </remarks>
        /// <param name="dia">Ej. 2022-06-21</param>
        /// <param name="mes">Ej. 2022-06</param>
        /// <param name="semana">Ej. 2022-W24</param>
        /// <response code="200">Se obtiene el PDF</response>        
        /// <response code="400">Alguno de los parametros no es validoo se encuentra en algun formato incorrecto</response>
        /// <response code="204">Error en el microServicio, no controlada por el gateway</response>  
        /// <returns>Se obtienen los cruces ferromex con descuento filtrandolos por los parametros pedidos anteriormente en formato PDF</returns>
        [HttpGet("Download/pdf/crucesferromex/{dia}/{mes}/{semana},/{tag}/{noDePlaca}/{noEconomico}")]
        [Produces("application/pdf")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DownloadReporteCrucesFerromexDescuentoResumen(string? dia, string? mes, string? semana, string? tag, string? noDePlaca, string? noEconomico)
        {
            dia = GetNullableString(dia); //Se comprueba si lo que se obtuvo no es un espacio en blanco 
            mes = GetNullableString(mes); //Se comprueba si lo que se obtuvo no es un espacio en blanco 
            semana = GetNullableString(semana); //Se comprueba si lo que se obtuvo no es un espacio en blanco 
            tag = GetNullableString(tag);
            noDePlaca = GetNullableString(noDePlaca);
            noEconomico = GetNullableString(noEconomico);

            string patternDia = @"(19|20)\d\d[-/.](0[1-9]|1[012])[-/.](0[1-9]|[1][0-9]|[2][0-9]|3[01])";

            if (dia != null)
            {
                if (Regex.IsMatch(dia, patternDia) == false)
                {
                    return BadRequest("El dia se encuentra en un formato incorrecto");
                }
            }

            string patternMes = @"(19|20)\d\d[-/.](0[1-9]|1[012])";

            if (mes != null)
            {
                if (Regex.IsMatch(mes, patternMes) == false)
                {
                    return BadRequest("El mes se encuentra en un formato incorrecto");
                }
            }

            string patternSemana = @"(19|20)\d\d[-/.](W0[1-9]|W1[0-9]|W2[0-9]|W3[0-9]|W4[0-9]|W5[0-3])";

            if (semana != null)
            {
                if (Regex.IsMatch(semana, patternSemana) == false)
                {
                    return BadRequest("La semana se encuentra en un formato incorrecto");
                }
            }

            var result = await _ferromexService.DownloadReporteCrucesFerromexDescuentoResumenAsync(dia, mes, semana, tag, noDePlaca, noEconomico);//Se llama al metodo asincrono de la interfaz, obteniendo el resultado del microServicio

            if (!result.Succeeded) //Se verifica que lo obtenido por el metodo no es nullo o erroneo a lo deseado
            {
                return StatusCode(result.Status, result.ErrorMessage); //Se devuelve al usuario el estatus del error del microServicio y el mensaje del error
            }
            else
            {
                return File(result.Content, "application/pdf", "Descuentos Detalle Resumen.pdf"); //Se devuelve al usuario el PDF requerido
            }

            return NoContent();  //Si no se entro en ninguna de las anterior opciones, se devuelve un noContent
        }

        /// <summary>
        /// Obtiene el reporte de los cruces de feromex concentrados en formato PDF
        /// </summary>
        /// <remarks>
        /// <para>Ejemplo de datos para obtener un PDF de ejemplo</para>
        /// <para>dia	2022-06-21</para>
        /// <para>mes   2022-06</para>
        /// <para>semana    2022-W26</para>
        /// </remarks>
        /// <param name="dia">Ej. 2022-06-21</param>
        /// <param name="mes">Ej. 2022-06</param>
        /// <param name="semana">Ej. 2022-W24</param>
        /// <response code="200">Se obtiene el PDF</response>        
        /// <response code="400">Alguno de los parametros no es validoo se encuentra en algun formato incorrecto</response>
        /// <response code="204">Error en el microServicio, no controlada por el gateway</response>  
        /// <returns>Se obtiene un concentrado de los cruces ferromex filtrandolos por los parametros pedidos anteriormente en formato PDF</returns>
        [HttpGet("Download/pdf/concentradosferromex/{dia}/{mes}/{semana}")]
        [Produces("application/pdf")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DownloadConcentradosFerromex(string? dia, string? mes, string? semana)
        {
            dia = GetNullableString(dia); //Se comprueba si lo que se obtuvo no es un espacio en blanco 
            mes = GetNullableString(mes); //Se comprueba si lo que se obtuvo no es un espacio en blanco 
            semana = GetNullableString(semana); //Se comprueba si lo que se obtuvo no es un espacio en blanco 

            string patternDia = @"(19|20)\d\d[-/.](0[1-9]|1[012])[-/.](0[1-9]|[1][0-9]|[2][0-9]|3[01])";

            if (dia != null)
            {
                if (Regex.IsMatch(dia, patternDia) == false)
                {
                    return BadRequest("El dia se encuentra en un formato incorrecto");
                }
            }

            string patternMes = @"(19|20)\d\d[-/.](0[1-9]|1[012])";

            if (mes != null)
            {
                if (Regex.IsMatch(mes, patternMes) == false)
                {
                    return BadRequest("El mes se encuentra en un formato incorrecto");
                }
            }

            string patternSemana = @"(19|20)\d\d[-/.](W0[1-9]|W1[0-9]|W2[0-9]|W3[0-9]|W4[0-9]|W5[0-3])";

            if (semana != null)
            {
                if (Regex.IsMatch(semana, patternSemana) == false)
                {
                    return BadRequest("La semana se encuentra en un formato incorrecto");
                }
            }

            var result = await _ferromexService.DownloadConcentradosFerromexAsync(dia, mes, semana); //Se llama al metodo asincrono de la interfaz, obteniendo el resultado del microServicio

            if (!result.Succeeded) //Se verifica que lo obtenido por el metodo no es nullo o erroneo a lo deseado
            {
                return StatusCode(result.Status, result.ErrorMessage); //Se devuelve al usuario el estatus del error del microServicio y el mensaje del error
            }
            else
            {
                return File(result.Content, "application/pdf", "Resumen Ingresos Ferromex.pdf"); //Se devuelve al usuario el PDF requerido
            }

            return NoContent(); //Si no se entro en ninguna de las anterior opciones, se devuelve un noContent
        }

        /// <summary>
        /// Obtiene el reporte de los mantenimientos de tags en formato PDF
        /// </summary>
        /// <remarks>
        /// <para>Ejemplo de datos para obtener un PDF de ejemplo</para>
        /// <para>tag	CPFI01376954</para>
        /// <para>estatus	ACTIVO</para>
        /// <para>fecha	2022-06-01</para>
        /// </remarks>
        /// <param name="tag">Ej.IMDM22961475</param>
        /// <param name="estatus">Ej. ACTIVO o INACTIVO</param>
        /// <param name="fecha">Ej. 2022-06-22</param>
        /// <response code="200">Se obtiene el PDF</response>        
        /// <response code="400">Alguno de los parametros no es validoo se encuentra en algun formato incorrecto</response>
        /// <response code="204">Error en el microServicio, no controlada por el gateway</response>  
        /// <returns>Se obtienen los tags en mantenimiento filtrandolos por los parametros pedidos anteriormente en formato PDF</returns>
        [HttpGet("Download/pdf/mantenimientotags/{tag}/{estatus}/{fecha}/{noDePlaca}/{noEconomico}")]
        [Produces("application/pdf")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DownloadMantenimientoTags(string? tag, string? estatus, string? fecha, string? noDePlaca, string? noEconomico)
        {
            tag = GetNullableString(tag); //Se comprueba si lo que se obtuvo no es un espacio en blanco 
            noDePlaca = GetNullableString(noDePlaca);
            noEconomico = GetNullableString(noEconomico);

            DateTime? fechaDt = null;

            if (!string.IsNullOrWhiteSpace(fecha)) //Se comprueba si lo que se obtuvo no es un espacio en blanco 
            {
                if (!DateTime.TryParse(fecha, out DateTime dt)) //Se covierte el string en dateTime y se comprueba si la fecha esta en formato correcto
                    return BadRequest($"La fecha '{fecha}' se encuentra en un formato incorrecto"); //Se devuelve un badRequest si la fecha obtenida esta en un formato incorrecto
                fechaDt = dt; //Si no esta en formato incorrecto, se guarda la fecha en una variable dateTime
            }

            bool? estatusBool = null;

            if (!string.IsNullOrWhiteSpace(estatus) && estatus.Trim().ToUpper().Equals("ACTIVO") //Se comprueba si lo recibido por el usuario no es un espacio en blanco y si el string recibido entra en algunos de esos parametros
               || !string.IsNullOrWhiteSpace(estatus) && estatus.Trim().ToUpper().Equals("TRUE"))
            {
                estatusBool = true;
            }
            if (!string.IsNullOrWhiteSpace(estatus) && estatus.Trim().ToUpper().Equals("INACTIVO") //Se comprueba si lo recibido por el usuario no es un espacio en blanco y si el string recibido entra en algunos de esos parametros
                || !string.IsNullOrWhiteSpace(estatus) && estatus.Trim().ToUpper().Equals("FALSE"))
            {
                estatusBool = false;
            }

            var result = await _ferromexService.DownloadMantenimientoTagsAsync(tag, estatusBool, fechaDt, noDePlaca, noEconomico); //Se llama al metodo asincrono de la interfaz, obteniendo el resultado del microServicio

            if (!result.Succeeded) //Se verifica que lo obtenido por el metodo no es nullo o erroneo a lo deseado
            {
                return StatusCode(result.Status, result.ErrorMessage); //Se devuelve al usuario el estatus del error del microServicio y el mensaje del error
            }
            else
            {
                return File(result.Content, "application/pdf", "Reporte Mantenimiento Tags.pdf"); //Se devuelve al usuario el PDF requerido
            }

            return NoContent(); //Si no se entro en ninguna de las anterior opciones, se devuelve un noContent
        }

        /// <summary>
        /// Obtiene el reporte del reporte operativo de cajero concentrado en formato PDF
        /// </summary>
        /// <remarks>
        /// <para>Ejemplo de datos para obtener un PDF de ejemplo</para>
        /// <para>IdBolsa	7</para>
        /// </remarks>
        /// <param name="idBolsa">Ej. 7</param>
        /// <response code="200">Se obtiene el PDF</response>        
        /// <response code="400">Alguno de los parametros no es validoo se encuentra en algun formato incorrecto</response>
        /// <response code="204">Error en el microServicio, no controlada por el gateway</response>  
        /// <returns>Se obtiene un concentrado del cajero filtrandolos por los parametros pedidos anteriormente en formato PDF</returns>
        [HttpGet("Download/pdf/reporteOperativo/reporteCajero/concentrado/{idBolsa}")]
        [Produces("application/pdf")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DownloadReporteOperativoCajeroConcentrado(string? idBolsa)
        {
            int idBolsaIn = 0;

            if (!string.IsNullOrWhiteSpace(idBolsa)) //Se comprueba si lo que se obtuvo no es un espacio en blanco
            {
                idBolsaIn = Convert.ToInt16(idBolsa); //Si no es un espacio en blanco se guarda lo obteniedo en una variable de tipo int
            }

            var result = await _ferromexService.DownloadReporteOperativoCajeroConcentradoAsync(idBolsaIn); //Se llama al metodo asincrono de la interfaz, obteniendo el resultado del microServicio

            if (!result.Succeeded) //Se verifica que lo obtenido por el metodo no es nullo o erroneo a lo deseado
            {
                return StatusCode(result.Status, result.ErrorMessage); //Se devuelve al usuario el estatus del error del microServicio y el mensaje del error
            }
            else
            {
                return File(result.Content, "application/pdf", "Concentrado Cajero.pdf"); //Se devuelve al usuario el PDF requerido
            }

            return NoContent(); //Si no se entro en ninguna de las anterior opciones, se devuelve un noContent
        }

        /// <summary>
        /// Obtiene el reporte del reporte operativo de cajero transacciones en formato PDF
        /// </summary>
        /// <remarks>
        /// <para>Ejemplo de datos para obtener un PDF de ejemplo</para>
        /// <para>IdBolsa	7</para>
        /// </remarks>
        /// <param name="idBolsa">Ej. 7</param>
        /// <response code="200">Se obtiene el PDF</response>        
        /// <response code="400">Alguno de los parametros no es validoo se encuentra en algun formato incorrecto</response>
        /// <response code="204">Error en el microServicio, no controlada por el gateway</response>  
        /// <returns>Se obtienen los cruces totales que hizo ese cajero filtrandolos por los parametros pedidos anteriormente en formato PDF</returns>
        [HttpGet("Download/pdf/reporteOperativo/reporteCajero/transacciones/{idBolsa}")]
        [Produces("application/pdf")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DownloadReporteOperativoCajeroTransacciones(string? idBolsa)
        {
            int idBolsaIn = 0;

            if (!string.IsNullOrWhiteSpace(idBolsa)) //Se comprueba si lo que se obtuvo no es un espacio en blanco
            {
                idBolsaIn = Convert.ToInt16(idBolsa); //Si no es un espacio en blanco se guarda lo obteniedo en una variable de tipo int
            }

            var result = await _ferromexService.DownloadReporteOperativoCajeroTransaccionesAsync(idBolsaIn); //Se llama al metodo asincrono de la interfaz, obteniendo el resultado del microServicio

            if (!result.Succeeded) //Se verifica que lo obtenido por el metodo no es nullo o erroneo a lo deseado
            {
                return StatusCode(result.Status, result.ErrorMessage); //Se devuelve al usuario el estatus del error del microServicio y el mensaje del error
            }
            else
            {
                return File(result.Content, "application/pdf", "Transacciones Cajero.pdf"); //Se devuelve al usuario el PDF requerido
            }

            return NoContent(); //Si no se entro en ninguna de las anterior opciones, se devuelve un noContent
        }

        /// <summary>
        /// Obtiene el reporte del reporte operativo de turno concentrado en formato PDF
        /// </summary>
        /// <remarks>
        /// <para>turno	1</para>
        /// <para>fecha	2022-06-21</para>
        /// </remarks>
        /// <param name="turno">Ej. 1, 2 o 3</param>
        /// <param name="fecha">Ej. 2022-06-22</param>
        /// <response code="200">Se obtiene el PDF</response>        
        /// <response code="400">Alguno de los parametros no es validoo se encuentra en algun formato incorrecto</response>
        /// <response code="204">Error en el microServicio, no controlada por el gateway</response>  
        /// <returns>Se obtiene el reporte operativo de turno filtrandolos por los parametros pedidos anteriormente en formato PDF</returns>
        [HttpGet("Download/pdf/reporteOperativo/reporteTurno/concentrado/{turno}/{fecha}")]
        [Produces("application/pdf")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DownloadReporteOperativoTurnoConcentrado(string? turno, string? fecha)
        {
            turno = GetNullableString(turno); //Se comprueba si lo que se obtuvo no es un espacio en blanco
            fecha = GetNullableString(fecha); //Se comprueba si lo que se obtuvo no es un espacio en blanco

            string patternDia = @"(19|20)\d\d[-/.](0[1-9]|1[012])[-/.](0[1-9]|[1][0-9]|[2][0-9]|3[01])";

            if (fecha != null)
            {
                if (Regex.IsMatch(fecha, patternDia) == false)
                {
                    return BadRequest("La fecha se encuentra en un formato incorrecto");
                }
            }

            int turnoI = 0;

            if (!string.IsNullOrWhiteSpace(turno)) //Se comprueba si lo que se obtuvo no es un espacio en blanco
            {
                turnoI = Convert.ToInt16(turno); //Si no es un espacio en blanco se guarda lo obteniedo en una variable de tipo int
            }

            var result = await _ferromexService.DownloadReporteOperativoTurnoConcentradoAsync(turnoI, fecha); //Se llama al metodo asincrono de la interfaz, obteniendo el resultado del microServicio

            if (!result.Succeeded) //Se verifica que lo obtenido por el metodo no es nullo o erroneo a lo deseado
            {
                return StatusCode(result.Status, result.ErrorMessage); //Se devuelve al usuario el estatus del error del microServicio y el mensaje del error
            }
            else
            {
                return File(result.Content, "application/pdf", "Concentrado Turno.pdf"); //Se devuelve al usuario el PDF requerido
            }

            return NoContent(); //Si no se entro en ninguna de las anterior opciones, se devuelve un noContent
        }

        /// <summary>
        /// Obtiene el reporte del reporte operativo de turno transacciones en formato PDF
        /// </summary>
        /// <remarks>
        /// <para>turno	1</para>
        /// <para>fecha	2022-06-21</para>
        /// </remarks>
        /// <param name="turno">Ej. 1, 2 o 3</param>
        /// <param name="fecha">Ej. 2022-06-22</param>
        /// <response code="200">Se obtiene el PDF</response>        
        /// <response code="400">Alguno de los parametros no es validoo se encuentra en algun formato incorrecto</response>
        /// <response code="204">Error en el microServicio, no controlada por el gateway</response>  
        /// <returns>Se obtiene el reporte operativo de las transacciones filtrandolos por los parametros pedidos anteriormente en formato PDF</returns>
        [HttpGet("Download/pdf/reporteOperativo/reporteTurno/transacciones/{turno}/{fecha}")]
        [Produces("application/pdf")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DownloadReporteOperativoTurnoTransacciones(string? turno, string? fecha)
        {
            turno = GetNullableString(turno); //Se comprueba si lo que se obtuvo no es un espacio en blanco
            fecha = GetNullableString(fecha); //Se comprueba si lo que se obtuvo no es un espacio en blanco

            string patternDia = @"(19|20)\d\d[-/.](0[1-9]|1[012])[-/.](0[1-9]|[1][0-9]|[2][0-9]|3[01])";

            if (fecha != null)
            {
                if (Regex.IsMatch(fecha, patternDia) == false)
                {
                    return BadRequest("La fecha se encuentra en un formato incorrecto");
                }
            }

            int turnoI = 0;

            if (!string.IsNullOrWhiteSpace(turno)) //Se comprueba si lo que se obtuvo no es un espacio en blanco
            {
                turnoI = Convert.ToInt16(turno); //Si no es un espacio en blanco se guarda lo obteniedo en una variable de tipo int
            }

            var result = await _ferromexService.DownloadReporteOperativoTurnoTransaccionesAsync(turnoI, fecha); //Se llama al metodo asincrono de la interfaz, obteniendo el resultado del microServicio

            if (!result.Succeeded) //Se verifica que lo obtenido por el metodo no es nullo o erroneo a lo deseado
            {
                return StatusCode(result.Status, result.ErrorMessage); //Se devuelve al usuario el estatus del error del microServicio y el mensaje del error
            }
            else
            {
                return File(result.Content, "application/pdf", "TransaccionesTurno.pdf"); //Se devuelve al usuario el PDF requerido
            }

            return NoContent(); //Si no se entro en ninguna de las anterior opciones, se devuelve un noContent
        }

        /// <summary>
        /// Obtiene las bolsas correspondientes al cajero y turno indicado
        /// </summary>
        /// <remarks>
        /// <para>Ejemplo de datos para obtener unas bolsas de ejemplo</para>
        /// <para>numCajero	500277</para>
        /// <para>turno	2</para>
        /// <para>fecha	2014-01-10</para>
        /// </remarks>
        /// <param name="numeroCajero">Ej. 555555</param>
        /// <param name="turno">Ej. 1, 2 o 3</param>
        /// <param name="fecha">Ej. 2022-06-22</param>
        /// <response code="200">Se obtiene las bolsas</response>        
        /// <response code="400">Alguno de los parametros no es validoo se encuentra en algun formato incorrecto</response>
        /// <response code="204">Error en el microServicio, no controlada por el gateway</response> 
        /// <returns>Se obtiene las bolsas con sus correspondientes datos, con ayuda de los anteriores filtros indicados anteriormente</returns>
        [HttpGet("reportecajero/bolsascajero/{numeroCajero}/{turno}/{fecha}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GeneracionBolsas(string? numeroCajero, string? turno, string? fecha)
        {
            int turnoI = 0;

            if (!string.IsNullOrWhiteSpace(turno)) //Se comprueba si lo que se obtuvo no es un espacio en blanco
            {
                turnoI = Convert.ToInt16(turno); //Si no es un espacio en blanco se guarda lo obteniedo en una variable de tipo int
            }

            DateTime? fechaDt = null;

            if (!string.IsNullOrWhiteSpace(fecha)) //Se comprueba si lo que se obtuvo no es un espacio en blanco 
            {
                if (!DateTime.TryParse(fecha, out DateTime dt)) //Se covierte el string en dateTime y se comprueba si la fecha esta en formato correcto
                    return BadRequest($"La fecha '{fecha}' se encuentra en un formato incorrecto"); //Se devuelve un badRequest si la fecha obtenida esta en un formato incorrecto
                fechaDt = dt; //Si no esta en formato incorrecto, se guarda la fecha en una variable dateTime
            }

            var result = await _ferromexService.GeneracionBolsasAsync(numeroCajero, turnoI, fechaDt); //Se llama al metodo asincrono de la interfaz, obteniendo el resultado del microServicio

            if (!result.Succeeded) //Se verifica que lo obtenido por el metodo no es nullo o erroneo a lo deseado
            {
                return StatusCode(result.Status, result.ErrorMessage); //Se devuelve al usuario el estatus del error del microServicio y el mensaje del error
            }
            else
            {
                return Ok(result); //Se devuelve un Ok con el resultado
            }

            return NoContent(); //Si no se entro en ninguna de las anterior opciones, se devuelve un noContent
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
        [HttpGet("registroInformacion/{paginaActual}/{numeroDeFilas}/{fecha}/{tag}/{carril}/{cuerpo}/{noDePlaca}/{noEconomico}/{clase}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CrucesPaginacion))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTransactions(string? paginaActual, string? numeroDeFilas, string? tag, string? carril, string? cuerpo, string? fecha, string? noDePlaca, string? noEconomico, string? clase)
        {
            clase = GetNullableString(clase);
            paginaActual = GetNullableString(paginaActual);
            numeroDeFilas = GetNullableString(numeroDeFilas);
            tag = GetNullableString(tag);
            carril = GetNullableString(carril);
            cuerpo = GetNullableString(cuerpo);
            fecha = GetNullableString(fecha);
            noDePlaca = GetNullableString(noDePlaca);
            noEconomico = GetNullableString(noEconomico);

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

            var tagsCountResponse = await _ferromexService.GetTransactionsCountAsync(tag, carril, cuerpo, fechaDt, noDePlaca, noEconomico, clase);
            var tags = tagsCountResponse.Content < numeroDeFilasInt
                ? await _ferromexService.GetTransactionsAsync(paginaActualInt, tagsCountResponse.Content, tag, carril, cuerpo, fechaDt, noDePlaca, noEconomico, clase)
                : await _ferromexService.GetTransactionsAsync(paginaActualInt, numeroDeFilasInt, tag, carril, cuerpo, fechaDt, noDePlaca, noEconomico, clase);

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

        //Cambios Richi
        [HttpGet("clases")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<TypeClass>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetClass()
        {
            var res = await _ferromexService.GetClassAsync();

            if (res != null)
            {
                List<TypeClass> clases = new();

                foreach (var clase in res.Content)
                {
                    clases.Add(new() { IdClass = clase.IdClass, NameClass = clase.NameClass, ClassCode = clase.ClassCode});
                }

                return Ok(clases);
            }
            return BadRequest();
        }

        /// <summary>
        /// Obtiene una lista de turnos
        /// </summary>                           
        /// <response code="200">Lista de turnos.</response>        
        /// <response code="400">Sin turnos para mostrar.</response>
        /// <response code="500">Error por excepcion no controlada en el Gateway.</response>        
        [HttpGet("turnos/{fecha}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Dictionary<string, string>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTurnos(string? fecha)
        {
            fecha = GetNullableString(fecha);

            DateTime fechaDt = DateTime.MinValue;
            if (!string.IsNullOrWhiteSpace(fecha))
            {
                if (!DateTime.TryParse(fecha, null, DateTimeStyles.RoundtripKind, out DateTime dt))
                    return BadRequest($"La fecha '{fecha}' se encuentra en un formato incorrecto");
                fechaDt = dt;
            }

            var res = await _ferromexService.GetTurnosAsync(fechaDt);
            if (res != null)
            {
                List<Turnos> turnos = new();
                foreach (var turno in res.Content)
                {
                    if (turno.HasValue)
                        turnos.Add(new(turno.ToString()));
                }
                if (turnos.Count > 0) return Ok(turnos);
                return NotFound();
            }
            return BadRequest();
        }

        public record Turnos(string? Value);
        #endregion
        static string? GetNullableString(string? value) => !string.IsNullOrWhiteSpace(value) && value.ToUpper().Contains("NULL") ? null : value;
    }
}
