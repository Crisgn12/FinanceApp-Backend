using Entidades.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface ICategoriaDAL: IDALGenerico<Categoria>
    {
        Task<IEnumerable<Categoria>> ObtenerCategoriasPorUsuario(int UsuarioID);
    }
}
