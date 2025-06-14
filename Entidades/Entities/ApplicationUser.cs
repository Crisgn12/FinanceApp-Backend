﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades.Entities
{
    public class ApplicationUser: IdentityUser
    {
        public string Nombre { get; set; } = null!;
        public string Apellidos { get; set; } = null!;
        public DateTime? FechaCreacion { get; set; }
    }
}
