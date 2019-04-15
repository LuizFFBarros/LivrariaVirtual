using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Livraria.Model
{
    public class Pedido
    {
        public int Codigo { get; set; }
        public Carrinho carrinho { get; set; }
        public DateTime DataCompra { get; set; }
        public string Status { get; set; }
    }
}
