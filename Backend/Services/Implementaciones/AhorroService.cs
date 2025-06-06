using Backend.DTO;
using Backend.Services.Interfaces;
using DAL.Interfaces;
using Entidades.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using Humanizer;

namespace Backend.Services.Implementaciones
{
    public class AhorroService : IAhorroService
    {
        ILogger<AhorroService> _logger;
        IUnidadDeTrabajo _unidadDeTrabajo;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AhorroService(IUnidadDeTrabajo unidad, ILogger<AhorroService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _unidadDeTrabajo = unidad;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        private AhorroDTO Convertir(Ahorro ahorro)
        {
            return new AhorroDTO
            {
                AhorroID = ahorro.AhorroId,
                UsuarioID = ahorro.UsuarioId,
                Nombre = ahorro.Nombre,
                Monto_Objetivo = ahorro.MontoObjetivo,
                Monto_Actual = ahorro.MontoActual,
                Fecha_Meta = ahorro.FechaMeta,
                Completado = ahorro.Completado,
                PorcentajeAvance = ahorro.MontoObjetivo == 0 ? 0 : (ahorro.MontoActual / ahorro.MontoObjetivo) * 100
            };
        }

        private Ahorro Convertir(AhorroDTO dto)
        {
            return new Ahorro
            {
                AhorroId = dto.AhorroID ?? 0,
                UsuarioId = dto.UsuarioID,
                Nombre = dto.Nombre,
                MontoObjetivo = dto.Monto_Objetivo.Value,
                MontoActual = dto.Monto_Actual.Value,
                FechaMeta = dto.Fecha_Meta,
                Completado = dto.Completado.Value,
                CreatedAt = DateTime.UtcNow
            };
        }

        public AhorroDTO AddAhorro(AhorroDTO ahorro)
        {
            try
            {
                _logger.LogError("Ingresa a AddAhorro");

                var usuarioId = GetCurrentUserId();           
                ahorro.UsuarioID = usuarioId;

                var entity = _unidadDeTrabajo.AhorroDALImpl.AddAhorro(Convertir(ahorro));
                _unidadDeTrabajo.GuardarCambios();
                return Convertir(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar ahorro");
                throw;
            }
        }

        public AhorroDTO UpdateAhorro(AhorroDTO dto)
        {
            try
            {
                _logger.LogError("Ingresa a UpdateAhorro");

                var usuarioId = GetCurrentUserId();
                dto.UsuarioID = usuarioId;

                if (dto.AhorroID == null || dto.AhorroID <= 0)
                    throw new ArgumentException("ID de ahorro inválido.");

                var entity = _unidadDeTrabajo.AhorroDALImpl.FindById(dto.AhorroID.Value);
                if (entity == null)
                    throw new Exception("No se encontró el ahorro con el ID especificado.");

                // Solo actualiza los campos que vienen en el DTO
                if (!string.IsNullOrWhiteSpace(dto.Nombre))
                    entity.Nombre = dto.Nombre;

                if (dto.Monto_Objetivo.HasValue)
                    entity.MontoObjetivo = dto.Monto_Objetivo.Value;

                if (dto.Completado.HasValue)
                    entity.Completado = dto.Completado.Value;

                if (dto.Fecha_Meta.HasValue)
                    entity.FechaMeta = dto.Fecha_Meta;

                entity.UpdatedAt = DateTime.UtcNow;

                VerificarProgreso(entity);

                _unidadDeTrabajo.AhorroDALImpl.UpdateAhorro(entity);
                _unidadDeTrabajo.GuardarCambios();

                _unidadDeTrabajo.AhorroDALImpl.ActualizarMontoActual(entity.AhorroId);

                var entidadRecargada = _unidadDeTrabajo.AhorroDALImpl.FindById(entity.AhorroId);

                return Convertir(entidadRecargada);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar ahorro");
                throw;
            }
        }

        public AhorroDTO DeleteAhorro(int id)
        {
            try
            {
                _logger.LogError("Ingresa a DeleteAhorro");

                var usuarioId = GetCurrentUserId();

                var entidadEnBd = _unidadDeTrabajo.AhorroDALImpl.FindById(id);
                if (entidadEnBd == null)
                {
                    _logger.LogWarning($"Ahorro con ID {id} no encontrado");
                    return null;
                }

                _unidadDeTrabajo.AhorroDALImpl.DeleteAhorro(id);
                _unidadDeTrabajo.GuardarCambios();
                return Convertir(entidadEnBd);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar ahorro");
                throw;
            }
        }

        public AhorroDTO GetAhorroById(int id)
        {
            try
            {
                _logger.LogError("Ingresa a GetAhorroById");

                var ahorro = _unidadDeTrabajo.AhorroDALImpl.FindById(id);
                if (ahorro == null)
                {
                    _logger.LogWarning($"Ahorro con ID {id} no encontrado");
                    return null;
                }

                return Convertir(ahorro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener ahorro por ID");
                throw;
            }
        }

        public List<AhorroDTO> GetAhorros()
        {
            try
            {
                _logger.LogError("Ingresa a GetAhorrosByUsuarioId");

                var usuarioId = GetCurrentUserId();
                var ahorrosEnt = _unidadDeTrabajo.AhorroDALImpl.GetAhorros(usuarioId);

                //  !!! Recalcular monto actual para cada ahorro:
                foreach (var a in ahorrosEnt)
                {
                    _unidadDeTrabajo.AhorroDALImpl.ActualizarMontoActual(a.AhorroId);
                }

                // Ahora recargamos cada uno para que traiga el monto_Actual actualizado
                var ahorrosRefrescados = ahorrosEnt
                    .Select(a => _unidadDeTrabajo.AhorroDALImpl.FindById(a.AhorroId))
                    .ToList();

                return ahorrosRefrescados.Select(a => Convertir(a)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener ahorros por ID de usuario");
                throw;
            }
        }

        private void VerificarProgreso(Ahorro ahorro)
        {
            var porcentaje = ahorro.MontoObjetivo == 0 ? 0 : (ahorro.MontoActual / ahorro.MontoObjetivo) * 100;
            if (porcentaje >= 100 && !ahorro.Completado)
            {
                ahorro.Completado = true;
                _logger.LogInformation($"Ahorro '{ahorro.Nombre}' completado!");
            }
            else if (porcentaje >= 50 && ahorro.UltimaNotificacion == null)
            {
                ahorro.UltimaNotificacion = DateTime.UtcNow;
                _logger.LogInformation($"Meta '{ahorro.Nombre}' llegó al 50%. ¡Buen trabajo!");
            }
        }

        public AhorroDTO ObtenerMetaConDetalle(int ahorroId)
        {
            try
            {
                _logger.LogError("Ingresa a ObtenerMetaConDetalle");

                var ahorro = _unidadDeTrabajo.AhorroDALImpl.FindById(ahorroId);
                if (ahorro == null)
                {
                    _logger.LogWarning($"Ahorro con ID {ahorroId} no encontrado");
                    return null;
                }

                var aportes = _unidadDeTrabajo.AporteMetaAhorroDALImpl
                                .ObtenerPorMeta(ahorroId)
                                .OrderByDescending(a => a.Fecha)
                                .ToList();

                var dto = Convertir(ahorro);
                dto.HistorialAportes = aportes.Select(a => new AporteMetaAhorroDTO
                {
                    Fecha = a.Fecha,
                    Monto = a.Monto,
                    Observaciones = a.Observaciones
                }).ToList();

                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener detalle de meta de ahorro");
                throw;
            }
        }

        public List<AhorroDTO> GetNotificaciones()
        {
            var usuarioId = GetCurrentUserId();
            var todas = _unidadDeTrabajo.AhorroDALImpl.GetAhorros(usuarioId);
            var notis = todas
                .Where(a => a.UltimaNotificacion.HasValue)
                .Select(a => Convertir(a))
                .ToList();
            return notis;
        }

        private string GetCurrentUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null || !user.Identity.IsAuthenticated)
            {
                _logger.LogError("Usuario no autenticado");
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
                _logger.LogError($"No se encontró el ID de usuario en los claims. Claims disponibles: {string.Join(", ", claims)}");
                throw new UnauthorizedAccessException("No se pudo obtener el ID del usuario del token");
            }

            return userId;
        }
    }
}