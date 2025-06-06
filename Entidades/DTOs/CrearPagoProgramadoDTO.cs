using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades.DTOs;
public class CrearPagoProgramadoDTO
{
    public string UsuarioId { get; set; }
    public string Titulo { get; set; } = null!;
    public string? Descripcion { get; set; }
    public decimal Monto { get; set; }
    public DateOnly FechaInicio { get; set; }
    public string Frecuencia { get; set; } = null!;
    public DateOnly? FechaFin { get; set; }
}