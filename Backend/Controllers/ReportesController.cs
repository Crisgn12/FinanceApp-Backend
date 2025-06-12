// Backend/Controllers/ReportesController.cs
using Backend.DTO.Reportes; // Ajusta el namespace si es diferente (Backend.DTO.Reportes)
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization; // Para [Authorize]
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Mime; // Para MediaTypeNames
using System.Security.Claims; // Para obtener el User.FindFirstValue(ClaimTypes.NameIdentifier)
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Asegúrate de que solo los usuarios autenticados puedan acceder a estos reportes
    public class ReportesController : ControllerBase
    {
        private readonly IReporteService _reporteService;

        public ReportesController(IReporteService reporteService)
        {
            _reporteService = reporteService;
        }

        /// <summary>
        /// Genera un reporte financiero en formato PDF para el usuario autenticado y un rango de fechas.
        /// </summary>
        /// <param name="request">DTO con la fecha de inicio, fecha de fin y título del reporte.</param>
        /// <returns>El archivo PDF del reporte.</returns>
        [HttpPost("generar-pdf")]
        [Produces("application/pdf")] // Indica que este endpoint produce un PDF
        public async Task<IActionResult> GenerarReporteFinancieroPdf([FromBody] ReporteFinancieroRequestDTO request)
        {
            // Obtener el ID del usuario autenticado
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("No se pudo identificar al usuario.");
            }

            try
            {
                // 1. Obtener los datos del reporte
                var reporteData = await _reporteService.GetReporteFinancieroDataAsync(userId, request);

                // 2. Generar el PDF
                var pdfBytes = _reporteService.GenerarPdfReporteFinanciero(reporteData);

                // 3. Devolver el PDF
                // El nombre del archivo se construye con el título del reporte y las fechas.
                var fileName = $"{reporteData.TituloReporte.Replace(" ", "_")}_{reporteData.FechaInicio.ToString("yyyyMMdd")}-{reporteData.FechaFin.ToString("yyyyMMdd")}.pdf";

                return File(pdfBytes, MediaTypeNames.Application.Pdf, fileName);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Loguea el error (implementa tu propio logger si es necesario)
                Console.WriteLine($"Error al generar reporte PDF: {ex.Message}");
                return StatusCode(500, "Ocurrió un error interno al generar el reporte.");
            }
        }

        // Si planeas guardar los informes en la base de datos (como tu entidad Informe sugiere)
        // podrías tener otro endpoint para listar los informes guardados.
        // Por ahora, solo nos enfocamos en la generación y descarga directa.
    }
}