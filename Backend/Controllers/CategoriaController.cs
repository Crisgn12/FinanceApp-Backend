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
    public class CategoriaController : ControllerBase
    {
        ICategoriaService _categoriaService;

        public CategoriaController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        [HttpGet]
        [Route("ObtenerCategoriasPorUsuario")]
        public async Task<IActionResult> ObtenerCategoriasPorUsuario()
        {
            return Ok(await _categoriaService.ObtenerCategoriasPorUsuarioID());
        }

        [HttpPost]
        [Route("CrearCategoria")]
        public Task<bool> CrearCategoriaPersonalizada(CategoriaDTO categoriaDTO)
        {
            return _categoriaService.CrearCategoriaPersonalizada(categoriaDTO);
        }

        [HttpPut]
        [Route("ActualizarCategoria")]
        public Task<bool> ActualizarCategoria(CategoriaDTO categoriaDTO)
        {
            return _categoriaService.ActualizarCategoriaAsync(categoriaDTO);
        }

        [HttpDelete]
        [Route("EliminarCategoria")]
        public Task<bool> EliminarCategoria(BorrarCategoriaDTO req)
        {
            return _categoriaService.EliminarCategoriaAsync(req);
        }
    }
}
