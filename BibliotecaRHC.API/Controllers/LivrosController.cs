using BibliotecaRHC.API.Models;
using BibliotecaRHC.API.Services;
using Microsoft.AspNetCore.Authorization;
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

    [Authorize]
    [HttpPost("adicionar-livro")]
    public async Task<IActionResult> Post([FromBody] Livro livro)
    {
        await _service.AdicionarLivro(livro);
        return CreatedAtAction(nameof(Get), new { id = livro.Id }, livro);
    }

    [Authorize]
    [HttpGet("obter-livros")]
    public async Task<IActionResult> Get() => Ok(await _service.ObterLivros());

    [Authorize]
    [HttpGet("obter-livro-por-id/{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var produto = await _service.ObterLivroPorId(id);
        if (produto == null) return NotFound();
        return Ok(produto);
    }

    [Authorize]
    [HttpPut("atualizar-livro/{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] Livro livro)
    {
        if (id != livro.Id) return BadRequest("ID do livro não corresponde ao ID fornecido na URL.");

        var existente = await _service.ObterLivroPorId(id);
        if (existente == null) return NotFound();

        await _service.AtualizarLivro(livro);
        return NoContent();
    }

    [Authorize]
    [HttpDelete("remover-livro/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.RemoverLivro(id);
        return NoContent();
    }

    [Authorize]
    [HttpGet("obter-codigo-proximo-livro")]
    public async Task<IActionResult> GetCodigoProximoLivro() => Ok(await _service.GeraCodigoProximoLivro());
}
