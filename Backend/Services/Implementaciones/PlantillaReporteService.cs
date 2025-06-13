// Backend/Services/Implementaciones/PlantillaReporteService.cs
using Backend.DTO.Reportes;
using Backend.Services.Interfaces;

namespace Backend.Services.Implementaciones
{
    public class PlantillaReporteService : IPlantillaReporteService
    {
        private readonly IReporteService _reporteService;

        public PlantillaReporteService(IReporteService reporteService)
        {
            _reporteService = reporteService;
        }

        public PlantillasDisponiblesResponseDTO GetPlantillasDisponibles()
        {
            var plantillas = new List<PlantillaDisponibleDTO>
            {
                new PlantillaDisponibleDTO
                {
                    Id = "completo",
                    Nombre = "Reporte Financiero Completo",
                    Descripcion = "Análisis integral con todas las secciones: ingresos, gastos, metas y transacciones detalladas",
                    Icono = "📊",
                    SeccionesIncluidas = new List<string> { "Resumen", "Gastos por Categoría", "Metas de Ahorro", "Transacciones" },
                    ConfiguracionPorDefecto = new ConfiguracionSeccionesDTO
                    {
                        IncluirResumenFinanciero = true,
                        IncluirGastosPorCategoria = true,
                        IncluirAbonosAMetas = true,
                        IncluirTransaccionesDetalladas = true,
                        IncluirIngresos = true
                    }
                },
                new PlantillaDisponibleDTO
                {
                    Id = "ingresos",
                    Nombre = "Reporte de Ingresos",
                    Descripcion = "Enfoque en ingresos y análisis de fuentes de ingreso",
                    Icono = "💰",
                    SeccionesIncluidas = new List<string> { "Resumen de Ingresos", "Detalle de Ingresos" },
                    ConfiguracionPorDefecto = new ConfiguracionSeccionesDTO
                    {
                        IncluirResumenFinanciero = true,
                        IncluirGastosPorCategoria = false,
                        IncluirAbonosAMetas = false,
                        IncluirTransaccionesDetalladas = true,
                        IncluirIngresos = true
                    }
                },
                new PlantillaDisponibleDTO
                {
                    Id = "gastos",
                    Nombre = "Análisis de Gastos",
                    Descripcion = "Desglose detallado de gastos por categoría y patrones de consumo",
                    Icono = "💸",
                    SeccionesIncluidas = new List<string> { "Gastos por Categoría", "Detalle de Gastos" },
                    ConfiguracionPorDefecto = new ConfiguracionSeccionesDTO
                    {
                        IncluirResumenFinanciero = true,
                        IncluirGastosPorCategoria = true,
                        IncluirAbonosAMetas = false,
                        IncluirTransaccionesDetalladas = true,
                        IncluirIngresos = false
                    }
                },
                new PlantillaDisponibleDTO
                {
                    Id = "metas",
                    Nombre = "Progreso de Metas",
                    Descripcion = "Seguimiento de metas de ahorro y progreso financiero",
                    Icono = "🎯",
                    SeccionesIncluidas = new List<string> { "Metas de Ahorro", "Abonos Realizados" },
                    ConfiguracionPorDefecto = new ConfiguracionSeccionesDTO
                    {
                        IncluirResumenFinanciero = true,
                        IncluirGastosPorCategoria = false,
                        IncluirAbonosAMetas = true,
                        IncluirTransaccionesDetalladas = false,
                        IncluirIngresos = false
                    }
                },
                new PlantillaDisponibleDTO
                {
                    Id = "ejecutivo",
                    Nombre = "Resumen Ejecutivo",
                    Descripcion = "Vista de alto nivel con métricas clave y KPIs principales",
                    Icono = "📈",
                    SeccionesIncluidas = new List<string> { "Métricas Clave", "Indicadores Principales" },
                    ConfiguracionPorDefecto = new ConfiguracionSeccionesDTO
                    {
                        IncluirResumenFinanciero = true,
                        IncluirGastosPorCategoria = true,
                        IncluirAbonosAMetas = true,
                        IncluirTransaccionesDetalladas = false,
                        IncluirIngresos = true
                    }
                }
            };

            return new PlantillasDisponiblesResponseDTO
            {
                Plantillas = plantillas,
                FechaConsulta = DateTime.Now
            };
        }

        public async Task<ReporteFinancieroDataDTO> GenerarReportePersonalizadoAsync(PlantillaReporteRequestDTO request)
        {
            // Convertir la solicitud de plantilla a solicitud de reporte base
            var baseRequest = new ReporteFinancieroRequestDTO
            {
                FechaInicio = request.FechaInicio,
                FechaFin = request.FechaFin,
                TituloReporte = !string.IsNullOrWhiteSpace(request.TituloReporte)
                    ? request.TituloReporte
                    : GenerarTituloAutomatico(request.TipoPlantilla)
            };

            // Obtener datos completos del reporte
            var datosCompletos = await _reporteService.GetReporteFinancieroDataAsync(baseRequest);

            // Aplicar personalización según la plantilla y configuración
            var datosPersonalizados = AplicarPersonalizacion(datosCompletos, request);

            return datosPersonalizados;
        }

        public byte[] GenerarPdfPersonalizado(ReporteFinancieroDataDTO data, PlantillaReporteRequestDTO configuracion)
        {
            // Por ahora, usar el generador PDF existente
            // En el futuro se puede crear lógica específica de plantillas para el PDF
            return _reporteService.GenerarPdfReporteFinanciero(data);
        }

