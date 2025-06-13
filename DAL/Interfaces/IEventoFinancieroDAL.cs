using Entidades.Entities;

namespace DAL.Interfaces
{
    public interface IEventosFinancieroDAL : IDALGenerico<EventosFinanciero>
    {
        EventosFinanciero CrearEventosFinanciero(EventosFinanciero evento);
        EventosFinanciero ActualizarEventosFinanciero(EventosFinanciero evento);
        void EliminarEventosFinanciero(int idEvento);
        void EliminarEventosPorRecurrencia(int recurrenciaId);
        void ActualizarEventosPorRecurrencia(int recurrenciaId, string? colorFondo = null, decimal? monto = null, bool? activo = null);
        List<EventosFinanciero> ListarEventosPorUsuarioYRango(string usuarioId, DateTime fechaInicio, DateTime fechaFin);
        EventosFinanciero GetEventosFinancieroById(int idEvento);
        List<EventosFinanciero> GetEventosPorUsuario(string usuarioId);
    }
}
