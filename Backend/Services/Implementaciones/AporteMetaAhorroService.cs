using Backend.DTO;
using Backend.Services.Interfaces;
using DAL.Interfaces;
using Entidades.Entities;

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
                MetaAhorroId = aporte.AhorroId,
                Fecha = aporte.Fecha,
                Monto = aporte.Monto,
                Observaciones = aporte.Observaciones
            };
        }

        private AporteMetaAhorro Convertir(AporteMetaAhorroDTO aporte)
        {
            return new AporteMetaAhorro
            {
                AporteId = aporte.AporteId ?? 0,
                AhorroId = aporte.MetaAhorroId,
                Fecha = aporte.Fecha,
                Monto = aporte.Monto,
                Observaciones = aporte.Observaciones
            };
        }

        public AporteMetaAhorroDTO AddAporte(AporteMetaAhorroDTO aporte)
        {
            try
            {
                if (aporte == null || aporte.Monto <= 0)
                {
                    _logger.LogWarning("Aporte inválido");
                    return null;
                }

                if (aporte.Fecha == default)
                {
                    aporte.Fecha = DateTime.UtcNow;
                }

                var entity = Convertir(aporte);
                var agregado = _unidad.AporteMetaAhorroDALImpl.Add(entity);
                if (!agregado) return null;

                _unidad.GuardarCambios();

                // 1) Recuperar la meta padre
                var meta = _unidad.AhorroDALImpl.FindById(entity.AhorroId);
                if (meta != null)
                {
                    meta.MontoActual += entity.Monto;
                    // 2) Verificar si alcanzó hito. Por ejemplo:
                    var porcentaje = meta.MontoObjetivo == 0 ? 0 : (meta.MontoActual / meta.MontoObjetivo) * 100;
                    if (porcentaje >= 100 && !meta.Completado)
                    {
                        meta.Completado = true;
                        meta.UltimaNotificacion = DateTime.UtcNow;
                    }
                    else if (porcentaje >= 50 && meta.UltimaNotificacion == null)
                    {
                        meta.UltimaNotificacion = DateTime.UtcNow;
                    }
                    // 3) Guardar la meta actualizada
                    _unidad.AhorroDALImpl.UpdateAhorro(meta);
                    _unidad.GuardarCambios();
                }


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

                // Recuperar la meta asociada
                var meta = _unidad.AhorroDALImpl.FindById(aporte.AhorroId);
                if (meta == null)
                {
                    _logger.LogWarning($"No se encontró la meta asociada con ID {aporte.AhorroId}");
                    return false;
                }

                // Revertir el monto
                meta.MontoActual -= aporte.Monto;
                if (meta.MontoActual < 0)
                    meta.MontoActual = 0;

                // Recalcular porcentaje
                var porcentaje = meta.MontoObjetivo == 0 ? 0 : (meta.MontoActual / meta.MontoObjetivo) * 100;

                // Actualizar estado de completado
                meta.Completado = porcentaje >= 100;

                // Si ya no alcanza el 50%, limpiar notificación (opcional)
                if (porcentaje < 50)
                {
                    meta.UltimaNotificacion = null;
                }

                // Guardar cambios en meta
                _unidad.AhorroDALImpl.UpdateAhorro(meta);
                // Eliminar el aporte
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

        private void RecalcularMontoActual(int metaAhorroId)
        {
            var nuevoMonto = _unidad.AporteMetaAhorroDALImpl.ObtenerTotalAportado(metaAhorroId);
            var meta = _unidad.AhorroDALImpl.FindById(metaAhorroId);
            if (meta != null)
            {
                meta.MontoActual = nuevoMonto;
                meta.Completado = meta.MontoObjetivo > 0 && meta.MontoActual >= meta.MontoObjetivo;

                _unidad.AhorroDALImpl.UpdateAhorro(meta);
                _unidad.GuardarCambios();
            }
        }
    }
}