using Backend.DTO;
using Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
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
            var result = _ahorroService.AddAhorro(ahorro);
            return Ok(result);
        }

        [HttpPost("obtenerAhorros")]
        public IActionResult ObtenerAhorros([FromBody] UsuarioDTO usuario)
        {
            var result = _ahorroService.GetAhorros(usuario.UsuarioId);
            return Ok(result);
        }

        [HttpPost("obtener-idAhorro")]
        public IActionResult GetAhorroById([FromBody] AhorroDTO request)
        {
            var result = _ahorroService.GetAhorroById(request.AhorroID.Value);
            return Ok(result);
        }

        [HttpPost("actualizar")]
        public IActionResult UpdateAhorro([FromBody] AhorroDTO ahorro)
        {
            var result = _ahorroService.UpdateAhorro(ahorro);
            return Ok(result);
        }

        [HttpPost("eliminar")]
        public IActionResult DeleteAhorro([FromBody] AhorroDTO request)
        {
            _ahorroService.DeleteAhorro(request.AhorroID.Value);
            return NoContent();
        }

        [HttpPost("detalle")]
        public IActionResult ObtenerMetaConDetalle([FromBody] AhorroDTO request)
        {
            var dto = _ahorroService.ObtenerMetaConDetalle(request.AhorroID.Value);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

    }
}
