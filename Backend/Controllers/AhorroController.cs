using Backend.DTO;
using Backend.Services.Interfaces;
using Entidades.DTOs;
using Entidades.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AhorroController : ControllerBase
    {
        private readonly IAhorroService _ahorroService;

        public AhorroController(IAhorroService ahorroService)
        {
            _ahorroService = ahorroService;
        }

        [HttpPost("crear")]
        public IActionResult CrearAhorro([FromBody] AhorroDTO ahorro)
        {
            if (ahorro == null || string.IsNullOrWhiteSpace(ahorro.Nombre) || ahorro.Monto_Objetivo <= 0)
                return BadRequest("Datos inválidos para la meta de ahorro.");

            var result = _ahorroService.AddAhorro(ahorro);
            return Ok(result);
        }

        [HttpGet("obtenerAhorros")]
        public IActionResult ObtenerAhorros()
        {
            var result = _ahorroService.GetAhorros();
            return Ok(result);
        }

        [HttpPost("obtener-idAhorro")]
        public IActionResult GetAhorroById([FromBody] AhorroDTO request)
        {
            if (request.AhorroID == null || request.AhorroID <= 0)
                return BadRequest("ID de ahorro inválido.");

            var result = _ahorroService.GetAhorroById(request.AhorroID.Value);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost("actualizar")]
        public IActionResult UpdateAhorro([FromBody] AhorroDTO ahorro)
        {
            if (ahorro.AhorroID == null || ahorro.AhorroID <= 0)
                return BadRequest("ID de ahorro inválido para actualizar.");

            var result = _ahorroService.UpdateAhorro(ahorro);
            return Ok(result);
        }

        [HttpPost("eliminar")]
        public IActionResult DeleteAhorro([FromBody] AhorroDTO request)
        {
            if (request.AhorroID == null || request.AhorroID <= 0)
                return BadRequest("ID de ahorro inválido para eliminar.");

            var dto = _ahorroService.DeleteAhorro(request.AhorroID.Value);
            if (dto == null) return NotFound();
            return NoContent();
        }

        [HttpPost("detalle")]
        public IActionResult ObtenerMetaConDetalle([FromBody] AhorroDTO request)
        {
            if (request.AhorroID == null || request.AhorroID <= 0)
                return BadRequest("ID de ahorro inválido.");

            var dto = _ahorroService.ObtenerMetaConDetalle(request.AhorroID.Value);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpGet("usuario/{usuarioId}/notificaciones")]
        public IActionResult GetNotificaciones()
        {
            var notis = _ahorroService.GetNotificaciones();
            return Ok(notis);
        }
    }
}