﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades.DTOs
{
    public class UsuarioDTO
    {
        public int UsuarioId { get; set; }

        public string? Nombre { get; set; }

        public string? Apellidos { get; set; }

        public string? Email { get; set; }
    }
}
