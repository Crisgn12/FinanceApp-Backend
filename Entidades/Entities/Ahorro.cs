using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entidades.Entities;

public partial class Ahorro
{
    [Column("AhorroID")]
    public int AhorroId { get; set; }

    [Column("UsuarioID")]
    public string UsuarioId { get; set; }

    public string? Nombre { get; set; } = null!;

    public decimal MontoObjetivo { get; set; }

    public decimal MontoActual { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool Completado { get; set; }

    public DateTime? FechaMeta { get; set; }

    public DateTime? UltimaNotificacion { get; set; }
    public virtual ICollection<AporteMetaAhorro> Aportes { get; set; } = new List<AporteMetaAhorro>();
}