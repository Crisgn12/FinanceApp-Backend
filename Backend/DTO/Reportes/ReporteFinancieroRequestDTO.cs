using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.DTO.Reportes
{
    public class ReporteFinancieroRequestDTO
    {
        [Required]
        public DateTime FechaInicio { get; set; }
        [Required]
        public DateTime FechaFin { get; set; }
        [Required]
        public string TituloReporte { get; set; } = "Reporte Financiero Personal";
        public string PeriodoSeleccionado { get; set; } = "Personalizado"; // Ej: "Semanal", "Mensual", "Personalizado"
    }
}