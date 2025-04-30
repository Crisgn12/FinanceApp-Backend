using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IUnidadDeTrabajo: IDisposable
    {
        // Poner aqui los repositorios (Las clases DAL)
        IUsuarioDAL UsuarioDAL { get; }

        // Metodos

        bool GuardarCambios();
    }
}
