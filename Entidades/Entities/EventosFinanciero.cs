using Entidades.Entities;
using System;
using System.Collections.Generic;

namespace Entidades.Entities;

public partial class EventosFinanciero
{
    public int IdEvento { get; set; }

    public string? Titulo { get; set; }

    public string? Descripcion { get; set; }

    public DateTime FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    public bool? EsTodoElDia { get; set; }

    public string? Tipo { get; set; }

    public decimal? Monto { get; set; }

    public string? ColorFondo { get; set; }

    public string? Frecuencia { get; set; }

    public int? Repeticiones { get; set; }

    public bool? Activo { get; set; }

    public string? Estado { get; set; }

    public DateTime? ProximaEjecucion { get; set; }

    public DateTime? UltimaEjecucion { get; set; }

    public string? UsuarioId { get; set; }

    public int? RecurrenciaId { get; set; }

    public virtual Recurrencia? Recurrencia { get; set; }

    /*public virtual AspNetUser? Usuario { get; set; }*/
}
