namespace Backend.DTO
{
    public class AhorroDTO
    {
        public int? AhorroID { get; set; }     
        public int UsuarioID { get; set; }
        public string Nombre { get; set; }
        public decimal Monto_Objetivo { get; set; }
        public decimal Monto_Actual { get; set; } = 0;
        public bool Completado { get; set; } = false;
        public DateTime? Fecha_Meta { get; set; }

        //public List<AporteMetaAhorroDTO>? HistorialAportes { get; set; }
        //public decimal PorcentajeAvance => Monto_Objetivo > 0 ? (Monto_Actual / Monto_Objetivo) * 100 : 0;

        public decimal PorcentajeAvance { get; set; }
        public List<AporteMetaAhorroDTO> HistorialAportes { get; set; }
    }
}
