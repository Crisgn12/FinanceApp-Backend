using DAL.Interfaces;
using Entidades.DTOs;
using Entidades.Entities;
using Humanizer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Implementaciones
{
    public class PagoProgramadoDAL : IPagoProgramadoDAL
    {
        private readonly FinanceAppContext _context;

        public PagoProgramadoDAL(FinanceAppContext context)
        {
            _context = context;
        }
        /* --------------- Crear --------------- */
        public async Task<bool> CrearPagoProgramadoAsync(CrearPagoProgramadoDTO dto)
        {
            var p = new[]
            {
                new SqlParameter("@UsuarioID",  dto.UsuarioId),
                new SqlParameter("@Titulo",     dto.Titulo),
                new SqlParameter("@Descripcion",(object?)dto.Descripcion ?? DBNull.Value),
                new SqlParameter("@Monto",      dto.Monto),
                new SqlParameter("@FechaInicio",dto.FechaInicio.ToDateTime(TimeOnly.MinValue)),
                new SqlParameter("@Frecuencia", dto.Frecuencia),
                new SqlParameter("@FechaFin",   (object?)dto.FechaFin?.ToDateTime(TimeOnly.MinValue) ?? DBNull.Value)
            };

            int filas = await _context.Database.ExecuteSqlRawAsync(
                "EXEC dbo.SP_CREAR_PAGO_PROGRAMADO @UsuarioID,@Titulo,@Descripcion,@Monto,@FechaInicio,@Frecuencia,@FechaFin", p);

            // (-1 cuando SET NOCOUNT ON) o (>0 filas afectadas) → éxito
            return filas == -1 || filas > 0;
        }
        /* --------------- Actualizar --------------- */
        public async Task<bool> ActualizarPagoProgramadoAsync(PagoProgramadoDTO dto)
        {
            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@PagoID", dto.PagoId),
                new SqlParameter("@UsuarioID", dto.UsuarioId)
            };

            var campos = new List<string>();

            // Solo agregar parámetros que tienen valor
            if (!string.IsNullOrEmpty(dto.Titulo))
            {
                parametros.Add(new SqlParameter("@Titulo", dto.Titulo));
                campos.Add("@Titulo = @Titulo");
            }

            if (!string.IsNullOrEmpty(dto.Descripcion))
            {
                parametros.Add(new SqlParameter("@Descripcion", dto.Descripcion));
                campos.Add("@Descripcion = @Descripcion");
            }

            if (dto.Monto > 0)
            {
                parametros.Add(new SqlParameter("@Monto", dto.Monto));
                campos.Add("@Monto = @Monto");
            }

            if (!string.IsNullOrEmpty(dto.Frecuencia))
            {
                parametros.Add(new SqlParameter("@Frecuencia", dto.Frecuencia));
                campos.Add("@Frecuencia = @Frecuencia");
            }

            if (dto.FechaInicio.HasValue)
            {
                parametros.Add(new SqlParameter("@FechaInicio", dto.FechaInicio.Value));
                campos.Add("@FechaInicio = @FechaInicio");
            }

            if (dto.FechaFin.HasValue)
            {
                parametros.Add(new SqlParameter("@FechaFin", dto.FechaFin.Value));
                campos.Add("@FechaFin = @FechaFin");
            }

            if (dto.Activo.HasValue)
            {
                parametros.Add(new SqlParameter("@Activo", dto.Activo.Value));
                campos.Add("@Activo = @Activo");
            }

            if (campos.Count == 0)
            {
                return false; // No hay nada que actualizar
            }

            var sql = $@"
                EXEC dbo.SP_ACTUALIZAR_PAGO_PROGRAMADO 
                    @PagoID = @PagoID,
                    @UsuarioID = @UsuarioID,
                    {string.Join(",\n                    ", campos)};
            ";

            int filas = await _context.Database.ExecuteSqlRawAsync(sql, parametros.ToArray());
            return filas > 0;
        }
        /* --------------- Cambiar Estado (NUEVO) --------------- */
        public async Task<bool> CambiarEstadoPagoProgramadoAsync(CambioEstadoDTO dto)
        {
            var p = new[]
            {
        new SqlParameter("@PagoID", dto.PagoId),
        new SqlParameter("@UsuarioID", dto.UsuarioId),
        new SqlParameter("@Estado", dto.Estado ?? (object)DBNull.Value)
    };

            var sql = @"
        EXEC dbo.SP_ACTUALIZAR_PAGO_PROGRAMADO 
            @PagoID = @PagoID,
            @UsuarioID = @UsuarioID,
            @Estado = @Estado;
    ";

            var result = await _context.FilasAfectadasResults
                .FromSqlRaw(sql, p)
                .ToListAsync();

            return result.FirstOrDefault()?.FilasAfectadas > 0;
        }

        /* --------------- Eliminar --------------- */
        public async Task<bool> EliminarPagoProgramadoAsync(PagoProgramadoDTO dto)
        {
            var p = new[]
            {
                new SqlParameter("@PagoID", dto.PagoId),
                new SqlParameter("@UsuarioID", dto.UsuarioId)
            };

            int filas = await _context.Database.ExecuteSqlRawAsync(
                "EXEC dbo.SP_ELIMINAR_PAGO_PROGRAMADO @PagoID,@UsuarioID", p);

            return filas == -1 || filas > 0;
        }
        /* --------------- Listar (filtro general) --------------- */
        public async Task<List<PagoProgramadoDTO>> ObtenerPagosProgramadosAsync(FiltroPagosProgramadosDTO f)
        {
            var p = new[]
            {
        new SqlParameter("@UsuarioID", f.UsuarioId),
        new SqlParameter("@FechaInicio", f.FechaInicio.HasValue ? f.FechaInicio.Value.ToDateTime(TimeOnly.MinValue) : (object)DBNull.Value),
        new SqlParameter("@FechaFin", f.FechaFin.HasValue ? f.FechaFin.Value.ToDateTime(TimeOnly.MinValue) : (object)DBNull.Value),
        new SqlParameter("@SoloActivos", f.SoloActivos.HasValue ? f.SoloActivos.Value : (object)DBNull.Value)
    };
            var rows = await _context.Pagos
                .FromSqlRaw("EXEC dbo.SP_OBTENER_PAGOS_PROGRAMADOS @UsuarioID, @FechaInicio, @FechaFin, @SoloActivos", p)
                .AsNoTracking()
                .ToListAsync();
            return rows.Select(pago => new PagoProgramadoDTO
            {
                PagoId = pago.PagoId,
                UsuarioId = pago.UsuarioId,
                Titulo = pago.Titulo,
                Descripcion = pago.Descripcion,
                Monto = pago.Monto,
                FechaVencimiento = pago.FechaVencimiento,
                Estado = pago.Estado,
                EsProgramado = pago.EsProgramado,
                Frecuencia = pago.Frecuencia,
                FechaInicio = pago.FechaInicio,
                FechaFin = pago.FechaFin,
                ProximoVencimiento = pago.ProximoVencimiento,
                Activo = pago.Activo
            }).ToList();
        }
        /* --------------- Listar próximos --------------- */
        public async Task<List<PagoProgramadoDTO>> ObtenerPagosProximosAsync(FiltroPagosProximosDTO f)
        {
            var p = new[]
            {
                new SqlParameter("@UsuarioID", f.UsuarioId),
                new SqlParameter("@Dias",      f.DiasAnticipacion)
            };

            // Materializar primero con ToListAsync() y luego proyectar
            var rows = await _context.Pagos
                .FromSqlRaw("EXEC dbo.SP_OBTENER_PAGOS_PROXIMOS @UsuarioID,@Dias", p)
                .AsNoTracking()
                .ToListAsync();

            return rows.Select(pago => new PagoProgramadoDTO
            {
                PagoId = pago.PagoId,
                UsuarioId = pago.UsuarioId,
                Titulo = pago.Titulo,
                Descripcion = pago.Descripcion,
                Monto = pago.Monto,
                ProximoVencimiento = pago.ProximoVencimiento!.Value,
                Frecuencia = pago.Frecuencia!,
                Activo = pago.Activo
            })
            .ToList();
        }
    }
}