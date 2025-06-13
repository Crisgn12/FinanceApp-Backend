// Backend/Controllers/ReportesController.cs - Versión extendida con plantillas

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
        private readonly IPlantillaReporteService _plantillaReporteService;
        private readonly ILogger<ReportesController> _logger;

        public ReportesController(
            IReporteService reporteService,
            IPlantillaReporteService plantillaReporteService,
            ILogger<ReportesController> logger)
        {
            _reporteService = reporteService;
            _plantillaReporteService = plantillaReporteService;
            _logger = logger;
        }

        #region Endpoints Originales (Mantenidos para compatibilidad)

        /// <summary>
        /// Genera un reporte financiero en formato PDF para el usuario autenticado.
        /// </summary>
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

                var reporteData = await _reporteService.GetReporteFinancieroDataAsync(request);
                var pdfBytes = _reporteService.GenerarPdfReporteFinanciero(reporteData);

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
        /// Obtiene los datos del reporte sin generar PDF
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

        [HttpGet("resumen")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ObtenerResumenRapido([FromQuery] DateTime? fechaInicio = null, [FromQuery] DateTime? fechaFin = null)
        {
            try
            {
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

        #endregion

        #region Nuevos Endpoints - Sistema de Plantillas

        /// <summary>
        /// Obtiene las plantillas de reportes disponibles
        /// </summary>
        [HttpGet("plantillas")]
        [ProducesResponseType(typeof(PlantillasDisponiblesResponseDTO), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public IActionResult ObtenerPlantillasDisponibles()
        {
            try
            {
                var plantillas = _plantillaReporteService.GetPlantillasDisponibles();
                return Ok(plantillas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener plantillas disponibles");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Genera un reporte personalizado basado en plantilla
        /// </summary>
        [HttpPost("personalizado/datos")]
        [ProducesResponseType(typeof(ReporteFinancieroDataDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GenerarReportePersonalizado([FromBody] PlantillaReporteRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("Generando reporte personalizado con plantilla: {TipoPlantilla}", request.TipoPlantilla);

                var reporteData = await _plantillaReporteService.GenerarReportePersonalizadoAsync(request);

                _logger.LogInformation("Reporte personalizado generado exitosamente");

                return Ok(reporteData);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Usuario no autenticado intentó generar reporte personalizado");
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Error de validación al generar reporte personalizado");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno al generar reporte personalizado");
                return StatusCode(500, "Ocurrió un error interno al generar el reporte personalizado.");
            }
        }

        /// <summary>
        /// Genera un reporte personalizado en formato PDF
        /// </summary>
        [HttpPost("personalizado/pdf")]
        [ProducesResponseType(typeof(FileResult), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GenerarReportePersonalizadoPdf([FromBody] PlantillaReporteRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("Generando reporte personalizado PDF con plantilla: {TipoPlantilla}", request.TipoPlantilla);

                var reporteData = await _plantillaReporteService.GenerarReportePersonalizadoAsync(request);
                var pdfBytes = _plantillaReporteService.GenerarPdfPersonalizado(reporteData, request);

                var fileName = GenerarNombreArchivoPersonalizado(reporteData, request);

                _logger.LogInformation("Reporte personalizado PDF generado exitosamente");

                return File(pdfBytes, MediaTypeNames.Application.Pdf, fileName);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Usuario no autenticado intentó generar reporte personalizado PDF");
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Error de validación al generar reporte personalizado PDF");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno al generar reporte personalizado PDF");
                return StatusCode(500, "Ocurrió un error interno al generar el reporte personalizado.");
            }
        }

        /// <summary>
        /// Vista previa de reporte personalizado con configuración específica
        /// </summary>
        [HttpPost("personalizado/vista-previa")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> VistaReportePersonalizado([FromBody] PlantillaReporteRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("Generando vista previa de reporte con plantilla: {TipoPlantilla}", request.TipoPlantilla);

                var reporteData = await _plantillaReporteService.GenerarReportePersonalizadoAsync(request);

                // Crear una respuesta simplificada para vista previa
                var vistaPrevia = new
                {
                    configuracion = new
                    {
                        tipoPlantilla = request.TipoPlantilla,
                        periodo = $"{request.FechaInicio:dd/MM/yyyy} - {request.FechaFin:dd/MM/yyyy}",
                        seccionesIncluidas = ObtenerSeccionesIncluidas(request.Secciones),
                        filtrosAplicados = ObtenerFiltrosAplicados(request.Filtros)
                    },
                    resumen = new
                    {
                        ingresos = reporteData.IngresosTotales,
                        gastos = reporteData.GastosTotales,
                        balance = reporteData.BalanceNeto,
                        ahorros = reporteData.AhorroTotalAbonado,
                        totalTransacciones = reporteData.Transacciones?.Count ?? 0,
                        totalCategorias = reporteData.GastosPorCategoria?.Count ?? 0
                    },
                    estructura = new
                    {
                        titulo = reporteData.TituloReporte,
                        fechaGeneracion = reporteData.FechaGeneracion,
                        usuario = reporteData.NombreCompletoUsuario
                    }
                };

                return Ok(vistaPrevia);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Usuario no autenticado intentó obtener vista previa");
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Error de validación en vista previa");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno al generar vista previa");
                return StatusCode(500, "Ocurrió un error interno al generar la vista previa.");
            }
        }

        #endregion

        #region Métodos Auxiliares

        private string GenerarNombreArchivo(ReporteFinancieroDataDTO data)
        {
            var fechaArchivo = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var nombreUsuario = data.NombreCompletoUsuario.Replace(" ", "_");
            return $"Reporte_Financiero_{nombreUsuario}_{fechaArchivo}.pdf";
        }

        private string GenerarNombreArchivoPersonalizado(ReporteFinancieroDataDTO data, PlantillaReporteRequestDTO request)
        {
            var fechaArchivo = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var nombreUsuario = data.NombreCompletoUsuario.Replace(" ", "_");
            var tipoPlantilla = request.TipoPlantilla.ToUpper();
            return $"Reporte_{tipoPlantilla}_{nombreUsuario}_{fechaArchivo}.pdf";
        }

        private object GenerarResumenMetricas(ReporteFinancieroDataDTO datos, DateTime inicio, DateTime fin)
        {
            var diasPeriodo = (fin - inicio).Days + 1;
            var promedioGastosDiario = diasPeriodo > 0 ? datos.GastosTotales / diasPeriodo : 0;
            var promedioIngresosDiario = diasPeriodo > 0 ? datos.IngresosTotales / diasPeriodo : 0;

            return new
            {
                periodo = new
                {
                    inicio = inicio.ToString("dd/MM/yyyy"),
                    fin = fin.ToString("dd/MM/yyyy"),
                    dias = diasPeriodo
                },
                metricas = new
                {
                    ingresosTotales = datos.IngresosTotales,
                    gastosTotales = datos.GastosTotales,
                    balanceNeto = datos.BalanceNeto,
                    ahorroTotal = datos.AhorroTotalAbonado,
                    promedioGastosDiario,
                    promedioIngresosDiario
                },
                indicadores = new
                {
                    tasaAhorro = datos.IngresosTotales > 0
                        ? Math.Round((datos.AhorroTotalAbonado / datos.IngresosTotales) * 100, 2)
                        : 0,
                    balancePositivo = datos.BalanceNeto > 0,
                    categoriaTopGasto = datos.GastosPorCategoria?.FirstOrDefault()?.NombreCategoria ?? "N/A",
                    porcentajeTopGasto = datos.GastosPorCategoria?.FirstOrDefault()?.Porcentaje ?? 0
                },
                fechaGeneracion = DateTime.Now
            };
        }

        private List<string> ObtenerSeccionesIncluidas(ConfiguracionSeccionesDTO secciones)
        {
            var seccionesIncluidas = new List<string>();

            if (secciones.IncluirResumenFinanciero) seccionesIncluidas.Add("Resumen Financiero");
            if (secciones.IncluirGastosPorCategoria) seccionesIncluidas.Add("Gastos por Categoría");
            if (secciones.IncluirAbonosAMetas) seccionesIncluidas.Add("Abonos a Metas");
            if (secciones.IncluirTransaccionesDetalladas) seccionesIncluidas.Add("Transacciones Detalladas");
            if (secciones.IncluirIngresos) seccionesIncluidas.Add("Ingresos");
            if (secciones.IncluirGraficos) seccionesIncluidas.Add("Gráficos");

            return seccionesIncluidas;
        }

        private object ObtenerFiltrosAplicados(ConfiguracionFiltrosDTO filtros)
        {
            return new
            {
                categoriasIncluidas = filtros.CategoriasIncluidas?.Count ?? 0,
                categoriasExcluidas = filtros.CategoriasExcluidas?.Count ?? 0,
                rangoMontos = new
                {
                    minimo = filtros.MontoMinimo,
                    maximo = filtros.MontoMaximo,
                    aplicado = filtros.MontoMinimo.HasValue || filtros.MontoMaximo.HasValue
                },
                tiposTransaccion = filtros.TiposTransaccion?.Count ?? 0
            };
        }

        #endregion
    }
}