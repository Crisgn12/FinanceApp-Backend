using Backend.Services.Interfaces;
using Entidades.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]                          
    public class PagoProgramadoController : ControllerBase
    {
        private readonly IPagoProgramadoService _service;

        public PagoProgramadoController(IPagoProgramadoService service)
        {
            _service = service;
        }

        /* ----------- POST: crear ----------- */
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CrearPagoProgramadoDTO dto)
            => Ok(await _service.CrearAsync(dto));

        /* ----------- PUT: actualizar ----------- */
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] PagoProgramadoDTO dto)
        {
            if (id != dto.PagoId) return BadRequest("ID inconsistente.");
            return Ok(await _service.ActualizarAsync(dto));
        }

        /* ----------- DELETE: eliminar ----------- */
        [HttpDelete("{id:int}/usuario")]
        public async Task<IActionResult> Eliminar(int id)
            => Ok(await _service.EliminarAsync(id));

        /* ----------- GET: listar todos (filtro) ----------- */
        [HttpGet("usuario")]
        public async Task<IActionResult> Listar(            
            [FromQuery] DateOnly? fechaInicio,
            [FromQuery] DateOnly? fechaFin,
            [FromQuery] bool? soloActivos)
        {
            var filtro = new FiltroPagosProgramadosDTO
            {
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                SoloActivos = soloActivos
            };
            return Ok(await _service.ListarAsync(filtro));
        }

        /* ----------- GET: listar próximos ----------- */
        [HttpGet("proximos/usuario")]
        public async Task<IActionResult> ListarProximos(int usuarioId, [FromQuery] int dias = 3)
            => Ok(await _service.ListarProximosAsync(new FiltroPagosProximosDTO
            {
                DiasAnticipacion = dias
            }));
    }
}
