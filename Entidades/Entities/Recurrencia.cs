using System;
using System.Collections.Generic;

namespace Entidades.Entities;

public partial class Recurrencia
{
    public int RecurrenciaId { get; set; }

    public string Frecuencia { get; set; } = null!;

    public int Intervalo { get; set; }

    public string? DiasSemana { get; set; }

    public int? DiaMes { get; set; }

    public int? Repeticiones { get; set; }

    public DateTime? FechaFin { get; set; }

    public DateTime? CreadoEn { get; set; }

    public virtual ICollection<EventosFinanciero> EventosFinancieros { get; set; } = new List<EventosFinanciero>();
}
