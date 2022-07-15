using System;
using System.Collections.Generic;

namespace ApiGateway.Models
{
    public partial class LogRole
    {
        public int Id { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string? IdUser { get; set; }
        public string AspNetRolesId { get; set; } = null!;
        public string TypeAction { get; set; } = null!;
        public string? OldNameRol { get; set; }
        public string? NewNameRol { get; set; }
        public bool Active { get; set; }
    }
}
