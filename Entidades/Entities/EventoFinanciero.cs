using System.ComponentModel.DataAnnotations.Schema;

namespace Entidades.Entities
{
    public partial class EventoFinanciero
    {
        [Column("Id_Evento")]
        public int IdEvento { get; set; }

        [Column("UsuarioID")]
        public string UsuarioId { get; set; }

        public string Titulo { get; set; }

        public string? Descripcion { get; set; }

        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }

        public bool EsTodoElDia { get; set; }

        public string Tipo { get; set; } // "Ingreso", "Gasto", "Recordatorio"

        public decimal? Monto { get; set; }

        public string? ColorFondo { get; set; }

        public string? Frecuencia { get; set; } // "Diaria", "Semanal", "Mensual", "Anual"

        public int? Repeticiones { get; set; }

        public bool Activo { get; set; }

        public int? RecurrenciaID { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}