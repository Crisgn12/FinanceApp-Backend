using System;
using System.Collections.Generic;

namespace Entidades.Entities;

public partial class Categoria
{
    public int CategoriaId { get; set; }

    public string? UsuarioId { get; set; }

    public string Nombre { get; set; } = null!;

    public string Tipo { get; set; } = null!;

    public bool EsPredeterminada { get; set; }

    public virtual ICollection<Presupuesto> Presupuestos { get; set; } = new List<Presupuesto>();

    public virtual ICollection<Transaccion> Transacciones { get; set; } = new List<Transaccion>();

    //public virtual Usuario Usuario { get; set; } = null!;
}
