using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Livraria.Model
{
    public class Carrinho
    {
        public int Codigo { get; set; }
        public List<Livro> Livros { get; set; }
        
    }
}
