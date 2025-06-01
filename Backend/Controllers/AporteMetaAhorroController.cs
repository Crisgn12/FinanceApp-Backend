using Azure.Core;
using Backend.DTO;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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
        public IActionResult GetAporteByMeta([FromBody] AhorroDTO request)
        {
            var lista = _service.GetAportesByMeta(request.AhorroID.Value);
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

    }
}
