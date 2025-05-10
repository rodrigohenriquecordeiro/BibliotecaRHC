using BibliotecaRHC.API.Domain;
using BibliotecaRHC.API.Models;
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
        var novoLivro = await _service.AdicionarLivroAsync(livro);
        return CreatedAtAction(nameof(GetById), new { id = novoLivro.Id }, novoLivro);
    }

    [Authorize]
    [HttpGet("obter-livros")]
    public async Task<IActionResult> GetTodos() => Ok(await _service.ObterTodosOsLivros());

    [Authorize]
    [HttpGet("obter-livro-por-id/{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var livro = await _service.ObterLivroPorId(id);
        if (livro == null) return NotFound();
        return Ok(livro);
    }

    [Authorize]
    [HttpPut("atualizar-livro/{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] Livro livro)
    {
        if (id != livro.Id)
            return BadRequest("ID do livro não corresponde ao ID fornecido na URL.");

        var atualizado = await _service.AtualizarLivro(livro);
        if (atualizado == null)
            return NotFound();

        return NoContent();
    }

    [Authorize]
    [HttpDelete("remover-livro/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var removido = await _service.RemoverLivro(id);
        if (removido == null) return NotFound();
        return NoContent();
    }

    [Authorize]
    [HttpGet("obter-codigo-proximo-livro")]
    public async Task<IActionResult> GetCodigoProximoLivro() => Ok(await _service.GeraCodigoProximoLivro());
}
