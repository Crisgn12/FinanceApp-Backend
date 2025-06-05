using Entidades.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Services.Interfaces
{
    public interface IPagoProgramadoService
    {
        Task<bool> CrearAsync(CrearPagoProgramadoDTO dto);
        Task<bool> ActualizarAsync(PagoProgramadoDTO dto);
        Task<bool> EliminarAsync(int pagoId, int usuarioId);
        Task<List<PagoProgramadoDTO>> ListarAsync(FiltroPagosProgramadosDTO filtro);
        Task<List<PagoProgramadoDTO>> ListarProximosAsync(FiltroPagosProximosDTO filtro);
    }
}
