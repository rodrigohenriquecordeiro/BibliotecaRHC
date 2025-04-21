using BibliotecaRHC.Models;
using BibliotecaRHC.Models.Entities;
using BibliotecaRHC.Services;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaRHC.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LivrosController : ControllerBase
{
    private readonly ILivroService _service;

    public LivrosController(ILivroService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult Get() => Ok(_service.ObterLivros());

    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        var produto = _service.ObterLivroPorId(id);
        if (produto == null) return NotFound();
        return Ok(produto);
    }

    [HttpPost]
    public IActionResult Post([FromBody] Livro livro)
    {
        _service.AdicionarLivro(livro);
        return CreatedAtAction(nameof(Get), new { id = livro.Id }, livro);
    }

    [HttpPut("{id}")]
    public IActionResult Put(int id, [FromBody] Livro livro)
    {
        if (id != livro.Id) return BadRequest("ID do livro não corresponde ao ID fornecido na URL.");

        var existente = _service.ObterLivroPorId(id);
        if (existente == null) return NotFound();

        _service.AtualizarLivro(livro);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _service.RemoverLivro(id);
        return NoContent();
    }
}
