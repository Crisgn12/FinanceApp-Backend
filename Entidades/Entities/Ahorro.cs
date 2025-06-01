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

    public DateTime? FechaMeta { get; set; }

    public DateTime? UltimaNotificacion { get; set; }

    public virtual Usuario Usuario { get; set; } = null!;
    public virtual ICollection<AporteMetaAhorro> Aportes { get; set; } = new List<AporteMetaAhorro>();

    //public decimal PorcentajeProgreso => MontoObjetivo == 0 ? 0 : (MontoActual / MontoObjetivo) * 100;
}