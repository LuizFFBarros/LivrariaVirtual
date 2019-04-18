using Livraria.Extension;
using Livraria.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Livraria.Controllers
{
    [Route("api/pedido")]
    [ApiController]
    public class PedidosController : ControllerBase
    {
        private static readonly HttpClient client = new HttpClient();
        public PedidosController()
        {
           
        }
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
            }
        };


        [HttpPost]
        [Route("pedido/")]
        public async Task<ActionResult<IEnumerable<string>>> FinalizarPedido([FromBody]FInalizarPedidoEntrada dadosEntrada)
        {
            //autenticar usuario
            client.BaseAddress = new System.Uri(@"https://localhost:5001/");
            var dadosChaveValorUsuario = dadosEntrada.Usuario.ToKeyValue();
            var urlEncoded = new FormUrlEncodedContent(dadosChaveValorUsuario);
            var urlString = await urlEncoded.ReadAsStringAsync();

            var urlRequisicao = $"/api/ValidarDados/usuario?{urlString}";

            var resultado = await client.GetAsync(urlRequisicao);
            if (resultado.StatusCode != HttpStatusCode.OK)
            {
                return NotFound("Dados de Usuario não encontrados.");
            }


            //chamar auditoria
            client.BaseAddress = new System.Uri(@"https://localhost:5010/");
            var dadosChaveValorAuditoria = dadosEntrada.Usuario.ToKeyValue();
            var urlEncodedAuditoria = new FormUrlEncodedContent(dadosChaveValorAuditoria);
            var urlStringAuditoria = await urlEncodedAuditoria.ReadAsStringAsync();

            //verifica estoque

            var urlRequisicaoAuditoria = $"/api/auditoria/produto?{dadosEntrada.Pedido.Codigo}";
            var resultadoAuditoria = await client.GetAsync(urlRequisicaoAuditoria);

            if (resultadoAuditoria.StatusCode != HttpStatusCode.OK)
                return BadRequest("Sem Estoque");

            //altera estoque
            urlRequisicaoAuditoria = $"/api/auditoria/produto?{dadosEntrada.Pedido.Codigo}&{urlStringAuditoria}";
            resultadoAuditoria = await client.PutAsync(urlRequisicaoAuditoria, urlEncodedAuditoria);



            //chamar transacao credito
            client.BaseAddress = new System.Uri(@"https://localhost:5003/");
            var dadosChaveValorCredito = dadosEntrada.Usuario.ToKeyValue();
            var urlEncodedCredito = new FormUrlEncodedContent(dadosChaveValorCredito);
            var urlStringCredito = await urlEncodedCredito.ReadAsStringAsync();

            var urlRequisicaoCredito = $"/api/CartaoCredito/cartao/compra";
            var resultadoCredito = await client.PostAsync(urlRequisicaoCredito, urlEncodedCredito);

            if(resultadoCredito.StatusCode != HttpStatusCode.OK)
                return BadRequest("Erro ao efetuar transacao de credito.");


            return Ok();
        }

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
            return Ok(pedido.Where(a => a.Codigo == id).FirstOrDefault());
        }

        [HttpGet]
        [Route("pedido/{id}/item")]
        public ActionResult<IEnumerable<string>> BuscarItensPedidos(int id)
        {
            return Ok(pedido.Where(a => a.Codigo == id).Select(a => a.carrinho).ToList());
        }
    }
    public class FInalizarPedidoEntrada
    {
        public Usuario Usuario { get; set; }
        public Pedido Pedido { get; set; }
        public Cartao Cartao { get; set; }
    }
    public class Cartao
    {
        public int Numerocartao { get; set; }
        public int CodigoVerificador { get; set; }
    }

}