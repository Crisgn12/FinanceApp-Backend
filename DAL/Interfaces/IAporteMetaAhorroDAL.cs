using Entidades.Entities;

namespace DAL.Interfaces
{
    public interface IAporteMetaAhorroDAL : IDALGenerico<AporteMetaAhorro>
    {
        List<AporteMetaAhorro> ObtenerPorMeta(int metaAhorroId);
        decimal ObtenerTotalAportado(int metaAhorroId);
    }
}
