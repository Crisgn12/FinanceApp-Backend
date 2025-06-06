using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades.DTOs
{
    public class ObtenerDetalleTransaccionDTO
    {
        public int TransaccionId { get; set; }
        public string? UsuarioId { get; set; }
    }
}
