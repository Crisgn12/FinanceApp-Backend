using Backend.Services.Interfaces;
using DAL.Implementaciones;
using DAL.Interfaces;
using Entidades.DTOs;
using Entidades.Entities;

namespace Backend.Services.Implementaciones
{
    public class TransaccionService : ITransaccionService
    {
        IUnidadDeTrabajo _unidadDeTrabajo;

        public TransaccionService(IUnidadDeTrabajo unidadDeTrabajo)
        {
            _unidadDeTrabajo = unidadDeTrabajo;
        }

        public async Task<bool> IngresarTransaccionAsync(TransaccionDTO transaccionDTO)
        {
            bool resultado = false;

            // Validaciones
            if (transaccionDTO.Monto <= 0)
            {
                throw new ArgumentException("El monto debe ser mayor que 0.");
            }
            if (transaccionDTO.Tipo != "Ingreso" && transaccionDTO.Tipo != "Gasto")
            {
                throw new ArgumentException("El tipo debe ser 'Ingreso' o 'Gasto'.");
            }
            if (transaccionDTO.Fecha == default)
            {
                throw new ArgumentException("La fecha es obligatoria.");
            }
            if (string.IsNullOrWhiteSpace(transaccionDTO.Titulo))
            {
                throw new ArgumentException("El título es obligatorio.");
            }

            transaccionDTO.TransaccionId = null;

            try
            {
                resultado = await _unidadDeTrabajo.TransaccionDAL.IngresarTransaccion(transaccionDTO);
                _unidadDeTrabajo.GuardarCambios();
                return resultado;
            }
            catch (Exception ex) when (ex.Message.Contains("no existe") || ex.Message.Contains("monto") || ex.Message.Contains("fecha") || ex.Message.Contains("tipo"))
            {
                throw new ArgumentException(ex.Message);
            }
        }

        public async Task<List<TransaccionDTO>> ObtenerTransaccionesPorUsuarioAsync(ObtenerTransaccionesPorUsuarioDTO req)
        {
            try
            {
                return await _unidadDeTrabajo.TransaccionDAL.ObtenerTransaccionesPorUsuario(req);
            }
            catch (Exception ex) when (ex.Message.Contains("no existe"))
            {
                throw new ArgumentException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener las transacciones: {ex.Message}");
            }
        }

        public async Task<TransaccionDTO> ObtenerDetalleTransaccionAsync(ObtenerDetalleTransaccionDTO req)
        {
            try
            {
                if (req.TransaccionId <= 0)
                {
                    throw new ArgumentException("El ID de la transacción es obligatorio.");
                }
                if (req.UsuarioId <= 0)
                {
                    throw new ArgumentException("El ID del usuario es obligatorio.");
                }

                return await _unidadDeTrabajo.TransaccionDAL.ObtenerDetalleTransaccion(req);
            }
            catch (Exception ex) when (ex.Message.Contains("no existe"))
            {
                throw new ArgumentException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el detalle de la transacción: {ex.Message}");
            }
        }

        public async Task<bool> ActualizarTransaccionAsync(TransaccionDTO transaccionDTO)
        {
            bool resultado = false;

            if (!transaccionDTO.TransaccionId.HasValue)
            {
                throw new ArgumentException("El ID de la transacción es obligatorio.");
            }
            if (transaccionDTO.Monto <= 0)
            {
                throw new ArgumentException("El monto debe ser mayor que 0.");
            }
            if (transaccionDTO.Tipo != "Ingreso" && transaccionDTO.Tipo != "Gasto")
            {
                throw new ArgumentException("El tipo debe ser 'Ingreso' o 'Gasto'.");
            }
            if (transaccionDTO.Fecha == default)
            {
                throw new ArgumentException("La fecha es obligatoria.");
            }
            if (string.IsNullOrWhiteSpace(transaccionDTO.Titulo))
            {
                throw new ArgumentException("El título es obligatorio.");
            }

            try
            {
                resultado = await _unidadDeTrabajo.TransaccionDAL.ActualizarTransaccion(transaccionDTO);

                _unidadDeTrabajo.GuardarCambios();

                return resultado;
            }
            catch (Exception ex) when (ex.Message.Contains("no existe") || ex.Message.Contains("monto") || ex.Message.Contains("fecha") || ex.Message.Contains("tipo"))
            {
                throw new ArgumentException(ex.Message);
            }
        }

        public async Task<bool> EliminarTransaccionAsync(EliminarTransaccionDTO req)
        {
            try
            {
                bool resultado = false;

                if (req.TransaccionID <= 0)
                {
                    throw new ArgumentException("El ID de la transacción es obligatorio.");
                }
                if (req.UsuarioID <= 0)
                {
                    throw new ArgumentException("El ID del usuario es obligatorio.");
                }

                resultado = await _unidadDeTrabajo.TransaccionDAL.EliminarTransaccion(req);

                _unidadDeTrabajo.GuardarCambios();

                return resultado;
            }
            catch (Exception ex) when (ex.Message.Contains("no existe"))
            {
                throw new ArgumentException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al eliminar la transacción: {ex.Message}");
            }
        }
    }
}
