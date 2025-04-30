using Backend.DTO;

namespace Backend.Services.Interfaces
{
    public interface IUsuarioService
    {
        IEnumerable<UsuarioDTO> ObtenerListaUsuarios();
    }
}
