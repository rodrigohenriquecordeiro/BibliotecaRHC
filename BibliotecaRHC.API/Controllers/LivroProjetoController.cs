using BibliotecaRHC.API.Models;
using BibliotecaRHC.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaRHC.API.Controllers;

[ApiController]
[Route("api/livro-projeto")]
public class LivroProjetoController : Controller
{
    private readonly ILivroProjetoService _service;

    public LivroProjetoController(ILivroProjetoService service)
    {
        _service = service;
    }

    [Authorize]
    [HttpPost("adicionar-livro-projeto")]
    public async Task<IActionResult> Post([FromBody] LivroProjeto livroProjeto)
    {
        var livroProjetoAdicionado = await _service.AdicionarLivroNoProjeto(livroProjeto);
        return CreatedAtAction(nameof(GetById), new { id = livroProjetoAdicionado.Id }, livroProjetoAdicionado);
    }

    [Authorize]
    [HttpGet("obter-livro-projeto")]
    public async Task<IActionResult> GetTodos() => Ok(await _service.ObterTodosOsLivrosDoProjeto());

    [Authorize]
    [HttpGet("obter-livro-projeto-por-id/{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var livro = await _service.ObterLivroDoProjetoPorId(id);
        if (livro == null) return NotFound();
        return Ok(livro);
    }

    [Authorize]
    [HttpPut("atualizar-livro-projeto/{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] LivroProjeto livroProjeto)
    {
        if (id != livroProjeto.Id)
            return BadRequest("ID do Livro no Projeto não corresponde ao ID fornecido na URL.");

        var atualizado = await _service.AtualizarLivroNoProjeto(livroProjeto);
        if (atualizado == null)
            return NotFound();

        return NoContent();
    }

    [Authorize]
    [HttpDelete("remover-livro-projeto/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var removido = await _service.RemoverLivroDoProjeto(id);
        if (removido == null) return NotFound();
        return NoContent();
    }
}
