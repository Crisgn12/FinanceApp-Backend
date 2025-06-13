using Backend.DTO;

namespace Backend.Services.Interfaces
{
    public interface IEventoFinancieroService
    {
        EventoFinancieroDTO CrearEventoFinanciero(EventoFinancieroDTO evento);
        EventoFinancieroDTO ActualizarEventoFinanciero(EventoFinancieroDTO evento);
        EventoFinancieroDTO EliminarEventoFinanciero(int idEvento);
        void EliminarEventosPorRecurrencia(int recurrenciaId);
        void ActualizarEventosPorRecurrencia(ActualizarEventosPorRecurrenciaDTO dto);
        List<EventoFinancieroDTO> ListarEventosPorUsuarioYRango(DateTime fechaInicio, DateTime fechaFin);
        EventoFinancieroDTO GetEventoFinancieroById(int idEvento);
        List<EventoFinancieroDTO> GetEventosPorUsuario();
    }
}