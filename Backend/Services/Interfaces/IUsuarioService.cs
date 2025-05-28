using Entidades.DTOs;
using Entidades.Request;
using Entidades.Response;

namespace Backend.Services.Interfaces
{
    public interface IUsuarioService
    {
        IEnumerable<UsuarioDTO> ObtenerListaUsuarios();
        ResCrearUsuario CrearUsuario(ReqCrearUsuario req);
    }
}
