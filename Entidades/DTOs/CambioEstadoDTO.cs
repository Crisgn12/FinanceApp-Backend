using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades.DTOs
{
    public class CambioEstadoDTO
    {
        public int PagoId { get; set; }
        public string UsuarioId { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
    }
}
