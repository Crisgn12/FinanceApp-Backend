using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades.DTOs;

/// Filtro para obtener los pagos próximos a vencer.
public class FiltroPagosProximosDTO
{
    public int UsuarioId { get; set; }
    /// Días de anticipación (por defecto 3).
    public int DiasAnticipacion { get; set; } = 3;
}
