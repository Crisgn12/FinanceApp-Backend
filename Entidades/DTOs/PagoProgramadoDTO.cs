using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades.DTOs;

public class PagoProgramadoDTO
{
    public int PagoId { get; set; }
    public int UsuarioId { get; set; }
    public string Titulo { get; set; } = null!;
    public string? Descripcion { get; set; }
    public decimal Monto { get; set; }
    public DateOnly ProximoVencimiento { get; set; }
    public string Frecuencia { get; set; } = null!;
    public bool Activo { get; set; }
}
