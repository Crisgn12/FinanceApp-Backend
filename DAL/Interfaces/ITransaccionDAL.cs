﻿using Entidades.DTOs;
using Entidades.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface ITransaccionDAL: IDALGenerico<Transaccion>
    {
        Task<bool> IngresarTransaccion(TransaccionDTO transaccionDTO);
        Task<List<TransaccionDTO>> ObtenerTransaccionesPorUsuario(ObtenerTransaccionesPorUsuarioDTO req);
        Task<TransaccionDTO> ObtenerDetalleTransaccion(ObtenerDetalleTransaccionDTO req);
        Task<bool> ActualizarTransaccion(TransaccionDTO transaccionDTO);
        Task<bool> EliminarTransaccion(EliminarTransaccionDTO req);
        Task<List<TotalxDiaDTO>> TotalGastosUltimos6diasPorUsuario(string usuarioId);
        Task<List<GraficoCategoriasDTO>> TotalGastosPorCategoria(string usuarioId);
        Task<decimal> TotalGastosxMes(string usuarioId);
        Task<decimal> TotalIngresosxMes(string usuarioId);
        Task<decimal> BalanceMesActual(string usuarioId);
    }
}
