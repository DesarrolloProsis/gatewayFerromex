using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shared
{
    public class Usuario
    {
        public string UsuarioId { get; set; }
        public string NombreUsuario { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Rol { get; set; }
        public string NombreCompleto { get; set; }
        public string Estatus { get; set; }
    }
    
    public class NuevoUsuario
    {
        public string Password { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Role { get; set; }
    }

    public class Respuesta
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string NombreUsuario { get; set; }
        public int Estatus { get; set; }
        public string EstatusText { get; set; }
    }
    public class RespuestaPaginacion
    {
        [JsonPropertyName("paginas_totales")]
        public int? PaginasTotales { get; set; }

        [JsonPropertyName("pagina_actual")]
        public int? PaginaActual { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<Usuario>? Usuarios { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<Rol>? Roles { get; set; }
    }
}
