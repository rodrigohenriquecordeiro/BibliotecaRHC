using BibliotecaRHC.API.Domain;
using BibliotecaRHC.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaRHC.API.Controllers;

[ApiController]
[Route("api/frases")]
public class FrasesInesqueciveisController : ControllerBase
{
    private readonly IFrasesInesqueciveisService _service;

    public FrasesInesqueciveisController(IFrasesInesqueciveisService service)
    {
        _service = service;
    }

    [Authorize]
    [HttpPost("adicionar-frase")]
    public async Task<IActionResult> Post([FromBody] FrasesInesqueciveis frase)
    {
        var novaFrase = await _service.AdicionarFrase(frase);
        return CreatedAtAction(nameof(GetById), new { id = novaFrase.Id }, novaFrase);
    }

    [Authorize]
    [HttpGet("obter-frases")]
    public async Task<IActionResult> GetTodos() => Ok(await _service.ObterTodasAsFrases());

    [Authorize]
    [HttpGet("obter-frase-por-id/{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var livro = await _service.ObterFrasePorId(id);
        if (livro == null) return NotFound();
        return Ok(livro);
    }

    [Authorize]
    [HttpPut("atualizar-frase/{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] FrasesInesqueciveis frase)
    {
        if (id != frase.Id)
            return BadRequest("ID da frase não corresponde ao ID fornecido na URL.");

        var atualizado = await _service.AtualizarFrase(frase);
        if (atualizado == null)
            return NotFound();

        return NoContent();
    }

    [Authorize]
    [HttpDelete("remover-frase/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var removido = await _service.RemoverFrase(id);
        if (removido == null) return NotFound();
        return NoContent();
    }

    [Authorize]
    [HttpGet("obter-frase-aleatoria")]
    public async Task<IActionResult> GetFraseAleatoria()
    {
        var frase = await _service.ObterFraseAleatoria();
        if (frase == null) return NotFound();
        return Ok(frase);
    }
}
