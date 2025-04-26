using BibliotecaRHC.Models;
using BibliotecaRHC.Services;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaRHC.API.Controllers;

[ApiController]
[Route("api/livros")]
public class LivrosController : ControllerBase
{
    private readonly ILivroService _service;

    public LivrosController(ILivroService service)
    {
        _service = service;
    }

    [HttpPost("adicionar-livro")]
    public async Task<IActionResult> Post([FromBody] Livro livro)
    {
        await _service.AdicionarLivro(livro);
        return CreatedAtAction(nameof(Get), new { id = livro.Id }, livro);
    }

    [HttpGet("obter-livros")]
    public async Task<IActionResult> Get() => Ok(await _service.ObterLivros());

    [HttpGet("obter-livro-por-id/{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var produto = await _service.ObterLivroPorId(id);
        if (produto == null) return NotFound();
        return Ok(produto);
    }

    [HttpPut("atualizar-livro/{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] Livro livro)
    {
        if (id != livro.Id) return BadRequest("ID do livro não corresponde ao ID fornecido na URL.");

        var existente = await _service.ObterLivroPorId(id);
        if (existente == null) return NotFound();

        await _service.AtualizarLivro(livro);
        return NoContent();
    }

    [HttpDelete("remover-livro/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.RemoverLivro(id);
        return NoContent();
    }

    [HttpGet("obter-codigo-ultimo-livro")]
    public async Task<IActionResult> GetUltimoCodigo()
    {
        int ultimoCodigo = await _service.ObterCodigoUltimoLivro();
        if (ultimoCodigo == 0) return NotFound();
        return Ok(ultimoCodigo);
    }
}
