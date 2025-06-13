// Backend/DTO/Reportes/PlantillaReporteDTO.cs
using System.ComponentModel.DataAnnotations;

namespace Backend.DTO.Reportes
{
    public class PlantillaReporteRequestDTO : IValidatableObject
    {
        [Required]
        public DateTime FechaInicio { get; set; }

        [Required]
        public DateTime FechaFin { get; set; }

        [Required]
        [StringLength(50)]
        public string TipoPlantilla { get; set; } = "completo";

        [StringLength(100)]
        public string TituloReporte { get; set; } = "";

        // Configuración de secciones
        public ConfiguracionSeccionesDTO Secciones { get; set; } = new();

        // Configuración de detalle
        public ConfiguracionDetalleDTO Detalle { get; set; } = new();

        // Filtros opcionales
        public ConfiguracionFiltrosDTO Filtros { get; set; } = new();

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

            if ((FechaFin - FechaInicio).TotalDays > 365)
            {
                yield return new ValidationResult(
                    "El rango de fechas no puede ser mayor a 1 año",
                    new[] { nameof(FechaFin) });
            }

            var tiposValidos = new[] { "completo", "ingresos", "gastos", "metas", "ejecutivo" };
            if (!tiposValidos.Contains(TipoPlantilla.ToLower()))
            {
                yield return new ValidationResult(
                    $"Tipo de plantilla inválido. Valores permitidos: {string.Join(", ", tiposValidos)}",
                    new[] { nameof(TipoPlantilla) });
            }
        }
    }

    public class ConfiguracionSeccionesDTO
    {
        public bool IncluirResumenFinanciero { get; set; } = true;
        public bool IncluirGastosPorCategoria { get; set; } = true;
        public bool IncluirAbonosAMetas { get; set; } = true;
        public bool IncluirTransaccionesDetalladas { get; set; } = true;
        public bool IncluirIngresos { get; set; } = true;
        public bool IncluirGraficos { get; set; } = false; // Para futuro
    }

    public class ConfiguracionDetalleDTO
    {
        public string NivelDetalle { get; set; } = "completo"; // "resumen", "completo", "detallado"
        public int MaximoTransacciones { get; set; } = 0; // 0 = sin límite
        public bool MostrarDescripciones { get; set; } = true;
        public bool MostrarPorcentajes { get; set; } = true;
        public string OrdenTransacciones { get; set; } = "fecha"; // "fecha", "monto", "categoria"
    }

    public class ConfiguracionFiltrosDTO
    {
        public List<string> CategoriasIncluidas { get; set; } = new();
        public List<string> CategoriasExcluidas { get; set; } = new();
        public decimal? MontoMinimo { get; set; }
        public decimal? MontoMaximo { get; set; }
        public List<string> TiposTransaccion { get; set; } = new(); // "Ingreso", "Gasto"
    }

    // DTO para respuesta de plantillas disponibles
    public class PlantillaDisponibleDTO
    {
        public string Id { get; set; } = "";
        public string Nombre { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public string Icono { get; set; } = "";
        public List<string> SeccionesIncluidas { get; set; } = new();
        public ConfiguracionSeccionesDTO ConfiguracionPorDefecto { get; set; } = new();
    }

    public class PlantillasDisponiblesResponseDTO
    {
        public List<PlantillaDisponibleDTO> Plantillas { get; set; } = new();
        public DateTime FechaConsulta { get; set; } = DateTime.Now;
    }
}