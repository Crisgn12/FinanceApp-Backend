using DAL.Interfaces;
using Entidades.Entities;
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

        public async Task<IEnumerable<Categoria>> ObtenerCategoriasPorUsuario(int UsuarioID)
        {
            return await _context.Categorias.FromSqlRaw("EXEC SP_OBTENER_CATEGORIAS_POR_USUARIO @UsuarioID = {0}", UsuarioID).ToListAsync();
        }
    }
}
