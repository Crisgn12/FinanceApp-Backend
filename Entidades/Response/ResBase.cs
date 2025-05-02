using Entidades.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades.Response
{
    public class ResBase
    {
        public bool resultado { get; set; }
        public List<Error> error { get; set; }
    }
}
