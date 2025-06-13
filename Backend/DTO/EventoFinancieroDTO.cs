namespace Backend.DTO
{
    public class EventosFinancieroDTO
    {
        public int? IdEvento { get; set; }
        public string? UsuarioID { get; set; }
        public string? Titulo { get; set; }
        public string? Descripcion { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public bool? EsTodoElDia { get; set; }
        public string? Tipo { get; set; } // "Ingreso", "Gasto", "Recordatorio"
        public decimal? Monto { get; set; }
        public string? ColorFondo { get; set; }
        public string? Frecuencia { get; set; } // "Diaria", "Semanal", "Mensual", "Anual"
        public int? Repeticiones { get; set; }
        public bool? Activo { get; set; } = true;
        public int? RecurrenciaId { get; set; }
    }

    public class EventosFinancieroRangoDTO
    {
        public string? UsuarioID { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }

    public class ActualizarEventosPorRecurrenciaDTO
    {
        public int RecurrenciaId { get; set; }
        public string? ColorFondo { get; set; }
        public decimal? Monto { get; set; }
        public bool? Activo { get; set; }
    }
    public class EliminarEventoDTO
    {
        public int IdEvento { get; set; }
    }

    public class EliminarEventosPorRecurrenciaDTO
    {
        public int RecurrenciaId { get; set; }
    }

    public class ListarEventosPorRangoDTO
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }

    public class ObtenerEventoPorIdDTO
    {
        public int IdEvento { get; set; }
    }
}