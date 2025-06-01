using Backend.Services.Interfaces;
using DAL.Interfaces;
using Entidades.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Services.Implementaciones
{
    public class PagoProgramadoService : IPagoProgramadoService
    {
        private readonly IPagoProgramadoDAL _repo;

        public PagoProgramadoService(IPagoProgramadoDAL repo)
        {
            _repo = repo;
        }

        public Task<bool> CrearAsync(CrearPagoProgramadoDTO dto)
        {
            if (dto.Monto <= 0)
                throw new ArgumentException("El monto debe ser mayor a cero.");

            if (string.IsNullOrWhiteSpace(dto.Titulo))
                throw new ArgumentException("El título es obligatorio.");

            return _repo.CrearPagoProgramadoAsync(dto);
        }

        public Task<bool> ActualizarAsync(PagoProgramadoDTO dto) =>
            _repo.ActualizarPagoProgramadoAsync(dto);

        public Task<bool> EliminarAsync(int pagoId, int usuarioId) =>
            _repo.EliminarPagoProgramadoAsync(pagoId, usuarioId);

        public Task<List<PagoProgramadoDTO>> ListarAsync(FiltroPagosProgramadosDTO filtro) =>
            _repo.ObtenerPagosProgramadosAsync(filtro);

        public Task<List<PagoProgramadoDTO>> ListarProximosAsync(FiltroPagosProximosDTO filtro) =>
            _repo.ObtenerPagosProximosAsync(filtro);
    }
}
