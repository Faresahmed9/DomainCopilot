
using DomainCopilot.Application.Tenant;
using DomainCopilot.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace DomainCopilot.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly CreateDocumentUseCase _createDocumentUseCase;
    private readonly ProcessDocumentUseCase _processDocumentUseCase;
    private readonly ITenantContext _tenantContext;

    public DocumentsController(
        CreateDocumentUseCase createDocumentUseCase,
        ProcessDocumentUseCase processDocumentUseCase,
        ITenantContext tenantContext)
    {
        _createDocumentUseCase = createDocumentUseCase;
        _processDocumentUseCase = processDocumentUseCase;
        _tenantContext = tenantContext;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(
        IFormFile file,
        [FromForm] string policyNumber,
        [FromForm] int policyVersion, 
        [FromForm] DateTime effectiveFrom,
        [FromForm] DateTime effectiveTo)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest("File is required.");
        }

        if (string.IsNullOrWhiteSpace(policyNumber))
        {
            return BadRequest("Policy number is required.");
        }

        if (policyVersion <= 0)
        {
            return BadRequest("Policy version must be greater than zero.");
        }
        if (effectiveTo <= effectiveFrom)
        {
            return BadRequest("EffectiveTo must be after EffectiveFrom.");
        }
        //var tenantId = Guid.Parse(
        //    "11111111-1111-1111-1111-111111111111");

        await using var stream = file.OpenReadStream();

        var document = await _createDocumentUseCase.ExecuteAsync(
            _tenantContext.TenantId,
            policyNumber,
            policyVersion,
            effectiveFrom,
            effectiveTo,
            file.FileName,
            file.ContentType,
            stream);

        return Ok(document);
    }

    [HttpPost("{documentId:guid}/process")]
    public async Task<IActionResult> Process(Guid documentId)
    {
        var chunkIds = await _processDocumentUseCase
            .ExecuteAsync(
                documentId,
                _tenantContext.TenantId);

        if (chunkIds is null)
        {
            return NotFound();
        }

        return Ok(new
        {
            documentId,
            chunkIds
        });
    }
}

