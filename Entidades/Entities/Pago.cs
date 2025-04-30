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
}
