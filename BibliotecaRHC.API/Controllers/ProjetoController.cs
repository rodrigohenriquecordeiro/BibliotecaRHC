using BibliotecaRHC.API.Models;
using BibliotecaRHC.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaRHC.API.Controllers;

[ApiController]
[Route("api/projetos")]
public class ProjetoController : Controller
{
    private readonly IProjetoService _service;

    public ProjetoController(IProjetoService service)
    {
        _service = service;
    }

    [Authorize]
    [HttpPost("adicionar-projeto")]
    public async Task<IActionResult> Post([FromBody] Projeto projeto)
    {
        var ProjetoAdicionado = await _service.AdicionarProjeto(projeto);
        return CreatedAtAction(nameof(GetById), new { id = ProjetoAdicionado.Id }, ProjetoAdicionado);
    }

    [Authorize]
    [HttpGet("obter-projetos")]
    public async Task<IActionResult> GetTodos() => Ok(await _service.ObterTodosOsProjetos());

    [Authorize]
    [HttpGet("obter-projeto-por-id/{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var projeto = await _service.ObterProjetoPorId(id);
        if (projeto == null) return NotFound();
        return Ok(projeto);
    }

    [Authorize]
    [HttpPut("atualizar-projeto/{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] Projeto projeto)
    {
        if (id != projeto.Id)
            return BadRequest("ID do Projeto não corresponde ao ID fornecido na URL.");

        var atualizado = await _service.AtualizarProjeto(projeto);
        if (atualizado == null)
            return NotFound();

        return NoContent();
    }

    [Authorize]
    [HttpDelete("remover-projeto/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var removido = await _service.RemoverProjeto(id);
        if (removido == null) return NotFound();
        return NoContent();
    }
}
