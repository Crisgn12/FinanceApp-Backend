namespace Entidades.Entities
{
    public partial class AporteMetaAhorro
    {
        public int AporteId { get; set; }

        public int AhorroId { get; set; }

        public DateTime Fecha { get; set; }

        public decimal Monto { get; set; }

        public string? Observaciones { get; set; }

        public virtual Ahorro Ahorro { get; set; } = null!;
    }
}