using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.DTO.Reportes
{
    public class ReporteFinancieroRequestDTO
    {
        [Required(ErrorMessage = "La fecha de inicio es requerida")]
        [DataType(DataType.DateTime)]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es requerida")]
        [DataType(DataType.DateTime)]
        public DateTime FechaFin { get; set; }

        [StringLength(100, ErrorMessage = "El título no puede exceder 100 caracteres")]
        public string TituloReporte { get; set; } = "Reporte Financiero Personal";

        public string PeriodoSeleccionado { get; set; } = "Personalizado"; // Ej: "Semanal", "Mensual", "Personalizado"


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (FechaFin < FechaInicio)
            {
                yield return new ValidationResult(
                    "La fecha de fin debe ser mayor o igual a la fecha de inicio",
                    new[] { nameof(FechaFin) });
            }

            if (FechaInicio > DateTime.Now)
            {
                yield return new ValidationResult(
                    "La fecha de inicio no puede ser futura",
                    new[] { nameof(FechaInicio) });
            }

            // No permitir rangos muy grandes (más de 1 año)
            if ((FechaFin - FechaInicio).TotalDays > 365)
            {
                yield return new ValidationResult(
                    "El rango de fechas no puede ser mayor a 1 año",
                    new[] { nameof(FechaFin) });
            }
        }










    }
}