// Backend/Services/Implementaciones/ReporteService.cs
using Backend.DTO.Reportes;
using Backend.Services.Interfaces;
using Entidades.Entities;
using DAL;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

// Para QuestPDF
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Services.Implementaciones
{
    public class ReporteService : IReporteService
    {
        private readonly FinanceAppContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ReporteService(FinanceAppContext context, UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ReporteFinancieroDataDTO> GetReporteFinancieroDataAsync(ReporteFinancieroRequestDTO request)
        {
            // Obtener el usuario autenticado
            var userId = GetCurrentUserId();

            // Validaciones de entrada
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("El ID del usuario es obligatorio.");
            }

            if (request == null)
            {
                throw new ArgumentException("Los datos de la solicitud son obligatorios.");
            }

            if (request.FechaInicio > request.FechaFin)
            {
                throw new ArgumentException("La fecha de inicio no puede ser mayor que la fecha de fin.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException("Usuario no encontrado.");
            }

            // Asegurar que las fechas incluyan todo el día
            DateOnly fechaInicioOnly = DateOnly.FromDateTime(request.FechaInicio);
            DateOnly fechaFinOnly = DateOnly.FromDateTime(request.FechaFin);

            try
            {
                var transacciones = await _context.Transacciones
                    .Include(t => t.Categoria)
                    .Where(t => t.UsuarioId == userId && t.Fecha >= fechaInicioOnly && t.Fecha <= fechaFinOnly)
                    .ToListAsync();

                var aportesMetas = await _context.AporteMetaAhorros
                    .Include(a => a.Ahorro)
                    .Where(a => a.Ahorro != null && a.Ahorro.UsuarioId == userId && a.Fecha >= request.FechaInicio && a.Fecha <= request.FechaFin)
                    .ToListAsync();

                var metas = await _context.Ahorros
                    .Where(a => a.UsuarioId == userId)
                    .ToListAsync();

                var reporteData = new ReporteFinancieroDataDTO
                {
                    NombreCompletoUsuario = user.UserName ?? user.Email ?? "Usuario",
                    FechaInicio = request.FechaInicio,
                    FechaFin = request.FechaFin,
                    FechaGeneracion = DateTime.Now,
                    TituloReporte = request.TituloReporte ?? "Reporte Financiero Personal"
                };

                // 1. Resumen Financiero Clave
                reporteData.IngresosTotales = transacciones
                    .Where(t => t.Tipo.Equals("Ingreso", StringComparison.OrdinalIgnoreCase))
                    .Sum(t => t.Monto);

                reporteData.GastosTotales = transacciones
                    .Where(t => t.Tipo.Equals("Gasto", StringComparison.OrdinalIgnoreCase))
                    .Sum(t => t.Monto);

                reporteData.BalanceNeto = reporteData.IngresosTotales - reporteData.GastosTotales;
                reporteData.AhorroTotalAbonado = aportesMetas.Sum(a => a.Monto);

                // 2. Desglose de Gastos por Categoría
                var gastosPorCategoria = transacciones
                    .Where(t => t.Tipo.Equals("Gasto", StringComparison.OrdinalIgnoreCase))
                    .GroupBy(t => new { t.CategoriaId, t.Categoria.Nombre })
                    .Select(g => new GastoPorCategoriaDataDTO
                    {
                        NombreCategoria = g.Key.Nombre ?? "Sin Categoría",
                        MontoGastado = g.Sum(t => t.Monto)
                    })
                    .OrderByDescending(g => g.MontoGastado)
                    .ToList();

                foreach (var gasto in gastosPorCategoria)
                {
                    gasto.Porcentaje = reporteData.GastosTotales > 0 ? (gasto.MontoGastado / reporteData.GastosTotales) * 100 : 0;
                    reporteData.GastosPorCategoria.Add(gasto);
                }

                // 3. Abonos a Metas
                var abonosAgrupadosPorMeta = aportesMetas
                    .GroupBy(a => a.MetaAhorroId)
                    .Select(g => new
                    {
                        MetaAhorroId = g.Key,
                        MontoAbonadoPeriodo = g.Sum(a => a.Monto)
                    })
                    .ToList();

                foreach (var abonoAgrupado in abonosAgrupadosPorMeta)
                {
                    var meta = metas.FirstOrDefault(m => m.AhorroId == abonoAgrupado.MetaAhorroId);
                    if (meta != null)
                    {
                        reporteData.AbonosAMetas.Add(new AbonoMetaAhorroDataDTO
                        {
                            NombreMeta = meta.Nombre ?? "Sin nombre",
                            MontoAbonadoPeriodo = abonoAgrupado.MontoAbonadoPeriodo,
                            MontoActualMeta = meta.MontoActual,
                            MontoObjetivoMeta = meta.MontoObjetivo,
                            ProgresoPorcentaje = meta.MontoObjetivo > 0 ? (meta.MontoActual / meta.MontoObjetivo) * 100 : 0
                        });
                    }
                }

                // 4. Transacciones Detalladas
                reporteData.Transacciones = transacciones
                    .OrderBy(t => t.Fecha)
                    .ThenBy(t => t.Titulo)
                    .Select(t => new TransaccionDataDTO
                    {
                        Fecha = t.Fecha,
                        Titulo = t.Titulo ?? "Sin título",
                        Descripcion = t.Descripcion ?? string.Empty,
                        CategoriaNombre = t.Categoria?.Nombre ?? "Sin Categoría",
                        Tipo = t.Tipo,
                        Monto = t.Monto
                    })
                    .ToList();

                return reporteData;
            }
            catch (Exception ex) when (ex.Message.Contains("no existe"))
            {
                throw new ArgumentException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al generar el reporte financiero: {ex.Message}");
            }
        }
        public byte[] GenerarPdfReporteFinanciero(ReporteFinancieroDataDTO data)
        {
            if (data == null)
            {
                throw new ArgumentException("Los datos del reporte son obligatorios.");
            }

            try
            {
                QuestPDF.Settings.License = LicenseType.Community;

                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(10));

                        // HEADER CORREGIDO - Usar Column en lugar de múltiples Text()
                        page.Header().Column(headerColumn =>
                        {
                            headerColumn.Item().Text(data.TituloReporte ?? "Reporte Financiero Personal")
                                .FontSize(18)
                                .SemiBold()
                                .AlignCenter();

                            headerColumn.Item().PaddingBottom(1, Unit.Centimetre);
                        });

                        page.Content()
                            .PaddingVertical(1, Unit.Centimetre)
                            .Column(column =>
                            {
                                column.Spacing(5);

                                // Información general del reporte
                                column.Item().Text(text =>
                                {
                                    text.Span("Usuario: ").SemiBold();
                                    text.Span(data.NombreCompletoUsuario);
                                });
                                column.Item().Text(text =>
                                {
                                    text.Span("Periodo: ").SemiBold();
                                    text.Span($"{data.FechaInicio.ToShortDateString()} - {data.FechaFin.ToShortDateString()}");
                                });
                                column.Item().Text(text =>
                                {
                                    text.Span("Fecha de Generación: ").SemiBold();
                                    text.Span(data.FechaGeneracion.ToString("dd/MM/yyyy HH:mm"));
                                });

                                column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten1);

                                // Resumen Financiero Clave
                                column.Item().Text("Resumen Financiero")
                                    .FontSize(14)
                                    .SemiBold()
                                    .Underline();

                                column.Item().PaddingLeft(20).Column(subColumn =>
                                {
                                    subColumn.Spacing(3);
                                    subColumn.Item().Text($"Ingresos Totales: {data.IngresosTotales:C2}");
                                    subColumn.Item().Text($"Gastos Totales: {data.GastosTotales:C2}");
                                    subColumn.Item().Text($"Balance Neto: {data.BalanceNeto:C2}");
                                    subColumn.Item().Text($"Ahorro Total Abonado en el Periodo: {data.AhorroTotalAbonado:C2}");
                                });

                                column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten1);

                                // Desglose de Gastos por Categoría
                                column.Item().Text("Gastos por Categoría")
                                    .FontSize(14)
                                    .SemiBold()
                                    .Underline();

                                if (data.GastosPorCategoria.Any())
                                {
                                    column.Item().Table(table =>
                                    {
                                        table.ColumnsDefinition(columns =>
                                        {
                                            columns.RelativeColumn(3);
                                            columns.RelativeColumn(2);
                                            columns.RelativeColumn(1);
                                        });

                                        table.Header(header =>
                                        {
                                            header.Cell().Element(CellStyle).Text("Categoría").SemiBold();
                                            header.Cell().Element(CellStyle).AlignRight().Text("Monto Gastado").SemiBold();
                                            header.Cell().Element(CellStyle).AlignRight().Text("%").SemiBold();
                                        });

                                        foreach (var categoria in data.GastosPorCategoria)
                                        {
                                            table.Cell().Element(CellStyleContent).Text(categoria.NombreCategoria);
                                            table.Cell().Element(CellStyleContent).AlignRight().Text($"{categoria.MontoGastado:C2}");
                                            table.Cell().Element(CellStyleContent).AlignRight().Text($"{categoria.Porcentaje:F2}%");
                                        }
                                    });
                                }
                                else
                                {
                                    column.Item().Text("No hay gastos registrados para este periodo.").Italic();
                                }

                                column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten1);

                                // Abonos a Metas
                                column.Item().Text("Abonos a Metas de Ahorro")
                                    .FontSize(14)
                                    .SemiBold()
                                    .Underline();

                                if (data.AbonosAMetas.Any())
                                {
                                    column.Item().Table(table =>
                                    {
                                        table.ColumnsDefinition(columns =>
                                        {
                                            columns.RelativeColumn(3);
                                            columns.RelativeColumn(2);
                                            columns.RelativeColumn(2);
                                            columns.RelativeColumn(1);
                                        });

                                        table.Header(header =>
                                        {
                                            header.Cell().Element(CellStyle).Text("Meta").SemiBold();
                                            header.Cell().Element(CellStyle).AlignRight().Text("Abonado en Periodo").SemiBold();
                                            header.Cell().Element(CellStyle).AlignRight().Text("Total Meta (Actual/Objetivo)").SemiBold();
                                            header.Cell().Element(CellStyle).AlignRight().Text("Progreso").SemiBold();
                                        });

                                        foreach (var abono in data.AbonosAMetas)
                                        {
                                            table.Cell().Element(CellStyleContent).Text(abono.NombreMeta);
                                            table.Cell().Element(CellStyleContent).AlignRight().Text($"{abono.MontoAbonadoPeriodo:C2}");
                                            table.Cell().Element(CellStyleContent).AlignRight().Text($"{abono.MontoActualMeta:C2} / {abono.MontoObjetivoMeta:C2}");
                                            table.Cell().Element(CellStyleContent).AlignRight().Text($"{abono.ProgresoPorcentaje:F2}%");
                                        }
                                    });
                                }
                                else
                                {
                                    column.Item().Text("No hay abonos a metas de ahorro registrados para este periodo.").Italic();
                                }

                                column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten1);

                                // Transacciones Detalladas
                                column.Item().Text("Transacciones Detalladas")
                                    .FontSize(14)
                                    .SemiBold()
                                    .Underline();

                                if (data.Transacciones.Any())
                                {
                                    column.Item().Table(table =>
                                    {
                                        table.ColumnsDefinition(columns =>
                                        {
                                            columns.RelativeColumn(1.5f); // Fecha
                                            columns.RelativeColumn(3);    // Titulo
                                            columns.RelativeColumn(2);    // Categoría
                                            columns.RelativeColumn(1);    // Tipo
                                            columns.RelativeColumn(1.5f); // Monto
                                        });

                                        table.Header(header =>
                                        {
                                            header.Cell().Element(CellStyle).Text("Fecha").SemiBold();
                                            header.Cell().Element(CellStyle).Text("Título").SemiBold();
                                            header.Cell().Element(CellStyle).Text("Categoría").SemiBold();
                                            header.Cell().Element(CellStyle).Text("Tipo").SemiBold();
                                            header.Cell().Element(CellStyle).AlignRight().Text("Monto").SemiBold();
                                        });

                                        foreach (var transaccion in data.Transacciones)
                                        {
                                            table.Cell().Element(CellStyleContent).Text(transaccion.Fecha.ToShortDateString());
                                            table.Cell().Element(CellStyleContent).Text(transaccion.Titulo);
                                            table.Cell().Element(CellStyleContent).Text(transaccion.CategoriaNombre);
                                            table.Cell().Element(CellStyleContent).Text(transaccion.Tipo);
                                            table.Cell().Element(CellStyleContent).AlignRight().Text($"{transaccion.Monto:C2}");
                                        }
                                    });
                                }
                                else
                                {
                                    column.Item().Text("No hay transacciones registradas para este periodo.").Italic();
                                }
                            });

                        page.Footer()
                            .AlignRight()
                            .Text(x =>
                            {
                                x.Span("Página ").FontSize(9);
                                x.CurrentPageNumber().FontSize(9);
                                x.Span(" de ").FontSize(9);
                                x.TotalPages().FontSize(9);
                            });
                    });
                });

                return document.GeneratePdf();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al generar el PDF del reporte: {ex.Message}");
            }
        }
        #region Auxiliares
        private string GetCurrentUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null || !user.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("Usuario no autenticado");
            }

            // Intenta obtener el ID de múltiples claims estándar
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                      ?? user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                      ?? user.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                // Log para depuración: muestra todos los claims disponibles
                var claims = user.Claims.Select(c => $"{c.Type}: {c.Value}");
                throw new UnauthorizedAccessException("No se pudo obtener el ID del usuario del token");
            }

            return userId;
        }
        #endregion

        // Métodos auxiliares para el estilo de las celdas
        private static IContainer CellStyle(IContainer container)
        {
            return container
                .BorderBottom(1)
                .BorderColor(Colors.Black)
                .Padding(5)
                .Background(Colors.Grey.Lighten3);
        }

        private static IContainer CellStyleContent(IContainer container)
        {
            return container
                .BorderBottom(0.5f)
                .BorderColor(Colors.Grey.Lighten3)
                .Padding(5);
        }
    }
}