using System;
using System.Collections.Generic;

namespace Entidades.Entities;

public partial class Informe
{
    public int InformeId { get; set; }

    public int UsuarioId { get; set; }

    public string Tipo { get; set; } = null!;

    public DateTime FechaGeneracion { get; set; }

    public string? UrlPdf { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Usuario Usuario { get; set; } = null!;
}
