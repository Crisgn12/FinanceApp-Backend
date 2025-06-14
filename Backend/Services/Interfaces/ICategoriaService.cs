﻿using Entidades.DTOs;

namespace Backend.Services.Interfaces
{
    public interface ICategoriaService
    {
        Task<IEnumerable<CategoriaDTO>> ObtenerCategoriasPorUsuarioID();
        Task<bool> CrearCategoriaPersonalizada(CategoriaDTO categoriaDTO);
        Task<bool> ActualizarCategoriaAsync(CategoriaDTO categoriaDTO);
        Task<bool> EliminarCategoriaAsync(BorrarCategoriaDTO req);
    }
}
