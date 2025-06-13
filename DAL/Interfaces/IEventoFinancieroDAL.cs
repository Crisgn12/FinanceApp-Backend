using Entidades.Entities;

namespace DAL.Interfaces
{
    public interface IEventoFinancieroDAL : IDALGenerico<EventoFinanciero>
    {
        EventoFinanciero CrearEventoFinanciero(EventoFinanciero evento);
        EventoFinanciero ActualizarEventoFinanciero(EventoFinanciero evento);
        void EliminarEventoFinanciero(int idEvento);
        void EliminarEventosPorRecurrencia(int recurrenciaId);
        void ActualizarEventosPorRecurrencia(int recurrenciaId, string? colorFondo = null, decimal? monto = null, bool? activo = null);
        List<EventoFinanciero> ListarEventosPorUsuarioYRango(string usuarioId, DateTime fechaInicio, DateTime fechaFin);
        EventoFinanciero GetEventoFinancieroById(int idEvento);
        List<EventoFinanciero> GetEventosPorUsuario(string usuarioId);
    }
}
