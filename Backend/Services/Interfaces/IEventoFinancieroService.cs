using Backend.DTO;

namespace Backend.Services.Interfaces
{
    public interface IEventosFinancieroService
    {
        EventosFinancieroDTO CrearEventosFinanciero(EventosFinancieroDTO evento);
        EventosFinancieroDTO ActualizarEventosFinanciero(EventosFinancieroDTO evento);
        EventosFinancieroDTO EliminarEventosFinanciero(int idEvento);
        void EliminarEventosPorRecurrencia(int RecurrenciaId);
        void ActualizarEventosPorRecurrencia(ActualizarEventosPorRecurrenciaDTO dto);
        List<EventosFinancieroDTO> ListarEventosPorUsuarioYRango(DateTime fechaInicio, DateTime fechaFin);
        EventosFinancieroDTO GetEventosFinancieroById(int idEvento);
        List<EventosFinancieroDTO> GetEventosPorUsuario();
    }
}