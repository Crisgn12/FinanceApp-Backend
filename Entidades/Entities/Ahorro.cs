using System;
using System.Collections.Generic;

namespace Entidades.Entities;

public partial class Ahorro
{
    public int AhorroId { get; set; }

    public int UsuarioId { get; set; }

    public string Nombre { get; set; } = null!;

    public decimal MontoObjetivo { get; set; }

    public decimal MontoActual { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool Completado { get; set; }

    public virtual Usuario Usuario { get; set; } = null!;
}
