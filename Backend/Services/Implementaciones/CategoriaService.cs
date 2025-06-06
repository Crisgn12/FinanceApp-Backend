using Backend.Services.Interfaces;
using DAL.Implementaciones;
using DAL.Interfaces;
using Entidades.DTOs;
using Entidades.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Backend.Services.Implementaciones
{
    public class CategoriaService: ICategoriaService
    {
        IUnidadDeTrabajo _unidadDeTrabajo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CategoriaService(IUnidadDeTrabajo unidadDeTrabajo, IHttpContextAccessor httpContextAccessor)
        {
            _unidadDeTrabajo = unidadDeTrabajo;
            _httpContextAccessor = httpContextAccessor;
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

        public async Task<IEnumerable<CategoriaDTO>> ObtenerCategoriasPorUsuarioID()
        {
            var usuarioID = GetCurrentUserId();

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

            categoriaDTO.UsuarioID = GetCurrentUserId();
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
                categoriaDTO.UsuarioID = GetCurrentUserId();

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
                req.UsuarioID = GetCurrentUserId();

                return await _unidadDeTrabajo.CategoriaDAL.EliminarCategoria(req);
            }
            catch (Exception ex) when (ex.Message.Contains("no existe") || ex.Message.Contains("no tiene permiso") || ex.Message.Contains("transacciones asociadas"))
            {
                throw new ArgumentException(ex.Message);
            }
        }

        private string GetCurrentUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null || !user.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("Usuario no autenticado");
            }

            // Intenta obtener el ID de múltiples claims estándar
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                      ?? user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                      ?? user.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                // Log para depuración: muestra todos los claims disponibles
                var claims = user.Claims.Select(c => $"{c.Type}: {c.Value}");
                throw new UnauthorizedAccessException("No se pudo obtener el ID del usuario del token");
            }

            return userId;
        }
    }
}
