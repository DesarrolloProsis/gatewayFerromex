using ApiGateway.Interfaces;
using ApiGateway.Models;
using ApiGateway.Services;
using MediatR;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using Shared;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using System.Security.Claims;
using System.Text.RegularExpressions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace ApiGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : Controller
    {
        private readonly IMediator _mediator;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUsuariosService _usuariosService;

        public IdentityController(IMediator mediator, SignInManager<ApplicationUser> signInManager, IUsuariosService usuariosService)
        {
            _mediator = mediator;
            _signInManager = signInManager;
            _usuariosService = usuariosService;
        }

        /// <summary>
        /// Registrar nuevo usuario 
        /// </summary>
        /// <param name="createCommand">Parametros de registro</param>
        /// <returns>Regresa el usuario creado.</returns>
        /// <response code="201">Regresa el objeto creado</response>
        /// <response code="400">Alguno de los datos requeridos es incorrecto</response>
        /// <response code="500">Error por excepcion no controlada en el Gateway</response>
        [HttpPost("register")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces("application/json", "application/problem+json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register(UserCreateCommand createCommand)
        {
            if (ModelState.IsValid)
            {
                var result = await _mediator.Send(createCommand);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }
                var userName = createCommand.Nombre[..3]
                    + Regex.Replace(createCommand.Apellidos, @"\s+", "");
                User? res = await _mediator.Send(new UserFindCommand
                {
                    Username = userName
                });
                //return CreatedAtAction("Find", new { res.Id }, res);
                Respuesta respuesta = new() { Estatus = 200, EstatusText = "OK", NombreUsuario = userName };
                return Ok(respuesta);
            }
            return BadRequest();
        }

        /// <summary>
        /// Registrar roles de usuario 
        /// </summary>
        /// <param name="addRolesCommand">Parametros de registro</param>
        /// <returns>Regresa el codigo de estado.</returns>
        /// <response code="204">Se agregaron correctamento los roles al usuario</response>
        /// <response code="400">Alguno de los datos requeridos es incorrecto</response>
        /// <response code="500">Error por excepcion no controlada en el Gateway</response>
        [HttpPost("addUserRoles")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces("application/json", "application/problem+json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(IEnumerable<IdentityError>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddUserRoles(UserAddRolesCommand addRolesCommand)
        {
            if (ModelState.IsValid)
            {
                var result = await _mediator.Send(addRolesCommand);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }
                return NoContent();
            }
            return BadRequest();
        }

        /// <summary>
        /// Registrar roles de usuario 
        /// </summary>
        /// <param name="addRolesCommand">Parametros de registro</param>
        /// <returns>Regresa el codigo de estado.</returns>
        /// <response code="204">Se agregaron correctamento los roles al usuario</response>
        /// <response code="400">Alguno de los datos requeridos es incorrecto</response>
        /// <response code="500">Error por excepcion no controlada en el Gateway</response>
        [HttpPost("addRoles")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces("application/json", "application/problem+json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(IEnumerable<IdentityError>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddRoles(AddRolesCommand addRolesCommand)
        {
            if (ModelState.IsValid)
            {
                var result = await _mediator.Send(addRolesCommand);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }
                return NoContent();
            }
            return BadRequest();
        }

        /// <summary>
        /// Registrar roles de usuario 
        /// </summary>
        /// <returns>Regresa el codigo de estado.</returns>
        /// <response code="200">Se agregaron correctamento los roles al usuario</response>
        /// <response code="400">Alguno de los datos requeridos es incorrecto</response>
        /// <response code="500">Error por excepcion no controlada en el Gateway</response>
        //[HttpGet("roles")]
        //[Produces("application/json", "application/problem+json")]
        //[ProducesResponseType(typeof(List<IdentityRole>), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(IEnumerable<IdentityError>), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        //public async Task<IActionResult> GetRoles()
        //{
        //    var result = await _mediator.Send(new GetRolesCommand());
        //    if (result == null || result?.Count < 1)
        //    {
        //        return BadRequest();
        //    }
        //    return Ok(result);
        //}

        /// <summary>
        /// Consultar usuario
        /// </summary>
        /// <param name="id">Realizar busqueda por Id</param>
        /// <param name="email">Realizar busqueda por email</param>
        /// <param name="userName">Realizar busqueda por nombre de usuario</param>
        /// <returns>Regresa la información del usuario consultado</returns>
        /// <response code="200">Regresa el objeto solicitado</response>
        /// <response code="400">Alguno de los datos requeridos es incorrecto</response>
        /// <response code="404">No se pudo encontrar el objeto solicitado</response>
        /// <response code="500">Error por excepcion no controlada en el Gateway</response>
        //[HttpGet("user")]
        //[Produces("application/json", "application/problem+json")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status404NotFound)]
        //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        //public async Task<IActionResult> Find(string? id = null, string? email = null, string? userName = null)
        //{
        //    if (!string.IsNullOrEmpty(id) || !string.IsNullOrEmpty(email) || !string.IsNullOrEmpty(userName))
        //    {
        //        UserFindCommand findCommand = new() { Id = id, Email = email, Username = userName };
        //        var result = await _mediator.Send(findCommand);
        //        if (!result.Succeeded)
        //        {
        //            return NotFound();
        //        }
        //        return Ok(result);
        //    }
        //    return BadRequest();
        //}

        //[HttpPost("token")]
        //public async Task<IActionResult> ExchangeAsync()
        //{
        //    var request = HttpContext.GetOpenIddictServerRequest() ??
        //                  throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        //    ClaimsPrincipal claimsPrincipal;

        //    ForbidResult? forbidResult = new(authenticationScheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        //    if (request.IsClientCredentialsGrantType())
        //    {
        //        var identity = new ClaimsIdentity(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        //        identity.AddClaim(Claims.Subject, request.ClientId ?? throw new InvalidOperationException());

        //        claimsPrincipal = new ClaimsPrincipal(identity);

        //        claimsPrincipal.SetScopes(request.GetScopes());
        //    }
        //    else if (request.IsAuthorizationCodeGrantType())
        //    {
        //        var authorization = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        //        if (!authorization.Succeeded)
        //        {
        //            forbidResult.Properties = new AuthenticationProperties(new Dictionary<string, string?>
        //            {
        //                [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidRequest,
        //                [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = authorization.Failure?.Message
        //            });
        //            return forbidResult;
        //        }

        //        claimsPrincipal = authorization.Principal;
        //    }
        //    else if (request.IsPasswordGrantType())
        //    {
                
        //        UserLoginCommand loginCommand = new()
        //        {
        //            UserName = request.Username,
        //            Password = request.Password,
        //            IsRfc6749 = true,
        //            //RefreshToken = Request.Cookies["refreshToken"],
        //        };
        //        var identity = await _mediator.Send(loginCommand);
        //        if (!identity.Succeeded)
        //        {
        //            forbidResult.Properties = new AuthenticationProperties(new Dictionary<string, string?>
        //            {
        //                [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidRequest,
        //                [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = identity.ErrorDescription
        //            });
        //            return forbidResult;
        //        }
        //        claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(identity.User);
        //        claimsPrincipal.SetScopes(request.GetScopes());
        //    }
        //    else
        //    {
        //        throw new InvalidOperationException("The specified grant type is not supported.");
        //    }

        //    foreach (var claim in claimsPrincipal.Claims)
        //    {
        //        claim.SetDestinations(GetDestinations(claim, claimsPrincipal));
        //    }

        //    return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        //}

        //[HttpGet("authorize")]
        //[HttpPost("authorize")]
        //[IgnoreAntiforgeryToken]
        //public async Task<IActionResult> Authorize()
        //{
        //    var request = HttpContext.GetOpenIddictServerRequest() ??
        //        throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        //    var authenticate = await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme);

        //    if (!authenticate.Succeeded)
        //    {
        //        return Challenge(
        //            authenticationSchemes: IdentityConstants.ApplicationScheme,
        //            properties: new AuthenticationProperties
        //            {
        //                RedirectUri = Request.PathBase + Request.Path + QueryString.Create(
        //                    Request.HasFormContentType ? Request.Form.ToList() : Request.Query.ToList())
        //            });
        //    }

        //    UserLoginCommand loginCommand = new()
        //    {
        //        UserName = authenticate.Principal.Identity?.Name,
        //        //RefreshToken = Request.Cookies["refreshToken"],
        //    };

        //    var identity = await _mediator.Send(loginCommand);
        //    if (!identity.Succeeded)
        //    {
        //        ForbidResult? forbidResult = new(authenticationScheme: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)
        //        {
        //            Properties = new AuthenticationProperties(new Dictionary<string, string?>
        //            {
        //                [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidRequest,
        //                [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = identity.ErrorDescription
        //            })
        //        };
        //        return forbidResult;
        //    }

        //    ClaimsPrincipal claimsPrincipal;
        //    claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(identity.User);
        //    claimsPrincipal.SetScopes(request.GetScopes());

        //    return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        //}

        //[HttpGet("logout")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        //public IActionResult Logout() => View();

        //[HttpPost("logout"), ActionName(nameof(Logout)), ValidateAntiForgeryToken]
        //public async Task<IActionResult> LogoutPost()
        //{
        //    await _signInManager.SignOutAsync();

        //    return SignOut(
        //        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
        //        properties: new AuthenticationProperties
        //        {
        //            RedirectUri = "/"
        //        });
        //}

        /// <summary>
        /// Solicitar JWT
        /// </summary>
        /// <param name="loginCommand">Parametros de autenticación</param>
        /// <returns>Regresa el token JWT al usuario autenticado.</returns>
        /// <response code="200">Regresa el objeto solicitado</response>
        /// <response code="400">Alguno de los datos requeridos es incorrecto</response>
        /// <response code="500">Error por excepcion no controlada en el Gateway</response>
        [HttpPost("login")]
        [Consumes("application/json")]
        [Produces("application/json", "application/problem+json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login(UserLoginCommand loginCommand)
        {
            if (ModelState.IsValid)
            {
                if (User.Identity != null && User.Identity.IsAuthenticated)
                {
                    loginCommand.UserName = User.Identity.Name;
                    //loginCommand.RefreshToken = Request.Cookies["refreshToken"];
                    loginCommand.RefreshToken = loginCommand.RefreshToken;
                }
                var result = await _mediator.Send(loginCommand);
                if (!result.Succeeded)
                {
                    return BadRequest(result);
                }
                //if (!string.IsNullOrEmpty(result.RefreshToken))
                //    RefreshTokenCookie(result.RefreshToken);
                return Ok(result);
            }
            return BadRequest();
        }

        /// <summary>
        /// Solicitar nuevo JWT
        /// </summary>
        /// <returns>Reexpide nuevos tokens al usuario autenticado por refresh_token</returns>
        /// <response code="200">Regresa el objeto solicitado</response>
        /// <response code="400">Alguno de los datos requeridos es incorrecto</response>
        /// <response code="500">Error por excepcion no controlada en el Gateway</response>
        //[HttpPost("refreshtoken")]
        //[Produces("application/json", "application/problem+json")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        //public async Task<IActionResult> RefreshToken()
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        UserLoginCommand loginCommand = new()
        //        {
        //            Email = (User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Email))?.Value),
        //            RefreshToken = Request.Cookies["refreshToken"],
        //        };
        //        var result = await _mediator.Send(loginCommand);
        //        if (!result.Succeeded)
        //        {
        //            return BadRequest(result);
        //        }
        //        RefreshTokenCookie(result.RefreshToken);
        //        return Ok(result);
        //    }
        //    return BadRequest();

        //}

        private void RefreshTokenCookie(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }

        private IEnumerable<string> GetDestinations(Claim claim, ClaimsPrincipal principal)
        {
            // Note: by default, claims are NOT automatically included in the access and identity tokens.
            // To allow OpenIddict to serialize them, you must attach them a destination, that specifies
            // whether they should be included in access tokens, in identity tokens or in both.

            switch (claim.Type)
            {
                case Claims.Name:
                    yield return Destinations.AccessToken;

                    if (principal.HasScope(Scopes.Profile))
                        yield return Destinations.IdentityToken;

                    yield break;

                case Claims.Email:
                    yield return Destinations.AccessToken;

                    if (principal.HasScope(Scopes.Email))
                        yield return Destinations.IdentityToken;

                    yield break;

                case Claims.Role:
                    yield return Destinations.AccessToken;

                    if (principal.HasScope(Scopes.Roles))
                        yield return Destinations.IdentityToken;

                    yield break;

                // Never include the security stamp in the access and identity tokens, as it's a secret value.
                case "AspNet.Identity.SecurityStamp": yield break;

                default:
                    yield return Destinations.AccessToken;
                    yield break;
            }
        }

        ///EPs GD
        #region EPs GD

        [HttpGet("user/{paginaActual}/{numeroDeFilas}/{nombre}/{estatus}")]
        public async Task<IActionResult> GetUsuarios(string? paginaActual, string? numeroDeFilas, string? nombre, string? estatus)
        {
            bool? estatusBool = null;
            int? paginaActualInt = null;
            int? numeroDeFilasInt = null;

            if (!string.IsNullOrWhiteSpace(paginaActual) && !string.IsNullOrWhiteSpace(numeroDeFilas))
            {
                if (!int.TryParse(paginaActual.Trim(), out int pa) || !int.TryParse(numeroDeFilas.Trim(), out int ndf))
                {
                    throw new ValidationException($"La paginacion se encuentra en un formato incorrecto");
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

            
            var count = await _usuariosService.CountUsuariosAsync(nombre, estatusBool);
            var usuarios = count < numeroDeFilasInt
                ? await _usuariosService.GetUsuariosAsync(paginaActualInt, count, nombre, estatusBool)
                : await _usuariosService.GetUsuariosAsync(paginaActualInt, numeroDeFilasInt, nombre, estatusBool);

            RespuestaPaginacion respuesta = new()
            {
                PaginasTotales = paginaActualInt == null 
                    ? null 
                    : (int?)Math.Ceiling(decimal.Divide(count, numeroDeFilasInt ?? 1)),
                PaginaActual = paginaActualInt,
                Usuarios = usuarios
            };
            return Ok(respuesta);
        }

        [HttpGet("roles/{paginaActual}/{numeroDeFilas}/{nombreRol}/{estatus}")]
        public async Task<IActionResult> GetRoles(string? paginaActual, string? numeroDeFilas, string? nombreRol, string? estatus)
        {
            bool? estatusBool = null;
            int? paginaActualInt = null;
            int? numeroDeFilasInt = null;

            if (!string.IsNullOrWhiteSpace(paginaActual) && !string.IsNullOrWhiteSpace(numeroDeFilas))
            {
                if (!int.TryParse(paginaActual.Trim(), out int pa) || !int.TryParse(numeroDeFilas.Trim(), out int ndf))
                {
                    throw new ValidationException($"La paginacion se encuentra en un formato incorrecto");
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

            
            var countRoles = await _usuariosService.CountRolesAsync(nombreRol, estatusBool);
            var roles = countRoles < numeroDeFilasInt
                ? await _usuariosService.GetRolesAsync(paginaActualInt, numeroDeFilasInt, nombreRol, estatusBool)
                : await _usuariosService.GetRolesAsync(paginaActualInt, numeroDeFilasInt, nombreRol, estatusBool);

            RespuestaPaginacion respuesta = new()
            {
                PaginasTotales = paginaActualInt == null 
                    ? null 
                    : (int?)Math.Ceiling(decimal.Divide(countRoles, numeroDeFilasInt ?? 1)),
                PaginaActual = paginaActualInt,
                Roles = roles
            };
            return Ok(respuesta);
        }

        [HttpPut("editRole")]
        public async Task<IActionResult> UpdateRole(Rol rol)
        {
            try
            {
                var res = await _usuariosService.UpdateRoleAsync(rol);
                if (res)
                    return Ok(new Respuesta() { Estatus = 200, EstatusText = "OK" });
                return BadRequest(new Respuesta() { Estatus = 400, EstatusText = "Rol invalido" });
            }
            catch (Exception e)
            {
                return BadRequest(new Respuesta() { Estatus = 400, EstatusText = e.Message });
            }
        }

        //[HttpPost("register")]
        //public async Task<IActionResult> CreateUsuario(NuevoUsuario nuevoUsuario)
        //{
        //    return BadRequest();
        //}

        [HttpPut("editUser")]
        public async Task<IActionResult> UpdateUsuario(Usuario usuario)
        {
            try
            {
                var res = await _usuariosService.UpdateUsuarioAsync(usuario);
                if (res)
                    return Ok(new Respuesta() { Estatus = 200, EstatusText = "OK" });
                return BadRequest(new Respuesta() { Estatus = 400, EstatusText = "Usuario invalido" });
            }
            catch (Exception e)
            {
                return BadRequest(new Respuesta() { Estatus = 400, EstatusText = e.Message });
            }
        }

        [HttpPut("changePassword")]
        public async Task<IActionResult> UpdatePassword(UsuarioUpdatePassword usuarioUpdatePassword)
        {
            try
            {
                var res = await _usuariosService.UpdateUsuarioPasswordAsync(usuarioUpdatePassword);
                if (res)
                    return Ok(new Respuesta() { Estatus = 200, EstatusText = "OK" });
                return BadRequest(new Respuesta() { Estatus = 400, EstatusText = "Usuario invalido" });
            }
            catch (Exception e)
            {
                return BadRequest(new Respuesta() { Estatus = 400, EstatusText = e.Message });
            }
        }

        #endregion
    }
}
