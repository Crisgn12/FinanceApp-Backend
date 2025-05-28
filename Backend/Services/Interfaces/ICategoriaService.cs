using Entidades.DTOs;

namespace Backend.Services.Interfaces
{
    public interface ICategoriaService
    {
        Task<IEnumerable<CategoriaDTO>> ObtenerCategoriasPorUsuarioID(int usuarioID);
    }
}
