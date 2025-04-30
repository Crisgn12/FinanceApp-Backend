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


        public UnidadDeTrabajo(FinanceAppContext context, IUsuarioDAL usuarioDAL)
        {
            this._context = context;
            this.UsuarioDAL = usuarioDAL;
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
