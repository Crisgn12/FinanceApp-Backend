using DAL.Interfaces;
using Entidades.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DAL.Implementaciones
{
    public class AhorroDALImpl : DALGenericoImpl<Ahorro>, IAhorroDAL
    {
        FinanceAppContext _context;

        public AhorroDALImpl(FinanceAppContext context) : base(context)
        {
            _context = context;
        }

        public Ahorro AddAhorro(Ahorro ahorro)
        {
            ahorro.CreatedAt = DateTime.UtcNow;
            ahorro.MontoActual = 0m;
            ahorro.Completado = false;
            ahorro.UltimaNotificacion = null;

            var query = "EXEC SP_ADD_AHORRO @UsuarioId, @Nombre, @Monto_Objetivo, @Monto_Actual, @Completado, @Fecha_Meta, @IdAhorro OUT";

            var parameters = new[]
            {
                new SqlParameter("@UsuarioId", ahorro.UsuarioId),
                new SqlParameter("@Nombre", ahorro.Nombre),
                new SqlParameter("@Monto_Objetivo", ahorro.MontoObjetivo),
                new SqlParameter("@Monto_Actual", ahorro.MontoActual),
                new SqlParameter("@Completado", ahorro.Completado),
                new SqlParameter("@Fecha_Meta", ahorro.FechaMeta ?? (object)DBNull.Value),
                new SqlParameter("@IdAhorro", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };

            _context.Database.ExecuteSqlRaw(query, parameters);

            ahorro.AhorroId = (int)parameters[6].Value;
            return ahorro;
        }

        public Ahorro UpdateAhorro(Ahorro ahorro)
        {
            ahorro.UpdatedAt = DateTime.UtcNow;

            var query = "EXEC SP_UPDATE_AHORRO @AhorroID, @Nombre, @Monto_Objetivo, @Completado, @Fecha_Meta, @Ultima_Notificacion";

            var parameters = new[]
            {
            new SqlParameter("@AhorroID", ahorro.AhorroId),
            new SqlParameter("@Nombre", ahorro.Nombre),
            new SqlParameter("@Monto_Objetivo", ahorro.MontoObjetivo),
            new SqlParameter("@Completado", ahorro.Completado),
            new SqlParameter("@Fecha_Meta", ahorro.FechaMeta ?? (object)DBNull.Value),
            new SqlParameter("@Ultima_Notificacion", ahorro.UltimaNotificacion ?? (object)DBNull.Value)
        };
            _context.Database.ExecuteSqlRaw(query, parameters);

            return ahorro;
        }

        public void DeleteAhorro(int ahorroId)
        {
            var ahorro = _context.Ahorros.Find(ahorroId);
            if (ahorro != null)
            {
                _context.Ahorros.Remove(ahorro);
                _context.SaveChanges();
            }
        }

        public Ahorro GetAhorroById(int ahorroId)
        {
            return _context.Ahorros.Find(ahorroId);
        }

        public List<Ahorro> GetAhorros(int usuarioId)
        {
            return _context.Ahorros.Where(a => a.UsuarioId == usuarioId).ToList();
        }

        public void ActualizarMontoActual(int ahorroId)
        {
            using var cmd = _context.Database.GetDbConnection().CreateCommand();
            cmd.CommandText = "SP_ACTUALIZAR_MONTO_ACTUAL";
            cmd.CommandType = CommandType.StoredProcedure;

            var param = cmd.CreateParameter();
            param.ParameterName = "@AhorroID";
            param.Value = ahorroId;
            cmd.Parameters.Add(param);

            _context.Database.OpenConnection();
            cmd.ExecuteNonQuery();
            _context.Database.CloseConnection();
        }
    }
}