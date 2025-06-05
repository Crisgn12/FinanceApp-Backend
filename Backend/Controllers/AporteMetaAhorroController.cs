using Backend.DTO;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class AporteMetaAhorroController : ControllerBase
    {
        private readonly IAporteMetaAhorroService _service;

        public AporteMetaAhorroController(IAporteMetaAhorroService service)
        {
            _service = service;
        }

        // POST: api/AporteMetaAhorro/agregar
        [HttpPost("agregar")]
        public IActionResult AddAporte([FromBody] AporteMetaAhorroDTO dto)
        {
            if (dto == null || dto.Monto <= 0 || dto.MetaAhorroId <= 0)
                return BadRequest("Datos del aporte inválidos.");

            var result = _service.AddAporte(dto);
            if (result == null)
                return StatusCode(500, "No se pudo agregar el aporte.");

            return Ok(result);
        }

        // POST: api/AporteMetaAhorro/porMeta
        [HttpPost("porMeta")]
        public IActionResult GetAporteByMeta([FromBody] AporteMetaAhorroDTO request)
        {
            var lista = _service.GetAportesByMeta(request.MetaAhorroId);
            return Ok(lista);
        }

        // POST: api/AporteMetaAhorro/eliminar
        [HttpPost("eliminar")]
        public IActionResult DeleteAporte([FromBody] AporteMetaAhorroDTO req)
        {
            if (req.AporteId <= 0)
                return BadRequest("ID de aporte inválido.");

            var resultado = _service.DeleteAporte(req.AporteId.Value);
            if (!resultado)
                return NotFound(new { mensaje = "Aporte no encontrado o no se pudo eliminar." });

            return Ok(new { mensaje = "Aporte eliminado correctamente." });
        }

        // POST: api/AporteMetaAhorro/editar
        [HttpPost("editar")]
        public IActionResult UpdateAporte([FromBody] AporteMetaAhorroDTO dto)
        {
            if (dto == null || dto.AporteId == null || dto.AporteId <= 0
                || dto.MetaAhorroId <= 0 || dto.Monto <= 0)
            {
                return BadRequest("Datos inválidos para actualizar el aporte.");
            }

            var actualizado = _service.UpdateAporte(dto);
            if (actualizado == null)
                return NotFound(new { mensaje = $"No existe aporte con ID {dto.AporteId}" });

            return Ok(actualizado);
        }
    }
}