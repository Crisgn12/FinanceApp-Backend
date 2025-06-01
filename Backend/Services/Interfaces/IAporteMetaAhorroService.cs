using Backend.DTO;

namespace Backend.Services.Interfaces
{
    public interface IAporteMetaAhorroService
    {
        AporteMetaAhorroDTO AddAporte(AporteMetaAhorroDTO aporte);
        List<AporteMetaAhorroDTO> GetAportesByMeta(int metaAhorroId);
        bool DeleteAporte(int aporteId);
        AporteMetaAhorroDTO GetAporteById(int aporteId);
        AporteMetaAhorroDTO UpdateAporte(AporteMetaAhorroDTO aporte);
    }
}
