using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Livraria.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Livraria.Controllers
{
    [Route("api/carrinho")]
    [ApiController]
    public class CarrinhoComprasController : ControllerBase
    {
        private static readonly HttpClient client = new HttpClient();
        public CarrinhoComprasController()
        {
            client.BaseAddress = new System.Uri(@"https://localhost:5001/");
        }
        List<Carrinho> carrinho = new List<Carrinho>
        {
            new Carrinho
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
            new Carrinho
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
        };


        [HttpGet]
        [Route("itens/{id}")]
        public async Task<ActionResult<IEnumerable<string>>> RetornaItensDoCarrinho(int id)
        {
            return Ok(carrinho.FirstOrDefault(a => a.Codigo == id));
        }




        [HttpPost]
        [Route("itens/{id}")]
        public async Task<ActionResult<IEnumerable<string>>> InsereItemCarrinho(int id, [FromBody]Livro item)
        {
            
            
            var urlRequisicao = $"/api/ValidarDados/usuario?Codigo=1&Nome=Luiz";
            
            var resultado = await client.GetAsync(urlRequisicao);

            var itensCarriho  = carrinho.Where(a => a.Codigo == id).FirstOrDefault();
            itensCarriho.Livros.Add(item);
            return Ok();
        }

        
        [HttpPut]
        [Route("itens/{id}")]
        public ActionResult<IEnumerable<string>> AlteraItemCarrinho(int id, [FromBody]Carrinho item)
        {
            var cart = carrinho.Where(a => a.Codigo == item.Codigo).FirstOrDefault();
            carrinho.Remove(cart);
            carrinho.Add(item);
            return Ok();
        }


        [HttpDelete]
        [Route("itens/{id}")]
        public ActionResult<IEnumerable<string>> RemoveItemCarrinho(int id)
        {
            var cart = carrinho.Where(a => a.Codigo == id).FirstOrDefault();
            carrinho.Remove(cart);
            return Ok();
        }

    }
    class Usuario
    {
        public int Codigo { get; set; }
        public string Nome { get; set; }
    }
}