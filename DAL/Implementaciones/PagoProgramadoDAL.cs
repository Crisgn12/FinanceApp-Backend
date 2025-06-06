using DAL.Interfaces;
using Entidades.DTOs;
using Entidades.Entities;
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
            var p = new[]
            {
                new SqlParameter("@PagoID",     dto.PagoId),
                new SqlParameter("@UsuarioID",  dto.UsuarioId),
                new SqlParameter("@Titulo",     dto.Titulo),
                new SqlParameter("@Descripcion",(object?)dto.Descripcion ?? DBNull.Value),
                new SqlParameter("@Monto",      dto.Monto),
                new SqlParameter("@Frecuencia", dto.Frecuencia),
                new SqlParameter("@FechaInicio", dto.FechaInicio.HasValue ? (object)dto.FechaInicio.Value : DBNull.Value),
                new SqlParameter("@FechaFin",    dto.FechaFin.HasValue    ? (object)dto.FechaFin.Value    : DBNull.Value),
                new SqlParameter("@Activo",      dto.Activo)
            };

            var sql = @"
                EXEC dbo.SP_ACTUALIZAR_PAGO_PROGRAMADO 
                    @PagoID       = @PagoID,
                    @UsuarioID    = @UsuarioID,
                    @Titulo       = @Titulo,
                    @Descripcion  = @Descripcion,
                    @Monto        = @Monto,
                    @Frecuencia   = @Frecuencia,
                    @FechaInicio  = @FechaInicio,
                    @FechaFin     = @FechaFin,
                    @Activo       = @Activo;
            ";

            int filas = await _context.Database.ExecuteSqlRawAsync(sql, p);
            return filas > 0;
        }

        /* --------------- Eliminar --------------- */
        public async Task<bool> EliminarPagoProgramadoAsync(int pagoId, string usuarioId)
        {
            var p = new[]
            {
                new SqlParameter("@PagoID",    pagoId),
                new SqlParameter("@UsuarioID", usuarioId)
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
                new SqlParameter("@UsuarioID",   f.UsuarioId),
                new SqlParameter("@FechaInicio", (object?)f.FechaInicio?.ToDateTime(TimeOnly.MinValue) ?? DBNull.Value),
                new SqlParameter("@FechaFin",    (object?)f.FechaFin?.ToDateTime(TimeOnly.MinValue) ?? DBNull.Value),
                new SqlParameter("@SoloActivos", (object?)f.SoloActivos ?? DBNull.Value)
            };

            // Materializar primero con ToListAsync() y luego proyectar
            var rows = await _context.Pagos
                .FromSqlRaw("EXEC dbo.SP_OBTENER_PAGOS_PROGRAMADOS @UsuarioID,@FechaInicio,@FechaFin,@SoloActivos", p)
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
