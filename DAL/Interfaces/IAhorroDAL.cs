using Entidades.Entities;

namespace DAL.Interfaces
{
    public interface IAhorroDAL : IDALGenerico<Ahorro>
    {
        Ahorro AddAhorro(Ahorro ahorro);
        Ahorro UpdateAhorro(Ahorro ahorro);
        void DeleteAhorro(int ahorroId);
        Ahorro GetAhorroById(int ahorroId);
        List<Ahorro> GetAhorros(int usuarioId);
        void ActualizarMontoActual(int ahorroId);
    }
}
