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
    [Route("v1/api/livros")]
    [ApiController]
    public class LivrosController : ControllerBase
    {

        List<Livro> livro = new List<Livro>
        {
           new Livro { Codigo =1, Autores = new string[]  { "Autor A", "Autor B" },             Ano = 2000, QtdPagina = 150, Titulo  = "Titulo 1", Tema  = "tema 1"},
           new Livro { Codigo =2, Autores = new string[]  { "Autor C", },                       Ano = 2008, QtdPagina = 200, Titulo  = "Titulo 2", Tema  = "tema 2"},
           new Livro { Codigo =3, Autores = new string[]  { "Autor B", },                       Ano = 2006, QtdPagina = 200, Titulo  = "Titulo 3", Tema  = "tema 3"},
                                                                                                                                                                   
           new Livro { Codigo =4, Autores = new string[]  { "Autor D", "Autor A" },             Ano = 2005, QtdPagina = 110,  Titulo = "Titulo 4", Tema  = "tema 4"},
           new Livro { Codigo =5, Autores = new string[]  { "Autor E", "Autor D" },             Ano = 1970, QtdPagina = 220,  Titulo = "Titulo 5", Tema  = "tema 5"},
           new Livro { Codigo =6, Autores = new string[]  { "Autor F", "Autor D" },             Ano = 1989, QtdPagina = 330,  Titulo = "Titulo 6", Tema  = "tema 6"},

           new Livro { Codigo =7, Autores = new string[]  { "Autor G", "Autor D" },             Ano = 1988, QtdPagina = 440,  Titulo = "Titulo 7", Tema  = "tema 7"},
           new Livro { Codigo =8, Autores = new string[]  { "Autor H" },                        Ano = 1999, QtdPagina = 550,  Titulo = "Titulo 8", Tema  = "tema 8"},
           new Livro { Codigo =9, Autores = new string[]  { "Autor I", "Autor A" },             Ano = 2008, QtdPagina = 50,   Titulo = "Titulo 9", Tema  = "tema 9"},

           new Livro { Codigo =10, Autores = new string[] { "Autor J" },                        Ano = 2009, QtdPagina = 60,   Titulo = "Titulo 10", Tema = "tema 10"},
           new Livro { Codigo =11, Autores = new string[] { "Autor K", "Autor A", "Autor B" },  Ano = 2010, QtdPagina = 70,   Titulo = "Titulo 11", Tema = "tema 11"},
           new Livro { Codigo =12, Autores = new string[] { "Autor L", "Autor C" },             Ano = 2019, QtdPagina = 80,   Titulo = "Titulo 12", Tema = "tema 12"},

           new Livro { Codigo =13, Autores = new string[] { "Autor M", "Autor Z" },             Ano = 2019, QtdPagina = 80,   Titulo = "Titulo 13", Tema = "tema 13"},
           new Livro { Codigo =14, Autores = new string[] { "Autor N", "Autor Y" },             Ano = 2019, QtdPagina = 80,   Titulo = "Titulo 14", Tema = "tema 14"},
           new Livro { Codigo =15, Autores = new string[] { "Autor O", "Autor X" },             Ano = 2019, QtdPagina = 80,   Titulo = "Titulo 15", Tema = "tema 15"},

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
        public ActionResult<IEnumerable<string>> BuscarTodosItens(int pagina)
        {
            dynamic expando = new ExpandoObject();
            expando.TotalItens = livro.Count;
            expando.IntervaloResultado = livro.Skip( (pagina == 0 ? pagina : pagina -1) * 5).Take(5);
            expando.PaginaInformada = pagina;
            return Ok(expando);
        }
        [HttpGet]
        [Route("itens/{id}")]
        public ActionResult<IEnumerable<string>> BuscarItem(int id)
        {
            return Ok(livro.Where(a => a.Codigo == id).FirstOrDefault());
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