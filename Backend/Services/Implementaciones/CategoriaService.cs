using Backend.Services.Interfaces;
using DAL.Implementaciones;
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

        public async Task<IEnumerable<CategoriaDTO>> ObtenerCategoriasPorUsuarioID(string usuarioID)
        {
            var categorias = await _unidadDeTrabajo.CategoriaDAL.ObtenerCategoriasPorUsuario(usuarioID);
            var listaCategorias = new List<CategoriaDTO>();
            foreach (var categoria in categorias)
            {
                listaCategorias.Add(Convertir(categoria));
            }
            return listaCategorias;
        }

        public async Task<bool> CrearCategoriaPersonalizada(CategoriaDTO categoriaDTO)
        {
            if (string.IsNullOrWhiteSpace(categoriaDTO.Nombre))
            {
                throw new ArgumentException("El nombre de la categoría es obligatorio.");
            }
            if (categoriaDTO.Tipo != "Ingreso" && categoriaDTO.Tipo != "Gasto")
            {
                throw new ArgumentException("El tipo debe ser 'Ingreso' o 'Gasto'.");
            }

            categoriaDTO.EsPredeterminada = false;
            categoriaDTO.CategoriaID = null;

            bool resultado = await _unidadDeTrabajo.CategoriaDAL.CrearCategoriaPersonalizada(categoriaDTO);

            _unidadDeTrabajo.GuardarCambios();

            return resultado;
        }

        public async Task<bool> ActualizarCategoriaAsync(CategoriaDTO categoriaDTO)
        {
            bool resultado;

            if (!categoriaDTO.CategoriaID.HasValue)
            {
                throw new ArgumentException("El ID de la categoría es obligatorio.");
            }
            if (string.IsNullOrWhiteSpace(categoriaDTO.Nombre))
            {
                throw new ArgumentException("El nombre de la categoría es obligatorio.");
            }
            if (categoriaDTO.Tipo != "Ingreso" && categoriaDTO.Tipo != "Gasto")
            {
                throw new ArgumentException("El tipo debe ser 'Ingreso' o 'Gasto'.");
            }

            try
            {
                resultado = await _unidadDeTrabajo.CategoriaDAL.ActualizarCategoria(categoriaDTO);
            }
            catch (Exception ex) when (ex.Message.Contains("Ya existe una categoría") || ex.Message.Contains("no existe") || ex.Message.Contains("no tiene permiso"))
            {
                throw new ArgumentException(ex.Message);
            }

            return resultado;
        }

        public async Task<bool> EliminarCategoriaAsync(BorrarCategoriaDTO req)
        {
            try
            {
                return await _unidadDeTrabajo.CategoriaDAL.EliminarCategoria(req);
            }
            catch (Exception ex) when (ex.Message.Contains("no existe") || ex.Message.Contains("no tiene permiso") || ex.Message.Contains("transacciones asociadas"))
            {
                throw new ArgumentException(ex.Message);
            }
        }
    }
}
