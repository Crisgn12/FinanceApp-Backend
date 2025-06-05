using DAL.Interfaces;
using Entidades.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Implementaciones
{
    public class UnidadDeTrabajo: IUnidadDeTrabajo
    {

        private FinanceAppContext _context;

        public IUsuarioDAL UsuarioDAL { get; private set; }
        public ICategoriaDAL CategoriaDAL { get; private set; }
        public ITransaccionDAL TransaccionDAL { get; private set; }
        public IPagoProgramadoDAL PagoProgramadoDAL { get; private set; }

        public IAhorroDAL AhorroDALImpl { get; private set; }
        public IAporteMetaAhorroDAL AporteMetaAhorroDALImpl { get; private set; }


        public UnidadDeTrabajo(FinanceAppContext context, IUsuarioDAL usuarioDAL, IAhorroDAL ahorroDALImpl, IAporteMetaAhorroDAL aporteMetaAhorroDALImpl, ICategoriaDAL categoriaDAL, ITransaccionDAL transaccionDAL, IPagoProgramadoDAL pagoProgramadoDAL)
        {
            this._context = context;
            this.UsuarioDAL = usuarioDAL;
            this.AhorroDALImpl = ahorroDALImpl;
            this.AporteMetaAhorroDALImpl = aporteMetaAhorroDALImpl;
            this.CategoriaDAL = categoriaDAL;
            this.TransaccionDAL = transaccionDAL;
            PagoProgramadoDAL = pagoProgramadoDAL;

        }

        public bool GuardarCambios()
        {
            try
            {
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
