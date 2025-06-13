using DAL.Interfaces;
using Entidades.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DAL.Implementaciones
{
    public class EventoFinancieroDALImpl : DALGenericoImpl<EventoFinanciero>, IEventoFinancieroDAL
    {
        FinanceAppContext _context;

        public EventoFinancieroDALImpl(FinanceAppContext context) : base(context)
        {
            _context = context;
        }

        public EventoFinanciero CrearEventoFinanciero(EventoFinanciero evento)
        {
            evento.CreatedAt = DateTime.UtcNow;

            var query = "EXEC sp_CrearEventoFinanciero @Titulo, @Descripcion, @FechaInicio, @FechaFin, @EsTodoElDia, @Tipo, @Monto, @ColorFondo, @Frecuencia, @Repeticiones, @Activo, @UsuarioID, @RecurrenciaID";

            var parameters = new[]
            {
                new SqlParameter("@Titulo", evento.Titulo),
                new SqlParameter("@Descripcion", evento.Descripcion ?? (object)DBNull.Value),
                new SqlParameter("@FechaInicio", evento.FechaInicio),
                new SqlParameter("@FechaFin", evento.FechaFin),
                new SqlParameter("@EsTodoElDia", evento.EsTodoElDia),
                new SqlParameter("@Tipo", evento.Tipo),
                new SqlParameter("@Monto", evento.Monto ?? (object)DBNull.Value),
                new SqlParameter("@ColorFondo", evento.ColorFondo ?? (object)DBNull.Value),
                new SqlParameter("@Frecuencia", evento.Frecuencia ?? (object)DBNull.Value),
                new SqlParameter("@Repeticiones", evento.Repeticiones ?? (object)DBNull.Value),
                new SqlParameter("@Activo", evento.Activo),
                new SqlParameter("@UsuarioID", evento.UsuarioId),
                new SqlParameter("@RecurrenciaID", evento.RecurrenciaID ?? (object)DBNull.Value)
            };

            _context.Database.ExecuteSqlRaw(query, parameters);

            // Obtener el evento creado por el usuario y fecha más reciente
            var eventoCreado = _context.EventosFinancieros
                .Where(e => e.UsuarioId == evento.UsuarioId && e.Titulo == evento.Titulo)
                .OrderByDescending(e => e.CreatedAt)
                .FirstOrDefault();

            return eventoCreado ?? evento;
        }

        public EventoFinanciero ActualizarEventoFinanciero(EventoFinanciero evento)
        {
            evento.UpdatedAt = DateTime.UtcNow;

            var query = "EXEC sp_ActualizarEventoFinanciero @Id_Evento, @Titulo, @Descripcion, @FechaInicio, @FechaFin, @EsTodoElDia, @Tipo, @Monto, @ColorFondo, @Frecuencia, @Repeticiones, @Activo, @RecurrenciaID";

            var parameters = new[]
            {
                new SqlParameter("@Id_Evento", evento.IdEvento),
                new SqlParameter("@Titulo", evento.Titulo),
                new SqlParameter("@Descripcion", evento.Descripcion ?? (object)DBNull.Value),
                new SqlParameter("@FechaInicio", evento.FechaInicio),
                new SqlParameter("@FechaFin", evento.FechaFin),
                new SqlParameter("@EsTodoElDia", evento.EsTodoElDia),
                new SqlParameter("@Tipo", evento.Tipo),
                new SqlParameter("@Monto", evento.Monto ?? (object)DBNull.Value),
                new SqlParameter("@ColorFondo", evento.ColorFondo ?? (object)DBNull.Value),
                new SqlParameter("@Frecuencia", evento.Frecuencia ?? (object)DBNull.Value),
                new SqlParameter("@Repeticiones", evento.Repeticiones ?? (object)DBNull.Value),
                new SqlParameter("@Activo", evento.Activo),
                new SqlParameter("@RecurrenciaID", evento.RecurrenciaID ?? (object)DBNull.Value)
            };

            _context.Database.ExecuteSqlRaw(query, parameters);
            return evento;
        }

        public void EliminarEventoFinanciero(int idEvento)
        {
            var query = "EXEC sp_EliminarEventoFinanciero @Id_Evento";
            var parameters = new[]
            {
                new SqlParameter("@Id_Evento", idEvento)
            };

            _context.Database.ExecuteSqlRaw(query, parameters);
        }

        public void EliminarEventosPorRecurrencia(int recurrenciaId)
        {
            var query = "EXEC sp_EliminarEventosPorRecurrencia @RecurrenciaID";
            var parameters = new[]
            {
                new SqlParameter("@RecurrenciaID", recurrenciaId)
            };

            _context.Database.ExecuteSqlRaw(query, parameters);
        }

        public void ActualizarEventosPorRecurrencia(int recurrenciaId, string? colorFondo = null, decimal? monto = null, bool? activo = null)
        {
            var query = "EXEC sp_ActualizarEventosPorRecurrencia @RecurrenciaID, @ColorFondo, @Monto, @Activo";
            var parameters = new[]
            {
                new SqlParameter("@RecurrenciaID", recurrenciaId),
                new SqlParameter("@ColorFondo", colorFondo ?? (object)DBNull.Value),
                new SqlParameter("@Monto", monto ?? (object)DBNull.Value),
                new SqlParameter("@Activo", activo ?? (object)DBNull.Value)
            };

            _context.Database.ExecuteSqlRaw(query, parameters);
        }

        public List<EventoFinanciero> ListarEventosPorUsuarioYRango(string usuarioId, DateTime fechaInicio, DateTime fechaFin)
        {
            var query = "EXEC sp_ListarEventosPorUsuarioYRango @UsuarioID, @FechaInicio, @FechaFin";
            var parameters = new[]
            {
                new SqlParameter("@UsuarioID", usuarioId),
                new SqlParameter("@FechaInicio", fechaInicio),
                new SqlParameter("@FechaFin", fechaFin)
            };

            return _context.EventosFinancieros
                .FromSqlRaw(query, parameters)
                .ToList();
        }

        public EventoFinanciero GetEventoFinancieroById(int idEvento)
        {
            return _context.EventosFinancieros.Find(idEvento);
        }

        public List<EventoFinanciero> GetEventosPorUsuario(string usuarioId)
        {
            return _context.EventosFinancieros
                .Where(e => e.UsuarioId == usuarioId && e.Activo)
                .OrderBy(e => e.FechaInicio)
                .ToList();
        }
    }
}