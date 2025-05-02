using Backend.DTO;
using Backend.Services.Interfaces;
using DAL.Interfaces;
using Entidades.Entities;
using Entidades.Request;
using Entidades.Response;

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
                //Hacer metodo Convertir
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

        public ResCrearUsuario CrearUsuario(ReqCrearUsuario req)
        {
            ResCrearUsuario res = new ResCrearUsuario();
            res.error = new List<Error>();

            try
            {

                if (req == null)
                {
                    Error error = new Error();
                    error.Descripcion = "El objeto de petición es nulo";
                    res.error.Add(error);
                }
                else
                {
                    if (String.IsNullOrEmpty(req.Nombre))
                    {
                        Error error = new Error();
                        error.Descripcion = "Nombre faltante";
                        res.error.Add(error);
                    }
                    if (String.IsNullOrEmpty(req.Apellidos))
                    {
                        Error error = new Error();
                        error.Descripcion = "Apellidos faltante";
                        res.error.Add(error);
                    }
                    if (String.IsNullOrEmpty(req.Email))
                    {
                        Error error = new Error();
                        error.Descripcion = "Email faltante";
                        res.error.Add(error);
                    }
                    if (String.IsNullOrEmpty(req.Password))
                    {
                        Error error = new Error();
                        error.Descripcion = "Password faltante";
                        res.error.Add(error);
                    }

                    if (res.error.Any())
                    {
                        res.resultado = false;
                    }
                    else
                    {
                        _unidadDeTrabajo.UsuarioDAL.Add(Convertir(req));

                        _unidadDeTrabajo.GuardarCambios();

                        res.resultado = true;
                    }
                }

            }
            catch (Exception ex)
            {
                Error error = new Error();
                error.Descripcion = "Error no controlado";
                res.error.Add(error);
                res.resultado = false;
            }

            return res;
        }

        private Usuario Convertir(ReqCrearUsuario req)
        {
            Usuario usuario = new Usuario();
            usuario.Nombre = req.Nombre;
            usuario.Apellidos = req.Apellidos;
            usuario.Email = req.Email;
            usuario.PasswordHash = req.Password;
            // CAMBIAR ESTO DESPUES PORQUE ESTA MALO
            usuario.LlaveHash = req.Password;
            usuario.FechaCreacion = DateTime.Now;
            return usuario;
        }
    }
}
