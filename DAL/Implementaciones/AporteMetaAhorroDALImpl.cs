using DAL.Interfaces;
using Entidades.Entities;

namespace DAL.Implementaciones
{
    public class AporteMetaAhorroDALImpl : DALGenericoImpl<AporteMetaAhorro>, IAporteMetaAhorroDAL
    {
        FinanceAppContext _context;

        public AporteMetaAhorroDALImpl(FinanceAppContext context) : base(context)
        {
            _context = context;
        }

        public bool Add(AporteMetaAhorro aporte)
        {
            aporte.Fecha = DateTime.UtcNow;

            _context.AporteMetaAhorros.Add(aporte);
            return _context.SaveChanges() > 0;
        }

        public void Remove(AporteMetaAhorro aporte)
        {
            _context.AporteMetaAhorros.Remove(aporte);
            _context.SaveChanges();
        }

        public AporteMetaAhorro FindById(int aporteId)
        {
            return _context.AporteMetaAhorros.Find(aporteId);
        }

        public List<AporteMetaAhorro> ObtenerPorMeta(int metaAhorroId)
        {
            return _context.AporteMetaAhorros
                           .Where(a => a.AhorroId == metaAhorroId)
                           .OrderBy(a => a.Fecha)
                           .ToList();
        }

        public decimal ObtenerTotalAportado(int ahorroId)
        {
            return _context.AporteMetaAhorros
                           .Where(a => a.AhorroId == ahorroId)
                           .Select(a => a.Monto)
                           .DefaultIfEmpty(0m)
                           .Sum();
        }

        public bool Update(AporteMetaAhorro aporte)
        {
            _context.AporteMetaAhorros.Update(aporte);
            return true;
        }
    }
}