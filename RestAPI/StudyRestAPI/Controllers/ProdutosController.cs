using Microsoft.AspNetCore.Mvc;
using StudyRestAPI.Data;
using StudyRestAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudyRestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProdutosController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var produtos = _context.Produtos.ToList();
            return Ok(produtos);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var produto = _context.Produtos.FirstOrDefault(p => p.Id == id);
            if(produto != null)
                return Ok(produto);

            return NotFound("");
        }

        [HttpPost]
        public IActionResult Post([FromBody] ProdutoTemp model)
        {
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

        public class ProdutoTemp
        {
            public string Nome { get; set; }
            public float Preco { get; set; }
        }
    }
}
