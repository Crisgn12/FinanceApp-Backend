using Backend.Services.Interfaces;
using DAL.Interfaces;
using Entidades.DTOs;
using Humanizer;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Backend.Services.Implementaciones
{
    public class PagoProgramadoService : IPagoProgramadoService
    {
        private readonly IPagoProgramadoDAL _repo;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PagoProgramadoService(IPagoProgramadoDAL repo, IHttpContextAccessor httpContextAccessor)
        {
            _repo = repo;
            _httpContextAccessor = httpContextAccessor;
        }
        public Task<bool> CrearAsync(CrearPagoProgramadoDTO dto)
        {
            if (dto.Monto <= 0)
                throw new ArgumentException("El monto debe ser mayor a cero.");

            if (string.IsNullOrWhiteSpace(dto.Titulo))
                throw new ArgumentException("El título es obligatorio.");

            var userId = GetCurrentUserId();
            dto.UsuarioId = userId;

            return _repo.CrearPagoProgramadoAsync(dto);
        }
        public Task<bool> ActualizarAsync(PagoProgramadoDTO dto)
        {
            var userId = GetCurrentUserId();
            dto.UsuarioId = userId;
            return _repo.ActualizarPagoProgramadoAsync(dto);
        }
        public async Task<bool> CambiarEstadoAsync(CambioEstadoDTO dto)
        {
            return await _repo.CambiarEstadoPagoProgramadoAsync(dto);
        }
        public Task<bool> EliminarAsync(PagoProgramadoDTO dto)
        {
            var usuarioId = GetCurrentUserId();
            return _repo.EliminarPagoProgramadoAsync(dto);
        }
        public Task<List<PagoProgramadoDTO>> ListarAsync(FiltroPagosProgramadosDTO filtro)
        {
            var userId = GetCurrentUserId();
            filtro.UsuarioId = userId;
            return _repo.ObtenerPagosProgramadosAsync(filtro);
        }
        public Task<List<PagoProgramadoDTO>> ListarProximosAsync(FiltroPagosProximosDTO filtro) {
            var userId = GetCurrentUserId();
            filtro.UsuarioId = userId;
            return _repo.ObtenerPagosProximosAsync(filtro);
        }
        private string GetCurrentUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null || !user.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("Usuario no autenticado");
            }

            // Intenta obtener el ID de múltiples claims estándar
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                      ?? user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                      ?? user.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                var claims = user.Claims.Select(c => $"{c.Type}: {c.Value}");
                throw new UnauthorizedAccessException("No se pudo obtener el ID del usuario del token");
            }

            return userId;
        }
    }
}
