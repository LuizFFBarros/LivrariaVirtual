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
    [Route("v1/api/pedidos")]
    [ApiController]
    public class PedidosController : ControllerBase
    {
        private HttpClient client;
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
        [Route("Pagamentos")]
        public async Task<ActionResult<IEnumerable<string>>> FinalizarPedido([FromBody]FInalizarPedidoEntrada dadosEntrada)
        {
            //autenticar usuario
            client = new HttpClient();
            client.BaseAddress = new System.Uri(@"https://localhost:5005/");
            var dadosChaveValorUsuario = dadosEntrada.Usuario.ToKeyValue();
            var urlEncoded = new FormUrlEncodedContent(dadosChaveValorUsuario);
            var urlString = await urlEncoded.ReadAsStringAsync();

            var urlRequisicao = $"/api/ValidarDados/usuarios/{dadosEntrada.Usuario.Codigo}";

            var resultado = await client.GetAsync(urlRequisicao);
            if (resultado.StatusCode != HttpStatusCode.OK)
            {
                return NotFound("Dados de Usuario não encontrados.");
            }

            client.Dispose();
            client = new HttpClient();


            //chamar auditoria - verificar estoque
            var codigosProduto = dadosEntrada.Pedido.carrinho.Livros.GroupBy(a => a.Codigo).Select(a => new { codigoProduto = a.Key, quantidade = a.Count() }).ToArray();
            var listaProdutoSemEstoque = new List<string>();
            for (int i = 0; i < codigosProduto.Length; i++)
            {
                client.Dispose();
                client = new HttpClient();
                client.BaseAddress = new System.Uri(@"https://localhost:5001/");
                var dadosChaveValorPedido = codigosProduto[i].ToKeyValue();
                var urlEncodedAuditoria = new FormUrlEncodedContent(dadosChaveValorPedido);
                var urlStringPedido = await urlEncodedAuditoria.ReadAsStringAsync();

                var urlRequisicaoAuditoria = $"/api/auditoria/produtos/{codigosProduto[i].codigoProduto}?{urlStringPedido}";
                var resultadoAuditoria = await client.GetAsync(urlRequisicaoAuditoria);
                if (resultadoAuditoria.StatusCode != HttpStatusCode.OK)
                    listaProdutoSemEstoque.Add($"O produto {dadosEntrada.Pedido.carrinho.Livros.Where(a => a.Codigo == codigosProduto[i].codigoProduto).Select(a => a.Titulo)} não possui estoque.");
            }

            if(listaProdutoSemEstoque.Any())
                return BadRequest(string.Join("\n", listaProdutoSemEstoque));



            //chamar auditoria - altera estoque

            for (int i = 0; i < codigosProduto.Length; i++)
            {
                client.Dispose();
                client = new HttpClient();
                client.BaseAddress = new System.Uri(@"https://localhost:5001/");
                var dadosChaveValorPedido = codigosProduto[i].ToKeyValue();
                var urlEncodedAuditoria = new FormUrlEncodedContent(dadosChaveValorPedido);
                var urlStringPedido = await urlEncodedAuditoria.ReadAsStringAsync();
                var urlRequisicaoAuditoria = $"/api/auditoria/produtos/{codigosProduto[i].codigoProduto}?{urlStringPedido}";
                var resultadoAuditoria = await client.PutAsync(urlRequisicaoAuditoria, urlEncodedAuditoria);
                if (resultadoAuditoria.StatusCode != HttpStatusCode.OK)
                    return BadRequest("Algo deu errado ao alterar o estoque");
            }

            

            client.Dispose();
            client = new HttpClient();

            //chamar transacao credito
            client.BaseAddress = new System.Uri(@"https://localhost:5003/");
            var dadosChaveValorCredito = dadosEntrada.Cartao.ToKeyValue();
            var urlEncodedCredito = new FormUrlEncodedContent(dadosChaveValorCredito);
            var urlStringCredito = await urlEncodedCredito.ReadAsStringAsync();

            var urlRequisicaoCredito = $"/api/CartaoCredito/cartao/compra";
            var resultadoCredito = await client.PostAsync(urlRequisicaoCredito, urlEncodedCredito);

            if(resultadoCredito.StatusCode != HttpStatusCode.OK)
                return BadRequest("Erro ao efetuar transação de credito.");


            return Ok();
        }

        [HttpPost]
        [Route("{id}")]
        public ActionResult<IEnumerable<string>> CadastraItemPedido(int id, [FromBody]Pedido item)
        {
            pedido.Add(item);
            return Ok();
        }


        [HttpPut]
        [Route("{id}")]
        public ActionResult<IEnumerable<string>> AlteraItemPedido(int id, [FromBody]Pedido item)
        {
            var dado = pedido.Where(a => a.Codigo == id).FirstOrDefault();
            if (dado == null)
                return NotFound("Pedido não encontrado");
            pedido.Remove(dado);
            pedido.Add(item);
            return Ok();
        }


        [HttpDelete]
        [Route("{id}")]
        public ActionResult<IEnumerable<string>> RemoveItemPedido(int id)
        {
            var dado = pedido.Where(a => a.Codigo == id).FirstOrDefault();
            if (dado == null)
                return NotFound("Pedido não encontrado");
            pedido.Remove(dado);
            return Ok();
        }


        [HttpGet]
        [Route("{id}")]
        public ActionResult<IEnumerable<string>> BuscarPedido(int id)
        {
            var item = pedido.Where(a => a.Codigo == id).FirstOrDefault();
            if (item == null)
                return NotFound("Pedido não encontrado");

            return Ok(item);
        }

        [HttpGet]
        [Route("{id}/itens")]
        public ActionResult<IEnumerable<string>> BuscarItensPedidos(int id)
        {

            var item = pedido.Where(a => a.Codigo == id).FirstOrDefault();
            if (item == null)
                return NotFound("Pedido não encontrado");
            
            return Ok(item.carrinho.Livros.ToList());
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