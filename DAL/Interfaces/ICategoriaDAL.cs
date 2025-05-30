using Entidades.DTOs;
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
        Task<bool> CrearCategoriaPersonalizada(CategoriaDTO categoria);
        Task<bool> ActualizarCategoria(CategoriaDTO categoria);
        Task<bool> EliminarCategoria(BorrarCategoriaDTO req);
    }
}
