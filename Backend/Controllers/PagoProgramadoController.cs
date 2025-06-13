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

        /* ----------- PUT: cambiar estado (NUEVO) ----------- */
        [HttpPut("{id:int}/estado")]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] CambioEstadoDTO dto)
        {
            if (id != dto.PagoId)
            {
                return BadRequest("El ID en la ruta no coincide con el del cuerpo.");
            }

            if (!EsEstadoValido(dto.Estado))
            {
                return BadRequest("Estado no válido.");
            }

            var exito = await _service.CambiarEstadoAsync(dto);
            if (!exito)
            {
                return StatusCode(500, "No se pudo actualizar el estado del pago.");
            }

            return Ok();
        }


        private bool EsEstadoValido(string estado)
        {
            // Lista de estados válidos según tu lógica
            var estadosValidos = new[] { "Vencido", "Pagado", "Pendiente"};
            return estadosValidos.Contains(estado);
        }

        /* ----------- DELETE: eliminar ----------- */
        [HttpDelete("{id:int}/usuario")]
        public async Task<IActionResult> Eliminar(PagoProgramadoDTO dto)
            => Ok(await _service.EliminarAsync(dto));

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
        public async Task<IActionResult> ListarProximos([FromQuery] int dias = 3)
            => Ok(await _service.ListarProximosAsync(new FiltroPagosProximosDTO
            {
                DiasAnticipacion = dias
            }));
    }
}
