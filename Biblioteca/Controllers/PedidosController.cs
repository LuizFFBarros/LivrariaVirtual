using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Livraria.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Livraria.Controllers
{
    [Route("api/pedido")]
    [ApiController]
    public class PedidosController : ControllerBase
    {

        List<Pedido> pedido = new List<Pedido>
        {
            new Pedido
            {
                Codigo = 1,
                carrinho =  new Carrinho
                {
                    Codigo = 1,
                    Livros = new List<Livro>
                    {
                        new Livro
                        {
                            codigo = 1,
                            Autores = new string[]
                            {
                                "Autor A",
                                "Autor B"
                            },
                            Ano = 2000,
                            QtdPagina = 150,
                            Titulo = "Titulo 1",
                            Tema = "tema 1"
                        },
                    }
                },
                DataCompra = Convert.ToDateTime("10/10/2010"),
                Status = "Em Processamento"
            },
            new Pedido
            {
                Codigo = 2,
                DataCompra = Convert.ToDateTime("5/5/2011"),
                Status = "Aguardadndo pagamento",
                carrinho = new Carrinho
                {
                    Codigo = 2,
                    Livros = new List<Livro>
                    {
                        new Livro
                        {
                            codigo =1,
                            Autores = new string[]
                            {
                                "Autor B"
                            },
                            Ano = 2006,
                            QtdPagina = 200,
                            Titulo = "Titulo 3",
                            Tema = "tema 3"
                        },
                        new Livro
                        {
                            codigo =1,
                            Autores = new string[]
                            {
                                "Autor C",
                                "Autor A"
                            },
                            Ano = 2000,
                            QtdPagina = 10,
                            Titulo =
                            "Titulo 4",
                            Tema = "tema 4"
                        },
                    }
                }
            }
        };

        
        [HttpPost]
        [Route("pedido/{id}")]
        public ActionResult<IEnumerable<string>> CadastraItemPedido(int id, [FromBody]Pedido item)
        {
            pedido.Add(item);
            return Ok();
        }

        
        [HttpPut]
        [Route("pedido/{id}")]
        public ActionResult<IEnumerable<string>> AlteraItemPedido(int id, [FromBody]Pedido item)
        {
            var dado = pedido.Where(a => a.Codigo == id).FirstOrDefault();
            pedido.Remove(dado);
            pedido.Add(item);
            return Ok();
        }

        
        [HttpDelete]
        [Route("pedido/{id}")]
        public ActionResult<IEnumerable<string>> RemoveItemPedido(int id)
        {
            var dado = pedido.Where(a => a.Codigo == id).FirstOrDefault();
            pedido.Remove(dado);
            return Ok();
        }

        
        [HttpGet]
        [Route("pedido/{id}")]
        public ActionResult<IEnumerable<string>> BuscarPedidos(int id)
        {
            return Ok( pedido.Where(a => a.Codigo == id).FirstOrDefault());
        }

        [HttpGet]
        [Route("pedido/{id}/item")]
        public ActionResult<IEnumerable<string>> BuscarItensPedidos(int id)
        {
            return Ok(pedido.Where(a => a.Codigo == id).Select(a => a.carrinho).ToList());
        }
    }
}