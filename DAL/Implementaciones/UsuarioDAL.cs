using DAL.Interfaces;
using Entidades.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Implementaciones
{
    public class UsuarioDAL: DALGenericoImpl<Usuario>, IUsuarioDAL
    {
        FinanceAppContext _context;

        public UsuarioDAL(FinanceAppContext context) : base(context)
        {
            _context = context;
        }
    }
}
