using Backend.DTO.Reportes;
using System.Threading.Tasks;
using System;

namespace Backend.Services.Interfaces
{
    public interface IReporteService
    {
        // Obtiene los datos del reporte para un usuario y rango de fechas
        Task<ReporteFinancieroDataDTO> GetReporteFinancieroDataAsync(ReporteFinancieroRequestDTO request);

        // Genera el PDF y devuelve los bytes
        byte[] GenerarPdfReporteFinanciero(ReporteFinancieroDataDTO data);
    }
}