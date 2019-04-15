using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Livraria.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Livraria.Controllers
{
    [Route("api/livros")]
    [ApiController]
    public class LivrosController : ControllerBase
    {

        List<Livro> livro = new List<Livro>
        {
           new Livro { codigo =1, Autores = new string[] { "Autor A", "Autor B" }, Ano = 2000, QtdPagina = 150, Titulo = "Titulo 1", Tema = "tema 1"},
           new Livro { codigo =1, Autores = new string[] { "Autor C", },           Ano = 2008, QtdPagina = 200, Titulo = "Titulo 2", Tema = "tema 2"},
           new Livro { codigo =1, Autores = new string[] { "Autor B", },           Ano = 2006, QtdPagina = 200, Titulo = "Titulo 3", Tema = "tema 3"},
           new Livro { codigo =1, Autores = new string[] { "Autor C", "Autor A" }, Ano = 2000, QtdPagina = 10,  Titulo = "Titulo 4", Tema = "tema 4"},

        };

        
        [HttpPost]
        [Route("itens")]
        public ActionResult<IEnumerable<string>> CadastraItem([FromBody] string livroJson)
        {
            return Ok();
        }
        [HttpPost]
        [Route("itens/{id}")]
        public ActionResult<IEnumerable<string>> CadastraComentarioItem([FromBody] string comentario)
        {
            return Ok();
        }




        
        [HttpPut]
        [Route("itens/{id}")]
        public ActionResult<IEnumerable<string>> AlteraItem(int id, [FromBody] string livroJson)
        {
            return Ok();
        }



        
        [HttpGet]
        [Route("itens")]
        public ActionResult<IEnumerable<string>> BuscarTodosItens()
        {

            return Ok(livro.ToList());
        }
        [HttpGet]
        [Route("itens/{id}")]
        public ActionResult<IEnumerable<string>> BuscarItem(int id)
        {
            return Ok(livro.Where(a => a.codigo == id).FirstOrDefault());
        }
        [HttpGet]
        [Route("itens/{autor}")]
        public ActionResult<IEnumerable<string>> BuscarItensPorAutor(string autor)
        {
            return Ok(livro.Where(a => a.Autores.Contains(autor)).ToList());
        }
        [HttpGet]
        [Route("itens/{autor}/tema")]
        public ActionResult<IEnumerable<string>> BuscarTemaPorAutor(string autor)
        {
            return Ok(livro.Where(a => a.Autores.Contains(autor)).Select(a=> a.Tema).ToList());
        }



    }
}