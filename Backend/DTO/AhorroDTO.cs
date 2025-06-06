namespace Backend.DTO
{
    public class AhorroDTO
    {
        public int? AhorroID { get; set; }     
        public string? UsuarioID { get; set; }
        public string? Nombre { get; set; }
        public decimal? Monto_Objetivo { get; set; }
        public decimal? Monto_Actual { get; set; } = 0;
        public bool? Completado { get; set; } = false;
        public DateTime? Fecha_Meta { get; set; }
        public decimal? PorcentajeAvance { get; set; }
        public List<AporteMetaAhorroDTO>? HistorialAportes { get; set; }
    }
}