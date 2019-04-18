using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Livraria.Model
{
    public class Livro
    {
        public int Codigo { get; set; }
        public string Titulo { get; set; }
        public int QtdPagina { get; set; }
        public int Ano { get; set; }
        public string[] Autores { get; set; }
        public string Tema { get; set; }
    }
}
