using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades.DTOs
{
    public class TransaccionDTO
    {
        public int? TransaccionId { get; set; }

        public int UsuarioId { get; set; }

        public int CategoriaId { get; set; }

        public string? NombreCategoria { get; set; }

        public string Titulo { get; set; } = null!;

        public string? Descripcion { get; set; }

        public decimal Monto { get; set; }

        public DateOnly Fecha { get; set; }

        public string Tipo { get; set; } = null!;
    }
}
