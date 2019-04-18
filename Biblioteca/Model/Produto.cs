using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Livraria.Model
{
    public class Produto
    {
        public int Codigo { get; set; }
        public string Tipo { get; set; }
        public int Quantidade { get; set; }
        public string Nome { get; set; }
    }
}
