// Backend/Controllers/ReportesController.cs - Versión mejorada y simplificada
using Backend.DTO.Reportes;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReportesController : ControllerBase
    {
        private readonly IReporteService _reporteService;
        private readonly ILogger<ReportesController> _logger;

        public ReportesController(IReporteService reporteService, ILogger<ReportesController> logger)
        {
            _reporteService = reporteService;
            _logger = logger;
        }

        /// <summary>
        /// Genera un reporte financiero en formato PDF para el usuario autenticado.
        /// </summary>
        /// <param name="request">DTO con la fecha de inicio, fecha de fin y título del reporte.</param>
        /// <returns>El archivo PDF del reporte.</returns>
        [HttpPost("generar-pdf")]
        [ProducesResponseType(typeof(FileResult), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GenerarReporteFinancieroPdf([FromBody] ReporteFinancieroRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("Iniciando generación de reporte PDF");

                // El servicio ahora maneja internamente la obtención del userId
                var reporteData = await _reporteService.GetReporteFinancieroDataAsync(request);
                var pdfBytes = _reporteService.GenerarPdfReporteFinanciero(reporteData);

                // Crear nombre del archivo
                var fileName = GenerarNombreArchivo(reporteData);

                _logger.LogInformation("Reporte PDF generado exitosamente");

                return File(pdfBytes, MediaTypeNames.Application.Pdf, fileName);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Usuario no autenticado intentó generar reporte");
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Error de validación al generar reporte");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno al generar reporte");
                return StatusCode(500, "Ocurrió un error interno al generar el reporte.");
            }
        }

        /// <summary>
        /// Obtiene los datos del reporte sin generar PDF (útil para preview o datos JSON)
        /// </summary>
        [HttpPost("datos")]
        [ProducesResponseType(typeof(ReporteFinancieroDataDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ObtenerDatosReporte([FromBody] ReporteFinancieroRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var reporteData = await _reporteService.GetReporteFinancieroDataAsync(request);
                return Ok(reporteData);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Usuario no autenticado intentó obtener datos de reporte");
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Error de validación al obtener datos del reporte");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener datos del reporte");
                return StatusCode(500, "Ocurrió un error interno al obtener los datos del reporte.");
            }
        }

        /// <summary>
        /// Obtiene un resumen rápido de las métricas principales
        /// </summary>
        [HttpGet("resumen")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ObtenerResumenRapido([FromQuery] DateTime? fechaInicio = null, [FromQuery] DateTime? fechaFin = null)
        {
            try
            {
                // Si no se proporcionan fechas, usar el mes actual
                var inicio = fechaInicio ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var fin = fechaFin ?? DateTime.Now;

                var request = new ReporteFinancieroRequestDTO
                {
                    FechaInicio = inicio,
                    FechaFin = fin,
                    TituloReporte = "Resumen Rápido"
                };

                var datos = await _reporteService.GetReporteFinancieroDataAsync(request);
                var resumen = GenerarResumenMetricas(datos, inicio, fin);

                return Ok(resumen);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Usuario no autenticado intentó obtener resumen");
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener resumen rápido");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene estadísticas de gastos por categoría para el periodo especificado
        /// </summary>
        [HttpGet("estadisticas-categorias")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ObtenerEstadisticasCategorias([FromQuery] DateTime? fechaInicio = null, [FromQuery] DateTime? fechaFin = null)
        {
            try
            {
                var inicio = fechaInicio ?? DateTime.Now.AddMonths(-1);
                var fin = fechaFin ?? DateTime.Now;

                var request = new ReporteFinancieroRequestDTO
                {
                    FechaInicio = inicio,
                    FechaFin = fin,
                    TituloReporte = "Estadísticas por Categoría"
                };

                var datos = await _reporteService.GetReporteFinancieroDataAsync(request);

                var estadisticas = new
                {
                    periodo = $"{inicio:dd/MM/yyyy} - {fin:dd/MM/yyyy}",
                    totalGastos = datos.GastosTotales,
                    categorias = datos.GastosPorCategoria.Select(c => new
                    {
                        categoria = c.NombreCategoria,
                        monto = c.MontoGastado,
                        porcentaje = c.Porcentaje,
                        esTop3 = datos.GastosPorCategoria.Take(3).Contains(c)
                    }).ToList(),
                    categoriaTop = datos.GastosPorCategoria.FirstOrDefault()?.NombreCategoria ?? "N/A",
                    montoTop = datos.GastosPorCategoria.FirstOrDefault()?.MontoGastado ?? 0
                };

                return Ok(estadisticas);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Usuario no autenticado intentó obtener estadísticas");
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas de categorías");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        #region Métodos Auxiliares

        private static string GenerarNombreArchivo(ReporteFinancieroDataDTO reporteData)
        {
            var fechaInicio = reporteData.FechaInicio.ToString("yyyyMMdd");
            var fechaFin = reporteData.FechaFin.ToString("yyyyMMdd");
            var tituloLimpio = LimpiarNombreArchivo(reporteData.TituloReporte);

            return $"{tituloLimpio}_{fechaInicio}-{fechaFin}.pdf";
        }

        private static string LimpiarNombreArchivo(string titulo)
        {
            if (string.IsNullOrWhiteSpace(titulo))
                return "Reporte_Financiero";

            return titulo
                .Replace(" ", "_")
                .Replace("/", "-")
                .Replace("\\", "-")
                .Replace(":", "-")
                .Replace("*", "")
                .Replace("?", "")
                .Replace("\"", "")
                .Replace("<", "")
                .Replace(">", "")
                .Replace("|", "-");
        }

        private static object GenerarResumenMetricas(ReporteFinancieroDataDTO datos, DateTime inicio, DateTime fin)
        {
            var diasPeriodo = (decimal)(fin - inicio).TotalDays;
            if (diasPeriodo <= 0) diasPeriodo = 1; // Evitar división por cero

            var promedioGastoDiario = datos.GastosTotales / diasPeriodo;
            var promedioIngresoDiario = datos.IngresosTotales / diasPeriodo;
            var porcentajeAhorro = datos.IngresosTotales > 0 ? (datos.AhorroTotalAbonado / datos.IngresosTotales) * 100 : 0;
            var tasaGasto = datos.IngresosTotales > 0 ? (datos.GastosTotales / datos.IngresosTotales) * 100 : 0;

            return new
            {
                periodo = new
                {
                    inicio = inicio.ToString("dd/MM/yyyy"),
                    fin = fin.ToString("dd/MM/yyyy"),
                    dias = (int)diasPeriodo
                },
                totales = new
                {
                    ingresos = datos.IngresosTotales,
                    gastos = datos.GastosTotales,
                    balanceNeto = datos.BalanceNeto,
                    ahorroTotal = datos.AhorroTotalAbonado
                },
                promedios = new
                {
                    gastoDiario = Math.Round(promedioGastoDiario, 2),
                    ingresoDiario = Math.Round(promedioIngresoDiario, 2)
                },
                indicadores = new
                {
                    porcentajeAhorro = Math.Round(porcentajeAhorro, 2),
                    tasaGasto = Math.Round(tasaGasto, 2),
                    saludFinanciera = ClasificarSaludFinanciera(datos.BalanceNeto, porcentajeAhorro)
                },
                contadores = new
                {
                    transacciones = datos.Transacciones.Count,
                    categorias = datos.GastosPorCategoria.Count,
                    metas = datos.AbonosAMetas.Count,
                    ingresos = datos.Transacciones.Count(t => t.Tipo.Equals("Ingreso", StringComparison.OrdinalIgnoreCase)),
                    gastos = datos.Transacciones.Count(t => t.Tipo.Equals("Gasto", StringComparison.OrdinalIgnoreCase))
                }
            };
        }

        private static string ClasificarSaludFinanciera(decimal balanceNeto, decimal porcentajeAhorro)
        {
            if (balanceNeto < 0)
                return "Crítica - Balance negativo";

            if (porcentajeAhorro >= 20)
                return "Excelente - Ahorro alto";

            if (porcentajeAhorro >= 10)
                return "Buena - Ahorro moderado";

            if (porcentajeAhorro >= 5)
                return "Regular - Ahorro bajo";

            return "Deficiente - Sin ahorro";
        }

        #endregion
    }
}