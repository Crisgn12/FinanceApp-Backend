using System;
using System.Collections.Generic;

namespace Entidades.Entities;

public partial class Presupuesto
{
    public int PresupuestoId { get; set; }

    public int UsuarioId { get; set; }

    public int CategoriaId { get; set; }

    public decimal Limite { get; set; }

    public string Periodo { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual Categoria Categoria { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
