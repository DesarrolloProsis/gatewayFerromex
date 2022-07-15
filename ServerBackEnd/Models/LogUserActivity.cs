using System;
using System.Collections.Generic;

namespace ApiGateway.Models
{
    public partial class LogUserActivity
    {
        public int Id { get; set; }
        public string? IdModifiedUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string IdUpdatedUser { get; set; } = null!;
        public string TypeAction { get; set; } = null!;
        public string? AspNetRolesIdOld { get; set; }
        public string? AspNetRolesIdNew { get; set; }
        public string? OldName { get; set; }
        public string? NewName { get; set; }
        public string? OldLastName { get; set; }
        public string? NewLastName { get; set; }
        public string? OldPass { get; set; }
        public string? NewePass { get; set; }
        public bool Active { get; set; }
    }
}
