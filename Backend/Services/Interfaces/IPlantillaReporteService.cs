// Backend/Services/Interfaces/IPlantillaReporteService.cs
using Backend.DTO.Reportes;

namespace Backend.Services.Interfaces
{
    public interface IPlantillaReporteService
    {
        /// <summary>
        /// Obtiene la lista de plantillas de reportes disponibles
        /// </summary>
        /// <returns>Lista de plantillas con su configuración por defecto</returns>
        PlantillasDisponiblesResponseDTO GetPlantillasDisponibles();

        /// <summary>
        /// Genera un reporte personalizado basado en una plantilla y configuración específica
        /// </summary>
        /// <param name="request">Configuración de la plantilla y personalización</param>
        /// <returns>Datos del reporte personalizado</returns>
        Task<ReporteFinancieroDataDTO> GenerarReportePersonalizadoAsync(PlantillaReporteRequestDTO request);

        /// <summary>
        /// Genera el PDF de un reporte personalizado
        /// </summary>
        /// <param name="data">Datos del reporte</param>
        /// <param name="configuracion">Configuración de la plantilla</param>
        /// <returns>Bytes del PDF generado</returns>
        byte[] GenerarPdfPersonalizado(ReporteFinancieroDataDTO data, PlantillaReporteRequestDTO configuracion);
    }
}