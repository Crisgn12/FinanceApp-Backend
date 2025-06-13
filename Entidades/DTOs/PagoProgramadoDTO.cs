using System;

namespace Entidades.DTOs;

public class PagoProgramadoDTO
{
    public int PagoId { get; set; }
    public string UsuarioId { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public DateOnly? FechaVencimiento { get; set; } // Cambiado a DateOnly?
    public string Estado { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } // Mantenido como DateTime para timestamp
    public bool EsProgramado { get; set; }
    public string Frecuencia { get; set; } = string.Empty;
    public DateOnly? FechaInicio { get; set; } // Cambiado a DateOnly?
    public DateOnly? FechaFin { get; set; } // Cambiado a DateOnly?
    public DateOnly? ProximoVencimiento { get; set; } // Cambiado a DateOnly?
    public bool? Activo { get; set; }
}