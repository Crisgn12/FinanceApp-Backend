using DAL.Interfaces;
using Entidades.DTOs;
using Entidades.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Implementaciones
{
    public class TransaccionDAL : DALGenericoImpl<Transaccion>, ITransaccionDAL
    {
        FinanceAppContext _context;

        public TransaccionDAL(FinanceAppContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> IngresarTransaccion(TransaccionDTO transaccionDTO)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@UsuarioID", transaccionDTO.UsuarioId),
                    new SqlParameter("@CategoriaID", transaccionDTO.CategoriaId),
                    new SqlParameter("@Descripcion", (object)transaccionDTO.Descripcion ?? DBNull.Value),
                    new SqlParameter("@Monto", transaccionDTO.Monto),
                    new SqlParameter("@Fecha", transaccionDTO.Fecha.ToDateTime(TimeOnly.MinValue)),
                    new SqlParameter("@Tipo", transaccionDTO.Tipo),
                    new SqlParameter("@Titulo", (object)transaccionDTO.Titulo ?? DBNull.Value)
                };

                var result = await _context.Transacciones
                    .FromSqlRaw("EXEC SP_CREAR_TRANSACCION @UsuarioID, @CategoriaID, @Descripcion, @Monto, @Fecha, @Tipo, @Titulo", parameters)
                    .ToListAsync();

                return result.Count > 0;
            }
            catch (SqlException ex) when (ex.Number == 50000)
            {
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al ingresar la transacción: {ex.Message}");
            }
        }

        public async Task<List<TransaccionDTO>> ObtenerTransaccionesPorUsuario(ObtenerTransaccionesPorUsuarioDTO req)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@UsuarioID", req.UsuarioId),
                    new SqlParameter("@FechaInicio", req.FechaInicio.HasValue ? req.FechaInicio.Value.ToDateTime(TimeOnly.MinValue) : (object)DBNull.Value),
                    new SqlParameter("@FechaFin", req.FechaFin.HasValue ? req.FechaFin.Value.ToDateTime(TimeOnly.MinValue) : (object)DBNull.Value),
                    new SqlParameter("@NombreCategoria", (object)req.NombreCategoria ?? DBNull.Value)
                };

                var transacciones = await _context.Database
                    .SqlQueryRaw<TransaccionDTO>(
                        "EXEC SP_OBTENER_TRANSACCIONES_POR_USUARIO @UsuarioID, @FechaInicio, @FechaFin, @NombreCategoria",
                        parameters)
                    .ToListAsync();

                return transacciones;
            }
            catch (SqlException ex) when (ex.Number == 50000)
            {
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener las transacciones: {ex.Message}");
            }
        }

        public async Task<TransaccionDTO> ObtenerDetalleTransaccion(ObtenerDetalleTransaccionDTO req)
        {
            try
            {
                var parameters = new[]
                {
                new SqlParameter("@TransaccionID", req.TransaccionId),
                new SqlParameter("@UsuarioID", req.UsuarioId)
            };

                var transacciones = await _context.Database
                    .SqlQueryRaw<TransaccionDTO>(
                        "EXEC SP_OBTENER_DETALLE_TRANSACCION @TransaccionID, @UsuarioID",
                        parameters)
                    .ToListAsync();

                return transacciones.FirstOrDefault();
            }
            catch (SqlException ex) when (ex.Number == 50000)
            {
                throw new Exception(ex.Message); // Maneja errores como transacción o usuario no existente
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el detalle de la transacción: {ex.Message}");
            }
        }

        public async Task<bool> ActualizarTransaccion(TransaccionDTO transaccionDTO)
        {
            try
            {
                if (!transaccionDTO.TransaccionId.HasValue)
                {
                    throw new ArgumentException("El ID de la transacción es obligatorio.");
                }
                if (string.IsNullOrEmpty(transaccionDTO.UsuarioId))
                {
                    throw new ArgumentException("El ID del usuario es obligatorio.");
                }

                var parameters = new[]
                {
                    new SqlParameter("@TransaccionID", transaccionDTO.TransaccionId.Value),
                    new SqlParameter("@UsuarioID", transaccionDTO.UsuarioId),
                    new SqlParameter("@CategoriaID", transaccionDTO.CategoriaId),
                    new SqlParameter("@Titulo", (object)transaccionDTO.Titulo ?? DBNull.Value),
                    new SqlParameter("@Descripcion", (object)transaccionDTO.Descripcion ?? DBNull.Value),
                    new SqlParameter("@Monto", transaccionDTO.Monto),
                    new SqlParameter("@Fecha", transaccionDTO.Fecha.ToDateTime(TimeOnly.MinValue)),
                    new SqlParameter("@Tipo", transaccionDTO.Tipo)
                };

                var filasAfectadas = await _context.Database
                    .ExecuteSqlRawAsync(
                        "EXEC SP_ACTUALIZAR_TRANSACCION @TransaccionID, @UsuarioID, @CategoriaID, @Titulo, @Descripcion, @Monto, @Fecha, @Tipo",
                        parameters);

                return filasAfectadas > 0;
            }
            catch (SqlException ex) when (ex.Number == 50000)
            {
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar la transacción: {ex.Message}");
            }
        }

        public async Task<bool> EliminarTransaccion(EliminarTransaccionDTO req)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@TransaccionID", req.TransaccionID),
                    new SqlParameter("@UsuarioID", req.UsuarioID)
                };

                var filasAfectadas = await _context.Database
                    .ExecuteSqlRawAsync(
                        "EXEC SP_ELIMINAR_TRANSACCION @TransaccionID, @UsuarioID",
                        parameters);

                return filasAfectadas > 0;
            }
            catch (SqlException ex) when (ex.Number == 50000)
            {
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al eliminar la transacción: {ex.Message}");
            }
        }

        public Task<List<TotalxDiaDTO>> TotalGastosUltimos6diasPorUsuario(string usuarioId)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@UsuarioID", usuarioId)
                };
                var totalGastos = _context.Database
                    .SqlQueryRaw<TotalxDiaDTO>(
                        "EXEC SP_OBTENER_GASTOS_ULTIMOS_6_DIAS_POR_USUARIO @UsuarioID",
                        parameters);
                return totalGastos.ToListAsync();
            }
            catch (SqlException ex) when (ex.Number == 50000)
            {
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el total de gastos: {ex.Message}");
            }
        }

        public Task<List<GraficoCategoriasDTO>> TotalGastosPorCategoria(string usuarioId)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@UsuarioID", usuarioId)
                };
                var totalGastosPorCategoria = _context.Database
                    .SqlQueryRaw<GraficoCategoriasDTO>(
                        "EXEC SP_OBTENER_GASTOS_POR_CATEGORIA @UsuarioID",
                        parameters);
                return totalGastosPorCategoria.ToListAsync();
            }
            catch (SqlException ex) when (ex.Number == 50000)
            {
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el total de gastos por categoría: {ex.Message}");
            }
        }
    }
}
