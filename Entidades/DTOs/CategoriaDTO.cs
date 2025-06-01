using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades.DTOs
{
    public class CategoriaDTO
    {
        public int? CategoriaID { get; set; }

        public int? UsuarioID { get; set; }

        public string Nombre { get; set; } = null!;

        public string Tipo { get; set; } = null!;

        public bool EsPredeterminada { get; set; }
    }
}
