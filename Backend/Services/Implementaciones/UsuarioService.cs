using Backend.DTO;
using Backend.Services.Interfaces;
using DAL.Interfaces;

namespace Backend.Services.Implementaciones
{
    public class UsuarioService: IUsuarioService
    {
        IUnidadDeTrabajo _unidadDeTrabajo;

        public UsuarioService(IUnidadDeTrabajo unidadDeTrabajo)
        {
            _unidadDeTrabajo = unidadDeTrabajo;
        }

        public IEnumerable<UsuarioDTO> ObtenerListaUsuarios()
        {
            var usuarios = _unidadDeTrabajo.UsuarioDAL.GetAll();
            var listaUsuarios = new List<UsuarioDTO>();
            foreach (var usuario in usuarios)
            {
                listaUsuarios.Add(new UsuarioDTO
                {
                    UsuarioId = usuario.UsuarioId,
                    Nombre = usuario.Nombre,
                    Apellidos = usuario.Apellidos,
                    Email = usuario.Email
                });
            }
            return listaUsuarios;
        }
    }
}
