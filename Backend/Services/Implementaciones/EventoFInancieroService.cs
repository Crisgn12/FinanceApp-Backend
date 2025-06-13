using Backend.DTO;
using Backend.Services.Interfaces;
using DAL.Interfaces;
using Entidades.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;

namespace Backend.Services.Implementaciones
{
    public class EventoFinancieroService : IEventoFinancieroService
    {
        private readonly ILogger<EventoFinancieroService> _logger;
        private readonly IUnidadDeTrabajo _unidadDeTrabajo;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EventoFinancieroService(
            IUnidadDeTrabajo unidad,
            ILogger<EventoFinancieroService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _unidadDeTrabajo = unidad;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        private EventoFinancieroDTO Convertir(EventoFinanciero evento)
        {
            return new EventoFinancieroDTO
            {
                IdEvento = evento.IdEvento,
                UsuarioID = evento.UsuarioId,
                Titulo = evento.Titulo,
                Descripcion = evento.Descripcion,
                FechaInicio = evento.FechaInicio,
                FechaFin = evento.FechaFin,
                EsTodoElDia = evento.EsTodoElDia,
                Tipo = evento.Tipo,
                Monto = evento.Monto,
                ColorFondo = evento.ColorFondo,
                Frecuencia = evento.Frecuencia,
                Repeticiones = evento.Repeticiones,
                Activo = evento.Activo,
                RecurrenciaID = evento.RecurrenciaID
            };
        }

        private EventoFinanciero Convertir(EventoFinancieroDTO dto)
        {
            return new EventoFinanciero
            {
                IdEvento = dto.IdEvento ?? 0,
                UsuarioId = dto.UsuarioID,
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion,
                FechaInicio = dto.FechaInicio.Value,
                FechaFin = dto.FechaFin.Value,
                EsTodoElDia = dto.EsTodoElDia.Value,
                Tipo = dto.Tipo,
                Monto = dto.Monto,
                ColorFondo = dto.ColorFondo,
                Frecuencia = dto.Frecuencia,
                Repeticiones = dto.Repeticiones,
                Activo = dto.Activo ?? true,
                RecurrenciaID = dto.RecurrenciaID,
                CreatedAt = DateTime.UtcNow
            };
        }

        public EventoFinancieroDTO CrearEventoFinanciero(EventoFinancieroDTO evento)
        {
            try
            {
                _logger.LogInformation("Ingresa a CrearEventoFinanciero");

                var usuarioId = GetCurrentUserId();
                evento.UsuarioID = usuarioId;

                ValidarEventoFinanciero(evento);

                var entity = _unidadDeTrabajo.EventoFinancieroDAL.CrearEventoFinanciero(Convertir(evento));
                _unidadDeTrabajo.GuardarCambios();

                return Convertir(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear evento financiero");
                throw;
            }
        }

        public EventoFinancieroDTO ActualizarEventoFinanciero(EventoFinancieroDTO dto)
        {
            try
            {
                _logger.LogInformation("Ingresa a ActualizarEventoFinanciero");

                var usuarioId = GetCurrentUserId();
                dto.UsuarioID = usuarioId;

                if (dto.IdEvento == null || dto.IdEvento <= 0)
                    throw new ArgumentException("ID de evento inválido.");

                var entity = _unidadDeTrabajo.EventoFinancieroDAL.FindById(dto.IdEvento.Value);
                if (entity == null)
                    throw new Exception("No se encontró el evento con el ID especificado.");

                ValidarEventoFinanciero(dto);

                // Actualizar los campos
                if (!string.IsNullOrWhiteSpace(dto.Titulo))
                    entity.Titulo = dto.Titulo;

                if (!string.IsNullOrWhiteSpace(dto.Descripcion))
                    entity.Descripcion = dto.Descripcion;

                if (dto.FechaInicio.HasValue)
                    entity.FechaInicio = dto.FechaInicio.Value;

                if (dto.FechaFin.HasValue)
                    entity.FechaFin = dto.FechaFin.Value;

                if (dto.EsTodoElDia.HasValue)
                    entity.EsTodoElDia = dto.EsTodoElDia.Value;

                if (!string.IsNullOrWhiteSpace(dto.Tipo))
                    entity.Tipo = dto.Tipo;

                if (dto.Monto.HasValue)
                    entity.Monto = dto.Monto.Value;

                if (!string.IsNullOrWhiteSpace(dto.ColorFondo))
                    entity.ColorFondo = dto.ColorFondo;

                if (!string.IsNullOrWhiteSpace(dto.Frecuencia))
                    entity.Frecuencia = dto.Frecuencia;

                if (dto.Repeticiones.HasValue)
                    entity.Repeticiones = dto.Repeticiones;

                if (dto.Activo.HasValue)
                    entity.Activo = dto.Activo.Value;

                if (dto.RecurrenciaID.HasValue)
                    entity.RecurrenciaID = dto.RecurrenciaID;

                entity.UpdatedAt = DateTime.UtcNow;

                _unidadDeTrabajo.EventoFinancieroDAL.ActualizarEventoFinanciero(entity);
                _unidadDeTrabajo.GuardarCambios();

                var entidadRecargada = _unidadDeTrabajo.EventoFinancieroDAL.FindById(entity.IdEvento);
                return Convertir(entidadRecargada);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar evento financiero");
                throw;
            }
        }

        public EventoFinancieroDTO EliminarEventoFinanciero(int idEvento)
        {
            try
            {
                _logger.LogInformation("Ingresa a EliminarEventoFinanciero");

                var usuarioId = GetCurrentUserId();

                var entidadEnBd = _unidadDeTrabajo.EventoFinancieroDAL.FindById(idEvento);
                if (entidadEnBd == null || entidadEnBd.UsuarioId != usuarioId)
                {
                    _logger.LogWarning($"Evento financiero con ID {idEvento} no encontrado o no pertenece al usuario");
                    return null;
                }

                _unidadDeTrabajo.EventoFinancieroDAL.EliminarEventoFinanciero(idEvento);
                _unidadDeTrabajo.GuardarCambios();

                return Convertir(entidadEnBd);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar evento financiero");
                throw;
            }
        }

        public void EliminarEventosPorRecurrencia(int recurrenciaId)
        {
            try
            {
                _logger.LogInformation("Ingresa a EliminarEventosPorRecurrencia");

                _unidadDeTrabajo.EventoFinancieroDAL.EliminarEventosPorRecurrencia(recurrenciaId);
                _unidadDeTrabajo.GuardarCambios();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar eventos por recurrencia");
                throw;
            }
        }

        public void ActualizarEventosPorRecurrencia(ActualizarEventosPorRecurrenciaDTO dto)
        {
            try
            {
                _logger.LogInformation("Ingresa a ActualizarEventosPorRecurrencia");

                _unidadDeTrabajo.EventoFinancieroDAL.ActualizarEventosPorRecurrencia(
                    dto.RecurrenciaID,
                    dto.ColorFondo,
                    dto.Monto,
                    dto.Activo);

                _unidadDeTrabajo.GuardarCambios();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar eventos por recurrencia");
                throw;
            }
        }

        public List<EventoFinancieroDTO> ListarEventosPorUsuarioYRango(DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                _logger.LogInformation("Ingresa a ListarEventosPorUsuarioYRango");

                var usuarioId = GetCurrentUserId();
                var eventosEnt = _unidadDeTrabajo.EventoFinancieroDAL
                    .ListarEventosPorUsuarioYRango(usuarioId, fechaInicio, fechaFin);

                return eventosEnt.Select(e => Convertir(e)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar eventos por usuario y rango");
                throw;
            }
        }

        public EventoFinancieroDTO GetEventoFinancieroById(int idEvento)
        {
            try
            {
                _logger.LogInformation("Ingresa a GetEventoFinancieroById");

                var evento = _unidadDeTrabajo.EventoFinancieroDAL.FindById(idEvento);
                if (evento == null)
                {
                    _logger.LogWarning($"Evento financiero con ID {idEvento} no encontrado");
                    return null;
                }

                return Convertir(evento);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener evento financiero por ID");
                throw;
            }
        }

        public List<EventoFinancieroDTO> GetEventosPorUsuario()
        {
            try
            {
                _logger.LogInformation("Ingresa a GetEventosPorUsuario");

                var usuarioId = GetCurrentUserId();
                var eventosEnt = _unidadDeTrabajo.EventoFinancieroDAL.GetEventosPorUsuario(usuarioId);

                return eventosEnt.Select(e => Convertir(e)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener eventos por usuario");
                throw;
            }
        }

        private void ValidarEventoFinanciero(EventoFinancieroDTO evento)
        {
            if (string.IsNullOrWhiteSpace(evento.Titulo))
                throw new ArgumentException("El título del evento es requerido.");

            if (!evento.FechaInicio.HasValue)
                throw new ArgumentException("La fecha de inicio es requerida.");

            if (!evento.FechaFin.HasValue)
                throw new ArgumentException("La fecha de fin es requerida.");

            if (evento.FechaInicio > evento.FechaFin)
                throw new ArgumentException("La fecha de inicio no puede ser mayor a la fecha de fin.");

            if (string.IsNullOrWhiteSpace(evento.Tipo))
                throw new ArgumentException("El tipo de evento es requerido.");

            var tiposValidos = new[] { "Ingreso", "Gasto", "Recordatorio" };
            if (!tiposValidos.Contains(evento.Tipo))
                throw new ArgumentException("Tipo de evento inválido. Debe ser: Ingreso, Gasto o Recordatorio.");

            if (!string.IsNullOrWhiteSpace(evento.Frecuencia))
            {
                var frecuenciasValidas = new[] { "Diaria", "Semanal", "Mensual", "Anual" };
                if (!frecuenciasValidas.Contains(evento.Frecuencia))
                    throw new ArgumentException("Frecuencia inválida. Debe ser: Diaria, Semanal, Mensual o Anual.");
            }
        }

        private string GetCurrentUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null || !user.Identity.IsAuthenticated)
            {
                _logger.LogError("Usuario no autenticado");
                throw new UnauthorizedAccessException("Usuario no autenticado");
            }

            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                      ?? user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                      ?? user.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                var claims = user.Claims.Select(c => $"{c.Type}: {c.Value}");
                _logger.LogError($"No se encontró el ID de usuario en los claims. Claims disponibles: {string.Join(", ", claims)}");
                throw new UnauthorizedAccessException("No se pudo obtener el ID del usuario del token");
            }

            return userId;
        }
    }
}