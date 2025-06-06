using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades.DTOs;

public class PagoProgramadoDTO
{
    public int PagoId { get; set; }
    public string? UsuarioId { get; set; }
    public string Titulo { get; set; } = null!;
    public string? Descripcion { get; set; }
    public decimal Monto { get; set; }
    public DateOnly ProximoVencimiento { get; set; }
    public string Frecuencia { get; set; } = null!;
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public bool Activo { get; set; }
}
