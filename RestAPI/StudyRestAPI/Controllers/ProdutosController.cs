using Microsoft.AspNetCore.Mvc;
using StudyRestAPI.Data;
using StudyRestAPI.Hateoas;
using StudyRestAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudyRestAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private Hateoas.Hateoas _hateoas;

        public ProdutosController(ApplicationDbContext context)
        {
            _context = context;
            _hateoas = new Hateoas.Hateoas("localhost:5001/api/v1/Produtos");
            _hateoas.AddAction("GET_INFO", "GET");
            _hateoas.AddAction("DELETE_PRODUCT", "DELETE");
            _hateoas.AddAction("EDIT_PRODUCT", "PATCH");
        }

        [HttpGet]
        public IActionResult Get()
        {
            var produtos = _context.Produtos.ToList();

            var produtosContainer = new List<ProdutoContainer>();

            foreach (var produto in produtos)
            {
                var produtoHateoas = new ProdutoContainer
                {
                    Produto = produto,
                    Links = _hateoas.GetActions(produto.Id.ToString())
                };
                produtosContainer.Add(produtoHateoas);
            }

            return Ok(produtosContainer);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var produto = _context.Produtos.FirstOrDefault(p => p.Id == id);
            if(produto != null)
            {
                var produtoHateoas = new ProdutoContainer
                {
                    Produto = produto,
                    Links = _hateoas.GetActions(produto.Id.ToString())
                };
                return Ok(produtoHateoas);
            }

            return NotFound("");
        }

        [HttpPost]
        public IActionResult Post([FromBody] ProdutoTemp model)
        {
            if (model.Preco <= 0)
                return BadRequest(new { msg = "O preço do produto não pode ser menor ou igual a 0." });
            
            if (model.Nome.Length <= 1)
                return BadRequest(new { msg = "O nome do produto precisa ter mais de um caracter." });

            var produto = new Produto();
            produto.Nome = model.Nome;
            produto.Preco = model.Preco;
            _context.Produtos.Add(produto);
            _context.SaveChanges();

            return Created(nameof(GetById), new { id = produto.Id });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var produto = _context.Produtos.FirstOrDefault(p => p.Id == id);
            if (produto != null)
            {
                _context.Produtos.Remove(produto);
                _context.SaveChanges();
                return Ok(produto);
            }                

            return NotFound("");
        }

        [HttpPatch("{id}")]
        public IActionResult Patch(int id, [FromBody] ProdutoTemp model)
        {
            if(id <= 0)
                return BadRequest(new { msg = "O id do produto é inválido!" });

            var produto = _context.Produtos.FirstOrDefault(x => x.Id == id);
            
            if(produto == null)
                return BadRequest(new { msg = "O id do produto é inválido!" });

            produto.Nome = !string.IsNullOrEmpty(model.Nome) ? model.Nome : produto.Nome;
            produto.Preco = model.Preco > 0 ? model.Preco : produto.Preco;

            _context.SaveChanges();

            return Ok(produto);
        }

        public class ProdutoTemp
        {
            public string Nome { get; set; }
            public float Preco { get; set; }
        }

        public class ProdutoContainer
        {
            public Produto Produto { get; set; }
            public Link[] Links { get; set; }
        }
    }
}
