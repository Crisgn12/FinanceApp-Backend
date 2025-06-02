using Backend.DTO;
using Backend.Services.Interfaces;
using DAL.Interfaces;
using Entidades.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Backend.Services.Implementaciones
{
    public class AporteMetaAhorroService : IAporteMetaAhorroService
    {
        private readonly ILogger<AporteMetaAhorroService> _logger;
        private readonly IUnidadDeTrabajo _unidad;

        public AporteMetaAhorroService(IUnidadDeTrabajo unidad, ILogger<AporteMetaAhorroService> logger)
        {
            _unidad = unidad;
            _logger = logger;
        }

        private AporteMetaAhorroDTO Convertir(AporteMetaAhorro aporte)
        {
            return new AporteMetaAhorroDTO
            {
                AporteId = aporte.AporteId,
                MetaAhorroId = aporte.MetaAhorroId,
                Fecha = aporte.Fecha,
                Monto = aporte.Monto,
                Observaciones = aporte.Observaciones
            };
        }

        private AporteMetaAhorro Convertir(AporteMetaAhorroDTO dto)
        {
            return new AporteMetaAhorro
            {
                AporteId = dto.AporteId ?? 0,
                MetaAhorroId = dto.MetaAhorroId,
                Fecha = dto.Fecha,
                Monto = dto.Monto,
                Observaciones = dto.Observaciones
            };
        }

        public AporteMetaAhorroDTO AddAporte(AporteMetaAhorroDTO aporte)
        {
            try
            {
                if (aporte == null || aporte.Monto <= 0 || aporte.MetaAhorroId <= 0)
                {
                    _logger.LogWarning("Aporte inválido");
                    return null;
                }

                if (aporte.Fecha == default)
                    aporte.Fecha = DateTime.UtcNow;

                var entity = Convertir(aporte);
                var agregado = _unidad.AporteMetaAhorroDALImpl.Add(entity);
                if (!agregado) return null;

                // Actualiza la meta en la misma transacción:
                var meta = _unidad.AhorroDALImpl.FindById(entity.MetaAhorroId);
                if (meta != null)
                {
                    meta.MontoActual += entity.Monto;
                    var porcentaje = meta.MontoObjetivo == 0 ? 0
                        : (meta.MontoActual / meta.MontoObjetivo) * 100m;

                    if (porcentaje >= 100m && !meta.Completado)
                    {
                        meta.Completado = true;
                        meta.UltimaNotificacion = DateTime.UtcNow;
                    }
                    else if (porcentaje >= 50m && meta.UltimaNotificacion == null)
                    {
                        meta.UltimaNotificacion = DateTime.UtcNow;
                    }

                    _unidad.AhorroDALImpl.UpdateAhorro(meta);
                }

                _unidad.GuardarCambios();
                return Convertir(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar aporte");
                throw;
            }
        }

        public List<AporteMetaAhorroDTO> GetAportesByMeta(int metaAhorroId)
        {
            try
            {
                var aportes = _unidad.AporteMetaAhorroDALImpl.ObtenerPorMeta(metaAhorroId);
                return aportes.Select(Convertir).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener aportes de la meta {metaAhorroId}");
                throw;
            }
        }

        public bool DeleteAporte(int aporteId)
        {
            try
            {
                var aporte = _unidad.AporteMetaAhorroDALImpl.FindById(aporteId);
                if (aporte == null)
                {
                    _logger.LogWarning($"No se encontró el aporte con ID {aporteId}");
                    return false;
                }

                var meta = _unidad.AhorroDALImpl.FindById(aporte.MetaAhorroId);
                if (meta == null)
                {
                    _logger.LogWarning($"No se encontró la meta con ID {aporte.MetaAhorroId}");
                    return false;
                }

                meta.MontoActual -= aporte.Monto;
                if (meta.MontoActual < 0m)
                    meta.MontoActual = 0m;

                var porcentaje = meta.MontoObjetivo == 0 ? 0
                    : (meta.MontoActual / meta.MontoObjetivo) * 100m;

                meta.Completado = porcentaje >= 100m;
                if (porcentaje < 50m)
                    meta.UltimaNotificacion = null;

                _unidad.AhorroDALImpl.UpdateAhorro(meta);
                _unidad.AporteMetaAhorroDALImpl.Remove(aporte);

                _unidad.GuardarCambios();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar el aporte con ID {aporteId}");
                throw;
            }
        }

        public AporteMetaAhorroDTO GetAporteById(int aporteId)
        {
            try
            {
                var aporte = _unidad.AporteMetaAhorroDALImpl.FindById(aporteId);
                return aporte == null ? null : Convertir(aporte);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener aporte con ID {aporteId}");
                throw;
            }
        }

        public AporteMetaAhorroDTO UpdateAporte(AporteMetaAhorroDTO dto)
        {
            try
            {
                if (dto == null || dto.AporteId == null || dto.AporteId <= 0 ||
                    dto.MetaAhorroId <= 0 || dto.Monto <= 0)
                {
                    _logger.LogWarning("Datos inválidos para actualizar el aporte");
                    return null;
                }

                var aporteExistente = _unidad.AporteMetaAhorroDALImpl.FindById(dto.AporteId.Value);
                if (aporteExistente == null)
                {
                    _logger.LogWarning($"No existe aporte con ID {dto.AporteId.Value}");
                    return null;
                }

                var meta = _unidad.AhorroDALImpl.FindById(aporteExistente.MetaAhorroId);
                if (meta == null)
                {
                    _logger.LogWarning($"No existe la meta con ID {aporteExistente.MetaAhorroId}");
                    return null;
                }

                meta.MontoActual -= aporteExistente.Monto;
                if (meta.MontoActual < 0m)
                    meta.MontoActual = 0m;

                aporteExistente.Fecha = dto.Fecha == default ? DateTime.UtcNow : dto.Fecha;
                aporteExistente.Monto = dto.Monto;
                aporteExistente.Observaciones = dto.Observaciones;

                _unidad.AporteMetaAhorroDALImpl.Update(aporteExistente);

                meta.MontoActual += dto.Monto;

                var porcentaje = meta.MontoObjetivo == 0 ? 0
                    : (meta.MontoActual / meta.MontoObjetivo) * 100m;

                if (porcentaje >= 100m && !meta.Completado)
                {
                    meta.Completado = true;
                    meta.UltimaNotificacion = DateTime.UtcNow;
                }
                else if (porcentaje >= 50m && meta.UltimaNotificacion == null)
                {
                    meta.UltimaNotificacion = DateTime.UtcNow;
                }
                else if (porcentaje < 50m)
                {
                    meta.UltimaNotificacion = null;
                    meta.Completado = false;
                }

                _unidad.AhorroDALImpl.UpdateAhorro(meta);

                _unidad.GuardarCambios();

                return Convertir(aporteExistente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar el aporte con ID {dto.AporteId}");
                throw;
            }
        }
    }
}
