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
    public class CategoriaDAL: DALGenericoImpl<Categoria>, ICategoriaDAL
    {
        FinanceAppContext _context;

        public CategoriaDAL(FinanceAppContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Categoria>> ObtenerCategoriasPorUsuario(string UsuarioID)
        {
            return await _context.Categorias.FromSqlRaw("EXEC SP_OBTENER_CATEGORIAS_POR_USUARIO @UsuarioID = {0}", UsuarioID).ToListAsync();
        }

        public async Task<bool> CrearCategoriaPersonalizada(CategoriaDTO categoria)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@UsuarioID", categoria.UsuarioID ?? (object)DBNull.Value),
                    new SqlParameter("@Nombre", categoria.Nombre),
                    new SqlParameter("@Tipo", categoria.Tipo),
                    new SqlParameter("@EsPredeterminada", categoria.EsPredeterminada)
                };

                var resultado = await _context.Categorias
                    .FromSqlRaw("EXEC SP_CREAR_CATEGORIA_PERSONALIZADA @UsuarioID, @Nombre, @Tipo, @EsPredeterminada", parameters)
                    .ToListAsync();

                return resultado.Count > 0;
            }
            catch (SqlException ex)
            {
                return false;
            }
        }

        public async Task<bool> ActualizarCategoria(CategoriaDTO categoria)
        {
            try
            {
                if (!categoria.CategoriaID.HasValue)
                    throw new ArgumentException("El ID de la categoría es obligatorio.");

                var parameters = new[]
                {
                    new SqlParameter("@CategoriaID", categoria.CategoriaID),
                    new SqlParameter("@UsuarioID", categoria.UsuarioID ?? (object)DBNull.Value),
                    new SqlParameter("@Nombre", categoria.Nombre),
                    new SqlParameter("@Tipo", categoria.Tipo),
                    new SqlParameter("@EsPredeterminada", categoria.EsPredeterminada)
                };

                var result = await _context.Categorias
                    .FromSqlRaw("EXEC SP_ACTUALIZAR_CATEGORIA @CategoriaID, @UsuarioID, @Nombre, @Tipo, @EsPredeterminada", parameters)
                    .ToListAsync();

                return result.Count > 0;
            }
            catch (SqlException ex) when (ex.Number == 50000)
            {
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar la categoría: {ex.Message}");
            }
        }

        public async Task<bool> EliminarCategoria(BorrarCategoriaDTO req)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@CategoriaID", req.CategoriaID),
                    new SqlParameter("@UsuarioID", req.UsuarioID)
                };

                var result = await _context.Database
                    .ExecuteSqlRawAsync("EXEC SP_ELIMINAR_CATEGORIA @CategoriaID, @UsuarioID", parameters);

                return true;
            }
            catch (SqlException ex) when (ex.Number == 50000)
            {
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al eliminar la categoría: {ex.Message}");
            }
        }
    }
}
