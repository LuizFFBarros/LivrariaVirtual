using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Livraria.Extension;
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
                        Codigo = 1,
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
                        Codigo =1,
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
                        Codigo =1,
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
        public ActionResult<IEnumerable<string>> RetornaItensDoCarrinho(int id)
        {
            return Ok(carrinho.FirstOrDefault(a => a.Codigo == id));
        }




        [HttpPost]
        [Route("itens")]
        public async Task<ActionResult<IEnumerable<string>>> InsereItemCarrinho(int id, [FromBody]DadosEntradaInsercaoExclusaoCarrinho dadosEntrada)
        {

            var dadosChaveValor = dadosEntrada.Usuario.ToKeyValue();
            var urlEncoded = new FormUrlEncodedContent(dadosChaveValor);
            var urlString = await urlEncoded.ReadAsStringAsync();

            var urlRequisicao = $"/api/ValidarDados/usuario?{urlString}";
            
            var resultado = await client.GetAsync(urlRequisicao);
            if (resultado.StatusCode != System.Net.HttpStatusCode.OK)
                return NotFound("Dados de Usuario não encontrados.");

            var itensCarriho  = carrinho.Where(a => a.Codigo == id).FirstOrDefault();
            if (itensCarriho.Livros.Where(a => a.Codigo == dadosEntrada.Livro.Codigo).Any())
                return BadRequest("Item ja existente no carrinho");

            itensCarriho.Livros.Add(dadosEntrada.Livro);
            return Ok("Item inserido com sucesso.");
        }

        
        [HttpPut]
        [Route("itens/{id}")]
        public async Task<ActionResult<IEnumerable<string>>> AlteraItemCarrinho(int id, [FromBody]DadosEntradaAlteracaoCarrinho dadosEntrada )
        {

            var dadosChaveValor = dadosEntrada.Usuario.ToKeyValue();
            var urlEncoded = new FormUrlEncodedContent(dadosChaveValor);
            var urlString = await urlEncoded.ReadAsStringAsync();

            var urlRequisicao = $"/api/ValidarDados/usuario?{urlString}";

            var resultado = await client.GetAsync(urlRequisicao);
            if (resultado.StatusCode != System.Net.HttpStatusCode.OK)
                return NotFound("Dados de Usuario não encontrados.");

            var cart = carrinho.Where(a => a.Codigo == dadosEntrada.Carrinho.Codigo).FirstOrDefault();
            carrinho.Remove(cart);
            carrinho.Add(dadosEntrada.Carrinho);
            return Ok();
        }


        [HttpDelete]
        [Route("itens/{id}")]
        public async Task<ActionResult<IEnumerable<string>>> RemoveItemCarrinho(int id,[FromQuery]DadosEntradaInsercaoExclusaoCarrinho dadosEntrada)
        {
            var dadosChaveValor = dadosEntrada.Usuario.ToKeyValue();
            var urlEncoded = new FormUrlEncodedContent(dadosChaveValor);
            var urlString = await urlEncoded.ReadAsStringAsync();

            var urlRequisicao = $"/api/ValidarDados/usuario?{urlString}";

            var resultado = await client.GetAsync(urlRequisicao);
            if (resultado.StatusCode != System.Net.HttpStatusCode.OK)
                return NotFound("Dados de Usuario não encontrados.");

            var cart = carrinho.Where(a => a.Codigo == id).FirstOrDefault();
            carrinho.Remove(cart);
            return Ok();
        }

    }
    public class DadosEntradaInsercaoExclusaoCarrinho
    {
        public Usuario Usuario { get; set; }
        public Livro Livro { get; set; }

    }
    public class DadosEntradaAlteracaoCarrinho
    {
        public Usuario Usuario { get; set; }
        public Carrinho Carrinho { get; set; }
    }



}