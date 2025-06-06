using System;
using System.Collections.Generic;

namespace Entidades.Entities;

public partial class Usuario
{
    public int UsuarioId { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string LlaveHash { get; set; } = null!;

    public DateTime? FechaCreacion { get; set; }

    //public virtual ICollection<Ahorro> Ahorros { get; set; } = new List<Ahorro>();

    //public virtual ICollection<Categoria> Categoria { get; set; } = new List<Categoria>();

    public virtual ICollection<Informe> Informes { get; set; } = new List<Informe>();

    public virtual ICollection<Notificacion> Notificaciones { get; set; } = new List<Notificacion>();

    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();

    public virtual ICollection<Presupuesto> Presupuestos { get; set; } = new List<Presupuesto>();

    //public virtual ICollection<Transaccion> Transacciones { get; set; } = new List<Transaccion>();
}
