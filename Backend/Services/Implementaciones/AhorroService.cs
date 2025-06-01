using Backend.DTO;
using Backend.Services.Interfaces;
using DAL.Interfaces;
using Entidades.Entities;

namespace Backend.Services.Implementaciones
{
    public class AhorroService : IAhorroService
    {
        ILogger<AhorroService> _logger;
        IUnidadDeTrabajo _unidadDeTrabajo;

        public AhorroService(IUnidadDeTrabajo unidad, ILogger<AhorroService> logger)
        {
            _unidadDeTrabajo = unidad;
            _logger = logger;
        }

        private AhorroDTO Convertir(Ahorro ahorro)
        {
            return new AhorroDTO
            {
                AhorroID = ahorro.AhorroId,
                UsuarioID = ahorro.UsuarioId,
                Nombre = ahorro.Nombre,
                Monto_Objetivo = ahorro.MontoObjetivo,
                Monto_Actual = ahorro.MontoActual,
                Fecha_Meta = ahorro.FechaMeta,
                Completado = ahorro.Completado,
                PorcentajeAvance = ahorro.MontoObjetivo == 0 ? 0 : (ahorro.MontoActual / ahorro.MontoObjetivo) * 100
            };
        }

        private Ahorro Convertir(AhorroDTO dto)
        {
            return new Ahorro
            {
                AhorroId = dto.AhorroID ?? 0,
                UsuarioId = dto.UsuarioID,
                Nombre = dto.Nombre,
                MontoObjetivo = dto.Monto_Objetivo,
                MontoActual = dto.Monto_Actual,
                FechaMeta = dto.Fecha_Meta,
                Completado = dto.Completado,
                CreatedAt = DateTime.UtcNow
            };
        }

        public AhorroDTO AddAhorro(AhorroDTO ahorro)
        {
            try
            {
                _logger.LogError("Ingresa a AddAhorro");

                var entity = _unidadDeTrabajo.AhorroDALImpl.AddAhorro(Convertir(ahorro));
                _unidadDeTrabajo.GuardarCambios();
                return Convertir(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar ahorro");
                throw;
            }
        }

        public AhorroDTO UpdateAhorro(AhorroDTO ahorro)
        {
            try
            {
                _logger.LogError("Ingresa a UpdateAhorro");

                var entity = Convertir(ahorro);
                entity.UpdatedAt = DateTime.UtcNow;

                VerificarProgreso(entity);

                _unidadDeTrabajo.AhorroDALImpl.UpdateAhorro(entity);
                _unidadDeTrabajo.GuardarCambios();
                return ahorro;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar ahorro");
                throw;
            }
        }

        public AhorroDTO DeleteAhorro(int id)
        {
            try
            {
                _logger.LogError("Ingresa a DeleteAhorro");

                var entidadEnBd = _unidadDeTrabajo.AhorroDALImpl.FindById(id);
                if (entidadEnBd == null)
                {
                    _logger.LogWarning($"Ahorro con ID {id} no encontrado");
                    return null;
                }

                _unidadDeTrabajo.AhorroDALImpl.DeleteAhorro(id);
                _unidadDeTrabajo.GuardarCambios();
                return Convertir(entidadEnBd);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar ahorro");
                throw;
            }
        }

        public AhorroDTO GetAhorroById(int id)
        {
            try
            {
                _logger.LogError("Ingresa a GetAhorroById");

                var ahorro = _unidadDeTrabajo.AhorroDALImpl.FindById(id);
                if (ahorro == null)
                {
                    _logger.LogWarning($"Ahorro con ID {id} no encontrado");
                    return null;
                }

                return Convertir(ahorro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener ahorro por ID");
                throw;
            }
        }

        public List<AhorroDTO> GetAhorros(int usuarioId)
        {
            try
            {
                _logger.LogError("Ingresa a GetAhorrosByUsuarioId");

                var ahorros = _unidadDeTrabajo.AhorroDALImpl.GetAhorros(usuarioId);
                var dtoList = ahorros.Select(a => Convertir(a)).ToList();
                return dtoList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener ahorros por ID de usuario");
                throw;
            }
        }

        private void VerificarProgreso(Ahorro ahorro)
        {
            var porcentaje = ahorro.MontoObjetivo == 0 ? 0 : (ahorro.MontoActual / ahorro.MontoObjetivo) * 100;
            if (porcentaje >= 100 && !ahorro.Completado)
            {
                ahorro.Completado = true;
                _logger.LogInformation($"Ahorro '{ahorro.Nombre}' completado!");
            }
            else if (porcentaje >= 50 && ahorro.UltimaNotificacion == null)
            {
                ahorro.UltimaNotificacion = DateTime.UtcNow;
                _logger.LogInformation($"Meta '{ahorro.Nombre}' llegó al 50%. ¡Buen trabajo!");
            }
        }

        public AhorroDTO ObtenerMetaConDetalle(int ahorroId)
        {
            try
            {
                _logger.LogError("Ingresa a ObtenerMetaConDetalle");

                var ahorro = _unidadDeTrabajo.AhorroDALImpl.FindById(ahorroId);
                if (ahorro == null)
                {
                    _logger.LogWarning($"Ahorro con ID {ahorroId} no encontrado");
                    return null;
                }

                var aportes = _unidadDeTrabajo.AporteMetaAhorroDALImpl
                                .ObtenerPorMeta(ahorroId)
                                .OrderByDescending(a => a.Fecha)
                                .ToList();

                var dto = Convertir(ahorro);
                dto.HistorialAportes = aportes.Select(a => new AporteMetaAhorroDTO
                {
                    Fecha = a.Fecha,
                    Monto = a.Monto,
                    Observaciones = a.Observaciones
                }).ToList();

                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener detalle de meta de ahorro");
                throw;
            }
        }

        public List<AhorroDTO> GetNotificaciones(int usuarioId)
        {
            var todas = _unidadDeTrabajo.AhorroDALImpl.GetAhorros(usuarioId);
            var notis = todas
                .Where(a => a.UltimaNotificacion.HasValue)
                .Select(a => Convertir(a))
                .ToList();
            return notis;
        }
    }
}