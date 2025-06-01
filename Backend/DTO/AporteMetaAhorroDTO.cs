namespace Backend.DTO
{
    public class AporteMetaAhorroDTO
    {
        public int? AporteId { get; set; }
        public int MetaAhorroId { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Monto { get; set; }
        public string? Observaciones { get; set; }
    }
}