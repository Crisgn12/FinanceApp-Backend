using Backend.Services.Interfaces;
using Entidades.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TransaccionController : ControllerBase
    {
        ITransaccionService _transaccionService;

        public TransaccionController(ITransaccionService transaccionService)
        {
            _transaccionService = transaccionService;
        }

        [HttpPost]
        [Route("IngresarTransaccion")]
        public Task<bool> IngresarTransaccion(TransaccionDTO transaccionDTO)
        {
            return _transaccionService.IngresarTransaccionAsync(transaccionDTO);
        }

        [HttpPost]
        [Route("ObtenerTransaccionesPorUsuario")]
        public Task<List<TransaccionDTO>> ObtenerTransaccionesPorUsuario(ObtenerTransaccionesPorUsuarioDTO req)
        {
            return _transaccionService.ObtenerTransaccionesPorUsuarioAsync(req);
        }

        [HttpPost]
        [Route("ObtenerDetalleTransaccion")]
        public Task<TransaccionDTO> ObtenerDetalleTransaccion(ObtenerDetalleTransaccionDTO req)
        {
            return _transaccionService.ObtenerDetalleTransaccionAsync(req);
        }

        [HttpPut]
        [Route("ActualizarTransaccion")]
        public Task<bool> ActualizarTransaccion(TransaccionDTO transaccionDTO)
        {
            return _transaccionService.ActualizarTransaccionAsync(transaccionDTO);
        }

        [HttpDelete]
        [Route("EliminarTransaccion")]
        public Task<bool> EliminarTransaccion(EliminarTransaccionDTO req)
        {
            return _transaccionService.EliminarTransaccionAsync(req);
        }
    }
}
