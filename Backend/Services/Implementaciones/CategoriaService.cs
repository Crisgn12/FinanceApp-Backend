using Backend.Services.Interfaces;
using DAL.Interfaces;
using Entidades.DTOs;
using Entidades.Entities;

namespace Backend.Services.Implementaciones
{
    public class CategoriaService: ICategoriaService
    {
        IUnidadDeTrabajo _unidadDeTrabajo;

        public CategoriaService(IUnidadDeTrabajo unidadDeTrabajo)
        {
            _unidadDeTrabajo = unidadDeTrabajo;
        }

        private CategoriaDTO Convertir(Categoria categoria)
        {
            return new CategoriaDTO
            {
                CategoriaID = categoria.CategoriaId,
                UsuarioID = categoria.UsuarioId,
                Nombre = categoria.Nombre,
                Tipo = categoria.Tipo,
                EsPredeterminada = categoria.EsPredeterminada
            };
        }

        public async Task<IEnumerable<CategoriaDTO>> ObtenerCategoriasPorUsuarioID(int usuarioID)
        {
            var categorias = await _unidadDeTrabajo.CategoriaDAL.ObtenerCategoriasPorUsuario(usuarioID);
            var listaCategorias = new List<CategoriaDTO>();
            foreach (var categoria in categorias)
            {
                listaCategorias.Add(Convertir(categoria));
            }
            return listaCategorias;
        }
    }
}
