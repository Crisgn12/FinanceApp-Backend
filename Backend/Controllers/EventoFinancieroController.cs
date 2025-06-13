using Backend.DTO;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EventosFinancieroController : ControllerBase
    {
        private readonly IEventosFinancieroService _EventosFinancieroService;

        public EventosFinancieroController(IEventosFinancieroService EventosFinancieroService)
        {
            _EventosFinancieroService = EventosFinancieroService;
        }

        [HttpPost("crear")]
        public IActionResult CrearEventosFinanciero([FromBody] EventosFinancieroDTO evento)
        {
            try
            {
                if (evento == null || string.IsNullOrWhiteSpace(evento.Titulo) ||
                    !evento.FechaInicio.HasValue || !evento.FechaFin.HasValue)
                    return BadRequest("Datos inválidos para el evento financiero.");

                var result = _EventosFinancieroService.CrearEventosFinanciero(evento);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost("actualizar")]
        public IActionResult ActualizarEventosFinanciero([FromBody] EventosFinancieroDTO evento)
        {
            try
            {
                if (evento.IdEvento == null || evento.IdEvento <= 0)
                    return BadRequest("ID de evento inválido para actualizar.");

                var result = _EventosFinancieroService.ActualizarEventosFinanciero(evento);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost("eliminar")]
        public IActionResult EliminarEventosFinanciero([FromBody] EliminarEventoDTO request)
        {
            try
            {
                if (request == null || request.IdEvento <= 0)
                    return BadRequest("ID de evento inválido.");

                var result = _EventosFinancieroService.EliminarEventosFinanciero(request.IdEvento);

                if (result == null)
                    return NotFound("Evento financiero no encontrado o no pertenece al usuario.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost("eliminar-por-recurrencia")]
        public IActionResult EliminarEventosPorRecurrencia([FromBody] EliminarEventosPorRecurrenciaDTO request)
        {
            try
            {
                if (request == null || request.RecurrenciaId <= 0)
                    return BadRequest("ID de recurrencia inválido.");

                _EventosFinancieroService.EliminarEventosPorRecurrencia(request.RecurrenciaId);
                return Ok("Eventos eliminados exitosamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost("actualizar-por-recurrencia")]
        public IActionResult ActualizarEventosPorRecurrencia([FromBody] ActualizarEventosPorRecurrenciaDTO dto)
        {
            try
            {
                if (dto == null || dto.RecurrenciaId <= 0)
                    return BadRequest("Datos inválidos para actualizar eventos por recurrencia.");

                _EventosFinancieroService.ActualizarEventosPorRecurrencia(dto);
                return Ok("Eventos actualizados exitosamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost("listar-por-rango")]
        public IActionResult ListarEventosPorUsuarioYRango([FromBody] ListarEventosPorRangoDTO request)
        {
            try
            {
                if (request == null || request.FechaInicio == default || request.FechaFin == default)
                    return BadRequest("Las fechas de inicio y fin son requeridas.");

                if (request.FechaInicio > request.FechaFin)
                    return BadRequest("La fecha de inicio no puede ser mayor a la fecha de fin.");

                var result = _EventosFinancieroService.ListarEventosPorUsuarioYRango(request.FechaInicio, request.FechaFin);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost("obtener")]
        public IActionResult GetEventosFinancieroById([FromBody] ObtenerEventoPorIdDTO request)
        {
            try
            {
                if (request == null || request.IdEvento <= 0)
                    return BadRequest("ID de evento inválido.");

                var result = _EventosFinancieroService.GetEventosFinancieroById(request.IdEvento);

                if (result == null)
                    return NotFound("Evento financiero no encontrado.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost("listar-por-usuario")]
        public IActionResult GetEventosPorUsuario()
        {
            try
            {
                var result = _EventosFinancieroService.GetEventosPorUsuario();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}