using BlazorHero.CleanArchitecture.Application.Features.Documents.Commands;
using BlazorHero.CleanArchitecture.Application.Features.Documents.Queries;
using BlazorHero.CleanArchitecture.Contracts.Documents;
using BlazorHero.CleanArchitecture.Shared.Constants.Permission;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorHero.CleanArchitecture.Server.Controllers.Utilities.Misc;

[Route("api/[controller]")]
[ApiController]
public class DocumentsController : BaseApiController<DocumentsController>
{
    /// <summary>
    ///     Get All Documents
    /// </summary>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="searchString"></param>
    /// <returns>Status 200 OK</returns>
    [Authorize(Policy = Permissions.Documents.View)]
    [HttpGet]
    public async Task<IActionResult> GetAll(int pageNumber, int pageSize, string searchString)
    {
        PaginatedResult<GetAllDocumentsResponse> docs =
            await Mediator.Send(new GetAllDocumentsQuery(pageNumber, pageSize, searchString));
        return Ok(docs);
    }

    /// <summary>
    ///     Get Document By Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Status 200 Ok</returns>
    [Authorize(Policy = Permissions.Documents.View)]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        Result<GetDocumentByIdResponse> document = await Mediator.Send(new GetDocumentByIdQuery(id));
        return Ok(document);
    }

    /// <summary>
    ///     Add/Edit Document
    /// </summary>
    /// <param name="command"></param>
    /// <returns>Status 200 OK</returns>
    [Authorize(Policy = Permissions.Documents.Create)]
    [HttpPost]
    public async Task<IActionResult> Post(AddEditDocumentCommand command) => Ok(await Mediator.Send(command));

    /// <summary>
    ///     Delete a Document
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Status 200 OK</returns>
    [Authorize(Policy = Permissions.Documents.Delete)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) => Ok(await Mediator.Send(new DeleteDocumentCommand(id)));
}
