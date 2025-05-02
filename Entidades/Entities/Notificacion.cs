using System;
using System.Collections.Generic;

namespace Entidades.Entities;

public partial class Notificacion
{
    public int NotificacionId { get; set; }

    public int UsuarioId { get; set; }

    public string Mensaje { get; set; } = null!;

    public bool Leida { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Usuario Usuario { get; set; } = null!;
}
