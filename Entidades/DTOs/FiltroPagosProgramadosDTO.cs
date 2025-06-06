using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades.DTOs;

/// Filtro para listar pagos programados (rango de fechas, activos, etc.).

public class FiltroPagosProgramadosDTO
{
    public string? UsuarioId { get; set; }
    public DateOnly? FechaInicio { get; set; }
    public DateOnly? FechaFin { get; set; }
    public bool? SoloActivos { get; set; }
}
