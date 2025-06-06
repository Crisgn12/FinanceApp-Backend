using Backend.DTO;
using Entidades.Entities;

namespace Backend.Services.Interfaces
{
    public interface IAhorroService
    {
        List<AhorroDTO> GetAhorros();
        AhorroDTO GetAhorroById(int ahorroId);
        AhorroDTO AddAhorro(AhorroDTO ahorro);
        AhorroDTO UpdateAhorro(AhorroDTO ahorro);
        AhorroDTO DeleteAhorro(int ahorroId);
        AhorroDTO ObtenerMetaConDetalle(int ahorroId);
        List<AhorroDTO> GetNotificaciones();
    }
}
