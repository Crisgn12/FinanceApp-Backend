using System;
using System.Collections.Generic;

namespace Entidades.Entities;

public partial class Pago
{
    public int PagoId { get; set; }

    public int UsuarioId { get; set; }

    public string Titulo { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public decimal Monto { get; set; }

    public DateOnly FechaVencimiento { get; set; }

    public string Estado { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual Usuario Usuario { get; set; } = null!;
    public bool EsProgramado { get; set; }
    public string? Frecuencia { get; set; }
    public DateOnly? FechaInicio { get; set; }
    public DateOnly? FechaFin { get; set; }
    public DateOnly? ProximoVencimiento { get; set; }
    public bool Activo { get; set; }
}
