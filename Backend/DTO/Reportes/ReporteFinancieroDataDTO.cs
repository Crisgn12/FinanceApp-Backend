using System;
using System.Collections.Generic;

namespace Backend.DTO.Reportes
{
    public class ReporteFinancieroDataDTO
    {
        public string NombreCompletoUsuario { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public string TituloReporte { get; set; } = string.Empty;

        // Resumen Financiero Clave
        public decimal IngresosTotales { get; set; }
        public decimal GastosTotales { get; set; }
        public decimal BalanceNeto { get; set; }
        public decimal AhorroTotalAbonado { get; set; }

        // Desglose de Gastos por Categoría
        public List<GastoPorCategoriaDataDTO> GastosPorCategoria { get; set; } = new List<GastoPorCategoriaDataDTO>();

        // Abonos a Metas
        public List<AbonoMetaAhorroDataDTO> AbonosAMetas { get; set; } = new List<AbonoMetaAhorroDataDTO>();

        // Transacciones Detalladas (Opcional, pero útil para una visión completa)
        public List<TransaccionDataDTO> Transacciones { get; set; } = new List<TransaccionDataDTO>();
    }

    public class GastoPorCategoriaDataDTO
    {
        public string NombreCategoria { get; set; } = string.Empty;
        public decimal MontoGastado { get; set; }
        public decimal Porcentaje { get; set; }
    }

    public class AbonoMetaAhorroDataDTO
    {
        public string NombreMeta { get; set; } = string.Empty;
        public decimal MontoAbonadoPeriodo { get; set; }
        public decimal MontoActualMeta { get; set; }
        public decimal MontoObjetivoMeta { get; set; }
        public decimal ProgresoPorcentaje { get; set; }
    }

    public class TransaccionDataDTO
    {
        public DateOnly Fecha { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string CategoriaNombre { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty; // "Ingreso" o "Gasto"
        public decimal Monto { get; set; }
    }
}