        private ReporteFinancieroDataDTO AplicarPersonalizacion(
            ReporteFinancieroDataDTO datosCompletos,
            PlantillaReporteRequestDTO request)
        {
            var datosPersonalizados = new ReporteFinancieroDataDTO
            {
                NombreCompletoUsuario = datosCompletos.NombreCompletoUsuario,
                FechaInicio = datosCompletos.FechaInicio,
                FechaFin = datosCompletos.FechaFin,
                FechaGeneracion = datosCompletos.FechaGeneracion,
                TituloReporte = datosCompletos.TituloReporte
            };

            // Aplicar filtros de configuración según secciones
            if (request.Secciones.IncluirResumenFinanciero)
            {
                datosPersonalizados.IngresosTotales = datosCompletos.IngresosTotales;
                datosPersonalizados.GastosTotales = datosCompletos.GastosTotales;
                datosPersonalizados.BalanceNeto = datosCompletos.BalanceNeto;
                datosPersonalizados.AhorroTotalAbonado = datosCompletos.AhorroTotalAbonado;
            }

            // Gastos por categoría
            if (request.Secciones.IncluirGastosPorCategoria)
            {
                datosPersonalizados.GastosPorCategoria = AplicarFiltrosCategorias(
                    datosCompletos.GastosPorCategoria, request.Filtros);
            }

            // Abonos a metas
            if (request.Secciones.IncluirAbonosAMetas)
            {
                datosPersonalizados.AbonosAMetas = datosCompletos.AbonosAMetas;
            }

            // Transacciones detalladas
            if (request.Secciones.IncluirTransaccionesDetalladas)
            {
                datosPersonalizados.Transacciones = AplicarFiltrosTransacciones(
                    datosCompletos.Transacciones, request);
            }

            return datosPersonalizados;
        }

        private List<GastoPorCategoriaDataDTO> AplicarFiltrosCategorias(
            List<GastoPorCategoriaDataDTO> categorias,
            ConfiguracionFiltrosDTO filtros)
        {
            var resultado = categorias.AsQueryable();

            // Filtrar por categorías incluidas
            if (filtros.CategoriasIncluidas.Any())
            {
                resultado = resultado.Where(c =>
                    filtros.CategoriasIncluidas.Contains(c.NombreCategoria, StringComparer.OrdinalIgnoreCase));
            }

            // Excluir categorías específicas
            if (filtros.CategoriasExcluidas.Any())
            {
                resultado = resultado.Where(c =>
                    !filtros.CategoriasExcluidas.Contains(c.NombreCategoria, StringComparer.OrdinalIgnoreCase));
            }

            // Filtrar por rango de montos
            if (filtros.MontoMinimo.HasValue)
            {
                resultado = resultado.Where(c => c.MontoGastado >= filtros.MontoMinimo.Value);
            }

            if (filtros.MontoMaximo.HasValue)
            {
                resultado = resultado.Where(c => c.MontoGastado <= filtros.MontoMaximo.Value);
            }

            return resultado.ToList();
        }

        private List<TransaccionDataDTO> AplicarFiltrosTransacciones(
            List<TransaccionDataDTO> transacciones,
            PlantillaReporteRequestDTO request)
        {
            var resultado = transacciones.AsQueryable();

            // Filtrar por tipos de transacción
            if (request.Filtros.TiposTransaccion.Any())
            {
                resultado = resultado.Where(t =>
                    request.Filtros.TiposTransaccion.Contains(t.Tipo, StringComparer.OrdinalIgnoreCase));
            }

            // Aplicar filtros según el tipo de plantilla
            switch (request.TipoPlantilla.ToLower())
            {
                case "ingresos":
                    resultado = resultado.Where(t => t.Tipo.Equals("Ingreso", StringComparison.OrdinalIgnoreCase));
                    break;
                case "gastos":
                    resultado = resultado.Where(t => t.Tipo.Equals("Gasto", StringComparison.OrdinalIgnoreCase));
                    break;
            }

            // Filtrar por categorías
            if (request.Filtros.CategoriasIncluidas.Any())
            {
                resultado = resultado.Where(t =>
                    request.Filtros.CategoriasIncluidas.Contains(t.CategoriaNombre, StringComparer.OrdinalIgnoreCase));
            }

            if (request.Filtros.CategoriasExcluidas.Any())
            {
                resultado = resultado.Where(t =>
                    !request.Filtros.CategoriasExcluidas.Contains(t.CategoriaNombre, StringComparer.OrdinalIgnoreCase));
            }

            // Filtrar por rango de montos
            if (request.Filtros.MontoMinimo.HasValue)
            {
                resultado = resultado.Where(t => t.Monto >= request.Filtros.MontoMinimo.Value);
            }

            if (request.Filtros.MontoMaximo.HasValue)
            {
                resultado = resultado.Where(t => t.Monto <= request.Filtros.MontoMaximo.Value);
            }

            // Aplicar ordenamiento
            resultado = request.Detalle.OrdenTransacciones.ToLower() switch
            {
                "monto" => resultado.OrderByDescending(t => t.Monto),
                "categoria" => resultado.OrderBy(t => t.CategoriaNombre).ThenBy(t => t.Fecha),
                _ => resultado.OrderBy(t => t.Fecha)
            };

            // Aplicar límite de transacciones
            if (request.Detalle.MaximoTransacciones > 0)
            {
                resultado = resultado.Take(request.Detalle.MaximoTransacciones);
            }

            return resultado.ToList();
        }

        private string GenerarTituloAutomatico(string tipoPlantilla)
        {
            return tipoPlantilla.ToLower() switch
            {
                "completo" => "Reporte Financiero Completo",
                "ingresos" => "Reporte de Ingresos",
                "gastos" => "Análisis de Gastos",
                "metas" => "Progreso de Metas de Ahorro",
                "ejecutivo" => "Resumen Ejecutivo Financiero",
                _ => "Reporte Financiero Personalizado"
            };
        }
    }
}