using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades.DTOs
{
    public class ObtenerTransaccionesPorUsuarioDTO
    {
        public int UsuarioId { get; set; }
        public DateOnly? FechaInicio { get; set; }
        public DateOnly? FechaFin { get; set; }
        public string? NombreCategoria { get; set; }
    }
}
