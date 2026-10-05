using DomainCopilot.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace DomainCopilot.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmbeddingsController : ControllerBase
{
    private readonly GenerateEmbeddingUseCase _generateEmbeddingUseCase;

    public EmbeddingsController(
        GenerateEmbeddingUseCase generateEmbeddingUseCase)
    {
        _generateEmbeddingUseCase = generateEmbeddingUseCase;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> Generate([FromBody] string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return BadRequest("Text is required.");
        }

        var embedding =
            await _generateEmbeddingUseCase.ExecuteAsync(text);

        return Ok(new
        {
            text,
            dimensions = embedding.Count,
            embedding
        });
    }
}