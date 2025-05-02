using System;
using System.Collections.Generic;

namespace Entidades.Entities;

public partial class Transaccion
{
    public int TransaccionId { get; set; }

    public int UsuarioId { get; set; }

    public int CategoriaId { get; set; }

    public string? Descripcion { get; set; }

    public decimal Monto { get; set; }

    public DateOnly Fecha { get; set; }

    public string Tipo { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual Categoria Categoria { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
