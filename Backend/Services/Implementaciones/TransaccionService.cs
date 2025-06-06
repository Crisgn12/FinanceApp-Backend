using Backend.Services.Interfaces;
using DAL.Implementaciones;
using DAL.Interfaces;
using Entidades.DTOs;
using Entidades.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Backend.Services.Implementaciones
{
    public class TransaccionService : ITransaccionService
    {
        IUnidadDeTrabajo _unidadDeTrabajo;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TransaccionService(IUnidadDeTrabajo unidadDeTrabajo, IHttpContextAccessor httpContextAccessor)
        {
            _unidadDeTrabajo = unidadDeTrabajo;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> IngresarTransaccionAsync(TransaccionDTO transaccionDTO)
        {
            bool resultado = false;

            // Validaciones
            if (transaccionDTO.Monto <= 0)
            {
                throw new ArgumentException("El monto debe ser mayor que 0.");
            }
            if (transaccionDTO.Tipo != "Ingreso" && transaccionDTO.Tipo != "Gasto")
            {
                throw new ArgumentException("El tipo debe ser 'Ingreso' o 'Gasto'.");
            }
            if (transaccionDTO.Fecha == default)
            {
                throw new ArgumentException("La fecha es obligatoria.");
            }
            if (string.IsNullOrWhiteSpace(transaccionDTO.Titulo))
            {
                throw new ArgumentException("El título es obligatorio.");
            }

            transaccionDTO.UsuarioId = GetCurrentUserId();
            transaccionDTO.TransaccionId = null;

            try
            {
                resultado = await _unidadDeTrabajo.TransaccionDAL.IngresarTransaccion(transaccionDTO);
                _unidadDeTrabajo.GuardarCambios();
                return resultado;
            }
            catch (Exception ex) when (ex.Message.Contains("no existe") || ex.Message.Contains("monto") || ex.Message.Contains("fecha") || ex.Message.Contains("tipo"))
            {
                throw new ArgumentException(ex.Message);
            }
        }

        public async Task<List<TransaccionDTO>> ObtenerTransaccionesPorUsuarioAsync(ObtenerTransaccionesPorUsuarioDTO req)
        {
            try
            {
                req.UsuarioId = GetCurrentUserId();

                return await _unidadDeTrabajo.TransaccionDAL.ObtenerTransaccionesPorUsuario(req);
            }
            catch (Exception ex) when (ex.Message.Contains("no existe"))
            {
                throw new ArgumentException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener las transacciones: {ex.Message}");
            }
        }

        public async Task<TransaccionDTO> ObtenerDetalleTransaccionAsync(ObtenerDetalleTransaccionDTO req)
        {
            try
            {
                if (req.TransaccionId <= 0)
                {
                    throw new ArgumentException("El ID de la transacción es obligatorio.");
                }

                req.UsuarioId = GetCurrentUserId();

                return await _unidadDeTrabajo.TransaccionDAL.ObtenerDetalleTransaccion(req);
            }
            catch (Exception ex) when (ex.Message.Contains("no existe"))
            {
                throw new ArgumentException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el detalle de la transacción: {ex.Message}");
            }
        }

        public async Task<bool> ActualizarTransaccionAsync(TransaccionDTO transaccionDTO)
        {
            bool resultado = false;

            if (!transaccionDTO.TransaccionId.HasValue)
            {
                throw new ArgumentException("El ID de la transacción es obligatorio.");
            }
            if (transaccionDTO.Monto <= 0)
            {
                throw new ArgumentException("El monto debe ser mayor que 0.");
            }
            if (transaccionDTO.Tipo != "Ingreso" && transaccionDTO.Tipo != "Gasto")
            {
                throw new ArgumentException("El tipo debe ser 'Ingreso' o 'Gasto'.");
            }
            if (transaccionDTO.Fecha == default)
            {
                throw new ArgumentException("La fecha es obligatoria.");
            }
            if (string.IsNullOrWhiteSpace(transaccionDTO.Titulo))
            {
                throw new ArgumentException("El título es obligatorio.");
            }

            try
            {
                transaccionDTO.UsuarioId = GetCurrentUserId();

                resultado = await _unidadDeTrabajo.TransaccionDAL.ActualizarTransaccion(transaccionDTO);

                _unidadDeTrabajo.GuardarCambios();

                return resultado;
            }
            catch (Exception ex) when (ex.Message.Contains("no existe") || ex.Message.Contains("monto") || ex.Message.Contains("fecha") || ex.Message.Contains("tipo"))
            {
                throw new ArgumentException(ex.Message);
            }
        }

        public async Task<bool> EliminarTransaccionAsync(EliminarTransaccionDTO req)
        {
            try
            {
                bool resultado = false;

                if (req.TransaccionID <= 0)
                {
                    throw new ArgumentException("El ID de la transacción es obligatorio.");
                }

                req.UsuarioID = GetCurrentUserId();

                resultado = await _unidadDeTrabajo.TransaccionDAL.EliminarTransaccion(req);

                _unidadDeTrabajo.GuardarCambios();

                return resultado;
            }
            catch (Exception ex) when (ex.Message.Contains("no existe"))
            {
                throw new ArgumentException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al eliminar la transacción: {ex.Message}");
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
